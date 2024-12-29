using New_SSQE.Preferences;
using Un4seen.Bass;

namespace New_SSQE.Audio
{
    internal class SoundQueue
    {
        private readonly string file;
        private readonly string sound;
        private readonly Queue<int> sounds = new();

        private readonly SYNCPROC sync;

        public SoundQueue(string file)
        {
            this.file = file;
            sound = Path.GetFileNameWithoutExtension(file);

            sync = new SYNCPROC(OnEnded);
        }

        private void OnEnded(int handle, int channel, int data, nint user)
        {
            Bass.BASS_ChannelSetPosition(channel, 0, BASSMode.BASS_POS_BYTE);
            sounds.Enqueue(channel);
        }

        private int GetNew()
        {
            int stream = Bass.BASS_StreamCreateFile(file, 0, 0, BASSFlag.BASS_STREAM_PRESCAN);
            BASSError err = Bass.BASS_ErrorGetCode();

            Bass.BASS_ChannelSetSync(stream, BASSSync.BASS_SYNC_END, 0, sync, nint.Zero);

            return stream;
        }

        public void Play()
        {
            if (sounds.Count == 0)
                sounds.Enqueue(GetNew());

            int stream = sounds.Dequeue();
            Bass.BASS_ChannelSetAttribute(stream, BASSAttribute.BASS_ATTRIB_VOL, sound == Settings.clickSound.Value ? 0.035f : SoundPlayer.Volume);
            Bass.BASS_ChannelPlay(stream, false);
        }
    }
}
