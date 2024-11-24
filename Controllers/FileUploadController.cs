using Amazon.S3;
using Amazon.S3.Model;
using log4net;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;
using System.Reflection;

namespace API_for_Uploading_Large_Files.Controllers
{   
    public class FileUploadController : Controller
    {
        private static ConcurrentDictionary<Guid, string> _fileStatus = new ConcurrentDictionary<Guid, string>();
        private readonly S3Service _s3Service;
        private readonly string _bucketName;
        private readonly IConfiguration _configuration;
        private readonly IAmazonS3 _s3Client;
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public FileUploadController(S3Service s3Service, IConfiguration configuration, IAmazonS3 s3Client)
        {
            _configuration = configuration;
            _s3Service = s3Service;
            _s3Client = s3Client;
        }
        
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile UploadedFile)
        {
            if (UploadedFile == null || UploadedFile.Length == 0)
            {
                return BadRequest("No file selected or the file is empty.");
            }

            try
            {
                var s3Key = await _s3Service.UploadFileAsync(UploadedFile);
                return Json(new { message = "File uploaded successfully!", key = s3Key });
            }
            catch (Exception ex)
            {
                Logger.Error("Error while uploading the file in s3" + ex.ToString());
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }

            //if (UploadedFile == null || UploadedFile.Length == 0)
            //{
            //    return BadRequest("No file selected or the file is empty.");
            //}

            //try
            //{
            //    // Save the file to the "Uploads" directory
            //    var uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
            //    if (!Directory.Exists(uploadsDir))
            //        Directory.CreateDirectory(uploadsDir);

            //    var filePath = Path.Combine(uploadsDir, UploadedFile.FileName);

            //    using (var fileStream = new FileStream(filePath, FileMode.Create))
            //    {
            //        await UploadedFile.CopyToAsync(fileStream);
            //    }

            //    return Json(new { message = "File uploaded successfully!" });
            //}
            //catch (Exception ex)
            //{
            //    return StatusCode(500, $"Internal server error: {ex.Message}");
            //}
        }       

    }
}
