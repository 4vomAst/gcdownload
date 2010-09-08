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
        private string fieldlogFileName = "geocache_visits.txt";
        private string archivePath = "";
        private bool garminFound = false;

        public string GarminRootDir
        {
            get { return garminRootDir; }
            set { garminRootDir = value; }
        }

        public string GpxPath
        {
            get
            {
                return garminRootDir + "garmin\\gpx";
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

        public bool isGarminDirectoryOk()
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
            DriveInfo[] drives = DriveInfo.GetDrives();

            foreach (DriveInfo drive in drives)
            {
                string rootPath = drive.Name;
                if (drive.IsReady)
                {
                    if (!garminFound)
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
            rKeyGpxDownload.SetValue("archivepath", archivePath);

            rKeySoftware.Close();
            rKeyWb.Close();
            rKeyGpxDownload.Close();
        }

    }
}
