using System;
using Un4seen.Bass;
using Un4seen.Bass.AddOn.Fx;
using Un4seen.Bass.Misc;
namespace Sound_Space_Editor
{
	class MusicPlayer : IDisposable
	{

		private object locker = new object();

		private int streamID;

		public MusicPlayer()
		{
			Bass.BASS_Init(-1, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero);
		}

		public float originval;

        public void Load(string file)
		{
			var stream = Bass.BASS_StreamCreateFile(file, 0, 0, BASSFlag.BASS_STREAM_DECODE | BASSFlag.BASS_STREAM_PRESCAN | BASSFlag.BASS_FX_FREESOURCE);
			var volume = Volume;
			var tempo = Tempo;

			Bass.BASS_StreamFree(streamID);

			Bass.BASS_SetConfig(BASSConfig.BASS_CONFIG_BUFFER, 250);
			Bass.BASS_SetConfig(BASSConfig.BASS_CONFIG_UPDATEPERIOD, 5);

			streamID = BassFx.BASS_FX_TempoCreate(stream, BASSFlag.BASS_STREAM_PRESCAN);

			Volume = volume;
			Tempo = tempo;

			Bass.BASS_ChannelGetAttribute(streamID, BASSAttribute.BASS_ATTRIB_TEMPO_FREQ, ref originval);

			Reset();
		}

		public void Play()
		{
			Bass.BASS_SetDevice(1);
			Bass.BASS_Start();
			Bass.BASS_ChannelPlay(streamID, false);
		}

		public void Pause()
		{
			var pos = Bass.BASS_ChannelGetPosition(streamID, BASSMode.BASS_POS_BYTES);

			Bass.BASS_ChannelPause(streamID);

			Bass.BASS_ChannelSetPosition(streamID, pos, BASSMode.BASS_POS_BYTES);
		}

		public void Stop()
		{
			Bass.BASS_ChannelStop(streamID);

			Bass.BASS_ChannelSetPosition(streamID, 0, BASSMode.BASS_POS_BYTES);
		}

		public float Tempo
		{
			set => Bass.BASS_ChannelSetAttribute(streamID, BASSAttribute.BASS_ATTRIB_TEMPO_FREQ, originval * value);
			get
			{
				float val = 0;

				Bass.BASS_ChannelGetAttribute(streamID, BASSAttribute.BASS_ATTRIB_TEMPO_FREQ, ref val);

				return -(val + 95) / 100;
			}
		}

		public float Volume
		{
			set => Bass.BASS_ChannelSetAttribute(streamID, BASSAttribute.BASS_ATTRIB_VOL, value);
			get
			{
				float val = 1;

				Bass.BASS_ChannelGetAttribute(streamID, BASSAttribute.BASS_ATTRIB_VOL, ref val);

				return val;
			}
		}

		public void Reset()
		{
			Stop();
			CurrentTime = TimeSpan.Zero;
		}

		public bool IsPlaying => Bass.BASS_ChannelIsActive(streamID) == BASSActive.BASS_ACTIVE_PLAYING;
		public bool IsPaused => Bass.BASS_ChannelIsActive(streamID) == BASSActive.BASS_ACTIVE_PAUSED;

		public TimeSpan TotalTime
		{
			get
			{
				long len = Bass.BASS_ChannelGetLength(streamID, BASSMode.BASS_POS_BYTES);
				var time = TimeSpan.FromSeconds(Bass.BASS_ChannelBytes2Seconds(streamID, len));

				return time;
			}
		}

		public TimeSpan CurrentTime
		{
			get
			{
				var pos = Bass.BASS_ChannelGetPosition(streamID, BASSMode.BASS_POS_BYTES);
				var length = Bass.BASS_ChannelGetLength(streamID, BASSMode.BASS_POS_BYTES);

				return TimeSpan.FromTicks((long)(TotalTime.Ticks * pos / (decimal)length));
			}
			set
			{
				//lock (locker)
				{
					var pos = Bass.BASS_ChannelSeconds2Bytes(streamID, value.TotalSeconds);

					Bass.BASS_ChannelSetPosition(streamID, pos, BASSMode.BASS_POS_BYTES);
				}
			}
		}

		public decimal Progress
		{
			get
			{
				var pos = Bass.BASS_ChannelGetPosition(streamID, BASSMode.BASS_POS_BYTES);
				var length = Bass.BASS_ChannelGetLength(streamID, BASSMode.BASS_POS_BYTES);

				return pos / (decimal)length;
			}
		}

		public void Dispose()
		{
			Bass.BASS_Free();
		}

	}

}