using CodeNameTwang.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Store;

namespace CodeNameTwang.UWP
{
    public class UWPUtilities : IUtilities
    {
        private bool set = false, set2 = false;
        private bool IsInFocus = true;
        bool LMousePressed = false;
        Tuple<double, double> last = Tuple.Create(0.0, 0.0);

        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        public Tuple<double, double> GetMousePosition()
        {
            if (!set) { 
                Windows.UI.Xaml.Window.Current.Activated += Current_Activated;
                set = !set;
            }
            if (!IsInFocus) return last;

            var pointerPosition = Windows.UI.Core.CoreWindow.GetForCurrentThread().PointerPosition;
            RECT Windowpos = new RECT();
            GetWindowRect(GetForegroundWindow(),out Windowpos);
            var x = pointerPosition.X - Windowpos.Left;
            var y = pointerPosition.Y - Windowpos.Top;
            return last = Tuple.Create(x-8, y-80);
        }

        private void Current_Activated(object sender, Windows.UI.Core.WindowActivatedEventArgs e)
        {
            if (e.WindowActivationState == Windows.UI.Core.CoreWindowActivationState.Deactivated)
            {
                // Show the "paused" UI. 
                IsInFocus = false;
            }
            else if (e.WindowActivationState == Windows.UI.Core.CoreWindowActivationState.PointerActivated || e.WindowActivationState == Windows.UI.Core.CoreWindowActivationState.CodeActivated)
            {
                // Show the "active" UI. 
                IsInFocus = true;
            }
        }

        public bool MouserPressed()
        {
            if (!set2) {
                Windows.UI.Xaml.Window.Current.CoreWindow.PointerPressed += (a,b)=> {
                    LMousePressed = true;
                };
                Windows.UI.Xaml.Window.Current.CoreWindow.PointerReleased += (a, b) => {
                    LMousePressed = false;
                };
                set2 = !set2;
            }
            return LMousePressed;
        }
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int Left;         
        public int Top;          
        public int Right;         
        public int Bottom;        
    }
}
