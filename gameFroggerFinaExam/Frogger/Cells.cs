using Microsoft.Graphics.Canvas; 
using Frogger;
using System.Drawing;   
using Windows.UI;   

namespace Frogger
{
    public class SafeZone : IDrawable
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public Color Color { get; set; }

        public SafeZone(float x, float y, float width, float height, Color color)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            Color = color;
        }

        public bool Contains(Rectangle frog)
        {
            // Checking if frog is within safe zone
            return frog.X < X + Width && frog.X + frog.Width > X &&
                   frog.Y < Y + Height && frog.Y + frog.Height > Y;
        }

        public void Draw(ICanvasResourceCreatorWithDpi sender, CanvasDrawingSession session)
        {
            // Drawin safe zone
            session.FillRectangle(X, Y, Width, Height, Color);
        }
    }
}
private List<SafeZone> safeZones;

public MainPage()
{
    this.InitializeComponent();
    // intialize
    InitializeSafeZones();
}

private void InitializeSafeZones()
{
    safeZones = new List<SafeZone>();
    // for 5 lanes and 80 pixels wide
    for (int i = 0; i < 5; i++)
    {
        safeZones.Add(new SafeZone(i * 100, 0, 80, 50, Colors.LightGreen));
    }
}

private void Canvas_Draw(ICanvasAnimatedControl sender, CanvasAnimatedDrawEventArgs args)
{
    foreach (var safeZone in safeZones)
    {
        safeZone.Draw(sender, args.DrawingSession);
    }

    //draw code
}
private void GameLoop(object sender, object e)
{
    // add in game loop

    foreach (var safeZone in safeZones)
    {
        if (safeZone.Contains(frog))
        {
            
            ScorePoints();
            ResetFrogPosition();
            break;
        }
    }
}
