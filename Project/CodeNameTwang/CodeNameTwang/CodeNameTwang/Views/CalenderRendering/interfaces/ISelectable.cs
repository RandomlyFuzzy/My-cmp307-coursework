using CodeNameTwang.ViewModels.DataStructures;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeNameTwang.Views.CalenderRendering
{
    public interface ISelectable
    {
        bool Inside(float x, float y);

    }
}
