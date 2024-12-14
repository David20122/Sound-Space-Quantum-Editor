using New_SSQE.Misc.Static;

namespace New_SSQE.Audio
{
    internal class SoundPlayer
    {
        private static readonly Dictionary<string, SoundQueue> queues = new();
        public static float Volume;

        static SoundPlayer()
        {
            string[] sounds = Directory.GetFiles(Assets.SOUNDS);

            foreach (string file in sounds)
                queues.Add(Path.GetFileNameWithoutExtension(file), new(file));
        }

        public static void Play(string fileName)
        {
            if (queues.TryGetValue(fileName, out SoundQueue? value))
                value.Play();
        }
    }
}
