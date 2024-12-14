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
            string[] lines = File.ReadAllLines(file);
            ObjModel model = new();

            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];
                string start = line[..line.IndexOf(' ')];

                switch (start)
                {
                    case "mtllib":
                        string mtlFile = line[7..];

                        if (File.Exists(mtlFile))
                            model.Material = ObjMaterial.FromFile(mtlFile);

                        break;

                    case "v":
                        float[] positionsV = line[2..].Split(' ').Where(pos => float.TryParse(pos, out _)).Select(pos => float.Parse(pos)).ToArray();
                        Vertex vertex = new(positionsV);

                        model.vertices.Add(vertex);

                        break;

                    case "vt":
                        float[] positionsVT = line[3..].Split(' ').Where(pos => float.TryParse(pos, out _)).Select(pos => float.Parse(pos)).ToArray();

                        if (positionsVT.Length >= 2)
                            model.uvs.Add((positionsVT[0], positionsVT[1]));

                        break;

                    case "vn":
                        float[] positionsVN = line[3..].Split(' ').Where(pos => float.TryParse(pos, out _)).Select(pos => float.Parse(pos)).ToArray();

                        if (positionsVN.Length == 3)
                            model.normals.Add((positionsVN[0], positionsVN[1], positionsVN[2]));

                        break;

                    case "f":
                        string[] faceArray = line[2..].Split(' ');

                        List<long> indexArray = new();
                        List<long> textureArray = new();
                        List<long> normalArray = new();

                        foreach (string data in faceArray)
                        {
                            string[] dataSplit = data.Split('/');

                            if (dataSplit.Length >= 1 && long.TryParse(dataSplit[0], out long index))
                                indexArray.Add(index - 1);
                            if (dataSplit.Length >= 2 && long.TryParse(dataSplit[1], out long texture))
                                textureArray.Add(texture);
                            if (dataSplit.Length == 3 && long.TryParse(dataSplit[2], out long normal))
                                normalArray.Add(normal);

                            Face face = new(indexArray.ToArray(), textureArray.ToArray(), normalArray.ToArray());

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

            foreach (Face face in faces)
            {
                for (int i = 0; i < face.Indices.Length; i++)
                {
                    long index = face.Indices[i];
                    Vertex vertex = vertices[(int)index];

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
