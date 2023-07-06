using System.Diagnostics;
using Minio;

namespace TheUnnamedService.DocumentStorageService.FileStore.Minio;

public class MinioRepository
{
    private readonly string _endpoint;

    public MinioRepository(string endpoint)
    {
        _endpoint = endpoint ?? throw new ArgumentNullException(nameof(endpoint));
    }

    public async Task GetBuckets()
    {
        var accessKey = "T4bWHCIEDKd7AQFdNlRQ";
        var secretKey = "XWZtaSfidUCpj0vnogepeQJ1NgbrUQUL3hmEG7Se";
        var secure = false;

        // Initialize the client with access credentials.
        MinioClient minio = new MinioClient()
            .WithEndpoint(_endpoint, 9000)
            .WithCredentials(accessKey, secretKey)
            //.WithSSL(secure)
            .Build();

        if (!minio.BucketExistsAsync(new BucketExistsArgs().WithBucket("int-inbox")).Result)
            await minio.MakeBucketAsync(new MakeBucketArgs().WithBucket("int-inbox")).ConfigureAwait(false);

        // Create an async task for listing buckets.
        var getListBucketsTask = await minio.ListBucketsAsync().ConfigureAwait(false);

        // Iterate over the list of buckets.
        foreach (var bucket in getListBucketsTask.Buckets)
        {
            Console.WriteLine(bucket.Name + " " + bucket.CreationDateDateTime);
        }
    }
}
