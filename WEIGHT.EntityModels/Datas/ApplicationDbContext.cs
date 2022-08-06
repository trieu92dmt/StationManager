using Microsoft.EntityFrameworkCore;
using WEIGHT.EntityModels.Models;

namespace WEIGHT.EntityModels.Datas
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext()
        {

        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<WeightDetailModel> WeightDetailModel { get; set; }
    }
}
