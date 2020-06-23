﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    public partial class EnableParent
    {
        [Flags]
        public enum Flag
        {
            SetEnableStateToOppositeOfParent = 0x01,
            PopIn = 0x02,
        }
    }
}
