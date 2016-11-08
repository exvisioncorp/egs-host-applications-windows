namespace Egs.ZkooTutorial
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.ComponentModel;

    partial class TutorialLargeCircleAreaButtonModel : ImageButtonModel
    {
        public TutorialLargeCircleAreaButtonModel()
            : base()
        {
            _Index = 0;
            _IsThumbDragged = false;
            _TapsCount = 0;
            _LongTapsCount = 0;
        }

        public void Reset()
        {
            IsEnabled = true;
            IsHovered = false;
            IsPressed = false;
            IsThumbDragged = false;
            TapsCount = 0;
            LongTapsCount = 0;
        }
    }
}
