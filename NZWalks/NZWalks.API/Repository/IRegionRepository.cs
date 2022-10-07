using NZWalks.API.Models.Domain;

namespace NZWalks.API.Repository
{
    public interface IRegionRepository
    {
      Task<IEnumerable<Region>>  GetAllAsync();
    }
}
