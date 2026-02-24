using System.Runtime.CompilerServices;

namespace Mutagen.Bethesda.Plugins.Binary.Overlay;

internal sealed class LazyPayload<T>(Lazy<bool> init, T fields) where T : class
{
    /// <summary>
    /// Direct field access for use during initialization (inside Lazy callback).
    /// Do not use for reads outside init context — use <see cref="Value"/> instead.
    /// </summary>
    internal T Fields => fields;

    /// <summary>
    /// Triggers lazy initialization if needed, then returns the payload fields.
    /// Use this for all property getters.
    /// </summary>
    public T Value
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get { _ = init.Value; return fields; }
    }
}
