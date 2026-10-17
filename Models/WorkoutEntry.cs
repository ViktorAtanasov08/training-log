using System.ComponentModel.DataAnnotations;

namespace TrainingLog.Models;

public class WorkoutEntry
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Enter an exercise name.")]
    [StringLength(80)]
    public string Exercise { get; set; } = string.Empty;

    [DataType(DataType.Date)]
    public DateTime Date { get; set; } = DateTime.Today;

    [Range(1, 20, ErrorMessage = "Sets must be between 1 and 20.")]
    public int Sets { get; set; } = 3;

    [Range(1, 100, ErrorMessage = "Reps must be between 1 and 100.")]
    public int Reps { get; set; } = 8;

    [Display(Name = "Weight (kg)")]
    [Range(typeof(decimal), "0", "1000", ErrorMessage = "Weight must be between 0 and 1000 kg.")]
    public decimal WeightKg { get; set; }
}
