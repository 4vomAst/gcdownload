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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace GcDownload
{
	public class CSettings
	{
        private string garminRootDir = "";
        private string sdCardRootDir = "";
        private string fieldlogFileName = "geocache_visits.txt";
        private string archivePath = "";
        private bool garminFound = false;
        private bool sdCardFound = false;
        private bool storeCachesOnSdCard = false;

        public string GarminRootDir
        {
            get { return garminRootDir; }
            set { garminRootDir = value; }
        }

        public string SdCardRootDir
        {
            get { return sdCardRootDir; }
            set { sdCardRootDir = value; }
        }

        public bool StoreCachesOnSdCard
        {
            get { return storeCachesOnSdCard; }
            set { storeCachesOnSdCard = value; }
        }

        public string GpxPath
        {
            get
            {
                if (storeCachesOnSdCard && isSdCardConnected())
                {
                    return sdCardRootDir + "garmin\\gpx";
                }
                else
                {
                    return garminRootDir + "garmin\\gpx";
                }
            }
        }

        public string FieldLogPath
        {
            get
            {
                return garminRootDir + "garmin\\" + fieldlogFileName;
            }
        }

        public string ArchivePath
        {
            get { return archivePath; }
            set { archivePath = value; }
        }

        public bool isArchivePathValid()
        {
            if (string.IsNullOrEmpty(archivePath)) return false;
            return Directory.Exists(archivePath);
        }

        public bool isGarminConnected()
        {
            garminFound = false;

            if ((File.Exists(garminRootDir + "garmin\\GarminDevice.xml"))
                && (File.Exists(garminRootDir + "garmin\\system.xml"))
                && (Directory.Exists(garminRootDir + "garmin\\gpx"))
                )
            {
                garminFound = true;
            };

            return garminFound;
        }

        public bool isSdCardConnected()
        {
            sdCardFound = false;

            if (Directory.Exists(sdCardRootDir + "garmin\\gpx"))
            {
                sdCardFound = true;
            };

            return sdCardFound;
        }

        public string[] getDriveNames()
        {
            DriveInfo[] drives = DriveInfo.GetDrives();
            string[] retVal = new string[drives.Length];
            int i = 0;

            foreach (DriveInfo drive in drives)
            {
                string name = drive.Name;
                if (drive.IsReady)
                {
                    name += " " + drive.VolumeLabel;
                }
                name += " (" + drive.DriveType.ToString() + ")";

                retVal[i++] = name;
            }

            return retVal;
        }

        public bool autoDetectGarmin()
        {
            garminFound = false;
            sdCardFound = false;
            DriveInfo[] drives = DriveInfo.GetDrives();

            foreach (DriveInfo drive in drives)
            {
                string rootPath = drive.Name;
                if (drive.IsReady && (drive.DriveType == DriveType.Removable))
                {
                    if (    (File.Exists(rootPath + "garmin\\GarminDevice.xml"))
                        &&  (File.Exists(rootPath + "garmin\\system.xml"))
                        &&  (Directory.Exists(rootPath + "garmin\\gpx"))
                        )
                    {
                        garminFound = true;
                        garminRootDir = rootPath;
                        break;
                    }
                }
            }

            if (garminFound)
            {
                foreach (DriveInfo drive in drives)
                {
                    if (drive.Name == garminRootDir) continue;

                    string rootPath = drive.Name;
                    if (drive.IsReady && (drive.DriveType == DriveType.Removable))
                    {
                        if (Directory.Exists(rootPath + "garmin\\gpx"))
                        {
                            sdCardFound = true;
                            sdCardRootDir = rootPath;
                            break;
                        }
                    }
                }
            }

            return garminFound;
        }

        public void readSettings()
        {
            Microsoft.Win32.RegistryKey rKeyRoot = Microsoft.Win32.Registry.CurrentUser;
            Microsoft.Win32.RegistryKey rKeySoftware = rKeyRoot.CreateSubKey("SOFTWARE");
            Microsoft.Win32.RegistryKey rKeyWb = rKeySoftware.CreateSubKey("wb");
            Microsoft.Win32.RegistryKey rKeyGpxDownload = rKeyWb.CreateSubKey("GpxDownload");


            garminRootDir = (string)rKeyGpxDownload.GetValue("garminRootDir", "");
            sdCardRootDir = (string)rKeyGpxDownload.GetValue("sdCardRootDir", "");
            archivePath = (string)rKeyGpxDownload.GetValue("archivepath", "");

            rKeySoftware.Close();
            rKeyWb.Close();
            rKeyGpxDownload.Close();
        }

        public void saveSettings()
        {
            Microsoft.Win32.RegistryKey rKeyRoot = Microsoft.Win32.Registry.CurrentUser;
            Microsoft.Win32.RegistryKey rKeySoftware = rKeyRoot.CreateSubKey("SOFTWARE");
            Microsoft.Win32.RegistryKey rKeyWb = rKeySoftware.CreateSubKey("wb");
            Microsoft.Win32.RegistryKey rKeyGpxDownload = rKeyWb.CreateSubKey("GpxDownload");

            rKeyGpxDownload.SetValue("garminRootDir", garminRootDir);
            rKeyGpxDownload.SetValue("sdCardRootDir", sdCardRootDir);
            rKeyGpxDownload.SetValue("archivepath", archivePath);

            rKeySoftware.Close();
            rKeyWb.Close();
            rKeyGpxDownload.Close();
        }

    }
}
