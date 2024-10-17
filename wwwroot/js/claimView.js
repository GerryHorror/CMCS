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

document.addEventListener('DOMContentLoaded', () => {
    initialiseClaimView();
});