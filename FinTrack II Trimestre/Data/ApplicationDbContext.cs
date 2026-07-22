using FinTrack_II_Trimestre.Models;
using Microsoft.EntityFrameworkCore;

namespace FinTrack_II_Trimestre.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Income> Incomes { get; set; }
        public DbSet<Expense> Expenses { get; set; }
    }
}
