using OptiKnoxAPI.Models;

namespace OptiKnoxAPI.Services
{
    public interface IAzureStorage
    {
        Task<BlobResponseDto> UploadAsync(IFormFile file,string FileName,string FileID);
        Task<BlobDto> DownloadAsync(string blobFilename);
       
        Task<BlobResponseDto> DeleteAsync(string blobFilename);
        Task<List<BlobDto>> ListAsync();
    }

}

