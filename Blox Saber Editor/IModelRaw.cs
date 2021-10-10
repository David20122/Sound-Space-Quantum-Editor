using OpenTK.Graphics.OpenGL;

namespace Sound_Space_Editor
{
    public interface IModelRaw
    {
        int VaoID { get; }
        int VertexCount { get; }
        int[] BufferIDs { get; }

        bool HasLocalData();

        void Render(PrimitiveType pt);
    }
}