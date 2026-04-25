using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Master.Application.Interfaces;
using Master.Infrastructure.Config;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Master.Infrastructure.Services
{
    public class S3Service : IFileService
    {
        private readonly AwsConfig _awsConfig;
        private readonly AmazonS3Client _s3Client;

        public S3Service(IOptions<AwsConfig> awsConfig)
        {
            _awsConfig = awsConfig.Value;

            var config = new AmazonS3Config
            {
                RegionEndpoint = RegionEndpoint.GetBySystemName(_awsConfig.Region)
            };

            _s3Client = new AmazonS3Client(_awsConfig.AccessKey, _awsConfig.SecretKey, config);
        }

        /// <summary>
        /// Uploads a file to S3 and returns its public URL.
        /// </summary>
        public async Task<string> UploadFileAsync(IFormFile file, string folderName = "uploads")
        {
            var fileExtension = Path.GetExtension(file.FileName);
            var fileName = $"{Guid.NewGuid()}{fileExtension}";
            var key = string.IsNullOrEmpty(folderName) ? fileName : $"{folderName}/{fileName}";

            using var stream = file.OpenReadStream();

            var uploadRequest = new TransferUtilityUploadRequest
            {
                InputStream = stream,
                Key = key,
                BucketName = _awsConfig.BucketName
            };

            var transferUtility = new TransferUtility(_s3Client);
            await transferUtility.UploadAsync(uploadRequest);

            return $"https://{_awsConfig.BucketName}.s3.{_awsConfig.Region}.amazonaws.com/{key}";
        }

        /// <summary>
        /// Deletes a file from S3 by its full URL.
        /// </summary>
        public async Task DeleteFileAsync(string fileUrl)
        {
            var uri = new Uri(fileUrl);
            var key = uri.AbsolutePath.TrimStart('/');

            await _s3Client.DeleteObjectAsync(_awsConfig.BucketName, key);
        }

        /// <summary>
        /// Lists files in S3, optionally filtered by folder prefix.
        /// </summary>
        public async Task<IEnumerable<FileMetadata>> ListFilesAsync(string? prefix = null)
        {
            var request = new ListObjectsV2Request
            {
                BucketName = _awsConfig.BucketName,
                Prefix = prefix
            };

            var response = await _s3Client.ListObjectsV2Async(request);

            var files = new List<FileMetadata>();
            foreach (var obj in response.S3Objects)
            {
                files.Add(new FileMetadata
                {
                    Key = obj.Key ?? string.Empty,
                    Size = (long)obj.Size,
                    LastModified = (DateTime)obj.LastModified,
                    Url = $"https://{_awsConfig.BucketName}.s3.{_awsConfig.Region}.amazonaws.com/{obj.Key}"
                });
            }

            return files;
        }
    }
}
