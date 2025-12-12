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
|| `TechChallenge.Application` | CQRS Handlers, Validation, and *Pipeline Behaviors*. |
|| `TechChallenge.Domain` | Business Rules (Entities, Interfaces). |
|| `TechChallenge.Infra` | Data Access (Entity Framework Core, Repositories) and external services. |
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

For security, passwords are not stored in plaintext. The system uses **BCrypt**, an industry-standard hashing algorithm specifically designed for password storage.

**Why BCrypt?**
- **Built-in Salt:** Automatically generates and manages unique salts for each password
- **Adaptive:** Configurable work factor (cost) that can be increased as hardware improves
- **Slow by Design:** Deliberately slower hashing makes brute-force attacks impractical
- **Industry Standard:** Widely adopted and battle-tested security practice

**How it works:**

1.  **Creation (`CreateEmployeeCommand`):** 
    - The plain password is hashed using BCrypt with a work factor of 12
    - BCrypt automatically generates a unique salt and incorporates it into the hash
    - Only the resulting BCrypt hash (which includes the salt) is stored in the database
    - Format: `$2a$12$[salt][hash]` (60 characters)

2.  **Login (`LoginCommand`):** 
    - The provided password is verified against the stored BCrypt hash
    - BCrypt extracts the salt from the stored hash and applies it to the provided password
    - If the resulting hash matches the stored hash, authentication succeeds

**Example:**
```csharp
// Hashing a password (during registration)
string hashedPassword = BCrypt.Net.BCrypt.HashPassword(plainPassword);

// Verifying a password (during login)
bool isValid = BCrypt.Net.BCrypt.Verify(plainPassword, storedHash);
```

### JWT and Access Roles (`AuthRole`)

The system uses **JWT (JSON Web Tokens)**. The user's Role (`EmployeeRoleType`) is critical for security.

### Sensitive Data Encryption

The `DocumentNumber` (CPF/CNPJ) is protected using a dual-layer approach:

1. **AES-256 Encryption:** The document number is encrypted before storage, ensuring data privacy and LGPD/GDPR compliance
2. **SHA-256 Hash Index:** A separate hash of the document number is stored for efficient duplicate detection without exposing the original data

**Benefits of this approach:**
- **Performance:** Fast duplicate checks using the hash index without decrypting all records
- **Security:** Original document numbers remain encrypted at rest
- **Compliance:** Meets data protection regulations (LGPD/GDPR)
- **Reversibility:** Document numbers can be decrypted when needed for display
```csharp
// Storage structure
public class EmployeeEntity
{
    public string DocumentNumber { get; set; }     // AES-256 encrypted
    public string DocumentNumberIndex { get; set; }  // SHA-256 hash for searches
}
```

---

## 🛠️ How to Set Up and Run the Project

Follow the steps below to set up and start the entire application environment (Backend API, Frontend, and PostgreSQL database).

### Prerequisites

* [.NET SDK 8.0 or higher](https://dotnet.microsoft.com/download) (Required only for running tests or applying migrations locally)
* [Git](https://git-scm.com/)
* [Docker](https://www.docker.com/) and [Docker Compose](https://docs.docker.com/compose/)

### Installation

1.  **Clone the repository:**
    ```bash
    git clone https://github.com/DouglasEBauler/tech-challenge.git
    cd tech-challenge
    ```

2.  **Configure Environment Variables:**
    * Create a `.env` file in the root directory:
        ```bash
        POSTGRES_USER=postgres
        POSTGRES_PASSWORD=postgres
        POSTGRES_DB=techChallenge
        ```
3.  **Frontend Dependencies** (Optional, handled by Docker, but for local use):
    * **Navigate to the Frontend directory:**
        ```bash
        cd frontend
        ```
    * **Install dependencies:**
        ```bash
        npm install # or yarn install
        cd .. # Return to the root directory
        ```

4.  **Database Configuration and Seeding:**
    * **Run Migrations and Seed Data:** Before running the entire environment for the first time, you must apply database migrations.
        ```bash
        # Ensure only the database container is started first to run migrations:
        docker-compose up -d postgres_db
        
        # Apply EF Core Migrations (requires .NET SDK locally):
        dotnet ef database update --project backend/src/TechChallenge.Api
        ```
    
    > **Initial Admin Access:**
    > If no user with the **Admin** role exists, the application will automatically create a default Admin user during the migration/seeding process.
    >
    > * **Email:** `admin@company.com`
    > * **Password:** `Admin@123`
    >

### Container postgres_db vs tech_pg

If you created a container manually named `tech_pg`, stop and remove it before using docker-compose:
```bash
docker stop tech_pg
docker rm tech_pg
docker-compose up -d
```

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