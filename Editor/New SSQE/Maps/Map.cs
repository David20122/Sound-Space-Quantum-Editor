using New_SSQE.Audio;
using New_SSQE.EditHistory;
using New_SSQE.Objects;
using New_SSQE.Objects.Other;
using New_SSQE.Preferences;

namespace New_SSQE.Maps
{
    internal class Map
    {
        private List<Note> notes = new();
        private List<MapObject> vfxObjects = new();
        private List<MapObject> specialObjects = new();

        private List<Note> bezierNodes = new();
        private List<Bookmark> bookmarks = new();
        private List<TimingPoint> timingPoints = new();

        private string? fileName = null;
        private string soundID = "-1";

        private float tempo = 0;
        private float zoom = 0;
        private float currentTime = 0;
        private float beatDivisor = 0;
        private long exportOffset = 0;

        private string mappers = "";
        private string songName = "";
        private string difficulty = "";
        private bool useCover = false;
        private string cover = "";
        private string customDifficulty = "";
        private float rating = 0;
        private bool useVideo = false;
        private string video = "";

        private string songOffset = "";
        private string songTitle = "";
        private string songArtist = "";
        private string mapCreator = "";
        private string mapCreatorPersonalLink = "";
        private string previewStartTime = "";
        private string previewDuration = "";
        private string novaCover = "";
        private string novaIcon = "";

        private List<URAction> urActions = new();
        private int urIndex = 0;

        public string FileID => Path.GetFileNameWithoutExtension(fileName) ?? soundID;
        public string? FileName => fileName;

        public bool IsSaved
        {
            get
            {
                Load(false);
                return CurrentMap.IsSaved;
            }
        }

        public void Load(bool loadAudio = true)
        {
            CurrentMap.LoadedMap?.Save();
            CurrentMap.LoadedMap = this;

            CurrentMap.VfxObjects = new(vfxObjects);
            CurrentMap.SelectedObjDuration = null;

            CurrentMap.SpecialObjects = new(specialObjects);

            CurrentMap.Notes = new(notes);
            CurrentMap.BezierNodes = bezierNodes.ToList();
            CurrentMap.TimingPoints = timingPoints.ToList();

            CurrentMap.Bookmarks = bookmarks.ToList();

            CurrentMap.FileName = fileName;
            CurrentMap.SoundID = soundID;
            CurrentMap.Zoom = zoom;

            if (loadAudio)
            {
                MapManager.LoadAudio(soundID);
                MusicPlayer.Volume = Settings.masterVolume.Value.Value;
                Settings.currentTime.Value.Max = (float)MusicPlayer.TotalTime.TotalMilliseconds;
                Settings.currentTime.Value.Step = (float)MusicPlayer.TotalTime.TotalMilliseconds / 2000f;
            }

            Settings.tempo.Value.Value = tempo;
            CurrentMap.SetTempo(tempo);

            Settings.currentTime.Value.Value = currentTime;
            Settings.beatDivisor.Value.Value = beatDivisor;
            Settings.exportOffset.Value = exportOffset;

            Settings.mappers.Value = mappers;
            Settings.songName.Value = songName;
            Settings.difficulty.Value = difficulty;
            Settings.useCover.Value = useCover;
            Settings.cover.Value = cover;
            Settings.customDifficulty.Value = customDifficulty;
            Settings.rating.Value = rating;
            Settings.useVideo.Value = useVideo;
            Settings.video.Value = video;

            Settings.songOffset.Value = songOffset;
            Settings.songTitle.Value = songTitle;
            Settings.songArtist.Value = songArtist;
            Settings.mapCreator.Value = mapCreator;
            Settings.mapCreatorPersonalLink.Value = mapCreatorPersonalLink;
            Settings.previewStartTime.Value = previewStartTime;
            Settings.previewDuration.Value = previewDuration;
            Settings.novaCover.Value = novaCover;
            Settings.novaIcon.Value = novaIcon;

            UndoRedoManager.Clear();

            for (int i = 0; i < urActions.Count; i++)
                UndoRedoManager.Add(urActions[i].Label, urActions[i].Undo, urActions[i].Redo, false);

            UndoRedoManager._index = urIndex;
        }

        public void Save()
        {
            vfxObjects = CurrentMap.VfxObjects.ToList();
            specialObjects = CurrentMap.SpecialObjects.ToList();

            notes = CurrentMap.Notes.ToList();
            bezierNodes = CurrentMap.BezierNodes.ToList();
            timingPoints = CurrentMap.TimingPoints.ToList();

            bookmarks = CurrentMap.Bookmarks.ToList();

            tempo = Settings.tempo.Value.Value;
            zoom = CurrentMap.Zoom;

            fileName = CurrentMap.FileName;
            soundID = CurrentMap.SoundID;

            currentTime = Settings.currentTime.Value.Value;
            beatDivisor = Settings.beatDivisor.Value.Value;
            exportOffset = (long)Settings.exportOffset.Value;

            mappers = Settings.mappers.Value;
            songName = Settings.songName.Value;
            difficulty = Settings.difficulty.Value;
            useCover = Settings.useCover.Value;
            cover = Settings.cover.Value;
            customDifficulty = Settings.customDifficulty.Value;
            rating = Settings.rating.Value;
            useVideo = Settings.useVideo.Value;
            video = Settings.video.Value;

            songOffset = Settings.songOffset.Value;
            songTitle = Settings.songTitle.Value;
            songArtist = Settings.songArtist.Value;
            mapCreator = Settings.mapCreator.Value;
            mapCreatorPersonalLink = Settings.mapCreatorPersonalLink.Value;
            previewStartTime = Settings.previewStartTime.Value;
            previewDuration = Settings.previewDuration.Value;
            novaCover = Settings.novaCover.Value;
            novaIcon = Settings.novaIcon.Value;

            urActions = UndoRedoManager.actions.ToList();
            urIndex = UndoRedoManager._index;

            MusicPlayer.Reset();
        }

        public bool Close(bool forced, bool fileForced = false, bool shouldSave = true)
        {
            Load(false);
            bool close = !shouldSave || MapManager.Save(forced, fileForced);

            if (close)
            {
                MapManager.Cache.Remove(this);
                MapManager.SaveCache();
            }

            return close;
        }

        public override string ToString()
        {
            string[] items =
            {
                ParseNotes(),
                ParseTimings(),
                ParseBookmarks(),

                tempo.ToString(Program.Culture),
                zoom.ToString(Program.Culture),

                fileName ?? "",
                soundID,

                currentTime.ToString(Program.Culture),
                beatDivisor.ToString(Program.Culture),
                exportOffset.ToString(Program.Culture),

                mappers,
                songName,
                difficulty,
                useCover.ToString(Program.Culture),
                cover,
                customDifficulty,

                songOffset,
                songTitle,
                songArtist,
                mapCreator,
                mapCreatorPersonalLink,
                previewStartTime,
                previewDuration,
                novaCover,
                novaIcon,

                rating.ToString(Program.Culture),
                useVideo.ToString(Program.Culture),
                video,

                ParseMapObjects()
            };

            return string.Join("\n\0", items);
        }

        private string ParseMapObjects()
        {
            string[] objStr = new string[vfxObjects.Count + specialObjects.Count];

            for (int i = 0; i < vfxObjects.Count; i++)
                objStr[i] = $"{vfxObjects[i].ID}|{vfxObjects[i].ToString()}";
            for (int i = 0; i < specialObjects.Count; i++)
                objStr[i + vfxObjects.Count] = $"{specialObjects[i].ID}|{specialObjects[i].ToString()}";

            return string.Join(',', objStr);
        }

        private string ParseNotes()
        {
            string[] notestr = new string[notes.Count];

            for (int i = 0; i < notes.Count; i++)
                notestr[i] = notes[i].ToString();

            return string.Join(',', notestr);
        }

        private string ParseTimings()
        {
            string[] timingstr = new string[timingPoints.Count];

            for (int i = 0; i < timingPoints.Count; i++)
                timingstr[i] = timingPoints[i].ToString();

            return string.Join(',', timingstr);
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
