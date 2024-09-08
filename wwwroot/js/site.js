// Claim view functionality

// This function initialises the claim view functionality. It filters the claims based on the search input and status filter.
function initialiseClaimView() {
    const searchInput = document.getElementById('claimSearch');
    const statusFilter = document.getElementById('claimStatus');
    const claimRows = document.querySelectorAll('.claim-row');
    const modal = document.getElementById('claimModal');
    const modalContent = document.getElementById('claimDetails');
    const closeBtn = document.querySelector('.claim-modal-close');
    // Check if the elements exist before adding event listeners (e.g. the search input and status filter are not present on the claim details page)
    if (searchInput && statusFilter) {
        searchInput.addEventListener('input', filterClaims);
        statusFilter.addEventListener('change', filterClaims);
    }
    // Filter the claims based on the search input and status filter
    function filterClaims() {
        const searchTerm = searchInput.value.toLowerCase();
        const statusTerm = statusFilter.value.toLowerCase();
        // This loops through each claim row and checks if the search term and status term match the row's text content and status attribute.
        claimRows.forEach(row => {
            const rowText = row.textContent.toLowerCase();
            const rowStatus = row.getAttribute('data-status').toLowerCase();
            const matchesSearch = rowText.includes(searchTerm);
            const matchesStatus = statusTerm === '' || rowStatus === statusTerm;
            row.style.display = matchesSearch && matchesStatus ? '' : 'none';
        });
    }
    // This adds a click event listener to each claim details button to display the claim details in a modal. It helps enhance the user experience by providing more information about the claim.
    document.querySelectorAll('.claim-details-btn').forEach(btn => {
        btn.addEventListener('click', function () {
            const row = this.closest('tr');
            const details = `
                <p><strong>Claim ID:</strong> ${row.cells[0].textContent}</p>
                <p><strong>Date:</strong> ${row.cells[1].textContent}</p>
                <p><strong>Amount:</strong> ${row.cells[2].textContent}</p>
                <p><strong>Status:</strong> ${row.cells[3].textContent}</p>
            `;
            modalContent.innerHTML = details;
            modal.style.display = 'block';
        });
    });
    // This adds a click event listener to the close button and the modal itself to close the modal when the user clicks outside the modal.
    if (closeBtn) {
        closeBtn.onclick = function () {
            modal.style.display = 'none';
        }
    }
    // Close the modal when the user clicks outside the modal
    window.onclick = function (event) {
        if (event.target == modal) {
            modal.style.display = 'none';
        }
    }
}

// Call the function when the DOM is fully loaded. This is needed because the script is loaded in the head.
document.addEventListener('DOMContentLoaded', function () {
    initialiseClaimView();
});

/* ********************************************************************************************************************************************************************** */

// Submit view functionality

// This function initialises the submit view functionality. It calculates the claim amount based on the hours worked and hourly rate.
document.addEventListener('DOMContentLoaded', function () {
    const submitForm = document.querySelector('.submit-form');
    if (submitForm) {
        const hourlyRateInput = document.getElementById('hourlyRate');
        const claimAmountInput = document.getElementById('claimAmount');
        const addEntryButton = document.getElementById('addEntry');
        const workEntriesContainer = document.getElementById('workEntries');
        const supportingDocumentInput = document.getElementById('supportingDocument');
        const successMessage = document.getElementById('successMessage');

        let entryCount = 1;

        function calculateClaimAmount() {
            const hourlyRate = parseFloat(hourlyRateInput.value) || 0;
            let totalHours = 0;

            document.querySelectorAll('.work-hours').forEach(input => {
                totalHours += parseFloat(input.value) || 0;
            });

            const totalAmount = totalHours * hourlyRate;
            claimAmountInput.value = totalAmount.toFixed(2);
        }

        function addNewWorkEntry() {
            const newEntry = `
                <div class="work-entry">
                    <div class="form-group">
                        <label class="submit-label">Work Date:</label>
                        <input type="date" name="WorkEntries[${entryCount}].WorkDate" class="submit-input work-date" required>
                    </div>
                    <div class="form-group">
                        <label class="submit-label">Hours Worked:</label>
                        <input type="number" name="WorkEntries[${entryCount}].HoursWorked" class="submit-input work-hours" step="0.5" min="0" required>
                    </div>
                </div>
            `;
            workEntriesContainer.insertAdjacentHTML('beforeend', newEntry);
            entryCount++;
        }

        hourlyRateInput.addEventListener('input', calculateClaimAmount);
        workEntriesContainer.addEventListener('input', function (event) {
            if (event.target.classList.contains('work-hours')) {
                calculateClaimAmount();
            }
        });

        addEntryButton.addEventListener('click', addNewWorkEntry);

        submitForm.addEventListener('submit', function (event) {
            event.preventDefault();
            if (!supportingDocumentInput.files.length) {
                alert('Please upload a supporting document before submitting.');
                return;
            }
            const successOverlay = document.getElementById('successOverlay');
            successOverlay.style.display = 'flex';
            submitForm.reset();
            setTimeout(() => {
                successOverlay.style.display = 'none';
            }, 3000);
        });
    }
});

/* ********************************************************************************************************************************************************************** */

// User Profile Functionality

// This function initialises the user profile functionality. It displays a modal message when the user updates their profile.
function initialiseUserProfile() {
    var form = document.getElementById('userProfileForm');
    var overlay = document.getElementById('userActionOverlay');
    var overlayText = document.getElementById('userActionText');

    if (form && overlay) {
        form.onsubmit = function (e) {
            e.preventDefault();
            showUserActionOverlay('Profile updated successfully!');
        };
    } else {
        console.error('One or more required elements for User Profile are missing from the page.');
    }

    function showUserActionOverlay(message) {
        overlayText.textContent = message;
        overlay.style.display = 'flex';
        document.body.style.overflow = 'hidden'; // Prevent scrolling when overlay is open

        setTimeout(() => {
            overlay.style.display = 'none';
            document.body.style.overflow = ''; // Restore scrolling
        }, 3000);
    }
}

// Call the initialisation function when the DOM is fully loaded
document.addEventListener('DOMContentLoaded', function () {
    // Check if we're on the User Profile page
    if (document.getElementById('userProfileForm')) {
        initialiseUserProfile();
    }
});

/* ********************************************************************************************************************************************************************** */

// New function for Verify view functionality

function initialiseVerifyView() {
    const searchInput = document.getElementById('claimSearch');
    const statusFilter = document.getElementById('claimStatus');
    const claimRows = document.querySelectorAll('.verify-row');
    const modal = document.getElementById('verifyModal');
    const modalContent = document.getElementById('verifyDetails');
    const closeBtn = document.querySelector('.verify-modal-close');

    function filterClaims() {
        const searchTerm = searchInput.value.toLowerCase();
        const statusTerm = statusFilter.value.toLowerCase();

        claimRows.forEach(row => {
            const rowText = row.textContent.toLowerCase();
            const rowStatus = row.getAttribute('data-status').toLowerCase();
            const matchesSearch = rowText.includes(searchTerm);
            const matchesStatus = statusTerm === '' || rowStatus === statusTerm;
            row.style.display = matchesSearch && matchesStatus ? '' : 'none';
        });
    }

    if (searchInput && statusFilter) {
        searchInput.addEventListener('input', filterClaims);
        statusFilter.addEventListener('change', filterClaims);
    }

    // Event delegation for dynamically added elements
    document.addEventListener('click', function (e) {
        if (e.target && e.target.classList.contains('verify-details-button')) {
            const row = e.target.closest('tr');
            const details = `
                <p><strong>Claim ID:</strong> ${row.cells[0].textContent}</p>
                <p><strong>User ID:</strong> ${row.getAttribute('data-user-id')}</p>
                <p><strong>Lecturer:</strong> ${row.cells[1].textContent}</p>
                <p><strong>Submission Date:</strong> ${row.cells[2].textContent}</p>
                <p><strong>Claim Amount:</strong> ${row.cells[3].textContent}</p>
                <p><strong>Status:</strong> ${row.cells[4].textContent}</p>
                <p><strong>Hours Worked:</strong> ${row.getAttribute('data-hours-worked')}</p>
                <p><strong>Hourly Rate:</strong> R${row.getAttribute('data-hourly-rate')}</p>
                <p><strong>Claim Type:</strong> ${row.getAttribute('data-claim-type')}</p>
                <p><strong>Description:</strong> ${row.getAttribute('data-description')}</p>
                <h4>Supporting Documents</h4>
                <ul>
                    ${row.getAttribute('data-documents').split(',').map(doc => `<li>${doc.trim()}</li>`).join('')}
                </ul>
            `;
            modalContent.innerHTML = details;
            modal.style.display = 'block';
        } else if (e.target && (e.target.classList.contains('verify-approve-button') || e.target.classList.contains('verify-reject-button'))) {
            const row = e.target.closest('tr');
            const claimId = row.cells[0].textContent;
            const action = e.target.classList.contains('verify-approve-button') ? 'approve' : 'reject';
            const statusCell = row.cells[4].querySelector('.verify-status');

            if (action === 'approve') {
                statusCell.textContent = 'Approved';
                statusCell.className = 'verify-status verify-status-approved';
            } else {
                statusCell.textContent = 'Rejected';
                statusCell.className = 'verify-status verify-status-rejected';
            }

            // Remove approve and reject buttons
            row.querySelectorAll('.verify-approve-button, .verify-reject-button').forEach(button => button.remove());

            // Show overlay
            showVerifyActionOverlay(action, claimId);
        }
    });

    if (closeBtn) {
        closeBtn.onclick = function () {
            modal.style.display = 'none';
        }
    }

    window.onclick = function (event) {
        if (event.target == modal) {
            modal.style.display = 'none';
        }
    }

    function showVerifyActionOverlay(action, claimId) {
        const overlay = document.getElementById('verifyActionOverlay');
        const icon = document.getElementById('verifyActionIcon');
        const text = document.getElementById('verifyActionText');

        if (action === 'approve') {
            icon.className = 'fas fa-check-circle';
            icon.style.color = 'var(--color-success)';
            text.textContent = `Claim ${claimId} has been approved.`;
        } else {
            icon.className = 'fas fa-times-circle';
            icon.style.color = 'var(--color-error)';
            text.textContent = `Claim ${claimId} has been rejected.`;
        }

        overlay.style.display = 'flex';
        setTimeout(() => {
            overlay.style.display = 'none';
        }, 3000);
    }
}

// Call the initialisation functions when the DOM is fully loaded
document.addEventListener('DOMContentLoaded', function () {
    initialiseClaimView();

    // Check if we're on the User Profile page
    if (document.getElementById('userProfileForm')) {
        initialiseUserProfile();
    }

    // Check if we're on the Verify Claims page
    if (document.querySelector('.verify-container')) {
        initialiseVerifyView();
    }

    // Check if we're on the Manage Lecturers page
    if (document.querySelector('.manage-lecturers-container')) {
        initialiseManageLecturers();
    }
});

/* ********************************************************************************************************************************************************************** */

// Function for Manage Lecturers view functionality

// This function initialises the Manage Lecturers view functionality. It allows the user to add, edit, and delete lecturers.
function initialiseManageLecturers() {
    const form = document.getElementById('lecturerForm');
    const lecturerTable = document.querySelector('.lecturer-table tbody');
    const submitButton = document.getElementById('submitButton');
    const formTitle = document.getElementById('formTitle');
    let editMode = false;

    if (form) {
        form.addEventListener('submit', function (e) {
            e.preventDefault();
            const lecturerId = document.getElementById('lecturerId').value;
            const firstName = document.getElementById('FirstName').value;
            const lastName = document.getElementById('LastName').value;
            const email = document.getElementById('Email').value;
            const phone = document.getElementById('PhoneNumber').value;

            if (editMode) {
                updateLecturerInTable(lecturerId, firstName, lastName, email, phone);
                showManageActionOverlay('update', `${firstName} ${lastName}`);
            } else {
                addLecturerToTable(firstName, lastName, email, phone);
                showManageActionOverlay('add', `${firstName} ${lastName}`);
            }

            resetForm();
        });
    }

    // Add event listeners to the edit and delete buttons in the lecturer table to allow the user to edit and delete lecturers
    if (lecturerTable) {
        lecturerTable.addEventListener('click', function (e) {
            if (e.target.classList.contains('edit-lecturer-button')) {
                const row = e.target.closest('tr');
                const [name, email, phone] = row.querySelectorAll('td');
                const [firstName, lastName] = name.textContent.split(' ');

                document.getElementById('lecturerId').value = row.dataset.id || '';
                document.getElementById('FirstName').value = firstName;
                document.getElementById('LastName').value = lastName;
                document.getElementById('Email').value = email.textContent;
                document.getElementById('PhoneNumber').value = phone.textContent;

                submitButton.textContent = 'Update Lecturer';
                formTitle.textContent = 'Edit Lecturer';
                editMode = true;
            } else if (e.target.classList.contains('delete-lecturer-button')) {
                if (confirm('Are you sure you want to delete this lecturer?')) {
                    const row = e.target.closest('tr');
                    const name = row.cells[0].textContent;
                    row.remove();
                    showManageActionOverlay('delete', name);
                }
            }
        });
    }
    // Reset the form and set the submit button text to 'Add Lecturer' when the user closes the modal or clicks the cancel button
    function addLecturerToTable(firstName, lastName, email, phone) {
        const newRow = lecturerTable.insertRow();
        newRow.dataset.id = Date.now().toString();
        newRow.innerHTML = `
            <td>${firstName} ${lastName}</td>
            <td>${email}</td>
            <td>${phone}</td>
            <td>
                <button class="edit-lecturer-button">Edit</button>
                <button class="delete-lecturer-button">Delete</button>
            </td>
        `;
    }
    // Update the lecturer details in the table when the user clicks the submit button in edit mode
    function updateLecturerInTable(id, firstName, lastName, email, phone) {
        const row = lecturerTable.querySelector(`tr[data-id="${id}"]`);
        if (row) {
            row.innerHTML = `
                <td>${firstName} ${lastName}</td>
                <td>${email}</td>
                <td>${phone}</td>
                <td>
                    <button class="edit-lecturer-button">Edit</button>
                    <button class="delete-lecturer-button">Delete</button>
                </td>
            `;
        }
    }
    // Reset the form and set the submit button text to 'Add Lecturer' when the user closes the modal or clicks the cancel button
    function resetForm() {
        form.reset();
        document.getElementById('lecturerId').value = '';
        submitButton.textContent = 'Add Lecturer';
        formTitle.textContent = 'Add New Lecturer';
        editMode = false;
    }
    function showManageActionOverlay(action, lecturerName) {
        const overlay = document.getElementById('manageActionOverlay');
        const icon = document.getElementById('manageActionIcon');
        const text = document.getElementById('manageActionText');

        switch (action) {
            case 'add':
                icon.className = 'fas fa-user-plus';
                icon.style.color = 'var(--color-success)';
                text.textContent = `Lecturer ${lecturerName} has been added.`;
                break;
            case 'update':
                icon.className = 'fas fa-user-edit';
                icon.style.color = 'var(--color-primary)';
                text.textContent = `Lecturer ${lecturerName} has been updated.`;
                break;
            case 'delete':
                icon.className = 'fas fa-user-minus';
                icon.style.color = 'var(--color-error)';
                text.textContent = `Lecturer ${lecturerName} has been deleted.`;
                break;
        }

        overlay.style.display = 'flex';
        setTimeout(() => {
            overlay.style.display = 'none';
        }, 3000);
    }
}