using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeNameTwang.Views.CalenderRendering
{
    interface IDrawable
    {
        void Draw(SKCanvas canvas, SKImageInfo info);

    }
}
