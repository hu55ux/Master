namespace Master.Infrastructure.Config
{
    public class AwsConfig
    {
        public const string SectionName = "AWS";
        public string AccessKey { get; set; } = string.Empty;
        public string SecretKey { get; set; } = string.Empty;
        public string Region { get; set; } = string.Empty;
        public string BucketName { get; set; } = string.Empty;
    }
}
