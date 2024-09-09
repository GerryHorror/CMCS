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
    // Get the modal and its details element from the DOM
    const modal = document.getElementById('claimModal');
    const modalDetails = document.getElementById('claimDetails');
    // Get the close button for the modal
    const closeBtn = document.querySelector('.claim-modal-close');
    // Get all the claim rows, search input, and status filter elements
    const claimRows = document.querySelectorAll('.claim-row');
    const searchInput = document.getElementById('claimSearch');
    const statusFilter = document.getElementById('claimStatus');

    // Function to show the modal with claim details
    const showModal = (row) => {
        // Get the claim data from the row's dataset
        const claimData = row.dataset;
        // Create the details HTML using the claim data
        const details = `
            <p><strong>Claim ID:</strong> ${row.cells[0].textContent}</p>
            <p><strong>User ID:</strong> ${claimData.userId}</p>
            <p><strong>Date:</strong> ${row.cells[1].textContent}</p>
            <p><strong>Claim Amount:</strong> ${row.cells[2].textContent}</p>
            <p><strong>Status:</strong> ${row.cells[3].textContent}</p>
            <p><strong>Hours Worked:</strong> ${claimData.hoursWorked}</p>
            <p><strong>Hourly Rate:</strong> R${claimData.hourlyRate}</p>
            <p><strong>Claim Type:</strong> ${claimData.claimType}</p>
            <p><strong>Description:</strong> ${claimData.description}</p>
            <h4>Supporting Documents</h4>
            <ul>
                ${claimData.documents.split(',').map(doc => `<li>${doc.trim()}</li>`).join('')}
            </ul>
        `;
        // Set the modal details and display the modal
        modalDetails.innerHTML = details;
        modal.style.display = 'block';
    };

    // Add event listener to show the modal when a claim details button is clicked
    document.addEventListener('click', (e) => {
        if (e.target.classList.contains('claim-details-button')) {
            const row = e.target.closest('tr');
            showModal(row);
        }
    });

    // Add event listener to close the modal when the close button is clicked
    if (closeBtn) {
        closeBtn.onclick = () => modal.style.display = 'none';
    }

    // Add event listener to close the modal when clicking outside of it
    window.onclick = (event) => {
        if (event.target == modal) modal.style.display = 'none';
    };

    // Function to filter claims based on search term and status
    const filterClaims = () => {
        const searchTerm = searchInput.value.toLowerCase();
        const statusTerm = statusFilter.value.toLowerCase();

        // Loop through each claim row and check if it matches the search term and status
        claimRows.forEach(row => {
            const rowText = row.textContent.toLowerCase();
            const rowStatus = row.getAttribute('data-status').toLowerCase();
            const matchesSearch = rowText.includes(searchTerm);
            const matchesStatus = statusTerm === '' || rowStatus === statusTerm;
            row.style.display = matchesSearch && matchesStatus ? '' : 'none';
        });
    };

    // Add event listeners to filter claims when the search input or status filter changes
    if (searchInput && statusFilter) {
        searchInput.addEventListener('input', filterClaims);
        statusFilter.addEventListener('change', filterClaims);
    }
};

// <------------------------------------------------------------------------------------------------------------------------------------------------------------>

// Submit view functionality - handles form submission, adding work entries, and uploading supporting documents
const initialiseSubmitView = () => {
    // Get the submit form and input elements for hourly rate, claim amount, and supporting document
    const submitForm = document.querySelector('.submit-form');
    if (submitForm) {
        const hourlyRateInput = document.getElementById('hourlyRate');
        const claimAmountInput = document.getElementById('claimAmount');
        const addEntryButton = document.getElementById('addEntry');
        const workEntriesContainer = document.getElementById('workEntries');
        const supportingDocumentInput = document.getElementById('supportingDocument');
        // Counter for work entries
        let entryCount = 1;
        // Function to calculate the claim amount based on hourly rate and total hours worked (sum of all work entries)
        const calculateClaimAmount = () => {
            // Get the hourly rate value from the input element or set it to 0 if empty
            const hourlyRate = parseFloat(hourlyRateInput.value) || 0;
            let totalHours = 0;
            // Loop through all work hours input elements and calculate the total hours worked
            document.querySelectorAll('.work-hours').forEach(input => {
                totalHours += parseFloat(input.value) || 0;
            });
            // Calculate the total amount by multiplying total hours worked with hourly rate and set the claim amount input value
            const totalAmount = totalHours * hourlyRate;
            claimAmountInput.value = totalAmount.toFixed(2);
        };
        // Function to add a new work entry (work date and hours worked) to the form
        const addNewWorkEntry = () => {
            // Create a new work entry HTML template with input fields for work date and hours worked (when the user clicks on the add entry button)
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
            // Insert the new work entry HTML template into the work entries container and increment the entry count
            workEntriesContainer.insertAdjacentHTML('beforeend', newEntry);
            entryCount++;
        };
        // Add event listeners for input, click, and form submission events
        hourlyRateInput.addEventListener('input', calculateClaimAmount);
        workEntriesContainer.addEventListener('input', (event) => {
            if (event.target.classList.contains('work-hours')) {
                calculateClaimAmount();
            }
        });
        // Add event listener for adding a new work entry when user clicks on the add entry button
        addEntryButton.addEventListener('click', addNewWorkEntry);
        // Add event listener for submitting the form and showing a success message when the form is submitted
        submitForm.addEventListener('submit', (event) => {
            // Prevent the default form submission behaviour to handle it manually
            event.preventDefault();
            if (!supportingDocumentInput.files.length) {
                alert('Please upload a supporting document before submitting.');
                return;
            }
            // Show a success message when the form is submitted successfully
            showActionOverlay('successOverlay', 'fas fa-check-circle', 'var(--color-success)', 'Claim successfully submitted!');
            submitForm.reset();
        });
    }
};

// <------------------------------------------------------------------------------------------------------------------------------------------------------------>

// User Profile Functionality - handles form submission and shows success message when the form is submitted
const initialiseUserProfile = () => {
    // Get the user profile form element
    const form = document.getElementById('userProfileForm');
    // Add event listener for form submission and show a success message when the form is submitted
    if (form) {
        form.onsubmit = (e) => {
            e.preventDefault();
            showActionOverlay('userActionOverlay', 'fas fa-user-check', 'var(--color-success)', 'Profile updated successfully!');
        };
    } else {
        console.error('User Profile form is missing from the page.');
    }
};

// <------------------------------------------------------------------------------------------------------------------------------------------------------------>

// Verify view functionality - handles search, filtering, viewing claim details, and approving/rejecting claims
// This function is used to initialise the verify view functionality
const initialiseVerifyView = () => {
    // Get elements for search and filtering claims
    const searchInput = document.getElementById('claimSearch');
    // Get elements for search and filtering claims
    const statusFilter = document.getElementById('claimStatus');
    // Get all claim rows and the modal elements
    const claimRows = document.querySelectorAll('.verify-row');
    // Get modal (popup) elements for displaying claim details and close button
    const modal = document.getElementById('verifyModal');
    // Get modal content element for displaying claim details
    const modalContent = document.getElementById('verifyDetails');
    // Get close button for the modal
    const closeBtn = document.querySelector('.verify-modal-close');

    // Function to filter claims based on search term and status filter. Performs a case-insensitive search to match the search term with the claim details.
    const filterClaims = () => {
        const searchTerm = searchInput.value.toLowerCase();
        const statusTerm = statusFilter.value.toLowerCase();

        // Loop through each claim row and check if it matches the search term and status filter
        claimRows.forEach(row => {
            const rowText = row.textContent.toLowerCase();
            const rowStatus = row.getAttribute('data-status').toLowerCase();
            const matchesSearch = rowText.includes(searchTerm);
            const matchesStatus = statusTerm === '' || rowStatus === statusTerm;
            row.style.display = matchesSearch && matchesStatus ? '' : 'none';
        });
    };
    // Add event listeners for filtering claims when user types in search or changes the status filter. If the search input or status filter is not found, the event listeners are not added.
    if (searchInput && statusFilter) {
        searchInput.addEventListener('input', filterClaims);
        statusFilter.addEventListener('change', filterClaims);
    }
    // Function to handle approve or reject action on a claim. Updates the status cell, removes approve/reject buttons, and shows an overlay message.
    const handleVerifyAction = (row, action) => {
        const claimId = row.cells[0].textContent;
        const statusCell = row.cells[4].querySelector('.verify-status');
        // Update the status cell text and class based on the action (approve or reject)
        statusCell.textContent = action === 'approve' ? 'Approved' : 'Rejected';
        statusCell.className = `verify-status verify-status-${action}d`;

        // Remove approve and reject buttons from the row after action is performed
        row.querySelectorAll('.verify-approve-button, .verify-reject-button').forEach(button => button.remove());

        // Show overlay message with success or error icon based on the action (approve or reject)
        const iconClass = action === 'approve' ? 'fas fa-check-circle' : 'fas fa-times-circle';
        const iconColor = action === 'approve' ? 'var(--color-success)' : 'var(--color-error)';
        const message = `Claim ${claimId} has been ${action}d.`;
        showActionOverlay('verifyActionOverlay', iconClass, iconColor, message);
    };

    // Add event listeners for viewing claim details and approving/rejecting claims when user clicks on the buttons in the claim row
    document.addEventListener('click', (e) => {
        // Check if the clicked element is a verify details, approve, or reject button
        if (e.target.classList.contains('verify-details-button')) {
            const row = e.target.closest('tr');
            // Get the claim details from the row attributes and display them in the modal content (popup)
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
            // Set the modal content to the claim details and display the modal
            modalContent.innerHTML = details;
            modal.style.display = 'block';
            // Add event listeners for approve and reject buttons in the modal content (popup) to handle the actions
        } else if (e.target.classList.contains('verify-approve-button')) {
            handleVerifyAction(e.target.closest('tr'), 'approve');
        } else if (e.target.classList.contains('verify-reject-button')) {
            handleVerifyAction(e.target.closest('tr'), 'reject');
        }
    });
    // Add event listener for closing the modal when user clicks on close button
    if (closeBtn) {
        closeBtn.onclick = () => {
            modal.style.display = 'none';
        }
    }
    // Add event listener for closing the modal when user clicks outside the modal
    window.onclick = (event) => {
        if (event.target == modal) {
            modal.style.display = 'none';
        }
    }
};

// <------------------------------------------------------------------------------------------------------------------------------------------------------------>

// Manage Lecturers functionality - handles adding, editing, and deleting lecturers in a table

// Function to initialise the Manage Lecturers view
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
        // Set the icon class, color, and message based on the action (add, update, delete)
        let iconClass, iconColor, message;

        // Switch statement to set the icon class, color, and message based on the action
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
        // Show the overlay message with the icon, color, and message for the action
        showActionOverlay('manageActionOverlay', iconClass, iconColor, message);
    };

    // Function to add a new lecturer to the table with the provided first name, last name, email, and phone number
    const addLecturerToTable = (firstName, lastName, email, phone) => {
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
    };

    // Function to update a lecturer in the table with the provided id, first name, last name, email, and phone number
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

    // Function to reset the form fields, set the submit button text to 'Add Lecturer', set the form title to 'Add New Lecturer', and set edit mode to false (add mode)
    const resetForm = () => {
        form.reset();
        document.getElementById('lecturerId').value = '';
        submitButton.textContent = 'Add Lecturer';
        formTitle.textContent = 'Add New Lecturer';
        editMode = false;
    };

    // Add event listeners for form submission and table row clicks to edit or delete lecturers in the table
    if (form) {
        form.addEventListener('submit', (e) => {
            e.preventDefault();
            const lecturerId = document.getElementById('lecturerId').value;
            const firstName = document.getElementById('FirstName').value;
            const lastName = document.getElementById('LastName').value;
            const email = document.getElementById('Email').value;
            const phone = document.getElementById('PhoneNumber').value;

            // Check if the first name, last name, email, and phone number are not empty
            if (editMode) {
                updateLecturerInTable(lecturerId, firstName, lastName, email, phone);
                handleLecturerAction('update', firstName, lastName);
            } else {
                addLecturerToTable(firstName, lastName, email, phone);
                handleLecturerAction('add', firstName, lastName);
            }
            // Reset the form fields after adding or updating a lecturer in the table
            resetForm();
        });
    }

    // Add event listener for table row clicks to edit or delete lecturers in the table based on the clicked button
    if (lecturerTable) {
        lecturerTable.addEventListener('click', (e) => {
            if (e.target.classList.contains('edit-lecturer-button')) {
                const row = e.target.closest('tr');
                const [name, email, phone] = row.querySelectorAll('td');
                const [firstName, lastName] = name.textContent.split(' ');

                // Set the form fields with the lecturer details from the row and set the submit button text to 'Update Lecturer', form title to 'Edit Lecturer', and edit mode to true
                document.getElementById('lecturerId').value = row.dataset.id || '';
                document.getElementById('FirstName').value = firstName;
                document.getElementById('LastName').value = lastName;
                document.getElementById('Email').value = email.textContent;
                document.getElementById('PhoneNumber').value = phone.textContent;

                // Set the submit button text to 'Update Lecturer', form title to 'Edit Lecturer', and edit mode to true
                submitButton.textContent = 'Update Lecturer';
                formTitle.textContent = 'Edit Lecturer';
                editMode = true;
            }
            // Else if the clicked element is a delete button, show a confirmation dialog and delete the lecturer row if confirmed
            else if (e.target.classList.contains('delete-lecturer-button')) {
                if (confirm('Are you sure you want to delete this lecturer?')) {
                    const row = e.target.closest('tr');
                    const name = row.cells[0].textContent;
                    row.remove();
                    handleLecturerAction('delete', ...name.split(' '));
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