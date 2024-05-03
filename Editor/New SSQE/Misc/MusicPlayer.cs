using System.Runtime.InteropServices;
using System.Threading.Channels;
using Un4seen.Bass;
using Un4seen.Bass.AddOn.Fx;
using Un4seen.Bass.Misc;

namespace New_SSQE
{
    internal class MusicPlayer
    {
        private int streamFileID;
        private int streamID;
        private string lastFile;

        private float originVal;

        public float[] WaveModel = Array.Empty<float>();

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

        public bool Load(string file)
        {
            if (file == null || !File.Exists(file))
                return true;
            if (lastFile != file)
                lastFile = file;

            Bass.BASS_StreamFree(streamID);
            Bass.BASS_StreamFree(streamFileID);

            var stream = Bass.BASS_StreamCreateFile(file, 0, 0, BASSFlag.BASS_STREAM_DECODE | BASSFlag.BASS_STREAM_PRESCAN | BASSFlag.BASS_FX_FREESOURCE);

            if (Bass.BASS_ChannelGetInfo(stream).ctype == BASSChannelType.BASS_CTYPE_STREAM_MF && RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                try
                {
                    EncoderLAME encoder = new(0);
                    BaseEncoder.EncodeFile(file, "assets/temp/tempaudio.mp3", encoder, null, true, true);
                    File.Move("assets/temp/tempaudio.mp3", file);

                    stream = Bass.BASS_StreamCreateFile(file, 0, 0, BASSFlag.BASS_STREAM_DECODE | BASSFlag.BASS_STREAM_PRESCAN | BASSFlag.BASS_FX_FREESOURCE);
                }
                catch (Exception ex)
                {
                    ActionLogging.Register($"Failed to encode file to mp3: {file}", "WARN", ex);
                    MessageBox.Show($"Failed to fix encoding for file '{file}'\n\nRhythia playtesting will not be possible with this asset.", "Warning", "OK");
                }
            }

            var tempo = Tempo;

            streamFileID = stream;
            streamID = BassFx.BASS_FX_TempoCreate(streamFileID, BASSFlag.BASS_STREAM_PRESCAN);

            Tempo = tempo;

            Bass.BASS_ChannelGetAttribute(streamID, BASSAttribute.BASS_ATTRIB_TEMPO_FREQ, ref originVal);
            Bass.BASS_ChannelSetSync(streamID, BASSSync.BASS_SYNC_END, 0, Sync, IntPtr.Zero);
            Waveform.Init(streamFileID);
            
            Reset();


            if (TotalTime.TotalMilliseconds < 0)
            {
                string id = Path.GetFileNameWithoutExtension(file);

                ActionLogging.Register($"Failed to load audio, error code: {Bass.BASS_ErrorGetCode()}", "WARN");
                var message = MessageBox.Show($"Audio file with id '{id}' is corrupt.\n\nWould you like to try importing a new file?", "Warning", "OK", "Cancel");

                return message == DialogResult.OK && MainWindow.Instance.PromptImport(id);
            }

            return true;
        }

        private void OnEnded(int handle, int channel, int data, IntPtr user)
        {
            Pause();
            CurrentTime = TotalTime;
            Settings.settings["currentTime"].Value = (float)(CurrentTime.TotalMilliseconds + 0.03 * (1 + (MainWindow.Instance.Tempo - 1) * 1.5));
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

                var pos = Bass.BASS_ChannelSeconds2Bytes(streamID, value.TotalSeconds - 0.03 * (1 + (MainWindow.Instance.Tempo - 1) * 1.5));

                Bass.BASS_ChannelSetPosition(streamID, Math.Max(pos, 0), BASSMode.BASS_POS_BYTE);
            }
            get
            {
                CheckDevice();

                var pos = Bass.BASS_ChannelGetPosition(streamID, BASSMode.BASS_POS_BYTE);

                return TimeSpan.FromSeconds(Bass.BASS_ChannelBytes2Seconds(streamID, pos) + 0.03 * (1 + (MainWindow.Instance.Tempo - 1) * 1.5));
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
