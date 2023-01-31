using System.Drawing;
using System.IO;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;

namespace Sound_Space_Editor.GUI
{
	class GuiWindowCreate : Gui
	{
        private readonly GuiLabel Label = new GuiLabel(832, 478, 256, 20, "Input Audio ID", 24, true);
        private readonly GuiTextbox IDBox = new GuiTextbox(832, 508, 256, 64, "", 24, false, true);

        private readonly GuiButton CreateButton = new GuiButton(832, 592, 256, 64, 0, "CREATE", 32, true);
        private readonly GuiButton ImportButton = new GuiButton(832, 666, 256, 64, 1, "IMPORT FILE", 32, true);
        private readonly GuiButton BackButton = new GuiButton(832, 740, 256, 64, 2, "BACK", 32, true);

        private int txid;
        private bool bgImg;

        public GuiWindowCreate() : base(0, 0, MainWindow.Instance.ClientSize.Width, MainWindow.Instance.ClientSize.Height)
        {
            buttons = new List<GuiButton> { CreateButton, ImportButton, BackButton };
            boxes = new List<GuiTextbox> { IDBox };
            labels = new List<GuiLabel> { Label };

            OnResize(MainWindow.Instance.ClientSize);

            if (File.Exists("background_menu.png"))
            {
                bgImg = true;

                using (Bitmap img = new Bitmap("background_menu.png"))
                    txid = TextureManager.GetOrRegister("menubg", img, true);
            }
        }

        public override void Render(float mousex, float mousey, float frametime)
        {
            GL.Color4(bgImg ? Color.FromArgb(255, 255, 255, 255) : Color.FromArgb(255, 30, 30, 30));

            if (bgImg)
                GLSpecial.TexturedQuad(rect, 0, 0, 1, 1, txid);
            else
                GLSpecial.Rect(rect);

            base.Render(mousex, mousey, frametime);
        }

        public override void OnResize(Size size)
        {
            rect = new RectangleF(0, 0, size.Width, size.Height);

            base.OnResize(size);
        }

        protected override void OnButtonClicked(int id)
        {
            var audioId = IDBox.text.Trim();
            var editor = MainWindow.Instance;

            switch (id)
            {
                case 0:
                    if (!string.IsNullOrWhiteSpace(audioId))
                        editor.LoadMap(audioId);

                    break;

                case 1:
                    if (editor.PromptImport(audioId, true))
                        editor.LoadMap(MainWindow.Instance.soundId);

                    break;

                case 2:
                    editor.SwitchWindow(new GuiWindowMenu());

                    break;
            }
        }
    }
}
