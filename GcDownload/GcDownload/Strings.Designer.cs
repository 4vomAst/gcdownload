﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace GcDownload {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Strings {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Strings() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("GcDownload.Strings", typeof(Strings).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Archived. .
        /// </summary>
        internal static string ArchivedGeocacheDescriptionPrefix {
            get {
                return ResourceManager.GetString("ArchivedGeocacheDescriptionPrefix", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The chosen archive directory is not valid. Click on the browse button &apos;...&apos; for selecting a directory where Geocache files should be archived..
        /// </summary>
        internal static string ErrorArchiveDirectoryNotFound {
            get {
                return ResourceManager.GetString("ErrorArchiveDirectoryNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Enter a valid Geocaching.com or Opencaching.de Geocache ID. Examples: GC12345 or OC12345..
        /// </summary>
        internal static string ErrorEnterGeocacheId {
            get {
                return ResourceManager.GetString("ErrorEnterGeocacheId", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Connect your GPS device to the computer and select it in the drop down list above. If the device is not listed, click the &apos;Detect&apos; button..
        /// </summary>
        internal static string ErrorGarminNotFound {
            get {
                return ResourceManager.GetString("ErrorGarminNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Geocache file {0} is invalid or cannot be opened..
        /// </summary>
        internal static string ErrorInvalidGpxFile {
            get {
                return ResourceManager.GetString("ErrorInvalidGpxFile", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Could not download Geocache file. Browse to the details page of a geocache and then press the download button. If the problem persists, the provider might have changed the layout of the geocache pages so that the application is no longer compatible..
        /// </summary>
        internal static string ErrorNoGeocachePageSelected {
            get {
                return ResourceManager.GetString("ErrorNoGeocachePageSelected", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Select the drive that is assigned to the internal SD card of your GPS device. If the drive is not listed, click the &apos;Detect&apos; button. Ensure that the directory &apos;garmin\gpx&apos; does exist on the SD card..
        /// </summary>
        internal static string ErrorSdCardNotFound {
            get {
                return ResourceManager.GetString("ErrorSdCardNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Geocache files (*.gpx)|*.gpx|All files (*.*)|*.*.
        /// </summary>
        internal static string FilterGpxFiles {
            get {
                return ResourceManager.GetString("FilterGpxFiles", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Log files (*.txt)|*.txt|All files (*.*)|*.*.
        /// </summary>
        internal static string FilterLogFiles {
            get {
                return ResourceManager.GetString("FilterLogFiles", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Choose a folder on your computer for storing the Geocache files:.
        /// </summary>
        internal static string MessageArchiveBrowseFolder {
            get {
                return ResourceManager.GetString("MessageArchiveBrowseFolder", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Moved {0} gpx files to directory &apos;{1}&apos;..
        /// </summary>
        internal static string MessageArchivedToDirectory {
            get {
                return ResourceManager.GetString("MessageArchivedToDirectory", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Archive corresponding Geocache files of found Geocaches on your computer? This will remove the files from your device..
        /// </summary>
        internal static string PromptArchive {
            get {
                return ResourceManager.GetString("PromptArchive", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Delete the complete field log?.
        /// </summary>
        internal static string PromptDeleteFieldLog {
            get {
                return ResourceManager.GetString("PromptDeleteFieldLog", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Delete {0} field log entries?.
        /// </summary>
        internal static string PromptDeleteFieldLogEntries {
            get {
                return ResourceManager.GetString("PromptDeleteFieldLogEntries", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Permanently delete {0} Geocache files from your device?.
        /// </summary>
        internal static string PromptDeleteGeocacheFiles {
            get {
                return ResourceManager.GetString("PromptDeleteGeocacheFiles", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Archive Geocache Files.
        /// </summary>
        internal static string TitleArchive {
            get {
                return ResourceManager.GetString("TitleArchive", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Delete Field Log.
        /// </summary>
        internal static string TitleDeleteFieldLog {
            get {
                return ResourceManager.GetString("TitleDeleteFieldLog", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Delete Geocache Files.
        /// </summary>
        internal static string TitleDeleteGeocache {
            get {
                return ResourceManager.GetString("TitleDeleteGeocache", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Geocache File Download.
        /// </summary>
        internal static string TitleDownload {
            get {
                return ResourceManager.GetString("TitleDownload", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Paperless Geocaching.
        /// </summary>
        internal static string TitleGeneric {
            get {
                return ResourceManager.GetString("TitleGeneric", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Geocache Search.
        /// </summary>
        internal static string TitleSearch {
            get {
                return ResourceManager.GetString("TitleSearch", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Settings.
        /// </summary>
        internal static string TitleSettings {
            get {
                return ResourceManager.GetString("TitleSettings", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Currently unavailable. .
        /// </summary>
        internal static string UnAvailableGeocacheDescriptionPrefix {
            get {
                return ResourceManager.GetString("UnAvailableGeocacheDescriptionPrefix", resourceCulture);
            }
        }
    }
}
