using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace SEWebApp.Models
{
    public class fundDBContext:DbContext
    {
        public fundDBContext(DbContextOptions<fundDBContext> options) : base(options)
        {

        }
        public DbSet<Avatar> Avatars { get; set; } = null!;
        public DbSet<MessageDataModel> Messages { get; set; } = null!;
        public DbSet<UserDataModel> Users { get; set; } = null!;
    }
}
