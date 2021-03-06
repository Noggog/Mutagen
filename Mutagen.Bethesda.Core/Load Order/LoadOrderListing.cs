using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Mutagen.Bethesda
{
    [DebuggerDisplay("LoadOrderListing {ToString()}")]
    public record LoadOrderListing
    {
        public ModKey ModKey { get; init; }
        public bool Enabled { get; init; }

        public LoadOrderListing()
        {
        }

        public LoadOrderListing(ModKey modKey, bool enabled)
        {
            ModKey = modKey;
            Enabled = enabled;
        }

        public override string ToString()
        {
            return $"[{(Enabled ? "X" : "_")}] {ModKey}";
        }

        public static implicit operator LoadOrderListing(ModKey modKey)
        {
            return new LoadOrderListing(modKey, enabled: true);
        }

        public static LoadOrderListing FromString(ReadOnlySpan<char> str, bool enabledMarkerProcessing)
        {
            str = str.Trim();
            bool enabled = true;
            if (enabledMarkerProcessing)
            {
                if (str[0] == '*')
                {
                    str = str[1..];
                }
                else
                {
                    enabled = false;
                }
            }
            if (!ModKey.TryFromNameAndExtension(str, out var key))
            {
                throw new ArgumentException($"Load order file had malformed line: {str.ToString()}");
            }
            return new LoadOrderListing(key, enabled);
        }

        public static Comparer<LoadOrderListing> GetComparer(Comparer<ModKey> comparer) =>
            Comparer<LoadOrderListing>.Create((x, y) => comparer.Compare(x.ModKey, y.ModKey));
    }
}
