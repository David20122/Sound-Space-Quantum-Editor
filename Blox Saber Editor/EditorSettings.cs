using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Globalization;
using System.Json;
using OpenTK.Input;
using Sound_Space_Editor.Properties;

namespace Sound_Space_Editor
{
	class EditorSettings
	{
		public static readonly string file = "settings.txt";

		public static bool Waveform = true;
		//public static bool BPMForm = true;
		public static int EditorBGOpacity = 255;
		public static int GridOpacity = 255;
		public static int TrackOpacity = 255;
		public static Color Color1 = Color.FromArgb(0, 255, 200);
		public static Color Color2 = Color.FromArgb(255, 0, 255);
		public static Color Color3 = Color.FromArgb(255, 0, 100);
		public static Color NoteColor1 = Color.FromArgb(255, 0, 255);
		public static Color NoteColor2 = Color.FromArgb(0, 255, 200);
		public static bool EnableAutosave = true;
		public static int AutosaveInterval = 5;
		public static bool CorrectOnCopy = true;

		public static KeyType SelectAll = new KeyType() { Key = Key.A, CTRL = true, SHIFT = false, ALT = false };
		public static KeyType Save = new KeyType() { Key = Key.S, CTRL = true, SHIFT = false, ALT = false };
		public static KeyType SaveAs = new KeyType() { Key = Key.S, CTRL = true, SHIFT = true, ALT = false };
		public static KeyType Undo = new KeyType() { Key = Key.Z, CTRL = true, SHIFT = false, ALT = false };
		public static KeyType Redo = new KeyType() { Key = Key.Y, CTRL = true, SHIFT = false, ALT = false };
		public static KeyType Copy = new KeyType() { Key = Key.C, CTRL = true, SHIFT = false, ALT = false };
		public static KeyType Paste = new KeyType() { Key = Key.V, CTRL = true, SHIFT = false, ALT = false };
		public static KeyType Delete = new KeyType() { Key = Key.Delete, CTRL = false, SHIFT = false, ALT = false };

		public static GridKeySet GridKeys = new GridKeySet() { TL = Key.Q, TC = Key.W, TR = Key.E, ML = Key.A, MC = Key.S, MR = Key.D, BL = Key.Z, BC = Key.X, BR = Key.C };

		public static void Load()
		{
			try
			{
				Reset();
				JsonObject result = (JsonObject)JsonValue.Parse(File.ReadAllText(file));

				if (result.TryGetValue("waveform", out var value))
					Waveform = value;
				if (result.TryGetValue("editorBGOpacity", out value))
					EditorBGOpacity = value;
				if (result.TryGetValue("gridOpacity", out value))
					GridOpacity = value;
				if (result.TryGetValue("trackOpacity", out value))
					TrackOpacity = value;
				if (result.TryGetValue("color1", out value))
					Color1 = Color.FromArgb(value[0], value[1], value[2]);
				if (result.TryGetValue("color2", out value))
					Color2 = Color.FromArgb(value[0], value[1], value[2]);
				if (result.TryGetValue("color3", out value))
					Color3 = Color.FromArgb(value[0], value[1], value[2]);
				if (result.TryGetValue("noteColor1", out value))
					NoteColor1 = Color.FromArgb(value[0], value[1], value[2]);
				if (result.TryGetValue("noteColor2", out value))
					NoteColor2 = Color.FromArgb(value[0], value[1], value[2]);
				if (result.TryGetValue("enableAutosave", out value))
					EnableAutosave = value;
				if (result.TryGetValue("autosaveInterval", out value))
					AutosaveInterval = value;
				if (result.TryGetValue("correctOnCopy", out value))
					CorrectOnCopy = value;

				if (result.TryGetValue("keybinds", out value))
                {
					JsonObject keybinds = (JsonObject)value;
					if (keybinds.TryGetValue("selectAll", out value))
                    {
						SelectAll.Key = ConvertToKey(value[0]);
						SelectAll.CTRL = value[1];
						SelectAll.SHIFT = value[2];
						SelectAll.ALT = value[3];
					}
					if (keybinds.TryGetValue("save", out value))
					{
						Save.Key = ConvertToKey(value[0]);
						Save.CTRL = value[1];
						Save.SHIFT = value[2];
						Save.ALT = value[3];
					}
					if (keybinds.TryGetValue("saveAs", out value))
					{
						SaveAs.Key = ConvertToKey(value[0]);
						SaveAs.CTRL = value[1];
						SaveAs.SHIFT = value[2];
						SaveAs.ALT = value[3];
					}
					if (keybinds.TryGetValue("undo", out value))
					{
						Undo.Key = ConvertToKey(value[0]);
						Undo.CTRL = value[1];
						Undo.SHIFT = value[2];
						Undo.ALT = value[3];
					}
					if (keybinds.TryGetValue("redo", out value))
					{
						Redo.Key = ConvertToKey(value[0]);
						Redo.CTRL = value[1];
						Redo.SHIFT = value[2];
						Redo.ALT = value[3];
					}
					if (keybinds.TryGetValue("copy", out value))
					{
						Copy.Key = ConvertToKey(value[0]);
						Copy.CTRL = value[1];
						Copy.SHIFT = value[2];
						Copy.ALT = value[3];
					}
					if (keybinds.TryGetValue("paste", out value))
					{
						Paste.Key = ConvertToKey(value[0]);
						Paste.CTRL = value[1];
						Paste.SHIFT = value[2];
						Paste.ALT = value[3];
					}
					if (keybinds.TryGetValue("delete", out value))
                    {
						Delete.Key = ConvertToKey(value[0]);
						Delete.CTRL = value[1];
						Delete.SHIFT = value[2];
						Delete.ALT = value[3];
                    }
					if (keybinds.TryGetValue("gridKeys", out value))
					{
						GridKeys.TL = ConvertToKey(value[0]);
						GridKeys.TC = ConvertToKey(value[1]);
						GridKeys.TR = ConvertToKey(value[2]);
						GridKeys.ML = ConvertToKey(value[3]);
						GridKeys.MC = ConvertToKey(value[4]);
						GridKeys.MR = ConvertToKey(value[5]);
						GridKeys.BL = ConvertToKey(value[6]);
						GridKeys.BC = ConvertToKey(value[7]);
						GridKeys.BR = ConvertToKey(value[8]);
					}
				}


				RefreshKeymapping();
			}
			catch
			{
				Console.WriteLine("error while loading settings");
			}

            Console.WriteLine("Loaded => {0} | {1} | {2} | {3} | {4} | {5} | {6} | {7} | {8} | {9} | {10} | {11}", Waveform, EditorBGOpacity, GridOpacity, TrackOpacity, Color1, Color2, Color3, NoteColor1, NoteColor2, EnableAutosave, AutosaveInterval, CorrectOnCopy);
		}

		private static Key ConvertToKey(string key)
        {
			return (Key)Enum.Parse(typeof(Key), key, true);
        }

		public static void Reset()
		{
			Waveform = true;
			EditorBGOpacity = 255;
			GridOpacity = 255;
			TrackOpacity = 255;
			Color1 = Color.FromArgb(0, 255, 200);
			Color2 = Color.FromArgb(255, 0, 255);
			Color3 = Color.FromArgb(255, 0, 100);
			NoteColor1 = Color.FromArgb(255, 0, 255);
			NoteColor2 = Color.FromArgb(0, 255, 200);
			EnableAutosave = true;
			AutosaveInterval = 5;
			CorrectOnCopy = true;
		}

		public static void RefreshKeymapping()
        {
			EditorWindow.Instance.KeyMapping.Clear();
			if (Settings.Default.Numpad)
            {
				EditorWindow.Instance.KeyMapping.Add(Key.Keypad7, new Tuple<int, int>(0, 0));
				EditorWindow.Instance.KeyMapping.Add(Key.Keypad8, new Tuple<int, int>(1, 0));
				EditorWindow.Instance.KeyMapping.Add(Key.Keypad9, new Tuple<int, int>(2, 0));

				EditorWindow.Instance.KeyMapping.Add(Key.Keypad4, new Tuple<int, int>(0, 1));
				EditorWindow.Instance.KeyMapping.Add(Key.Keypad5, new Tuple<int, int>(1, 1));
				EditorWindow.Instance.KeyMapping.Add(Key.Keypad6, new Tuple<int, int>(2, 1));

				EditorWindow.Instance.KeyMapping.Add(Key.Keypad1, new Tuple<int, int>(0, 2));
				EditorWindow.Instance.KeyMapping.Add(Key.Keypad2, new Tuple<int, int>(1, 2));
				EditorWindow.Instance.KeyMapping.Add(Key.Keypad3, new Tuple<int, int>(2, 2));
			}
			else
            {
				EditorWindow.Instance.KeyMapping.Add(GridKeys.TL, new Tuple<int, int>(0, 0));
				EditorWindow.Instance.KeyMapping.Add(GridKeys.TC, new Tuple<int, int>(1, 0));
				EditorWindow.Instance.KeyMapping.Add(GridKeys.TR, new Tuple<int, int>(2, 0));

				EditorWindow.Instance.KeyMapping.Add(GridKeys.ML, new Tuple<int, int>(0, 1));
				EditorWindow.Instance.KeyMapping.Add(GridKeys.MC, new Tuple<int, int>(1, 1));
				EditorWindow.Instance.KeyMapping.Add(GridKeys.MR, new Tuple<int, int>(2, 1));

				EditorWindow.Instance.KeyMapping.Add(GridKeys.BL, new Tuple<int, int>(0, 2));
				EditorWindow.Instance.KeyMapping.Add(GridKeys.BC, new Tuple<int, int>(1, 2));
				EditorWindow.Instance.KeyMapping.Add(GridKeys.BR, new Tuple<int, int>(2, 2));
			}
		}

		public static void SaveSettings()
		{
			RefreshKeymapping();
			JsonObject jsonFinal = new JsonObject(Array.Empty<KeyValuePair<string, JsonValue>>())
			{
				{"waveform", Waveform},
				{"editorBGOpacity", EditorBGOpacity},
				{"gridOpacity", GridOpacity},
				{"trackOpacity", TrackOpacity},
				{"color1", new JsonArray(Color1.R, Color1.G, Color1.B)},
				{"color2", new JsonArray(Color2.R, Color2.G, Color2.B)},
				{"color3", new JsonArray(Color3.R, Color3.G, Color3.B)},
				{"noteColor1", new JsonArray(NoteColor1.R, NoteColor1.G, NoteColor1.B)},
				{"noteColor2", new JsonArray(NoteColor2.R, NoteColor2.G, NoteColor2.B)},
				{"enableAutosave", EnableAutosave},
				{"autosaveInterval",AutosaveInterval},
				{"correctOnCopy",CorrectOnCopy},
				{"keybinds", new JsonObject(Array.Empty<KeyValuePair<string, JsonValue>>()) {
						{"selectAll", new JsonArray(SelectAll.Key.ToString(), SelectAll.CTRL, SelectAll.SHIFT, SelectAll.ALT)},
						{"save", new JsonArray(Save.Key.ToString(), Save.CTRL, Save.SHIFT, Save.ALT)},
						{"saveAs", new JsonArray(SaveAs.Key.ToString(), SaveAs.CTRL, SaveAs.SHIFT, SaveAs.ALT)},
						{"undo", new JsonArray(Undo.Key.ToString(), Undo.CTRL, Undo.SHIFT, Undo.ALT)},
						{"redo", new JsonArray(Redo.Key.ToString(), Redo.CTRL, Redo.SHIFT, Redo.ALT)},
						{"copy", new JsonArray(Copy.Key.ToString(), Copy.CTRL, Copy.SHIFT, Copy.ALT)},
						{"paste", new JsonArray(Paste.Key.ToString(), Paste.CTRL, Paste.SHIFT, Paste.ALT)},
						{"delete", new JsonArray(Delete.Key.ToString(), Delete.CTRL, Delete.SHIFT, Delete.ALT)},
						{"gridKeys", new JsonArray(GridKeys.TL.ToString(), GridKeys.TC.ToString(), GridKeys.TR.ToString(), GridKeys.ML.ToString(), GridKeys.MC.ToString(), GridKeys.MR.ToString(), GridKeys.BL.ToString(), GridKeys.BC.ToString(), GridKeys.BR.ToString())},
					} 
				}
			};
			try
			{
				File.WriteAllText(file, FormatJson(jsonFinal.ToString()));
				Console.WriteLine("Saved => {0} | {1} | {2} | {3} | {4} | {5} | {6} | {7} | {8} | {9} | {10} | {11}", Waveform, EditorBGOpacity, GridOpacity, TrackOpacity, Color1, Color2, Color3, NoteColor1, NoteColor2, EnableAutosave, AutosaveInterval, CorrectOnCopy);
			}
			catch
			{
				Console.WriteLine("failed to save");
			}
		}

		//thank you whoever made this so i didnt have to
		public static string FormatJson(string json, string indent = "     ")
		{
			var indentation = 0;
			var quoteCount = 0;
			var escapeCount = 0;

			var result =
				from ch in json ?? string.Empty
				let escaped = (ch == '\\' ? escapeCount++ : escapeCount > 0 ? escapeCount-- : escapeCount) > 0
				let quotes = ch == '"' && !escaped ? quoteCount++ : quoteCount
				let unquoted = quotes % 2 == 0
				let colon = ch == ':' && unquoted ? ": " : null
				let nospace = char.IsWhiteSpace(ch) && unquoted ? string.Empty : null
				let lineBreak = ch == ',' && unquoted ? ch + Environment.NewLine + string.Concat(Enumerable.Repeat(indent, indentation)) : null
				let openChar = (ch == '{' || ch == '[') && unquoted ? ch + Environment.NewLine + string.Concat(Enumerable.Repeat(indent, ++indentation)) : ch.ToString()
				let closeChar = (ch == '}' || ch == ']') && unquoted ? Environment.NewLine + string.Concat(Enumerable.Repeat(indent, --indentation)) + ch : ch.ToString()
				select colon ?? nospace ?? lineBreak ?? (
					openChar.Length > 1 ? openChar : closeChar
				);

			return string.Concat(result);
		}

		public class KeyType
        {
			public Key Key;
			public bool CTRL;
			public bool SHIFT;
			public bool ALT;
        }

		public class GridKeySet
		{
			public Key TL;
			public Key TC;
			public Key TR;
			public Key ML;
			public Key MC;
			public Key MR;
			public Key BL;
			public Key BC;
			public Key BR;
        }
	}
}
