﻿using System.Linq;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using Vostok.Logging.File.Helpers;
using Vostok.Logging.File.Rolling.Strategies;
using Vostok.Logging.File.Rolling.Suffixes;

namespace Vostok.Logging.File.Tests.Rolling.Strategies
{
    [TestFixture]
    internal class RollingStrategyHelper_Tests
    {
        private IFileSuffixFormatter<int> suffixFormatter;
        private IFileSystem fileSystem;

        [SetUp]
        public void TestSetup()
        {
            fileSystem = Substitute.For<IFileSystem>();
            fileSystem.GetFilesByPrefix("logs/log.txt").Returns(new FilePath[] {"logs/log1.txt"});

            suffixFormatter = Substitute.For<IFileSuffixFormatter<int>>();
            suffixFormatter.TryParseSuffix("1").Returns(1);
            suffixFormatter.TryParseSuffix("2").Returns(2);
            suffixFormatter.TryParseSuffix("3").Returns(3);
        }

        [Test]
        public void Should_parse_suffix_using_provided_formatter()
        {
            RollingStrategyHelper.DiscoverExistingFiles("logs/log.txt", fileSystem, suffixFormatter)
                .Single().suffix.Should().Be(1);
        }

        [Test]
        public void Should_not_return_paths_where_suffix_could_not_be_parsed()
        {
            fileSystem.GetFilesByPrefix("logs/log.txt").Returns(new FilePath[] {"logs/log1.txt", "logs/log~.txt"});

            RollingStrategyHelper.DiscoverExistingFiles("logs/log.txt", fileSystem, suffixFormatter).Should().HaveCount(1);
        }

        [Test]
        public void Should_return_files_in_correct_order()
        {
            fileSystem.GetFilesByPrefix("logs/log.txt").Returns(new FilePath[] { "logs/log3.txt", "logs/log1.txt", "logs/log2.txt" });

            RollingStrategyHelper.DiscoverExistingFiles("logs/log.txt", fileSystem, suffixFormatter).Select(e => e.path)
                .Should().Equal("logs/log1.txt", "logs/log2.txt", "logs/log3.txt");
        }
    }
}