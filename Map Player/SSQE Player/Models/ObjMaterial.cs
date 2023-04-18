namespace SSQE_Player.Models
{
    internal class ObjMaterial
    {
        public string TextureFile;

        private ObjMaterial() { }

        public static ObjMaterial FromFile(string file)
        {
            var lines = File.ReadAllLines(file);
            var material = new ObjMaterial();

            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i];

                if (line.StartsWith("map_Kd "))
                {
                    var textureFile = line[7..];

                    if (File.Exists(textureFile))
                        material.TextureFile = textureFile;
                    else
                        Console.WriteLine($"Mapname '{textureFile}' was not found");

                    break;
                }
            }

            return material;
        }
    }
}
