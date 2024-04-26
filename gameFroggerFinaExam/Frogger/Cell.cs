using Microsoft.Graphics.Canvas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frogger
{
     public class Cell : IDrawable
     {
          public Windows.Foundation.Rect Position { get; private set; }
          public CanvasBitmap Image { get; private set; }
          private CanvasBitmap originalImage;
          private CanvasBitmap frogInCellImage;

          public Cell(Windows.Foundation.Rect position, CanvasBitmap image, CanvasBitmap frogInCellImage)
          {
               Position = position;
               this.originalImage = image;
               this.frogInCellImage = frogInCellImage;
               Image = originalImage;
          }

          public void Draw(CanvasDrawingSession session)
          {
               session.DrawImage(Image, Position);
          }

          public bool Contains(Rectangle frog)
          {
               var frogRect = frog.ToRect();
               // We use boundary checking since Rect does not have an IntersectsWith method in UWP
               return frogRect.Left < Position.Right && frogRect.Right > Position.Left &&
                      frogRect.Top < Position.Bottom && frogRect.Bottom > Position.Top;
          }

          public void MarkAsCompleted()
          {
               Image = frogInCellImage;
          }

          public void Reset()
          {
               Image = originalImage;
          }
     }

}
