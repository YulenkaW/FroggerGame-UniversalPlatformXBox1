using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;

namespace Frogger
{
    public sealed partial class MainPage : Page
    {
        private List<Distraction> distractions;
        private DispatcherTimer gameTimer;
        private List<CanvasBitmap> images;
        private Rectangle frog;
        private List<IDrawable> drawables;
        private Windows.UI.Core.KeyEventArgs preKeyEvent;
        private List<Cell> cells;

        public MainPage()
        {
            this.InitializeComponent();

            Window.Current.CoreWindow.KeyDown += Canvas_KeyPressed;
            Window.Current.CoreWindow.KeyUp += Canvas_KeyLift;

            distractions = new List<Distraction>();
            frog = new Rectangle
            {
                X = 50,
                Y = 50,
                Width = 20,
                Height = 20,
                Color = Colors.White
            };
            drawables = new List<IDrawable>();
            drawables.Add(frog);

            // Initialize cells
            cells = new List<Cell>();
            for (int i = 0; i < 4; i++)
            {
                cells.Add(new Cell
                {
                    X = 50 + i * 100,  
                    Y = 0,             // Top of the map
                    Width = 20,         // Cell width
                    Height = 20,        // Cell height
                    Color = Colors.Green // Color of the cells
                });
            }
            drawables.AddRange(cells);

            // Initialize game timer
            gameTimer = new DispatcherTimer();
            gameTimer.Interval = TimeSpan.FromMilliseconds(33); // ~30 FPS
            gameTimer.Tick += GameLoop;
            gameTimer.Start();
        }

        private void Canvas_KeyLift(CoreWindow sender, KeyEventArgs args)
        {
            preKeyEvent = null;
        }

        private void Canvas_KeyPressed(CoreWindow sender, KeyEventArgs e)
        {
            if (preKeyEvent is null || preKeyEvent.VirtualKey != e.VirtualKey)
            {
                if (e.VirtualKey == Windows.System.VirtualKey.Left)
                {
                    frog.X -= 20;
                }
                else if (e.VirtualKey == Windows.System.VirtualKey.Right)
                {
                    frog.X += 20;
                }
                else if (e.VirtualKey == Windows.System.VirtualKey.Up)
                {
                    frog.Y -= 20;
                }
                else if (e.VirtualKey == Windows.System.VirtualKey.Down)
                {
                    frog.Y += 20;
                }
                preKeyEvent = e;
            }
        }

        private void GameLoop(object sender, object e)
        {
            // Update game state
            foreach (var distraction in distractions)
            {
                distraction.Update();
            }

            // Check for collisions
            foreach (var cell in cells)
            {
                if (frog.X >= cell.X && frog.X <= cell.X + cell.Width &&
                    frog.Y >= cell.Y && frog.Y <= cell.Y + cell.Height)
                {
                    
                }
            }
        }

        private void Canvas_Draw(ICanvasAnimatedControl sender, CanvasAnimatedDrawEventArgs args)
        {
           
            foreach (IDrawable draw in drawables)
            {
                draw.Draw(args.DrawingSession);
            }
        }

        private void Canvas_Update(ICanvasAnimatedControl sender, CanvasAnimatedUpdateEventArgs args)
        {

        }

        private async void Canvas_CreateResources(CanvasAnimatedControl sender, Microsoft.Graphics.Canvas.UI.CanvasCreateResourcesEventArgs args)
        {
         
            string assetsFolderPath = Windows.ApplicationModel.Package.Current.InstalledLocation.Path + "\\Assets\\";
            images = new List<CanvasBitmap>();

            var image1 = await CanvasBitmap.LoadAsync(sender, new Uri($"ms-appx:///Assets/wood4.png"));
            images.Add(image1);

            var image2 = await CanvasBitmap.LoadAsync(sender, new Uri($"ms-appx:///Assets/truckLeft.png"));
            images.Add(image2);

            var image3 = await CanvasBitmap.LoadAsync(sender, new Uri($"ms-appx:///Assets/arrow.png"));
            images.Add(image3);

            for (int i = 0; i < 1; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    int x = 50 + (j * 200);
                    int y = 50 + (i * 50);

                    distractions.Add(new Distraction(x + 50, y, image1, true, 2));  // right
                    distractions.Add(new Distraction(x + 50, y + 50, image2, false, 5)); // left
                    distractions.Add(new Distraction(x + 50, y + 100, image3, true, 4));  // right
                }
            }

            if (images.Count > 0)
            {
                drawables.AddRange(distractions);
            }
        }
    }

    public interface IDrawable
    {
        void Draw(CanvasDrawingSession session);
    }

    public class Distraction : IDrawable
    {
        private int x;
        private int y;
        private CanvasBitmap image;
        private bool moveRight;
        private int speed;

        public Distraction(int x, int y, CanvasBitmap image, bool moveRight, int speed)
        {
            this.x = x;
            this.y = y;
            this.image = image;
            this.moveRight = moveRight;
            this.speed = speed;
        }

        public void Update()
        {
            if (moveRight)
            {
                x += speed;
                if (x > 800) // Width of the canvas
                {
                    x = -100;
                }
            }
            else
            {
                x -= speed;
                if (x < -100)
                {
                    x = 800;
                }
            }
        }

        public void Draw(CanvasDrawingSession session)
        {
            session.DrawImage(image, new Rect(x, y, 100, 100), new Rect(0, 0, image.Size.Width, image.Size.Height));
        }
    }

    public class Rectangle : IDrawable
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public Color Color { get; set; }

        public void Draw(CanvasDrawingSession session)
        {
            session.FillRectangle(X, Y, Width, Height, Color);
        }
    }

    public class Cell : IDrawable
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public Color Color { get; set; }

        public void Draw(CanvasDrawingSession session)
        {
            session.FillRectangle(X, Y, Width, Height, Color);
        }
    }
}
