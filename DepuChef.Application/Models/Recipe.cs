﻿namespace DepuChef.Application.Models;

public class Recipe : AuditedModel
{
    public Guid Id { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? PrepTime { get; set; }
    public string? CookTime { get; set; }
    public string? TotalTime { get; set; }
    public int Servings { get; set; }
    public List<Ingredient>? Ingredients { get; set; }
    public List<Instruction>? Instructions { get; set; }
    public List<Note>? Notes { get; set; }
    public decimal Confidence { get; set; }
    public int? Rating { get; set; }
}