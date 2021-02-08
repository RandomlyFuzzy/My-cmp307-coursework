using System;
using System.Collections.Generic;
using System.Text;

namespace CodeNameTwang.Services.Interfaces
{
    public interface IUtilities
    {
        Tuple<double, double> GetMousePosition();
        bool MouserPressed();
    }
}
