using DTOs.WeighSession;
using Microsoft.EntityFrameworkCore;
using WeighSession.API.Repositories.Interfaces;
using WeighSession.Infrastructure.Models;

namespace WeighSession.API.Repositories
{
    public class WeighSessionRepository : IWeighSessionRepository
    {
        private readonly DataCollectionContext _context;

        public WeighSessionRepository(DataCollectionContext context)
        {
            _context = context;
        }
        public async Task<List<WeighSessionResponse>> GetWeighSessionAsync(string keyWord, string plantCode, string type)
        {
            var result = await _context.WeighSessionModel.Where(x => true).Select(x => new WeighSessionResponse
            {
                Key = x.DateKey,
                Value = x.ScaleCode
            }).ToListAsync();

            return result;
        }
    }
}
