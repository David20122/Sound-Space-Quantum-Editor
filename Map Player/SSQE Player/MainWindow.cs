﻿using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.Common;
using SSQE_Player.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.ComponentModel;
using OpenTK.Windowing.Common.Input;
using SkiaSharp;
using System.Resources;
using SSQE_Player.Models;

namespace SSQE_Player
{
    internal class MainWindow : GameWindow
    {
        public static MainWindow Instance;
        public MusicPlayer MusicPlayer = new();
        public SoundPlayer SoundPlayer = new();
        public ModelManager ModelManager = new();

        public List<Note> Notes = new();

        public static readonly Vector3 NoteSize = Vector3.One * 0.875f;
        public static readonly Vector3 CursorSize = new(0.275f, 0.275f, 0f);
        public Vector3 CursorPos = new();

        public GuiWindowMain CurrentWindow;
        public Camera Camera = new();

        public float StartTime;
        public float Tempo;

        public MainWindow(bool fromStart) : base(GameWindowSettings.Default, new NativeWindowSettings()
        {
            Size = (1280, 720),
            Title = $"Sound Space Map Player {Assembly.GetExecutingAssembly().GetName().Version}",
            NumberOfSamples = 32,
            WindowState = WindowState.Fullscreen,
            Vsync = VSyncMode.Off
        })
        {
            Shader.Init();
            FontRenderer.Init();

            // icon broken idk

            Instance = this;

            CursorState = CursorState.Grabbed;

            Settings.Load();

            if (fromStart)
                Settings.settings["currentTime"].Value = 0f;
            StartTime = Settings.settings["currentTime"].Value;

            var startIndex = LoadMap(File.ReadAllText("assets/temp/tempmap.txt"));
            RegisterModels();

            Camera = new();
            CurrentWindow = new(startIndex);

            MusicPlayer.Volume = Settings.settings["masterVolume"].Value;
            SoundPlayer.Volume = Settings.settings["sfxVolume"].Value;

            SetTempo(Settings.settings["tempo"].Value);
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);

            var mouse = MouseState;

            if (MusicPlayer.IsPlaying)
                Settings.settings["currentTime"].Value = (float)MusicPlayer.CurrentTime.TotalMilliseconds;

            Camera.Update(mouse.X, mouse.Y);
            CurrentWindow?.Render(mouse.X, mouse.Y, (float)args.Time);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
            
            SwapBuffers();
        }

        protected override void OnLoad()
        {
            GL.ClearColor(0f, 0f, 0f, 1f);

            GL.Enable(EnableCap.Multisample);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            var w = Math.Max(e.Width, 1280);
            var h = Math.Max(e.Height, 720);
            Size = new Vector2i(w, h);

            base.OnResize(new ResizeEventArgs(w, h));
            GL.Viewport(0, 0, w, h);

            Shader.SetViewport(Shader.FontTexProgram, w, h);

            Camera.CalculateProjection();
            CurrentWindow?.OnResize(Size);

            OnRenderFrame(new FrameEventArgs());
        }

        protected override void OnKeyDown(KeyboardKeyEventArgs e)
        {
            switch (e.Key)
            {
                case Keys.Escape:
                case Keys.F4 when e.Alt:
                    Close();
                    break;

                case Keys.F11:
                    WindowState = WindowState == WindowState.Fullscreen ? WindowState.Normal : WindowState.Fullscreen;
                    break;

                case Keys.R:
                    CurrentWindow.Resetting = true;
                    break;

                case Keys.Tab:
                    CurrentWindow.Reset();
                    break;

                case Keys.Space:
                    if (CurrentWindow.Paused && !CurrentWindow.Unpausing)
                    {
                        Settings.settings["currentTime"].Value = CurrentWindow.PauseTime - 750f;
                        MusicPlayer.Play();
                        CurrentWindow.Unpausing = true;
                    }
                    else if (!CurrentWindow.Unpausing && MusicPlayer.IsPlaying && CurrentWindow.PauseTime + 1500f <= Settings.settings["currentTime"].Value)
                    {
                        MusicPlayer.Pause();

                        CurrentWindow.PauseTime = Settings.settings["currentTime"].Value;
                        CurrentWindow.Pauses++;
                        CurrentWindow.Paused = true;
                    }
                    break;
            }
        }

        protected override void OnKeyUp(KeyboardKeyEventArgs e)
        {
            switch (e.Key)
            {
                case Keys.R:
                    CurrentWindow.Resetting = false;
                    break;

                case Keys.Space:
                    if (CurrentWindow.Paused)
                    {
                        MusicPlayer.Pause();
                        Settings.settings["currentTime"].Value = CurrentWindow.PauseTime;
                    }

                    CurrentWindow.Unpausing = false;
                    break;
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            MusicPlayer.Dispose();
        }

        protected override void OnFocusedChanged(FocusedChangedEventArgs e)
        {
            CursorState = IsFocused ? CursorState.Grabbed : CursorState.Normal;
        }




        private int LoadMap(string data)
        {
            var currentTime = Settings.settings["currentTime"].Value;
            var noteColors = Settings.settings["noteColors"];
            var colorCount = noteColors.Count;

            var startIndex = 0;

            var split = data.Split(',');

            try
            {
                var id = split[0];

                for (int i = 1; i < split.Length; i++)
                {
                    var subsplit = split[i].Split('|');

                    var x = float.Parse(subsplit[0]);
                    var y = float.Parse(subsplit[1]);
                    var ms = long.Parse(subsplit[2]);

                    if (ms < currentTime)
                        startIndex++;

                    Notes.Add(new Note(x, y, ms, noteColors[i % colorCount]));
                }

                MusicPlayer.Load($"cached/{id}.asset");
            }
            catch
            {
                CursorState = CursorState.Normal;
                Close();
            }

            return startIndex;
        }

        private void RegisterModels()
        {
            var noteModel = ObjModel.FromFile("assets/models/note.obj");
            var cursorModel = ObjModel.FromFile("assets/models/cursor.obj");

            ModelManager.RegisterModel("note", noteModel.GetVertices());
            ModelManager.RegisterModel("cursor", cursorModel.GetVertices());
        }

        private void SetTempo(float newTempo)
        {
            var tempoA = Math.Min(newTempo, 0.9f);
            var tempoB = (newTempo - tempoA) * 2f;

            Tempo = tempoA + tempoB + 0.1f;
            MusicPlayer.Tempo = Tempo;
        }
    }
}
