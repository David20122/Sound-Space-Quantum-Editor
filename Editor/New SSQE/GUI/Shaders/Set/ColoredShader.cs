namespace New_SSQE.GUI.Shaders.Set
{
    internal static class ColoredShader
    {
        public readonly static string Vertex = @"#version 330 core
layout (location = 0) in vec2 aPosition;
layout (location = 1) in vec4 aColor; // using all 4 components this time
layout (location = 2) in vec4 aOffset; // x, y, s, a

out vec4 vertexColor;

uniform mat4 Projection;

void main()
{
    gl_Position = Projection * vec4(aPosition * aOffset.z + aOffset.xy, 0.0f, 1.0f);
    vertexColor = vec4(aColor.xyz, aColor.w * aOffset.w);
}";

        public static string Fragment => MainShader.Fragment;
    }
}
