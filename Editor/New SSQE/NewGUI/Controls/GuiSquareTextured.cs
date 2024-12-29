using SkiaSharp;

namespace New_SSQE.NewGUI.Controls
{
    internal class GuiSquareTextured : GuiSquare
    {
        public GuiSquareTextured(float x, float y, float w, float h, string texture, string? fileSource = null) : base(x, y, w, h, null, false)
        {
            if (fileSource != null && File.Exists(fileSource))
            {
                using FileStream fs = File.OpenRead(fileSource);
                textures = [new(texture, SKBitmap.Decode(fs))];
            }
            else
                textures = [new(texture)];

            textureIndex = 0;
        }
        public GuiSquareTextured(string texture, string? fileSource = null) : this(0, 0, 1920, 1080, texture, fileSource) { }

        public override float[] Draw()
        {
            textures[0].Draw(rect);
            return base.Draw();
        }
    }
}
