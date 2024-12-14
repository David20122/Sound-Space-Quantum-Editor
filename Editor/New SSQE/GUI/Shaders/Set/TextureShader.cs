namespace New_SSQE.GUI.Shaders.Set
{
    internal static class TextureShader
    {
        public readonly static string Vertex = @"#version 330 core
layout (location = 0) in vec2 aPosition;
layout (location = 1) in vec2 aTexCoord;
layout (location = 2) in float aAlpha;

out vec2 texCoord;
out float alpha;

uniform mat4 Projection;
                                                
void main()
{
    gl_Position = Projection * vec4(aPosition, 0.0f, 1.0f);
    texCoord = aTexCoord;
    alpha = aAlpha;
}";

        public readonly static string Fragment = @"#version 330 core
out vec4 FragColor;
in vec2 texCoord;
in float alpha;

uniform sampler2D texture0;
                                                
void main()
{
    vec4 color = texture(texture0, texCoord);
    FragColor = vec4(color.xyz, color.w * alpha);
}";
    }
}
