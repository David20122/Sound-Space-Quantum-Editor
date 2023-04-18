﻿using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace SSQE_Player.GUI
{
    internal class Shader
    {
        // Texture15: "main" font
        
        public static int Program;
        public static int ModelProgram;
        public static int FontTexProgram;

        private static readonly string vertexShader = @"#version 430 core
                                                          layout (location = 0) in vec3 position;
                                                          layout (location = 1) in vec4 color;
                                                          out vec4 pass_color;

                                                          layout (location = 2) uniform mat4 projectionMatrix;

                                                          void main()
                                                          {
                                                              gl_Position = projectionMatrix * vec4(position, 1.0);
                                                              pass_color = color;
                                                          }";

        private static readonly string fragmentShader = @"#version 430 core
                                                          in vec4 pass_color;
                                                          out vec4 out_Color;

                                                          void main()
                                                          {
                                                              out_Color = pass_color;
                                                          }";

        private static readonly string modelVertShader = @"#version 430 core
                                                          layout (location = 0) in vec3 position;
                                                          out vec4 pass_color;

                                                          layout (location = 1) uniform mat4 transformationMatrix;
                                                          layout (location = 2) uniform mat4 projectionMatrix;
                                                          layout (location = 3) uniform mat4 viewMatrix;
                                                          layout (location = 4) uniform vec4 colorIn;

                                                          void main()
                                                          {
	                                                          vec4 worldPos = transformationMatrix * vec4(position, 1.0);
	                                                          gl_Position = projectionMatrix * viewMatrix * worldPos;
	                                                          pass_color = colorIn;
                                                          }";

        private static readonly string modelFragShader = @"#version 430 core
                                                          in vec4 pass_color;
                                                          out vec4 out_Color;

                                                          void main()
                                                          {
	                                                          out_Color = pass_color;
                                                          }";

        private readonly static string fontTexVertShader = @"#version 430 core
                                               layout (location = 0) in vec2 aPosition;
                                               layout (location = 1) in vec4 aCharLayout;
                                               layout (location = 2) in float aCharAlpha;

                                               out vec4 texColor;
                                               out vec2 texCoord;

                                               uniform vec4 TexLookup[128];
                                               uniform vec2 CharSize;
                                               
                                               uniform vec4 TexColor;
                                               uniform vec2 ViewportSize;
                                                
                                               void main()
                                               {
                                                   vec4 texLocation = TexLookup[int(aCharLayout.w)];

                                                   float x = aCharLayout.x + aPosition.x * aCharLayout.z;
                                                   float y = aCharLayout.y + aPosition.y * aCharLayout.z;
                                                   float tx = texLocation.x + texLocation.z * (aPosition.x / CharSize.x);
                                                   float ty = texLocation.y + texLocation.w * (aPosition.y / CharSize.y);

                                                   float nx = x / ViewportSize.x * 2.0f - 1.0f;
                                                   float ny = y / ViewportSize.y * -2.0f + 1.0f;

                                                   gl_Position = vec4(nx, ny, 0.0f, 1.0f);

                                                   texColor = vec4(TexColor.x, TexColor.y, TexColor.z, TexColor.w * (1.0f - aCharAlpha));
                                                   texCoord = vec2(tx, ty);
                                               }";

        private readonly static string fontTexFragShader = @"#version 430 core
                                               out vec4 FragColor;

                                               in vec4 texColor;
                                               in vec2 texCoord;

                                               uniform sampler2D texture0;
                                               
                                               void main()
                                               {
                                                   FragColor = vec4(texColor.x, texColor.y, texColor.z, texture(texture0, texCoord).w * texColor.w);
                                               }";

        public static void Init()
        {
            Program = CompileShader(vertexShader, fragmentShader);
            ModelProgram = CompileShader(modelVertShader, modelFragShader);
            FontTexProgram = CompileShader(fontTexVertShader, fontTexFragShader);
        }

        private static int CompileShader(string vertShader, string fragShader)
        {
            int vs = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vs, vertShader);
            GL.CompileShader(vs);

            int fs = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fs, fragShader);
            GL.CompileShader(fs);

            int program = GL.CreateProgram();
            GL.AttachShader(program, vs);
            GL.AttachShader(program, fs);

            GL.LinkProgram(program);

            GL.DetachShader(program, vs);
            GL.DetachShader(program, fs);

            GL.DeleteShader(vs);
            GL.DeleteShader(fs);

            return program;
        }

        public static void SetViewport(int program, float w, float h)
        {
            GL.UseProgram(program);
            int location = GL.GetUniformLocation(program, "ViewportSize");
            GL.Uniform2(location, (float)w, (float)h);
        }

        public static void SetTransform(Matrix4 transform)
        {
            GL.UseProgram(ModelProgram);
            GL.UniformMatrix4(1, false, ref transform);
        }

        public static void SetProjection(Matrix4 projection)
        {
            GL.UseProgram(ModelProgram);
            GL.UniformMatrix4(2, false, ref projection);
        }

        public static void SetView(Matrix4 view)
        {
            GL.UseProgram(ModelProgram);
            GL.UniformMatrix4(3, false, ref view);
        }

        public static void SetColor(Vector4 vec)
        {
            GL.UseProgram(ModelProgram);
            GL.Uniform4(4, vec);
        }

        public static void SetProjView(Matrix4 projection, Matrix4 view)
        {
            GL.UseProgram(Program);
            var projview = view * projection;
            GL.UniformMatrix4(2, false, ref projview);
        }
    }
}
