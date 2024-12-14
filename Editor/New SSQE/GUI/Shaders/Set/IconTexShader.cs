namespace New_SSQE.GUI.Shaders.Set
{
    internal static class IconTexShader
    {
        public readonly static string Vertex = @"#version 330 core
layout (location = 0) in vec2 aPosition;
layout (location = 1) in vec4 aTexCoord; // only using x/y but inputting vec4 for compatibility
layout (location = 2) in vec4 aOffset; // x, c, i, a

out vec2 texCoord;
out vec4 aColor;

uniform vec2 SpriteSize;
uniform mat4 Projection;
uniform vec4 NoteColors[32];

void main()
{
    int yOff = int(aOffset.z * SpriteSize.x);
    int xOff = int(aOffset.z - yOff / SpriteSize.x);

    gl_Position = Projection * vec4(aPosition.x + aOffset.x, aPosition.y, 0.0f, 1.0f);

    texCoord = vec2((aTexCoord.x + xOff) * SpriteSize.x, (aTexCoord.y + yOff) * SpriteSize.y);
    aColor = vec4(NoteColors[int(aOffset.y)].xyz, aOffset.w);
}";

        public readonly static string Fragment = @"#version 330 core
out vec4 FragColor;

in vec2 texCoord;
in vec4 aColor;

uniform sampler2D texture0;

void main()
{
    vec4 color = texture(texture0, texCoord);
    FragColor = color * aColor;
}";
    }
}
