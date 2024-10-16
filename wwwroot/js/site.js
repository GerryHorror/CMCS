/*
    Student Name: Gérard Blankenberg
    Student Number: ST10046280
    Module: PROG6212
    POE Part 1
*/

/**
 * This JavaScript file contains various functions to handle different views and functionalities on the web app.
 * The functionalities are modularised into separate functions for better organisation and maintainability.
 * Below is a brief overview of each section and its purpose:
 *
 * 1. showActionOverlay:
 *    - A reusable function to display overlay messages (e.g., success or error messages) to the user.
 *    - Parameters: overlayId (ID of the overlay element), iconClass (CSS class for the icon), iconColor (colour of the icon), message (text message to display), duration (how long the overlay is shown).
 *
 * 2. initialiseClaimView:
 *    - Handles the search, filtering, and viewing of claim details.
 *    - Adds event listeners for search input, status filter, and claim detail buttons.
 *
 * 3. initialiseSubmitView:
 *    - Manages form submission for submitting claims, adding work entries, and uploading supporting documents.
 *    - Calculates the claim amount based on hourly rate and total hours worked.
 *
 * 4. initialiseUserProfile:
 *    - Handles the user profile form submission and displays a success message upon successful update.
 *
 * 5. initialiseVerifyView:
 *    - Manages the verification of claims, including searching, filtering, viewing details, and approving/rejecting claims.
 *    - Adds event listeners for claim actions and displays appropriate messages.
 *
 * 6. initialiseManageLecturers:
 *    - Handles adding, editing, and deleting lecturers in a table.
 *    - Manages form submission and updates the lecturer table accordingly.
 *
 * The initialisation functions are called when the DOM content is fully loaded to ensure that the JavaScript code is executed after the HTML content is loaded.
 */

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

// Claim view functionality - handles search, filtering, and viewing claim details
const initialiseClaimView = () => {
    const modal = document.getElementById('claimModal');
    const modalDetails = document.getElementById('claimDetails');
    const closeBtn = document.querySelector('.claim-modal-close');
    const claimRows = document.querySelectorAll('.claim-row');
    const searchInput = document.getElementById('claimSearch');
    const statusFilter = document.getElementById('claimStatus');

    const showModal = async (claimId) => {
        try {
            const response = await fetch(`/Claim/Details/${claimId}`);
            if (!response.ok) {
                throw new Error('Failed to fetch claim details');
            }
            const claim = await response.json();

            const details = `
            <p><strong>Claim ID:</strong> ${claim.claimID}</p>
            <p><strong>Submission Date:</strong> ${new Date(claim.submissionDate).toLocaleDateString()}</p>
            <p><strong>Claim Amount:</strong> R${claim.claimAmount.toFixed(2)}</p>
            <p><strong>Status:</strong> ${claim.statusName}</p>
            <p><strong>Hours Worked:</strong> ${claim.hoursWorked}</p>
            <p><strong>Hourly Rate:</strong> R${claim.hourlyRate.toFixed(2)}</p>
            <p><strong>Claim Type:</strong> ${claim.claimType}</p>
            <p><strong>Supporting Documents:</strong> ${claim.documents.length > 0 ? claim.documents.join(', ') : 'No documents'}</p>
        `;

            modalDetails.innerHTML = details;
            modal.style.display = 'block';
        } catch (error) {
            console.error('Error fetching claim details:', error);
        }
    };

    document.addEventListener('click', (e) => {
        if (e.target.classList.contains('claim-details-button')) {
            const claimId = e.target.getAttribute('data-id');
            showModal(claimId);
        }
    });

    if (closeBtn) {
        closeBtn.onclick = () => modal.style.display = 'none';
    }

    window.onclick = (event) => {
        if (event.target == modal) modal.style.display = 'none';
    };

    const filterClaims = () => {
        const searchTerm = searchInput.value.toLowerCase();
        const statusTerm = statusFilter.value.toLowerCase();

        claimRows.forEach(row => {
            const rowText = row.textContent.toLowerCase();
            const rowStatus = row.getAttribute('data-status').toLowerCase();
            const matchesSearch = rowText.includes(searchTerm);
            const matchesStatus = statusTerm === '' || rowStatus === statusTerm;
            row.style.display = matchesSearch && matchesStatus ? '' : 'none';
        });
    };

    if (searchInput && statusFilter) {
        searchInput.addEventListener('input', filterClaims);
        statusFilter.addEventListener('change', filterClaims);
    }
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
        };

        hourlyRateInput.addEventListener('input', calculateClaimAmount);
        workEntriesContainer.addEventListener('input', (event) => {
            if (event.target.classList.contains('work-hours')) {
                calculateClaimAmount();
            }
        });
        addEntryButton.addEventListener('click', addNewWorkEntry);

        submitForm.addEventListener('submit', async (event) => {
            event.preventDefault();
            if (!supportingDocumentInput.files.length) {
                alert('Please upload a supporting document before submitting.');
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

// <------------------------------------------------------------------------------------------------------------------------------------------------------------>

// User Profile Functionality - handles form submission and shows success message when the form is submitted
const initialiseUserProfile = () => {
    const form = document.getElementById('userProfileForm');
    if (form) {
        form.onsubmit = async (e) => {
            e.preventDefault();

            try {
                const formData = new FormData(form);
                const response = await fetch(form.action, {
                    method: 'POST',
                    body: formData,
                    headers: {
                        'X-Requested-With': 'XMLHttpRequest'
                    }
                });

                const result = await response.json();
                console.log('Server response:', result);

                if (result.success) {
                    showActionOverlay('userActionOverlay', 'fas fa-user-check', 'var(--color-success)', 'Profile updated successfully!');
                    // Clear password field after successful update
                    document.getElementById('UserPassword').value = '';
                } else {
                    let errorMessage = result.message;
                    if (result.errors && result.errors.length > 0) {
                        errorMessage += ': ' + result.errors.join(', ');
                    }
                    showActionOverlay('userActionOverlay', 'fas fa-exclamation-circle', 'var(--color-error)', errorMessage);
                }
            } catch (error) {
                console.error('Error:', error);
                showActionOverlay('userActionOverlay', 'fas fa-exclamation-circle', 'var(--color-error)', 'An error occurred. Please try again.');
            }
        };
    } else {
        console.error('User Profile form is missing from the page.');
    }
};

// <------------------------------------------------------------------------------------------------------------------------------------------------------------>

// Verify view functionality - handles search, filtering, viewing claim details, and approving/rejecting claims
// This function is used to initialise the verify view functionality
const initialiseVerifyView = () => {
    const searchInput = document.getElementById('claimSearch');
    const statusFilter = document.getElementById('claimStatus');
    const claimRows = document.querySelectorAll('.verify-row');
    const modal = document.getElementById('verifyModal');
    const modalContent = document.getElementById('verifyDetails');
    const closeBtn = document.querySelector('.verify-modal-close');

    const filterClaims = () => {
        const searchTerm = searchInput.value.toLowerCase();
        const statusTerm = statusFilter.value.toLowerCase();

        claimRows.forEach(row => {
            const rowText = row.textContent.toLowerCase();
            const rowStatus = row.getAttribute('data-status').toLowerCase();
            const matchesSearch = rowText.includes(searchTerm);
            const matchesStatus = statusTerm === '' || rowStatus === statusTerm;
            row.style.display = matchesSearch && matchesStatus ? '' : 'none';
        });
    };

    if (searchInput && statusFilter) {
        searchInput.addEventListener('input', filterClaims);
        statusFilter.addEventListener('change', filterClaims);
    }

    const handleVerifyAction = async (claimId, action) => {
        try {
            const response = await fetch('/Approval/UpdateStatus', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({
                    claimId: parseInt(claimId),
                    status: action === 'approve' ? 'Approved' : 'Rejected'
                }),
            });

            if (!response.ok) {
                const errorText = await response.text();
                throw new Error(`Failed to update claim status: ${errorText}`);
            }

            const result = await response.json();
            if (result.success) {
                const row = document.querySelector(`.verify-row[data-id="${claimId}"]`);
                const statusCell = row.querySelector('.verify-status');
                statusCell.textContent = action === 'approve' ? 'Approved' : 'Rejected';
                statusCell.className = `verify-status verify-status-${action === 'approve' ? 'approved' : 'rejected'}`;
                row.querySelectorAll('.verify-approve-button, .verify-reject-button').forEach(btn => btn.remove());
                showActionOverlay('verifyActionOverlay', `fas fa-${action === 'approve' ? 'check' : 'times'}-circle`, `var(--color-${action === 'approve' ? 'success' : 'error'})`, result.message);
            }
        } catch (error) {
            console.error('Error:', error);
            showActionOverlay('verifyActionOverlay', 'fas fa-exclamation-circle', 'var(--color-error)', error.message);
        }
    };

    const showClaimDetails = async (claimId) => {
        try {
            const response = await fetch(`/Approval/Details/${claimId}`);
            if (!response.ok) {
                throw new Error('Failed to fetch claim details');
            }
            const claim = await response.json();

            const details = `
                <p><strong>Claim ID:</strong> ${claim.claimID}</p>
                <p><strong>Lecturer:</strong> ${claim.firstName} ${claim.lastName}</p>
                <p><strong>Submission Date:</strong> ${new Date(claim.submissionDate).toLocaleDateString()}</p>
                <p><strong>Claim Amount:</strong> R${claim.claimAmount.toFixed(2)}</p>
                <p><strong>Status:</strong> ${claim.statusName}</p>
                <p><strong>Hours Worked:</strong> ${claim.hoursWorked}</p>
                <p><strong>Hourly Rate:</strong> R${claim.hourlyRate.toFixed(2)}</p>
                <p><strong>Claim Type:</strong> ${claim.claimType}</p>
                <h4>Supporting Documents:</h4>
                <ul>
                    ${claim.documents.map(doc => `<li>${doc}</li>`).join('')}
                </ul>
            `;

            modalContent.innerHTML = details;
            modal.style.display = 'block';
        } catch (error) {
            console.error('Error fetching claim details:', error);
        }
    };

    document.addEventListener('click', (e) => {
        if (e.target.classList.contains('verify-details-button')) {
            const claimId = e.target.getAttribute('data-id');
            showClaimDetails(claimId);
        } else if (e.target.classList.contains('verify-approve-button')) {
            const claimId = e.target.getAttribute('data-id');
            handleVerifyAction(claimId, 'approve');
        } else if (e.target.classList.contains('verify-reject-button')) {
            const claimId = e.target.getAttribute('data-id');
            handleVerifyAction(claimId, 'reject');
        }
    });

    if (closeBtn) {
        closeBtn.onclick = () => modal.style.display = 'none';
    }

    window.onclick = (event) => {
        if (event.target == modal) {
            modal.style.display = 'none';
        }
    };
};
// <------------------------------------------------------------------------------------------------------------------------------------------------------------>

// Manage Lecturers functionality - handles adding, editing, and deleting lecturers in a table

// Function to initialise the Manage view
const initialiseManageLecturers = () => {
    // Get the lecturer form, lecturer table, submit button, form title, and edit mode flag
    const form = document.getElementById('lecturerForm');
    const lecturerTable = document.querySelector('.lecturer-table tbody');
    const submitButton = document.getElementById('submitButton');
    const formTitle = document.getElementById('formTitle');
    // Flag to track if the form is in edit mode
    let editMode = false;

    // Function to show an overlay message with an icon and message based on the action (add, update, delete) and lecturer name
    const handleLecturerAction = (action, firstName, lastName) => {
        const lecturerName = `${firstName} ${lastName}`;
        let iconClass, iconColor, message;

        switch (action) {
            case 'add':
                iconClass = 'fas fa-user-plus';
                iconColor = 'var(--color-success)';
                message = `Lecturer ${lecturerName} has been added.`;
                break;
            case 'update':
                iconClass = 'fas fa-user-edit';
                iconColor = 'var(--color-primary)';
                message = `Lecturer ${lecturerName} has been updated.`;
                break;
            case 'delete':
                iconClass = 'fas fa-user-minus';
                iconColor = 'var(--color-error)';
                message = `Lecturer ${lecturerName} has been deleted.`;
                break;
        }
        showActionOverlay('manageActionOverlay', iconClass, iconColor, message);
    };

    // Function to add a new lecturer to the table
    const addLecturerToTable = (id, firstName, lastName, email, phone) => {
        const newRow = lecturerTable.insertRow();
        newRow.dataset.id = id;
        newRow.innerHTML = `
            <td>${firstName} ${lastName}</td>
            <td>${email}</td>
            <td>${phone}</td>
            <td>
                <button class="edit-lecturer-button">Edit</button>
                <button class="delete-lecturer-button">Delete</button>
            </td>
        `;
    };

    // Function to update a lecturer in the table
    const updateLecturerInTable = (id, firstName, lastName, email, phone) => {
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
    };

    // Function to reset the form fields
    const resetForm = () => {
        form.reset();
        document.getElementById('lecturerId').value = '';
        submitButton.textContent = 'Add Lecturer';
        formTitle.textContent = 'Add New Lecturer';
        editMode = false;
    };

    // Event listener for form submission
    if (form) {
        form.addEventListener('submit', (e) => {
            e.preventDefault();

            const formData = new FormData(form);
            console.log('RoleID:', formData.get('RoleID'));
            //let lecturerId = null;

            // Log form data to console for debugging
            console.log('Form data:', Object.fromEntries(formData));

            if (editMode) {
                const lecturerIdElement = document.getElementById('lecturerId');
                if (lecturerIdElement) {
                    lecturerId = lecturerIdElement.value;
                }
            }

            const url = editMode ? `/User/EditLecturer/${lecturerId}` : '/User/AddLecturer';
            const method = editMode ? 'PUT' : 'POST';

            fetch(url, {
                method: method,
                body: formData
            })
                .then(response => response.json())
                .then(data => {
                    if (data.success) {
                        if (editMode) {
                            updateLecturerInTable(
                                lecturerId,
                                formData.get('FirstName'),
                                formData.get('LastName'),
                                formData.get('UserEmail'),
                                formData.get('PhoneNumber')
                            );
                            handleLecturerAction('update', formData.get('FirstName'), formData.get('LastName'));
                        } else {
                            addLecturerToTable(
                                data.id,
                                formData.get('FirstName'),
                                formData.get('LastName'),
                                formData.get('UserEmail'),
                                formData.get('PhoneNumber')
                            );
                            handleLecturerAction('add', formData.get('FirstName'), formData.get('LastName'));
                        }
                        resetForm();
                    } else {
                        console.error('Failed to add lecturer:', data);
                        alert('Failed to add lecturer: ' + data.message + '\n' + (data.errors ? data.errors.join('\n') : ''));
                    }
                })
                .catch(error => console.error('Error:', error));
        });
    }

    // Event listener for table row clicks to edit or delete lecturers
    if (lecturerTable) {
        lecturerTable.addEventListener('click', (e) => {
            if (e.target.classList.contains('edit-lecturer-button')) {
                const row = e.target.closest('tr');
                const [name, email, phone] = row.querySelectorAll('td');
                const [firstName, lastName] = name.textContent.split(' ');

                document.getElementById('lecturerId').value = row.dataset.id || '';
                document.getElementById('RoleID').value = row.dataset.roleId || '';
                document.getElementById('UserName').value = row.dataset.userName || '';
                document.getElementById('FirstName').value = firstName;
                document.getElementById('LastName').value = lastName;
                document.getElementById('UserEmail').value = email.textContent;
                document.getElementById('PhoneNumber').value = phone.textContent;

                // Populate other fields if available in dataset attributes
                document.getElementById('Address').value = row.dataset.address || '';
                document.getElementById('BankName').value = row.dataset.bankName || '';
                document.getElementById('BranchCode').value = row.dataset.branchCode || '';
                document.getElementById('BankAccountNumber').value = row.dataset.bankAccountNumber || '';

                submitButton.textContent = 'Update Lecturer';
                formTitle.textContent = 'Edit Lecturer';
                editMode = true;
            } else if (e.target.classList.contains('delete-lecturer-button')) {
                if (confirm('Are you sure you want to delete this lecturer?')) {
                    const row = e.target.closest('tr');
                    const lecturerId = row.dataset.id;
                    const name = row.cells[0].textContent;

                    // Make a DELETE request to delete the lecturer
                    fetch(`/User/DeleteLecturer/${lecturerId}`, {
                        method: 'DELETE'
                    })
                        .then(response => response.json())
                        .then(data => {
                            if (data.success) {
                                row.remove();
                                handleLecturerAction('delete', ...name.split(' '));
                            } else {
                                console.error('Failed to delete lecturer:', data.message);
                            }
                        })
                        .catch(error => console.error('Error:', error));
                }
            }
        });
    }
};

// <------------------------------------------------------------------------------------------------------------------------------------------------------------>

// Call the initialisation functions when the DOM content is loaded (i.e. when the page is fully loaded).
// This ensures that the JavaScript code is executed after the HTML content is loaded.

// Initialise the Claim View functionality
document.addEventListener('DOMContentLoaded', () => {
    initialiseClaimView();
    initialiseSubmitView();

    // Check if the user profile form exists on the page and initialise the user profile functionality
    if (document.getElementById('userProfileForm')) {
        initialiseUserProfile();
    }

    // Check if the verify container exists on the page and initialise the verify view functionality
    if (document.querySelector('.verify-container')) {
        initialiseVerifyView();
    }

    // Check if the manage lecturers container exists on the page and initialise the manage lecturers functionality
    if (document.querySelector('.manage-lecturers-container')) {
        initialiseManageLecturers();
    }
});

// <--------------------------------------------------------- END OF JAVASCRIPT CODE --------------------------------------------------------->