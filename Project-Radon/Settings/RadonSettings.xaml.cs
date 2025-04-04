﻿using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Yttrium;
using muxc = Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Project_Radon.Settings
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RadonSettings : muxc.Page
    {
        public RadonSettings()
        {
            InitializeComponent();
            NavView.ItemInvoked += async (_, e) =>
            {
                if (!e.IsSettingsInvoked)
                    switch ((e.InvokedItemContainer as NavigationViewItem).Tag.ToString())
                    {
                        case "personalize":
                            Settings_Frame.Navigate(typeof(RadonSettings_Personalize), null);
                            break;
                        case "user":
                            Settings_Frame.Navigate(typeof(RadonSettings_User), null);
                            break;
                        case "advanced":
                            Settings_Frame.Navigate(typeof(RadonSettings_Advanced), null);
                            break;
                        case "about":
                            await new AboutDialog().ShowAsync();
                            break;
                        default: //Means else 
                            Settings_Frame.Navigate(typeof(RadonSettings_General), null);
                            break;
                    }
            };
        }

        private readonly List<(string Tag, Type Page)> _pages = new List<(string Tag, Type Page)>
        {
            ("general", typeof(RadonSettings_General)),
            ("personalize", typeof(RadonSettings_Personalize)),
            ("user", typeof(RadonSettings_User)),
            ("advanced", typeof(RadonSettings_Advanced)),
            ("about", typeof(AboutDialog)),
        };


        private void NavView_Loaded(object sender, RoutedEventArgs e)
        {
            Settings_Frame.Navigate(typeof(RadonSettings_General), null);
        }

        private void NavView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            //if (NavView.Tag == "general")
            //{
            //    Settings_Frame.Navigate(typeof(RadonSettings_General));
            //}

            //else if (NavView.Tag == "personalize")
            //{
            //    Settings_Frame.Navigate(typeof(RadonSettings_Personalize));
            //}

            //else if (NavView.Tag == "user")
            //{
            //    Settings_Frame.Navigate(typeof(RadonSettings_User));
            //}

            //else if (NavView.Tag == "advanced")
            //{
            //    Settings_Frame.Navigate(typeof(RadonSettings_Advanced));
            //}

            //else if (NavView.Tag == "about")
            //{
            //    await new AboutDialog().ShowAsync();
            //}


        }


    }
}
