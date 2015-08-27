using Microsoft.WindowsAPICodePack.Shell.PropertySystem;
using MS.WindowsAPICodePack.Internal;
using NagMePlenty.ShellHelpers;
using System;
using System.Diagnostics;
using System.IO;

namespace NagMePlenty
{
    class ShortcutHelper
    {
        private String APP_ID;

        public ShortcutHelper(String APP_ID)
        {
            this.APP_ID = APP_ID;
        }

        // In order to display toasts, a desktop application must have a shortcut on the Start menu.
        // Also, an AppUserModelID must be set on that shortcut.
        // The shortcut should be created as part of the installer. The following code shows how to create
        // a shortcut and assign an AppUserModelID using Windows APIs. 
        //
        // Included in this project is a wxs file that be used with the WiX toolkit
        // to make an installer that creates the necessary shortcut. One or the other should be used.
        public bool InitShortcut()
        {
            String shortcutPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + 
                "\\Microsoft\\Windows\\Start Menu\\Programs\\NagMePlenty.lnk";
            if (!File.Exists(shortcutPath))
            {
                InstallShortcut(shortcutPath);
                return true;
            }
            return false;
        }

        private void InstallShortcut(String shortcutPath)
        {
            // Find the path to the current executable
            String exePath = Process.GetCurrentProcess().MainModule.FileName;
            IShellLinkW newShortcut = (IShellLinkW)new CShellLink();

            // Create a shortcut to the exe
            ShellHelpers.ErrorHelper.VerifySucceeded(newShortcut.SetPath(exePath));
            ShellHelpers.ErrorHelper.VerifySucceeded(newShortcut.SetArguments(""));

            // Open the shortcut property store, set the AppUserModelId property
            IPropertyStore newShortcutProperties = (IPropertyStore)newShortcut;

            using (PropVariant appId = new PropVariant(APP_ID))
            {
                ShellHelpers.ErrorHelper.VerifySucceeded(
                    newShortcutProperties.SetValue(SystemProperties.System.AppUserModel.ID, appId));
                ShellHelpers.ErrorHelper.VerifySucceeded(newShortcutProperties.Commit());
            }

            // Commit the shortcut to disk
            IPersistFile newShortcutSave = (IPersistFile)newShortcut;

            ShellHelpers.ErrorHelper.VerifySucceeded(newShortcutSave.Save(shortcutPath, true));
        }
    }
}
