using System;
using System.Collections.Generic;
using Un4seen.Bass;
using Un4seen.Bass.AddOn.Fx;

namespace Sound_Space_Editor
{
	class SoundPlayer
	{
		private readonly Dictionary<string, string> _sounds = new Dictionary<string, string>();

		public SoundPlayer()
		{
		}

		public void Cache(string id, string ext = "wav")
		{
			_sounds.Add(id, $"assets/sounds/{id}.{ext}");
		}

		public void Play(string id, float volume = 1, float speed = 1)
		{
			if (_sounds.TryGetValue(id, out var sound))
			{
				var s = Bass.BASS_StreamCreateFile(sound, 0, 0, BASSFlag.BASS_STREAM_DECODE | BASSFlag.BASS_STREAM_PRESCAN | BASSFlag.BASS_FX_FREESOURCE);//sound, 0, 0, BASSFlag.BASS_STREAM_AUTOFREE);
				
				s = BassFx.BASS_FX_TempoCreate(s, BASSFlag.BASS_STREAM_PRESCAN | BASSFlag.BASS_STREAM_AUTOFREE | BASSFlag.BASS_FX_FREESOURCE | BASSFlag.BASS_MUSIC_AUTOFREE);

				Bass.BASS_ChannelSetAttribute(s, BASSAttribute.BASS_ATTRIB_VOL, volume);
				Bass.BASS_ChannelSetAttribute(s, BASSAttribute.BASS_ATTRIB_FREQ, speed * 44100);

				Bass.BASS_ChannelPlay(s, false);
			}
		}
	}
}