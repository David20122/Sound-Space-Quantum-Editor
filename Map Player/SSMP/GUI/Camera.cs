using System;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace SSQE_Player.GUI
{
    class Camera
    {
        private Vector2 lastMouse;
        private Vector2 rotation = new Vector2(0, MathHelper.Pi);

        private Matrix4 ortho;
        private Matrix4 projection;
        private Matrix4 view;

        private Vector3 lookVector;
        private Vector3 cameraPos;

        private readonly float rad88 = MathHelper.DegreesToRadians(88);
        private readonly float rad70 = MathHelper.DegreesToRadians(70);

        private string camMode = Settings.settings["cameraMode"].Current;
        private bool lockCursor = Settings.settings["lockCursor"];

        public Vector2 lockedPos = new Vector2(0, 0);

        public void UpdateCursor()
        {
            var main = MainWindow.Instance;

            Vector3 pos;

            if (camMode == "spin")
                pos = cameraPos + new Vector3(lookVector.X, lookVector.Y, -Math.Abs(lookVector.Z)) * (float)Math.Abs(Math.Abs(cameraPos.Z + 0.15f) / lookVector.Z);
            else
                pos = new Vector3(lockedPos.X, lockedPos.Y, 0);

            var xf = MathHelper.Clamp(pos.X, -1.5f + main.cursorSize.X / 2f, 1.5f - main.cursorSize.X / 2f);
            var yf = MathHelper.Clamp(pos.Y, -1.5f + main.cursorSize.Y / 2f, 1.5f - main.cursorSize.Y / 2f);

            main.cursorPos = new Vector3(xf, yf, 0);
        }

        public void UpdateCamera(Point pos)
        {
            var current = new Vector2(pos.X, pos.Y);
            var cameraOffset = new Vector3();

            if (lastMouse != null)
            {
                var sensitivity = Settings.settings["sensitivity"];
                var diff = current - lastMouse;

                if (camMode == "spin")
                    rotation = new Vector2(MathHelper.Clamp(rotation.X + diff.Y / 1000f * sensitivity, -rad88, rad88), rotation.Y + diff.X / 1000f * sensitivity);
                else
                {
                    lockedPos -= new Vector2(diff.X / 250f * sensitivity, diff.Y / 250f * sensitivity);

                    var cursorSize = MainWindow.Instance.cursorSize;

                    var xf = MathHelper.Clamp(lockedPos.X, -1.5f + cursorSize.X / 2f, 1.5f - cursorSize.X / 2f);
                    var yf = MathHelper.Clamp(lockedPos.Y, -1.5f + cursorSize.Y / 2f, 1.5f - cursorSize.Y / 2f);

                    if (lockCursor)
                        lockedPos = new Vector2(xf, yf);

                    if (camMode == "half lock")
                    {
                        var parallax = Settings.settings["parallax"];

                        cameraOffset = new Vector3(xf * parallax / 5f, yf * parallax / 5f, 0);
                    }
                }

                var rot = Matrix4.CreateRotationY(rotation.Y) * Matrix4.CreateRotationX(rotation.X);

                lookVector = (rot * -Vector4.UnitZ).Xyz;
                cameraPos = -Vector3.UnitZ * 3.5f + lookVector * new Vector3(1.25f, 1.25f, 0) + cameraOffset;

                view = Matrix4.CreateTranslation(-cameraPos) * rot;
            }

            lastMouse = current;

            SetView();
            SetProjection();
            UpdateCursor();
        }

        public void CalculateProjection()
        {
            projection = Matrix4.CreatePerspectiveFieldOfView(rad70, MainWindow.Instance.Size.Width / (float)MainWindow.Instance.Size.Height, 0.1f, 1000);
            ortho = Matrix4.CreateOrthographicOffCenter(0, MainWindow.Instance.Width, MainWindow.Instance.Height, 0, 0, 1);
        }

        public void SetProjection()
        {
            var m = view * projection;

            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref m);

            Shader.SetProjectionMatrix(projection);
        }

        public void SetOrtho()
        {
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref ortho);
        }

        public void SetView()
        {
            Shader.SetViewMatrix(view);
        }
    }
}
