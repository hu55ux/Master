using Master.Application.Common;
using Master.Application.Interfaces;
using MediatR;

namespace Master.Application.Features.Files.Commands.UploadFile
{
    public class UploadFileHandler : IRequestHandler<UploadFileCommand, ApiResponse<UploadFileResponse>>
    {
        private readonly IFileService _fileService;

        public UploadFileHandler(IFileService fileService)
        {
            _fileService = fileService;
        }

        public async Task<ApiResponse<UploadFileResponse>> Handle(UploadFileCommand request, CancellationToken cancellationToken)
        {
            if (request.File == null || request.File.Length == 0)
            {
                return ApiResponse<UploadFileResponse>.ErrorResponse("No file uploaded or file is empty.");
            }

            try
            {
                var fileUrl = await _fileService.UploadFileAsync(request.File, request.FolderName);
                
                var response = new UploadFileResponse
                {
                    Url = fileUrl
                };

                return ApiResponse<UploadFileResponse>.SuccessResponse(response, "File uploaded successfully.");
            }
            catch (Exception ex)
            {
                return ApiResponse<UploadFileResponse>.ErrorResponse($"File upload failed: {ex.Message}");
            }
        }
    }
}
