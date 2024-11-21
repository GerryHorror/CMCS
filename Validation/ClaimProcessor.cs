using CMCS.Data;
using CMCS.Models;
using Microsoft.EntityFrameworkCore;

namespace CMCS.Validation
{
    public class ClaimProcessor
    {
        // Properties for the ClaimValidator and CMCSDbContext

        private readonly ClaimValidator _validator;
        private readonly CMCSDbContext _context;

        // Constructor to initialise the ClaimProcessor with the ClaimValidator and CMCSDbContext
        public ClaimProcessor(ClaimValidator validator, CMCSDbContext context)
        {
            _validator = validator;
            _context = context;
        }

        // Method to process the claim asynchronously and return a tuple with the success status, message, and auto-approval status
        public async Task<(bool Success, string Message, bool AutoApproved)> ProcessClaimAsync(ClaimModel claim)
        {
            // Validate the claim using the ClaimValidator
            var validationResult = await _validator.ValidateAsync(claim);

            // If the claim is not valid, return an error message
            if (!validationResult.IsValid)
            {
                return (false, string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)), false);
            }

            // Check for supporting documents
            var hasDocuments = await _context.Documents
                .AnyAsync(d => d.ClaimID == claim.ClaimID);

            if (!hasDocuments)
            {
                return (false, "Supporting documents are required for claim processing", false);
            }

            // Auto-approve criteria
            bool isStandardRate = claim.HourlyRate >= 150 && claim.HourlyRate <= 250;
            bool isRoutineHours = claim.HoursWorked <= 20;
            bool isLowRiskAmount = claim.ClaimAmount <= 5000;

            if (isStandardRate && isRoutineHours && isLowRiskAmount)
            {
                var approvedStatus = await _context.ClaimStatuses
                    .FirstOrDefaultAsync(s => s.StatusName == "Approved");

                claim.StatusID = approvedStatus.StatusID;
                claim.ApprovalDate = DateTime.Now;

                await _context.SaveChangesAsync();
                return (true, "Claim automatically approved - Standard rates", true);
            }

            // Build detailed message for manual review
            var reasons = new List<string>();
            if (!isStandardRate) reasons.Add("Non-standard rate");
            if (!isRoutineHours) reasons.Add("Extended hours");
            if (!isLowRiskAmount) reasons.Add("High claim amount");

            return (true, $"Claim requires manual review: {string.Join(", ", reasons)}", false);
        }
    }
}