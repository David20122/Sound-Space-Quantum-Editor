using OpenTK.Graphics.OpenGL;

namespace Sound_Space_Editor
{
    public class ModelRaw
    {
        public int VaoID { get; }
        public int[] BufferIDs { get; }

        public int VertexCount { get; protected set; }

        /*
        public ModelRaw(int vaoID, int valuesPerVertice, List<RawQuad> quads, params int[] bufferIDs)
        {
            VaoID = vaoID;
            BufferIDs = bufferIDs;

            foreach (RawQuad quad in quads)
                VertexCount += quad.vertices.Length / valuesPerVertice;
        }*/

        //buffer IDs must be in this order and step: 0;1;2;3; ...
        public ModelRaw(int vaoID, int vertexCount, params int[] bufferIDs)
        {
            VaoID = vaoID;
            BufferIDs = bufferIDs;

            VertexCount = vertexCount;
        }

        public bool HasLocalData() => true;

        public void Render(PrimitiveType pt) => GL.DrawArrays(pt, 0, VertexCount);

        public void Dispose()
        {
            ModelManager.DisposeOf(this);
        }
    }
}