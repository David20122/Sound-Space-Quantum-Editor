namespace New_SSQE.GUI.Shaders.Set
{
    internal static class TimelineShader
    {
        public readonly static string Vertex = @"#version 330 core
layout (location = 0) in vec2 aPosition;
layout (location = 1) in vec4 aColor; // only using one component but inputting vec4 for compatibility
layout (location = 2) in vec4 aOffset; // x, y, a, c
                                               
out vec4 vertexColor;

uniform mat4 Projection;
uniform vec4 Colors[11];
                                                
void main()
{
    vec4 color = Colors[int(aOffset.w)];

    gl_Position = Projection * vec4(aPosition + aOffset.xy, 0.0f, 1.0f);
    vertexColor = vec4(color.xyz, aColor.w * aOffset.z);
}";

        public static string Fragment => MainShader.Fragment;
    }
}
