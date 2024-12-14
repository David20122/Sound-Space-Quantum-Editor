using New_SSQE.FileParsing.Formats;
using New_SSQE.Objects.Other;

namespace New_SSQE.FileParsing
{
    internal class Parser
    {
        // true: vfx, false: special
        public static readonly Dictionary<int, bool> VfxLookup = new()
        {
            {2, true },
            {3, true },
            {4, true },
            {5, true },
            {6, true },
            {7, true },
            {8, true },
            {9, true },
            {10, true },
            {11, true },
            {12, false },
        };

        private static long encodeDiff = 0;
        private static long encodePrev = 0;

        public static long EncodeTimestamp(long ms)
        {
            long diff = ms - encodePrev;
            long offset = diff - encodeDiff;

            encodeDiff = diff;
            encodePrev = ms;

            return offset;
        }

        public static void ResetEncode()
        {
            encodeDiff = 0;
            encodePrev = 0;
        }

        private static long decodeTotal = 0;
        private static long decodePrev = 0;

        public static long DecodeTimestamp(long ms)
        {
            decodePrev += ms;
            decodeTotal += decodePrev;

            return decodeTotal;
        }

        public static void ResetDecode()
        {
            decodeTotal = 0;
            decodePrev = 0;
        }

        // MAPS

        // TXT
        public static string Parse(string data) => TXT.Parse(data);
        public static string SaveLegacy(string id, bool copy = false, bool applyOffset = true) => TXT.SaveLegacy(id, copy, applyOffset);
        public static string Save(string audioId, bool copy = false, bool applyOffset = true) => TXT.Save(audioId, copy, applyOffset);
        // SSPM
        public static string ParseSSPM(string path) => SSPM.Parse(path);
        public static void SaveSSPM(string path) => SSPM.Save(path);
        // OSU
        public static string ParseOSU(string path) => OSU.Parse(path);
        // NOVA
        public static string ParseNOVA(string path) => NPK.Parse(path);
        public static void SaveNOVA(string path) => NPK.Save(path);
        // PHXM
        public static string ParsePHXM(string path) => PHXM.Parse(path);
        // RHYM
        public static string ParseRHYM(string path) => RHYM.Parse(path);
        public static void SaveRHYM(string path) => RHYM.Save(path);
        // PHZ
        public static bool IsValidPHZ(string path) => PHZ.IsValid(path);
        public static string ParsePHZ(string path) => PHZ.Parse(path);
        public static void SavePHZ(string path) => PHZ.Save(path);

        // REPLAYS

        // QER
        public static List<ReplayNode> ParseQER(string path, out float tempo) => QER.Parse(path, out tempo);
        public static void SaveQER(string path, float tempo, List<ReplayNode> nodes) => QER.Save(path, tempo, nodes);
        // SSPRE
        public static List<ReplayNode> ParseSSPRE(string path, out float tempo) => SSPRE.Parse(path, out tempo);
        // PHXRE
        public static List<ReplayNode> ParsePHXR(string path, out float tempo) => PHXR.Parse(path, out tempo);

        // PROPERTIES

        // INI
        public static void ParseINI(string data) => INI.Parse(data);
        public static void ParseINILegacy(string data) => INI.ParseLegacy(data);
        public static string SaveINI() => INI.Save();
    }
}
