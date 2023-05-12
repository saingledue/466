using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace ABABI.Models
{
    public class ABABIDBContext:DbContext
    {
        public ABABIDBContext(DbContextOptions<ABABIDBContext> options) : base(options)
        {

        }
        public DbSet<Avatar> Avatars { get; set; } = null!;
        public DbSet<MessageDataModel> Messages { get; set; } = null!;
        public DbSet<UserDataModel> Users { get; set; } = null!;
        public DbSet<Post> Posts { get; set; } = null!;
    }
}
