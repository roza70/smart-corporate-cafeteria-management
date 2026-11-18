namespace smart_corporate_cafeteria_management.Services
{
    public class InventoryService
    {
        public string CheckStock()
        {
            int stockQuantity = 3;
            int minimumStock = 5;

            string output = "Inventory Stock Report\n\n";

            while (stockQuantity < minimumStock)
            {
                output += $"Low Stock Alert! Current Stock: {stockQuantity}\n";

                stockQuantity++;
            }

            output += $"\nFinal Stock Quantity: {stockQuantity}";

            return output;
        }
    }
}