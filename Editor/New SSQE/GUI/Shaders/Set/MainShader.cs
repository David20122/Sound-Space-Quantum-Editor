namespace New_SSQE.GUI.Shaders.Set
{
    internal static class MainShader
    {
        public readonly static string Vertex = @"#version 330 core
layout (location = 0) in vec2 aPosition;
layout (location = 1) in vec4 aColor;
out vec4 vertexColor;

uniform mat4 Projection;
                                                
void main()
{
    gl_Position = Projection * vec4(aPosition, 0.0f, 1.0f);
    vertexColor = aColor;
}";

        public readonly static string Fragment = @"#version 330 core
out vec4 FragColor;
in vec4 vertexColor;
                                                
void main()
{
    FragColor = vertexColor;
}";
    }
}
