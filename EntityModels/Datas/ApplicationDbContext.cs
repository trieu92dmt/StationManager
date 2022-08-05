using EntityModels.EntityModels;
using Microsoft.EntityFrameworkCore;

namespace EntityModels.Datas
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
