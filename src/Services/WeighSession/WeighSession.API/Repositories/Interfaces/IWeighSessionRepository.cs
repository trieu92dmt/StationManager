using DTOs.WeighSession;

namespace WeighSession.API.Repositories.Interfaces
{
    public interface IWeighSessionRepository
    {
        Task<List<WeighSessionResponse>> GetWeighSessionAsync(string keyWord, string plantCode, string type);
    }
}
