using System.Collections.Generic;

namespace FinTrack_II_Trimestre.Models.ViewModels
{
    public class CategoryIndexViewModel
    {
        public DashboardStatsViewModel Stats { get; set; } = new DashboardStatsViewModel();
        public List<CategoryExpenseRow> Categorias { get; set; } = new List<CategoryExpenseRow>();
    }

    public class CategoryExpenseRow
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = "";
        public decimal CategoryPercentage { get; set; }
        public bool CategoryStatus { get; set; }
        public decimal TotalEgresos { get; set; } // suma de Expense.ExpenseAmount de esa categoria
    }
}
