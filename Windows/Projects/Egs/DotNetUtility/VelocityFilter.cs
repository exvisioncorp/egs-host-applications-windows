namespace DotNetUtility
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Diagnostics;
    using System.Runtime.Serialization;
    using System.ComponentModel;

    class VelocityFilter
    {
        public double FramesPerSecond { get; set; }
        public double XDotEmaCoefficient { get; set; }
        public double X { get; private set; }
        public double XDot { get; private set; }
        double XPrevious;
        double XDotPrevious;

        public VelocityFilter()
        {
            FramesPerSecond = 100.0;
            XDotEmaCoefficient = 1.0;
            X = 0;
            XDot = 0;
            XPrevious = 0;
            XDotPrevious = 0;
        }

        public void Initialize(double initialX)
        {
            X = initialX;
            XDot = 0;
            XPrevious = initialX;
            XDotPrevious = 0;
        }

        public void Update(double newX)
        {
            XPrevious = X;
            XDotPrevious = XDot;
            X = newX;
            var measuredXDot = (X - XPrevious) * FramesPerSecond;
            XDot = XDotEmaCoefficient * measuredXDot + (1.0 - XDotEmaCoefficient) * XDotPrevious;
        }
    }
}
