namespace smart_corporate_cafeteria_management.Services
{
    public class MealEligibilityService
    {
        public bool CanCollectMeal(bool isEmployeeActive, bool hasCollectedLunch)
        {
            if (isEmployeeActive && !hasCollectedLunch)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}