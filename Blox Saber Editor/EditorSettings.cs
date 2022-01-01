﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Globalization;

namespace Sound_Space_Editor
{
	public class EditorSettings
	{
		public static readonly string file = "settings.txt";
		public static bool Waveform;
		//public static bool BPMForm;
		public static int EditorBGOpacity;
		public static int GridOpacity;
		public static int TrackOpacity;
		public static string Color1;
		public static string Color2;
		public static string NoteColor1;
		public static string NoteColor2;
		public static string NoteColors;

		public static void Load()
		{
			try
			{
				foreach (string text in File.ReadAllLines(file))
				{
					if (text.Contains("--"))
						continue;
					string[] setting = text.Trim().Replace(" ", "").Split(new char[] { '=' });
					string settingName = setting[0];
					string value = setting[1];
					Console.WriteLine("{0}, {1}", settingName, value);
					switch (settingName)
					{
						case "Waveform":
							bool resA;
							bool.TryParse(value, out resA);
							Waveform = resA;
							break;
						/*
						case "BPMForm":
							bool resE;
							bool.TryParse(value, out resE);
							BPMForm = resE;
							break;
						*/
						case "EditorBGOpacity":
							int resB;
							int.TryParse(value, out resB);
							EditorBGOpacity = resB;
							break;
						case "GridOpacity":
							int resC;
							int.TryParse(value, out resC);
							GridOpacity = resC;
							break;
						case "TrackOpacity":
							int resD;
							int.TryParse(value, out resD);
							TrackOpacity = resD;
							break;
						case "Color1":
							Color1 = value;
							break;
						case "Color2":
							Color2 = value;
							break;
						case "NoteColor1":
							NoteColor1 = value;
							break;
						case "NoteColor2":
							NoteColor2 = value;
							break;
						case "NoteColors":
							NoteColors = value;
							break;
					}
				}
			}
			catch
			{
				Reset();
				Console.WriteLine("no settings.txt - loading default settings");
			}

            Console.WriteLine("Loaded => {0} | {1} | {2} | {3} | {4} | {5} | {6} | {7} || {8}", Waveform, EditorBGOpacity, GridOpacity, TrackOpacity, Color1, Color2, NoteColor1, NoteColor2, NoteColors);
		}

		public static void Reset()
		{
			Waveform = true;
			//BPMForm = false;
			EditorBGOpacity = 255;
			GridOpacity = 255;
			TrackOpacity = 255;
			Color1 = "0,255,200";
			Color2 = "255,0,255";
			NoteColor1 = "255,0,255";
			NoteColor2 = "0,255,200";
			NoteColors = "255,0,255|0,255,200";
		}

		public static void Save()
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendLine("-- DON'T EDIT THIS, LAUNCH EDITOR AND GO IN SETTINGS INSTEAD");
			sb.AppendLine(string.Format("Waveform={0}", Waveform));
			//sb.AppendLine(string.Format("BPMForm={0}", BPMForm));
			sb.AppendLine(string.Format("EditorBGOpacity={0}", EditorBGOpacity));
			sb.AppendLine(string.Format("GridOpacity={0}", GridOpacity));
			sb.AppendLine(string.Format("TrackOpacity={0}", TrackOpacity));
			sb.AppendLine(string.Format("Color1={0}", Color1));
			sb.AppendLine(string.Format("Color2={0}", Color2));
			sb.AppendLine(string.Format("NoteColor1={0}", NoteColor1));
			sb.AppendLine(string.Format("NoteColor2={0}", NoteColor2));
			sb.AppendLine(string.Format("NoteColors={0}", NoteColors));
			try
			{
				File.WriteAllText(file, sb.ToString());
				Console.WriteLine("Saved => {0} | {1} | {2} | {3} | {4} | {5} | {6} | {7} || {8}", Waveform, EditorBGOpacity, GridOpacity, TrackOpacity, Color1, Color2, NoteColor1, NoteColor2, NoteColors);
			}
			catch
			{
				Console.WriteLine("failed to save");
			}
		}
	}
}
