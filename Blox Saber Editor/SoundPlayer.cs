using System;
using System.Collections.Generic;
using Un4seen.Bass;
using Un4seen.Bass.AddOn.Fx;

namespace Sound_Space_Editor
{
	//class SoundPlayer : IDisposable
	//{
	//	private AudioContext _context;

	//	private readonly Dictionary<string, Tuple<int, int>> _sounds = new Dictionary<string, Tuple<int, int>>();

	//	private string _lastId;

	//	public void Init()
	//	{
	//		_context = new AudioContext();

	//		AL.Listener(ALListenerf.Gain, 1);
	//		AL.Listener(ALListener3f.Position, 0, 0, 0);
	//		AL.Listener(ALListener3f.Velocity, 0, 0, 0);
	//	}

	//	public void Cache(string id, string ext = "wav")
	//	{
	//		//create a buffer
	//		byte[] data;
	//		WaveFormat format;

	//		using (var afr = new AudioFileReader($"assets/sounds/{id}.{ext}"))
	//		{
	//			data = new byte[afr.Length];

	//			var provider = afr.ToSampleProvider().ToStereo().ToWaveProvider16();
	//			provider.Read(data, 0, data.Length);

	//			format = provider.WaveFormat;
	//		}

	//		var buffer = AL.GenBuffer();
	//		AL.BufferData(buffer, ALFormat.Stereo16, data, data.Length, format.SampleRate);

	//		//create audio source
	//		var source = AL.GenSource();
	//		AL.Source(source, ALSourcef.Gain, 0f);
	//		AL.Source(source, ALSourcef.Pitch, 1);
	//		AL.Source(source, ALSource3f.Position, 0, 0, 0);

	//		AL.BindBufferToSource(source, buffer);

	//		_sounds.Add(id, new Tuple<int, int>(source, buffer));
	//	}

	//	public void Play(string id, float volume = 1)
	//	{
	//		if (_sounds.TryGetValue(id, out var sound))
	//		{
	//			if (id != _lastId)
	//			{
	//				_lastId = id;

	//				AL.Source(sound.Item1, ALSourcei.Buffer, sound.Item2);
	//			}

	//			AL.Source(sound.Item1, ALSourcef.Gain, volume);
	//			AL.SourcePlay(sound.Item1);
	//		}
	//	}

	//	public void Dispose()
	//	{
	//		foreach (var tuple in _sounds.Values)
	//		{
	//			AL.DeleteSource(tuple.Item1);
	//			AL.DeleteBuffer(tuple.Item2);
	//		}

	//		_context.Dispose();
	//	}
	//}

	class SoundPlayer : IDisposable
	{
		private readonly Dictionary<string, string> _sounds = new Dictionary<string, string>();

		public SoundPlayer()
		{
			Bass.BASS_Init(-1, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero);
		}

		public void Cache(string id, string ext = "wav")
		{
			_sounds.Add(id, $"assets/sounds/{id}.{ext}");
		}

		public void Play(string id, float volume = 1, float pitch = 1)
		{
			if (_sounds.TryGetValue(id, out var sound))
			{
				var s = Bass.BASS_StreamCreateFile(sound, 0, 0, BASSFlag.BASS_STREAM_DECODE | BASSFlag.BASS_STREAM_PRESCAN | BASSFlag.BASS_FX_FREESOURCE);//sound, 0, 0, BASSFlag.BASS_STREAM_AUTOFREE);
				
				s = BassFx.BASS_FX_TempoCreate(s, BASSFlag.BASS_STREAM_PRESCAN | BASSFlag.BASS_STREAM_AUTOFREE);

				Bass.BASS_ChannelSetAttribute(s, BASSAttribute.BASS_ATTRIB_VOL, volume);
				Bass.BASS_ChannelSetAttribute(s, BASSAttribute.BASS_ATTRIB_TEMPO_PITCH, (pitch - 1) * 60);

				//Bass.BASS_ChannelPlay(sound, false);
				Bass.BASS_ChannelPlay(s, false);
			}
		}

		public void Dispose()
		{
			Bass.BASS_Free();
		}
	}
}