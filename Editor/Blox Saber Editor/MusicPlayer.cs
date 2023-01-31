using System;
using System.Collections.Generic;
using System.IO;
using Un4seen.Bass;
using Un4seen.Bass.AddOn.Fx;
using Un4seen.Bass.Misc;

namespace Sound_Space_Editor
{
	class MusicPlayer
	{
		private int streamFileID;
		private int streamID;
		private string lastFile;
		public float originval;

		//private float[] samples;

		public ModelRaw WaveModel;

		private SYNCPROC Sync;

		public MusicPlayer()
		{
			Init();
			Sync = new SYNCPROC(OnEnded);
		}

		/// <summary>
		///		This function makes sure your default device is being used, if not, reload Bass and the song back and continue as if nothing happened.
		/// </summary>
		private void CheckDevice()
		{
			var device = Bass.BASS_ChannelGetDevice(streamID);
			var info = Bass.BASS_GetDeviceInfo(device);

			if (info != null && (!info.IsDefault || !info.IsEnabled))
			{
				var pos = Bass.BASS_ChannelGetPosition(streamID, BASSMode.BASS_POS_BYTES);
				var secs = TimeSpan.FromSeconds(Bass.BASS_ChannelBytes2Seconds(streamID, pos));

				var state = Bass.BASS_ChannelIsActive(streamID);
				var volume = 0.3f;

				Bass.BASS_ChannelGetAttribute(streamID, BASSAttribute.BASS_ATTRIB_VOL, ref volume);

				Reload();

				Load(lastFile);

				Volume = volume;
				CurrentTime = secs;

				switch (state)
				{
					case BASSActive.BASS_ACTIVE_PAUSED:
					case BASSActive.BASS_ACTIVE_STOPPED:
						Bass.BASS_ChannelPause(streamID);
						Bass.BASS_ChannelSetPosition(streamID, pos, BASSMode.BASS_POS_BYTES);
						break;
					case BASSActive.BASS_ACTIVE_STALLED:
					case BASSActive.BASS_ACTIVE_PLAYING:
						Bass.BASS_ChannelPlay(streamID, false);
						break;
				}
			}
		}

		public void Load(string file)
		{
			if (file == null || !File.Exists(file))
				return;

			Bass.BASS_StreamFree(streamID);
			Bass.BASS_StreamFree(streamFileID);

			if (lastFile != file)
			{
				lastFile = file;

				WaveModel?.Dispose();

				var wf = new WaveForm(file);
				wf.FrameResolution = 0.005;

				if (wf.RenderStart(false, BASSFlag.BASS_STREAM_DECODE | BASSFlag.BASS_STREAM_PRESCAN | BASSFlag.BASS_FX_FREESOURCE))
				{
					var samples = new float[wf.Wave.data.Length];

					var maxPeak = 0f;

					for (int i = 0; i < wf.Wave.data.Length; i++)
					{
						var sample = wf.Wave.data[i];

						var peakLeft = sample.left / (float)short.MaxValue;
						var peakRight = sample.right / (float)short.MaxValue;

						var peak = Math.Max(Math.Abs(peakLeft), Math.Abs(peakRight));

						maxPeak = Math.Max(peak, maxPeak);

						samples[i] = peak;
					}

					var vertexes = new Queue<float>();

					// averaging values
					for (int i = 0; i < samples.Length; i++)
					{
						if (maxPeak > 0.001)
						{
							samples[i] /= maxPeak;
						}
						samples[i] *= 0.9f;

						vertexes.Enqueue(i / (float)samples.Length * 100000);
						vertexes.Enqueue(samples[i]);
					}

					WaveModel = ModelManager.LoadModel2ToVao(vertexes.ToArray());
					wf.Reset();
				}
			}

			var stream = Bass.BASS_StreamCreateFile(file, 0, 0, BASSFlag.BASS_STREAM_DECODE | BASSFlag.BASS_STREAM_PRESCAN | BASSFlag.BASS_FX_FREESOURCE);
			var tempo = Tempo;

			streamFileID = stream;
			streamID = BassFx.BASS_FX_TempoCreate(streamFileID, BASSFlag.BASS_STREAM_PRESCAN);

			Tempo = tempo;

			Bass.BASS_ChannelGetAttribute(streamID, BASSAttribute.BASS_ATTRIB_TEMPO_FREQ, ref originval);
			Bass.BASS_ChannelSetSync(streamID, BASSSync.BASS_SYNC_END, 0, Sync, IntPtr.Zero);

			Reset();
		}

		/*
		public float GetPeak(double time)
		{
			if (samples == null)
				return 0;

			var length = TotalTime.TotalMilliseconds;
			var p = time / length;
			var index = Math.Max(0, Math.Min(samples.Length - 1, p * samples.Length));

			var alpha = index % 1.0;

			var first = 0f;
			var next = 0f;

			if (samples.Length > 1)
			{
				var indexFirst = (int)index;
				var indexSecond = Math.Min(samples.Length - 1, indexFirst + 1);

				first = samples[indexFirst];
				next = samples[indexSecond];
			}

			return first + (next - first) * (float)alpha;
		}*/

		private void OnEnded(int handle, int channel, int data, IntPtr user)
		{
			Pause();
			CurrentTime = TotalTime;
			Settings.settings["currentTime"].Value = (float)(CurrentTime.TotalMilliseconds + 0.03 * MainWindow.Instance.tempo);
		}

		public void Play()
		{
			CurrentTime = TimeSpan.FromMilliseconds(Settings.settings["currentTime"].Value);
			if (CurrentTime != TimeSpan.FromMilliseconds(Settings.settings["currentTime"].Value))
				Reset();
			CheckDevice();

			Bass.BASS_ChannelPlay(streamID, false);
		}

		public void Pause()
		{
			CheckDevice();

			var pos = Bass.BASS_ChannelGetPosition(streamID, BASSMode.BASS_POS_BYTES);

			Bass.BASS_ChannelPause(streamID);
			Bass.BASS_ChannelSetPosition(streamID, pos, BASSMode.BASS_POS_BYTES);
			Settings.settings["currentTime"].Value = (float)CurrentTime.TotalMilliseconds;
		}

		public void Stop()
		{
			CheckDevice();

			Bass.BASS_ChannelStop(streamID);
			Bass.BASS_ChannelSetPosition(streamID, 0, BASSMode.BASS_POS_BYTES);
		}

		public float Tempo
		{
			set
			{
				CheckDevice();

				Bass.BASS_ChannelSetAttribute(streamID, BASSAttribute.BASS_ATTRIB_TEMPO_FREQ, originval * value);
			}
			get
			{
				CheckDevice();

				float val = 0;

				Bass.BASS_ChannelGetAttribute(streamID, BASSAttribute.BASS_ATTRIB_TEMPO_FREQ, ref val);

				return -(val + 95) / 100;
			}
		}

		public float Volume
		{
			set
			{
				CheckDevice();
				Bass.BASS_ChannelSetAttribute(streamID, BASSAttribute.BASS_ATTRIB_VOL, value);
			}
			get
			{
				CheckDevice();

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

		public bool IsPlaying
		{
			get
			{
				CheckDevice();

				return Bass.BASS_ChannelIsActive(streamID) == BASSActive.BASS_ACTIVE_PLAYING;
			}
		}

		public bool IsPaused
		{
			get
			{
				CheckDevice();

				return Bass.BASS_ChannelIsActive(streamID) == BASSActive.BASS_ACTIVE_PAUSED;
			}
		}

		public TimeSpan TotalTime
		{
			get
			{
				CheckDevice();

				long len = Bass.BASS_ChannelGetLength(streamID, BASSMode.BASS_POS_BYTES);
				var time = TimeSpan.FromSeconds(Bass.BASS_ChannelBytes2Seconds(streamID, len) - 0.001);

				return time;
			}
		}

		public TimeSpan CurrentTime
		{
			get
			{
				CheckDevice();

				var pos = Bass.BASS_ChannelGetPosition(streamID, BASSMode.BASS_POS_BYTES);

				return TimeSpan.FromSeconds(Bass.BASS_ChannelBytes2Seconds(streamID, pos) + 0.03 * MainWindow.Instance.tempo);
			}
			set
			{
				CheckDevice();

				var pos = Bass.BASS_ChannelSeconds2Bytes(streamID, value.TotalSeconds - 0.03 * MainWindow.Instance.tempo);

				Bass.BASS_ChannelSetPosition(streamID, pos, BASSMode.BASS_POS_BYTES);
			}
		}

		public decimal Progress
		{
			get
			{
				CheckDevice();

				var pos = Bass.BASS_ChannelGetPosition(streamID, BASSMode.BASS_POS_BYTES);
				var length = Bass.BASS_ChannelGetLength(streamID, BASSMode.BASS_POS_BYTES);

				return pos / (decimal)length;
			}
		}

		private static void Init()
		{
			Bass.BASS_Init(-1, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero);

			Bass.BASS_SetConfig(BASSConfig.BASS_CONFIG_BUFFER, 250);
			Bass.BASS_SetConfig(BASSConfig.BASS_CONFIG_UPDATEPERIOD, 5);
		}

		public static void Reload()
		{
			Dispose();
			Init();
		}

		public static void Dispose()
		{
			Bass.BASS_Free();
		}
	}
}