/*
    Student Name: Gérard Blankenberg
    Student Number: ST10046280
    Module: PROG6212
    POE Part 2
*/

// This class will be used to create the WorkEntry object. This object will be used to store the work entry details. The work entry details will be stored in the database and will be used to track the work entries of the user. The work entry details will have the following columns:
//  - WorkDate: This will be the date of the work entry.
//  - HoursWorked: This will be the number of hours worked on that day.

using System.ComponentModel.DataAnnotations;

namespace CMCS.Models
{
    public class WorkEntry
    {
        [Required(ErrorMessage = "Work date is required")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [PastOrPresentDate(ErrorMessage = "Work date cannot be after the submission date.")]
        public DateTime WorkDate { get; set; }

        [Required(ErrorMessage = "Hours worked is required")]
        [Range(1, 8, ErrorMessage = "Hours worked must be between 1 and 8")]
        public decimal HoursWorked { get; set; }
    }

    public class PastOrPresentDateAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            DateTime date = (DateTime)value;
            if (date > DateTime.Today)
            {
                return new ValidationResult(ErrorMessage ?? "Date cannot be after the submission date.");
            }
            return ValidationResult.Success;
        }
    }
}