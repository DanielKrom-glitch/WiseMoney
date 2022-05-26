using Microsoft.EntityFrameworkCore;
using WiseMoney.Models;

namespace WiseMoney.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<Conta> Conta { get; set; }
        public DbSet<Cliente> Cliente { get; set; }
        public DbSet<Operacao> Operacao { get; set; }

    }
}
