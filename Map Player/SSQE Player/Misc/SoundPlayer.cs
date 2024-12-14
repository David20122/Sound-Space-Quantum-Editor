using Un4seen.Bass;
using Un4seen.Bass.AddOn.Fx;

namespace SSQE_Player
{
    internal class SoundPlayer
    {
        private readonly Dictionary<string, string> files = new();
        public float Volume;

        public SoundPlayer()
        {
            string[] sounds = Directory.GetFiles("assets/sounds");

            foreach (string file in sounds)
                files.Add(Path.GetFileNameWithoutExtension(file), file);
        }

        public void Play(string fileName)
        {
            if (files.TryGetValue(fileName, out string? value))
            {
                int s = Bass.BASS_StreamCreateFile(value, 0, 0, BASSFlag.BASS_STREAM_DECODE | BASSFlag.BASS_MUSIC_PRESCAN);
                s = BassFx.BASS_FX_TempoCreate(s, BASSFlag.BASS_MUSIC_AUTOFREE | BASSFlag.BASS_FX_FREESOURCE);

                Bass.BASS_ChannelSetAttribute(s, BASSAttribute.BASS_ATTRIB_VOL, fileName == Settings.hitSound.Value ? Volume : 0.035f);

                Bass.BASS_ChannelPlay(s, false);
            }
        }
    }
}
