# ASP.NET Backend + WPF Frontend

A robust, distributed desktop application architecture consisting of a **WPF (Windows Presentation Foundation)** client and an **ASP.NET Core Web API** backend. 

This project was built to demonstrate enterprise-level .NET engineering practices, strictly separating the user interface (Desktop Client) from business logic and data persistence (RESTful API).

## 🧠 What I Learned & Implemented

This architecture was designed to showcase modern C# engineering standards. Key technical milestones include:

* **Distributed System Architecture:** Decoupled the desktop client from direct database access. The WPF app communicates exclusively via asynchronous HTTP requests to a secure ASP.NET Core REST API.
* **MVVM Pattern Mastery (WPF):** Strictly adhered to the Model-View-ViewModel pattern in the desktop application to ensure total separation of concerns, testability, and UI responsiveness. Implemented `INotifyPropertyChanged` and custom `ICommand` interfaces.
* **RESTful API Design (ASP.NET):** Built a scalable backend using ASP.NET Core controllers, handling routing, status codes, and JSON serialization.
* **Asynchronous Programming:** Extensive use of `async/await` and `Task` throughout the entire data flow (from UI button clicks to database queries) to prevent UI thread blocking.
* **Data Access (Entity Framework Core):** Implemented EF Core for database operations using the Code-First approach, managing migrations and relationships cleanly.
* **Dependency Injection:** Utilized the built-in .NET IoC container in the API for injecting database contexts and services.

## 🛠 Technologies & Architecture

**Frontend (Client):**
* **Framework:** WPF (.NET 10.0)
* **Language:** C#
* **Architecture:** MVVM (Model-View-ViewModel)
* **Libraries:** `Newtonsoft.Json` / `System.Text.Json`, `HttpClient`

**Backend (API):**
* **Framework:** ASP.NET Core Web API
* **Language:** C#
* **ORM:** Entity Framework Core
* **Database:** MariaDB
* **Documentation:** Swagger / OpenAPI

## 🚀 Quick Start (Developer Experience)

The solution is divided into two main projects. To run the application locally, you must start the API backend first, followed by the WPF desktop client.

### Prerequisites
* IDE (with .NET desktop development and ASP.NET workloads)
* .NET 10.0 SDK
* A local SQL server running

### 1. Setup & Run the ASP.NET Core API
1. Clone the repository: `git clone https://github.com/JUSICK/ASP.NET-WPF.git`
2. Open the `.slnx` (Solution) file in your IDE.
3. Set the **CsFullStackApp** project as the Startup Project.
4. Open `appsettings.json` and ensure the `MariaDbConnection` points to your local database instance. (IF you are opening using CLI - dotnet restore for installing NuGet packages)
5. Open the Package Manager Console and update the database (FOR Visual Studio):
   ```powershell
   Update-Database
   ```
5. Open the CLI and update the database (FOR Rider JetBrains):
   ```powershell
   cd .\MyFullStackAppApi\
   dotnet ef database update
   ```
<img width="492" height="242" alt="Screenshot 2026-06-06 110353" src="https://github.com/user-attachments/assets/369ca066-0874-4d4d-81f6-14b92dbe69cd" />

<img width="1876" height="989" alt="Screenshot 2026-06-07 002641" src="https://github.com/user-attachments/assets/dec5697c-93b1-4e7d-8944-c033fe5f27dc" />
