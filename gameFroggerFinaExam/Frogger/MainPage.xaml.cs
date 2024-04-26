using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Microsoft.Graphics.Canvas.UI;
using NuGet.Protocol.Plugins;
using Windows.Gaming.Input;
using System.Diagnostics;
using System.Reflection.Metadata;
using NuGet.Packaging;
using static System.Net.Mime.MediaTypeNames;
using Newtonsoft.Json.Linq;
using Windows.Storage;





// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409
namespace Frogger
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    /// 

    public sealed partial class MainPage : Page
    {
        private List<Distraction> distractions;
        private List<CanvasBitmap> images;
        private DispatcherTimer playMusicTimer;
        private DispatcherTimer gameTimer;
        private DispatcherTimer gameLoopTimer;
        private int totalTimeInSeconds = 90; // Initialize the countdown to 90 seconds

        private CanvasBitmap boomImage;
        private CanvasBitmap tortoiseImage;
        private CanvasBitmap cellImage;
        private CanvasBitmap frogInCellImage;

        private int lives;
        private int frogY;
        private int frogX;
        private int[] cellIndex = { 0, 0, 0, 0, 0 };
        private int totalCellsFilled;
      
        
        Score score;
        Rectangle frog;
        List<Rectangle> cells;

        private const int LeftBorder = 10;
        private const int RightBorder = 930;
        private const int BottomBorder = 540;
        private const int TopBorder = 0;
        private double WaterTop = 0;
        private double WaterBottom = 200;
        
        List<IDrawable> drawables;

        //Key used to determine if a key is pressed
        Windows.UI.Core.KeyEventArgs preKeyEvent;
       

        public MainPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Required;
            SetupGameTimer(); // Set up the game timer

            lives = 3;
            frogY = 0;
            totalCellsFilled = 0;

            UpdateLivesDisplay();

            playMusicTimer = new DispatcherTimer();
            playMusicTimer.Interval = TimeSpan.FromSeconds(0.3); // Adjust the delay as needed
            playMusicTimer.Tick += PlayMusicTimer_Tick;
            playMusicTimer.Start();

            Window.Current.CoreWindow.KeyDown += Canvas_KeyPressed;
            Window.Current.CoreWindow.KeyUp += Canvas_KeyLift;
            SystemNavigationManager.GetForCurrentView().BackRequested += SystemNavigationManager_BackRequested;

            distractions = new List<Distraction>();
            score = new Score();

            //Frog, x and y are the cordinate where its drawn
            frog = new Rectangle(400, 500, 50, 50)
            {
                X = 400,
                Y = 450,
                Width = 50,
                Height = 50,
                color = Colors.White
            };

            drawables = new List<IDrawable>();
            drawables.Add(frog);

            cells = new List<Rectangle>();

            for (int i = 0; i < 5; i++) { cells.Add(new Rectangle(1000, 1000, 50, 50)); }

            
        }

          private void GameTimer_Tick(object sender, object e)
          {
            
            totalTimeInSeconds--;

            // We have a TextBlock named CountdownText in our XAML
            CountdownText.Text = TimeSpan.FromSeconds(totalTimeInSeconds).ToString("mm':'ss");

            // Check if the time has run out
            if (totalTimeInSeconds <= 0)
            {
                // The level is over
                HandleGameComplete();
            }
          }
        private void HandleGameComplete()
        {
            // Stop the game timer as the level is complete or time is up
            gameTimer.Stop();
            gameLoopTimer.Stop();



            ShowTimerOverScreen(); 
        }
        private void SetupGameTimer()
        {
            gameTimer = new DispatcherTimer();
            gameLoopTimer = new DispatcherTimer();
            gameTimer.Interval = TimeSpan.FromSeconds(1); // Timer ticks every second
            gameLoopTimer.Interval = TimeSpan.FromMilliseconds(33);
            gameLoopTimer.Tick += GameLoop;
            gameTimer.Tick += GameTimer_Tick;
            gameTimer.Start(); // Start the countdown
            gameLoopTimer.Start();
        }
        private void ShowTimerOverScreen()
        {
            // Show game over message
            GameOverTextBlock.Visibility = Visibility.Visible;
            GameOverTextBlock.Text = "Game Over! Click to restart.";
            GameOverTextBlock.Tapped += RestartGame;
            GameOverButton.Visibility = Visibility.Visible;
            GameOverButton.IsEnabled = true;
            GameOverButton.Click += RestartGame;
        }

        public void FrogReachedCell(int cellNumber)
        {
            // Assuming cellNumber is a zero-based index of the cell where the frog has reached
            if (cellIndex[cellNumber] == 0) // Check if the cell is not already filled
            {
                cellIndex[cellNumber] = 1; // Mark the cell as filled
                totalCellsFilled++; // Increment the total cells filled
                CheckForLevelCompletion(); // Check if the level is completed
            }
        }
        private void CheckForLevelCompletion()
        {
            // Assuming there are 5 cells in total to be filled
            if (totalCellsFilled == 5)
            {
                // All cells are filled, the level is complete
                HandleGameComplete();
            }
        }

        // Code used to deal with the back fucntion caused by B sbox button, created with the help of learn.microsoft.com
        private void SystemNavigationManager_BackRequested(object sender, BackRequestedEventArgs e)
        {
            if (!e.Handled)
            {
                e.Handled = this.BackRequest();
            }
        }
        
        private void Canvas_CreateResources(CanvasAnimatedControl sender, CanvasCreateResourcesEventArgs args)
        {
            args.TrackAsyncAction(CreateResources(sender).AsAsyncAction());
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


        private void Canvas_KeyLift(CoreWindow sender, KeyEventArgs args)
        {
            // sets key null to allow for single click movement
            preKeyEvent = null;
        }

        private void Canvas_KeyPressed(Windows.UI.Core.CoreWindow sender, Windows.UI.Core.KeyEventArgs e)
        {
            // Checks previous key to have single click movement
            if (preKeyEvent is null || preKeyEvent.VirtualKey != e.VirtualKey)
            {
                int moveDistance = 50; // Distance to move on key press


                int currentX = frog.X;
                int currentY = frog.Y;

                switch (e.VirtualKey)
                {
                    case Windows.System.VirtualKey.Left:
                    case Windows.System.VirtualKey.GamepadDPadLeft:
                        currentX -= moveDistance;
                        break;
                    case Windows.System.VirtualKey.Right:
                    case Windows.System.VirtualKey.GamepadDPadRight:
                        currentX += moveDistance;
                        break;
                    case Windows.System.VirtualKey.Up:
                    case Windows.System.VirtualKey.GamepadDPadUp:
                        currentY -= moveDistance;
                        break;
                    case Windows.System.VirtualKey.Down:
                    case Windows.System.VirtualKey.GamepadDPadDown:
                        currentY += moveDistance;
                        break;
                }


                if (IsWithinBorders(currentX, currentY))
                {
                    frog.X = currentX;
                    frog.Y = currentY;
                  //  IsSafePosition(currentX, currentY);
                    //dateScoreOnPosition(currentX, currentY);
                    //UpdateScore();
                }
            }
        }


        private bool IsWithinBorders(int x, int y)
        {
           
            return x >= LeftBorder && x + frog.Width <= RightBorder &&
                   y >= TopBorder && y + frog.Height <= BottomBorder;
        }

          private bool IsSafePosition(int x, int y)
          {
               frog.X = x;
               frog.Y = y;

               bool isFrogInWater = y >= WaterTop && y + frog.Height <= WaterBottom;
               bool isOnSafeDistraction = false;

               foreach (var distraction in distractions)
               {
                    if (frog.Intersects(distraction))
                    {
                         Console.WriteLine($"Intersection with Distraction - Safe: {distraction.IsSafe}");
                         if (distraction.IsSafe)
                         {
                              isOnSafeDistraction = true;
                              break;
                         }
                    }
               }

               if (isFrogInWater)
               {
                    if (isOnSafeDistraction)
                    {
                         return true; // Frog is safe because it's on a safe distraction
                    }
                    else
                    {
                         return false; // Unsafe if in water and not on a safe distraction
                    }
               }

               return true; // Safe if not in water or on a safe distraction
          }

          private void UpdateScore()
          {

            frogY = frog.Y;
            score.SetMovementScore(frogY);
            totalScoreTextBox.Text = $"Total Score: {score.GetScore()}";
          }


        private void UpdateScoreOnPosition(int x, int y)
        {
            foreach (var distraction in distractions)
            {
                if (frog.Intersects(distraction))
                {
                    if (distraction.IsHarmful)
                    {
                        score.Decrement();
                    }
                    else if (distraction.IsSafe)
                    {
                        if (distraction.Image == cellImage)
                        {
                            // Increment score by 500 when the frog reaches a cell
                            score.SetCellScore();
                        }
                        else
                        {
                            score.Increment();
                        }
                    }

                }
            }
        }

        private void GameLoop(object sender, object e)
        {
            int screenWidth = (int)canvas.ActualWidth;

            foreach (var distraction in distractions)
            {
                distraction.Update(screenWidth);

                // Check if there is a collision with the frog
                if (frog.Intersects(distraction))
                {
                    if (distraction.IsHarmful)
                    {
                        HandleCollision();
                    }
                    else
                    {
                        MoveFrogWithDistraction(distraction);// frog moves with harmless distractions
                    }
                }

            }

            CheckFrogAtEdge();

               if (!IsSafePosition(frog.X, frog.Y))
               {
                    ResetFrogPosition();
                    DecreaseLives(); // Reset if position not safe
               }


            frogY = frog.Y;
            frogX = frog.X;
            score.SetMovementScore(frogY);
            if  (frogY < 40)
            {
                if( cellIndex[0] == 0 && ( ( (frogX > 40) && (frogX < 120) ) || 
                    ( ( (frogX + 60) > 40) && ((frogX + 60) < 120) ) ) )
                {
                    cellIndex[0] = 1;
                    score.SetCellScore();
                    cells[0].X = 40;
                    cells[0].Y = 0;
                    totalCellsFilled++;
                }
                else if (cellIndex[1] == 0 && (((frogX > 220) && (frogX < 300)) ||
                    (((frogX + 60) > 220) && ((frogX + 60) < 300))))
                {
                    cellIndex[1] = 1;
                    score.SetCellScore();
                    cells[1].X = 220;
                    cells[1].Y = 0;
                    totalCellsFilled++;
                }
                else if (cellIndex[2] == 0 && (((frogX > 400) && (frogX < 480)) ||
                    (((frogX + 60) > 400) && ((frogX + 60) < 480))))
                {
                    cellIndex[2] = 1;
                    score.SetCellScore();
                    cells[2].X = 400;
                    cells[2].Y = 0;
                    totalCellsFilled++;
                }
                else if (cellIndex[3] == 0 && (((frogX > 580) && (frogX < 660)) ||
                    (((frogX + 60) > 580) && ((frogX + 60) < 660))))
                {
                    cellIndex[3] = 1;
                    score.SetCellScore();
                    cells[3].X = 580;
                    cells[3].Y = 0;
                    totalCellsFilled++;
                }
                else if (cellIndex[4] == 0 && (((frogX > 760) && (frogX < 840)) ||
                    (((frogX + 60) > 760) && ((frogX + 60) < 840))))
                {
                    cellIndex[4] = 1;
                    score.SetCellScore();
                    cells[4].X = 760;
                    cells[4].Y = 0;
                    totalCellsFilled++;
                }
                else
                {
                    DecreaseLives();
                }
                ResetFrogPosition();
            }
            CheckBoxCells();
            totalScoreTextBox.Text = $"Total Score: {score.GetScore()}";
        }

        private void CheckBoxCells()
        {
            if (totalCellsFilled == 5)
            {
                for (int i = 0; i < totalCellsFilled; i++)
                {
                    cellIndex[i] = 0;
                    cells[i].X = 1000;
                    cells[i].Y = 1000;
                }
                totalCellsFilled = 0;
            }
        }
        
        private void ResetCells()
        {
            totalCellsFilled = 0;
            for (int i = 0; i < cellIndex.Length; i++)
            {
                cellIndex[i] = 0;
                cells[i].X = 1000;
                cells[i].Y = 1000;
            }
        }

        private void CheckFrogAtEdge()
        {
            // System.Diagnostics.Debug.WriteLine($"Frog X = {frog.X}, Frog Y = {frog.Y}");

            if (frog.X < LeftBorder || frog.X + frog.Width > RightBorder)
            {
                DecreaseLives();
                ResetFrogPosition(); // Optionally reset the frog's position
            }
        }

        private void MoveFrogWithDistraction(Distraction distraction)
        {
            if (!distraction.IsHarmful)
            {
                frog.X += distraction.Speed; // frog moves with harmless distraction
                CheckFrogAtEdge();
            }

        }

        private void HandleCollision()
        {

            // Example of handling a collision, can adjust 
            ResetFrogPosition();
            DecreaseLives();
        }

        private void ResetFrogPosition()
        {
            // Reset frog to initial spot
            frog.X = 400;
            frog.Y = 450;
            score.BreakPointReset();
        }

        private void DecreaseLives()
        {
            lives--;
            UpdateLivesDisplay();

            if (lives == 0)
            {
                GameOver();
            }
        }

        private void UpdateLivesDisplay()
        {
            LivesTextBlock.Text = $"Lives: {lives}";
        }

        private void GameOver()
        {
            ShowGameOverScreen();
            StopGame();
            StopMusic();
        }

        private void StopGame()
        {
            if (gameTimer != null && gameTimer.IsEnabled)
            {
                gameTimer.Stop();
            }
            if (gameLoopTimer != null && gameLoopTimer.IsEnabled)
            {
                gameLoopTimer.Stop();
            }
            if (playMusicTimer != null && playMusicTimer.IsEnabled)
            {
                 playMusicTimer.Stop();  // Stop the music timer if it's running
            }

          }

        private void ShowGameOverScreen()
        {
            GameOverTextBlock.Visibility = Visibility.Visible;
            GameOverTextBlock.Text = "Game Over! Click to restart.";
            GameOverTextBlock.Tapped += RestartGame;
            GameOverButton.Visibility = Visibility.Visible;
            GameOverButton.IsEnabled = true;
            GameOverButton.Click += RestartGame;
        }

        public void RestartGame(object sender, RoutedEventArgs e)
        {
            GameOverTextBlock.Visibility = Visibility.Collapsed;
            GameOverButton.Visibility = Visibility.Collapsed;
            GameOverButton.IsEnabled = false;
            lives = 3;
            totalTimeInSeconds = 90;
            //SetupGameTimer(); Not needed for restart
            UpdateLivesDisplay();
            ResetFrogPosition();
            gameTimer.Start();
            gameLoopTimer.Start();
            score.FullReset();
            ResetCells();
            ResetMusic();
            ApplyMusicPreference();
          }
          private void StopMusic()
          {
               if (BackgroundMusicPlayer.CurrentState != MediaElementState.Stopped)
               {
                    BackgroundMusicPlayer.Stop();
                    //Debug.WriteLine("Music stopped due to game over.");
               }
          }

          protected override void OnNavigatedTo(NavigationEventArgs e)
          {
               base.OnNavigatedTo(e);
               ApplyMusicPreference();
          }

          private void ApplyMusicPreference()
          {
               bool musicEnabled = GetMusicPreference();
               if (musicEnabled)
               {
                    ResetMusic();
               }
               else
               {
                    BackgroundMusicPlayer.Stop();
               }
          }

          private bool GetMusicPreference()
          {
               if (ApplicationData.Current.LocalSettings.Values.TryGetValue("MusicEnabled", out object musicSetting))
               {
                    return (bool)musicSetting;
               }
               return true; // Default value if not set
          }

          private void ResetMusic()
          {
               bool musicEnabled = GetMusicPreference();
               if (!musicEnabled)
               {
                    //Debug.WriteLine("Music disabled by user preference.");
                    BackgroundMusicPlayer.Stop(); // Stop the music if preference is off
                    return;
               }

               //Debug.WriteLine("Resetting music playback.");
               BackgroundMusicPlayer.Stop();
               BackgroundMusicPlayer.Position = TimeSpan.Zero;
               BackgroundMusicPlayer.Play();
          }

          // Handler for keyboard input
          protected override void OnKeyDown(KeyRoutedEventArgs e)
        {
            base.OnKeyDown(e);

            // Handle key down events, e.g., moving the frog 
        }
          private void PlayMusicTimer_Tick(object sender, object e)
          {
               bool musicEnabled = GetMusicPreference();

               // If music is disabled, stop the timer and the music
               if (!musicEnabled)
               {
                    //Debug.WriteLine("Music disabled by user preference. Stopping timer and music playback.");
                    playMusicTimer.Stop();
                    BackgroundMusicPlayer.Stop();
                    return;
               }

               // If the player has a source and the music is not currently playing, start playback
               if (BackgroundMusicPlayer.Source != null &&
                   (BackgroundMusicPlayer.CurrentState == MediaElementState.Paused ||
                    BackgroundMusicPlayer.CurrentState == MediaElementState.Stopped))
               {
                    //Debug.WriteLine("Playing music from timer tick.");
                    BackgroundMusicPlayer.Play();
               }
               else
               {
                    //Debug.WriteLine($"Timer tick but music is already playing or no source set.");
               }

               // Consider whether the timer should continue to tick or if it should stop after music starts playing.
               // If it's meant to check at regular intervals (e.g., in case music gets paused unexpectedly), don't stop the timer here.
          }


          private void BackgroundMusicPlayer_Loaded(object sender, RoutedEventArgs e)
          {
               if (GetMusicPreference())
               {
                    ResetMusic();
               }
          }

        private void BackgroundMusicPlayer_MediaEnded(object sender, RoutedEventArgs e)
        {
               if (GetMusicPreference())
               {
                    ResetMusic();
               }
        }

        private void Canvas_Draw(ICanvasAnimatedControl sender, CanvasAnimatedDrawEventArgs args)
        {

            //this method is drawing figures
            //here frog is above everything else

            foreach (IDrawable draw in drawables)
            {
                if (!(draw is Rectangle))
                {
                    draw.Draw(args.DrawingSession);

                }
               
            }

            foreach (Rectangle rec in cells)
            {
                rec.Draw(args.DrawingSession);
            }

            // Now draw the frog last so it appears on top
            frog.Draw(args.DrawingSession);

        }

        // this one moves objects
        private void Canvas_Update(ICanvasAnimatedControl sender, CanvasAnimatedUpdateEventArgs args)
        {
            if (Gamepad.Gamepads.Count > 0)
            {
                Gamepad gamepad = Gamepad.Gamepads.First();
                var reading = gamepad.GetCurrentReading();
                frog.X += (int)(reading.LeftThumbstickX * 3);
                frog.Y += (int)(reading.LeftThumbstickY * -3);
                
            }

        }
        private async Task LoadFrogInCellImage(CanvasAnimatedControl sender)
        {
            frogInCellImage = await CanvasBitmap.LoadAsync(sender, new Uri("ms-appx:///Assets/frogincell.png"));
        }
        

        private async Task CreateResources(CanvasAnimatedControl sender)
        {
            
            // Set up rows and columns for the distractions based on your needs
            int numberOfLines = 8;
            int safeZone = 8;
            int snakeRow = 4;
            int trucksRow = 5;
            int carsRow = 7;
            int raceCarRow = 6;
            int boxRow1 = 3;
            int logRow = 2;
            int boxRow3 = 1;
            int cellsRow = 0;

            string assetsFolderPath = Package.Current.InstalledLocation.Path + "\\Assets\\";
            images = new List<CanvasBitmap>();

            var image1 = await CanvasBitmap.LoadAsync(sender, new Uri("ms-appx:///Assets/woodBoxResized.png"));  //wood box
            images.Add(image1);

            var image2 = await CanvasBitmap.LoadAsync(sender, new Uri($"ms-appx:///Assets/woodLogResized.png")); // wood log
            images.Add(image2);

            var image3 = await CanvasBitmap.LoadAsync(sender, new Uri($"ms-appx:///Assets/woodBoxResized.png")); // wood box
            images.Add(image3);

            var image5 = await CanvasBitmap.LoadAsync(sender, new Uri($"ms-appx:///Assets/redTruckResized.png")); //red truck
            images.Add(image5);

            var image6 = await CanvasBitmap.LoadAsync(sender, new Uri($"ms-appx:///Assets/tacoTruck2Resized.png")); //taco truck
            images.Add(image6);

            var image8 = await CanvasBitmap.LoadAsync(sender, new Uri($"ms-appx:///Assets/raceCarResized.png")); //race car
            images.Add(image8);

            cellImage = await CanvasBitmap.LoadAsync(sender, new Uri("ms-appx:///Assets/frogCellResized.png")); // frog cell

            var image4 = await CanvasBitmap.LoadAsync(sender, new Uri($"ms-appx:///Assets/frog2Resized.png"));
            frog.image = image4;

            var imageCellFrog = await CanvasBitmap.LoadAsync(sender, new Uri($"ms-appx:///Assets/frog2Resized.png")); //frog in cell
            frogInCellImage = imageCellFrog;

            var image10 = await CanvasBitmap.LoadAsync(sender, new Uri($"ms-appx:///Assets/harmfulSnake.png")); // snake
            images.Add(image10);

            for (int i = 0; i < 5; i++)
            {
                cells[i].image = frogInCellImage;
            }

            await LoadFrogInCellImage(sender);
            // await LoadFrogInCellImage(sender);
            int gridSize = 50;

            // Add distractions with your defined logic
            //added one more parameter
            AddDistractionsForRow(cellsRow, 3, cellImage, false, 0, false, true);  // Cells row with  cells spread equally
            AddDistractionsForRow(boxRow1, 5, image3, true, -4, false, true);  // First row of boxes spaced out
            AddDistractionsForRow(logRow, 7, image2, true, 5, false, true);  // Second row of boxes - pairs of boxes
            AddDistractionsForRow(boxRow3, 4, image3, true, -7, false, true);  // Third row of boxes spaced out
            AddDistractionsForRow(trucksRow, 4, image5, true, -8, true, false);  // Trucks row
            AddDistractionsForRow(carsRow, 4, image6, true, 5, true, false);  // Cars row
            AddDistractionsForRow(raceCarRow, 7, image8, true, 14, true, false); //Race car row
            AddDistractionsForRow(snakeRow, 10, image10, true, 4, true, false); // snake


            
            if (images.Count > 0)
            {
                drawables.AddRange(distractions);
            }

        }

        // This is the helper method for adding distractions in a row with a specific spacing
        private void AddDistractionsForRow(int line, int spacingMultiplier, CanvasBitmap image, bool isMoving, int speed, bool isHarmful, bool isSafe)
        {
            int gridSize = 50; // define gridSize here if it's not a class member

            int startX = (((int)canvas.ActualWidth % gridSize) / 2) + 40; // This is based on your previous code
            int startY = 0 + (line * gridSize); // Calculate the starting Y based on the line number
            int spaceBetween = spacingMultiplier * 60; // Calculate space between distractions

            // Determine the number of distractions that can fit on a row based on screen width
            int distractionsPerRow = ((int)canvas.ActualWidth - startX) / spaceBetween;

            for (int col = 0; col < distractionsPerRow; col++)
            {
                int x = startX + (col * spaceBetween);
                distractions.Add(new Distraction(x, startY, image, isMoving, speed, isHarmful, isSafe));
            }
        }

        private void canvas_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}












