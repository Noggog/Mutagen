using System.Buffers.Binary;
using Mutagen.Bethesda.Plugins.Binary.Headers;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Meta;
using Noggog;

namespace Mutagen.Bethesda.Plugins.Binary.Overlay;

/// <summary>
/// Provides lazy, thread-safe, one-time decompression of major record data.
/// Header data is always available from original bytes (never compressed).
/// Content data is decompressed on first access if the record is compressed.
/// </summary>
internal sealed class LazyMajorRecordData
{
    private readonly ReadOnlyMemorySlice<byte> _originalSlice;
    private readonly GameConstants _meta;
    private readonly Lazy<byte[]>? _decompressedContent;
    private readonly int _headerLength;
    private readonly int _structDataStart;
    private readonly int _structDataLength;

    public LazyMajorRecordData(ReadOnlyMemorySlice<byte> originalFullRecord, GameConstants meta)
    {
        _originalSlice = originalFullRecord;
        _meta = meta;

        var header = meta.MajorRecordHeader(originalFullRecord);
        _headerLength = header.HeaderLength;
        _structDataStart = meta.MajorConstants.TypeAndLengthLength;
        _structDataLength = _headerLength - _structDataStart;

        if (header.IsCompressed)
        {
            _decompressedContent = new Lazy<byte[]>(
                DecompressContent,
                LazyThreadSafetyMode.ExecutionAndPublication);
        }
    }

    /// <summary>
    /// Header data (flags, FormID, version control, etc.) - always from original bytes.
    /// This is the header portion after the Type and Length bytes.
    /// </summary>
    public ReadOnlyMemorySlice<byte> StructData =>
        _originalSlice.Slice(_structDataStart, _structDataLength);

    /// <summary>
    /// Content data - lazily decompressed if the record is compressed.
    /// </summary>
    public ReadOnlyMemorySlice<byte> RecordData =>
        _decompressedContent != null
            ? _decompressedContent.Value
            : _originalSlice.Slice(_headerLength);

    /// <summary>
    /// Whether this record data represents a compressed record.
    /// </summary>
    public bool IsCompressed => _decompressedContent != null;

    /// <summary>
    /// Returns the original full record slice (header + compressed/uncompressed content).
    /// Used when creating deferred streams for FillSubrecordTypes.
    /// </summary>
    public ReadOnlyMemorySlice<byte> OriginalSlice => _originalSlice;

    private byte[] DecompressContent()
    {
        // Compressed content format: [4 bytes: uncompressed length][compressed data]
        var compressedContent = _originalSlice.Slice(_headerLength);
        var uncompressedLength = BinaryPrimitives.ReadUInt32LittleEndian(compressedContent);
        var compressedData = compressedContent.Slice(4);

        return Decompression.Decompress(compressedData, uncompressedLength);
    }
}
