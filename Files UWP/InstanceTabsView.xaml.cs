﻿using Files.Filesystem;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Storage;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Files
{
    /// <summary>
    /// The Instance Tabs Component for Project Mumbai
    /// </summary>
    public sealed partial class InstanceTabsView : Page
    {
        public static TabView tabView;
        public string navArgs;
        public InstanceTabsView()
        {
            this.InitializeComponent();
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.Auto;
            var CoreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            CoreTitleBar.ExtendViewIntoTitleBar = true;
            tabView = TabStrip;
            var titleBar = ApplicationView.GetForCurrentView().TitleBar;
            titleBar.ButtonInactiveBackgroundColor = Color.FromArgb(0, 255, 255, 255);
            titleBar.ButtonHoverBackgroundColor = Color.FromArgb(75, 10, 10, 10);
            if (App.Current.RequestedTheme == ApplicationTheme.Dark)
            {
                titleBar.ButtonBackgroundColor = Color.FromArgb(0, 0, 0, 0);
                titleBar.ButtonForegroundColor = Colors.White;
                titleBar.ButtonHoverBackgroundColor = Color.FromArgb(75, 240, 240, 240);
                //titleBar.BackgroundColor = Color.FromArgb(255, 25, 25, 25);
            }
            else if (App.Current.RequestedTheme == ApplicationTheme.Light)
            {
                titleBar.ButtonBackgroundColor = Color.FromArgb(0, 255, 255, 255);
                titleBar.ButtonForegroundColor = Colors.Black;
                titleBar.ButtonHoverBackgroundColor = Color.FromArgb(75, 155, 155, 155);
            }

            if (this.RequestedTheme == ElementTheme.Dark)
            {
                titleBar.ButtonForegroundColor = Colors.White;
                titleBar.ButtonHoverBackgroundColor = Color.FromArgb(75, 240, 240, 240);
                //titleBar.BackgroundColor = Color.FromArgb(255, 25, 25, 25);
            }
            else if (this.RequestedTheme == ElementTheme.Light)
            {
                titleBar.ButtonForegroundColor = Colors.Black;
                titleBar.ButtonHoverBackgroundColor = Color.FromArgb(75, 155, 155, 155);
                //titleBar.BackgroundColor = Colors.Transparent;
            }
            
        }
        
        protected override void OnNavigatedTo(NavigationEventArgs eventArgs)
        {
            navArgs = eventArgs.Parameter?.ToString();

            if (string.IsNullOrEmpty(navArgs))
            {
                AddNewTab(typeof(ProHome), "Start");
            }
            else
            {
                AddNewTab(typeof(ProHome), navArgs);
            }

            Microsoft.UI.Xaml.Controls.FontIconSource icon = new Microsoft.UI.Xaml.Controls.FontIconSource();
            icon.Glyph = "\xE713";
            if ((tabView.SelectedItem as TabViewItem).Header.ToString() != "Settings" && (tabView.SelectedItem as TabViewItem).IconSource != icon)
            {
                App.selectedTabInstance = ItemViewModel.GetCurrentSelectedTabInstance<ProHome>();
            }
        }

        public void AddNewTab(Type t, string path)
        {
            Frame frame = new Frame();
            //frame.Navigate(t, path);
            string tabLocationHeader = null;
            Microsoft.UI.Xaml.Controls.FontIconSource fontIconSource = new Microsoft.UI.Xaml.Controls.FontIconSource();
            Microsoft.UI.Xaml.Controls.IconSource tabIcon;
            if (path != null)
            {
                if (path == "Settings")
                {
                    tabLocationHeader = "Settings";
                    fontIconSource.Glyph = "\xE713";
                    foreach (TabViewItem item in tabView.TabItems)
                    {
                        if (item.Header.ToString() == "Settings")
                        {
                            tabView.SelectedItem = item;
                            return;
                        }
                    }
                }
                else if (path == ProHome.DesktopPath)
                {
                    tabLocationHeader = "Desktop";
                    fontIconSource.Glyph = "\xE8FC";
                }
                else if (path == ProHome.DownloadsPath)
                {
                    tabLocationHeader = "Downloads";
                    fontIconSource.Glyph = "\xE896";
                }
                else if (path == ProHome.DocumentsPath)
                {
                    tabLocationHeader = "Documents";
                    fontIconSource.Glyph = "\xE8A5";
                }
                else if (path == ProHome.PicturesPath)
                {
                    tabLocationHeader = "Pictures";
                    fontIconSource.Glyph = "\xEB9F";
                }
                else if (path == ProHome.MusicPath)
                {
                    tabLocationHeader = "Music";
                    fontIconSource.Glyph = "\xEC4F";
                }
                else if (path == ProHome.VideosPath)
                {
                    tabLocationHeader = "Videos";
                    fontIconSource.Glyph = "\xE8B2";
                }
                else if (path == ProHome.OneDrivePath)
                {
                    tabLocationHeader = "OneDrive";
                    fontIconSource.Glyph = "\xE753";
                }
                
                else if (path == "Start")
                {
                    tabLocationHeader = "Start";
                    fontIconSource.Glyph = "\xE737";
                }
                else if (path == "New tab")
                {
                    tabLocationHeader = "New tab";
                    fontIconSource.Glyph = "\xE737";
                }
                else
                {
                    tabLocationHeader = Path.GetDirectoryName(path);
                    fontIconSource.Glyph = "\xE8B7";
                }
            }    

            tabIcon = fontIconSource;
            Grid gr = new Grid();
            gr.Children.Add(frame);
            gr.HorizontalAlignment = HorizontalAlignment.Stretch;
            gr.VerticalAlignment = VerticalAlignment.Stretch;
            TabViewItem tvi = new TabViewItem()
            {
                Header = tabLocationHeader,
                Content = gr,
                Width = 200,
                IconSource = tabIcon
            };
            tabView.TabItems.Add(tvi);
            TabStrip.SelectedItem = TabStrip.TabItems[TabStrip.TabItems.Count - 1];
            if(tabView.SelectedItem == tvi)
            {
                (((tabView.SelectedItem as TabViewItem).Content as Grid).Children[0] as Frame).Navigate(t, path);
            }
        }

        public async void SetSelectedTabInfo(string text, string currentPathForTabIcon = null)
        {
            string tabLocationHeader;
            Microsoft.UI.Xaml.Controls.FontIconSource fontIconSource = new Microsoft.UI.Xaml.Controls.FontIconSource();
            Microsoft.UI.Xaml.Controls.IconSource tabIcon;

            if (currentPathForTabIcon == null && text == "Settings")
            {
                tabLocationHeader = "Settings";
                fontIconSource.Glyph = "\xE713";
            }
            else if (currentPathForTabIcon == null && text == "New tab")
            {
                tabLocationHeader = "New tab";
                fontIconSource.Glyph = "\xE737";
            }
            else if (currentPathForTabIcon == null && text == "Start")
            {
                tabLocationHeader = "Start";
                fontIconSource.Glyph = "\xE737";
            }
            else if (currentPathForTabIcon == ProHome.DesktopPath)
            {
                tabLocationHeader = "Desktop";
                fontIconSource.Glyph = "\xE8FC";
            }
            else if (currentPathForTabIcon == ProHome.DownloadsPath)
            {
                tabLocationHeader = "Downloads";
                fontIconSource.Glyph = "\xE896";
            }
            else if (currentPathForTabIcon == ProHome.DocumentsPath)
            {
                tabLocationHeader = "Documents";
                fontIconSource.Glyph = "\xE8A5";
            }
            else if (currentPathForTabIcon == ProHome.PicturesPath)
            {
                tabLocationHeader = "Pictures";
                fontIconSource.Glyph = "\xEB9F";
            }
            else if (currentPathForTabIcon == ProHome.MusicPath)
            {
                tabLocationHeader = "Music";
                fontIconSource.Glyph = "\xEC4F";
            }
            else if (currentPathForTabIcon == ProHome.VideosPath)
            {
                tabLocationHeader = "Videos";
                fontIconSource.Glyph = "\xE8B2";
            }
            else if (currentPathForTabIcon == ProHome.OneDrivePath)
            {
                tabLocationHeader = "OneDrive";
                fontIconSource.Glyph = "\xE753";
            }
            else
            {
                // If path is a drive's root
                if (NormalizePath(Path.GetPathRoot(currentPathForTabIcon)) == NormalizePath(currentPathForTabIcon))
                {
                    if (NormalizePath(currentPathForTabIcon) != NormalizePath("A:") && NormalizePath(currentPathForTabIcon) != NormalizePath("B:"))
                    {
                        var remDriveNames = (await KnownFolders.RemovableDevices.GetFoldersAsync()).Select(x => x.DisplayName);

                        if (!remDriveNames.Contains(NormalizePath(currentPathForTabIcon)))
                        {
                            fontIconSource.Glyph = "\xEDA2";
                            tabLocationHeader = "Local Disk (" + NormalizePath(currentPathForTabIcon) + ")";
                        }
                        else
                        {
                            fontIconSource.Glyph = "\xE88E";
                            tabLocationHeader = (await KnownFolders.RemovableDevices.GetFolderAsync(currentPathForTabIcon)).DisplayName;
                        }
                    }
                    else
                    {
                        fontIconSource.Glyph = "\xE74E";
                        tabLocationHeader = "Floppy Disk (" + NormalizePath(currentPathForTabIcon) + ")";
                    }
                }
                else
                {
                    fontIconSource.Glyph = "\xE8B7";
                    tabLocationHeader = currentPathForTabIcon.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar).Split('\\', StringSplitOptions.RemoveEmptyEntries).Last();
                }

            }
            tabIcon = fontIconSource;
            (tabView.SelectedItem as TabViewItem).Header = tabLocationHeader;
            (tabView.SelectedItem as TabViewItem).IconSource = tabIcon;
        }

        public static string NormalizePath(string path)
        {
            if (path.Contains('\\'))
            {
                return Path.GetFullPath(new Uri(path).LocalPath)
                .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                .ToUpperInvariant();
            }
            else
            {
                return Path.GetFullPath(path)
                .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                .ToUpperInvariant();
            }
            
        }

        private void DragArea_Loaded(object sender, RoutedEventArgs e)
        {
            Window.Current.SetTitleBar(sender as Grid);
        }

        public void TabStrip_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(TabStrip.SelectedItem == null)
            {
                if(e.RemovedItems.Count > 0)
                {
                    var itemToReselect = e.RemovedItems[0];
                    if (TabStrip.TabItems.Contains(itemToReselect))
                    {
                        TabStrip.SelectedItem = itemToReselect;
                    }
                }
            }
            else
            {
                Microsoft.UI.Xaml.Controls.FontIconSource icon = new Microsoft.UI.Xaml.Controls.FontIconSource();
                icon.Glyph = "\xE713";
                if ((tabView.SelectedItem as TabViewItem).Header.ToString() != "Settings" && (tabView.SelectedItem as TabViewItem).IconSource != icon)
                {
                    App.selectedTabInstance = ItemViewModel.GetCurrentSelectedTabInstance<ProHome>();
                }
            }

        }

        private void TabStrip_TabCloseRequested(Microsoft.UI.Xaml.Controls.TabView sender, Microsoft.UI.Xaml.Controls.TabViewTabCloseRequestedEventArgs args)
        {
            if (TabStrip.TabItems.Count == 1)
            {
                Application.Current.Exit();
            }
            else if(TabStrip.TabItems.Count > 1)
            {
                int tabIndexToClose = TabStrip.TabItems.IndexOf(args.Tab);
                TabStrip.TabItems.RemoveAt(tabIndexToClose);
            }
            
        }

        private void TabStrip_AddTabButtonClick(TabView sender, object args)
        {
            AddNewTab(typeof(ProHome), "New tab");
        }
    }

}
