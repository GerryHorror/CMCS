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

// User Profile Functionality - handles form submission and shows success message when the form is submitted
const initialiseUserProfile = () => {
    // Define branch codes
    const branchCodes = {
        StandardBank: "051001",
        FNB: "250655",
        ABSA: "632005",
        Capitec: "470010",
        Nedbank: "198765",
        TymeBank: "678910",
        BankOfAthens: "410506",
        BidvestBank: "462005",
        Investec: "580105",
        SAPostBank: "460005",
        AfricanBank: "430000",
        DiscoveryBank: "679000",
        OldMutual: "462005"
    };

    // Get form and relevant elements
    const form = document.getElementById('userProfileForm');
    const bankNameSelect = document.getElementById('BankName');
    const branchCodeInput = document.getElementById('BranchCode');

    // Ensure that all necessary elements are present
    if (bankNameSelect && branchCodeInput) {
        // Function to set branch code based on selected bank
        const setBranchCode = () => {
            const selectedBank = bankNameSelect.value;
            branchCodeInput.value = branchCodes[selectedBank] || '';
        };

        // Set initial branch code when page loads
        setBranchCode();

        // Update branch code when bank selection changes
        bankNameSelect.addEventListener('change', setBranchCode);
    } else {
        console.error("Bank name select or branch code input not found.");
    }

    // Handle form submission
    if (form) {
        form.onsubmit = async (e) => {
            e.preventDefault();
            console.log("Form submission triggered");

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
                    const passwordField = document.getElementById('UserPassword');
                    if (passwordField) {
                        passwordField.value = '';
                    } else {
                        console.warn("Password field not found.");
                    }
                } else {
                    let errorMessage = result.message;
                    if (result.errors && result.errors.length > 0) {
                        errorMessage += ': ' + result.errors.join(', ');
                    }
                    showActionOverlay('userActionOverlay', 'fas fa-exclamation-circle', 'var(--color-error)', errorMessage);
                }
            } catch (error) {
                console.error('Error during form submission:', error);
                showActionOverlay('userActionOverlay', 'fas fa-exclamation-circle', 'var(--color-error)', 'An error occurred. Please try again.');
            }
        };
    } else {
        console.error('User Profile form is missing from the page.');
    }
};

// Ensure the profile initialization occurs once DOM is fully loaded
document.addEventListener('DOMContentLoaded', initialiseUserProfile);