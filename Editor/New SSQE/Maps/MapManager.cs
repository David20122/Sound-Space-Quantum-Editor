using New_SSQE.Audio;
using New_SSQE.EditHistory;
using New_SSQE.ExternalUtils;
using New_SSQE.FileParsing;
using New_SSQE.GUI;
using New_SSQE.Misc.Dialogs;
using New_SSQE.Misc.Network;
using New_SSQE.Misc.Static;
using New_SSQE.Objects;
using New_SSQE.Preferences;
using System.IO.Compression;

namespace New_SSQE.Maps
{
    internal static class MapManager
    {
        public static List<Map> Cache = new();
        private static readonly string cacheFile = $"{Assets.TEMP}\\cache.txt";

        public static Map? FromCache(string data)
        {
            try
            {
                CurrentMap.LoadedMap?.Save();
                CurrentMap.Reset();

                string[] items = data.Split("\n\0");

                if (items.Length > 28)
                {
                    string[] objStr = items[28].Length > 0 ? items[28].Split(",") : Array.Empty<string>();

                    foreach (string item in objStr)
                    {
                        MapObject? obj = MOParser.Parse(null, item.Split('|'));

                        if (obj != null && Parser.VfxLookup[obj.ID])
                            CurrentMap.VfxObjects.Add(obj);
                        else if (obj != null)
                            CurrentMap.SpecialObjects.Add(obj);
                    }
                }

                string[] notestr = items[0].Length > 0 ? items[0].Split(",") : Array.Empty<string>();
                string[] timingstr = items[1].Length > 0 ? items[1].Split(",") : Array.Empty<string>();
                string[] bookmarkstr = items[2].Length > 0 ? items[2].Split(",") : Array.Empty<string>();

                for (int i = 0; i < notestr.Length; i++)
                    CurrentMap.Notes.Add(new(notestr[i]));
                for (int i = 0; i < timingstr.Length; i++)
                    CurrentMap.TimingPoints.Add(new(timingstr[i]));
                for (int i = 0; i < bookmarkstr.Length; i++)
                    CurrentMap.Bookmarks.Add(new(bookmarkstr[i]));

                Settings.tempo.Value.Value = float.Parse(items[3], Program.Culture);
                CurrentMap.Zoom = float.Parse(items[4], Program.Culture);

                CurrentMap.FileName = items[5] != "" ? items[5] : null;
                CurrentMap.SoundID = items[6];

                Settings.currentTime.Value.Value = float.Parse(items[7], Program.Culture);
                Settings.beatDivisor.Value.Value = float.Parse(items[8], Program.Culture);
                Settings.exportOffset.Value = long.Parse(items[9]);

                Settings.mappers.Value = items[10];
                Settings.songName.Value = items[11];
                Settings.difficulty.Value = items[12];
                Settings.useCover.Value = bool.Parse(items[13]);
                Settings.cover.Value = items[14];
                Settings.customDifficulty.Value = items[15];

                Settings.songOffset.Value = items[16];
                Settings.songTitle.Value = items[17];
                Settings.songArtist.Value = items[18];
                Settings.mapCreator.Value = items[19];
                Settings.mapCreatorPersonalLink.Value = items[20];
                Settings.previewStartTime.Value = items[21];
                Settings.previewDuration.Value = items[22];
                Settings.novaCover.Value = items[23];
                Settings.novaIcon.Value = items[24];

                Settings.rating.Value = float.Parse(items[25], Program.Culture);
                Settings.useVideo.Value = bool.Parse(items[26]);
                Settings.video.Value = items[27];

                Map map = new();
                map.Save();

                return map;
            }
            catch { return null; }
        }

        public static void SaveCache()
        {
            List<string> data = new();
            foreach (Map map in Cache)
                data.Add(map.ToString());

            string text = string.Join("\r\0", data);
            File.WriteAllText(cacheFile, text);
        }

        public static void LoadCache()
        {
            if (File.Exists(cacheFile) && !string.IsNullOrWhiteSpace(File.ReadAllText(cacheFile)))
            {
                Cache.Clear();
                string[] data = File.ReadAllText(cacheFile).Split("\r\0");

                for (int i = 0; i < data.Length; i++)
                {
                    Map? map = FromCache(data[i]);

                    if (map != null)
                        Cache.Add(map);
                }
            }
        }



        public static void Load(Map map, bool loadAudio = true)
        {
            map.Load(loadAudio);
            MainWindow.Instance.SwitchWindow(new GuiWindowEditor());
        }

        public static bool Load(string pathOrData, bool file = false, bool autosave = false, bool downloadFile = false)
        {
            foreach (Map map in Cache)
            {
                if (file && pathOrData == map.FileName)
                {
                    Load(map);
                    return true;
                }
            }

            CurrentMap.Reset();

            CurrentMap.FileName = file ? pathOrData : null;

            UndoRedoManager.Clear();
            MusicPlayer.Reset();

            if (file)
            {
                string? old = CurrentMap.FileName;
                file = false;
                CurrentMap.FileName = null;

                switch (Path.GetExtension(pathOrData))
                {
                    case ".sspm":
                        pathOrData = Parser.ParseSSPM(pathOrData);
                        break;
                    case ".osu":
                        pathOrData = Parser.ParseOSU(pathOrData);
                        break;
                    case ".nch":
                        pathOrData = Parser.ParseNOVA(pathOrData);
                        break;
                    case ".npk":
                        string tempNPK = $"{Assets.TEMP}\\nova";
                        Directory.CreateDirectory(tempNPK);
                        foreach (string temp in Directory.GetFiles(tempNPK))
                            File.Delete(temp);
                        ZipFile.ExtractToDirectory(pathOrData, tempNPK);

                        pathOrData = Parser.ParseNOVA($"{tempNPK}\\chart.nch");
                        break;
                    case ".phxm":
                        pathOrData = Parser.ParsePHXM(pathOrData);
                        break;
                    case ".phz":
                        string tempPHZ = $"{Assets.TEMP}\\pulsus";
                        Directory.CreateDirectory(tempPHZ);
                        foreach (string temp in Directory.GetFiles(tempPHZ))
                            File.Delete(temp);
                        ZipFile.ExtractToDirectory(pathOrData, tempPHZ);

                        pathOrData = Parser.ParsePHZ(Directory.GetFiles(tempPHZ).FirstOrDefault() ?? "");
                        break;
                    case ".json":
                        if (Parser.IsValidPHZ(pathOrData))
                            pathOrData = Parser.ParsePHZ(pathOrData);
                        else
                            return false;
                        break;
                    case not ".txt":
                        return false;
                    case ".txt":
                        file = true;
                        CurrentMap.FileName = old;
                        break;
                }
            }
            if (pathOrData == "")
                return false;

            string data = file ? File.ReadAllText(pathOrData) : pathOrData;

            try
            {
                if (!downloadFile)
                {
                    while (true)
                        data = WebClient.DownloadString(data);
                }
                else
                {
                    string path = $"{Assets.TEMP}\\tempdownload.sspm";

                    WebClient.DownloadFile(data, path);
                    data = Parser.ParseSSPM(path);
                }
            }
            catch { }

            try
            {
                string id = Parser.Parse(data);

                CurrentMap.Notes.Sort();

                if (LoadAudio(id))
                {
                    CurrentMap.SoundID = id;

                    Settings.currentTime.Value = new SliderSetting(0f, (float)MusicPlayer.TotalTime.TotalMilliseconds, (float)MusicPlayer.TotalTime.TotalMilliseconds / 2000f);
                    Settings.beatDivisor.Value.Value = 3f;
                    Settings.tempo.Value.Value = 0.9f;
                    Settings.exportOffset.Value = 0;

                    if (file)
                    {
                        string? propertyFile = Path.ChangeExtension(CurrentMap.FileName, ".ini");

                        if (File.Exists(propertyFile))
                            Parser.ParseINI(File.ReadAllText(propertyFile));
                    }
                    else if (autosave)
                        Parser.ParseINI(Settings.autosavedProperties.Value);

                    CurrentMap.SortTimings();
                    CurrentMap.SortBookmarks();

                    CurrentMap.LoadedMap?.Save();
                    CurrentMap.LoadedMap = new();
                    CurrentMap.LoadedMap.Save();

                    Cache.Add(CurrentMap.LoadedMap);
                    SaveCache();

                    MainWindow.Instance.SwitchWindow(new GuiWindowEditor());
                }
                else
                    CurrentMap.LoadedMap = null;
            }
            catch (Exception ex)
            {
                Logging.Register($"Failed to load map data", LogSeverity.WARN, ex);
                MessageBox.Show($"Failed to load map data: {ex.Message}\n\nExit and check '*\\logs.txt' for more info", MBoxIcon.Warning, MBoxButtons.OK);

                if (CurrentMap.LoadedMap != null)
                    Cache.Remove(CurrentMap.LoadedMap);
                CurrentMap.LoadedMap = null;

                return false;
            }

            return CurrentMap.SoundID != "-1";
        }



        public static bool Save(bool forced, bool fileForced = false, bool reload = true)
        {
            if (CurrentMap.FileName != null && !File.Exists(CurrentMap.FileName))
                CurrentMap.FileName = null;

            if (CurrentMap.FileName != null)
                Settings.lastFile.Value = CurrentMap.FileName;
            Settings.Save(reload);

            string tempSID = CurrentMap.SoundID;
            string? tempFN = CurrentMap.FileName;

            string data = Parser.Save(CurrentMap.SoundID);

            if (forced || (CurrentMap.FileName == null && (CurrentMap.Notes.Count > 0 || CurrentMap.TimingPoints.Count > 0)) || (CurrentMap.FileName != null && data != File.ReadAllText(CurrentMap.FileName)))
            {
                DialogResult result = DialogResult.No;

                if (!forced)
                    result = MessageBox.Show($"{Path.GetFileNameWithoutExtension(CurrentMap.FileName) ?? "Untitled Song"} ({CurrentMap.SoundID})\n\nWould you like to save before closing?", MBoxIcon.Warning, MBoxButtons.Yes_No_Cancel);

                if (forced || result == DialogResult.Yes)
                {
                    if (CurrentMap.FileName == null || fileForced)
                    {
                        DialogResult dialog = new SaveFileDialog()
                        {
                            Title = "Save Map As",
                            Filter = "Text Documents(*.txt)|*.txt"
                        }.RunWithSetting(Settings.defaultPath, out string fileName);

                        if (dialog == DialogResult.OK)
                        {
                            File.WriteAllText(fileName, data);
                            SaveProperties(fileName);
                            CurrentMap.FileName = fileName;

                            Logging.Register($"Successfully saved to file: {CurrentMap.FileName}");
                        }
                        else
                            return false;
                    }
                    else
                    {
                        File.WriteAllText(CurrentMap.FileName, data);
                        SaveProperties(CurrentMap.FileName);

                        Logging.Register($"Successfully saved to file: {CurrentMap.FileName}");
                    }
                }
                else if (result == DialogResult.Cancel)
                    return false;
            }

            Logging.Register($"Save returned true with fields: {forced} | {fileForced} | {reload}\n{tempSID} | {tempFN ?? ""}");

            return true;
        }



        public static void ImportProperties(string? file = null)
        {
            if (file == null)
            {
                DialogResult result = new OpenFileDialog()
                {
                    Title = "Select .ini File",
                    Filter = "Map Property Files (*.ini)|*.ini"
                }.RunWithSetting(Settings.defaultPath, out string fileName);

                if (result == DialogResult.OK)
                    file = fileName;
                else
                    return;
            }

            Parser.ParseINI(File.ReadAllText(file));

            if (MainWindow.Instance.CurrentWindow is GuiWindowEditor editor)
                editor.Timeline.GenerateOffsets();
        }

        public static void SaveProperties(string filePath)
        {
            string file = Path.ChangeExtension(filePath, ".ini");

            File.WriteAllText(file, Parser.SaveINI());
            Settings.lastFile.Value = filePath;
        }



        public static bool ImportAudio(string id, bool create = false)
        {
            DialogResult result = new OpenFileDialog()
            {
                Title = "Select Audio File",
                Filter = $"Audio Files ({MusicPlayer.SupportedExtensionsString})|{MusicPlayer.SupportedExtensionsString}"
            }.RunWithSetting(Settings.audioPath, out string fileName);

            if (result == DialogResult.OK)
            {
                if (string.IsNullOrWhiteSpace(id))
                    id = Path.GetFileNameWithoutExtension(fileName);
                id = Exporting.FixID(id);

                if (fileName != $"{Assets.CACHED}\\{id}.asset")
                    File.Copy(fileName, $"{Assets.CACHED}\\{id}.asset", true);

                if (create)
                    CurrentMap.SoundID = id;

                return MusicPlayer.Load($"{Assets.CACHED}\\{id}.asset");
            }

            return false;
        }

        public static bool LoadAudio(string id)
        {
            try
            {
                if (!File.Exists($"{Assets.CACHED}\\{id}.asset"))
                {
                    if (Settings.skipDownload.Value)
                    {
                        DialogResult message = MessageBox.Show($"No asset with id '{id}' is present in cache.\n\nWould you like to import a file with this id?", MBoxIcon.Warning, MBoxButtons.OK_Cancel);

                        return message == DialogResult.OK && ImportAudio(id);
                    }
                    else
                        WebClient.DownloadFile($"https://assetdelivery.roblox.com/v1/asset/?id={id}", $"{Assets.CACHED}\\{id}.asset", FileSource.Roblox);
                }

                return MusicPlayer.Load($"{Assets.CACHED}\\{id}.asset");
            }
            catch (Exception e)
            {
                Logging.Register("Failed to load audio", LogSeverity.WARN, e);
                DialogResult message = MessageBox.Show($"Failed to download asset with id '{id}':\n\n{e.Message}\n\nWould you like to import a file with this id instead?", MBoxIcon.Error, MBoxButtons.OK_Cancel);

                if (message == DialogResult.OK)
                    return ImportAudio(id);
            }

            return false;
        }



        private static long currentAutosave;

        public static void Autosave(bool forced = false)
        {
            if (MainWindow.Instance.CurrentWindow is GuiWindowEditor editor && (forced || CurrentMap.Notes.Count > 0))
            {
                if (CurrentMap.FileName == null)
                {
                    Settings.autosavedFile.Value = Parser.Save(CurrentMap.SoundID);
                    Settings.autosavedProperties.Value = Parser.SaveINI();
                    Settings.Save(false);

                    editor.ShowToast("AUTOSAVED", Settings.color1.Value);
                }
                else if (Save(true, false, false))
                    editor.ShowToast("AUTOSAVED", Settings.color1.Value);
            }
        }

        public static void BeginAutosaveLoop(long time)
        {
            if (Settings.enableAutosave.Value)
            {
                currentAutosave = time;

                Task.Run(() =>
                {
                    while (currentAutosave == time)
                    {
                        Thread.Sleep((int)(Settings.autosaveInterval.Value * 60000f));
                        if (currentAutosave == time)
                            Autosave();
                    }
                });
            }
        }
    }
}
