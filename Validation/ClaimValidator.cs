using FluentValidation;
using CMCS.Models;

namespace CMCS.Validation
{
    // This class is used to validate the claim details of the user. The ClaimID is the primary key and UserID is the foreign key. The ClaimAmount is the amount of the claim, ClaimType is the type of the claim, ClaimStatus is the status of the claim, SubmissionDate is the date when the claim was submitted and ApprovalDate is the date when the claim was approved.

    public class ClaimValidator : AbstractValidator<ClaimModel>
    {
        public ClaimValidator()
        {
            RuleFor(x => x.HoursWorked).GreaterThan(0).WithMessage("Hours must be greater than 0").LessThanOrEqualTo(40).WithMessage("Hours cannot exceed 40");
            RuleFor(x => x.HourlyRate).InclusiveBetween(150, 350).WithMessage("Hourly rate must be between R150 and R350");
            RuleFor(x => x.SubmissionDate).LessThanOrEqualTo(DateTime.Now).WithMessage("Submission date cannot be in the future");
        }
    }
}