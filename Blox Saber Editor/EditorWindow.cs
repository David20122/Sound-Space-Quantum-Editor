using System;
using System.Collections.Generic;
using System.Globalization;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using Sound_Space_Editor.Gui;
using Sound_Space_Editor.Properties;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using KeyPressEventArgs = OpenTK.KeyPressEventArgs;
using Discord;
using System.Threading.Tasks;
using System.Diagnostics;

//god

namespace Sound_Space_Editor
{
	class EditorWindow : GameWindow
	{
		private bool discordEnabled = false;
		public static EditorWindow Instance;

		public FontRenderer FontRenderer;
		public FontRenderer SquareFontRenderer;
		public FontRenderer SquareOFontRenderer;

		public bool IsPaused { get; private set; }

		private bool Autosaving = false;
		private bool SelectTool = false;

		public GuiScreen GuiScreen { get; private set; }

		public MusicPlayer MusicPlayer;
		public SoundPlayer SoundPlayer;

		public Discord.Discord discord;
		public Discord.ActivityManager activityManager;
		public Discord.UserManager userManager;
		public Discord.NetworkManager networkManager;
		public Discord.LobbyManager lobbyManager;

		public readonly Dictionary<Key, Tuple<int, int>> KeyMapping = new Dictionary<Key, Tuple<int, int>>();

		//private readonly GuiScreenEditor _screenEditor;

		private Random _rand = new Random();

		private Rectangle _lastWindowRect;

		public readonly UndoRedo UndoRedo = new UndoRedo();

		public bool IsDraggingNoteOnTimeLine => _draggingNoteTimeline && _draggedNotes.FirstOrDefault() is Note n && n.DragStartMs != n.Ms;
		public bool IsDraggingPointOnTimeline => _draggingPointTimeline && _draggedPoint is BPM n && n.DragStartMs != n.Ms;
		public List<Note> SelectedNotes = new List<Note>();
		public List<Note> _draggedNotes = new List<Note>();
		private Note _draggedNote;
		public BPM _draggedPoint;

		private Point _clickedMouse;
		private Point _lastMouse;

		public bool IsFullscreen;

		private Color _flashColor;
		private float _brightness;

		private int _dragStartX;
		private long _dragStartMs;
		private long _dragNoteStartMs;
		private long _dragPointStartMs;

		private float _dragStartIndexX;
		private float _dragStartIndexY;

		private long _mouseDownMs;

		private bool _rightDown;
		public bool _controlDown;
		public bool _altDown;
		public bool _shiftDown;
		private bool _draggingNoteTimeline;
		private bool _draggingPointTimeline;
		private bool _draggingNoteGrid;
		private bool _draggingTimeline;

		private bool _wasPlaying;

		private string _file;

		private string _soundId = "-1";
		public float tempo = 1f;
		public TimeSpan currentTime;
		public TimeSpan totalTime;

		public NoteList Notes = new NoteList();

		private float _zoom = 1;

		public Color Color1;
		public Color Color2;
		public Color Color3;
		public Color NoteColor1;
		public Color NoteColor2;
		public float Zoom
		{
			get => _zoom;
			set => _zoom = Math.Max(0.01f, Math.Min(10, value));
		}

		public float CubeStep => 50 * 10 * Zoom;

		public string LauncherDir;
		public string cacheFolder;
		public string currentEditorVersion;
		public string downloadedVersionString;

		public string currentData;

		public bool inconspicuousvar = false;

		public EditorWindow(long offset, string launcherDir) : base(1280, 720, new GraphicsMode(32, 8, 0, 8), "Sound Space Quantum Editor " + Application.ProductVersion)
		{
			LauncherDir = launcherDir;
			cacheFolder = Path.Combine(launcherDir, "cached/");
			Instance = this;
			this.WindowState = OpenTK.WindowState.Maximized;
			Icon = Resources.icon;
			VSync = VSyncMode.On;
			TargetUpdatePeriod = 1.0 / 20.0;

			CheckForUpdates();

			//TargetRenderFrequency = 60;

			MusicPlayer = new MusicPlayer { Volume = 0.25f };
			SoundPlayer = new SoundPlayer();

			FontRenderer = new FontRenderer("main");
			SquareOFontRenderer = new FontRenderer("Squareo");
			SquareFontRenderer = new FontRenderer("Square");

			EditorSettings.Load();

			UpdateColors();

			OpenGuiScreen(new GuiScreenMenu());

			SoundPlayer.Cache("hit", false);
			SoundPlayer.Cache("click", false);
			SoundPlayer.Cache("metronome", false);

			LoadSound("1091083826");

			SoundPlayer.Cache("1091083826", true);

			EditorSettings.RefreshKeymapping();

			SelectTool = Settings.Default.SelectTool;

			if (discordEnabled)
			{
				discord = new Discord.Discord(751010237388947517L, (ulong)Discord.CreateFlags.NoRequireDiscord);
				activityManager = discord.GetActivityManager();
				userManager = discord.GetUserManager();
				networkManager = discord.GetNetworkManager();
				lobbyManager = discord.GetLobbyManager();
			}
		}

		void CheckForUpdates()
        {
			var versionInfo = FileVersionInfo.GetVersionInfo(Application.ExecutablePath);
			currentEditorVersion = versionInfo.FileVersion;

			try
            {
				WebClient wc = new WebClient();
				string reply = wc.DownloadString("https://raw.githubusercontent.com/David20122/SSQEUpdater/main/version");
				string trimmedReply = reply.TrimEnd();
				downloadedVersionString = trimmedReply;

				if (currentEditorVersion != downloadedVersionString)
				{
					object[] settings = {
						Settings.Default.MasterVolume,
						Settings.Default.SFXVolume,
						Settings.Default.GridNumbers,
						Settings.Default.ApproachSquares,
						Settings.Default.AnimateBackground,
						Settings.Default.Autoplay,
						Settings.Default.BGDim,
						Settings.Default.Quantum,
						Settings.Default.AutoAdvance,
						Settings.Default.SfxOffset,
						Settings.Default.Numpad,
						Settings.Default.QuantumGridLines,
						Settings.Default.QuantumGridSnap,
						Settings.Default.Metronome,
						Settings.Default.LegacyBPM,
						Settings.Default.SeparateClickTools,
						Settings.Default.AutosavedFile.Replace(',', '&'),
						Settings.Default.TrackHeight,
						Settings.Default.CurveBezier,
						Settings.Default.LastFile.Replace(' ', '>').Replace(',', '<'),
						Settings.Default.GridLetters,
						Settings.Default.CursorPos,
						Settings.Default.SelectTool,
						Settings.Default.ApproachRate,
					};

					try
					{
						Process.Start("SSQEUpdater.exe", string.Join(" ", settings));
						Environment.Exit(0);
					}
					catch
					{
						MessageBox.Show("Failed to locate 'SSQEUpdater.exe'\nDid you rename or move the file?", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
					}
				}
			} 
			catch
            {
				MessageBox.Show("Failed to check for updates", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
		}

		public void UpdateColors()
		{
			try
			{
				Color1 = EditorSettings.Color1;
				Color2 = EditorSettings.Color2;
				Color3 = EditorSettings.Color3;
				NoteColor1 = EditorSettings.NoteColor1;
				NoteColor2 = EditorSettings.NoteColor2;

				Console.WriteLine("Updated Colors => {0} | {1} | {2} | {3} | {4}", string.Join(", ", Color1), string.Join(", ", Color2), string.Join(", ", Color3), string.Join(", ", NoteColor1), string.Join(", ", NoteColor2));
			}
			catch
			{
				UpdateColors();

				MessageBox.Show("Colors have been reset to default beacuse one or more were invalid.");
			}
		}

		private void ResetPoints(bool updatelist)
        {
			GuiTrack.BPMs = GuiTrack.BPMs.OrderBy(o => o.Ms).ToList();

			if (updatelist && TimingsWindow.inst != null)
				TimingsWindow.inst.ResetList(_draggedPoint != null ? GuiTrack.BPMs.IndexOf(_draggedPoint) : 0);
		}

		public void UpdateActivity(string state)
		{
			if (!discordEnabled)
			{
				return;
			}
			var activity = new Discord.Activity
			{
				State = state,
				Details = "Version " + Application.ProductVersion,
				Timestamps =
				{
					Start = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
				},
				Instance = true,
			};
			this.discord.ActivityManagerInstance.UpdateActivity(activity, (result) =>
			{
				if (result == Discord.Result.Ok)
				{
					Console.WriteLine("Success!");
				}
				else
				{
					Console.WriteLine("Failed");
				}
			});
		}

		public string ReadLine(string FilePath, int LineNumber)
		{
			string result = "";
			try
			{
				if (File.Exists(FilePath))
				{
					using (StreamReader _StreamReader = new StreamReader(FilePath))
					{
						for (int a = 0; a < LineNumber; a++)
						{
							result = _StreamReader.ReadLine();
						}
					}
				}
			}
			catch { }
			return result;
		}

		public void ToggleFullscreen()
		{
			if (!IsFullscreen)
			{
				_lastWindowRect = new Rectangle(Location, Size);

				WindowBorder = WindowBorder.Hidden;

				Location = Point.Empty;
				Size = Screen.GetBounds(Location).Size;

				IsFullscreen = true;
			}
			else
			{
				WindowBorder = WindowBorder.Resizable;

				Location = _lastWindowRect.Location;
				Size = _lastWindowRect.Size;

				IsFullscreen = false;
			}
		}

		protected override void OnLoad(EventArgs e)
		{
			GL.Enable(EnableCap.Blend);
			GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

			GL.Hint(HintTarget.LineSmoothHint, HintMode.Nicest);
			GL.Hint(HintTarget.PolygonSmoothHint, HintMode.Nicest);

			GL.Enable(EnableCap.Texture2D);
			GL.ActiveTexture(TextureUnit.Texture0);

			UpdateActivity("Sitting in the menu");
		}

		protected override void OnRenderFrame(FrameEventArgs e)
		{
			GL.Clear(ClearBufferMask.ColorBufferBit);

			GL.PushMatrix();

			var b = (float)Math.Pow(_brightness, 7) * 0.25f;
			GL.ClearColor(_flashColor.R / 255f * b, _flashColor.G / 255f * b, _flashColor.B / 255f * b, 1);

			_brightness = (float)Math.Max(0, _brightness - e.Time);

			GuiScreen?.Render((float)e.Time, _lastMouse.X, _lastMouse.Y);

			if (GuiScreen is GuiScreenEditor editor)
			{
				if (_drawPreview && (!Settings.Default.SeparateClickTools || !SelectTool))
				{
					editor.Grid.RenderFakeNote(_previewNote.X, _previewNote.Y, null);
				}

				if (MusicPlayer.IsPlaying)
                {
					currentTime = MusicPlayer.CurrentTime;
					AlignTimeline();
				}

				if (_draggingNoteTimeline)
				{
					var rect = editor.Track.ClientRectangle;

					foreach (var draggedNote in _draggedNotes)
					{
						var posX = currentTime.TotalSeconds * CubeStep;
						var noteX = editor.Track.ScreenX - posX + draggedNote.DragStartMs / 1000f * CubeStep;

						GL.Color3(0.75f, 0.75f, 0.75f);
						Glu.RenderQuad((int)noteX, (int)rect.Y, 1, rect.Height);
					}
				}

				if (_draggingPointTimeline && _draggedPoint != null)
                {
					var rect = editor.Track.ClientRectangle;

					var posX = currentTime.TotalSeconds * CubeStep;
					var pointX = editor.Track.ScreenX - posX + _draggedPoint.DragStartMs / 1000f * CubeStep;

					GL.Color3(0.75f, 0.75f, 0.75f);
					Glu.RenderQuad((int)pointX, (int)rect.Y, 1, rect.Height);
                }

				if (_rightDown)
				{
					if (editor.Track.ClientRectangle.Contains(_clickedMouse))
					{/*
					var startX = _clickedMouse.X;

					var time = (long)currentTime.TotalMilliseconds;
					var over = _mouseDownMs - time;
					var offset = over / 1000M * (decimal)CubeStep;

					startX += (int)offset;

					var x = Math.Min(_lastMouse.X, startX);
					var y = Math.Min(_lastMouse.Y, _clickedMouse.Y);

					var w = Math.Max(_lastMouse.X, startX) - x;
					var h = Math.Min((int)g.Track.ClientRectangle.Height, Math.Max(_lastMouse.Y, _clickedMouse.Y)) - y;
					*/

						var rect = UpdateSelection(editor);

						GL.Color4(0, 1, 0.2f, 0.2f);
						Glu.RenderQuad(rect.X, rect.Y, rect.Width, rect.Height);
						GL.Color4(0, 1, 0.2f, 1);
						Glu.RenderOutline(rect.X, rect.Y, rect.Width, rect.Height);
					}
				}
			}

			GL.PopMatrix();
			SwapBuffers();
		}

		protected override void OnResize(EventArgs e)
		{
			if (ClientSize.Width < 800)
			{
				ClientSize = new Size(800, ClientSize.Height);
			}
			if (ClientSize.Height < 600)
			{
				ClientSize = new Size(ClientSize.Width, 600);
			}

			GL.Viewport(ClientRectangle);

			GL.MatrixMode(MatrixMode.Projection);
			var m = Matrix4.CreateOrthographicOffCenter(0, Width, Height, 0, 0, 1);
			GL.LoadMatrix(ref m);

			GuiScreen?.OnResize(ClientSize);

			OnRenderFrame(new FrameEventArgs());
		}

		protected override void OnUpdateFrame(FrameEventArgs e)
		{
			if (discordEnabled)
			{
				try { discord.RunCallbacks(); }
				catch { }
			}
		}
		private PointF _previewNote;
		private bool _drawPreview = false;

		private bool GridContains(Point pos)
        {
			if (GuiScreen is GuiScreenEditor editor)
            {
				if (Settings.Default.Quantum)
				{
					var boundxmin = editor.Grid.ClientRectangle.X - editor.Grid.ClientRectangle.Width / 3;
					var boundxmax = editor.Grid.ClientRectangle.Right + editor.Grid.ClientRectangle.Width / 3;
					var boundymin = editor.Grid.ClientRectangle.Y - editor.Grid.ClientRectangle.Height / 3;
					var boundymax = editor.Grid.ClientRectangle.Bottom + editor.Grid.ClientRectangle.Height / 3;

					return pos.X <= boundxmax && pos.X >= boundxmin && pos.Y <= boundymax && pos.Y >= boundymin;
				}
				else
					return editor.Grid.ClientRectangle.Contains(pos);
			}
			
			return false;
        }
		protected override void OnMouseMove(MouseMoveEventArgs e)
		{
			_lastMouse = e.Position;

			GuiScreen?.OnMouseMove(e.X, e.Y);

			/* dumb lag machine
			if (_rightDown && GuiScreen is GuiScreenEditor g)
			{
				if (g.Track.ClientRectangle.Contains(_clickedMouse))
                {
					UpdateSelection(g);
				}
			}
			*/

			if (GuiScreen is GuiScreenEditor editor)
			{
				if (_placingNotes || ((!Settings.Default.SeparateClickTools || !SelectTool) && GridContains(e.Position)))
				{
					var pos = e.Position;
					var rect = editor.Grid.ClientRectangle;
					var increment = 1f;

					if (Settings.Default.Quantum)
						increment = (float)(editor.NoteAlign.Value + 1f) / 3f;

					var x = (float)((pos.X - (rect.X + (rect.Width / 2))) / rect.Width * 3) + 1;
					var y = (float)((pos.Y - (rect.Y + (rect.Height / 2))) / rect.Height * 3) + 1;

					if (editor.QuantumGridSnap.Toggle)
					{
						x = (float)(Math.Floor((x + 1 / increment / 2) * increment) / increment);
						y = (float)(Math.Floor((y + 1 / increment / 2) * increment) / increment);
					}

					x = (float)Math.Max(-0.850d, x);
					x = (float)Math.Min(2.850d, x);
					y = (float)Math.Max(-0.850d, y);
					y = (float)Math.Min(2.850d, y);
					if (_placingNotes)
					{
						_drawPreview = false;
						if (_lastPos.X == x && _lastPos.Y == y) return;
						_lastPos = new PointF(x, y);
						var note = new Note(x, y, (int)currentTime.TotalMilliseconds);
						Notes.Add(note);
						UndoRedo.AddUndoRedo("ADD NOTE", () =>
						{
							Notes.Remove(note);
						}, () =>
						{
							Notes.Add(note);
						});
						if (editor.AutoAdvance.Toggle)
						{
							var bpm = GetCurrentBpm(currentTime.TotalMilliseconds, false).bpm;
							if (bpm >= 1)
							{
								var beatDivisor = GuiTrack.BeatDivisor;

								var lineSpace = 60 / bpm;
								var stepSmall = lineSpace / beatDivisor * 1000;

								long closestBeat =
									GetClosestBeatScroll((long)currentTime.TotalMilliseconds, false, 1);

								if (GetCurrentBpm(currentTime.TotalMilliseconds, false).bpm == 0 && GetCurrentBpm(closestBeat, false).bpm != 0)
									closestBeat = GetCurrentBpm(closestBeat, false).Ms;

								try
								{
									currentTime = TimeSpan.FromMilliseconds(closestBeat);
									AlignTimeline();
								}
								catch
								{

								}
							}
						}
					} else if (!Settings.Default.SeparateClickTools || !SelectTool)
					{
						_drawPreview = true;
						_previewNote = new PointF(x, y);
					}
				} else
				{
					_drawPreview = false;
				}
				if (_draggingNoteTimeline)
				{
					var x = Math.Abs(e.X - _dragStartX) >= 5 ? e.X : _dragStartX;
					
					OnDraggingTimelineNotes(x);
				}
				if (_draggingPointTimeline)
                {
					var x = Math.Abs(e.X - _dragStartX) >= 5 ? e.X : _dragStartX;

					OnDraggingTimelinePoint(x);
                }
				if (editor.Timeline.Dragging)
				{
					editor.Timeline.Progress = Math.Max(0, Math.Min(1, (e.X - editor.ClientRectangle.Height / 2f) /
								   (editor.ClientRectangle.Width - editor.ClientRectangle.Height)));

					//MusicPlayer.Pause();

					var time = MathHelper.Clamp(totalTime.TotalMilliseconds * editor.Timeline.Progress, 0, totalTime.TotalMilliseconds - 1);
					currentTime = TimeSpan.FromMilliseconds(time);
					AlignTimeline();
				}
				if (editor.MasterVolume.Dragging)
				{
					var rect = editor.MasterVolume.ClientRectangle;
					var lineSize = rect.Height - rect.Width;
					var step = lineSize / editor.MasterVolume.MaxValue;

					var tick = MathHelper.Clamp(Math.Round((lineSize - (e.Y - rect.Y - rect.Width / 2)) / step), 0, editor.MasterVolume.MaxValue);

					editor.MasterVolume.Value = (int)tick;

					MusicPlayer.Volume = Math.Max(0,
						Math.Min(1, (float)tick / editor.MasterVolume.MaxValue));
				}
				if (editor.SfxVolume.Dragging)
				{
					var rect = editor.SfxVolume.ClientRectangle;
					var lineSize = rect.Height - rect.Width;
					var step = lineSize / editor.SfxVolume.MaxValue;

					var tick = MathHelper.Clamp(Math.Round((lineSize - (e.Y - rect.Y - rect.Width / 2)) / step), 0, editor.SfxVolume.MaxValue);

					editor.SfxVolume.Value = (int)tick;
				}
				if (editor.BeatSnapDivisor.Dragging)
				{
					var rect = editor.BeatSnapDivisor.ClientRectangle;
					var step = (rect.Width - rect.Height) / editor.BeatSnapDivisor.MaxValue;

					var tick = (int)MathHelper.Clamp(Math.Round((e.X - rect.X - rect.Height / 2) / step), 0, editor.BeatSnapDivisor.MaxValue);

					editor.BeatSnapDivisor.Value = tick;
					GuiTrack.BeatDivisor = tick + 1;
				}
				if (editor.Tempo.Dragging)
				{
					var rect = editor.Tempo.ClientRectangle;
					var step = (rect.Width - rect.Height) / editor.Tempo.MaxValue;

					var tick = (int)MathHelper.Clamp(Math.Round((e.X - rect.X - rect.Height / 2) / step), 0, editor.Tempo.MaxValue);

					editor.Tempo.Value = tick;

					if (tick > 15)
						tick = (tick - 16) * 2 + 16;

					MusicPlayer.Tempo = MathHelper.Clamp(0.2f + tick * 0.05f, 0.2f, 2);
					tempo = MathHelper.Clamp(0.2f + tick * 0.05f, 0.2f, 2);
				}
				if (editor.NoteAlign.Dragging)
				{
					var rect = editor.NoteAlign.ClientRectangle;
					var step = (rect.Width - rect.Height) / editor.NoteAlign.MaxValue;

					var tick = (int)MathHelper.Clamp(Math.Round((e.X - rect.X - rect.Height / 2) / step), 0, editor.NoteAlign.MaxValue);

					editor.NoteAlign.Value = tick;
				}
				if (editor.TrackHeight.Dragging)
                {
					var rect = editor.TrackHeight.ClientRectangle;
					var lineSize = rect.Height - rect.Width;
					var step = lineSize / editor.TrackHeight.MaxValue;

					var tick = MathHelper.Clamp(Math.Round((lineSize - (e.Y - rect.Y - rect.Width / 2)) / step), 0, editor.TrackHeight.MaxValue);

					editor.TrackHeight.Value = (int)tick;

					editor.OnResize(ClientSize);
                }
				if (editor.TrackCursorPos.Dragging)
                {
					var rect = editor.TrackCursorPos.ClientRectangle;
					var step = (rect.Width - rect.Height) / editor.TrackCursorPos.MaxValue;

					var tick = (int)MathHelper.Clamp(Math.Round((e.X - rect.X - rect.Height / 2) / step), 0, editor.TrackCursorPos.MaxValue);

					editor.TrackCursorPos.Value = tick;
					GuiTrack.CursorPos = editor.TrackCursorPos.Value;
                }
				if (editor.ApproachRate.Dragging)
				{
					var rect = editor.ApproachRate.ClientRectangle;
					var lineSize = rect.Height - rect.Width;
					var step = lineSize / editor.ApproachRate.MaxValue;

					var tick = MathHelper.Clamp(Math.Round((lineSize - (e.Y - rect.Y - rect.Width / 2)) / step), 0, editor.ApproachRate.MaxValue);

					editor.ApproachRate.Value = (int)tick;
					GuiGrid.ApproachRate = editor.ApproachRate.Value + 1;
				}

				if (_draggingNoteGrid)
				{
					OnDraggingGridNote(e.Position);
				}

				if (_draggingTimeline)
				{
					OnDraggingTimeline(e.X);
				}
			}

			if (GuiScreen is GuiScreenMenu menu)
            {
				if (menu.ScrollBar.Dragging)
                {
					var rect = menu.ScrollBar.ClientRectangle;
					var lineSize = rect.Height - rect.Width;
					var step = lineSize / menu.ScrollBar.MaxValue;

					var tick = MathHelper.Clamp(Math.Round((lineSize - (e.Y - rect.Y - rect.Width / 2)) / step), 0, menu.ScrollBar.MaxValue);

					menu.ScrollBar.Value = (int)tick;
					menu.AssembleChangelog();
				}
            }
		}

		protected override void OnMouseLeave(EventArgs e)
		{
			//_rightDown = false;

			if (GuiScreen is GuiScreenEditor editor)
				editor.OnMouseLeave();

			if (GuiScreen is GuiScreenMenu menu)
				menu.OnMouseLeave();
		}

		protected override void OnFocusedChanged(EventArgs e)
		{
			if (!Focused)
			{
				OnMouseLeave(null);
				OnMouseUp(new MouseButtonEventArgs(_lastMouse.X, _lastMouse.Y, MouseButton.Left, false));
			}
		}
		private bool _placingNotes = false; // pardon my bad practices -kermet
		private PointF _lastPos;
		protected override void OnMouseDown(MouseButtonEventArgs e)
		{
			_clickedMouse = e.Position;
			_mouseDownMs = (long)currentTime.TotalMilliseconds;

			if (e.Button == MouseButton.Right)
				_rightDown = true;

			if (GuiScreen is GuiScreenEditor editor)
			{
				if (!_rightDown)
				{
					_draggedNote = null;
					_draggedPoint = null;

					if (editor.Track.MouseOverNote is Note tn)
					{
						if (MusicPlayer.IsPlaying)
							MusicPlayer.Pause();

						_draggingNoteTimeline = true;

						_dragStartX = e.X;
						tn.DragStartMs = tn.Ms;

						_draggedNote = tn;

						if (_controlDown)
							if (_draggedNotes.Contains(tn))
								_draggedNotes.Remove(tn);
							else
								_draggedNotes.Add(tn);
						else if (_shiftDown && _draggedNotes.Count > 0)
						{
							var startms = _draggedNotes.FirstOrDefault().Ms;
							var endms = tn.Ms;
							_draggedNotes.Clear();
							foreach (var note in Notes.ToList())
							{
								if ((endms >= note.Ms && note.Ms >= startms) || (startms >= note.Ms && note.Ms >= endms))
									_draggedNotes.Add(note);
							}
						}
						else if (!_draggedNotes.Contains(tn))
                        {
							_draggedNotes.Clear();
							_draggedNotes.Add(tn);
                        }

						/*if (!SelectedNotes.Contains(tn))
						{
							SelectedNotes.Clear();

							SelectedNotes.Add(tn);
							//Console.WriteLine(SelectedNotes.Count);
						}*/

						SelectedNotes = _draggedNotes;

						foreach (var note in _draggedNotes)
						{
							note.DragStartMs = note.Ms;
						}

						_dragNoteStartMs = tn.Ms;
					}
					else if (editor.Grid.MouseOverNote is Note gn && (!Settings.Default.SeparateClickTools || SelectTool))
					{
						if (MusicPlayer.IsPlaying)
							MusicPlayer.Pause();

						_draggingNoteGrid = true;

						if (_controlDown)
							if (_draggedNotes.Contains(gn))
								_draggedNotes.Remove(gn);
							else
								_draggedNotes.Add(gn);
						else if (_shiftDown && _draggedNotes.Count > 0)
						{
							var startms = _draggedNotes.FirstOrDefault().Ms;
							var endms = gn.Ms;
							_draggedNotes.Clear();
							foreach (var note in Notes.ToList())
							{
								if ((endms >= note.Ms && note.Ms >= startms) || (startms >= note.Ms && note.Ms >= endms))
									_draggedNotes.Add(note);
							}
						}
						else if (!_draggedNotes.Contains(gn))
						{
							_draggedNotes.Clear();
							_draggedNotes.Add(gn);
						}

						/*if (!SelectedNotes.Contains(gn))
						{
							SelectedNotes.Clear();

							SelectedNotes.Add(gn);
						}*/

						_dragStartIndexX = _draggedNotes.FirstOrDefault().X;
						_dragStartIndexY = _draggedNotes.FirstOrDefault().Y;

						SelectedNotes = _draggedNotes;
					}
					else if (editor.CanClick(e.Position) && GridContains(e.Position) && (!Settings.Default.SeparateClickTools || !SelectTool))
					{
						_placingNotes = true;
						var pos = e.Position;
						var rect = editor.Grid.ClientRectangle;
						var increment = 1f;

						if (Settings.Default.Quantum)
							increment = (float)(editor.NoteAlign.Value + 1f) / 3f;

						var x = (float)((pos.X - (rect.X + (rect.Width / 2))) / rect.Width * 3) + 1;
						var y = (float)((pos.Y - (rect.Y + (rect.Height / 2))) / rect.Height * 3) + 1;

						if (editor.QuantumGridSnap.Toggle)
						{
							x = (float)(Math.Floor((x + 1 / increment / 2) * increment) / increment);
							y = (float)(Math.Floor((y + 1 / increment / 2) * increment) / increment);
						}

						x = (float)Math.Max(-0.850d, x);
						x = (float)Math.Min(2.850d, x);
						y = (float)Math.Max(-0.850d, y);
						y = (float)Math.Min(2.850d, y);
						_lastPos = new PointF(x, y);
						var note = new Note(x, y, (int)currentTime.TotalMilliseconds);
						Notes.Add(note);
						UndoRedo.AddUndoRedo("ADD NOTE", () =>
						{
							Notes.Remove(note);
						}, () =>
						{
							Notes.Add(note);
						});
						if (editor.AutoAdvance.Toggle)
						{
							var bpm = GetCurrentBpm(currentTime.TotalMilliseconds, false).bpm;
							if (bpm >= 1)
							{
								var beatDivisor = GuiTrack.BeatDivisor;

								var lineSpace = 60 / bpm;
								var stepSmall = lineSpace / beatDivisor * 1000;

								long closestBeat =
									GetClosestBeatScroll((long)currentTime.TotalMilliseconds, false, 1);

								if (GetCurrentBpm(currentTime.TotalMilliseconds, false).bpm == 0 && GetCurrentBpm(closestBeat, false).bpm != 0)
									closestBeat = GetCurrentBpm(closestBeat, false).Ms;

								try
								{
									currentTime = TimeSpan.FromMilliseconds(closestBeat);
									AlignTimeline();
								}
								catch
								{

								}
							}
						}
					}
					else if (editor.Track.MouseOverPoint is BPM pn)
                    {
						if (MusicPlayer.IsPlaying)
							MusicPlayer.Pause();

						_draggingPointTimeline = true;

						_dragStartX = e.X;
						pn.DragStartMs = pn.Ms;

						_draggedPoint = pn;

						_dragPointStartMs = pn.Ms;
                    }
					else if (editor.Track.ClientRectangle.Contains(e.Position))
					{
						_wasPlaying = MusicPlayer.IsPlaying;

						if (_wasPlaying)
							MusicPlayer.Pause();

						_draggingTimeline = true;
						_dragStartX = e.X;
						_dragStartMs = (int)currentTime.TotalMilliseconds;
					}
					else if (editor.MasterVolume.ClientRectangle.Contains(e.Position))
					{
						editor.MasterVolume.Dragging = true;
						OnMouseMove(new MouseMoveEventArgs(e.X, e.Y, 0, 0));
					}
					else if (editor.SfxVolume.ClientRectangle.Contains(e.Position))
					{
						editor.SfxVolume.Dragging = true;
						OnMouseMove(new MouseMoveEventArgs(e.X, e.Y, 0, 0));
					}
					else if (editor.BeatSnapDivisor.ClientRectangle.Contains(e.Position))
					{
						editor.BeatSnapDivisor.Dragging = true;
						OnMouseMove(new MouseMoveEventArgs(e.X, e.Y, 0, 0));
					}
					else if (editor.Tempo.ClientRectangle.Contains(e.Position))
					{
						editor.Tempo.Dragging = true;
						OnMouseMove(new MouseMoveEventArgs(e.X, e.Y, 0, 0));
					}
					else if (editor.NoteAlign.ClientRectangle.Contains(e.Position))
					{
						editor.NoteAlign.Dragging = true;
						OnMouseMove(new MouseMoveEventArgs(e.X, e.Y, 0, 0));
					}
					else if (editor.TrackHeight.ClientRectangle.Contains(e.Position) && editor.TrackHeight.Visible)
                    {
						editor.TrackHeight.Dragging = true;
						OnMouseMove(new MouseMoveEventArgs(e.X, e.Y, 0, 0));
                    }
					else if (editor.TrackCursorPos.ClientRectangle.Contains(e.Position) && editor.TrackCursorPos.Visible)
                    {
						editor.TrackCursorPos.Dragging = true;
						OnMouseMove(new MouseMoveEventArgs(e.X, e.Y, 0, 0));
                    }
					else if (editor.ApproachRate.ClientRectangle.Contains(e.Position) && editor.ApproachRate.Visible)
                    {
						editor.ApproachRate.Dragging = true;
						OnMouseMove(new MouseMoveEventArgs(e.X, e.Y, 0, 0));
                    }
					else if (editor.CanClick(e.Position))
					{
						SelectedNotes.Clear();
						_draggedNotes.Clear();

						_draggingNoteGrid = false;
						_draggingNoteTimeline = false;
					}
				}
				else if (editor.Track.ClientRectangle.Contains(e.Position))
				{
					SelectedNotes.Clear();
					_draggedNotes.Clear();
					_draggedPoint = null;

					_draggingNoteGrid = false;
					_draggingNoteTimeline = false;
					_draggingPointTimeline = false;
				}
				else if (editor.CanClick(e.Position))
				{
					SelectedNotes.Clear();
					_draggedNotes.Clear();

					_draggingNoteGrid = false;
					_draggingNoteTimeline = false;
				}

				if (editor.ClientRectangle.Contains(e.Position))
				{
					if (MusicPlayer.IsPlaying)
						MusicPlayer.Pause();
					editor.Timeline.Dragging = true;

					OnMouseMove(new MouseMoveEventArgs(e.X, e.Y, 0, 0));
				}
			}

			if (GuiScreen is GuiScreenMenu menu)
            {
				if (menu.ScrollBar.ClientRectangle.Contains(e.Position))
				{
					menu.ScrollBar.Dragging = true;
					OnMouseMove(new MouseMoveEventArgs(e.X, e.Y, 0, 0));
				}
			}

			if (e.Button == MouseButton.Left)
				GuiScreen?.OnMouseClick(e.X, e.Y);
		}

		protected override void OnMouseUp(MouseButtonEventArgs e)
		{
			if (_draggingNoteTimeline)
			{
				if (MusicPlayer.IsPlaying)
					MusicPlayer.Pause();

				if (_draggedNotes.Count > 0)
				{
					var notes = _draggedNotes.ToList();

					if (notes.FirstOrDefault() is Note note)
					{
						var start = note.DragStartMs;
						var diff = note.Ms - start;

						if (diff != 0)
						{
							Notes.Sort();

							long[] startMs = new long[notes.Count];

							for (var i = 0; i < startMs.Length; i++)
							{
								startMs[i] = notes[i].DragStartMs;
							}

							UndoRedo.AddUndoRedo("MOVE NOTE" + (_draggedNotes.Count > 1 ? "S" : ""), () =>
							{
								for (var index = 0; index < notes.Count; index++)
								{
									var note1 = notes[index];

									start = startMs[index];
									note1.Ms = start;
								}

								Notes.Sort();
							}, () =>
							{
								for (var index = 0; index < notes.Count; index++)
								{
									var note1 = notes[index];

									start = startMs[index];
									note1.Ms = start + diff;
								}

								Notes.Sort();
							});
						}
					}
				}
			}

			if (_draggingPointTimeline)
            {
				if (MusicPlayer.IsPlaying)
					MusicPlayer.Pause();

				if (_draggedPoint != null && _draggedPoint is BPM point)
                {
					var start = point.DragStartMs;
					var diff = point.Ms - start;

					if (diff != 0)
                    {
						UndoRedo.AddUndoRedo("MOVE POINT", () =>
						{
							point.Ms = start;
							ResetPoints(true);
						}, () =>
						{
							point.Ms = start + diff;
							ResetPoints(true);
						});

						ResetPoints(true);
					}
                }
            }

			if (_draggingNoteGrid && _draggedNotes.FirstOrDefault() is Note note2)
			{
				if (MusicPlayer.IsPlaying)
					MusicPlayer.Pause();
				//OnDraggingGridNote(_lastMouse);

				var xdiff = note2.X - _dragStartIndexX;
				var ydiff = note2.Y - _dragStartIndexY;

				if (note2.X != _dragStartIndexX || note2.Y != _dragStartIndexY)
				{
					var selectednotes = _draggedNotes.ToList();

					UndoRedo.AddUndoRedo("REPOSITION NOTE" + (_draggedNotes.Count > 1 ? "S" : ""), () =>
					{
						foreach (var note in selectednotes)
                        {
							note.X -= xdiff;
							note.Y -= ydiff;
                        }
					}, () =>
					{
						foreach (var note in selectednotes)
                        {
							note.X += xdiff;
							note.Y += ydiff;
                        }
					});
				}
			}

			if (_draggingTimeline)
			{
				if (_wasPlaying)
					MusicPlayer.Pause();
				OnDraggingTimeline(e.X);

				if (_wasPlaying)
					MusicPlayer.Play();
			}

			if (e.Button == MouseButton.Right)
				_rightDown = false;

			if (GuiScreen is GuiScreenEditor editor)
			{
				if (editor.MasterVolume.Dragging || editor.SfxVolume.Dragging || editor.TrackHeight.Dragging || editor.TrackCursorPos.Dragging || editor.ApproachRate.Dragging)
				{
					Settings.Default.MasterVolume = (decimal)editor.MasterVolume.Value / editor.MasterVolume.MaxValue;
					Settings.Default.SFXVolume = (decimal)editor.SfxVolume.Value / editor.SfxVolume.MaxValue;
					Settings.Default.TrackHeight = editor.TrackHeight.Value;
					Settings.Default.CursorPos = editor.TrackCursorPos.Value;
					Settings.Default.ApproachRate = editor.ApproachRate.Value;

					Settings.Default.Save();
				}

				editor.BeatSnapDivisor.Dragging = false;
				editor.MasterVolume.Dragging = false;
				editor.SfxVolume.Dragging = false;
				editor.Timeline.Dragging = false;
				editor.Tempo.Dragging = false;
				editor.NoteAlign.Dragging = false;
				editor.TrackHeight.Dragging = false;
				editor.TrackCursorPos.Dragging = false;
				editor.ApproachRate.Dragging = false;
			}

			if (GuiScreen is GuiScreenMenu menu)
            {
				menu.ScrollBar.Dragging = false;
            }

			_placingNotes = false;
			_draggingNoteTimeline = false;
			_draggingPointTimeline = false;
			_draggingNoteGrid = false;
			_draggingTimeline = false;
		}

		protected override void OnKeyPress(KeyPressEventArgs e)
		{
			GuiScreen.OnKeyTyped(e.KeyChar);
		}

		private bool CompareKeybind(Key keyf, EditorSettings.KeyType key)
        {
			return key.Key == keyf && key.CTRL == _controlDown && key.SHIFT == _shiftDown && key.ALT == _altDown;
        }

		private string CompareKeys(KeyboardKeyEventArgs e)
		{
			var key = e.Key;
			if (key == Key.BackSpace)
				key = Key.Delete;

			if (CompareKeybind(key, EditorSettings.SelectAll))
				return "SelectAll";
			if (CompareKeybind(key, EditorSettings.Save))
				return "Save";
			if (CompareKeybind(key, EditorSettings.SaveAs))
				return "SaveAs";
			if (CompareKeybind(key, EditorSettings.Undo))
				return "Undo";
			if (CompareKeybind(key, EditorSettings.Redo))
				return "Redo";
			if (CompareKeybind(key, EditorSettings.Copy))
				return "Copy";
			if (CompareKeybind(key, EditorSettings.Paste))
				return "Paste";
			if (CompareKeybind(key, EditorSettings.Delete))
				return "Delete";
			if (CompareKeybind(key, EditorSettings.HFlip))
				return "HFlip";
			if (CompareKeybind(key, EditorSettings.VFlip))
				return "VFlip";
			if (CompareKeybind(key, EditorSettings.SwitchClickTool))
				return "SwitchClickTool";
			if (CompareKeybind(key, EditorSettings.Quantum))
				return "Quantum";
			if (CompareKeybind(key, EditorSettings.OpenTimings))
				return "OpenTimings";
			if (CompareKeybind(key, EditorSettings.OpenBookmarks))
				return "OpenBookmarks";
			if (CompareKeybind(key, EditorSettings.StoreNodes))
				return "StoreNodes";
			if (CompareKeybind(key, EditorSettings.DrawBezier))
				return "DrawBezier";
			if (CompareKeybind(key, EditorSettings.AnchorNode))
				return "AnchorNode";

			if (key == Key.Number0)
				return "Pattern0";
			if (key == Key.Number1)
				return "Pattern1";
			if (key == Key.Number2)
				return "Pattern2";
			if (key == Key.Number3)
				return "Pattern3";
			if (key == Key.Number4)
				return "Pattern4";
			if (key == Key.Number5)
				return "Pattern5";
			if (key == Key.Number6)
				return "Pattern6";
			if (key == Key.Number7)
				return "Pattern7";
			if (key == Key.Number8)
				return "Pattern8";
			if (key == Key.Number9)
				return "Pattern9";

			return "";
        }

		private void CreatePattern(string pattern)
        {
			if (pattern == "")
				return;

			var culture = (CultureInfo)CultureInfo.CurrentCulture.Clone();
			culture.NumberFormat.NumberDecimalSeparator = ".";

			string[] patternsplit = pattern.Split(',');
			var toAdd = new List<Note>();
			
			foreach (var note in patternsplit)
            {
				string[] notesplit = note.Split('|');
				var x = float.Parse(notesplit[0], culture);
				var y = float.Parse(notesplit[1], culture);
				var time = int.Parse(notesplit[2], culture);

				var ms = GetClosestBeatScroll((long)currentTime.TotalMilliseconds, false, time);

				toAdd.Add(new Note(x, y, ms));
            }

			Notes.AddAll(toAdd);

			UndoRedo.AddUndoRedo("ADD NOTE" + (toAdd.Count > 1 ? "S" : ""), () =>
			{
				Notes.RemoveAll(toAdd);
			}, () =>
			{
				Notes.AddAll(toAdd);
			});
		}

		private string BindPattern(int key)
        {
			var culture = (CultureInfo)CultureInfo.CurrentCulture.Clone();
			culture.NumberFormat.NumberDecimalSeparator = ".";

			string pattern = "";
			long minDist = 0;

			for (int i = 0; i + 1 < SelectedNotes.Count; i++)
            {
				var dist = Math.Abs(SelectedNotes[i].Ms - SelectedNotes[i + 1].Ms);

				if (dist > 0)
					if (minDist > 0)
						minDist = Math.Min(minDist, dist);
					else
						minDist = dist;
            }

			foreach (var note in SelectedNotes)
            {
				var offset = SelectedNotes[0].Ms;

				var x = note.X.ToString(culture);
				var y = note.Y.ToString(culture);
				var time = (minDist > 0 ? Math.Round((double)(note.Ms - offset) / minDist) : 0).ToString(culture);

				pattern += $",{x}|{y}|{time}";
            }

			if (pattern.Length > 0)
				pattern = pattern.Substring(1, pattern.Length - 1);

			if (GuiScreen is GuiScreenEditor editor)
				editor.ShowToast($"BOUND PATTERN TO KEY {key}", Color1);

			return pattern;
        }

		protected override void OnKeyUp(KeyboardKeyEventArgs e)
		{
			_controlDown = e.Control || Keyboard.GetState().IsKeyDown(Key.ControlLeft) ||
									   Keyboard.GetState().IsKeyDown(Key.LControl);
			_altDown = e.Alt || Keyboard.GetState().IsKeyDown(Key.AltLeft) ||
									   Keyboard.GetState().IsKeyDown(Key.LAlt);
			_shiftDown = e.Shift || Keyboard.GetState().IsKeyDown(Key.ShiftLeft) ||
									   Keyboard.GetState().IsKeyDown(Key.LShift);
		}

		public List<Note> Bezier(List<Note> finalnodes, int divisor)
        {
			var finalnotes = new List<Note>();

			if (GuiScreen is GuiScreenEditor editor)
            {
				try
				{
					var k = finalnodes.Count - 1;
					decimal tdiff = finalnodes[k].Ms - finalnodes[0].Ms;
					decimal d = 1m / (divisor * k);
					if (!Settings.Default.CurveBezier)
						d = 1m / divisor;
					if (Settings.Default.CurveBezier)
					{
						for (decimal t = d; t <= 1; t += d)
						{
							float xf = 0;
							float yf = 0;
							decimal tf = finalnodes[0].Ms + tdiff * t;
							for (int v = 0; v <= k; v++)
							{
								var note = finalnodes[v];
								var bez = (double)editor.BinomialCoefficient(k, v) * (Math.Pow(1 - (double)t, k - v) * Math.Pow((double)t, v));

								xf += (float)(bez * note.X);
								yf += (float)(bez * note.Y);
							}
							finalnotes.Add(new Note(xf, yf, (long)tf));
						}
					}
					else
					{
						for (int v = 0; v < k; v++)
						{
							var note = finalnodes[v];
							var nextnote = finalnodes[v + 1];
							decimal xdist = (decimal)(nextnote.X - note.X);
							decimal ydist = (decimal)(nextnote.Y - note.Y);
							decimal tdist = nextnote.Ms - note.Ms;

							for (decimal t = 0; t < 1; t += d)
							{
								if (t > 0)
								{
									var xf = (decimal)note.X + xdist * t;
									var yf = (decimal)note.Y + ydist * t;
									var tf = note.Ms + tdist * t;

									finalnotes.Add(new Note((float)xf, (float)yf, (long)tf));
								}
							}
						}
					}
				}
				catch (OverflowException)
				{
					editor.ShowToast("TOO MANY NODES", Color.FromArgb(255, 200, 0));
					return null;
				}
				catch
				{
					editor.ShowToast("FAILED TO DRAW CURVE", Color.FromArgb(255, 200, 0));
					return null;
				}
			}

			return finalnotes;
        }

		public List<Note> CombineLists(List<Note> listA, List<Note> listB)
        {
			foreach (var note in listB)
				listA.Add(note);

			return listA;
        }

		public void UndoRedoBezier(List<Note> notes, List<Note> nodes)
        {
			Notes.RemoveAll(nodes);
			Notes.AddAll(notes);
			UndoRedo.AddUndoRedo("DRAW BEZIER", () =>
			{
				Notes.AddAll(nodes);
				Notes.RemoveAll(notes);
			}, () =>
			{
				Notes.RemoveAll(nodes);
				Notes.AddAll(notes);
			});
		}

		protected override void OnKeyDown(KeyboardKeyEventArgs e)
		{
			_controlDown = e.Control || Keyboard.GetState().IsKeyDown(Key.ControlLeft) ||
						   Keyboard.GetState().IsKeyDown(Key.LControl);
			_altDown = e.Alt || Keyboard.GetState().IsKeyDown(Key.AltLeft) ||
						   Keyboard.GetState().IsKeyDown(Key.LAlt);
			_shiftDown = e.Shift || Keyboard.GetState().IsKeyDown(Key.ShiftLeft) ||
						   Keyboard.GetState().IsKeyDown(Key.LShift);

			if (e.Key == Key.F11)
			{
				ToggleFullscreen();

				return;
			}

			GuiScreen.OnKeyDown(e.Key, e.Control);

			if (GuiScreen is GuiScreen gs && !gs.AllowInput())
				return;

			if (GuiScreen is GuiScreenEditor editor)
			{
				switch (CompareKeys(e))
				{
					case "SelectAll":
						Notes.Sort();

						SelectedNotes = Notes.ToList();
						_draggedNotes = Notes.ToList();
						return;
					case "Save":
						if (_file == null)
						{
							var wasPlaying = MusicPlayer.IsPlaying;

							if (wasPlaying)
								MusicPlayer.Pause();

							if (PromptSave())
							{
								editor.ShowToast("SAVED", Color1);
								currentData = ParseData(false);
							}

							if (wasPlaying)
								MusicPlayer.Play();
						}
						else
						{
							if (WriteFile(_file))
							{
								editor.ShowToast("SAVED", Color1);
								currentData = ParseData(false);
							}
						}
						return;
					case "SaveAs":
						var wasPlaying1 = MusicPlayer.IsPlaying;

						if (wasPlaying1)
							MusicPlayer.Pause();

						if (PromptSave())
						{
							editor.ShowToast("SAVED", Color1);
							currentData = ParseData(false);
						}

						if (wasPlaying1)
							MusicPlayer.Play();
						return;
					case "Undo":
						if (UndoRedo.CanUndo)
						{
							if (MusicPlayer.IsPlaying)
								MusicPlayer.Pause();

							UndoRedo.Undo();
						}
						return;
					case "Redo":
						if (UndoRedo.CanRedo)
						{
							if (MusicPlayer.IsPlaying)
								MusicPlayer.Pause();

							UndoRedo.Redo();
						}
						return;
					case "Copy":
						try
						{
							var copied = SelectedNotes.Select(n => n.Clone()).ToList();

							Clipboard.SetData("notes", copied);

							editor.ShowToast("COPIED NOTES", Color1);
						}
						catch
						{
							editor.ShowToast("FAILED TO COPY", Color1);
						}
						return;
					case "Paste":
						try
						{
							if (Clipboard.ContainsData("notes"))
							{
								if (MusicPlayer.IsPlaying)
									MusicPlayer.Pause();

								var copied = ((List<Note>)Clipboard.GetData("notes")).ToList();

								var lowest = copied.Min(n => n.Ms);

								copied.ForEach(n =>
									n.Ms = (long)currentTime.TotalMilliseconds + n.Ms - lowest);

								_draggedNotes.Clear();
								SelectedNotes.Clear();

								SelectedNotes.AddRange(copied);

								Notes.AddAll(copied);

								_draggingNoteGrid = false;
								_draggingNoteTimeline = false;

								UndoRedo.AddUndoRedo("PASTE NOTES", () =>
								{
									Notes.RemoveAll(copied);
								}, () =>
								{
									Notes.AddAll(copied);

									_draggedNotes.Clear();
									SelectedNotes.Clear();
									SelectedNotes.AddRange(copied);

									_draggingNoteGrid = false;
									_draggingNoteTimeline = false;
								});
							}
						}
						catch
						{
							editor.ShowToast("FAILED TO PASTE", Color1);
						}
						return;
					case "Delete":
						if (editor.AllowInput())
						{
							if (SelectedNotes.Count > 0)
                            {
								var toRemove = new List<Note>(SelectedNotes);

								Notes.RemoveAll(toRemove);

								UndoRedo.AddUndoRedo("DELETE NOTE" + (toRemove.Count > 1 ? "S" : ""), () =>
								{
									Notes.AddAll(toRemove);
								}, () =>
								{
									Notes.RemoveAll(toRemove);
								});

								SelectedNotes.Clear();
								_draggingNoteGrid = false;
								_draggingNoteTimeline = false;
							}
							if (_draggedPoint != null)
                            {
								var toRemove = new BPM(_draggedPoint.bpm, _draggedPoint.Ms);

								BPM pointtodel = new BPM(0,0);
								foreach (var point in GuiTrack.BPMs)
                                {
									if (point.bpm == toRemove.bpm && point.Ms == toRemove.Ms)
										pointtodel = point;
                                }
								GuiTrack.BPMs.Remove(pointtodel);

								GuiTrack.BPMs.Remove(toRemove);

								UndoRedo.AddUndoRedo("DELETE POINT", () =>
                                {
									GuiTrack.BPMs.Add(toRemove);

									ResetPoints(true);
								}, () =>
                                {
									BPM pointtodelr = new BPM(0, 0);
									foreach (var point in GuiTrack.BPMs)
									{
										if (point.bpm == toRemove.bpm && point.Ms == toRemove.Ms)
											pointtodelr = point;
									}
									GuiTrack.BPMs.Remove(pointtodelr);

									ResetPoints(true);
								});

								ResetPoints(true);
							}
						}
						return;
					case "HFlip":
						var selectedH = SelectedNotes.ToList();
						foreach (var node in selectedH)
						{
							node.X = 2 - node.X;
						}

						if (selectedH.Count > 0)
							editor.ShowToast("HORIZONTAL FLIP", Color1);

						UndoRedo.AddUndoRedo("HORIZONTAL FLIP", () =>
						{
							foreach (var node in selectedH)
							{
								node.X = 2 - node.X;
							}

						}, () =>
						{
							foreach (var node in selectedH)
							{
								node.X = 2 - node.X;
							}

						});
						return;
					case "VFlip":
						var selectedV = SelectedNotes.ToList();
						foreach (var node in selectedV)
						{
							node.Y = 2 - node.Y;
						}

						if (selectedV.Count > 0)
							editor.ShowToast("VERTICAL FLIP", Color1);

						UndoRedo.AddUndoRedo("VERTICAL FLIP", () =>
						{
							foreach (var node in selectedV)
							{
								node.Y = 2 - node.Y;
							}

						}, () =>
						{
							foreach (var node in selectedV)
							{
								node.Y = 2 - node.Y;
							}

						});
						return;
					case "SwitchClickTool":
						SelectTool = !SelectTool;
						Settings.Default.SelectTool = SelectTool;
						Settings.Default.Save();
						return;
					case "Quantum":
						editor.Quantum.Toggle = !editor.Quantum.Toggle;
						Settings.Default.Quantum = editor.Quantum.Toggle;
						Settings.Default.Save();
						return;
					case "OpenTimings":
						if (TimingsWindow.inst != null)
							TimingsWindow.inst.Close();
						new TimingsWindow().Show();
						return;
					case "OpenBookmarks":
						if (BookmarkSetup.inst != null)
							BookmarkSetup.inst.Close();
						new BookmarkSetup().Show();
						return;
					case "StoreNodes":
						if (SelectedNotes.Count > 1)
							editor.beziernodes = SelectedNotes.ToList();
						return;
					case "DrawBezier":
						if (int.TryParse(editor.BezierBox.Text, out var divisor) && divisor > 0 && ((editor.beziernodes != null && editor.beziernodes.Count > 1) || SelectedNotes.Count > 1))
						{
							var success = true;
							var finalnodes = SelectedNotes.ToList();
							if (editor.beziernodes != null && editor.beziernodes.Count > 1)
								finalnodes = editor.beziernodes;
							var finalnotes = new List<Note>();

							var anchored = new List<int>() { 0 };

							for (int i = 0; i < finalnodes.Count; i++)
							{
								if (finalnodes[i].Anchored && !anchored.Contains(i))
									anchored.Add(i);
							}
							if (!anchored.Contains(finalnodes.Count - 1))
								anchored.Add(finalnodes.Count - 1);

							for (int i = 1; i < anchored.Count; i++)
							{
								var newnodes = new List<Note>();
								for (int j = anchored[i - 1]; j <= anchored[i]; j++)
								{
									newnodes.Add(finalnodes[j]);
								}
								var finalbez = Bezier(newnodes, divisor);
								success = finalbez != null;
								if (success)
									finalnotes = CombineLists(finalnotes, finalbez);
							}

							SelectedNotes.Clear();
							if (!Settings.Default.CurveBezier)
								finalnodes = new List<Note>();
							else
								finalnotes.Add(finalnodes[0]);

							if (success)
								UndoRedoBezier(finalnotes, finalnodes);
						}

						for (int i = 0; i < Notes.Count; i++)
							Notes[i].Anchored = false;

						editor.beziernodes.Clear();
						return;
					case "AnchorNode":
						foreach (var note in SelectedNotes)
							note.Anchored = !note.Anchored;
						return;
					case "Pattern0":
						if (_shiftDown && SelectedNotes.Count > 0)
							EditorSettings.Pattern0 = BindPattern(0);
						else if (_controlDown)
                        {
							EditorSettings.Pattern0 = "";
							editor.ShowToast($"UNBOUND PATTERN 0", Color1);
						}
						else
							CreatePattern(EditorSettings.Pattern0);
						return;
					case "Pattern1":
						if (_shiftDown && SelectedNotes.Count > 0)
							EditorSettings.Pattern1 = BindPattern(1);
						else if (_controlDown)
						{
							EditorSettings.Pattern1 = "";
							editor.ShowToast($"UNBOUND PATTERN 1", Color1);
						}
						else
							CreatePattern(EditorSettings.Pattern1);
						return;
					case "Pattern2":
						if (_shiftDown && SelectedNotes.Count > 0)
							EditorSettings.Pattern2 = BindPattern(2);
						else if (_controlDown)
						{
							EditorSettings.Pattern2 = "";
							editor.ShowToast($"UNBOUND PATTERN 2", Color1);
						}
						else
							CreatePattern(EditorSettings.Pattern2);
						return;
					case "Pattern3":
						if (_shiftDown && SelectedNotes.Count > 0)
							EditorSettings.Pattern3 = BindPattern(3);
						else if (_controlDown)
						{
							EditorSettings.Pattern3 = "";
							editor.ShowToast($"UNBOUND PATTERN 3", Color1);
						}
						else
							CreatePattern(EditorSettings.Pattern3);
						return;
					case "Pattern4":
						if (_shiftDown && SelectedNotes.Count > 0)
							EditorSettings.Pattern4 = BindPattern(4);
						else if (_controlDown)
						{
							EditorSettings.Pattern4 = "";
							editor.ShowToast($"UNBOUND PATTERN 4", Color1);
						}
						else
							CreatePattern(EditorSettings.Pattern4);
						return;
					case "Pattern5":
						if (_shiftDown && SelectedNotes.Count > 0)
							EditorSettings.Pattern5 = BindPattern(5);
						else if (_controlDown)
						{
							EditorSettings.Pattern5 = "";
							editor.ShowToast($"UNBOUND PATTERN 5", Color1);
						}
						else
							CreatePattern(EditorSettings.Pattern5);
						return;
					case "Pattern6":
						if (_shiftDown && SelectedNotes.Count > 0)
							EditorSettings.Pattern6 = BindPattern(6);
						else if (_controlDown)
						{
							EditorSettings.Pattern6 = "";
							editor.ShowToast($"UNBOUND PATTERN 6", Color1);
						}
						else
							CreatePattern(EditorSettings.Pattern6);
						return;
					case "Pattern7":
						if (_shiftDown && SelectedNotes.Count > 0)
							EditorSettings.Pattern7 = BindPattern(7);
						else if (_controlDown)
						{
							EditorSettings.Pattern7 = "";
							editor.ShowToast($"UNBOUND PATTERN 7", Color1);
						}
						else
							CreatePattern(EditorSettings.Pattern7);
						return;
					case "Pattern8":
						if (_shiftDown && SelectedNotes.Count > 0)
							EditorSettings.Pattern8 = BindPattern(8);
						else if (_controlDown)
						{
							EditorSettings.Pattern8 = "";
							editor.ShowToast($"UNBOUND PATTERN 8", Color1);
						}
						else
							CreatePattern(EditorSettings.Pattern8);
						return;
					case "Pattern9":
						if (_shiftDown && SelectedNotes.Count > 0)
							EditorSettings.Pattern9 = BindPattern(9);
						else if (_controlDown)
						{
							EditorSettings.Pattern9 = "";
							editor.ShowToast($"UNBOUND PATTERN 9", Color1);
						}
						else
							CreatePattern(EditorSettings.Pattern9);
						return;
				}

				if ((e.Key == Key.Left || e.Key == Key.Right) && SelectedNotes.Count == 0)
				{
					if (MusicPlayer.IsPlaying)
						MusicPlayer.Pause();

					var bpm = GetCurrentBpm(currentTime.TotalMilliseconds, false).bpm;

					if (bpm > 0)
					{
						var beatDivisor = GuiTrack.BeatDivisor;

						var lineSpace = 60 / bpm;
						var stepSmall = lineSpace / beatDivisor * 1000;

						long closestBeat =
							GetClosestBeatScroll((long)currentTime.TotalMilliseconds, e.Key == Key.Left, 1);

						if (GetCurrentBpm(currentTime.TotalMilliseconds, false).bpm == 0 && GetCurrentBpm(closestBeat, false).bpm != 0)
							closestBeat = GetCurrentBpm(closestBeat, false).Ms;

						try
						{
							currentTime = TimeSpan.FromMilliseconds(closestBeat);
							AlignTimeline();
						}
						catch
						{

						}
					}
				}

				if (e.Shift && e.Control && e.Key == Key.M && (Notes.Count == 69 || inconspicuousvar))
				{
					inconspicuousvar = !inconspicuousvar;
					editor.ShowToast("funny mode " + (inconspicuousvar ? "on" : "off"), Color1);
				}

				//make sure to not register input while we're typing into a text box

				if (!MusicPlayer.IsPlaying && SelectedNotes.Count > 0 && _draggingNoteTimeline)
				{
					if (e.Key == Key.Left)
					{
						foreach (var node in SelectedNotes)
						{
							node.Ms--;
						}

						Notes.Sort();
					}
					if (e.Key == Key.Right)
					{
						foreach (var node in SelectedNotes)
						{
							node.Ms++;
						}

						Notes.Sort();
					}
				}

				if (e.Key == Key.Space)
				{
					if (!_draggingTimeline && !_draggingNoteTimeline && !editor.Timeline.Dragging)
					{
						if (MusicPlayer.IsPlaying)
						{
							MusicPlayer.Pause();
						}
						else
						{
							if (currentTime.TotalMilliseconds >= totalTime.TotalMilliseconds - 1)
								currentTime = TimeSpan.FromMilliseconds(0);
							AlignTimeline();
							MusicPlayer.Play();
						}
					}
				}

				if (!e.Control)
				{
					if (KeyMapping.TryGetValue(e.Key, out var tuple))
					{
						var note = new Note(tuple.Item1, tuple.Item2,
							(int)currentTime.TotalMilliseconds);

						Notes.Add(note);

						UndoRedo.AddUndoRedo("ADD NOTE", () =>
						{
							Notes.Remove(note);
						}, () =>
						{
							Notes.Add(note);
						});

						if (GuiScreen is GuiScreenEditor gse)
						{
							if (gse.AutoAdvance.Toggle)
							{
								var bpm = GetCurrentBpm(currentTime.TotalMilliseconds, false).bpm;
								if (bpm < 1)
								{
									return;

								}
								else
								{
									var beatDivisor = GuiTrack.BeatDivisor;

									var lineSpace = 60 / bpm;
									var stepSmall = lineSpace / beatDivisor * 1000;

									long closestBeat =
										GetClosestBeatScroll((long)currentTime.TotalMilliseconds, false, 1);

									if (GetCurrentBpm(currentTime.TotalMilliseconds, false).bpm == 0 && GetCurrentBpm(closestBeat, false).bpm != 0)
										closestBeat = GetCurrentBpm(closestBeat, false).Ms;

                                    try
                                    {
										currentTime = TimeSpan.FromMilliseconds(closestBeat);
										AlignTimeline();
									} catch
                                    {

                                    }
								}
							}
							else
							{

							}
						}
					}
				}
			}
		}

		protected override void OnMouseWheel(MouseWheelEventArgs e)
		{
			if (GuiScreen is GuiScreenEditor editor)
			{
				if (_controlDown)
				{
					if (Zoom < 0.1f || (Zoom == 0.1f && e.DeltaPrecise < 0))
						Zoom += e.DeltaPrecise * 0.01f;
					else
						Zoom += e.DeltaPrecise * 0.1f;
					Zoom = (float)Math.Round(Zoom, 2);
					if (Zoom > 0.1f)
						Zoom = (float)Math.Round(Zoom * 10) / 10;
				}
				else if (_shiftDown)
				{
					editor.BeatSnapDivisor.Value += (int)e.DeltaPrecise;
					editor.BeatSnapDivisor.Value = MathHelper.Clamp(editor.BeatSnapDivisor.Value, 0, editor.BeatSnapDivisor.MaxValue);
					GuiTrack.BeatDivisor = editor.BeatSnapDivisor.Value + 1;
				}
				else
				{
					if (MusicPlayer.IsPlaying)
						MusicPlayer.Pause();
					var time = (long)currentTime.TotalMilliseconds;
					var maxTime = (long)totalTime.TotalMilliseconds - 1;

					var closest = GetClosestBeatScroll(time, e.DeltaPrecise < 0, 1);
					var bpm = GetCurrentBpm(0, false);

					if (closest >= 0 || bpm.bpm > 33)
					{
						/*var bpmDivided = 60 / bpm.bpm * 1000 / GuiTrack.BeatDivisor;

						var offset = (bpmDivided + bpm.Ms) % bpmDivided;

						time += (long)(e.DeltaPrecise * bpmDivided);

						time = (long)(Math.Round((time - offset) / bpmDivided) * bpmDivided + offset);*/
						time = closest;
					}
					else
					{
						time += (long)(e.DeltaPrecise / 10 * 1000 / Zoom * 0.5f);
					}

					if (GetCurrentBpm(currentTime.TotalMilliseconds, false).bpm == 0 && GetCurrentBpm(time, false).bpm != 0)
						time = GetCurrentBpm(time, false).Ms;

					time = Math.Min(maxTime, Math.Max(0, time));

					currentTime = TimeSpan.FromMilliseconds(time);
					AlignTimeline();
				}
			}

			if (GuiScreen is GuiScreenMenu menu)
            {
				menu.ScrollBar.Value += (int)e.DeltaPrecise;
				menu.ScrollBar.Value = MathHelper.Clamp(menu.ScrollBar.Value, 0, menu.ScrollBar.MaxValue);
				menu.AssembleChangelog();
			}
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			EditorSettings.SaveSettings();
			if (TimingPoints.Instance != null)
				TimingPoints.Instance.Close();

			Settings.Default.Save();

			if (GuiScreen is GuiScreenEditor)
				WriteIniFile();

			e.Cancel = !WillClose();

			if (!e.Cancel)
			{
				BassManager.Dispose();
				ModelManager.Cleanup();
			}
		}

		public bool WillClose()
		{
			if (_soundId != "-1" && currentData != ParseData(false))
			{
				var wasPlaying = MusicPlayer.IsPlaying;

				if (wasPlaying)
					MusicPlayer.Pause();

				var wasFullscreen = IsFullscreen;

				if (IsFullscreen)
				{
					ToggleFullscreen();
				}

				var r = MessageBox.Show("oh hey there\nnice map\nwant to save it?", "Close", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);

				if (wasFullscreen)
				{
					ToggleFullscreen();
				}

				if (r == DialogResult.Yes)
					PromptSave();

				if (r == DialogResult.Cancel)
				{
					if (wasPlaying)
					{
						MusicPlayer.Play();
					}

					return false;
				}
			}
			Notes.Clear();
			return true;
		}

		private long GetClosestBeatScroll(long ms, bool negative, int iterations)
        {
			/*
			if (GuiTrack.BPMs.Count == 0 || GuiTrack.BPMs[0].Ms - 5 > ms)
				return -1;

			long closestms = -1;
			int index = 0;
			var bpmints = new List<long>();

			for (int i = 0; i < GuiTrack.BPMs.Count; i++)
			{
				var bpm = GuiTrack.BPMs[i];
				if (bpm.bpm > 33)
				{
					double curms = bpm.Ms;
					var nextms = totalTime.TotalMilliseconds;

					if (i + 1 < GuiTrack.BPMs.Count)
						nextms = GuiTrack.BPMs[i + 1].Ms;

					double interval = 60000 / bpm.bpm / GuiTrack.BeatDivisor;

					while (curms < nextms)
					{
						if (!bpmints.Contains((long)Math.Round(curms)))
							bpmints.Add((long)Math.Round(curms));
						curms += interval;
					}
				}
			}

			bpmints.Add((long)totalTime.TotalMilliseconds - 1);

			for (int i = 0; i < bpmints.Count; i++)
			{
				if (i == 0)
					closestms = bpmints[0];
				else
				{
					if (Math.Abs(closestms - ms) > Math.Abs(bpmints[i] - closestms) / 2)
					{
						closestms = bpmints[i];
						index = i;
					}
				}
			}

			if (next)
			{
				if (negative)
				{
					if (closestms < ms && Math.Abs(closestms - ms) > 5)
						return closestms;
					if (index > 0)
						closestms = bpmints[index - 1];
					else
						closestms = -1;
				}
				else
				{
					if (closestms > ms && Math.Abs(closestms - ms) > 5)
						return closestms;
					if (index + 1 < bpmints.Count)
						closestms = bpmints[index + 1];
				}
			}
			*/
			
			long closestms = GetClosestBeat(ms, false);

			if (GetCurrentBpm(closestms, false).bpm == 0)
				return -1;

			for (int i = 0; i < iterations; i++)
            {
				var curbpm = GetCurrentBpm(ms, negative);
				var interval = 60000 / curbpm.bpm / GuiTrack.BeatDivisor;

				if (negative)
				{
					closestms = GetClosestBeat(ms, true);

					if (closestms >= ms)
						closestms = GetClosestBeat(closestms - (long)interval, false);
				}
				else
				{
					if (closestms <= ms)
						closestms = GetClosestBeat(closestms + (long)interval, false);

					if (GetCurrentBpm(ms, false).Ms != GetCurrentBpm(closestms, false).Ms)
						closestms = GetCurrentBpm(closestms, false).Ms;
				}
				ms = closestms;
			}

			if (closestms < 0)
				return -1;

			closestms = (long)MathHelper.Clamp(closestms, 0, totalTime.TotalMilliseconds - 1);

			return closestms;
		}

		private long GetClosestBeat(long ms, bool draggingpoint)
		{
			/* wayyyy too much going on here
			var lastDiffMs = long.MaxValue;
			var closestMs = long.MaxValue;

			void CheckCloser(long beat)
			{
				var diffMs = Math.Abs(beat - ms);

				if (diffMs <= lastDiffMs) // TODO - used to only be '='
				{
					lastDiffMs = diffMs;

					closestMs = beat;
				}
			}

			if (!(GuiScreen is GuiScreenEditor gui)) return 0;

			var rect = gui.Track.ClientRectangle;

			float audioTime = (float)currentTime.TotalMilliseconds;
			float posX = audioTime / 1000 * CubeStep;

			var screenX = gui.Track.ScreenX;

			var bpm = GuiTrack.Bpm;
			float bpmOffset = GuiTrack.BpmOffset;
			var beatDivisor = GuiTrack.BeatDivisor;

			var lineSpace = 60 / bpm * CubeStep;
			var stepSmall = lineSpace / beatDivisor;

			var lineX = screenX - posX + bpmOffset / 1000 * CubeStep;
			if (lineX < 0)
				lineX %= lineSpace;

			while (lineSpace > 0 && lineX < rect.Width)
			{
				//bpm line
				var timelineMs = (long)Math.Floor((decimal)(lineX - screenX + posX) / (decimal)CubeStep * 1000);

				if (timelineMs != long.MaxValue && timelineMs != long.MinValue)
					CheckCloser(timelineMs);

				for (int j = 1; j <= beatDivisor; j++)
				{
					var xo = Math.Floor(lineX + j * stepSmall);

					if (j < beatDivisor)
					{
						//divided bpm line
						timelineMs = (long)Math.Floor((xo - screenX + posX) / CubeStep * 1000);

						if (timelineMs != long.MaxValue && timelineMs != long.MinValue)
							CheckCloser(timelineMs); //beats.Add(timelineMs);
					}
				}

				lineX += lineSpace;
			}

			return closestMs;
			

			var currentbpm = GetCurrentBpm(ms, draggingpoint);
			float interval = 60000 / currentbpm.bpm / GuiTrack.BeatDivisor;
			float remainder = (ms - currentbpm.Ms) % interval;
			float closestms = ms - remainder;

			if (remainder > interval / 2)
				closestms += interval;

			if (next)
				if (negative)
					closestms -= interval;
				else
					closestms += interval;
			*/
			long closestms = -1;

			var bpm = GetCurrentBpm(ms, draggingpoint);

			if (bpm.bpm > 33)
			{
				var bpmDivided = 60 / bpm.bpm * 1000 / GuiTrack.BeatDivisor;

				var offset = bpm.Ms % bpmDivided;

				closestms = (long)Math.Round((long)Math.Round((ms - offset) / bpmDivided) * bpmDivided + offset);
			}

			return closestms;
		}

		private long GetClosestNote(long ms)
        {
			double closestms = -1;

			foreach (var note in Notes.ToList())
            {
				if (Math.Abs(note.Ms - ms) < Math.Abs(closestms - ms))
					closestms = note.Ms;
            }

			return (long)Math.Round(closestms);
        }

		private void OnDraggingTimelineNotes(int mouseX)
		{
			var pixels = mouseX - _dragStartX;
			var msDiff = pixels / CubeStep * 1000;

			var audioTime = (float)currentTime.TotalMilliseconds;

			if (GuiScreen is GuiScreenEditor gui && _draggedNote != null)
			{
				var clickMs = (int)(Math.Max(0, _clickedMouse.X - gui.Track.ScreenX + audioTime / 1000 * CubeStep) / CubeStep * 1000);
				var clickOff = clickMs - _dragNoteStartMs;
				var cursorMs = (int)(Math.Max(0, mouseX - gui.Track.ScreenX + audioTime / 1000 * CubeStep) / CubeStep * 1000) - clickOff;

				if (_draggedNotes.Count > 0 && GetCurrentBpm(cursorMs, false).bpm > 0)
				{
					var lineSpace = 60 / GetCurrentBpm(cursorMs, false).bpm * CubeStep;
					var stepSmall = lineSpace / GuiTrack.BeatDivisor;
					var snap = stepSmall / 1.75f;

					float threshold = snap;

					if (snap < 1)
						threshold = 1;
					else if (snap > 12)
						threshold = 12;

					var snappedMs = GetClosestBeat(_draggedNote.Ms, false);

					if (Math.Abs(snappedMs - cursorMs) / 1000f * CubeStep <= threshold) //8 pixels
						msDiff = -(_draggedNote.DragStartMs - snappedMs);
				}

				foreach (var note in _draggedNotes)
				{
					var time = note.DragStartMs + (int)msDiff;

					time = (int)Math.Max(0, Math.Min(totalTime.TotalMilliseconds, time));

					note.Ms = time;
				}

				Notes.Sort();
			}
		}

		private void OnDraggingTimelinePoint(int mouseX)
        {
			var pixels = mouseX - _dragStartX;
			var msDiff = pixels / CubeStep * 1000;

			var audioTime = (float)currentTime.TotalMilliseconds;

			if (GuiScreen is GuiScreenEditor gui && _draggedPoint != null)
            {
				var clickMs = (int)((_clickedMouse.X - gui.Track.ScreenX + audioTime / 1000 * CubeStep) / CubeStep * 1000);
				var clickOff = clickMs - _dragPointStartMs;
				var cursorMs = (int)((mouseX - gui.Track.ScreenX + audioTime / 1000 * CubeStep) / CubeStep * 1000 - clickOff);

				var lineSpace = 60 / GetCurrentBpm(cursorMs, true).bpm * CubeStep;
				var stepSmall = lineSpace / GuiTrack.BeatDivisor;
				var snap = stepSmall / 1.75f;

				float threshold = snap;

				if (snap < 1)
					threshold = 1;
				else if (snap > 12)
					threshold = 12;

				var snappedMs = GetClosestBeat(_draggedPoint.Ms, true);
				var snappedNote = GetClosestNote(_draggedPoint.Ms);

				if (Math.Abs(snappedNote - cursorMs) < Math.Abs(snappedMs - cursorMs))
					snappedMs = snappedNote;

				if (Math.Abs(snappedMs - cursorMs) / 1000f * CubeStep <= threshold) //8 pixels
					msDiff = -(_draggedPoint.DragStartMs - snappedMs);

				if (Math.Abs(cursorMs) / 1000f * CubeStep <= threshold) //8 pixels
					msDiff = -_draggedPoint.DragStartMs;

				var time = _draggedPoint.DragStartMs + (int)msDiff;

				time = (int)Math.Min(totalTime.TotalMilliseconds, time);

				_draggedPoint.Ms = time;

				ResetPoints(false);
			}
        }

		private void OnDraggingGridNote(Point pos)
		{
			if (GuiScreen is GuiScreenEditor editor && _draggedNotes.FirstOrDefault() is Note note)
			{
				var rect = editor.Grid.ClientRectangle;
				if (GuiScreen is GuiScreenEditor gse)
				{
					if (gse.Quantum.Toggle)
					{
						var increment = (float)(gse.NoteAlign.Value + 1f) / 3f;

						var newX = (float)((pos.X - (rect.X + (rect.Width / 2))) / rect.Width * 3) + 1;
						var newY = (float)((pos.Y - (rect.Y + (rect.Height / 2))) / rect.Height * 3) + 1;

						if (editor.QuantumGridSnap.Toggle)
                        {
							newX = (float)(Math.Floor((newX + 1 / increment / 2) * increment) / increment);
							newY = (float)(Math.Floor((newY + 1 / increment / 2) * increment) / increment);
						}

						var xdiff = newX - note.X;
						var ydiff = newY - note.Y;

						var maxX = note.X;
						var minX = note.X;
						var maxY = note.Y;
						var minY = note.Y;

						foreach (var selectednote in SelectedNotes)
						{
							if (selectednote != note)
							{
								maxX = (float)Math.Max(selectednote.X, maxX);
								minX = (float)Math.Min(selectednote.X, minX);
								maxY = (float)Math.Max(selectednote.Y, maxY);
								minY = (float)Math.Min(selectednote.Y, minY);
							}
						}
						
						xdiff = (float)Math.Max(-0.850d, minX + xdiff) - minX;
						xdiff = (float)Math.Min(2.850d, maxX + xdiff) - maxX;
						ydiff = (float)Math.Max(-0.850d, minY + ydiff) - minY;
						ydiff = (float)Math.Min(2.850d, maxY + ydiff) - maxY;

						note.X += xdiff;
						note.Y += ydiff;

						foreach (var selectednote in SelectedNotes)
						{
							if (selectednote != note)
							{
								selectednote.X += xdiff;
								selectednote.Y += ydiff;
							}
						}
					}
					else
					{
						var newX = (int)Math.Floor((pos.X - rect.X) / rect.Width * 3);
						var newY = (int)Math.Floor((pos.Y - rect.Y) / rect.Height * 3);

						var selectedatedge = false;

						foreach (var selectednote in SelectedNotes)
						{
							if (selectednote != note)
							{
								selectednote.X -= note.X;
								selectednote.Y -= note.Y;

								if (selectednote.X + newX < 0 || selectednote.X + newX > 2 || selectednote.Y + newY < 0 || selectednote.Y + newY > 2)
                                {
									selectedatedge = true;
                                }
							}
						}

						if (newX < 0 || newX > 2 || newY < 0 || newY > 2 || selectedatedge)
						{
							foreach (var selectednote in SelectedNotes)
							{
								if (selectednote != note)
								{
									selectednote.X += note.X;
									selectednote.Y += note.Y;
								}
							}
							return;
						}

						foreach (var selectednote in SelectedNotes)
						{
							if (selectednote != note)
							{
								selectednote.X += newX;
								selectednote.Y += newY;
							}
						}

						note.X = newX;
						note.Y = newY;
					}
				}
			}
		}

		private void OnDraggingTimeline(int mouseX)
		{
			if (GuiScreen is GuiScreenEditor editor)
			{
				var pixels = mouseX - _dragStartX;
				var msDiff = pixels / CubeStep * 1000;

				var time = _dragStartMs - (int)msDiff;

				if (GetCurrentBpm(time, false).bpm > 33)
					time = GetClosestBeat(time, false);
				/*
				var bpm = GetCurrentBpm(time, false);

				if (bpm.bpm > 33 && time >= bpm.Ms && time < totalTime.TotalMilliseconds - 1)
				{
					var bpmDivided = 60 / bpm.bpm * 1000 / GuiTrack.BeatDivisor;

					var offset = (bpmDivided + bpm.Ms) % bpmDivided;

					time = (long)Math.Round((long)Math.Round(time / (decimal)bpmDivided) * bpmDivided + offset);
				}
				*/
				time = (int)Math.Max(0, Math.Min(totalTime.TotalMilliseconds - 1, time));

				currentTime = TimeSpan.FromMilliseconds(time);
				AlignTimeline();
			}
		}

		private Rectangle UpdateSelection(GuiScreenEditor gse)
		{
			var startX = _clickedMouse.X;

			var time = (long)currentTime.TotalMilliseconds;
			var over = _mouseDownMs - time;
			var offset = over / 1000M * (decimal)CubeStep;

			startX += (int)offset;

			var x = Math.Min(_lastMouse.X, startX);
			var y = Math.Min(_lastMouse.Y, _clickedMouse.Y);

			var w = Math.Max(_lastMouse.X, startX) - x;
			var h = Math.Min((int)gse.Track.ClientRectangle.Height, Math.Max(_lastMouse.Y, _clickedMouse.Y)) - y;

			var rect = new Rectangle(x, y, w, h);

			var list = gse.Track.GetNotesInRect(rect);

			SelectedNotes = list;
			//Console.WriteLine(SelectedNotes.Count);
			_draggedNotes = new List<Note>(list);

			return rect;
		}

		public void LoadFile(string file)
		{
			var data = File.ReadAllText(file);

			try
			{
				var wc = new SecureWebClient();
				while (true)
				{
					data = wc.DownloadString(data);
				}
			}
			catch
			{
				
			}

			if (LoadMap(data, true) && GuiScreen is GuiScreenEditor gse)
			{
				_file = file;
				Settings.Default.LastFile = file;
				Settings.Default.Save();

				GuiSliderTimeline.Bookmarks.Clear();
				GuiTrack.BPMs.Clear();
				gse.Offset.Text = "0";
				GuiTrack.NoteOffset = 0;
				gse.BeatSnapDivisor.Value = 3;
				GuiTrack.BeatDivisor = 4;

				UpdateActivity(Path.GetFileName(_file));

				var ini = Path.ChangeExtension(file, "ini");

				if (File.Exists(ini))
				{
					var lines = File.ReadAllLines(ini);
					var oldformat = false;

					foreach (var line in lines)
					{
						if (line.Contains('='))
						{
							var splits = line.Split('=');

							if (splits.Length == 2)
							{
								var property = splits[0].Trim().ToLower();
								var value = splits[1].Trim().ToLower();

								if (property == "bookmarks")
								{
									var bookmarks = value.Split(',');
									foreach (var item in bookmarks)
									{
										var values = item.Split('|');
										if (values.Count() == 2 && int.TryParse(values[1], out var ms) && ms > 0)
										{
											GuiSliderTimeline.Bookmarks.Add(new Bookmark(values[0], ms));
										}
									}
								}
								else if (property == "bpm" /*&& decimal.TryParse(value, out var bpm)*/)
								{
									var culture = (CultureInfo)CultureInfo.CurrentCulture.Clone();
									culture.NumberFormat.NumberDecimalSeparator = ".";
									var bpmsplit = value.Split(',');
									foreach (var item in bpmsplit)
                                    {
										var bpmms = item.Split('|');
										if (float.TryParse(bpmms[0], NumberStyles.Float, culture, out var bpm) && bpm > 0)
                                        {
											if (bpmms.Count() > 1 && int.TryParse(bpmms[1], out var ms))
												GuiTrack.BPMs.Add(new BPM(bpm, ms));
											else
                                            {
												GuiTrack.BPMs.Add(new BPM(bpm, 0));
												oldformat = true;
											}
										}

										ResetPoints(true);
									}

									//GuiTrack.Bpm = (float)bpm;
								}
								else if (property == "time" && long.TryParse(value, out var time))
								{
									//gse.Timeline.Value = (int)(time / totalTime.TotalMilliseconds);
									currentTime = TimeSpan.FromMilliseconds(time);
									AlignTimeline();
								}
								else if (property == "divisor" && long.TryParse(value, out var divisor))
                                {
									gse.BeatSnapDivisor.Value = (int)divisor - 1;

									GuiTrack.BeatDivisor = (int)divisor;
                                }
								else if (property == "offset" && long.TryParse(value, out var offset))
                                {
									if (oldformat)
                                    {
										GuiTrack.BPMs[0].Ms = offset;
										offset = 0;
                                    }
									GuiTrack.NoteOffset = offset;
									gse.Offset.Text = offset.ToString();

									foreach (var note in Notes.ToList())
										note.Ms -= offset;
								}
								else if (property == "legacybpm" && float.TryParse(value, out var legacybpm))
                                {
									GuiTrack.Bpm = legacybpm;
                                }
								else if (property == "legacyoffset" && long.TryParse(value, out var legacyoffset))
                                {
									GuiTrack.BpmOffset = legacyoffset;
                                }
							}
						}
					}
				}
			}
		}

		public void AlignTimeline()
        {
			if (GuiScreen is GuiScreenEditor gse)
				gse.Timeline.Progress = (float)(currentTime.TotalMilliseconds / totalTime.TotalMilliseconds);
        }

		public bool LoadMap(string data, bool fromFile)
		{
			_file = null;

			Notes.Clear();

			SelectedNotes.Clear();
			_draggedNotes.Clear();

			_draggingNoteGrid = false;
			_draggingNoteTimeline = false;

			_draggedNote = null;

			if (!fromFile)
			{
				UpdateActivity("Untitled");
			}

			var splits = Regex.Matches(data, "([^,]+)");

			try
			{
				var id = splits[0];

				for (int i = 1; i < splits.Count; i++)
				{
					var chunk = splits[i];

					var chunkSplit = Regex.Matches(chunk.Value, "([^|]+)");
					var culture = (CultureInfo)CultureInfo.CurrentCulture.Clone();
					culture.NumberFormat.NumberDecimalSeparator = ".";
					//old var x = 2 - float.Parse(chunkSplit[0].Value.Replace(',', '.'));
					//old var y = 2 - float.Parse(chunkSplit[1].Value.Replace(',', '.'));
					var x = 2 - float.Parse(chunkSplit[0].Value, culture);
					var y = 2 - float.Parse(chunkSplit[1].Value, culture);
					var ms = long.Parse(chunkSplit[2].Value);

					Notes.Add(new Note(x, y, ms));
				}
				_soundId = id.Value;
				if (LoadSound(_soundId))
				{
					MusicPlayer.Load(cacheFolder + _soundId + ".asset");
					totalTime = MusicPlayer.TotalTime;

					var gui = new GuiScreenEditor();

					OpenGuiScreen(gui);
				}
				else
					_soundId = "-1";

				GuiTrack.BPMs.Clear();
				GuiTrack.NoteOffset = 0;
				GuiTrack.TextBpm = 0;

				currentTime = TimeSpan.Zero;
				currentData = data;
			}
			catch
			{
				MessageBox.Show("An error has occured while loading map data.");
				return false;
			}

			return _soundId != "-1";
		}

		public void CreateMap(string id)
		{
			LoadMap(id, false);
		}

		public BPM GetCurrentBpm(double currentms, bool draggingbpm)
        {
			if (!Settings.Default.LegacyBPM)
            {
				BPM currentbpm = new BPM(0, 0);

				foreach (var bpm in GuiTrack.BPMs)
                {
					if ((!draggingbpm && bpm.Ms <= currentms) || (draggingbpm && bpm.Ms < currentms))
						currentbpm = bpm;
				}

				return currentbpm;
			}
			else
            {
				return new BPM(GuiTrack.Bpm, GuiTrack.BpmOffset);
            }
        }

		private bool PromptSave()
		{
			using (var sfd = new SaveFileDialog
			{
				Title = "Save map",
				Filter = "Text Documents (*.txt)|*.txt"
			})
			{
				if (_file != null)
				{
					sfd.InitialDirectory = Path.GetDirectoryName(_file);

					sfd.FileName = Path.GetFileNameWithoutExtension(_file);
				}

				var wasFullscreen = IsFullscreen;

				if (IsFullscreen)
				{
					ToggleFullscreen();
				}

				var result = sfd.ShowDialog();

				if (wasFullscreen)
				{
					ToggleFullscreen();
				}

				if (result == DialogResult.OK)
				{
					_file = sfd.FileName;

					WriteFile(sfd.FileName);

					return true;
				}
			}

			return false;
		}

		public string ParseData(bool copy)
		{
			try
			{
				var sb = new StringBuilder();

				sb.Append(_soundId.ToString());

				for (int i = 0; i < Notes.Count; i++)
				{
					Note note = Notes[i];
					var ms = note.Ms + GuiTrack.NoteOffset;
					var culture = (CultureInfo)CultureInfo.CurrentCulture.Clone();
					culture.NumberFormat.NumberDecimalSeparator = ".";
					var gridX = Math.Round(2 - note.X, 2);
					var gridY = Math.Round(2 - note.Y, 2);
					if (copy && EditorSettings.CorrectOnCopy)
                    {
						gridX = MathHelper.Clamp(gridX, -0.850, 2.850);
						gridY = MathHelper.Clamp(gridY, -0.850, 2.850);
						ms = (long)MathHelper.Clamp(ms, 0, totalTime.TotalMilliseconds - 1);
                    }
					string xp = gridX.ToString(culture);
					string yp = gridY.ToString(culture);

					sb.Append($",{xp}|{yp}|{ms}");
				}
				return sb.ToString();
			}
			catch (Exception e)
			{
				MessageBox.Show(e.StackTrace, "Error parsing map data", MessageBoxButtons.OK,
	MessageBoxIcon.Error);
				return "false";
			}
		}

		private bool WriteFile(string file)
		{
			if (file == null)
				return false;
			
			try
			{
				var data = ParseData(false);

				File.WriteAllText(file, data, Encoding.UTF8);

				WriteIniFile();

				Settings.Default.LastFile = file;
				Settings.Default.Save();
			}
			catch { return false; }

			return true;
		}
		private string BookmarksToString()
		{
			string final = "";

			foreach (var bookmark in GuiSliderTimeline.Bookmarks)
			{
				var nameEsc = bookmark.Name.Replace('|','-').Replace(',','-');
				var msEsc = bookmark.MS.ToString();
				final += $",{nameEsc}|{msEsc}";
			}

			if (final.Length > 0)
				final = final.Substring(1, final.Length - 1);

			return final;
		}
		private string BpmsToString()
        {
			string final = "";

			foreach (var bpm in GuiTrack.BPMs)
            {
				var culture = (CultureInfo)CultureInfo.CurrentCulture.Clone();
				culture.NumberFormat.NumberDecimalSeparator = ".";
				var bpmf = bpm.bpm.ToString(culture);
				var msf = bpm.Ms.ToString(culture);
				final += $",{bpmf}|{msf}";
            }

			if (final.Length > 0)
				final = final.Substring(1, final.Length - 1);

			return final;
        }

		private void WriteIniFile()
		{
			if (_file == null)
				return;
			
			var iniFile = Path.ChangeExtension(_file, ".ini");

			File.WriteAllLines(iniFile, new[] { $@"BPM={BpmsToString()}", $@"Bookmarks={BookmarksToString()}", $@"Offset={GuiTrack.NoteOffset}", $@"LegacyBPM={GuiTrack.Bpm}", $@"LegacyOffset={GuiTrack.BpmOffset}", $@"Time={(long)currentTime.TotalMilliseconds}", $@"Divisor={GuiTrack.BeatDivisor}" }, Encoding.UTF8);
		}

		private bool LoadSound(string id)
		{
			try
			{
				if (!Directory.Exists(cacheFolder))
					Directory.CreateDirectory(cacheFolder);

				if (!File.Exists(cacheFolder + id + ".asset"))
				{
					using (var wc = new SecureWebClient())
					{
						wc.DownloadFile("https://assetdelivery.roblox.com/v1/asset/?id=" + id, cacheFolder + id + ".asset");
					}
				}

				return true;
			}
			catch (Exception e)
			{
				var message = MessageBox.Show($"Failed to download asset with id '{id}':\n\n{e.Message}\n\nWould you like to import a file with this id instead?", "Error", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
				if (message == DialogResult.OK)
                {
					using (var dialog = new OpenFileDialog
					{
						Title = "Select Audio File",
						Filter = "Audio Files (*.mp3;*.ogg;*.wav;*.flac;*.asset)|*.mp3;*.ogg;*.wav;*.flac;*.asset"
					})
					{
						if (dialog.ShowDialog() == DialogResult.OK)
                        {
							File.Copy(dialog.FileName, cacheFolder + id + ".asset", true);
							return true;
						}
					}
				}
			}

			return false;
		}

		private void Autosave()
		{
			if (GuiScreen is GuiScreenEditor editor && Notes.Count > 0)
			{
				if (_file == null)
				{
					Settings.Default.AutosavedFile = ParseData(false);

					editor.ShowToast("AUTOSAVED", Color1);
				}
				else if (WriteFile(_file))
				{
					editor.ShowToast("AUTOSAVED", Color1);
					currentData = ParseData(false);
				}
			}
		}

		private void RunAutosave()
        {
			Autosaving = true;
			if (EditorSettings.EnableAutosave && GuiScreen is GuiScreenEditor)
			{
				var delay = Task.Delay(EditorSettings.AutosaveInterval * 60000).ContinueWith(_ =>
				{
					Autosave();
					RunAutosave();
				});
			}
			else
				Autosaving = false;
        }

		public void OpenGuiScreen(GuiScreen s)
		{
			if (GuiScreen is GuiScreenEditor)
			{
				currentData = null;

				_brightness = 0;
				_draggedNote = null;

				_rightDown = false;
				_controlDown = false;
				_altDown = false;
				_draggingNoteTimeline = false;
				_draggingNoteGrid = false;
				_draggingTimeline = false;

				_wasPlaying = false;
				_soundId = "-1";

				_file = null;
			}

			GuiScreen?.OnClosing();

			IsPaused = s != null && s.Pauses;

			GuiScreen = s;

			if (!Autosaving)
				RunAutosave();
		}
	}

	class SecureWebClient : WebClient
	{
		protected override WebRequest GetWebRequest(Uri address)
		{
			HttpWebRequest request = base.GetWebRequest(address) as HttpWebRequest;
			if (request != null)
            {
				request.UserAgent = "RobloxProxy";
				request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
			}
			return request;
		}
	}

	class NoteList
	{
		private List<Note> _notes = new List<Note>();

		public int Count
		{
			get
			{
				lock (_notes)
				{
					return _notes.Count;
				}
			}
		}

		public void Add(Note note)
		{
			lock (_notes)
			{
				_notes.Add(note);
			}

			Sort();
		}

		public void Remove(Note note)
		{
			lock (_notes)
			{
				_notes.Remove(note);
			}

			Sort();
		}

		public void Clear()
		{
			lock (_notes)
			{
				_notes.Clear();
			}
		}

		private Color MergeColors(Color c1, Color c2)
        {
			var r = (c1.R + c2.R) / 2;
			var g = (c1.G + c2.G) / 2;
			var b = (c1.B + c2.B) / 2;

			return Color.FromArgb(1, r, g, b);
        }

		private bool IsMegaNote(int i)
        {
			var ms = _notes[i].Ms;

			return (i - 1 >= 0 && _notes[i - 1].Ms == ms) || (i + 1 < _notes.Count() && _notes[i + 1].Ms == ms);
        }

		public void Sort()
		{
			lock (_notes)
			{
				_notes = new List<Note>(_notes.OrderBy(n => n.Ms));
				for (int i = 0; i < _notes.Count; i++)
                {
					if (IsMegaNote(i))
						_notes[i].Color = MergeColors(EditorSettings.NoteColor1, EditorSettings.NoteColor2);
					else
                    {
						if (i % 2 == 0)
							_notes[i].Color = EditorSettings.NoteColor2;
						else
							_notes[i].Color = EditorSettings.NoteColor1;
					}
                }
			}
		}

		public Note LastOrDefault(Func<Note, bool> predicate)
		{
			lock (_notes)
			{
				return _notes.LastOrDefault(predicate);
			}
		}

		public void RemoveAll(List<Note> notes)
		{
			lock (_notes)
			{
				foreach (var note in notes)
				{
					_notes.Remove(note);
				}
			}

			Sort();
		}

		public void AddAll(List<Note> notes)
		{
			lock (_notes)
			{
				_notes.AddRange(notes);
			}

			Sort();
		}

		public Note this[int index]
		{
			get
			{
				lock (_notes)
				{
					return _notes[index];
				}
			}
			set
			{
				lock (_notes)
				{
					_notes[index] = value;
				}
			}
		}

		public List<Note> ToList()
		{
			lock (_notes)
			{
				return new List<Note>(_notes);
			}
		}
	}
}