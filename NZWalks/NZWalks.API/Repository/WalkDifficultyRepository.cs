using Microsoft.EntityFrameworkCore;
using NZWalks.API.Data;
using NZWalks.API.Models.Domain;

namespace NZWalks.API.Repository
{
    public class WalkDifficultyRepository : IWalkDifficultyRepository
    {
        private readonly NZWalksDbContext nZWalksDbContext;
        public WalkDifficultyRepository(NZWalksDbContext nZWalksDbContext)
        {
            this.nZWalksDbContext = nZWalksDbContext;
        }

        public async Task<WalkDifficulty> AddAsync(WalkDifficulty walkDifficulty)
        {
           walkDifficulty.Id = Guid.NewGuid();
          await nZWalksDbContext.AddAsync(walkDifficulty);
            nZWalksDbContext.SaveChangesAsync();
            return walkDifficulty;
        }

        public async Task<WalkDifficulty> DeleteAsync(Guid id)
        {
            var existingWalkDifficulty= await nZWalksDbContext.WalkDifficulties.FindAsync(id);

            if(existingWalkDifficulty == null)
            {
                return null;
            }

            nZWalksDbContext.WalkDifficulties.Remove(existingWalkDifficulty);
            await nZWalksDbContext.SaveChangesAsync();

            return existingWalkDifficulty;
        }

        public async Task<IEnumerable<WalkDifficulty>> GetAllAsync()
        {
            return await nZWalksDbContext.WalkDifficulties.ToListAsync();
        }

        public Task<WalkDifficulty> GetAsync(Guid id)
        {
            return nZWalksDbContext.WalkDifficulties.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<WalkDifficulty> UpdateAsync(Guid id, WalkDifficulty walkDifficulty)
        {
          var existingWalkDifficulty= await nZWalksDbContext.WalkDifficulties.FindAsync(id);

            if (existingWalkDifficulty != null)
            {
                existingWalkDifficulty.Code = walkDifficulty.Code;
                await nZWalksDbContext.SaveChangesAsync();
                return existingWalkDifficulty;
            }

            return null;
        }
    }
}
