# Contract Monthly Claim System (CMCS) - Part 2

## Overview

The Contract Monthly Claim System (CMCS) helps independent contractor lecturers submit and approve their monthly claims more easily. This system makes claim management more efficient and transparent.

### Languages and Frameworks
<p align="left">
<a href="#"><img alt="C#" src="https://img.shields.io/badge/c%23-%23239120.svg?style=for-the-badge&logo=c-sharp&logoColor=white"></a>
<a href="#"><img alt="ASP.NET Core" src="https://img.shields.io/badge/ASP.NET%20Core-%235C2D91.svg?style=for-the-badge&logo=.net&logoColor=white"></a>
<a href="#"><img alt="HTML5" src="https://img.shields.io/badge/html5-%23E34F26.svg?style=for-the-badge&logo=html5&logoColor=white"></a>
<a href="#"><img alt="CSS3" src="https://img.shields.io/badge/css3-%231572B6.svg?style=for-the-badge&logo=css3&logoColor=white"></a>
<a href="#"><img alt="JavaScript" src="https://img.shields.io/badge/javascript-%23F7DF1E.svg?style=for-the-badge&logo=javascript&logoColor=black"></a>
</p>

### Frameworks and Libraries
<p align="left">
<a href="#"><img alt="Bootstrap" src="https://img.shields.io/badge/bootstrap-%237952B3.svg?style=for-the-badge&logo=bootstrap&logoColor=white"></a>
<a href="#"><img alt="Entity Framework Core" src="https://img.shields.io/badge/Entity%20Framework%20Core-%23512BD4.svg?style=for-the-badge&logo=.net&logoColor=white"></a>
<a href="#"><img alt="Font Awesome" src="https://img.shields.io/badge/Font%20Awesome-%23528DD7.svg?style=for-the-badge&logo=font-awesome&logoColor=white"></a>
<a href="#"><img alt="Google Fonts" src="https://img.shields.io/badge/Google%20Fonts-%234285F4.svg?style=for-the-badge&logo=google-fonts&logoColor=white"></a>
</p>

### IDE/Editors
<p align="left">
<a href="#"><img alt="Visual Studio" src="https://img.shields.io/badge/Visual%20Studio-%235C2D91.svg?style=for-the-badge&logo=visual-studio&logoColor=white"></a>


## Recent Changes and Improvements

1. Implemented full CRUD functionality for claims and user management.
2. Added unit tests to ensure reliability and maintainability.
3. Integrated a local database using Entity Framework Core.
4. Implemented a DatabaseSeeder for the initial data population.
5. Enhanced user interface with responsive design and improved user experience.
6. Added role-based access control for different user types.
7. Implemented file upload functionality for supporting documents.
8. Added client-side and server-side validation for data integrity.

## Installation and Setup

### Prerequisites
- 🖥 Visual Studio 2022 or later.
- ⚙️ .NET Core 6.0 SDK or later.
- 🗄 SQL Server (LocalDB or Express).

### Steps to Run the MVC Application
1. Clone the repository: `git clone https://github.com/GerryHorror/CMCS.git `
2. Open the solution in Visual Studio.
3. Right-click on the `Solution` folder and click on `Restore NuGet Packages`
4. Navigate to the Package Manager Console `Tools` > `NuGet Package Manager` > `Package Manager Console`
5. Run the following commands:
   `Add-Migration Recreate`
   `Update-Database`
6. Build the solution (Ctrl + Shift + B)
7. Run the application (F5)

### Running Unit Tests
1. In Visual Studio, go to Test > Run All Tests.
2. Alternatively, use the Test Explorer to run specific tests.

## Database Implementation

The application uses a local SQL Server database implemented with Entity Framework Core. The `DatabaseSeeder` class in `DatabaseSeeder.cs` populates initial data.

*Note:** I have included `appsettings.json` in the repo to simplify setting up the connection string. This is not considered good programming practice because appsettings.json often contains sensitive configuration details, such as database connection strings, API keys, and other environment-specific settings. Exposing this file could lead to security vulnerabilities in a real-world scenario. However, for simplicity during development and to make the setup process easier, I have included it here. These settings should be managed securely in a production environment using tools like Azure Key Vault, AWS Secrets Manager, or environment variables.

### How DatabaseSeeder Works
- It checks if the database is empty.
- If empty, it seeds roles, claim statuses, and sample users.
- This ensures the application has the necessary data to function on the first run.

### Seeded Users
For demonstration purposes, the following users are seeded:

1. Lecturer:
- Username: sipho.nkosi
- Password: password
2. Coordinator:
- Username: fatima.patel
- Password: password
3. Manager:
- Username: johan.vandermerwe
- Password: password

*Note:** In a real-world scenario, these credentials would not be publicly shared, and users would be required to change their passwords upon first login.

## Features

- User authentication with role-based access control
- Claim submission for lecturers
- Claim verification for coordinators and managers
- User profile management
- Document upload functionality
- Responsive design for various devices

## Project Structure

- `Controllers/`: Houses MVC controllers
- `Models/`: Contains data model definitions
- `Views/`: Stores Razor view files
- `wwwroot/`: Holds static files (CSS, JavaScript, images)

## References
- [Unit testing C# with MSTest and .NET](https://learn.microsoft.com/en-us/dotnet/core/testing/unit-testing-with-mstest)
- [Testing EF Core Applications](https://learn.microsoft.com/en-us/ef/core/testing/)
- [Moq](https://github.com/devlooped/moq/wiki/Quickstart)
- [Seed Data in Entity Framework Core](https://dotnettutorials.net/lesson/seed-data-in-entity-framework-core/)
- [Unit testing best practices with .NET Core and .NET Standard](https://learn.microsoft.com/en-us/dotnet/core/testing/unit-testing-best-practices#mocking-dependencies)
- [HTML elements reference](https://developer.mozilla.org/en-US/docs/Web/HTML/Element)
- [BootStrap Documentation](https://getbootstrap.com/docs/5.0/getting-started/introduction/)
- [Font Awesome Icons](https://fontawesome.com/v5.15/icons?d=gallery&p=2&m=free)
- [Google Fonts -  Open Sans](https://fonts.google.com/specimen/Open+Sans?query=open+sans)
- [Google Fonts -  Roboto](https://fonts.google.com/specimen/Roboto?query=roboto)
- [Session and state management in ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/app-state?view=aspnetcore-5.0)
- [Razor syntax reference for ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/mvc/views/razor?view=aspnetcore-5.0)
- [Basic concepts of flexbox](https://developer.mozilla.org/en-US/docs/Web/CSS/CSS_Flexible_Box_Layout/Basic_Concepts_of_Flexbox))
- [CSS grid layout](https://developer.mozilla.org/en-US/docs/Web/CSS/CSS_Grid_Layout)
- [CSS selectors](https://developer.mozilla.org/en-US/docs/Web/CSS/CSS_Selectors)
- [The box model](https://developer.mozilla.org/en-US/docs/Learn/CSS/Building_blocks/The_box_model)
- [z-index](https://developer.mozilla.org/en-US/docs/Web/CSS/z-index)
- [background](https://developer.mozilla.org/en-US/docs/Web/CSS/background)
- [Using media queries](https://developer.mozilla.org/en-US/docs/Web/CSS/Media_Queries/Using_media_queries)
- [Using CSS animations](https://developer.mozilla.org/en-US/docs/Web/CSS/CSS_Animations/Using_CSS_animations)
- [position](https://developer.mozilla.org/en-US/docs/Web/CSS/position)
- [jQuery Validation Plugin](https://jqueryvalidation.org/)
- [jQuery](https://api.jquery.com/)
- [JavaScript](https://developer.mozilla.org/en-US/docs/Web/JavaScript)
- [Document: querySelector() method](https://developer.mozilla.org/en-US/docs/Web/API/Document/querySelector)
- [Document: querySelectorAll() method](https://developer.mozilla.org/en-US/docs/Web/API/Document/querySelectorAll)
- [Document: getElementById() method](https://developer.mozilla.org/en-US/docs/Web/API/Document/getElementById)
- [EventTarget: addEventListener() method](https://developer.mozilla.org/en-US/docs/Web/API/EventTarget/addEventListener)
- [Element: innerHTML property](https://developer.mozilla.org/en-US/docs/Web/API/Element/innerHTML)
- [HTMLElement: style property](https://developer.mozilla.org/en-US/docs/Web/API/HTMLElement/style)
- [HTMLElement: classList property](https://developer.mozilla.org/en-US/docs/Web/API/Element/classList)
- [HTMLElement: closest() method](https://developer.mozilla.org/en-US/docs/Web/API/Element/closest)
- [setTimeout() global function](https://developer.mozilla.org/en-US/docs/Web/API/WindowOrWorkerGlobalScope/setTimeout)
- [Event: preventDefault() method](https://developer.mozilla.org/en-US/docs/Web/API/Event/preventDefault)
- [HTMLFormElement: reset() method](https://developer.mozilla.org/en-US/docs/Web/API/HTMLFormElement/reset)
- [HTMLInputElement: value property](https://developer.mozilla.org/en-US/docs/Web/API/HTMLInputElement/value)
- [HTMLFormElement: submit() method](https://developer.mozilla.org/en-US/docs/Web/API/HTMLFormElement/submit)
- [HTMLTableElement: insertRow() method](https://developer.mozilla.org/en-US/docs/Web/API/HTMLTableElement/insertRow)
- [Element: insertAdjacentHTML() method](https://developer.mozilla.org/en-US/docs/Web/API/Element/insertAdjacentHTML)
- [Overview of ASP.NET Core MVC](https://learn.microsoft.com/en-us/aspnet/core/mvc/overview?view=aspnetcore-8.0)
- [Protection of Personal Information Act 4 of 2013](https://www.gov.za/documents/protection-personal-information-act)
