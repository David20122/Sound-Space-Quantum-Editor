using OpenTK.Mathematics;

namespace SSQE_Player.GUI
{
    internal class Camera
    {
        private Vector2 rotation = (0, MathHelper.Pi);

        private Matrix4 projection;
        private Matrix4 view;

        private Vector3 lookVector;
        private Vector3 cameraPos;

        private readonly float rad88 = MathHelper.DegreesToRadians(88);
        private readonly float radFoV = MathHelper.DegreesToRadians(Settings.settings["fov"]);

        private readonly string camMode = Settings.settings["cameraMode"].Current;
        private readonly bool lockCursor = Settings.settings["lockCursor"];
        private readonly float sensitivity = Settings.settings["sensitivity"];
        private readonly float parallax = Settings.settings["parallax"];

        public Vector2 LockedPos = (0, 0);

        public void Update(float mousex, float mousey)
        {
            Vector3 camOffset = new();

            if (camMode == "spin")
                rotation = (MathHelper.Clamp(rotation.X + mousey / 1000f * sensitivity, -rad88, rad88), rotation.Y + mousex / 1000f * sensitivity);
            else
            {
                LockedPos -= (mousex / 250f * sensitivity, mousey / 250f * sensitivity);

                var cursorSize = MainWindow.CursorSize;

                var xf = MathHelper.Clamp(LockedPos.X, -1.5f + cursorSize.X / 2f, 1.5f - cursorSize.X / 2f);
                var yf = MathHelper.Clamp(LockedPos.Y, -1.5f + cursorSize.Y / 2f, 1.5f - cursorSize.Y / 2f);

                if (lockCursor)
                    LockedPos = (xf, yf);

                if (camMode == "half lock")
                    camOffset = (xf * parallax / 5f, yf * parallax / 5f, 0);
            }

            var rot = Matrix4.CreateRotationY(rotation.Y) * Matrix4.CreateRotationX(rotation.X);

            lookVector = (rot * -Vector4.UnitZ).Xyz;
            cameraPos = -Vector3.UnitZ * 3.5f + lookVector * (1.25f, 1.25f, 0) + camOffset;

            view = Matrix4.CreateTranslation(-cameraPos) * rot;

            Shader.SetProjection(projection);
            Shader.SetView(view);
            Shader.SetProjView(projection, view);

            // update cursor
            {
                var cursorSize = MainWindow.CursorSize;

                Vector3 pos;

                if (camMode == "spin")
                    pos = cameraPos + new Vector3(lookVector.X, lookVector.Y, -Math.Abs(lookVector.Z)) * (float)Math.Abs(Math.Abs(cameraPos.Z + 0.15f) / lookVector.Z);
                else
                    pos = (LockedPos.X, LockedPos.Y, 0);

                var xf = MathHelper.Clamp(pos.X, -1.5f + cursorSize.X / 2f, 1.5f - cursorSize.X / 2f);
                var yf = MathHelper.Clamp(pos.Y, -1.5f + cursorSize.Y / 2f, 1.5f - cursorSize.Y / 2f);

                MainWindow.Instance.CursorPos = (xf, yf, 0);
            }
        }

        public void CalculateProjection()
        {
            projection = Matrix4.CreatePerspectiveFieldOfView(radFoV, MainWindow.Instance.Size.X / (float)MainWindow.Instance.Size.Y, 0.1f, 1000);
        }
    }
}
