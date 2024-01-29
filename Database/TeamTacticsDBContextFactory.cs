using Microsoft.EntityFrameworkCore;

namespace Team_Tactics_Backend.Database
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
