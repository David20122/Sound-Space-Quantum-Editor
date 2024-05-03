using Un4seen.Bass;
using Un4seen.Bass.AddOn.Fx;

namespace New_SSQE
{
    internal class SoundPlayer
    {
        private readonly Dictionary<string, string> files = new();
        public float Volume;

        public SoundPlayer()
        {
            var sounds = Directory.GetFiles("assets/sounds");

            foreach (var file in sounds)
                files.Add(Path.GetFileNameWithoutExtension(file), file);
        }

        public void Play(string fileName)
        {
            if (files.TryGetValue(fileName, out var value))
            {
                var s = Bass.BASS_StreamCreateFile(value, 0, 0, BASSFlag.BASS_STREAM_DECODE | BASSFlag.BASS_MUSIC_PRESCAN);
                s = BassFx.BASS_FX_TempoCreate(s, BASSFlag.BASS_STREAM_AUTOFREE | BASSFlag.BASS_FX_FREESOURCE);

                Bass.BASS_ChannelSetAttribute(s, BASSAttribute.BASS_ATTRIB_VOL, fileName == Settings.settings["clickSound"] ? 0.035f : Volume);

                Bass.BASS_ChannelPlay(s, false);
            }
        }
    }
}
