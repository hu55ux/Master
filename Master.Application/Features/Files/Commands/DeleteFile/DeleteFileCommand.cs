using Master.Application.Common;
using Master.Application.Interfaces;
using MediatR;

namespace Master.Application.Features.Files.Commands.DeleteFile
{
    public class DeleteFileCommand : IRequest<ApiResponse<bool>>
    {
        public string FileUrl { get; set; } = null!;
    }

    public class DeleteFileHandler : IRequestHandler<DeleteFileCommand, ApiResponse<bool>>
    {
        private readonly IFileService _fileService;

        public DeleteFileHandler(IFileService fileService)
        {
            _fileService = fileService;
        }

        public async Task<ApiResponse<bool>> Handle(DeleteFileCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _fileService.DeleteFileAsync(request.FileUrl);
                return ApiResponse<bool>.SuccessResponse(true, "File deleted successfully.");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse($"Failed to delete file: {ex.Message}");
            }
        }
    }
}
