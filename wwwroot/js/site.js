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