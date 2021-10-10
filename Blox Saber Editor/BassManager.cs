using System;
using System.Collections.Generic;
using Un4seen.Bass;
using Un4seen.Bass.AddOn.Fx;

namespace Sound_Space_Editor
{
	static class BassManager
	{
		static BassManager()
		{
			Init();
		}

		private static void Init()
		{
			Bass.BASS_Init(-1, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero);

			Bass.BASS_SetConfig(BASSConfig.BASS_CONFIG_BUFFER, 250);
			Bass.BASS_SetConfig(BASSConfig.BASS_CONFIG_UPDATEPERIOD, 5);
		}

		public static bool CheckDevice(int streamID)
		{
			var device = Bass.BASS_ChannelGetDevice(streamID);
			var info = Bass.BASS_GetDeviceInfo(device);

			if (info != null && (!info.IsDefault || !info.IsEnabled))
			{
				return false;
			}

			return true;
		}

		public static void Reload()
		{
			Dispose();
			Init();
		}

		public static void Dispose()
		{
			Bass.BASS_Free();
		}
	}
}