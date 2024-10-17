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

document.addEventListener('DOMContentLoaded', () => {
    if (document.querySelector('.manage-lecturers-container')) {
        initialiseManageLecturers();
    }
});