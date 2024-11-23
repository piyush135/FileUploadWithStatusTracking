using Amazon;
using Amazon.S3;
using Amazon.S3.Transfer;

namespace API_for_Uploading_Large_Files
{
    public class S3Service
    {
        private readonly IAmazonS3 _s3Client;
        private readonly string _bucketName;

        public S3Service(IConfiguration configuration)
        {
            var awsOptions = configuration.GetSection("AWS");
            var region = awsOptions["Region"];
            _bucketName = awsOptions["BucketName"]; // Add BucketName to appsettings.json

            _s3Client = new AmazonS3Client(
                awsOptions["AccessKey"],
                awsOptions["SecretKey"],
                RegionEndpoint.GetBySystemName(region)
            );
        }

        public async Task<string> UploadFileAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is null or empty.");

            var keyName = Guid.NewGuid() + Path.GetExtension(file.FileName);

            using (var newMemoryStream = new MemoryStream())
            {
                await file.CopyToAsync(newMemoryStream);

                var uploadRequest = new TransferUtilityUploadRequest
                {
                    InputStream = newMemoryStream,
                    Key = keyName,
                    BucketName = _bucketName,
                    ContentType = file.ContentType
                };

                var transferUtility = new TransferUtility(_s3Client);
                await transferUtility.UploadAsync(uploadRequest);
            }

            return keyName; // Return the S3 key for the uploaded file
        }
    }
}
