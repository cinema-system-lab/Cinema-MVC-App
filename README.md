🎬 Cinema MVC Application

<img width="88" height="20" alt="image" src="https://github.com/user-attachments/assets/f0d7568d-32ce-4b85-8913-0907addfb8eb" />

<img width="98" height="20" alt="image" src="https://github.com/user-attachments/assets/20ce9ce3-18bc-449d-a7d0-d655015e0fc6" />

<img width="62" height="20" alt="image" src="https://github.com/user-attachments/assets/1c741648-4843-4ff1-8d83-feaf0e9b9f13" />

<img width="82" height="20" alt="image" src="https://github.com/user-attachments/assets/a0e6d4d6-95d6-4c87-b0d2-ce6569b15477" />

Cinema MVC Application is a web-based cinema management and ticket booking system built with ASP.NET Core MVC (.NET 8).
The project follows Clean Architecture (Onion Architecture) principles and focuses on scalability, maintainability, and clear separation of responsibilities.

🔴 Live Demo: https://cinema-mvc-app.azurewebsites.net/

📌 Overview

The application allows users to browse movies, select sessions, choose seats, and book tickets, while administrators can fully manage movies, halls, sessions, and bookings through a dedicated admin panel.

The system is integrated with:

Azure SQL Database for data storage

TMDB API for automated movie data import

✨ Features
👤 User Functionality

Movie catalog with detailed information

Session selection with timezone-aware date and time display

Interactive seat map (Standard / VIP / IMAX halls)

Secure ticket access after successful payment

Order history with filtering by date and status

QR code generation for purchased tickets

⚙️ Administrator Functionality

Movie management (create, update, delete)

TMDB API integration for fetching movie data

Cinema hall configuration with flexible seat layouts

Session scheduling and hall assignment

Booking and payment status monitoring

Basic financial and booking reports

🏗️ Architecture

The solution is structured according to Clean Architecture principles and consists of three main layers:

📂 Core

Domain entities

DTOs

Enums

Service interfaces

Validators (FluentValidation)

This layer has no dependencies on frameworks or external libraries.

📂 Infrastructure

Entity Framework Core (data access)

Database migrations

External service integrations (TMDB, payment services)

Configuration and persistence logic

📂 MVC-CinemaApp

ASP.NET Core MVC project

Controllers and Razor Views

ViewModels

Client-side assets (JavaScript, CSS)

🛠️ Technology Stack
Area	Technologies
Backend	C#, .NET 8, ASP.NET Core MVC
Database	MS SQL Server, Azure SQL
ORM	Entity Framework Core
Frontend	Razor Views, JavaScript (ES6+), Bootstrap 5, AJAX
DevOps	Azure App Service, Docker, GitHub Actions
Tools	Rider, Visual Studio, Jira, Discord
🚀 Getting Started
1. Clone the repository
git clone https://github.com/your-username/Cinema-MVC-App.git

2. Configure the database

Update the DefaultConnection string in
appsettings.Development.json with your local or Azure SQL credentials.

3. Apply migrations
dotnet ef database update --project Infrastructure --startup-project MVC-CinemaApp

4. Run the application
dotnet run --project MVC-CinemaApp

👥 Team

The project was developed as a team effort by:

Nizar — Team Lead

Artem

Tetiana

Stepan

Maksym

Bohdan

📄 License

This project is licensed under the MIT License.
