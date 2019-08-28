/*
Copyright (c) 2010-2019 Wolfgang Wallhaeuser

https://github.com/4vomast/gcdownload

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/

using System;
using System.IO;

namespace GcDownload
{
	public class CSettings
	{
        public string GarminRootDir { get; set; } = string.Empty;
        public string SdCardRootDir { get; set; } = string.Empty;
        public bool StoreCachesOnSdCard { get; set; } = false;

        public string GpxPath
        {
            get
            {
                if (StoreCachesOnSdCard && IsSdCardConnected)
                {
                    return SdCardRootDir + "garmin\\gpx";
                }
                else
                {
                    return GarminRootDir + "garmin\\gpx";
                }
            }
        }

        public string FieldLogPath
        {
            get
            {
                return GarminRootDir + "garmin\\geocache_visits.txt";
            }
        }

        public string ArchivePath { get; set; } = string.Empty;

        public bool IsArchivePathValid => Directory.Exists(ArchivePath);

        public bool IsGarminConnected => File.Exists(GarminRootDir + "garmin\\GarminDevice.xml")
                    && File.Exists(GarminRootDir + "garmin\\system.xml")
                    && Directory.Exists(GarminRootDir + "garmin\\gpx");

        public bool IsSdCardConnected => Directory.Exists(SdCardRootDir + "garmin\\gpx");

        public string[] GetDriveNames()
        {
            var drives = DriveInfo.GetDrives();
            var retVal = new string[drives.Length];
            int idx = 0;

            foreach (var drive in drives)
            {
                var name = drive.Name;
                if (drive.IsReady)
                {
                    name += " " + drive.VolumeLabel;
                }
                name += " (" + drive.DriveType.ToString() + ")";

                retVal[idx++] = name;
            }

            return retVal;
        }

        public bool AutoDetectGarmin()
        {
            var garminFound = false;
            var drives = DriveInfo.GetDrives();

            foreach (var drive in drives)
            {
                var rootPath = drive.Name;
                if (drive.IsReady && (drive.DriveType == DriveType.Removable))
                {
                    if (    (File.Exists(rootPath + "garmin\\GarminDevice.xml"))
                        &&  (File.Exists(rootPath + "garmin\\system.xml"))
                        &&  (Directory.Exists(rootPath + "garmin\\gpx"))
                        )
                    {
                        garminFound = true;
                        GarminRootDir = rootPath;
                        break;
                    }
                }
            }

            if (!garminFound) return false;

            foreach (var drive in drives)
            {
                if (drive.Name == GarminRootDir) continue;

                var rootPath = drive.Name;
                if (drive.IsReady && (drive.DriveType == DriveType.Removable))
                {
                    if (!Directory.Exists(rootPath + "garmin\\gpx"))
                    {
                        continue;
                    }

                    SdCardRootDir = rootPath;
                    break;
                }
            }

            return true;
        }

        public void ReadSettings()
        {
            Microsoft.Win32.RegistryKey rKeyRoot = Microsoft.Win32.Registry.CurrentUser;
            Microsoft.Win32.RegistryKey rKeySoftware = rKeyRoot.CreateSubKey("SOFTWARE");
            Microsoft.Win32.RegistryKey rKeyWb = rKeySoftware.CreateSubKey("wb");
            Microsoft.Win32.RegistryKey rKeyGpxDownload = rKeyWb.CreateSubKey("GpxDownload");

            GarminRootDir = (string)rKeyGpxDownload.GetValue("garminRootDir", "");
            SdCardRootDir = (string)rKeyGpxDownload.GetValue("sdCardRootDir", "");
            ArchivePath = (string)rKeyGpxDownload.GetValue("archivepath", "");

            rKeySoftware.Close();
            rKeyWb.Close();
            rKeyGpxDownload.Close();
        }

        public void SaveSettings()
        {
            Microsoft.Win32.RegistryKey rKeyRoot = Microsoft.Win32.Registry.CurrentUser;
            Microsoft.Win32.RegistryKey rKeySoftware = rKeyRoot.CreateSubKey("SOFTWARE");
            Microsoft.Win32.RegistryKey rKeyWb = rKeySoftware.CreateSubKey("wb");
            Microsoft.Win32.RegistryKey rKeyGpxDownload = rKeyWb.CreateSubKey("GpxDownload");

            rKeyGpxDownload.SetValue("garminRootDir", GarminRootDir);
            rKeyGpxDownload.SetValue("sdCardRootDir", SdCardRootDir);
            rKeyGpxDownload.SetValue("archivepath", ArchivePath);

            rKeySoftware.Close();
            rKeyWb.Close();
            rKeyGpxDownload.Close();
        }

    }
}
