using FluentAssertions;
using NUnit.Framework;

namespace TheUnnamedProject.Core.Tests
{
    [TestFixture]
    public class TheRepositoryTests
    {
        private readonly string _testPath = "c:\\Workspace\\_UnnamedTestEnsureFolder";

        [Test]
        [Explicit]
        public void EnsureFolder()
        {
            if (Directory.Exists(_testPath)) Directory.Delete(_testPath, true);
            var sut = new TheRepository(_testPath);
            sut.EnsureStore();
            sut.GetDocumentTypes().Count().Should().Be(4);
            sut.GetFilemaps().Count().Should().Be(3);
        }
    }
}