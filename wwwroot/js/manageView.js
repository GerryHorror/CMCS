// Reusable function for showing overlays. This function is used to show success and error messages to the user.
const showActionOverlay = (overlayId, iconClass, iconColor, message, duration = 3000) => {
    // Get the overlay, icon, and text elements from the DOM using the overlayId parameter
    const overlay = document.getElementById(overlayId);
    const icon = overlay.querySelector('i');
    const text = overlay.querySelector('p');
    // Set the icon class, color, and text content to the values passed in as parameters to the function
    icon.className = iconClass;
    icon.style.color = iconColor;
    text.textContent = message;
    // Display the overlay by setting its display style to 'flex' (i.e., show the overlay as a flex container)
    overlay.style.display = 'flex';
    setTimeout(() => {
        overlay.style.display = 'none';
    }, duration);
};

// Manage Lecturers functionality - handles adding, editing, and deleting lecturers in a table
const initialiseManageLecturers = () => {
    // Get the lecturer form, lecturer table, submit button, form title, and edit mode flag
    const form = document.getElementById('lecturerForm');
    const lecturerTable = document.querySelector('.lecturer-table tbody');
    const submitButton = document.getElementById('submitButton');
    const formTitle = document.getElementById('formTitle');
    let editMode = false;

    const branchCodes = {
        StandardBank: "051001",
        FNB: "250655",
        ABSA: "632005",
        Capitec: "470010",
        Nedbank: "198765",
        TymeBank: "678910",
        BankofAthens: "410506",
        BidvestBank: "462005",
        Investec: "580105",
        SAPostBank: "460005",
        AfricanBank: "430000",
        DiscoveryBank: "679000",
        OldMutual: "462005"
    };

    const bankNameSelect = document.getElementById('BankName');
    const branchCodeInput = document.getElementById('BranchCode');

    if (bankNameSelect && branchCodeInput) {
        bankNameSelect.addEventListener('change', function () {
            const selectedBank = this.value;
            branchCodeInput.value = branchCodes[selectedBank] || '';
        });
    }

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
    const addLecturerToTable = (id, firstName, lastName, email, phone, roleName) => {
        const newRow = lecturerTable.insertRow();
        newRow.dataset.id = id;
        newRow.innerHTML = `
            <td>${firstName} ${lastName}</td>
            <td>${email}</td>
            <td>${phone}</td>
            <td>${roleName || ''}</td>
            <td>
                <button class="edit-lecturer-button">Edit</button>
                <button class="delete-lecturer-button">Delete</button>
            </td>
        `;
    };

    // Function to update a lecturer in the table
    const updateLecturerInTable = (id, firstName, lastName, email, phone, roleName) => {
        const row = lecturerTable.querySelector(`tr[data-id="${id}"]`);
        if (row) {
            row.innerHTML = `
                <td>${firstName} ${lastName}</td>
                <td>${email}</td>
                <td>${phone}</td>
                <td>${roleName || ''}</td>
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

            // Log form data to console for debugging
            console.log('Form data:', Object.fromEntries(formData));

            let lecturerId;
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
                                formData.get('PhoneNumber'),
                                formData.get('RoleID'),
                            );
                            handleLecturerAction('update', formData.get('FirstName'), formData.get('LastName'));
                        } else {
                            addLecturerToTable(
                                data.id,
                                formData.get('FirstName'),
                                formData.get('LastName'),
                                formData.get('UserEmail'),
                                formData.get('PhoneNumber'),
                                data.roleName
                            );
                            handleLecturerAction('add', formData.get('FirstName'), formData.get('LastName'));
                        }
                        resetForm();
                    } else {
                        console.error('Failed to add lecturer:', data);
                        alert('Failed to add lecturer: ' + data.message + '\n' + (data.errors ? data.errors.join('\n') : ''));
                    }
                })
                .catch(error => {
                    console.error('Error:', error);
                    showActionOverlay('manageActionOverlay', 'fas fa-exclamation-circle', 'var(--color-error)', 'An unexpected error occurred. Please try again.');
                });
        });
    }

    // Show the "Coming Soon" modal
    const showComingSoonModal = () => {
        const modal = document.getElementById('comingSoonModal');
        const closeButton = document.querySelector('.close-button');

        modal.style.display = 'block';

        // Close the modal when the user clicks on the close button
        closeButton.addEventListener('click', () => {
            modal.style.display = 'none';
        });

        // Close the modal when the user clicks outside of the modal content
        window.addEventListener('click', (event) => {
            if (event.target === modal) {
                modal.style.display = 'none';
            }
        });
    };

    // Event listener for table row clicks to edit or delete lecturers
    if (lecturerTable) {
        lecturerTable.addEventListener('click', (e) => {
            if (e.target.classList.contains('edit-lecturer-button') || e.target.classList.contains('delete-lecturer-button')) {
                // Show a modal or overlay indicating that the functionality is coming soon
                showActionOverlay('manageActionOverlay', 'fas fa-tools', 'var(--color-warning)', 'This feature is coming soon!');
            }
        });
    }
};

// Document ready function to initialize the Manage Lecturers functionality
document.addEventListener('DOMContentLoaded', () => {
    if (document.querySelector('.manage-lecturers-container')) {
        initialiseManageLecturers();
    }
});