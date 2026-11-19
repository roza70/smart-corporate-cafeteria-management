namespace smart_corporate_cafeteria_management.Models
{
    public class Recipe
    {
        public int RecipeId { get; set; }

        public string RecipeName { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public int ServingSize { get; set; }

        public string PreparationInstructions { get; set; } = string.Empty;
    }
}