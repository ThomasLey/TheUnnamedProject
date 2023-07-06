using NUnit.Framework;

namespace TheUnnamedService.DocumentStorageService.FileStore.Tests
{
    [TestFixture]
    public class Class1
    {
        [Test]
        public async Task GetBuckets()
        {
            var sut = new MinioRepository("localhost");

            await sut.GetBuckets();
        }
    }
}