using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace RobloxPortable
{
    public partial class Form1 : Form
    {
        // Import the CreateSymbolicLink function from the Windows API
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern bool CreateSymbolicLink(string symlinkFilePath, string targetFilePath, SymbolicLinkType linkType);
        private enum SymbolicLinkType
        {
            File = 0,
            Directory = 1
        }
        private class ListBoxItem {
            public String Text;
            public Color BackColor = Color.White;
            public Color TextColor = Color.Black;
            public ListBoxItem(String text,Color backcolor,Color textxolor)
            {
                this.Text = text;
                this.BackColor = backcolor;
                this.TextColor = textxolor;
            }
        }
        static String username;
        public Form1()
        {
            InitializeComponent(); 
            RunRoblox();
        }

        public void RunRoblox()
        {
            username = Environment.UserName;

            string localAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            String apppath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            String robloxappdatapath = localAppDataPath + @"\Roblox";
            string programFilesX86Path = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
#if DEBUG
            apppath = @"E:\RobloxPortable";
#endif
            String robloxportablepath = apppath + @"\Roblox";
          

            if (Directory.Exists(robloxportablepath)==false)
            {
                AddError("Roblox Portable not found '" + robloxportablepath);
                return;
            }
            else
            {
                AddLog("Roblox portable location " + robloxportablepath);
            }
            String robloxportableversionpath = robloxportablepath + @"\Versions";
            String version = GetVersion(robloxportableversionpath);
            

           
           
            if (String.IsNullOrEmpty(version))
            {
                AddError("Roblox portable version not found in " + robloxportableversionpath);
                return;
            }
            else
            {
                AddLog("Roblox portable version based on Folder Creation Time " + version );
            }
            //ROBLOX NORMAL
            String robloxversion = robloxappdatapath + @"\Versions\" + version;

            
            if (DeleteFolder(robloxappdatapath) == false)
            {
                return;
            }
            makelink(robloxappdatapath, robloxportablepath);

            //CLEAR ROBLOX DI PROGRAM FILES X86 
            String robloxprogramfilespath = programFilesX86Path + @"\Roblox";
            if (DeleteFolder(robloxprogramfilespath) == false)
            {
                return;
            }

            try
            {
                AddLog("Create folder " + robloxprogramfilespath);
                Directory.CreateDirectory(robloxprogramfilespath);
            }catch (Exception ex)
            {
                AddError("Error create folder " + ex.Message);
            }
            
            makelink(robloxprogramfilespath+@"\Versions", robloxportablepath+@"\Versions");



            String RobloxStudioLauncher = robloxprogramfilespath + @"\Versions\RobloxStudioLauncherBeta.exe";
            String RobloxLauncher = robloxprogramfilespath + @"\Versions\" + version + @"\RobloxPlayerLauncher.exe";


            AddLog("Roblox launcher location: " + RobloxLauncher);

            AddLog("Modify Registry Roblox");
            try
            {
                Registry.SetValue(@"HKEY_CLASSES_ROOT\roblox", "", @"URL: Roblox Protocol");
                Registry.SetValue(@"HKEY_CLASSES_ROOT\roblox", "URL Protocol", "");
                Registry.SetValue(@"HKEY_CLASSES_ROOT\roblox\DefaultIcon", "", RobloxLauncher);
                Registry.SetValue(@"HKEY_CLASSES_ROOT\roblox\shell\open\command", "", @"""" + RobloxLauncher + @""" %1");
            }
            catch (Exception ex)
            {
                AddError(ex.Message);
                return;
            }

            AddLog("Modify Registry Roblox Studio");
            try
            {
                Registry.SetValue(@"HKEY_CLASSES_ROOT\roblox-studio", "", @"URL: Roblox Protocol");
                Registry.SetValue(@"HKEY_CLASSES_ROOT\roblox-studio", "URL Protocol", "");
                Registry.SetValue(@"HKEY_CLASSES_ROOT\roblox-studio\DefaultIcon", "", RobloxStudioLauncher);
                Registry.SetValue(@"HKEY_CLASSES_ROOT\roblox-studio\shell\open\command", "", @"""" + RobloxStudioLauncher + @""" %1");
            }catch (Exception ex)
            {
                AddError(ex.Message);
                return;
            }

            AddLog("Modify Registry Roblox Player");
            try
            {
                Registry.SetValue(@"HKEY_CLASSES_ROOT\roblox-player", "", @"URL: Roblox Protocol");
                Registry.SetValue(@"HKEY_CLASSES_ROOT\roblox-player", "URL Protocol", "");
                Registry.SetValue(@"HKEY_CLASSES_ROOT\roblox-player\DefaultIcon", "", RobloxLauncher);
                Registry.SetValue(@"HKEY_CLASSES_ROOT\roblox-player\shell\open\command", "", @"""" + RobloxLauncher + @""" %1");
            }catch (Exception ex)
            {
                AddError(ex.Message);
                return;
            }

            AddLog("Modify Registry Roblox Corporation");
            try
            {
                Registry.SetValue(@"HKEY_CURRENT_USER\SOFTWARE\ROBLOX Corporation\Environments", "roblox-player", "roblox-player");
                Registry.SetValue(@"HKEY_CURRENT_USER\SOFTWARE\ROBLOX Corporation\Environments\roblox-player", "curQTStudioVer", "");
                Registry.SetValue(@"HKEY_CURRENT_USER\SOFTWARE\ROBLOX Corporation\Environments\roblox-player", "curQTStudioUrl", "");
                Registry.SetValue(@"HKEY_CURRENT_USER\SOFTWARE\ROBLOX Corporation\Environments\roblox-player", "", RobloxLauncher);
                Registry.SetValue(@"HKEY_CURRENT_USER\SOFTWARE\ROBLOX Corporation\Environments\roblox-player", "version", version);
                Registry.SetValue(@"HKEY_CURRENT_USER\SOFTWARE\ROBLOX Corporation\Environments\roblox-player", "curPlayerVer", version);
                Registry.SetValue(@"HKEY_CURRENT_USER\SOFTWARE\ROBLOX Corporation\Environments\roblox-player", "curPlayerUrl", "www.roblox.com");
                Registry.SetValue(@"HKEY_CURRENT_USER\SOFTWARE\ROBLOX Corporation\Environments\roblox-player", "LaunchExp", "InApp");
                Registry.SetValue(@"HKEY_CURRENT_USER\SOFTWARE\ROBLOX Corporation\Environments\roblox-player\Capabilities", "ApplicationDescription", "Play Roblox!");
                                     
                Registry.SetValue(@"HKEY_CURRENT_USER\SOFTWARE\ROBLOX Corporation\Environments\roblox-player\Capabilities", "ApplicationIcon", @""""+RobloxLauncher +@""",0",RegistryValueKind.ExpandString);
                Registry.SetValue(@"HKEY_CURRENT_USER\SOFTWARE\ROBLOX Corporation\Environments\roblox-player\Capabilities", "ApplicationName", "Roblox Player");

                Registry.SetValue(@"HKEY_CURRENT_USER\SOFTWARE\ROBLOX Corporation\Environments\roblox-player\Capabilities\UrlAssociations", "roblox-player", "roblox-player");
                Registry.SetValue(@"HKEY_CURRENT_USER\SOFTWARE\ROBLOX Corporation\Environments\roblox-player\Capabilities\UrlAssociations", "roblox", "roblox");

                Registry.SetValue(@"HKEY_CURRENT_USER\SOFTWARE\ROBLOX Corporation\Environments\roblox-player\Channel", "", "");
                Registry.SetValue(@"HKEY_CURRENT_USER\SOFTWARE\ROBLOX Corporation\Environments\roblox-player\Versions", "version0", version); 

                Registry.SetValue(@"HKEY_CURRENT_USER\SOFTWARE\ROBLOX Corporation\Environments\RobloxPlayer", "", "");
                Registry.SetValue(@"HKEY_CURRENT_USER\SOFTWARE\ROBLOX Corporation\Environments\RobloxPlayer\Channel", "www.roblox.com", "zwinplayer64");

                Registry.SetValue(@"HKEY_CURRENT_USER\SOFTWARE\ROBLOX Corporation\Roblox", "CPath", localAppDataPath+ @"\rbxcsettings.rbx");
                Registry.SetValue(@"HKEY_CURRENT_USER\SOFTWARE\ROBLOX Corporation\Roblox", "InferredCrash", 0,RegistryValueKind.DWord);
                Registry.SetValue(@"HKEY_CURRENT_USER\SOFTWARE\ROBLOX Corporation\Roblox", "FilteredExceptionCrash", 0, RegistryValueKind.DWord);

            }
            catch (Exception ex)
            {
                AddError(ex.Message);
                return;
            }
            //run roblox
           
            String url = @"https://www.roblox.com";
            AddLog("Running Roblox website " + url);


            //Process.Start(RobloxLauncher);
            Process.Start(url);

        }
        private String GetVersion(String robloxpath)
        {
            String r = "";
            DirectoryInfo directory = new DirectoryInfo(robloxpath);
            // Get the subdirectories within the directory
            DirectoryInfo[] directories = directory.GetDirectories();

            // Sort the directories by DateCreated
            var sortedDirectories = directories.OrderByDescending(dir => dir.CreationTime);

            // Display the sorted directories
            foreach (DirectoryInfo dir in sortedDirectories)
            {
                Console.WriteLine($"{dir.Name} - {dir.CreationTime}");
               
                if (dir.Name.StartsWith("version-"))
                {
                    if (File.Exists(dir.FullName + @"\RobloxPlayerLauncher.exe"))
                    {
                        r = dir.Name;
                        break;
                    }
                }
                
            }
            return r;
        }
        Boolean DeleteFolder(String folderpath)
        {
            try
            {
                if (Directory.Exists(folderpath))
                {
                    Directory.Delete(folderpath, true);
                }                
                AddLog("Delete folder '" + folderpath);
                return true;
            }
            catch (Exception ex)
            {
                AddError("Cannot delete folder '" + folderpath + "' " + ex.Message);
                return false;
            }
        }
        Boolean makelink(String linkPath, string targetPath)
        {


            // Create the symbolic link using the Windows API
            bool success = CreateSymbolicLink(linkPath, targetPath, SymbolicLinkType.Directory);

            if (success)
            {
               AddLog("LINK "+linkPath+ "=>"+targetPath);
                return true;
            }
            else
            {
                int errorCode = Marshal.GetLastWin32Error();
                AddError("Failed to create symbolic link. Error code: " + errorCode);
                return false;
            }
        }
        private void AddLog(String text)
        {
            listBox1.Items.Add(new ListBoxItem(text, Color.White, Color.Black));
            if (listBox1.Items.Count > 100 )
            {
                listBox1.Items.RemoveAt(0);
                
            }
            listBox1.SelectedIndex = listBox1.Items.Count - 1;
        }
        private void AddError(String text)
        {
            listBox1.Items.Add(new ListBoxItem(text, Color.Red, Color.White));
            if (listBox1.Items.Count > 100)
            {
                listBox1.Items.RemoveAt(0);

            }
            listBox1.SelectedIndex = listBox1.Items.Count - 1;
            Debug.WriteLine(text);
        }
 

        private void listBox1_DrawItem(object sender, DrawItemEventArgs e)
        {
            ListBox listBox = (ListBox)sender;

            // Check if the item index is valid
            if (e.Index >= 0 && e.Index < listBox.Items.Count)
            {
                // Get the item text
                ListBoxItem item = (ListBoxItem)listBox.Items[e.Index];

                // Set the default foreground and background colors
                Color foreColor = item.TextColor;
                Color backColor = item.BackColor;


                // Set the custom colors for the item
                e.Graphics.FillRectangle(new SolidBrush(backColor), e.Bounds);
                e.Graphics.DrawString(item.Text, e.Font, new SolidBrush(foreColor), e.Bounds);

                // Draw the focus rectangle if the item is selected
                if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
                    e.DrawFocusRectangle();
            }
        }
    }
}
