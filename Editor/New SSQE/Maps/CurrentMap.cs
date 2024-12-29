using New_SSQE.Audio;
using New_SSQE.ExternalUtils;
using New_SSQE.FileParsing;
using New_SSQE.GUI;
using New_SSQE.Objects;
using New_SSQE.Objects.Managers;
using New_SSQE.Objects.Other;
using New_SSQE.Preferences;
using OpenTK.Mathematics;
using System.Drawing;

namespace New_SSQE.Maps
{
    internal static class CurrentMap
    {
        public static ObjectList<Note> Notes = new();
        public static ObjectList<MapObject> VfxObjects = new();
        public static ObjectList<MapObject> SpecialObjects = new();

        public static List<Note> BezierNodes = new();
        public static List<Bookmark> Bookmarks = new();
        public static List<TimingPoint> TimingPoints = new();

        public static TimingPoint? SelectedPoint = null;
        public static MapObject? SelectedObjDuration = null;

        public static string? FileName;
        public static string SoundID = "-1";
        public static string FileID => Path.GetFileNameWithoutExtension(FileName) ?? SoundID;

        public static float Tempo = 1f;
        public static float Zoom = 1f;
        public static float NoteStep => 500f * Zoom;

        public static Map? LoadedMap;

        public static void ClearSelection()
        {
            Notes.ClearSelection();
            VfxObjects.ClearSelection();
            SpecialObjects.ClearSelection();
            SelectedPoint = null;
            SelectedObjDuration = null;
        }

        public static bool IsSaved => FileName != null && File.Exists(FileName) && File.ReadAllText(FileName) == Parser.Save(SoundID);

        public static void Reset()
        {
            LoadedMap?.Save();
            LoadedMap = null;

            Tempo = 1f;
            Zoom = 1f;

            Notes.Clear();
            VfxObjects.Clear();
            SpecialObjects.Clear() ;
            
            BezierNodes.Clear();
            Bookmarks.Clear();
            TimingPoints.Clear();

            SelectedPoint = null;
            SelectedObjDuration = null;

            FileName = null;
            SoundID = "-1";

            Settings.mappers.Value = "";
            Settings.songName.Value = Path.GetFileNameWithoutExtension(FileName) ?? "Untitled Song";
            Settings.difficulty.Value = "N/A";
            Settings.useCover.Value = true;
            Settings.cover.Value = "Default";
            Settings.customDifficulty.Value = "";
            Settings.rating.Value = 0;
            Settings.useVideo.Value = false;
            Settings.video.Value = "";

            Settings.songOffset.Value = "";
            Settings.songTitle.Value = "";
            Settings.songArtist.Value = "";
            Settings.mapCreator.Value = "";
            Settings.mapCreatorPersonalLink.Value = "";
            Settings.previewStartTime.Value = "";
            Settings.previewDuration.Value = "";
            Settings.novaCover.Value = "";
            Settings.novaIcon.Value = "";

            GC.Collect();
        }



        public static void IncrementZoom(float increment)
        {
            float step = Zoom < 0.1f || (Zoom == 0.1f && increment < 0) ? 0.01f : 0.1f;

            Zoom = (float)Math.Round(Zoom + increment * step, 2);
            if (Zoom > 0.1f)
                Zoom = (float)Math.Round(Zoom * 10) / 10;

            Zoom = MathHelper.Clamp(Zoom, 0.01f, 10f);
        }

        public static void SetTempo(float tempo)
        {
            float tempoA = Math.Min(tempo, 0.9f);
            float tempoB = (tempo - tempoA) * 2f;

            Tempo = tempoA + tempoB + 0.1f;
            MusicPlayer.Tempo = Tempo;
        }



        public static void SortTimings(bool updateList = true)
        {
            TimingPoints = new(TimingPoints.OrderBy(n => n.Ms));

            if (updateList)
                TimingsWindow.Instance?.ResetList();

            if (MainWindow.Instance.CurrentWindow is GuiWindowEditor editor)
                editor.Timeline.GenerateOffsets();
        }

        public static void SortBookmarks(bool updateList = true)
        {
            Bookmarks = new(Bookmarks.OrderBy(n => n.Ms));

            if (updateList)
                BookmarksWindow.Instance?.ResetList();

            if (MainWindow.Instance.CurrentWindow is GuiWindowEditor editor)
                editor.Timeline.GenerateOffsets();
        }



        public static void CopyBookmarks()
        {
            string[] data = new string[Bookmarks.Count];

            for (int i = 0; i < Bookmarks.Count; i++)
            {
                Bookmark bookmark = Bookmarks[i];

                if (bookmark.Ms != bookmark.EndMs)
                    data[i] = $"{bookmark.Ms}-{bookmark.EndMs} ~ {bookmark.Text.Replace(" ~", "_~")}";
                else
                    data[i] = $"{bookmark.Ms} ~ {bookmark.Text.Replace(" ~", "_~")}";
            }

            if (data.Length == 0)
                return;

            Clipboard.SetText(string.Join("\n", data));

            if (MainWindow.Instance.CurrentWindow is GuiWindowEditor editor)
                editor.ShowToast("COPIED TO CLIPBOARD", Color.FromArgb(0, 255, 200));
        }

        public static void PasteBookmarks()
        {
            string data = Clipboard.GetText();
            string[] bookmarks = data.Split('\n');

            List<Bookmark> tempBookmarks = new();

            for (int i = 0; i < bookmarks.Length; i++)
            {
                bookmarks[i] = bookmarks[i].Trim();

                string[] split = bookmarks[i].Split(" ~");
                if (split.Length != 2)
                    continue;

                string[] subsplit = split[0].Split("-");

                if (subsplit.Length == 1 && long.TryParse(subsplit[0], out long ms))
                    tempBookmarks.Add(new Bookmark(split[1].Trim().Replace("_~", " ~"), ms, ms));
                else if (subsplit.Length == 2 && long.TryParse(subsplit[0], out long startMs) && long.TryParse(subsplit[1], out long endMs))
                    tempBookmarks.Add(new Bookmark(split[1].Trim().Replace("_~", " ~"), startMs, endMs));
            }

            if (tempBookmarks.Count > 0)
                BookmarkManager.Replace("PASTE BOOKMARK[S]", Bookmarks, tempBookmarks);
        }



        private static readonly Dictionary<int, bool> vfxSet = new()
        {
            {0, false },
            {1, false },
            {2, true },
            {3, true },
            {4, true },
            {5, true },
            {6, true },
            {7, true },
            {8, true },
            {9, true },
            {10, true },
            {11, true },
            {12, false },
        };

        public static (List<MapObject>, List<MapObject>) SplitVFXSpecial(List<MapObject> objects)
        {
            List<MapObject> vfxTemp = new();
            List<MapObject> specTemp = new();

            foreach (MapObject obj in objects)
            {
                if (vfxSet[obj.ID])
                    vfxTemp.Add(obj);
                else
                    specTemp.Add(obj);
            }

            return (vfxTemp, specTemp);
        }
    }
}
