using HotelListing.API.Data;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;

namespace HotelListing.API.Contracts
{
    public interface ICountriesRepository : IGenericRepository<Country>
    {
        Task<Country> GetDetails(int id);
    }
}
