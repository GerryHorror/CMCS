using FluentValidation;
using CMCS.Models;

namespace CMCS.Validation
{
    public class ClaimValidator : AbstractValidator<ClaimModel>
    {
        public ClaimValidator()
        {
            RuleFor(x => x.HoursWorked).GreaterThan(0).WithMessage("Hours must be greater than 0").LessThanOrEqualTo(40).WithMessage("Hours cannot exceed 40");
            RuleFor(x => x.HourlyRate).InclusiveBetween(150, 350).WithMessage("Hourly rate must be between R150 and R350");
            //RuleFor(x => x.ClaimAmount).Must((claim, amount) => amount == claim.HoursWorked * claim.HourlyRate).WithMessage("Claim amount does not match hours × rate");
            RuleFor(x => x.SubmissionDate).LessThanOrEqualTo(DateTime.Now).WithMessage("Submission date cannot be in the future");
        }
    }
}