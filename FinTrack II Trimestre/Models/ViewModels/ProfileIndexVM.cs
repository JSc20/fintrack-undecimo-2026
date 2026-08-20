using System;
using System.Collections.Generic;

namespace FinTrack_II_Trimestre.Models.ViewModels
{
    public class ProfileIndexVM
    {
        // --- Datos del Perfil ---
        public int ProfileId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int Age { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;

        // --- Resumen de Balance (Mes Actual) ---
        public decimal TotalIncomes { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal NetBalance => TotalIncomes - TotalExpenses;

        // --- Transacciones Recientes ---
        public List<RecentTransactionVM> RecentTransactions { get; set; } = new List<RecentTransactionVM>();

        // --- Meta de Ahorro ---
        public string SavingsGoalName { get; set; } = string.Empty;
        public decimal SavingsCurrentAmount { get; set; }
        public decimal SavingsTargetAmount { get; set; }
        public bool HasActiveSavingsGoal => !string.IsNullOrEmpty(SavingsGoalName) && SavingsTargetAmount > 0;

        // --- Plan Activo ---
        public string ActivePlan { get; set; } = "Sin plan";
    }

    public class RecentTransactionVM
    {
        public string Description { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public bool IsIncome { get; set; }
    }
}
