using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Skyrim.Internals;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Mutagen.Bethesda.UnitTests
{
    public class Gendered_Tests
    {
        /// <summary>
        /// Tests that a head marker with a gendered marker that is empty doesn't throw,
        /// but instead acts as if it didn't exist
        /// </summary>
        [Fact]
        public void RaceHeadPart_DanglingMarker()
        {
            byte[] bytes = new byte[]
            {
                0x4E, 0x41, 0x4D, 0x30, 0x00, 0x00,
                0x4D, 0x4E, 0x41, 0x4D, 0x00, 0x00, 0x4D, 0x50, 0x41, 0x49, 0x04, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x4D, 0x50, 0x41, 0x56, 0x20, 0x00, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
                0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
                0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x4D, 0x50, 0x41, 0x49, 0x04, 0x00,
                0x01, 0x00, 0x00, 0x00, 0x4D, 0x50, 0x41, 0x56, 0x20, 0x00, 0xFF, 0xFF, 0x1F, 0x00, 0xFF,
                0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
                0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x4D, 0x50, 0x41,
                0x49, 0x04, 0x00, 0x02, 0x00, 0x00, 0x00, 0x4D, 0x50, 0x41, 0x56, 0x20, 0x00, 0xFF, 0xFF,
                0xFF, 0xFF, 0x7F, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
                0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
                0x4D, 0x50, 0x41, 0x49, 0x04, 0x00, 0x03, 0x00, 0x00, 0x00, 0x4D, 0x50, 0x41, 0x56, 0x20,
                0x00, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
                0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
                0xFF, 0xFF, 0xFF, 0x46, 0x54, 0x53, 0x4D, 0x04, 0x00, 0x59, 0x32, 0x07, 0x00, 0x46, 0x54,
                0x53, 0x4D, 0x04, 0x00, 0x5A, 0x32, 0x07, 0x00, 0x46, 0x54, 0x53, 0x4D, 0x04, 0x00, 0x5B,
                0x32, 0x07, 0x00, 0x4E, 0x41, 0x4D, 0x30, 0x00, 0x00, 0x46, 0x4E, 0x41, 0x4D, 0x00, 0x00
            };
            var frame = new MutagenFrame(
                new MutagenMemoryReadStream(
                    bytes, 
                    new ParsingBundle(
                        GameRelease.SkyrimSE, 
                        new Internals.MasterReferenceReader(Skyrim.Constants.Skyrim))));
            var headData = Mutagen.Bethesda.Binary.GenderedItemBinaryTranslation.ParseMarkerPerItem<HeadData>(
                frame: frame,
                maleMarker: RecordTypes.MNAM,
                femaleMarker: RecordTypes.FNAM,
                marker: RecordTypes.NAM0,
                femaleRecordConverter: Race_Registration.HeadDataFemaleConverter,
                transl: HeadData.TryCreateFromBinary);
        }
    }
}
