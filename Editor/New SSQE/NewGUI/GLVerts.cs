namespace New_SSQE.NewGUI
{
    internal class GLVerts
    {
        public static float[] Texture(float x, float y, float w, float h, float tx = 0, float ty = 0, float tw = 1, float th = 1, float alpha = 1)
        {
            return new float[]
            {
                x, y, tx, ty, alpha,
                x + w, y, tx + tw, ty, alpha,
                x, y + h, tx, ty + th, alpha,

                x + w, y + h, tx + tw, ty + th, alpha,
                x, y + h, tx, ty + th, alpha,
                x + w, y, tx + tw, ty, alpha
            };
        }
    }
}
