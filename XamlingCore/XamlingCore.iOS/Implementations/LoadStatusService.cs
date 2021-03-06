using System.Diagnostics;
using MonoTouch.UIKit;
using Xamarin.Forms;
using XamlingCore.iOS.Controls.Loader;
using XamlingCore.iOS.Root;
using XamlingCore.Portable.Contract.UI;
using XamlingCore.Portable.View.Special;
using XamlingCore.XamarinThings.Contract;

namespace XamlingCore.iOS.Implementations
{
    public class LoadStatusService : LoadStatusServiceBase
    {
        private LoadingOverlayViewBase _spinnerInstance;

        public LoadStatusService(IDispatcher dispatcher) : base(dispatcher)
        {
        }

        public override void ShowIndicator(string text)
        {
            if (!string.IsNullOrWhiteSpace(text))
            {
                _hideTray();
                _showFullScreen(text);
            }
            else
            {
                //don't show the tray loader if the full screen spinner is up
                if (_spinnerInstance == null)
                {
                    _hideFullScreen();
                    _showTray();    
                }
            }
        }

        public override void HideIndicator()
        {
            _hideFullScreen();
            _hideTray();
        }

        void _showFullScreen(string text)
        {
            if (_spinnerInstance != null)
            {
                _spinnerInstance.Text = text;
                return;
            }

            _spinnerInstance = new LoadingOverlayView(UIScreen.MainScreen.Bounds);

            _spinnerInstance.Text = text;

            XiOSRoot.RootWindow.AddSubview(_spinnerInstance);
        }

        void _hideFullScreen()
        {
            if (_spinnerInstance != null)
            {
               _spinnerInstance.RemoveFromSuperview();
            }

            _spinnerInstance = null;
        }

        void _hideTray()
        {
            UIApplication.SharedApplication.NetworkActivityIndicatorVisible = false;
        }

        void _showTray()
        {
            UIApplication.SharedApplication.NetworkActivityIndicatorVisible = true;
        }

        
    }
}