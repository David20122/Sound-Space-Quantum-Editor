using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Drawing;

namespace New_SSQE.Preferences
{
    internal partial class Settings
    {
        public static readonly Setting<bool> waveform = true;
        public static readonly Setting<bool> classicWaveform = false;
        public static readonly Setting<bool> enableAutosave = true;
        public static readonly Setting<bool> correctOnCopy = true;
        public static readonly Setting<bool> gridNumbers = false;
        public static readonly Setting<bool> approachSquares = false;
        public static readonly Setting<bool> autoplay = true;
        public static readonly Setting<bool> numpad = false;
        public static readonly Setting<bool> quantumGridLines = true;
        public static readonly Setting<bool> quantumGridSnap = true;
        public static readonly Setting<bool> metronome = false;
        public static readonly Setting<bool> separateClickTools = false;
        public static readonly Setting<bool> enableQuantum = false;
        public static readonly Setting<bool> autoAdvance = false;
        public static readonly Setting<bool> selectTool = false;
        public static readonly Setting<bool> curveBezier = true;
        public static readonly Setting<bool> gridLetters = true;
        public static readonly Setting<bool> skipDownload = true;
        public static readonly Setting<bool> lockCursor = true;
        public static readonly Setting<bool> reverseScroll = false;
        public static readonly Setting<bool> useVSync = false;
        public static readonly Setting<bool> checkUpdates = true;
        public static readonly Setting<bool> fullscreenPlayer = true;
        public static readonly Setting<bool> approachFade = true;
        public static readonly Setting<bool> gridGuides = false;
        public static readonly Setting<bool> applyOnPaste = false;
        public static readonly Setting<bool> jumpPaste = false;
        public static readonly Setting<bool> limitPlayerFPS = false;
        public static readonly Setting<bool> useRhythia = true;
        public static readonly Setting<bool> pauseScroll = true;
        public static readonly Setting<bool> clampSR = true;
        public static readonly Setting<bool> pasteReversed = false;
        public static readonly Setting<bool> adjustNotes = false;
        public static readonly Setting<bool> notePushback = false;

        public static readonly Setting<float> editorBGOpacity = 255;
        public static readonly Setting<float> gridOpacity = 255;
        public static readonly Setting<float> trackOpacity = 255;
        public static readonly Setting<float> autosaveInterval = 5;
        public static readonly Setting<float> waveformDetail = 5;
        public static readonly Setting<float> sfxOffset = 0;
        public static readonly Setting<float> exportOffset = 0;
        public static readonly Setting<float> bezierDivisor = 4;
        public static readonly Setting<float> sensitivity = 1;
        public static readonly Setting<float> parallax = 1;
        public static readonly Setting<float> approachDistance = 1;
        public static readonly Setting<float> hitWindow = 55;
        public static readonly Setting<float> fov = 70;
        public static readonly Setting<float> noteScale = 1;
        public static readonly Setting<float> cursorScale = 1;

        public static readonly Setting<string> language = "english";
        public static readonly Setting<string> autosavedFile = "";
        public static readonly Setting<string> autosavedProperties = "";
        public static readonly Setting<string> lastFile = "";
        public static readonly Setting<string> clickSound = "click";
        public static readonly Setting<string> hitSound = "hit";
        public static readonly Setting<string> defaultPath = "";
        public static readonly Setting<string> audioPath = "";
        public static readonly Setting<string> exportPath = "";
        public static readonly Setting<string> coverPath = "";
        public static readonly Setting<string> importPath = "";
        public static readonly Setting<string> rhythiaPath = "";
        public static readonly Setting<string> rhythiaFolderPath = "";
        public static readonly Setting<string> replayPath = "";
        public static readonly Setting<ListSetting> cameraMode = new ListSetting(0, "half lock", "full lock", "spin");
        public static readonly Setting<ListSetting> modchartGame = new ListSetting(1, "Rhythia", "Nova");
        public static readonly Setting<ListSetting> exportType = new ListSetting(0, "Rhythia (SSPM)", "Nova (NPK)");

        public static readonly Setting<float> vfxDuration = 0;
        public static readonly Setting<ListSetting> vfxStyle = new ListSetting(0, "Linear", "Sine", "Back", "Quad", "Quart", "Quint", "Bounce", "Elastic", "Exponential", "Circular", "Cubic");
        public static readonly Setting<ListSetting> vfxDirection = new ListSetting(2, "In", "Out", "InOut");
        public static readonly Setting<SliderSetting> vfxIntensity = new SliderSetting(0.5f, 1, 0.01f);
        public static readonly Setting<float> vfxFOV = 1;
        public static readonly Setting<Color> vfxColor = Color.White;
        public static readonly Setting<float> vfxVectorX = 0;
        public static readonly Setting<float> vfxVectorY = 0;
        public static readonly Setting<float> vfxVectorZ = 0;
        public static readonly Setting<float> vfxDegrees = 0;
        public static readonly Setting<float> vfxFactor = 0;
        public static readonly Setting<string> vfxString = "";
        public static readonly Setting<ListSetting> vfxStrength = new ListSetting(0, "Low", "Medium", "High", "Extreme");

        public static readonly Setting<Keybind> extrasBeat = new Keybind(Keys.S, false, false, false);

        public static readonly Setting<Color> color1 = Color.FromArgb(0, 255, 200);
        public static readonly Setting<Color> color2 = Color.FromArgb(255, 0, 255);
        public static readonly Setting<Color> color3 = Color.FromArgb(255, 0, 100);
        public static readonly Setting<Color> color4 = Color.FromArgb(90, 90, 90);
        public static readonly Setting<Color> color5 = Color.FromArgb(255, 204, 0);
        public static readonly Setting<List<Color>> noteColors = new List<Color>() { Color.FromArgb(255, 0, 255), Color.FromArgb(0, 255, 200) };

        public static readonly Setting<SliderSetting> trackHeight = new SliderSetting(16, 32, 1);
        public static readonly Setting<SliderSetting> cursorPos = new SliderSetting(40, 100, 1);
        public static readonly Setting<SliderSetting> approachRate = new SliderSetting(9, 29, 1);
        public static readonly Setting<SliderSetting> playerApproachRate = new SliderSetting(9, 29, 1);
        public static readonly Setting<SliderSetting> masterVolume = new SliderSetting(0.05f, 1, 0.01f);
        public static readonly Setting<SliderSetting> sfxVolume = new SliderSetting(0.1f, 1, 0.01f);
        public static readonly Setting<SliderSetting> fpsLimit = new SliderSetting(60, 305, 5);

        public static readonly Setting<SliderSetting> currentTime = new SliderSetting(0, 0, 0);
        public static readonly Setting<SliderSetting> beatDivisor = new SliderSetting(3, 31, 0.5f);
        public static readonly Setting<SliderSetting> tempo = new SliderSetting(0.9f, 1.4f, 0.05f);
        public static readonly Setting<SliderSetting> quantumSnapping = new SliderSetting(0, 57, 1);
        public static readonly Setting<SliderSetting> changelogPosition = new SliderSetting(0, 0, 1);

        public static readonly Setting<Keybind> selectAll = new Keybind(Keys.A, true, false, false);
        public static readonly Setting<Keybind> save = new Keybind(Keys.S, true, false, false);
        public static readonly Setting<Keybind> saveAs = new Keybind(Keys.S, true, false, true);
        public static readonly Setting<Keybind> undo = new Keybind(Keys.Z, true, false, false);
        public static readonly Setting<Keybind> redo = new Keybind(Keys.Y, true, false, false);
        public static readonly Setting<Keybind> copy = new Keybind(Keys.C, true, false, false);
        public static readonly Setting<Keybind> paste = new Keybind(Keys.V, true, false, false);
        public static readonly Setting<Keybind> cut = new Keybind(Keys.X, true, false, false);
        public static readonly Setting<Keybind> delete = new Keybind(Keys.Delete, false, false, false);
        public static readonly Setting<Keybind> hFlip = new Keybind(Keys.H, false, false, true);
        public static readonly Setting<Keybind> vFlip = new Keybind(Keys.V, false, false, true);
        public static readonly Setting<Keybind> switchClickTool = new Keybind(Keys.Tab, false, false, false);
        public static readonly Setting<Keybind> quantum = new Keybind(Keys.Q, true, false, false);
        public static readonly Setting<Keybind> openTimings = new Keybind(Keys.T, true, false, false);
        public static readonly Setting<Keybind> openBookmarks = new Keybind(Keys.B, true, false, false);
        public static readonly Setting<Keybind> storeNodes = new Keybind(Keys.S, false, false, true);
        public static readonly Setting<Keybind> drawBezier = new Keybind(Keys.D, false, false, true);
        public static readonly Setting<Keybind> anchorNode = new Keybind(Keys.A, false, false, true);
        public static readonly Setting<Keybind> openDirectory = new Keybind(Keys.D, true, false, true);
        public static readonly Setting<Keybind> exportSSPM = new Keybind(Keys.E, true, true, false);
        public static readonly Setting<Keybind> createBPM = new Keybind(Keys.B, true, false, true);

        public static readonly Setting<List<Keys>> gridKeys = new List<Keys> { Keys.Q, Keys.W, Keys.E, Keys.A, Keys.S, Keys.D, Keys.Z, Keys.X, Keys.C };
        public static readonly Setting<List<string>> patterns = new List<string> { "", "", "", "", "", "", "", "", "", "" };

        public static readonly Setting<string> mappers = "";
        public static readonly Setting<string> songName = "";
        public static readonly Setting<string> difficulty = "N/A";
        public static readonly Setting<bool> useCover = false;
        public static readonly Setting<string> cover = "Default";
        public static readonly Setting<string> customDifficulty = "";
        public static readonly Setting<float> rating = 0;
        public static readonly Setting<bool> useVideo = false;
        public static readonly Setting<string> video = "";

        public static readonly Setting<string> songOffset = "";
        public static readonly Setting<string> songTitle = "";
        public static readonly Setting<string> songArtist = "";
        public static readonly Setting<string> mapCreator = "";
        public static readonly Setting<string> mapCreatorPersonalLink = "";
        public static readonly Setting<string> previewStartTime = "";
        public static readonly Setting<string> previewDuration = "";
        public static readonly Setting<string> novaCover = "";
        public static readonly Setting<string> novaIcon = "";

        public static readonly Setting<bool> debugMode = false;
        public static readonly Setting<bool> msaa = true;

        public static readonly Setting<string> SSQE_Player_Version = "";
        public static readonly Setting<string> SSQE_Updater_Version = "";

        public static readonly Setting<bool> detectWarningShown = false;
    }
}
