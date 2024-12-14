namespace New_SSQE.GUI.Shaders.Set
{
    internal static class FontShader
    {
        public readonly static string Vertex = @"#version 330 core
layout (location = 0) in vec2 aPosition;
layout (location = 1) in vec4 aCharLayout;
layout (location = 2) in float aCharAlpha;

out vec4 texColor;
out vec2 texCoord;

uniform vec4 TexLookup[128];
uniform vec2 CharSize;
                                               
uniform vec4 TexColor;
uniform mat4 Projection;
                                                
void main()
{
    vec4 texLocation = TexLookup[int(aCharLayout.w)];

    float x = aCharLayout.x + aPosition.x * aCharLayout.z;
    float y = aCharLayout.y + aPosition.y * aCharLayout.z;
    float tx = texLocation.x + texLocation.z * (aPosition.x / CharSize.x);
    float ty = texLocation.y + texLocation.w * (aPosition.y / CharSize.y);

    gl_Position = Projection * vec4(x, y, 0.0f, 1.0f);

    texColor = vec4(TexColor.xyz, TexColor.w * (1.0f - aCharAlpha));
    texCoord = vec2(tx, ty);
}";

        public readonly static string Fragment = @"#version 330 core
out vec4 FragColor;

in vec4 texColor;
in vec2 texCoord;

uniform sampler2D texture0;
                                               
void main()
{
    FragColor = vec4(texColor.xyz, texture(texture0, texCoord).w * texColor.w);
}";
    }
}
