// Reusable function for showing overlays. This function is used to show success and error messages to the user.
const showActionOverlay = (overlayId, iconClass, iconColor, message, duration = 3000) => {
    // Get the overlay, icon and text elements from the DOM using the overlayId parameter
    const overlay = document.getElementById(overlayId);
    const icon = overlay.querySelector('i');
    const text = overlay.querySelector('p');
    // Set the icon class, color and text content to the values passed in as parameters to the function
    icon.className = iconClass;
    icon.style.color = iconColor;
    text.textContent = message;
    // Display the overlay by setting its display style to 'flex' (i.e. show the overlay as a flex container)
    overlay.style.display = 'flex';
    setTimeout(() => {
        overlay.style.display = 'none';
    }, duration);
};

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

        // Get today's date and two months ago for date restrictions
        const today = new Date();
        const twoMonthsAgo = new Date();
        twoMonthsAgo.setMonth(twoMonthsAgo.getMonth() - 2);

        const calculateClaimAmount = () => {
            const hourlyRate = parseFloat(hourlyRateInput.value) || 0;
            let totalHours = 0;
            document.querySelectorAll('.work-hours').forEach(input => {
                totalHours += parseFloat(input.value) || 0;
            });
            const totalAmount = totalHours * hourlyRate;
            claimAmountInput.value = totalAmount.toFixed(2);
        };

        const addNewWorkEntry = () => {
            const newEntry = `
                <div class="work-entry">
                    <div class="form-group">
                        <label class="submit-label" title="Please select a date within the last two months and no later than today">Work Date:</label>
                        <input type="date" name="WorkEntries[${entryCount}].WorkDate" class="submit-input work-date" min="${twoMonthsAgo.toISOString().split('T')[0]}" max="${today.toISOString().split('T')[0]}" required>
                    </div>
                    <div class="form-group">
                        <label class="submit-label" title="Enter hours between 1 and 8">Hours Worked:</label>
                        <input type="number" name="WorkEntries[${entryCount}].HoursWorked" class="submit-input work-hours" min="0.5" max="8" step="0.5" required oninput="this.value = this.value < 0 ? 0 : this.value">
                    </div>
                    <button type="button" class="btn btn-danger remove-entry">Remove</button>
                </div>
            `;
            workEntriesContainer.insertAdjacentHTML('beforeend', newEntry);
            entryCount++;
        };

        const removeWorkEntry = (event) => {
            if (event.target.classList.contains('remove-entry')) {
                const entries = document.querySelectorAll('.work-entry');
                // Prevent removing the last entry
                if (entries.length > 1) {
                    event.target.closest('.work-entry').remove();
                    renumberEntries();
                    calculateClaimAmount();
                } else {
                    showActionOverlay('successOverlay', 'fas fa-exclamation-circle', 'var(--color-error)', 'At least one work entry is required');
                }
            }
        };

        const renumberEntries = () => {
            document.querySelectorAll('.work-entry').forEach((entry, index) => {
                const dateInput = entry.querySelector('.work-date');
                const hoursInput = entry.querySelector('.work-hours');

                dateInput.name = `WorkEntries[${index}].WorkDate`;
                hoursInput.name = `WorkEntries[${index}].HoursWorked`;

                // Ensure date restrictions are maintained
                dateInput.min = twoMonthsAgo.toISOString().split('T')[0];
                dateInput.max = today.toISOString().split('T')[0];
            });
            entryCount = document.querySelectorAll('.work-entry').length;
        };

        // Add validation to prevent users from manually entering dates outside allowed range
        const validateDateInput = (input) => {
            const dateValue = new Date(input.value);
            if (dateValue < twoMonthsAgo || dateValue > today) {
                input.value = "";
                showActionOverlay('successOverlay', 'fas fa-exclamation-circle', 'var(--color-error)', 'Please select a date within the last two months and no later than today.');
            }
        };

        hourlyRateInput.addEventListener('input', calculateClaimAmount);
        workEntriesContainer.addEventListener('input', (event) => {
            if (event.target.classList.contains('work-hours')) {
                calculateClaimAmount();
            }
            if (event.target.classList.contains('work-date')) {
                validateDateInput(event.target);
            }
        });
        workEntriesContainer.addEventListener('click', removeWorkEntry);
        addEntryButton.addEventListener('click', addNewWorkEntry);

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