using Mutagen.Bethesda.Internals;
using Noggog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Mutagen.Bethesda.Binary
{
    /// <summary>
    /// A class that wraps a stream with Mutagen-specific binary reading functionality
    /// </summary>
    public class MutagenBinaryReadStream : BinaryReadStream, IMutagenReadStream
    {
        private readonly string? _path;

        /// <inheritdoc/>
        public long OffsetReference { get; }

        /// <inheritdoc/>
        public ParsingBundle MetaData { get; }

        /// <summary>
        /// Constructor that opens a read stream to a path
        /// </summary>
        /// <param name="path">Path to read from</param>
        /// <param name="metaData">Bundle of all related metadata for parsing</param>
        /// <param name="bufferSize">Size of internal buffer</param>
        /// <param name="offsetReference">Optional offset reference position to use</param>
        public MutagenBinaryReadStream(
            string path,
            ParsingBundle metaData,
            int bufferSize = 4096,
            long offsetReference = 0)
            : base(path, bufferSize)
        {
            this._path = path;
            this.MetaData = metaData;
            this.OffsetReference = offsetReference;
        }

        /// <summary>
        /// Constructor that opens a read stream to a path
        /// </summary>
        /// <param name="path">Path to read from</param>
        /// <param name="release">Game Release the stream is for</param>
        /// <param name="bufferSize">Size of internal buffer</param>
        /// <param name="offsetReference">Optional offset reference position to use</param>
        public MutagenBinaryReadStream(
            ModPath path,
            GameRelease release,
            int bufferSize = 4096,
            long offsetReference = 0)
            : base(path, bufferSize)
        {
            this._path = path;
            this.MetaData = new ParsingBundle(release, MasterReferenceReader.FromPath(path, release))
            {
                ModKey = path.ModKey
            };
            this.OffsetReference = offsetReference;
        }

        /// <summary>
        /// Constructor that wraps an existing stream
        /// </summary>
        /// <param name="stream">Stream to wrap and read from</param>
        /// <param name="metaData">Bundle of all related metadata for parsing</param>
        /// <param name="bufferSize">Size of internal buffer</param>
        /// <param name="dispose">Whether to dispose the source stream</param>
        /// <param name="offsetReference">Optional offset reference position to use</param>
        public MutagenBinaryReadStream(
            Stream stream,
            ParsingBundle metaData,
            int bufferSize = 4096,
            bool dispose = true,
            long offsetReference = 0)
            : base(stream, bufferSize, dispose)
        {
            this.MetaData = metaData;
            this.OffsetReference = offsetReference;
        }

        /// <summary>
        /// Constructor that wraps an existing stream
        /// </summary>
        /// <param name="stream">Stream to wrap and read from</param>
        /// <param name="modKey">ModKey</param>
        /// <param name="release">Game Release the stream is for</param>
        /// <param name="bufferSize">Size of internal buffer</param>
        /// <param name="dispose">Whether to dispose the source stream</param>
        /// <param name="offsetReference">Optional offset reference position to use</param>
        public MutagenBinaryReadStream(
            Stream stream,
            ModKey modKey,
            GameRelease release,
            int bufferSize = 4096,
            bool dispose = true,
            long offsetReference = 0)
            : base(stream, bufferSize, dispose)
        {
            var startPos = stream.Position;
            this.MetaData = new ParsingBundle(release, MasterReferenceReader.FromStream(stream, modKey, release))
            {
                ModKey = modKey
            };
            stream.Position = startPos;
            this.OffsetReference = offsetReference;
        }

        /// <summary>
        /// Reads an amount of bytes into an internal array and returns a new stream wrapping those bytes.
        /// OffsetReference is updated to be aligned to the original source starting position.
        /// This call will advance the source stream by the number of bytes.
        /// The returned stream will be ready to read and start at its Position 0.
        /// </summary>
        /// <param name="length">Number of bytes to read and reframe</param>
        /// <returns>A new stream wrapping an internal array, set to position 0.</returns>
        public IMutagenReadStream ReadAndReframe(int length)
        {
            var offset = this.OffsetReference + this.Position;
            return new MutagenMemoryReadStream(
                this.ReadMemory(length, readSafe: true),
                this.MetaData, 
                offsetReference: offset);
        }

        public override string ToString()
        {
            return $"{(_path == null ? " " : $"{_path}: ")}{this._stream.Position}-{this._stream.Length} ({this._stream.Remaining()})";
        }
    }
}
