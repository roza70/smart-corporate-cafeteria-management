namespace smart_corporate_cafeteria_management.Services
{
    public class MealCalculationService
    {
        public decimal CalculateIngredient(int employeeCount, decimal quantityPerEmployee)
        {
            decimal totalQuantity = employeeCount * quantityPerEmployee;

            return totalQuantity;
        }
    }
}