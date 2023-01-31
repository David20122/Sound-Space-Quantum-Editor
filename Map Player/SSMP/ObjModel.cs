using System.Collections.Generic;
using System.Linq;
using OpenTK;
using System.IO;

namespace SSQE_Player
{
    class ObjModel
    {
        private readonly List<Face> faces = new List<Face>();
        private readonly List<Vertex> vertexes = new List<Vertex>();
        private readonly List<Vector2> uvs = new List<Vector2>();
        private readonly List<Vector3> normals = new List<Vector3>();

        public ObjMaterial Material;

        private ObjModel()
        {

        }

        public static ObjModel FromFile(string file)
        {
            var lines = File.ReadAllLines(file);

            var model = new ObjModel();

            for (var i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                var start = line.Substring(0, line.IndexOf(' '));

                switch (start)
                {
                    case "mtllib":
                        var mtlFile = line.Substring(7);

                        if (File.Exists(mtlFile))
                            model.Material = ObjMaterial.FromFile(mtlFile);

                        break;

                    case "v":
                        var positionsV = line.Substring(2).Split(' ').Where(pos => float.TryParse(pos, out _)).Select(pos => float.Parse(pos)).ToArray();
                        var vertex = new Vertex(positionsV);

                        model.vertexes.Add(vertex);

                        break;

                    case "vt":
                        var positionsVT = line.Substring(3).Split(' ').Where(pos => float.TryParse(pos, out _)).Select(pos => float.Parse(pos)).ToArray();

                        if (positionsVT.Length >= 2)
                            model.uvs.Add(new Vector2(positionsVT[0], positionsVT[1]));

                        break;

                    case "vn":
                        var positionsVN = line.Substring(3).Split(' ').Where(pos => float.TryParse(pos, out _)).Select(pos => float.Parse(pos)).ToArray();

                        if (positionsVN.Length == 3)
                            model.normals.Add(new Vector3(positionsVN[0], positionsVN[1], positionsVN[2]));

                        break;

                    case "f":
                        var faceArray = line.Substring(2).Split(' ');

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

        public float[] GetVertexes()
        {
            float[] vertexesF = new float[faces.Count * 3 * 3];
            int floatCount = 0;

            foreach (var face in faces)
            {
                for (var i = 0; i < face.Indices.Length; i++)
                {
                    var index = face.Indices[i];
                    var vertex = vertexes[(int)index];

                    for (var floatIndex = 0; floatIndex < vertex.Positions.Length; floatIndex++)
                        vertexesF[floatCount++] = vertex.Positions[floatIndex];
                }
            }

            return vertexesF;
        }
    }

    struct Face
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

        public Vertex(params float[] position)
        {
            Positions = position;
        }
    }
}
