using Microsoft.EntityFrameworkCore;

namespace BasicPdfWebApp.Models
{
    public class FileContext : DbContext
    {
        public FileContext(DbContextOptions<FileContext> options)
            :base(options)
        {

        }

        public DbSet<FileItemEntity> FileItems{ get; set; }

        public DbSet<FileItemDTO> FileItemDTO { get; set; }
    }
}
