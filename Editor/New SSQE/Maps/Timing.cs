using New_SSQE.Audio;
using New_SSQE.Objects;
using New_SSQE.Objects.Managers;
using New_SSQE.Preferences;
using OpenTK.Mathematics;

namespace New_SSQE.Maps
{
    internal static class Timing
    {
        public static List<Note> GetNotesFromPoint(TimingPoint point)
        {
            int index = CurrentMap.TimingPoints.IndexOf(point);
            long next = index + 1 < CurrentMap.TimingPoints.Count ? CurrentMap.TimingPoints[index + 1].Ms : (long)Settings.currentTime.Value.Max;
            List<Note> notes = [];

            for (int i = 0; i < CurrentMap.Notes.Count; i++)
            {
                Note note = CurrentMap.Notes[i];

                if (note.Ms >= point.Ms && note.Ms < next)
                    notes.Add(note);
            }

            return notes;
        }

        public static void UpdatePoint(TimingPoint point, float newBpm, long newMs)
        {
            float mult = point.BPM / newBpm;
            List<Note> notes = GetNotesFromPoint(point);

            NoteManager.Edit("ADJUST NOTE[S]", notes, (n) => n.Ms = (long)((n.Ms - point.Ms) * mult) + point.Ms);
        }

        public static void MovePoints(List<TimingPoint> points, long offset)
        {
            List<Note> toAdjust = [];

            for (int i = 0; i < points.Count; i++)
            {
                TimingPoint point = points[i];
                List<Note> notes = GetNotesFromPoint(point);

                for (int j = 0; j < notes.Count; j++)
                    toAdjust.Add(notes[j]);
            }

            NoteManager.Edit("ADJUST NOTE[S]", toAdjust, (n) => n.Ms += offset);
        }



        public static long GetClosestBeat(float currentMs, bool draggingPoint = false)
        {
            long closestMs = -1;
            TimingPoint point = GetCurrentBpm(currentMs, draggingPoint);

            if (point.BPM > 0)
            {
                float interval = 60000f / point.BPM / (Settings.beatDivisor.Value.Value + 1f);
                float offset = point.Ms % interval;

                closestMs = (long)Math.Round((long)Math.Round((currentMs - offset) / interval) * interval + offset);
            }

            return closestMs;
        }

        public static long GetClosestBeatScroll(float currentMs, bool negative = false, int iterations = 1)
        {
            long closestMs = GetClosestBeat(currentMs);

            if (GetCurrentBpm(closestMs).BPM <= 0)
                return -1;

            for (int i = 0; i < iterations; i++)
            {
                TimingPoint currentPoint = GetCurrentBpm(currentMs, negative);
                float interval = 60000 / currentPoint.BPM / (Settings.beatDivisor.Value.Value + 1f);

                if (negative)
                {
                    closestMs = GetClosestBeat(currentMs, true);

                    if (closestMs >= currentMs)
                        closestMs = GetClosestBeat(closestMs - (long)interval);
                }
                else
                {
                    if (closestMs <= currentMs)
                        closestMs = GetClosestBeat(closestMs + (long)interval);

                    if (GetCurrentBpm(currentMs).Ms != GetCurrentBpm(closestMs).Ms)
                        closestMs = GetCurrentBpm(closestMs, false).Ms;
                }

                currentMs = closestMs;
            }

            if (closestMs < 0)
                return -1;

            return (long)MathHelper.Clamp(closestMs, 0, Settings.currentTime.Value.Max);
        }

        public static TimingPoint GetCurrentBpm(float currentMs, bool draggingPoint = false)
        {
            TimingPoint currentPoint = new(0, 0);

            for (int i = 0; i < CurrentMap.TimingPoints.Count; i++)
            {
                TimingPoint point = CurrentMap.TimingPoints[i];

                if (point.Ms < currentMs || (!draggingPoint && point.Ms == currentMs))
                    currentPoint = point;
            }

            return currentPoint;
        }

        public static void Advance(bool reverse = false)
        {
            SliderSetting setting = Settings.currentTime.Value;
            float bpm = GetCurrentBpm(setting.Value).BPM;

            if (bpm > 0)
                setting.Value = GetClosestBeatScroll(setting.Value, reverse);
        }

        public static void Scroll(bool reverse, float mult = 1)
        {
            float delta = mult * (reverse ? -1 : 1);

            SliderSetting setting = Settings.currentTime.Value;
            float currentTime = setting.Value;
            float totalTime = setting.Max;

            if (MusicPlayer.IsPlaying && !Settings.pauseScroll.Value)
            {
                currentTime += delta * 500f * CurrentMap.Tempo;
                currentTime = MathHelper.Clamp(currentTime, 0f, totalTime);

                setting.Value = currentTime;

                MusicPlayer.CurrentTime = TimeSpan.FromMilliseconds(currentTime);
            }
            else
            {
                if (MusicPlayer.IsPlaying)
                    MusicPlayer.Pause();

                long closest = GetClosestBeatScroll(currentTime, delta < 0);
                TimingPoint bpm = GetCurrentBpm(0);

                currentTime = closest >= 0 || bpm.BPM > 0 ? closest : currentTime + delta / 10f * 1000f / CurrentMap.Zoom * 0.5f;

                if (GetCurrentBpm(setting.Value).BPM == 0 && GetCurrentBpm(currentTime).BPM != 0)
                    currentTime = GetCurrentBpm(currentTime).Ms;

                currentTime = MathHelper.Clamp(currentTime, 0f, totalTime);

                setting.Value = currentTime;
            }
        }
    }
}
