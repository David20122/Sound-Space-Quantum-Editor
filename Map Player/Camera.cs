using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace Map_Player
{
    class Camera
    {
        public Vector3 position;
        public Vector3 target;
        public Matrix4 matrix;
        private Vector3 direction;
        private Vector3 up;
        private Vector3 right;
        public float pitch;
        public float yaw;
        private Player player;

        public Camera(Player playr)
        {
            player = playr;
            position = new Vector3(0, 0, 0);
            target = Vector3.Zero;
            direction = Vector3.Normalize(position - target);
            right = Vector3.Normalize(Vector3.Cross(Vector3.UnitY, direction));
            up = Vector3.Cross(direction, right);
            matrix = Matrix4.LookAt(position, position + target, up);
        }
        public void Update()
        {
            target.X = (float)Math.Cos(MathHelper.DegreesToRadians(pitch)) * (float)Math.Cos(MathHelper.DegreesToRadians(yaw));
            target.Y = (float)Math.Sin(MathHelper.DegreesToRadians(pitch));
            target.Z = (float)Math.Cos(MathHelper.DegreesToRadians(pitch)) * (float)Math.Sin(MathHelper.DegreesToRadians(yaw));
            target = Vector3.Normalize(target);

            direction = Vector3.Normalize(position - target);
            right = Vector3.Normalize(Vector3.Cross(Vector3.UnitY, direction));
            up = Vector3.Cross(direction, right);
            matrix = Matrix4.LookAt(position, position + target, up);
        }
        public void MouseUpdate(MouseMoveEventArgs e)
        {
            yaw += e.XDelta * 0.1f;
            if (pitch > 89.0f)
            {
                pitch = 89.0f;
            }
            else if (pitch < -89.0f)
            {
                pitch = -89.0f;
            }
            else
            {
                pitch -= e.YDelta * 0.1f;
            }
        }
    }
}
