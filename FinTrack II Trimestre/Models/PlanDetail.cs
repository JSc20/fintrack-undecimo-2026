using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinTrack_II_Trimestre.Models
{
    public class PlanDetail
    {
        [Key]
        public int DetailId { get; set; }

        public int PlanId { get; set; }

        [ForeignKey(nameof(PlanId))]    
        public virtual BudgetPlan? BudgetPlan { get; set; } 

        public int CategoryId { get; set; }

        [ForeignKey(nameof(CategoryId))]
        public virtual Category? Category { get; set; }
    }
}