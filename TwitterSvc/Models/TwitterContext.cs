using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;
using TwitterSvc.Models;


namespace TwitterSvc.Models
{
    public class TwitterContext : DbContext
    {
        private int count;
        public TwitterContext(DbContextOptions<TwitterContext> options)
            : base(options)
        {
            count = 5;
        }

        public DbSet<TwitterData> DataEntries { get; set; } = null!;

        public DbSet<TwitterSvc.Models.TweetData> TweetData { get; set; }
    }
}
