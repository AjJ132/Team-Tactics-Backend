using Microsoft.EntityFrameworkCore;

namespace TeamTacticsBackend.Database
{
    public class TeamTacticsDBContextFactory : IDbContextFactory<TeamTacticsDBContext>
    {
        private readonly DbContextOptions<TeamTacticsDBContext> _options;

        public TeamTacticsDBContextFactory(DbContextOptions<TeamTacticsDBContext> options)
        {
            _options = options;
        }

        public TeamTacticsDBContext CreateDbContext()
        {
            return new TeamTacticsDBContext(_options);
        }
    }
}
