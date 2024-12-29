using New_SSQE.ExternalUtils;
using New_SSQE.Maps;
using New_SSQE.Misc.Dialogs;
using New_SSQE.Misc.Static;
using New_SSQE.Preferences;
using Un4seen.Bass;
using Un4seen.Bass.AddOn.Fx;
using Un4seen.Bass.Misc;

namespace New_SSQE.Audio
{
    internal class MusicPlayer
    {
        private static int streamFileID;
        private static int streamID;
        private static string lastFile;
        public static BASSChannelType ctype;

        private static float originVal;

        public static float[] WaveModel = Array.Empty<float>();

        private static readonly SYNCPROC sync;

        static MusicPlayer()
        {
            Init();
            sync = new SYNCPROC(OnEnded);
        }

        private static void RunMP3Convert()
        {
            if (!File.Exists($"{Assets.THIS}\\lame.exe"))
            {
                MessageBox.Show("MP3 encoder not present from latest release", MBoxIcon.Warning, MBoxButtons.OK);
                return;
            }

            try
            {
                Pause();
                EncoderLAME encoder = new(0) { LAME_Bitrate = 192 };
                bool success = BaseEncoder.EncodeFile(lastFile, $"{Assets.TEMP}\\tempaudio.mp3", encoder, null, true, true);

                if (success)
                {
                    File.Move($"{Assets.TEMP}\\tempaudio.mp3", lastFile);

                    streamFileID = Bass.BASS_StreamCreateFile(lastFile, 0, 0, BASSFlag.BASS_STREAM_DECODE | BASSFlag.BASS_STREAM_PRESCAN | BASSFlag.BASS_FX_FREESOURCE);
                }
                else
                    throw new FormatException(Bass.BASS_ErrorGetCode().ToString());
            }
            catch (Exception ex)
            {
                Logging.Register($"Failed to encode file to mp3: {lastFile}", LogSeverity.WARN, ex);
                MessageBox.Show($"Failed to convert file {lastFile}\n\nExternal playtesting may not be possible with this asset.", MBoxIcon.Warning, MBoxButtons.OK);
            }

            Load(lastFile);
            Volume = Settings.masterVolume.Value.Value;
        }

        public static void ConvertToMP3()
        {
            if (Platform.IsLinux)
                return;

            if (IsMP3)
            {
                DialogResult result = MessageBox.Show("This asset is already an MP3. Do you want to convert it anyway?\n\nThis may take a while.", MBoxIcon.Info, MBoxButtons.Yes_No);
                if (result != DialogResult.Yes)
                    return;
            }
            else
            {
                DialogResult result = MessageBox.Show("Are you sure you want to convert this asset to MP3?\n\nThis may take a while.", MBoxIcon.Info, MBoxButtons.Yes_No);
                if (result != DialogResult.Yes)
                    return;
            }

            RunMP3Convert();
        }

        private static void CheckDevice()
        {
            try
            {
                int device = Bass.BASS_ChannelGetDevice(streamID);
                BASS_DEVICEINFO? info = Bass.BASS_GetDeviceInfo(device);

                if (info != null && (!info.IsDefault || !info.IsEnabled))
                {
                    long pos = Bass.BASS_ChannelGetPosition(streamID, BASSMode.BASS_POS_BYTE);
                    TimeSpan secs = TimeSpan.FromSeconds(Bass.BASS_ChannelBytes2Seconds(streamID, pos));

                    BASSActive state = Bass.BASS_ChannelIsActive(streamID);
                    float volume = 0.2f;

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

        public static bool Load(string file)
        {
            if (file == null || !File.Exists(file))
                return true;
            if (lastFile != file)
                lastFile = file;

            bool Error(BASSError err)
            {
                string id = Path.GetFileNameWithoutExtension(file);

                Logging.Register($"Audio failed to load - {err}\n{file}", LogSeverity.ERROR);
                DialogResult message = MessageBox.Show($"Audio file with id '{id}' is corrupt.\n\nWould you like to try importing a new file?", MBoxIcon.Warning, MBoxButtons.OK_Cancel);

                return message == DialogResult.OK && MapManager.ImportAudio(id);
            }

            Bass.BASS_StreamFree(streamID);
            Bass.BASS_StreamFree(streamFileID);

            int stream = Bass.BASS_StreamCreateFile(file, 0, 0, BASSFlag.BASS_STREAM_DECODE | BASSFlag.BASS_STREAM_PRESCAN | BASSFlag.BASS_FX_FREESOURCE);
            BASSError err = Bass.BASS_ErrorGetCode();

            if (Bass.BASS_ChannelGetInfo(stream) == null)
                return Error(err);

            ctype = Bass.BASS_ChannelGetInfo(stream).ctype;
            if (ctype == BASSChannelType.BASS_CTYPE_STREAM_MF)
                RunMP3Convert();

            float tempo = Tempo;

            streamFileID = stream;
            streamID = BassFx.BASS_FX_TempoCreate(streamFileID, BASSFlag.BASS_STREAM_PRESCAN);

            Tempo = tempo;

            Bass.BASS_ChannelGetAttribute(streamID, BASSAttribute.BASS_ATTRIB_TEMPO_FREQ, ref originVal);
            Bass.BASS_ChannelSetSync(streamID, BASSSync.BASS_SYNC_END, 0, sync, nint.Zero);

            Reset();

            if (TotalTime.TotalMilliseconds < 0)
                return Error(Bass.BASS_ErrorGetCode());

            if (Settings.waveform.Value)
                Waveform.Init((int)Bass.BASS_ChannelSeconds2Bytes(streamFileID, 1.0));

            return true;
        }

        private static void OnEnded(int handle, int channel, int data, nint user)
        {
            Pause();
            CurrentTime = TotalTime;
            Settings.currentTime.Value.Value = (float)(CurrentTime.TotalMilliseconds + 0.03 * (1 + (CurrentMap.Tempo - 1) * 1.5));
        }

        public static void Play()
        {
            CurrentTime = TimeSpan.FromMilliseconds(Settings.currentTime.Value.Value);
            CheckDevice();

            Bass.BASS_ChannelPlay(streamID, false);
        }

        public static void Pause()
        {
            CheckDevice();

            long pos = Bass.BASS_ChannelGetPosition(streamID, BASSMode.BASS_POS_BYTE);

            Bass.BASS_ChannelPause(streamID);
            Bass.BASS_ChannelSetPosition(streamID, pos, BASSMode.BASS_POS_BYTE);
            Settings.currentTime.Value.Value = (float)CurrentTime.TotalMilliseconds;
        }

        public static void Stop()
        {
            CheckDevice();

            Bass.BASS_ChannelStop(streamID);
            Bass.BASS_ChannelSetPosition(streamID, 0, BASSMode.BASS_POS_BYTE);
        }

        public static int GetData(ref short[] buffer)
        {
            return Bass.BASS_ChannelGetData(streamFileID, buffer, buffer.Length * 2);
        }

        public static float Tempo
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

        public static float Volume
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

        public static void Reset()
        {
            Stop();

            CurrentTime = TimeSpan.Zero;
        }

        public static bool IsPlaying
        {
            get
            {
                CheckDevice();

                return Bass.BASS_ChannelIsActive(streamID) == BASSActive.BASS_ACTIVE_PLAYING;
            }
        }

        public static TimeSpan TotalTime
        {
            get
            {
                CheckDevice();

                long len = Bass.BASS_ChannelGetLength(streamID, BASSMode.BASS_POS_BYTE);

                return TimeSpan.FromSeconds(Bass.BASS_ChannelBytes2Seconds(streamID, len) - 0.001);
            }
        }

        public static TimeSpan CurrentTime
        {
            set
            {
                CheckDevice();

                long pos = Bass.BASS_ChannelSeconds2Bytes(streamID, value.TotalSeconds - 0.03 * (1 + (CurrentMap.Tempo - 1) * 1.5));

                Bass.BASS_ChannelSetPosition(streamID, Math.Max(pos, 0), BASSMode.BASS_POS_BYTE);
            }
            get
            {
                CheckDevice();

                long pos = Bass.BASS_ChannelGetPosition(streamID, BASSMode.BASS_POS_BYTE);

                return TimeSpan.FromSeconds(Bass.BASS_ChannelBytes2Seconds(streamID, pos) + 0.03 * (1 + (CurrentMap.Tempo - 1) * 1.5));
            }
        }

        public static bool IsMP3 => ctype == BASSChannelType.BASS_CTYPE_STREAM_MP3;
        public static bool IsOGG => ctype == BASSChannelType.BASS_CTYPE_STREAM_OGG;

        public static float DetectBPM(long start, long end)
        {
            CheckDevice();
            float result = BassFx.BASS_FX_BPM_DecodeGet(streamFileID, start / 1000d, end / 1000d, 0, BASSFXBpm.BASS_FX_BPM_BKGRND | BASSFXBpm.BASS_FX_BPM_MULT2, null, nint.Zero);

            if (result < 0)
                Logging.Register($"Detect BPM failed with error code: {Bass.BASS_ErrorGetCode()}", LogSeverity.WARN);

            BassFx.BASS_FX_BPM_Free(streamFileID);
            return result;
        }

        private static void Init()
        {
            Bass.BASS_Init(-1, 44100, BASSInit.BASS_DEVICE_DEFAULT, nint.Zero);

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

        public static string SupportedExtensionsString => Bass.SupportedStreamExtensions + ";*.egg;*.flac;*.asset";
        public static HashSet<string> SupportedExtensions => SupportedExtensionsString.Replace("*", "").Split(';').ToHashSet();
    }
}
