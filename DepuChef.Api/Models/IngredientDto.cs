namespace DepuChef.Api.Models;

public class IngredientDto
{
    public string? Category { get; set; }
    public List<IngredientItemDto>? Items { get; set; }
    public int? Calories { get; set; }
    public List<HealthySubstitutionDto>? HealthySubstitutions { get; set; }
    public int? CaloriesAfterSubstitution { get; set; }
}

public class IngredientItemDto
{
    public string? Name { get; set; }
    public int? Calories { get; set; }
}

public class HealthySubstitutionDto
{
    public string? Original { get; set; }
    public string? Substitute { get; set; }
}