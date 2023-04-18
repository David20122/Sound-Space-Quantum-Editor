using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSQE_Player.Models
{
    internal class ObjModel
    {
        private readonly List<Face> faces = new();
        private readonly List<Vertex> vertices = new();
        private readonly List<Vector2> uvs = new();
        private readonly List<Vector3> normals = new();

        public ObjMaterial Material;

        private ObjModel() { }

        public static ObjModel FromFile(string file)
        {
            var lines = File.ReadAllLines(file);
            var model = new ObjModel();

            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                var start = line[..line.IndexOf(' ')];

                switch (start)
                {
                    case "mtllib":
                        var mtlFile = line[7..];

                        if (File.Exists(mtlFile))
                            model.Material = ObjMaterial.FromFile(mtlFile);

                        break;

                    case "v":
                        var positionsV = line[2..].Split(' ').Where(pos => float.TryParse(pos, out _)).Select(pos => float.Parse(pos)).ToArray();
                        var vertex = new Vertex(positionsV);

                        model.vertices.Add(vertex);

                        break;

                    case "vt":
                        var positionsVT = line[3..].Split(' ').Where(pos => float.TryParse(pos, out _)).Select(pos => float.Parse(pos)).ToArray();

                        if (positionsVT.Length >= 2)
                            model.uvs.Add((positionsVT[0], positionsVT[1]));

                        break;

                    case "vn":
                        var positionsVN = line[3..].Split(' ').Where(pos => float.TryParse(pos, out _)).Select(pos => float.Parse(pos)).ToArray();

                        if (positionsVN.Length == 3)
                            model.normals.Add((positionsVN[0], positionsVN[1], positionsVN[2]));

                        break;

                    case "f":
                        var faceArray = line[2..].Split(' ');

                        var indexArray = new List<long>();
                        var textureArray = new List<long>();
                        var normalArray = new List<long>();

                        foreach (var data in faceArray)
                        {
                            var dataSplit = data.Split('/');

                            if (dataSplit.Length >= 1 && long.TryParse(dataSplit[0], out var index))
                                indexArray.Add(index - 1);
                            if (dataSplit.Length >= 2 && long.TryParse(dataSplit[1], out var texture))
                                textureArray.Add(texture);
                            if (dataSplit.Length == 3 && long.TryParse(dataSplit[2], out var normal))
                                normalArray.Add(normal);

                            var face = new Face(indexArray.ToArray(), textureArray.ToArray(), normalArray.ToArray());

                            model.faces.Add(face);
                        }

                        break;
                }
            }

            return model;
        }

        public float[] GetVertices()
        {
            float[] verticesF = new float[faces.Count * 3 * 3];
            int k = 0;

            foreach (var face in faces)
            {
                for (var i = 0; i < face.Indices.Length; i++)
                {
                    var index = face.Indices[i];
                    var vertex = vertices[(int)index];

                    for (int j = 0; j < vertex.Positions.Length; j++)
                        verticesF[k++] = vertex.Positions[j];
                }
            }

            return verticesF;
        }
    }

    readonly struct Face
    {
        public readonly long[] Indices;
        public readonly long[] UVs;
        public readonly long[] Normals;

        public Face(long[] indices, long[] uvs, long[] normals)
        {
            Indices = indices;
            UVs = uvs;
            Normals = normals;
        }
    }

    struct Vertex
    {
        public float[] Positions;

        public Vertex(params float[] positions)
        {
            Positions = positions;
        }
    }
}
