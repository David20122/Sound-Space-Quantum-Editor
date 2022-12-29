﻿using System;
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
		public static KeyType HFlip = new KeyType() { Key = Key.H, CTRL = false, SHIFT = true, ALT = false };
		public static KeyType VFlip = new KeyType() { Key = Key.V, CTRL = false, SHIFT = true, ALT = false };
		public static KeyType SwitchClickTool = new KeyType() { Key = Key.Tab, CTRL = false, SHIFT = false, ALT = false };
		public static KeyType Quantum = new KeyType() { Key = Key.Q, CTRL = true, SHIFT = false, ALT = false };
		public static KeyType OpenTimings = new KeyType() { Key = Key.T, CTRL = true, SHIFT = false, ALT = false };
		public static KeyType OpenBookmarks = new KeyType() { Key = Key.B, CTRL = true, SHIFT = false, ALT = false };
		public static KeyType StoreNodes = new KeyType() { Key = Key.S, CTRL = false, SHIFT = true, ALT = false };
		public static KeyType DrawBezier = new KeyType() { Key = Key.D, CTRL = false, SHIFT = true, ALT = false };
		public static KeyType AnchorNode = new KeyType() { Key = Key.A, CTRL = false, SHIFT = true, ALT = false };

		public static GridKeySet GridKeys = new GridKeySet() { TL = Key.Q, TC = Key.W, TR = Key.E, ML = Key.A, MC = Key.S, MR = Key.D, BL = Key.Z, BC = Key.X, BR = Key.C };

		public static string Pattern0 = "";
		public static string Pattern1 = "";
		public static string Pattern2 = "";
		public static string Pattern3 = "";
		public static string Pattern4 = "";
		public static string Pattern5 = "";
		public static string Pattern6 = "";
		public static string Pattern7 = "";
		public static string Pattern8 = "";
		public static string Pattern9 = "";

        public static float ApproachTime = 0.5f;
        public static float ApproachDistance = 4;
        public static float Sensitivity = 1;
        public static float Parallax = 0;


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
                if (result.TryGetValue("approachTime", out value))
                    ApproachTime = value;
                if (result.TryGetValue("approachDistance", out value))
                    ApproachDistance = value;
                if (result.TryGetValue("sensitivity", out value))
                    Sensitivity = value;
                if (result.TryGetValue("parallax", out value))
                    Parallax = value;

                if (result.TryGetValue("keybinds", out value))
                {
					JsonObject keybinds = (JsonObject)value;
					if (keybinds.TryGetValue("selectAll", out value))
						SelectAll = ConvertToKeybind(value);
					if (keybinds.TryGetValue("save", out value))
						Save = ConvertToKeybind(value);
					if (keybinds.TryGetValue("saveAs", out value))
						SaveAs = ConvertToKeybind(value);
					if (keybinds.TryGetValue("undo", out value))
						Undo = ConvertToKeybind(value);
					if (keybinds.TryGetValue("redo", out value))
						Redo = ConvertToKeybind(value);
					if (keybinds.TryGetValue("copy", out value))
						Copy = ConvertToKeybind(value);
					if (keybinds.TryGetValue("paste", out value))
						Paste = ConvertToKeybind(value);
					if (keybinds.TryGetValue("delete", out value))
						Delete = ConvertToKeybind(value);
					if (keybinds.TryGetValue("horizontalFlip", out value))
						HFlip = ConvertToKeybind(value);
					if (keybinds.TryGetValue("verticalFlip", out value))
						VFlip = ConvertToKeybind(value);
					if (keybinds.TryGetValue("switchClickTool", out value))
						SwitchClickTool = ConvertToKeybind(value);
					if (keybinds.TryGetValue("quantum", out value))
						Quantum = ConvertToKeybind(value);
					if (keybinds.TryGetValue("openTimings", out value))
						OpenTimings = ConvertToKeybind(value);
					if (keybinds.TryGetValue("openBookmarks", out value))
						OpenBookmarks = ConvertToKeybind(value);
					if (keybinds.TryGetValue("storeNodes", out value))
						StoreNodes = ConvertToKeybind(value);
					if (keybinds.TryGetValue("drawBezier", out value))
						DrawBezier = ConvertToKeybind(value);
					if (keybinds.TryGetValue("anchorNode", out value))
						AnchorNode = ConvertToKeybind(value);

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

					if (keybinds.TryGetValue("patterns", out value))
                    {
						Pattern0 = value[0];
						Pattern1 = value[1];
						Pattern2 = value[2];
						Pattern3 = value[3];
						Pattern4 = value[4];
						Pattern5 = value[5];
						Pattern6 = value[6];
						Pattern7 = value[7];
						Pattern8 = value[8];
						Pattern9 = value[9];
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

		private static KeyType ConvertToKeybind(JsonValue value)
        {
			var key = new KeyType();
			key.Key = ConvertToKey(value[0]);
			key.CTRL = value[1];
			key.SHIFT = value[2];
			key.ALT = value[3];
			return key;
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
			ApproachTime = 0.5f;
			ApproachDistance = 4;
			Sensitivity = 1;
			Parallax = 0;
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
                {"approachTime", ApproachTime},
                {"approachDistance",ApproachDistance},
                {"sensitivity",Sensitivity},
                {"parallax",Parallax},
                {"keybinds", new JsonObject(Array.Empty<KeyValuePair<string, JsonValue>>()) {
					{"selectAll", new JsonArray(SelectAll.Key.ToString(), SelectAll.CTRL, SelectAll.SHIFT, SelectAll.ALT)},
					{"save", new JsonArray(Save.Key.ToString(), Save.CTRL, Save.SHIFT, Save.ALT)},
					{"saveAs", new JsonArray(SaveAs.Key.ToString(), SaveAs.CTRL, SaveAs.SHIFT, SaveAs.ALT)},
					{"undo", new JsonArray(Undo.Key.ToString(), Undo.CTRL, Undo.SHIFT, Undo.ALT)},
					{"redo", new JsonArray(Redo.Key.ToString(), Redo.CTRL, Redo.SHIFT, Redo.ALT)},
					{"copy", new JsonArray(Copy.Key.ToString(), Copy.CTRL, Copy.SHIFT, Copy.ALT)},
					{"paste", new JsonArray(Paste.Key.ToString(), Paste.CTRL, Paste.SHIFT, Paste.ALT)},
					{"delete", new JsonArray(Delete.Key.ToString(), Delete.CTRL, Delete.SHIFT, Delete.ALT)},
					{"horizontalFlip", new JsonArray(HFlip.Key.ToString(), HFlip.CTRL, HFlip.SHIFT, HFlip.ALT)},
					{"verticalFlip", new JsonArray(VFlip.Key.ToString(), VFlip.CTRL, VFlip.SHIFT, VFlip.ALT)},
					{"switchClickTool", new JsonArray(SwitchClickTool.Key.ToString(), SwitchClickTool.CTRL, SwitchClickTool.SHIFT, SwitchClickTool.ALT)},
					{"quantum", new JsonArray(Quantum.Key.ToString(), Quantum.CTRL, Quantum.SHIFT, Quantum.ALT)},
					{"openTimings", new JsonArray(OpenTimings.Key.ToString(), OpenTimings.CTRL, OpenTimings.SHIFT, OpenTimings.ALT)},
					{"openBookmarks", new JsonArray(OpenBookmarks.Key.ToString(), OpenBookmarks.CTRL, OpenBookmarks.SHIFT, OpenBookmarks.ALT)},
					{"storeNodes", new JsonArray(StoreNodes.Key.ToString(), StoreNodes.CTRL, StoreNodes.SHIFT, StoreNodes.ALT)},
					{"drawBezier", new JsonArray(DrawBezier.Key.ToString(), DrawBezier.CTRL, DrawBezier.SHIFT, DrawBezier.ALT)},
					{"anchorNode", new JsonArray(AnchorNode.Key.ToString(), AnchorNode.CTRL, AnchorNode.SHIFT, AnchorNode.ALT)},
					{"gridKeys", new JsonArray(GridKeys.TL.ToString(), GridKeys.TC.ToString(), GridKeys.TR.ToString(), GridKeys.ML.ToString(), GridKeys.MC.ToString(), GridKeys.MR.ToString(), GridKeys.BL.ToString(), GridKeys.BC.ToString(), GridKeys.BR.ToString())},
					{"patterns", new JsonArray(Pattern0, Pattern1, Pattern2, Pattern3, Pattern4, Pattern5, Pattern6, Pattern7, Pattern8, Pattern9)},
				}}
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
