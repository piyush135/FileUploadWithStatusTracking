using Amazon.S3;
using Amazon.S3.Model;
using log4net;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;
using System.Reflection;

namespace API_for_Uploading_Large_Files.Controllers
{
    public class FileUploadTrackController : Controller
    {
        private static ConcurrentDictionary<Guid, string> _fileStatus = new ConcurrentDictionary<Guid, string>();

        private readonly IConfiguration _configuration;
        private readonly IAmazonS3 _s3Client;
        private readonly string _bucketName;
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public FileUploadTrackController(IConfiguration configuration, IAmazonS3 s3Client)
        {
            _configuration = configuration;
            _s3Client = s3Client;
            _bucketName = _configuration["AWS:BucketName"]; // Retrieve the S3 Bucket Name from configuration
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile uploadedFile)
        {
            if (uploadedFile == null || uploadedFile.Length == 0)
            {
                return BadRequest("No file selected or the file is empty.");
            }

            // Generate a unique ID for tracking
            var processingId = Guid.NewGuid();

            // Add initial status to ConcurrentDictionary
            _fileStatus.TryAdd(processingId, "Pending");

            // Background processing
            _ = Task.Run(async () =>
            {
                try
                {
                    _fileStatus[processingId] = "Processing"; // Update status to processing

                    // Generate a unique file key
                    var fileKey = $"{Guid.NewGuid()}{Path.GetExtension(uploadedFile.FileName)}";

                    // Create upload request
                    using (var stream = uploadedFile.OpenReadStream())
                    {
                        var uploadRequest = new PutObjectRequest
                        {
                            BucketName = _bucketName,
                            Key = fileKey,
                            InputStream = stream,
                            ContentType = uploadedFile.ContentType                          
                        };

                        var response = await _s3Client.PutObjectAsync(uploadRequest);

                        if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
                        {
                            // Update status to completed
                            _fileStatus[processingId] = "Completed";
                        }
                        else
                        {
                            // Update status to failed
                            _fileStatus[processingId] = "Failed";
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error("File Upload with track error " + ex.ToString());
                    // Log exception and set status to failed
                    //_logger.LogError(ex, "Error during file upload for ID: {ProcessingId}", processingId);
                    _fileStatus[processingId] = "Failed";
                }
            });

            // Return HTTP 202 with processing ID
            return Accepted(new { ProcessingId = processingId });
        }

        // GET: Track the Status of File Processing
        [HttpGet("status/{processingId}")]
        public IActionResult TrackStatus(Guid processingId)
        {
            // Check if the processing ID exists
            if (!_fileStatus.ContainsKey(processingId))
            {
                return NotFound("Processing ID not found.");
            }

            // Return the current status of the processing
            return Ok(new { ProcessingId = processingId, Status = _fileStatus[processingId] });
        }
    }
}
