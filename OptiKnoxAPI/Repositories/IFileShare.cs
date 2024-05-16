using OptiKnoxAPI.Models;

namespace OptiKnoxAPI.Repositories
{
    public interface IFileShare
    {
        Task FileUploadAsync(FileDetails fileDetails);
        Task FileDownloadAsync(string fileShareName);
    }
}
