﻿using Mutagen.Bethesda.Binary;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    namespace Internals
    {
        public partial class APerkEffectBinaryOverlay
        {
            public byte Rank => throw new NotImplementedException();

            public byte Priority => throw new NotImplementedException();

            public IReadOnlyList<IPerkConditionGetter> Conditions => throw new NotImplementedException();
        }
    }
}
