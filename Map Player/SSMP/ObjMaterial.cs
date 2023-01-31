using System;
using System.IO;

namespace SSQE_Player
{
    class ObjMaterial
    {
        public string TextureFile { get; private set; }

        private ObjMaterial()
        {

        }

        public static ObjMaterial FromFile(string file)
        {
            var lines = File.ReadAllLines(file);

            var material = new ObjMaterial();

            for (var i = 0; i < lines.Length; i++)
            {
                var line = lines[i];

                if (line.StartsWith("map_Kd "))
                {
                    var textureFile = line.Substring(7);

                    if (File.Exists(textureFile))
                        material.TextureFile = textureFile;
                    else
                        Console.WriteLine($"MapName '{textureFile}' was not found");

                    break;
                }
            }

            return material;
        }
    }
}
