using System.IO;
using System.Collections.Generic;
using Un4seen.Bass;
using Un4seen.Bass.AddOn.Fx;

namespace Sound_Space_Editor
{
	class SoundPlayer
	{
        private Dictionary<string, string> files = new Dictionary<string, string>();
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
                var s = Bass.BASS_StreamCreateFile(value, 0, 0, BASSFlag.BASS_STREAM_DECODE | BASSFlag.BASS_STREAM_PRESCAN | BASSFlag.BASS_FX_FREESOURCE);//sound, 0, 0, BASSFlag.BASS_STREAM_AUTOFREE);

                s = BassFx.BASS_FX_TempoCreate(s, BASSFlag.BASS_STREAM_PRESCAN | BASSFlag.BASS_STREAM_AUTOFREE | BASSFlag.BASS_FX_FREESOURCE | BASSFlag.BASS_MUSIC_AUTOFREE);

                Bass.BASS_ChannelSetAttribute(s, BASSAttribute.BASS_ATTRIB_VOL, fileName == "hit" ? Volume : 0.035f);

                Bass.BASS_ChannelPlay(s, false);
            }
        }
    }
}