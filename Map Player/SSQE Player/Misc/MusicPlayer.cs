using System;
using Un4seen.Bass;
using System.IO;
using Un4seen.Bass.AddOn.Fx;

namespace SSQE_Player
{
    internal class MusicPlayer
    {
        private int streamFileID;
        private int streamID;
        private string lastFile;

        private float originVal;

        private SYNCPROC Sync;

        public MusicPlayer()
        {
            Init();
            Sync = new SYNCPROC(OnEnded);
        }

        private void CheckDevice()
        {
            try
            {
                var device = Bass.BASS_ChannelGetDevice(streamID);
                var info = Bass.BASS_GetDeviceInfo(device);

                if (info != null && (!info.IsDefault || !info.IsEnabled))
                {
                    var pos = Bass.BASS_ChannelGetPosition(streamID, BASSMode.BASS_POS_BYTE);
                    var secs = TimeSpan.FromSeconds(Bass.BASS_ChannelBytes2Seconds(streamID, pos));

                    var state = Bass.BASS_ChannelIsActive(streamID);
                    var volume = 0.2f;

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
                            Bass.BASS_ChannelSetPosition(streamID, pos, BASSMode.BASS_POS_BYTE);
                            break;
                        case BASSActive.BASS_ACTIVE_STALLED:
                        case BASSActive.BASS_ACTIVE_PLAYING:
                            Bass.BASS_ChannelPlay(streamID, false);
                            break;
                    }
                }
            }
            catch { }
        }

        public void Load(string file)
        {
            if (file == null || !File.Exists(file))
                return;
            if (lastFile != file)
                lastFile = file;

            Bass.BASS_StreamFree(streamID);
            Bass.BASS_StreamFree(streamFileID);

            var stream = Bass.BASS_StreamCreateFile(file, 0, 0, BASSFlag.BASS_STREAM_DECODE | BASSFlag.BASS_STREAM_PRESCAN | BASSFlag.BASS_FX_FREESOURCE);
            var tempo = Tempo;

            streamFileID = stream;
            streamID = BassFx.BASS_FX_TempoCreate(streamFileID, BASSFlag.BASS_STREAM_PRESCAN);

            Tempo = tempo;

            Bass.BASS_ChannelGetAttribute(streamID, BASSAttribute.BASS_ATTRIB_TEMPO_FREQ, ref originVal);
            Bass.BASS_ChannelSetSync(streamID, BASSSync.BASS_SYNC_END, 0, Sync, IntPtr.Zero);

            Reset();
        }

        private void OnEnded(int handle, int channel, int data, IntPtr user)
        {
            Pause();
            CurrentTime = TotalTime;
            Settings.settings["currentTime"].Value = (float)(CurrentTime.TotalMilliseconds);

            MainWindow.Instance.Close();
        }

        public void Play()
        {
            CurrentTime = TimeSpan.FromMilliseconds(Settings.settings["currentTime"].Value);
            CheckDevice();

            Bass.BASS_ChannelPlay(streamID, false);
        }

        public void Pause()
        {
            CheckDevice();

            var pos = Bass.BASS_ChannelGetPosition(streamID, BASSMode.BASS_POS_BYTE);

            Bass.BASS_ChannelPause(streamID);
            Bass.BASS_ChannelSetPosition(streamID, pos, BASSMode.BASS_POS_BYTE);
            Settings.settings["currentTime"].Value = (float)CurrentTime.TotalMilliseconds;
        }

        public void Stop()
        {
            CheckDevice();

            Bass.BASS_ChannelStop(streamID);
            Bass.BASS_ChannelSetPosition(streamID, 0, BASSMode.BASS_POS_BYTE);
        }

        public float Tempo
        {
            set
            {
                CheckDevice();

                Bass.BASS_ChannelSetAttribute(streamID, BASSAttribute.BASS_ATTRIB_TEMPO_FREQ, originVal * value);
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

        public TimeSpan TotalTime
        {
            get
            {
                CheckDevice();

                long len = Bass.BASS_ChannelGetLength(streamID, BASSMode.BASS_POS_BYTE);

                return TimeSpan.FromSeconds(Bass.BASS_ChannelBytes2Seconds(streamID, len) - 0.001);
            }
        }

        public TimeSpan CurrentTime
        {
            set
            {
                CheckDevice();

                var pos = Bass.BASS_ChannelSeconds2Bytes(streamID, value.TotalSeconds);

                Bass.BASS_ChannelSetPosition(streamID, Math.Max(pos, 0), BASSMode.BASS_POS_BYTE);
            }
            get
            {
                CheckDevice();

                var pos = Bass.BASS_ChannelGetPosition(streamID, BASSMode.BASS_POS_BYTE);

                return TimeSpan.FromSeconds(Bass.BASS_ChannelBytes2Seconds(streamID, pos));
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
