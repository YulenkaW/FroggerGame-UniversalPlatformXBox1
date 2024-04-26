using Microsoft.Graphics.Canvas;
using System.Diagnostics;
using Windows.Foundation;
using Windows.UI;

namespace Frogger
{
    public class Rectangle : IDrawable
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public Color color;
        public CanvasBitmap image { get; set; }

        public Rectangle (int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
          
        }


        public void SetImage(CanvasBitmap newImage)
        {
            image = newImage;
            Width = (int)image.SizeInPixels.Width;  // Update width to match image width
            Height = (int)image.SizeInPixels.Height;  // Update height to match image height
        }


        public void Draw(CanvasDrawingSession session)
        {
            // Draw the hitbox for debugging
            // session.DrawRectangle(X, Y, Width, Height, color);

            if (image != null)
            {
                // Calculate the centered position
                float imageX = X + (Width / 2) - (image.SizeInPixels.Width / 2);
                float imageY = Y + (Height / 2) - (image.SizeInPixels.Height / 2);

                // Draw the image centered in the hitbox
                session.DrawImage(image, imageX, imageY);
            }
        }

        public bool Intersects(Distraction distraction)
        {
            int Padding = 5;
            var frogLeft = X + Padding;
            var frogRight = X + Width - Padding;
            var frogTop = Y + Padding;
            var frogBottom = Y + Height - Padding;

            var distractionLeft = distraction.X + Padding;
            var distractionRight = distraction.X + distraction.Width - Padding;
            var distractionTop = distraction.Y + Padding;
            var distractionBottom = distraction.Y + distraction.Height - Padding;

            bool intersects = frogLeft < distractionRight &&
                              frogRight > distractionLeft &&
                              frogTop < distractionBottom &&
                              frogBottom > distractionTop;

            if (intersects)
            {
                    //Use for debugging
                //Debug.WriteLine($"Collision detected: Frog({frogLeft}, {frogTop}, {frogRight}, {frogBottom}) " +
                               // $"Distraction({distractionLeft}, {distractionTop}, {distractionRight}, {distractionBottom})");
            }

            return intersects;
        }

        public Rect ToRect()
        {
            return new Rect(X, Y, Width, Height);
        }
    }
}


/*using Microsoft.Graphics.Canvas;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;

namespace Frogger
{
    internal class Rectangle : IDrawable
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public Color color;
        public CanvasBitmap image {  get; set; }

          public void Draw(CanvasDrawingSession session)
          {
               // Draw the hitbox for debugging (you might want to remove this in the final game)
               //session.DrawRectangle(X, Y, Width, Height, color);

               if (image != null)
               {
                    // Calculate the centered position
                    float imageX = X + (Width / 2) - (image.SizeInPixels.Width / 2);
                    float imageY = Y + (Height / 2) - (image.SizeInPixels.Height / 2);

                    // Draw the image centered in the hitbox
                    session.DrawImage(image, imageX, imageY);
               }
          }


          // This method detects collisions with a distraction
          public bool Intersects(Distraction distraction)
          {
               int Padding = 5;
               var frogLeft = X + Padding;
               var frogRight = X + Width - Padding;
               var frogTop = Y + Padding;
               var frogBottom = Y + Height - Padding;

               var distractionLeft = distraction.X + Padding;
               var distractionRight = distraction.X + distraction.Image.Bounds.Width - Padding;
               var distractionTop = distraction.Y + Padding;
               var distractionBottom = distraction.Y + distraction.Image.Bounds.Height - Padding;

               bool intersects = frogLeft < distractionRight &&
                                 frogRight > distractionLeft &&
                                 frogTop < distractionBottom &&
                                 frogBottom > distractionTop;



            if (intersects)
               {
                    Debug.WriteLine($"Collision detected: Frog({frogLeft}, {frogTop}, {frogRight}, {frogBottom}) " +
                                    $"Distraction({distractionLeft}, {distractionTop}, {distractionRight}, {distractionBottom})");
               }

               return intersects;
          }
        public Rect ToRect()
    {
        return new Rect(X, Y, Width, Height);
    }

     }
}
*/