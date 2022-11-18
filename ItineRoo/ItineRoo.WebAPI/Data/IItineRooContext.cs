using ItineRoo.WebAPI.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace ItineRoo.WebAPI.Data
{
    public interface IItineRooContext
    {
        DbSet<User> Users { get; }
    }
}