﻿using ApplicationJampay.CardReaderAPI.Interop.Unix;
using System;

namespace ApplicationJampay.CardReaderAPI
{
    public static class Platform
    {
        /// <summary>
        /// Returns <c>true</c> if the operation system runs on Windows. <c>false</c> otherwise.
        /// </summary>
        public static bool IsWindows { get; }

        /// <summary>
        /// Platform smart card library.
        /// </summary>
        internal static ISCardApi Lib { get; }

        static Platform()
        {
            var platform = Environment.OSVersion.Platform;

            if (
                platform == PlatformID.Win32S ||
                platform == PlatformID.Win32Windows ||
                platform == PlatformID.Win32NT ||
                platform == PlatformID.WinCE
            )
            {
                IsWindows = true;
                Lib = new WinSCardAPI();
                return;
            }

            IsWindows = false;
            Lib = new PCSCliteAPI();
        }
    }
}