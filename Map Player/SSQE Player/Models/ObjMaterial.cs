namespace SSQE_Player.Models
{
    internal class ObjMaterial
    {
        public string TextureFile;

        private ObjMaterial() { }

        public static ObjMaterial FromFile(string file)
        {
            string[] lines = File.ReadAllLines(file);
            ObjMaterial material = new();

            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];

                if (line.StartsWith("map_Kd "))
                {
                    string textureFile = line[7..];

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
