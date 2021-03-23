﻿using System;
using System.IO;
using System.Text;
using Mutagen.Bethesda.Pex.DataTypes;
using Mutagen.Bethesda.Pex.Exceptions;
using Mutagen.Bethesda.Pex.Interfaces;

namespace Mutagen.Bethesda.Pex
{
    public static class PexParser
    {
        /// <summary>
        /// Read a .pex file.
        /// </summary>
        /// <param name="file">Path to the file.</param>
        /// <param name="isBigEndian">Whether the file uses LE (F04) or BE (everything else).</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">The file does not exist.</exception>
        /// <exception cref="PexParsingException">Exception while parsing the file.</exception>
        public static IPexFile ParsePexFile(string file, bool isBigEndian)
        {
            if (!File.Exists(file))
                throw new ArgumentException($"Input file does not exist {file}!", nameof(file));
            
            using var fs = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.Read);
            using var br = new PexReader(fs, Encoding.UTF8, isBigEndian);

            //https://en.uesp.net/wiki/Skyrim_Mod:Compiled_Script_File_Format
            var pexFile = new PexFile(br);

            if (fs.Position != fs.Length)
                throw new PexParsingException("Finished reading but end of the stream was not reached! " +
                                              $"Current position: {fs.Position} " +
                                              $"Stream length: {fs.Length} " +
                                              $"Missing: {fs.Length - fs.Position}");
            
            return pexFile;
        }

        /// <summary>
        /// Write to a .pex file.
        /// </summary>
        /// <param name="pexFile">The pex file to write.</param>
        /// <param name="outputPath">Output path.</param>
        /// <param name="isBigEndian">Whether to write LE (F04) or BE (everything else).</param>
        public static void WritePexFile(this IPexFile pexFile, string outputPath, bool isBigEndian)
        {
            var dirName = Path.GetDirectoryName(outputPath);
            Directory.CreateDirectory(dirName ?? string.Empty);
            
            if (File.Exists(outputPath))
                File.Delete(outputPath);

            using var fs = File.Open(outputPath, FileMode.Create, FileAccess.ReadWrite, FileShare.Read);
            using var bw = new PexWriter(fs, Encoding.UTF8, isBigEndian);
            
            pexFile.Write(bw);
        }
    }
}
