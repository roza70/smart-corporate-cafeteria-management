namespace smart_corporate_cafeteria_management.Services
{
    public class MultipleEmployeeMealService
    {
        public string CheckEmployees()
        {
            bool[] employeeStatus = { true, true, false, true, false };
            bool[] collectedStatus = { false, true, false, false, true };

            string output = "Employee Meal Checking Results\n\n";

            for (int i = 0; i < employeeStatus.Length; i++)
            {
                if (employeeStatus[i] && !collectedStatus[i])
                {
                    output += $"Employee {i + 1}: Meal Allowed\n";
                }
                else
                {
                    output += $"Employee {i + 1}: Meal Denied\n";
                }
            }

            return output;
        }
    }
}