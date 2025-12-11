# 🚀 TechChallenge Employee API

This project is part of the TechChallenge and implements a RESTful API for employee information management. The architecture adheres to **Clean Architecture** principles, utilizing the **CQRS** (Command Query Responsibility Segregation) pattern and **MediatR** for decoupling and processing commands and queries.

---

## 🏗️ Architecture and Technology

The project is structured in layers to ensure separation of concerns, testability, and maintainability.

### Core Technologies

* **Backend Language:** C# (.NET Core / .NET 8)
* **Frontend Framework:** Angular (TypeScript)
* **Messaging Pattern:** **MediatR** (CQRS Implementation)
* **Validation:** **FluentValidation** (used in MediatR *Pipeline Behaviors*)
* **Authentication:** JWT (JSON Web Token)
* **Cryptography:** AES-256 (for sensitive data like `DocumentNumber`)
* **Password Security:** **Salt and Hashing** (SHA256)
* **Persistence:** PostgreSQL
* **ORM:** Entity Framework Core

### Project Structure

| Component | Layer | Description |
| :--- | :--- | :--- |
| **Backend API** | `TechChallenge.Api` | Entry point (Controllers, injection, *middlewares*). Handles authentication and exposes employee management endpoints. |
| `TechChallenge.Application` | CQRS Handlers, Validation, and *Pipeline Behaviors*. |
| `TechChallenge.Domain` | Business Rules (Entities, Interfaces). |
| `TechChallenge.Infra` | Data Access (Entity Framework Core, Repositories) and external services. |
| **Frontend** | `frontend/` | Angular application responsible for the user interface, routing, and communication with the API. Includes pages for Login, Employee List, and Employee Detail. |

---

## 📦 Implemented Functionalities (CQRS)

Employee management functionalities are divided into Commands (write operations) and Queries (read operations).

### Commands (Write Operations)

| Command | Description | Key Validations |
| :--- | :--- | :--- |
| `CreateEmployeeCommand` | Creates a new employee in the system. | CPF/CNPJ duplicated, Email duplicated, Minimum Age (18+), Password Rules. **Saves password securely using Salt and Hash.** |
| `UpdateEmployeeCommand` | Updates an existing employee's registration data. | Valid Employee ID, Duplicate Email (excluding their own), Minimum Age (18+). |
| `DeleteEmployeeCommand` | Removes an employee from the system. | Valid and Existing Employee ID. |
| `LoginCommand` | Authenticates the employee via email and password. | Required fields (Email, Password). **Compares provided password against stored Salt and Hash.** |

### Queries (Read Operations)

| Query | Description | Authorization | Data Handling |
| :--- | :--- | :--- | :--- |
| `GetAllCommand` | Returns a list of all employees. | Implements authorization rules in the `DomainService`. | Maps all properties, including phones and manager's name. |
| `GetEmployeeByIdCommand` | Returns details of a specific employee by ID. | Implements access validation in the `DomainService`. | **Decrypts** the `DocumentNumber` (CPF/CNPJ) from the database before returning it to the client. |

---

## 🔑 Authentication, Authorization, and Security

### Password Storage Security

For security, passwords are not stored in plaintext. When an employee is created or logs in:

1.  **Creation (`CreateEmployeeCommand`):** The plain password is combined with a unique **Salt**, and then a strong **Hashing algorithm** (SHA256) is applied. Only the resulting Hash and the unique Salt are stored in the database.
2.  **Login (`LoginCommand`):** The provided password is combined with the stored Salt from the database, hashed using the same algorithm, and the resulting hash is compared against the stored Hash.

### JWT and Access Roles (`AuthRole`)

The system uses **JWT (JSON Web Tokens)**. The user's Role (`EmployeeRoleType`) is critical for security.

### Sensitive Data Encryption

The `DocumentNumber` is stored encrypted in the database using **AES-256**.

---

## 🛠️ How to Set Up and Run the Project

Follow the steps below to set up and start the entire application environment (Backend API, Frontend, and PostgreSQL database).

### Prerequisites

* [.NET SDK 8.0 or higher](https://dotnet.microsoft.com/download) (Required only for running tests or applying migrations locally)
* [Git](https://git-scm.com/)
* [Docker](https://www.docker.com/) and [Docker Compose](https://docs.docker.com/compose/)

### Installation

1.  **Clone the repository:**
    ```bash
    git clone [Repository URL]
    cd TechChallenge
    ```

2.  **Restore dependencies** (Backend):
    ```bash
    dotnet restore
    ```
    
3.  **Frontend Dependencies** (The Docker build should handle this, but for local development):
    ```bash
    cd frontend
    npm install # or yarn install
    cd ..
    ```

4.  **Database Configuration and Seeding:**
    * **Run Migrations and Seed Data:** Before running the entire environment for the first time, you must apply database migrations.
        ```bash
        # Ensure only the database container is started first to run migrations:
        docker-compose up -d postgres
        
        # Apply EF Core Migrations (requires .NET SDK locally):
        dotnet ef database update --project TechChallenge.Infra
        
        # Stop the database container if you plan to run everything together next:
        # docker-compose down
        ```
    
    > **Initial Admin Access:**
    > If no user with the **Admin** role exists, the application will automatically create a default Admin user during the migration/seeding process.
    >
    > * **Email:** `admin@company.com`
    > * **Password:** `Admin@123`
    >

### Running the Entire Environment (Recommended)

Use the following command to build the Docker images (if necessary) and start the Backend API, Frontend, and PostgreSQL database simultaneously in the background.

```bash
docker-compose up --build -d
```

---

## 🧪 Unit Tests

All validators, Handlers, and Queries have complete coverage using **xUnit**, **Moq**, and **Bogus**.

To run the tests:

```bash
dotnet test