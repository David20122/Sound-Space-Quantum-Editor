using New_SSQE.EditHistory;
using New_SSQE.ExternalUtils;
using New_SSQE.Maps;
using New_SSQE.Objects;
using New_SSQE.Objects.Managers;
using New_SSQE.Preferences;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace New_SSQE.GUI.Input
{
    internal class KeybindManager
    {
        public static void ParseKeybind(Keys key, bool ctrl, bool alt, bool shift)
        {
            if (MainWindow.Instance.CurrentWindow is not GuiWindowEditor gse)
                return;

            List<string> keybinds = Settings.CompareKeybind(key, ctrl, alt, shift);

            foreach (string keybind in keybinds)
            {
                if (keybind.Contains("gridKey") && !GuiTrack.RenderMapObjects)
                {
                    string rep = keybind.Replace("gridKey", "");
                    string[] xy = rep.Split('|');

                    int x = 2 - int.Parse(xy[0]);
                    int y = 2 - int.Parse(xy[1]);
                    long ms = Timing.GetClosestBeat(Settings.currentTime.Value.Value);

                    Note note = new(x, y, (long)(ms >= 0 ? ms : Settings.currentTime.Value.Value));
                    NoteManager.Add("ADD NOTE", note);

                    if (Settings.autoAdvance.Value)
                        Timing.Advance();

                    continue;
                }

                if (keybind.Contains("pattern"))
                {
                    int index = int.Parse(keybind.Replace("pattern", ""));

                    if (shift)
                        Patterns.StorePattern(index);
                    else if (ctrl)
                        Patterns.ClearPattern(index);
                    else
                        Patterns.RecallPattern(index);

                    continue;
                }

                if (keybind.Contains("extras") && GuiTrack.RenderMapObjects && !GuiTrack.VFXObjects)
                {
                    long ms = Timing.GetClosestBeat(Settings.currentTime.Value.Value);
                    ms = ms > 0 ? ms : (long)Settings.currentTime.Value.Value;
                    MapObject? obj = null;

                    switch (keybind.Replace("extras", ""))
                    {
                        case "Beat":
                            obj = new Beat(ms);
                            break;
                    }

                    if (obj != null)
                        SpecialObjectManager.Add($"ADD {obj.Name?.ToUpper()}", obj);

                    continue;
                }

                switch (keybind)
                {
                    case "selectAll":
                        if (GuiTrack.RenderMapObjects && GuiTrack.VFXObjects)
                            CurrentMap.VfxObjects.Selected = new(CurrentMap.VfxObjects);
                        else if (GuiTrack.RenderMapObjects)
                            CurrentMap.SpecialObjects.Selected = new(CurrentMap.SpecialObjects);
                        else
                        {
                            CurrentMap.SelectedPoint = null;
                            CurrentMap.Notes.Selected = new(CurrentMap.Notes);
                        }

                        break;

                    case "save":
                        if (MapManager.Save(true))
                            gse.ShowToast("SAVED", Settings.color1.Value);

                        break;

                    case "saveAs":
                        if (MapManager.Save(true, true))
                            gse.ShowToast("SAVED", Settings.color1.Value);

                        break;

                    case "undo":
                        UndoRedoManager.Undo();

                        break;

                    case "redo":
                        UndoRedoManager.Redo();

                        break;

                    case "copy":
                        try
                        {
                            if (CurrentMap.Notes.Selected.Count > 0)
                            {
                                List<MapObject> copied = CurrentMap.Notes.Selected.Cast<MapObject>().ToList();
                                Clipboard.SetData(copied);

                                gse.ShowToast("COPIED NOTES", Settings.color1.Value);
                            }
                            else if (CurrentMap.VfxObjects.Selected.Count > 0)
                            {
                                List<MapObject> copied = CurrentMap.VfxObjects.Selected.ToList();
                                Clipboard.SetData(copied);

                                gse.ShowToast("COPIED OBJECTS", Settings.color1.Value);
                            }
                            else if (CurrentMap.SpecialObjects.Selected.Count > 0)
                            {
                                List<MapObject> copied = CurrentMap.SpecialObjects.Selected.ToList();
                                Clipboard.SetData(copied);

                                gse.ShowToast("COPIED OBJECTS", Settings.color1.Value);
                            }
                        }
                        catch (Exception ex)
                        {
                            Logging.Register("Failed to copy objects", LogSeverity.WARN, ex);
                            gse.ShowToast("FAILED TO COPY", Settings.color1.Value);
                        }

                        break;

                    case "paste":
                        try
                        {
                            List<MapObject> copied = Clipboard.GetData().ToList();

                            if (copied.Count > 0)
                            {
                                bool isNote = copied.FirstOrDefault() is Note;

                                long offset = copied.Min(n => n.Ms);
                                long max = copied.Max(n => n.Ms);

                                copied.ForEach(n => n.Ms = (long)MathHelper.Clamp(Settings.currentTime.Value.Value + n.Ms - offset, 0, Settings.currentTime.Value.Max));

                                if (isNote && !GuiTrack.RenderMapObjects)
                                {
                                    List<Note> copiedAsNotes = copied.Cast<Note>().ToList();

                                    if (Settings.pasteReversed.Value)
                                    {
                                        for (int i = 0; i < copiedAsNotes.Count / 2; i++)
                                        {
                                            Note cur = copiedAsNotes[i];
                                            Note opp = copiedAsNotes[copiedAsNotes.Count - 1 - i];

                                            Vector2 curPos = (cur.X, cur.Y);
                                            Vector2 oppPos = (opp.X, opp.Y);

                                            cur.X = oppPos.X;
                                            cur.Y = oppPos.Y;

                                            opp.X = curPos.X;
                                            opp.Y = curPos.Y;
                                        }
                                    }

                                    if (Settings.applyOnPaste.Value)
                                    {
                                        if (float.TryParse(gse.RotateBox.Text, out float deg) && float.TryParse(gse.ScaleBox.Text, out float scale))
                                        {
                                            foreach (Note note in copiedAsNotes)
                                            {
                                                Patterns.Rotate(note, deg);
                                                Patterns.Scale(note, scale);
                                            }
                                        }
                                    }

                                    NoteManager.Add("PASTE NOTE[S]", copiedAsNotes);
                                }
                                else if (!isNote && GuiTrack.RenderMapObjects)
                                {
                                    (List<MapObject> vfxCopy, List<MapObject> specialCopy) = CurrentMap.SplitVFXSpecial(copied);

                                    if (GuiTrack.VFXObjects)
                                        VfxObjectManager.Add("PASTE OBJECT[S]", vfxCopy);
                                    else
                                        SpecialObjectManager.Add("PASTE OBJECT[S]", specialCopy);
                                }

                                if (Settings.jumpPaste.Value)
                                {
                                    Settings.currentTime.Value.Value += max - offset;
                                    if (Settings.autoAdvance.Value)
                                        Timing.Advance();
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Logging.Register("Failed to paste objects", LogSeverity.WARN, ex);
                            gse.ShowToast("FAILED TO PASTE", Settings.color1.Value);
                        }

                        break;

                    case "cut":
                        try
                        {
                            if (CurrentMap.Notes.Selected.Count > 0)
                            {
                                List<Note> copied = CurrentMap.Notes.Selected.ToList();
                                Clipboard.SetData(copied.Cast<MapObject>().ToList());

                                NoteManager.Remove("CUT NOTE[S]", copied);
                                gse.ShowToast("CUT NOTES", Settings.color1.Value);
                            }
                            else if (CurrentMap.Notes.Selected.Count > 0)
                            {
                                List<MapObject> copied = CurrentMap.VfxObjects.Selected.ToList();
                                Clipboard.SetData(copied);

                                VfxObjectManager.Remove("CUT OBJECT[S]", copied);
                                gse.ShowToast("CUT OBJECTS", Settings.color1.Value);
                            }
                            else if (CurrentMap.SpecialObjects.Selected.Count > 0)
                            {
                                List<MapObject> copied = CurrentMap.SpecialObjects.Selected.ToList();
                                Clipboard.SetData(copied);

                                SpecialObjectManager.Remove("CUT OBJECT[S]", copied);
                                gse.ShowToast("CUT OBJECTS", Settings.color1.Value);
                            }
                        }
                        catch (Exception ex)
                        {
                            Logging.Register("Failed to cut objects", LogSeverity.WARN, ex);
                            gse.ShowToast("FAILED TO CUT", Settings.color1.Value);
                        }

                        break;

                    case "delete":
                        if (CurrentMap.Notes.Selected.Count > 0)
                            NoteManager.Remove("DELETE NOTE[S]");
                        else if (CurrentMap.SelectedPoint != null)
                            PointManager.Remove("DELETE POINT");
                        else if (CurrentMap.VfxObjects.Selected.Count > 0)
                            VfxObjectManager.Remove("DELETE OBJECT[S]");
                        else if (CurrentMap.SpecialObjects.Selected.Count > 0)
                            SpecialObjectManager.Remove("DELETE OBJECT[S]");

                        break;

                    case "hFlip":
                        if (CurrentMap.Notes.Selected.Count > 0)
                        {
                            gse.ShowToast("HORIZONTAL FLIP", Settings.color1.Value);
                            NoteManager.Edit("HORIZONTAL FLIP", Patterns.HorizontalFlip);
                        }

                        break;

                    case "vFlip":
                        if (CurrentMap.Notes.Selected.Count > 0)
                        {
                            gse.ShowToast("VERTICAL FLIP", Settings.color1.Value);
                            NoteManager.Edit("VERTICAL FLIP", Patterns.VerticalFlip);
                        }

                        break;

                    case "switchClickTool":
                        Settings.selectTool.Value ^= true;

                        break;

                    case "quantum":
                        Settings.enableQuantum.Value ^= true;

                        break;

                    case "openTimings":
                        TimingsWindow.ShowWindow();

                        break;

                    case "openBookmarks":
                        BookmarksWindow.ShowWindow();

                        break;

                    case "storeNodes":
                        if (CurrentMap.Notes.Selected.Count > 1)
                            CurrentMap.BezierNodes = CurrentMap.Notes.Selected.ToList();

                        break;

                    case "drawBezier":
                        Patterns.RunBezier();

                        break;

                    case "anchorNode":
                        if (CurrentMap.Notes.Selected.Count > 0)
                            NoteManager.Edit("ANCHOR NODE[S]", n => n.Anchored ^= true);

                        break;

                    case "openDirectory":
                        Platform.OpenDirectory();

                        break;

                    case "exportSSPM":
                        ExportSSPM.ShowWindow();

                        break;

                    case "createBPM":
                        if (CurrentMap.Notes.Selected.Count == 2)
                        {
                            Note first = CurrentMap.Notes.Selected[0];
                            Note second = CurrentMap.Notes.Selected[1];

                            long minMs = Math.Min(first.Ms, second.Ms);
                            long maxMs = Math.Max(first.Ms, second.Ms);
                            float bpm = (float)Math.Round(60000f / (maxMs - minMs) * 4) / 4;

                            if (bpm > 0)
                                PointManager.Add("CREATE TIMING POINT", new(bpm, minMs));
                        }

                        break;
                }
            }
        }
    }
}
