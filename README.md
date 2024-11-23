# File Upload with Tracking using ASP.NET Core Razor Pages and Amazon S3

## Description
This project demonstrates how to implement a file upload system in an ASP.NET Core Razor Pages application, where files are uploaded to Amazon S3. It also includes tracking for each uploaded file, such as tracking upload status and metadata.

### Key Features:
- **File Upload**: Upload files to Amazon S3 directly from the application.
- **File Tracking**: Track metadata of each uploaded file, including file size, upload timestamp, and status.
- **ASP.NET Core Razor Pages**: The UI is built with Razor Pages, providing a clean and efficient way to handle file uploads.
- **Amazon S3 Integration**: Upload files to a specified S3 bucket for secure cloud storage.
- **File Metadata**: Track and store file metadata in the database.

## Getting Started

### Prerequisites
Before running the project, ensure that the following tools and services are installed and configured:
- **.NET 6+** (or latest version)
- **Amazon Web Services (AWS) Account**: For S3 access.
- **AWS SDK for .NET**: The project relies on the AWS SDK to interact with Amazon S3.
- **Database**: SQL Server (or preferred database for storing file metadata).

### Installation

1. **Clone the Repository:**
   ```bash
   git clone https://github.com/yourusername/file-upload-s3-tracking.git

2. Install Dependencies: Ensure you have the necessary NuGet packages installed:

AWSSDK.S3 (AWS SDK for S3).
Microsoft.EntityFrameworkCore (for database integration).
Microsoft.AspNetCore.RazorPages (for Razor Pages functionality).
Microsoft.Extensions.Configuration (for reading configuration files).
Run the following command to restore NuGet packages:

3. Configure AWS S3: Set up AWS S3 access by providing your AWS Access Key and Secret Key in your appsettings.json or through environment variables.

4. Add the following to your appsettings.json:

{
  "AWS": {
    "AccessKey": "<your-access-key>",
    "SecretKey": "<your-secret-key>",
    "BucketName": "<your-s3-bucket-name>",
    "Region": "<your-s3-region>"
  }
}   
