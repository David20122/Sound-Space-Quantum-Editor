using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.IO;

namespace SSQE_Player.GUI
{
    class Shader
    {
        private static readonly List<Shader> Shaders = new List<Shader>();

        private int vsh;
        private int fsh;
        private int program;

        public readonly string ShaderName;

        private readonly Dictionary<string, int> Uniforms = new Dictionary<string, int>();

        public Shader(string shaderName, params string[] uniforms)
        {
            ShaderName = shaderName;

            Init();

            RegisterUniforms("transformationMatrix", "projectionMatrix", "viewMatrix");
            RegisterUniforms(uniforms);

            Shaders.Add(this);
        }

        private void Init()
        {
            LoadShader(ShaderName);

            program = GL.CreateProgram();

            GL.AttachShader(program, vsh);
            GL.AttachShader(program, fsh);

            BindAttribute(0, "position");

            GL.LinkProgram(program);
            GL.ValidateProgram(program);
        }

        private int GetUniformLocation(string uniform)
        {
            return Uniforms.TryGetValue(uniform, out var i) ? i : -1;
        }

        private void RegisterUniforms(params string[] uniforms)
        {
            Bind();

            foreach  (var uniform in uniforms)
            {
                if (Uniforms.ContainsKey(uniform))
                    continue;

                var location = GL.GetUniformLocation(program, uniform);

                if (location == -1)
                    continue;

                Uniforms.Add(uniform, location);
            }

            Unbind();
        }

        public void SetVector4(string uniform, Vector4 vec)
        {
            var location = GetUniformLocation(uniform);

            if (location != -1)
                GL.Uniform4(location, vec);
        }

        public void SetMatrix4(string uniform, Matrix4 mat)
        {
            var location = GetUniformLocation(uniform);

            if (location != -1)
                GL.UniformMatrix4(location, false, ref mat);
        }

        public static void SetProjectionMatrix(Matrix4 mat)
        {
            for (var i = 0; i < Shaders.Count; i++)
            {
                var shader = Shaders[i];

                shader.Bind();
                shader.SetMatrix4("projectionMatrix", mat);
                shader.Unbind();
            }
        }

        public static void SetViewMatrix(Matrix4 mat)
        {
            for (var i = 0; i < Shaders.Count; i++)
            {
                var shader = Shaders[i];

                shader.Bind();
                shader.SetMatrix4("viewMatrix", mat);
                shader.Unbind();
            }
        }

        private void BindAttribute(int attrib, string variable)
        {
            GL.BindAttribLocation(program, attrib, variable);
        }

        private void LoadShader(string shaderName)
        {
            string vshCode = File.ReadAllText($"assets/shaders/{shaderName}.vsh");
            string fshCode = File.ReadAllText($"assets/shaders/{shaderName}.fsh");

            vsh = GL.CreateShader(ShaderType.VertexShader);
            fsh = GL.CreateShader(ShaderType.FragmentShader);

            GL.ShaderSource(vsh, vshCode);
            GL.ShaderSource(fsh, fshCode);

            GL.CompileShader(vsh);
            GL.CompileShader(fsh);
        }

        public void Bind()
        {
            GL.UseProgram(program);
        }

        public void Unbind()
        {
            GL.UseProgram(0);
        }

        public void Destroy()
        {
            Unbind();

            GL.DetachShader(program, vsh);
            GL.DetachShader(program, fsh);

            GL.DeleteShader(vsh);
            GL.DeleteShader(fsh);

            GL.DeleteProgram(program);
        }

        public static void DestroyAll()
        {
            for (int i = 0; i < Shaders.Count; i++)
                Shaders[i].Destroy();
        }
    }
}
