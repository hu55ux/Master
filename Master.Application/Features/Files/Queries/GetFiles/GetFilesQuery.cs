using Master.Application.Common;
using Master.Application.Interfaces;
using MediatR;

namespace Master.Application.Features.Files.Queries.GetFiles
{
    public class GetFilesQuery : IRequest<ApiResponse<IEnumerable<FileMetadata>>>
    {
        public string? Prefix { get; set; }
    }

    public class GetFilesHandler : IRequestHandler<GetFilesQuery, ApiResponse<IEnumerable<FileMetadata>>>
    {
        private readonly IFileService _fileService;

        public GetFilesHandler(IFileService fileService)
        {
            _fileService = fileService;
        }

        public async Task<ApiResponse<IEnumerable<FileMetadata>>> Handle(GetFilesQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var files = await _fileService.ListFilesAsync(request.Prefix);
                return ApiResponse<IEnumerable<FileMetadata>>.SuccessResponse(files);
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<FileMetadata>>.ErrorResponse($"Failed to list files: {ex.Message}");
            }
        }
    }
}
