using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Frogger
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Options : Page
    {
        public Options()
        {
            this.InitializeComponent();
            SystemNavigationManager.GetForCurrentView().BackRequested += SystemNavigationManager_BackRequested;
        }

        private void OptionsBackButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.GoBack();
        }
        private void SystemNavigationManager_BackRequested(object sender, BackRequestedEventArgs e)
        {
            if (!e.Handled)
            {
                e.Handled = this.BackRequest();
            }
        }
        public Frame appFrame { get { return this.Frame; } }
        private bool BackRequest()
        {
            // Get a hold of the current frame so that we can inspect the app back stack
            if (this.appFrame == null)
                return false;

            // Check to see if this is the top-most page on the app back stack
            if (this.appFrame.CanGoBack)
            {
                // If not, set the event to handled and go back to the previous page in the
                // app.
                this.appFrame.GoBack();
                return true;
            }
            return false;

        }

          private void MusicToggleSwitch_Toggled(object sender, RoutedEventArgs e)
          {
               var toggleSwitch = sender as ToggleSwitch;
               if (toggleSwitch != null)
               {
                    ApplicationData.Current.LocalSettings.Values["MusicEnabled"] = toggleSwitch.IsOn;
               }
          }

          protected override void OnNavigatedTo(NavigationEventArgs e)
          {
               base.OnNavigatedTo(e);
               bool musicEnabled = GetMusicPreference();
               MusicToggleSwitch.IsOn = musicEnabled;
          }

          private bool GetMusicPreference()
          {
               if (ApplicationData.Current.LocalSettings.Values.TryGetValue("MusicEnabled", out object musicSetting))
               {
                    return (bool)musicSetting;
               }
               // Default to true if the setting hasn't been set yet
               return true;
          }

     }

}
