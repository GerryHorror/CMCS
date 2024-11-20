// Reusable function for showing overlays. This function is used to show success and error messages to the user.
const showActionOverlay = (overlayId, iconClass, iconColor, message, duration = 3000) => {
    // Get the overlay, icon and text elements from the DOM using the overlayId parameter
    const overlay = document.getElementById(overlayId);
    const icon = overlay.querySelector('i');
    const text = overlay.querySelector('p');
    // Set the icon class, color and text content to the values passed in as parameters to the function (i.e . iconClass, iconColor and message)
    icon.className = iconClass;
    icon.style.color = iconColor;
    text.textContent = message;
    // Display the overlay by setting its display style to 'flex' (i.e. show the overlay as a flex container)
    overlay.style.display = 'flex';
    setTimeout(() => {
        overlay.style.display = 'none';
    }, duration);
};

// <------------------------------------------------------------------------------------------------------------------------------------------------------------>

// Submit view functionality - handles form submission, adding work entries, and uploading supporting documents
const initialiseSubmitView = () => {
    const submitForm = document.querySelector('.submit-form');
    if (submitForm) {
        const hourlyRateInput = document.getElementById('HourlyRate');
        const claimAmountInput = document.getElementById('ClaimAmount');
        const addEntryButton = document.getElementById('addEntry');
        const workEntriesContainer = document.getElementById('workEntries');
        const supportingDocumentInput = document.getElementById('supportingDocument');
        let entryCount = 1;

        // Function to format currency amounts using the ZAR currency
        const formatCurrency = (amount) => {
            // Format number to 2 decimal places with ZAR currency
            return new Intl.NumberFormat('en-ZA', {
                style: 'currency',
                currency: 'ZAR',
                minimumFractionDigits: 2,
                maximumFractionDigits: 2
            }).format(amount);
        };

        // Function to calculate the claim amount based on the hourly rate and total hours worked
        const calculateClaimAmount = () => {
            const hourlyRate = parseFloat(hourlyRateInput.value) || 0;
            let totalHours = 0;
            document.querySelectorAll('.work-hours').forEach(input => {
                totalHours += parseFloat(input.value) || 0;
            });
            const totalAmount = totalHours * hourlyRate;

            // Update hidden input with raw number for form submission
            claimAmountInput.value = totalAmount.toFixed(2);

            // Update display input with formatted currency
            const displayElement = document.getElementById('displayClaimAmount');
            if (displayElement) {
                // Remove currency symbol from formatted value since we already show R in the input group
                const formattedValue = formatCurrency(totalAmount).replace('ZAR', '').trim();
                displayElement.value = formattedValue;
            }
        };

        const addNewWorkEntry = () => {
            const newEntry = `
                <div class="work-entry">
                    <div class="form-group">
                        <label class="submit-label">Work Date:</label>
                        <input type="date" name="WorkEntries[${entryCount}].WorkDate" class="submit-input work-date" required>
                    </div>
                    <div class="form-group">
                        <label class="submit-label">Hours Worked:</label>
                        <input type="number" name="WorkEntries[${entryCount}].HoursWorked" class="submit-input work-hours" step="0.5" min="1" max="8" required>
                    </div>
                    <button type="button" class="btn btn-danger remove-entry">Remove</button>
                </div>
            `;
            workEntriesContainer.insertAdjacentHTML('beforeend', newEntry);
            entryCount++;
        };

        // Function to remove a work entry when the 'Remove' button is clicked
        const removeWorkEntry = (event) => {
            if (event.target.classList.contains('remove-entry')) {
                event.target.closest('.work-entry').remove();
                renumberEntries();
                calculateClaimAmount();
            }
        };

        // Function to renumber the work entries when one is removed
        const renumberEntries = () => {
            document.querySelectorAll('.work-entry').forEach((entry, index) => {
                entry.querySelector('.work-date').name = `WorkEntries[${index}].WorkDate`;
                entry.querySelector('.work-hours').name = `WorkEntries[${index}].HoursWorked`;
            });
            entryCount = document.querySelectorAll('.work-entry').length;
        };

        // Event listeners for the form elements
        hourlyRateInput.addEventListener('input', calculateClaimAmount);
        workEntriesContainer.addEventListener('input', (event) => {
            if (event.target.classList.contains('work-hours')) {
                calculateClaimAmount();
            }
        });
        workEntriesContainer.addEventListener('click', removeWorkEntry);
        addEntryButton.addEventListener('click', addNewWorkEntry);

        // Function to validate work entries
        function validateWorkEntry(hours, date) {
            const errors = [];
            if (hours < 1 || hours > 8) {
                errors.push('Hours worked must be between 1 and 8.');
            }
            if (new Date(date) > new Date()) {
                errors.push('Work date cannot be in the future.');
            }
            return errors;
        }

        // Add validation to work entries before submitting the form
        workEntriesContainer.addEventListener('change', (event) => {
            if (event.target.classList.contains('work-hours') || event.target.classList.contains('work-date')) {
                const entry = event.target.closest('.work-entry');
                const hours = parseFloat(entry.querySelector('.work-hours').value);
                const date = entry.querySelector('.work-date').value;
                const errors = validateWorkEntry(hours, date);

                // Display error messages if there are any
                const errorDiv = entry.querySelector('.error-messages') || document.createElement('div');
                errorDiv.className = 'error-messages text-danger';
                errorDiv.textContent = errors.join(', ');
                if (!entry.querySelector('.error-messages')) {
                    entry.appendChild(errorDiv);
                }
            }
        });

        submitForm.addEventListener('submit', async (event) => {
            event.preventDefault();

            // File validation
            const supportingDocument = supportingDocumentInput.files[0];
            if (!supportingDocument) {
                showActionOverlay('successOverlay', 'fas fa-exclamation-circle', 'var(--color-error)', 'Please upload a supporting document before submitting.');
                return;
            }

            // Check file size (5MB limit)
            if (supportingDocument.size > 5 * 1024 * 1024) {
                showActionOverlay('successOverlay', 'fas fa-exclamation-circle', 'var(--color-error)', 'File size exceeds 5MB limit.');
                return;
            }

            // Check file type
            const allowedExtensions = ['.pdf', '.docx', '.xlsx'];
            const fileExtension = supportingDocument.name.split('.').pop().toLowerCase();
            if (!allowedExtensions.includes('.' + fileExtension)) {
                showActionOverlay('successOverlay', 'fas fa-exclamation-circle', 'var(--color-error)', 'Only .pdf, .docx, and .xlsx files are allowed.');
                return;
            }

            const formData = new FormData(submitForm);
            try {
                const response = await fetch(submitForm.action, {
                    method: 'POST',
                    body: formData
                });
                const result = await response.json();
                if (result.success) {
                    showActionOverlay('successOverlay', 'fas fa-check-circle', 'var(--color-success)', 'Claim successfully submitted!');
                    submitForm.reset();
                    // Reset work entries to initial state
                    workEntriesContainer.innerHTML = '';
                    addNewWorkEntry();
                } else {
                    let errorMessage = result.message;
                    if (result.errors && result.errors.length > 0) {
                        errorMessage += ': ' + result.errors.join(', ');
                    }
                    showActionOverlay('successOverlay', 'fas fa-exclamation-circle', 'var(--color-error)', errorMessage);
                }
            } catch (error) {
                console.error('Error:', error);
                showActionOverlay('successOverlay', 'fas fa-exclamation-circle', 'var(--color-error)', 'An error occurred. Please try again.');
            }
        });
    }
};

document.addEventListener('DOMContentLoaded', () => {
    initialiseSubmitView();
});