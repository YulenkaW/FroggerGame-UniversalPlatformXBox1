using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Gaming.Input;
using Windows.Media.Devices;
using Windows.UI;
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
    public sealed partial class MainMenu : Page
    {
        public MainMenu()
        {
            this.InitializeComponent();
            
            startButton.UseSystemFocusVisuals = true;
            controlsButton.UseSystemFocusVisuals = true;
            optionsbutton.UseSystemFocusVisuals = true;
            CredtisButton.UseSystemFocusVisuals = true;
            

        }
        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage) , null);
        }

        private void ControlButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate (typeof(ControlsPage) , null);
        }

        private void OptionsButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Options) , null);
        }

        private void CreditsButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(CreditsPage) , null);
        }

        private void Canvas_Update(ICanvasAnimatedControl sender, CanvasAnimatedUpdateEventArgs args)
        {
            if (Gamepad.Gamepads.Count > 0)
            {
                Gamepad gamepad = Gamepad.Gamepads.First();
                var reading = gamepad.GetCurrentReading();

                if (reading.Buttons.HasFlag(GamepadButtons.B))
                {

                }
               
                
            }
        }
    }
}

