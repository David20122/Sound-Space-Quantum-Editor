using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace New_SSQE.Types
{
    internal class Map
    {
        private List<Note> notes;
        private List<Note> selectedNotes;
        private List<Note> bezierNodes;

        private List<TimingPoint> timingPoints;
        private TimingPoint? selectedPoint;

        private List<Bookmark> bookmarks;

        private float tempo;
        private float zoom;

        private string? fileName;
        private string soundID;

        private float currentTime;
        private float beatDivisor;
        private long exportOffset;

        private string mappers;
        private string songName;
        private string difficulty;
        private bool useCover;
        private string cover;
        private string customDifficulty;

        private List<URAction> urActions;
        private int urIndex;

        private CultureInfo culture;

        public string FileName => Path.GetFileNameWithoutExtension(fileName) ?? soundID;
        public string? RawFileName => fileName;

        public Map()
        {
            culture = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            culture.NumberFormat.NumberDecimalSeparator = ".";
        }

        public void MakeCurrent(bool loadAudio = true)
        {
            var editor = MainWindow.Instance;

            editor.CurrentMap?.Save();
            editor.CurrentMap = this;

            editor.Notes = notes.ToList();
            editor.SelectedNotes = selectedNotes.ToList();
            editor.BezierNodes = bezierNodes.ToList();

            editor.TimingPoints = timingPoints.ToList();
            editor.SelectedPoint = selectedPoint;

            editor.Bookmarks = bookmarks.ToList();

            editor.Tempo = tempo;
            editor.Zoom = zoom;

            editor.FileName = fileName;
            editor.SoundID = soundID;

            if (loadAudio)
            {
                editor.LoadAudio(soundID);
                Settings.settings["currentTime"].Max = (float)editor.MusicPlayer.TotalTime.TotalMilliseconds;
                Settings.settings["currentTime"].Step = (float)editor.MusicPlayer.TotalTime.TotalMilliseconds / 2000f;
            }

            editor.MusicPlayer.Tempo = tempo;

            Settings.settings["currentTime"].Value = currentTime;
            Settings.settings["beatDivisor"].Value = beatDivisor;
            Settings.settings["exportOffset"] = exportOffset;

            Settings.settings["mappers"] = mappers;
            Settings.settings["songName"] = songName;
            Settings.settings["difficulty"] = difficulty;
            Settings.settings["useCover"] = useCover;
            Settings.settings["cover"] = cover;
            Settings.settings["customDifficulty"] = customDifficulty;

            editor.UndoRedoManager.Clear();

            for (int i = 0; i < urActions.Count; i++)
                editor.UndoRedoManager.Add(urActions[i].Label, urActions[i].Undo, urActions[i].Redo, false);

            editor.UndoRedoManager._index = urIndex;

            editor.UpdateSelection();
        }

        public void Save()
        {
            var editor = MainWindow.Instance;

            notes = editor.Notes.ToList();
            selectedNotes = editor.SelectedNotes.ToList();
            bezierNodes = editor.BezierNodes.ToList();

            timingPoints = editor.TimingPoints.ToList();
            selectedPoint = editor.SelectedPoint;

            bookmarks = editor.Bookmarks.ToList();

            tempo = editor.Tempo;
            zoom = editor.Zoom;

            fileName = editor.FileName;
            soundID = editor.SoundID;

            currentTime = Settings.settings["currentTime"].Value;
            beatDivisor = Settings.settings["beatDivisor"].Value;
            exportOffset = Settings.settings["exportOffset"];

            mappers = Settings.settings["mappers"];
            songName = Settings.settings["songName"];
            difficulty = Settings.settings["difficulty"];
            useCover = Settings.settings["useCover"];
            cover = Settings.settings["cover"];
            customDifficulty = Settings.settings["customDifficulty"];

            urActions = editor.UndoRedoManager.actions.ToList();
            urIndex = editor.UndoRedoManager._index;

            editor.MusicPlayer.Reset();
        }

        public override string ToString()
        {
            string[] items =
            {
                ParseNotes(),
                ParseTimings(),
                ParseBookmarks(),

                tempo.ToString(culture),
                zoom.ToString(culture),

                fileName ?? "",
                soundID,

                currentTime.ToString(culture),
                beatDivisor.ToString(culture),
                exportOffset.ToString(culture),

                mappers,
                songName,
                difficulty,
                useCover.ToString(culture),
                cover,
                customDifficulty
            };

            return string.Join("\n\0", items);
        }

        public bool FromString(string data)
        {
            try
            {
                Save();

                string[] items = data.Split("\n\0");

                string[] notestr = items[0].Length > 0 ? items[0].Split(",") : Array.Empty<string>();
                string[] timingstr = items[1].Length > 0 ? items[1].Split(",") : Array.Empty<string>();
                string[] bookmarkstr = items[2].Length > 0 ? items[2].Split(",") : Array.Empty<string>();

                for (int i = 0; i < notestr.Length; i++)
                    notes.Add(new(notestr[i], culture));
                for (int i = 0; i < timingstr.Length; i++)
                    timingPoints.Add(new(timingstr[i], culture));
                for (int i = 0; i < bookmarkstr.Length; i++)
                    bookmarks.Add(new(bookmarkstr[i]));

                tempo = float.Parse(items[3], culture);
                zoom = float.Parse(items[4], culture);

                fileName = items[5] != "" ? items[5] : null;
                soundID = items[6];

                currentTime = float.Parse(items[7], culture);
                beatDivisor = float.Parse(items[8], culture);
                exportOffset = int.Parse(items[9]);

                mappers = items[10];
                songName = items[11];
                difficulty = items[12];
                useCover = bool.Parse(items[13]);
                cover = items[14];
                customDifficulty = items[15];

                urIndex = -1;

                return true;
            }
            catch { return false; }
        }

        public bool Close(bool forced, bool fileForced = false, bool shouldSave = true)
        {
            var editor = MainWindow.Instance;

            MakeCurrent(false);
            bool close = !shouldSave || editor.SaveMap(forced, fileForced);

            if (close)
            {
                editor.Maps.Remove(this);
                editor.CacheMaps();
            }

            return close;
        }

        public bool IsSaved()
        {
            MakeCurrent(false);
            return MainWindow.Instance.IsSaved();
        }

        private string ParseNotes()
        {
            string[] notestr = new string[notes.Count];

            for (int i = 0; i < notes.Count; i++)
                notestr[i] = notes[i].ToString(culture);

            string str = string.Join("", notestr);
            return str[Math.Min(1, str.Length)..];
        }

        private string ParseTimings()
        {
            string[] timingstr = new string[timingPoints.Count];

            for (int i = 0; i < timingPoints.Count; i++)
                timingstr[i] = timingPoints[i].ToString(culture);

            string str = string.Join("", timingstr);
            return str[Math.Min(1, str.Length)..];
        }

        private string ParseBookmarks()
        {
            string[] bookmarkstr = new string[bookmarks.Count];

            for (int i = 0; i < bookmarks.Count; i++)
                bookmarkstr[i] = bookmarks[i].ToString();

            string str = string.Join("", bookmarkstr);
            return str[Math.Min(1, str.Length)..];
        }
    }
}
