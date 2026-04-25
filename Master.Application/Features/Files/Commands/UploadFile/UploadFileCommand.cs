using Master.Application.Common;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Master.Application.Features.Files.Commands.UploadFile
{
    public class UploadFileCommand : IRequest<ApiResponse<UploadFileResponse>>
    {
        public IFormFile File { get; set; } = null!;
        public string FolderName { get; set; } = "uploads";
    }

    public class UploadFileResponse
    {
        public string Url { get; set; } = string.Empty;
    }
}
