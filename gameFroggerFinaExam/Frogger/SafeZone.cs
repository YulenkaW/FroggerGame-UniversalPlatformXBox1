using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
using Windows.UI;

namespace Frogger
{
    internal class SafeZone : IDrawable
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public Color Color { get; set; }  // Property for color

        // Constructor to set properties
        public SafeZone(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            Color = Color.FromArgb(128, 0, 255, 0);  // Semi-transparent green
        }

        // Draw the SafeZone
        public void Draw(CanvasDrawingSession session)
        {
            using (var brush = new CanvasSolidColorBrush(session, Color))
            {
                session.FillRectangle(X, Y, Width, Height, brush);
            }
            
        }

        // Checking if frog is in the zone
        public bool Contains(Rectangle frog)
        {
            return frog.X < X + Width && frog.X + frog.Width > X &&
                   frog.Y < Y + Height && frog.Y + frog.Height > Y;
        }
    }
}
