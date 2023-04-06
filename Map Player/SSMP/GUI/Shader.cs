using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.IO;
using System;

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

        private static readonly string cursorVertShader = @"#version 330

                                                            in vec3 position;

                                                            out vec4 pass_color;

                                                            uniform mat4 transformationMatrix;
                                                            uniform mat4 projectionMatrix;
                                                            uniform mat4 viewMatrix;
                                                            uniform vec4 colorIn;

                                                            void main(void) {
	                                                            vec4 worldPos = transformationMatrix * vec4(position, 1.0);
	                                                            gl_Position = projectionMatrix * viewMatrix * worldPos;

	                                                            pass_color = colorIn;
                                                            }";

        private static readonly string cursorFragShader = @"#version 330

                                                            in vec4 pass_color;

                                                            out vec4 out_Color;

                                                            void main(void){
	                                                            out_Color = pass_color;
                                                            }";

        private static readonly string noteVertShader = @"#version 330

                                                          in vec3 position;

                                                          out vec4 pass_color;

                                                          uniform mat4 transformationMatrix;
                                                          uniform mat4 projectionMatrix;
                                                          uniform mat4 viewMatrix;
                                                          uniform vec4 colorIn;

                                                          void main(void) {
	                                                          vec4 worldPos = transformationMatrix * vec4(position, 1.0);
	                                                          gl_Position = projectionMatrix * viewMatrix * worldPos;

	                                                          pass_color = colorIn;
                                                          }";

        private static readonly string noteFragShader = @"#version 330

                                                          in vec4 pass_color;

                                                          out vec4 out_Color;

                                                          void main(void){
	                                                          out_Color = pass_color;
                                                          }";

        private static readonly Dictionary<string, Tuple<string, string>> SourceLookup = new Dictionary<string, Tuple<string, string>>
        {
            {"cursor", new Tuple<string, string>(cursorVertShader, cursorFragShader) },
            {"note", new Tuple<string, string>(noteVertShader, noteFragShader) }
        };

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
            string vshCode = SourceLookup[shaderName].Item1;
            string fshCode = SourceLookup[shaderName].Item2;

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
