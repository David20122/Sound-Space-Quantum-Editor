using New_SSQE.Maps;
using New_SSQE.Objects;
using New_SSQE.Objects.Managers;
using New_SSQE.Preferences;
using OpenTK.Mathematics;

namespace New_SSQE.FileParsing.Formats
{
    internal class TXT
    {
        public static string Parse(string data)
        {
            List<Note> notes = CurrentMap.Notes;
            List<MapObject> vfxObjects = CurrentMap.VfxObjects;
            List<MapObject> specObjects = CurrentMap.SpecialObjects;
            List<TimingPoint> timingPoints = CurrentMap.TimingPoints;

            string[] split = data.Split('`');

            if (split.Length == 1)
            {
                string[] mapData = split[0].Split(',');

                for (int i = 1; i < mapData.Length; i++)
                {
                    if (!string.IsNullOrWhiteSpace(mapData[i]))
                    {
                        try
                        {
                            string[] subData = mapData[i].Split('|');
                            notes.Add(new($"{subData[2]}|{subData[0]}|{subData[1]}"));
                        }
                        catch (Exception ex)
                        {
                            throw new AggregateException($"Note failed to parse: {mapData[i]}", ex);
                        }
                    }
                }

                return mapData[0];
            }
            else
            {
                string fileSignature = split[0];

                switch (fileSignature)
                {
                    case "ssmapv2":
                        string id = split[1];
                        int diffID = int.Parse(split[2]);
                        string diffName = Exporting.Difficulties.Keys.ToArray()[diffID];
                        string customDiff = split[3];

                        Settings.difficulty.Value = diffName;
                        Settings.customDifficulty.Value = customDiff == diffName ? "" : customDiff;

                        string[] objSets = split[4].Split('/');

                        for (int i = 0; i < objSets.Length; i++)
                        {
                            if (string.IsNullOrWhiteSpace(objSets[i]))
                                continue;

                            string[] objs = objSets[i].Split(',');

                            Parser.ResetDecode();

                            for (int j = 0; j < objs.Length; j++)
                            {
                                MapObject? obj = MOParser.Parse(i, objs[j].Split('|'));

                                if (obj != null)
                                {
                                    if (obj is Note n)
                                        notes.Add(n);
                                    else if (obj is TimingPoint p)
                                        timingPoints.Add(p);
                                    else if (Parser.VfxLookup[i])
                                        vfxObjects.Add(obj);
                                    else
                                        specObjects.Add(obj);

                                    obj.Ms = Parser.DecodeTimestamp(obj.Ms);
                                }
                            }
                        }

                        return id;

                    default:
                        throw new NotImplementedException($"Map format not supported - ${fileSignature}");
                }
            }
        }

        public static string SaveLegacy(string id, bool copy = false, bool applyOffset = true)
        {
            List<Note> notes = CurrentMap.Notes;

            long offset = (long)Settings.exportOffset.Value;
            string[] final = new string[notes.Count + 1];
            final[0] = id.Replace(",", "");

            for (int i = 0; i < notes.Count; i++)
            {
                Note clone = notes[i].Clone();

                if (applyOffset)
                    clone.Ms += offset;
                if (copy)
                {
                    clone.X = MathHelper.Clamp(clone.X, -0.85f, 2.85f);
                    clone.Y = MathHelper.Clamp(clone.Y, -0.85f, 2.85f);

                    clone.Ms = (long)MathHelper.Clamp(clone.Ms, 0, Settings.currentTime.Value.Max);
                }

                double x = Math.Round(clone.X, 2);
                double y = Math.Round(clone.Y, 2);

                final[i + 1] = $"{x.ToString(Program.Culture)}|{y.ToString(Program.Culture)}|{clone.Ms}";
            }

            return string.Join(',', final);
        }

        public static string Save(string audioId, bool copy = false, bool applyOffset = true)
        {
            return SaveLegacy(audioId, copy, applyOffset);

            ObjectList<Note> notes = CurrentMap.Notes;
            ObjectList<MapObject> vfxObjects = new(CurrentMap.VfxObjects.OrderBy(n => n.ID).ThenBy(n => n.Ms));
            ObjectList<MapObject> specObjects = new(CurrentMap.SpecialObjects.OrderBy(n => n.ID).ThenBy(n => n.Ms));
            List<TimingPoint> timingPoints = CurrentMap.TimingPoints;

            long staticOffset = (long)Settings.exportOffset.Value;

            List<string> final = new()
            {
                "ssmapv2", audioId,
                Exporting.Difficulties[Settings.difficulty.Value].ToString(),
                string.IsNullOrWhiteSpace(Settings.customDifficulty.Value) ? Settings.difficulty.Value : Settings.customDifficulty.Value
            };

            Parser.ResetEncode();

            string SaveObject(MapObject obj)
            {
                MapObject clone = obj.Clone();

                if (applyOffset)
                    clone.Ms += staticOffset;
                if (copy && clone is Note n)
                {
                    n.X = MathHelper.Clamp(n.X, -0.85f, 2.85f);
                    n.Y = MathHelper.Clamp(n.Y, -0.85f, 2.85f);

                    n.Ms = (long)MathHelper.Clamp(n.Ms, 0, Settings.currentTime.Value.Max);
                }

                clone.Ms = Parser.EncodeTimestamp(clone.Ms);

                return clone.ToString();
            }

            List<string> objStrs = new();

            string[] noteSet = new string[notes.Count];
            for (int i = 0; i < notes.Count; i++)
                noteSet[i] = SaveObject(notes[i]);
            objStrs.Add(string.Join(',', noteSet));

            Parser.ResetEncode();

            string[] timingSet = new string[timingPoints.Count];
            for (int i = 0; i < timingPoints.Count; i++)
                timingSet[i] = SaveObject(timingPoints[i]);
            objStrs.Add(string.Join(',', timingSet));

            int id = 1;
            int index = 0;

            List<string> tempSet = new();

            while (index < vfxObjects.Count)
            {
                MapObject obj = vfxObjects[index];

                if (obj.ID != id)
                {
                    for (int i = id; i < obj.ID; i++)
                    {
                        if (i > 1)
                            objStrs.Add(string.Join(',', tempSet));
                        tempSet = new();
                    }

                    id = obj.ID;

                    Parser.ResetEncode();
                    continue;
                }

                tempSet.Add(SaveObject(obj));

                index++;
            }

            if (tempSet.Count > 0)
                objStrs.Add(string.Join(',', tempSet));
            tempSet = new();

            int count = objStrs.Count - 1;
            int maxId = count;
            id = 1;

            Parser.ResetEncode();
            index = 0;

            while (index < specObjects.Count)
            {
                MapObject obj = specObjects[index];

                if (obj.ID > maxId)
                {
                    for (int i = maxId; i < obj.ID; i++)
                    {
                        if (i > count)
                            objStrs.Add(string.Join(',', tempSet));
                        tempSet = new();
                    }

                    maxId = obj.ID;
                    id = obj.ID;

                    Parser.ResetEncode();
                    continue;
                }
                else if (obj.ID != id)
                {
                    objStrs[obj.ID] = string.Join(',', tempSet);
                    tempSet = new();

                    id = obj.ID;

                    Parser.ResetEncode();
                }

                tempSet.Add(SaveObject(obj));

                index++;
            }

            if (tempSet.Count > 0)
                objStrs.Add(string.Join(',', tempSet));
            final.Add(string.Join('/', objStrs));

            return string.Join("`", final);
        }
    }
}
