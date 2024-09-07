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
    const hoursWorkedInput = document.getElementById('hoursWorked');
    const hourlyRateInput = document.getElementById('hourlyRate');
    const claimAmountInput = document.getElementById('claimAmount');

    // Check if the elements exist before adding event listeners (e.g. the claim amount input is not present on the claim details page)
    if (hoursWorkedInput && hourlyRateInput && claimAmountInput) {
        function calculateClaimAmount() {
            const hoursWorked = parseFloat(hoursWorkedInput.value) || 0;
            const hourlyRate = parseFloat(hourlyRateInput.value) || 0;
            const totalAmount = hoursWorked * hourlyRate;
            claimAmountInput.value = totalAmount.toFixed(2);
        }
        // Add event listeners to the hours worked and hourly rate inputs to calculate the claim amount when the user inputs values
        hoursWorkedInput.addEventListener('input', calculateClaimAmount);
        hourlyRateInput.addEventListener('input', calculateClaimAmount);
    }
});