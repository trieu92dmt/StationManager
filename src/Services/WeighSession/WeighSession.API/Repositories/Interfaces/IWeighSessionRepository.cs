using DTOs.WeighSession;

namespace WeighSession.API.Repositories.Interfaces
{
    public interface IWeighSessionRepository
    {
        Task<List<WeightHeadResponse>> GetWeightHeadAsync(string keyWord, string plantCode, string type);
    }
}
