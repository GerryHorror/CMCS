# Contract Monthly Claim System (CMCS) - Part 1

## Overview

The Contract Monthly Claim System (CMCS) helps independent contractor lecturers submit and approve their monthly claims more easily. This system makes claim management more efficient and transparent.

This is Part 1 of the CMCS development project. The main focus is on design, planning, and prototyping. I have included some basic features to give users a sense of the web app, even though it doesn't have full functionality yet. By including these basic functions, I can gather early feedback, make potential improvements, and make sure that the final product meets user expectations and needs. These features are not fully developed and are only meant to show the potential of the full system in the next development phases.

## Features

- User authentication with role-based access control
- Claim submission interface for lecturers
- Claim verification tools for coordinators and managers
- User profile management system
- Document upload functionality
- Responsive design for various devices

## Technology Stack

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


## Project Structure

- `Controllers/`: Houses MVC controllers
- `Models/`: Contains data model definitions
- `Views/`: Stores Razor view files
- `wwwroot/`: Holds static files (CSS, JavaScript, images)

## Setup and Installation

1. Clone the repository
2. Ensure .NET Core SDK is installed
3. Navigate to the project directory
4. Run `dotnet restore` to install dependencies
5. Execute `dotnet run` to launch the application

## Database Schema

The application utilises the following main entities:
- User
- Claim
- Document
- Role
- ClaimStatus

Please refer to the UML diagram in the documentation for detailed relationships.

## Key Design Decisions

- MVC architecture for clear separation of concerns
- Responsive UI design implemented with Bootstrap
- Role-based access control for enhanced security
- Modular code structure to improve maintainability

## References
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