namespace smart_corporate_cafeteria_management.Services
{
    public class EmployeeIdentificationService
    {
        public bool CanIdentifyEmployee(bool hasEmployeeID, bool hasQRCode)
        {
            if (hasEmployeeID || hasQRCode)
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