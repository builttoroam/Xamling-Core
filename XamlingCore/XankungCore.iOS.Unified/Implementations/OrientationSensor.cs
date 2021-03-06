﻿using System;
using UIKit;
using XamlingCore.Portable.Contract.Device;
using XamlingCore.Portable.Model.Orientation;

namespace XamlingCore.iOS.Unified.Implementations
{
    public class OrientationSensor : IOrientationSensor
    {
        public event EventHandler OrientationChanged;

        public XPageOrientation Orientation { get; private set; }

        public bool UpsideDown { get; private set; }

        public OrientationSensor()
        {
            _orientationUpdated();
        }

        public void OnRotated()
        {
            if (_orientationUpdated())
            {
                if (OrientationChanged != null)
                {
                    OrientationChanged(this, EventArgs.Empty);
                }
            }
        }

        bool _orientationUpdated()
        {
            
            XPageOrientation _orientation = Orientation;
            if (UIDevice.CurrentDevice.Orientation == UIDeviceOrientation.LandscapeLeft ||
                UIDevice.CurrentDevice.Orientation == UIDeviceOrientation.LandscapeRight)
            {
                _orientation = XPageOrientation.Landscape;
            }
            if (UIDevice.CurrentDevice.Orientation == UIDeviceOrientation.Portrait ||
                UIDevice.CurrentDevice.Orientation == UIDeviceOrientation.PortraitUpsideDown)
            {
                _orientation = XPageOrientation.Portrait;
            }

            UpsideDown = UIDevice.CurrentDevice.Orientation == UIDeviceOrientation.LandscapeRight
                         || UIDevice.CurrentDevice.Orientation == UIDeviceOrientation.PortraitUpsideDown;

            //if (_orientation != Orientation)
           // {
                Orientation = _orientation;
                return true;
           // }
//return false;
        }
    }
}
