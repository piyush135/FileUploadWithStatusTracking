using Amazon.S3.Model;
using Amazon.S3;
using log4net.Appender;
using log4net.Core;
using Amazon;

namespace API_for_Uploading_Large_Files.Logging
{
    public class S3Appender : AppenderSkeleton
    {
        // AWS S3 Configuration Properties
        public string AWSAccessKey { get; set; }
        public string AWSSecretKey { get; set; }
        public string BucketName { get; set; }
        public string Region { get; set; }
        public string LogFilePrefix { get; set; } = "logs/";        
        private string _logFilePathInS3 = "logs/application-log.txt";

        private IAmazonS3 _s3Client;

        private readonly IConfiguration _configuration;

        public S3Appender()
        {
            //_configuration = configuration;
            //_s3Client = s3Client;
            //AWSAccessKey = _configuration["AWS:AccessKey"];
            //AWSSecretKey = _configuration["AWS:SecretKey"];
            //BucketName = _configuration["AWS:BucketName"];
            //Region = _configuration["AWS:Region"];
        }

        protected override void Append(LoggingEvent loggingEvent)
        {
            try
            {
                if (_s3Client == null)
                {
                    InitializeS3Client();
                }

                string logMessage = RenderLoggingEvent(loggingEvent);
                string objectKey = $"{LogFilePrefix}{DateTime.UtcNow:yyyy-MM-dd_HH-mm-ss-fff}.log";

                // Upload log to S3
                using (var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(logMessage)))
                {
                    var request = new PutObjectRequest
                    {
                        BucketName = BucketName,
                        Key = _logFilePathInS3,  // Single log file in S3
                        InputStream = stream,
                        ContentType = "text/plain",
                        //CannedACL = S3CannedACL.PublicReadWrite // Optional, specify permissions as needed
                    };

                    _s3Client.PutObjectAsync(request).Wait();
                }
            }
            catch (Exception ex)
            {
                // Handle errors in logging (optional)
                Console.Error.WriteLine($"S3Appender Error: {ex.Message}");
            }
        }

        private void InitializeS3Client()
        {
            if (string.IsNullOrEmpty(AWSAccessKey) || string.IsNullOrEmpty(AWSSecretKey))
            {
                throw new ArgumentException("AWSAccessKey and AWSSecretKey must be provided.");
            }

            var regionEndpoint = RegionEndpoint.GetBySystemName(Region ?? "us-east-1");

            _s3Client = new AmazonS3Client(AWSAccessKey, AWSSecretKey, regionEndpoint);
        }

        protected override void OnClose()
        {
            _s3Client?.Dispose();
            base.OnClose();
        }
    }
}
