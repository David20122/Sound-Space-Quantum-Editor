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
		public static Color NoteColor1 = Color.FromArgb(255, 0, 255);
		public static Color NoteColor2 = Color.FromArgb(0, 255, 200);
		public static bool EnableAutosave = true;
		public static int AutosaveInterval = 5;
		public static bool CorrectOnCopy = true;

		public static void Load()
		{
			try
			{
				Reset();
				var result = JsonValue.Parse(File.ReadAllText(file));

				if (result.ContainsKey("Waveform"))
					Waveform = result["Waveform"];
				if (result.ContainsKey("EditorBGOpacity"))
					EditorBGOpacity = result["EditorBGOpacity"];
				if (result.ContainsKey("GridOpacity"))
					GridOpacity = result["GridOpacity"];
				if (result.ContainsKey("TrackOpacity"))
					TrackOpacity = result["TrackOpacity"];
				if (result.ContainsKey("Color1"))
					Color1 = Color.FromArgb(result["Color1"]["R"], result["Color1"]["G"], result["Color1"]["B"]);
				if (result.ContainsKey("Color2"))
					Color2 = Color.FromArgb(result["Color2"]["R"], result["Color2"]["G"], result["Color2"]["B"]);
				if (result.ContainsKey("NoteColor1"))
					NoteColor1 = Color.FromArgb(result["NoteColor1"]["R"], result["NoteColor1"]["G"], result["NoteColor1"]["B"]);
				if (result.ContainsKey("NoteColor2"))
					NoteColor2 = Color.FromArgb(result["NoteColor2"]["R"], result["NoteColor2"]["G"], result["NoteColor2"]["B"]);
				if (result.ContainsKey("EnableAutosave"))
					EnableAutosave = result["EnableAutosave"];
				if (result.ContainsKey("AutosaveInterval"))
					AutosaveInterval = result["AutosaveInterval"];
				if (result.ContainsKey("CorrectOnCopy"))
					CorrectOnCopy = result["CorrectOnCopy"];
			}
			catch
			{
				Console.WriteLine("error while loading settings");
			}

            Console.WriteLine("Loaded => {0} | {1} | {2} | {3} | {4} | {5} | {6} | {7} | {8} | {9} | {10}", Waveform, EditorBGOpacity, GridOpacity, TrackOpacity, Color1, Color2, NoteColor1, NoteColor2, EnableAutosave, AutosaveInterval, CorrectOnCopy);
		}

		public static void Reset()
		{
			Waveform = true;
			EditorBGOpacity = 255;
			GridOpacity = 255;
			TrackOpacity = 255;
			Color1 = Color.FromArgb(0, 255, 200);
			Color2 = Color.FromArgb(255, 0, 255);
			NoteColor1 = Color.FromArgb(255, 0, 255);
			NoteColor2 = Color.FromArgb(0, 255, 200);
			EnableAutosave = true;
			AutosaveInterval = 5;
			CorrectOnCopy = true;
		}

		public static void Save()
		{
			var jsonFinal = new JsonObject(Array.Empty<KeyValuePair<string, JsonValue>>())
			{
				{"Waveform", Waveform},
				{"EditorBGOpacity", EditorBGOpacity},
				{"GridOpacity", GridOpacity},
				{"TrackOpacity", TrackOpacity},
				{"Color1", new JsonObject(Array.Empty<KeyValuePair<string, JsonValue>>()) {
						{"R", Color1.R},
						{"G", Color1.G},
						{"B", Color1.B}
					}
				},
				{"Color2", new JsonObject(Array.Empty<KeyValuePair<string, JsonValue>>()) {
						{"R", Color2.R},
						{"G", Color2.G},
						{"B", Color2.B}
					}
				},
				{"NoteColor1", new JsonObject(Array.Empty<KeyValuePair<string, JsonValue>>()) {
						{"R", NoteColor1.R},
						{"G", NoteColor1.G},
						{"B", NoteColor1.B}
					}
				},
				{"NoteColor2", new JsonObject(Array.Empty<KeyValuePair<string, JsonValue>>()) {
						{"R", NoteColor2.R},
						{"G", NoteColor2.G},
						{"B", NoteColor2.B}
					}
				},
				{"EnableAutosave", EnableAutosave},
				{"AutosaveInterval",AutosaveInterval},
				{"CorrectOnCopy",CorrectOnCopy}
			};
			try
			{
				File.WriteAllText(file, jsonFinal.ToString());
				Console.WriteLine("Saved => {0} | {1} | {2} | {3} | {4} | {5} | {6} | {7} | {8} | {9} | {10}", Waveform, EditorBGOpacity, GridOpacity, TrackOpacity, Color1, Color2, NoteColor1, NoteColor2, EnableAutosave, AutosaveInterval, CorrectOnCopy);
			}
			catch
			{
				Console.WriteLine("failed to save");
			}
		}
	}
}
