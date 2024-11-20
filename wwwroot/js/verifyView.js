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

                // Update status display
                const statusText = result.isAutoApproved ? 'Auto-Approved' :
                    (action === 'approve' ? 'Approved' : 'Rejected');

                statusCell.textContent = statusText;
                statusCell.className = `verify-status verify-status-${statusText.toLowerCase()}`;

                // Only remove buttons if the claim was approved (auto or manual) or rejected
                if (result.isAutoApproved || action === 'approve' || action === 'reject') {
                    row.querySelectorAll('.verify-approve-button, .verify-reject-button')
                        .forEach(btn => btn.remove());
                }

                // Show appropriate message with icon
                const iconClass = result.isAutoApproved ? 'fas fa-robot' :
                    `fas fa-${action === 'approve' ? 'check' : 'times'}-circle`;

                const iconColor = action === 'approve' || result.isAutoApproved ?
                    'var(--color-success)' : 'var(--color-error)';

                showActionOverlay('verifyActionOverlay', iconClass, iconColor, result.message);
            }
        } catch (error) {
            console.error('Error:', error);
            showActionOverlay('verifyActionOverlay',
                'fas fa-exclamation-circle',
                'var(--color-error)',
                error.message
            );
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

document.addEventListener('DOMContentLoaded', () => {
    if (document.querySelector('.verify-container')) {
        initialiseVerifyView();
    }
});