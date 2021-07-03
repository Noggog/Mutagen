﻿using System.IO;
using FluentAssertions;
using Mutagen.Bethesda.Environments;
using Mutagen.Bethesda.Plugins.Order;
using Noggog;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Order
{
    public class CreationClubPahContextTests
    {
        [Fact]
        public void NotUsed()
        {
            new CreationClubPathContext(
                    new GameCategoryInjection(GameCategory.Skyrim),
                    new CreationClubEnabledInjection(false),
                    new DataDirectoryInjection("C:/SomeFolder"))
                .Path
                .Should().BeNull();
        }

        [Fact]
        public void Typical()
        {
            var dataDir = "C:/SomeFolder";
            foreach (var category in EnumExt.GetValues<GameCategory>())
            {
                new CreationClubPathContext(
                        new GameCategoryInjection(category),
                        new CreationClubEnabledInjection(true),
                        new DataDirectoryInjection(dataDir))
                    .Path
                    .Should().Be(new FilePath($"C:/{category}.ccc"));
            }
        }
    }
}