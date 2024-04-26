using Microsoft.Graphics.Canvas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;

namespace Frogger
{
    public class Distraction : IDrawable
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; } = 50; // Default size
        public int Height { get; set; } = 50; // Default size
        public bool IsMoving { get; set; }
        public int Speed { get; set; }
        public CanvasBitmap Image { get; set; }
        public bool IsHarmful { get; set; } // differentiate between boxes(f) and cars(t)
        public bool IsSafe { get; set; }

        public Distraction(int x, int y, CanvasBitmap image, bool isMoving, int speed, bool isHarmful, bool isSafe)
        {
            X = x;
            Y = y;
            Image = image;
            IsMoving = isMoving;
            Speed = speed;
            IsHarmful = isHarmful;
            IsSafe = isSafe;
        }

          public void Update(int screenWidth)
          {
               X += Speed;

               // Rightward movement wrap-around
               if (Speed > 0 && X > screenWidth) // Right edge of the distraction goes past the screen width
               {
                    // Object should re-enter from the left
                    X = (int)-Image.SizeInPixels.Width;
               }
               // Leftward movement wrap-around
               else if (Speed < 0 && X + Image.SizeInPixels.Width < 0) // Left edge of the distraction goes past screen left
               {
                    // Object should re-enter from the right
                    X = screenWidth;
               }
          }



          // New method to check if a given point (x, y) is within the distraction
          public bool Contains(int pointX, int pointY)
          {
               return (pointX >= X && pointX <= X + Image.SizeInPixels.Width) &&
                      (pointY >= Y && pointY <= Y + Image.SizeInPixels.Height);
          }

          public void Draw(CanvasDrawingSession session)
          {
               if (Image != null)
               {
                    float centeredX = X + (Width / 2) - (Image.SizeInPixels.Width / 2);
                    float centeredY = Y + (Height / 2) - (Image.SizeInPixels.Height / 2);
                    session.DrawImage(Image, centeredX, centeredY);
               }

               // Draw the hitbox around the distraction for debugging
              // session.DrawRectangle(X, Y, Width, Height, Colors.Green);
          }

        public void ChangeImage(CanvasBitmap newImage)
        {
            Image = newImage;
        }

        public Windows.Foundation.Rect Rect
        {
            get { return new Windows.Foundation.Rect(X, Y, Width, Height); }
        }
        // Intersection check method (simple bounding box collision detection)
        public bool Intersects(Distraction other)
        {
            return X < other.X + other.Image.SizeInPixels.Width &&
                   X + Image.SizeInPixels.Width > other.X &&
                   Y < other.Y + other.Image.SizeInPixels.Height &&
                   Y + Image.SizeInPixels.Height > other.Y;
        }

        /*public bool IsBoom(CanvasBitmap boomImage)
        {
            return Image == boomImage;
        }*/
    }

}
