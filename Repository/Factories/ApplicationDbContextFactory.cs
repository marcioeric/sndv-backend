using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Repository.Factories
{
    public class ApplicationContextFactory : IDesignTimeDbContextFactory<ApplicationContext>

    {
        public ApplicationContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationContext>();
            optionsBuilder.UseNpgsql("User ID=postgres;Password=17122019;Host=localhost;Database=sndv;");
            return new ApplicationContext(optionsBuilder.Options);
        }
    }
}