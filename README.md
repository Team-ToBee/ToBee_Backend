# **ToBee Productivity Tool - Backend**

### **Overview**

Welcome to the backend repository for the **ToBee Productivity Tool**, a powerful application designed to help users boost productivity by managing tasks,
focusing with Pomodoro sessions, collaborating on projects, and receiving AI-powered productivity recommendations.

This tool is ideal for:
- **Professionals** managing multiple projects and deadlines.
- **Students** balancing study sessions and other activities.


The backend service is the core of this application, managing users, tasks, collaborations, and AI recommendations, while seamlessly handling API requests, data storage, and third-party integrations.

---

## **Key Features**

### **1. Task Management**
- Users can create, update, and delete tasks.
- Tasks can be prioritized based on deadlines and importance.
- Each task can be associated with a Pomodoro session for better time management.

### **2. Pomodoro Timer**
- Users can initiate Pomodoro sessions to break work into focused intervals followed by short breaks.
- The backend tracks Pomodoro sessions, ensuring users stay productive and avoid burnout.

### **3. Collaboration**
- Users can share tasks and collaborate with others on projects.
- Real-time updates and notifications ensure smooth teamwork and productivity tracking.

### **4. AI-Powered Productivity Recommendations**
- The backend integrates with an AI engine to provide users with personalized productivity tips based on their behavior.
- Suggestions include task prioritization, break timings, and focus improvement.

### **5. Progress Reports**
- Users receive daily or weekly reports detailing their productivity trends, including tasks completed and time spent focusing.
- AI-generated recommendations are also included in the reports to help users improve.

### **6. Reward System**
- Users are rewarded with points and badges for completing tasks and staying focused, encouraging consistent productivity.

---

## **Technology Stack**

- **Framework:** ASP.NET Core  
- **Database:** PostgreSQL / SQLServer   
- **API:** REST API for communication between  mobile app, and backend  
- **ORM:** EntityFrameworkCore 
- **Authentication:** JWT (JSON Web Tokens)  
- **AI Recommendations:** Python / Machine Learning models

---
## **Project Structure**

```plaintext
/backend
│
├── models/             # Database models for users, tasks, sessions, etc.
├── routes/             # API routes for different functionalities (tasks, users, collaboration)
├── controllers/        # Business logic handling API requests and responses
├── services/           # Services such as AI recommendations, notification service, etc.
├── migrations/         # Database migrations for creating and updating tables
├── middlewares/        # Middleware for authentication and request validation
├── config/             # Environment variables and database configurations
├── tests/              # Unit and integration tests
├── package.json        # Node.js dependencies and scripts
└── README.md           # Project overview and setup instructions
```

## Getting Started

### Prerequisites

- .NET 8 SDK installed on your machine.
- SQL Server or a compatible database for storing application data.

### Setup

1. **Clone the Repository**: `git clone https://github.com/Team-ToBee/ToBee_Backend.git`
2. **Database Configuration**: Update the `appsettings.json` files in the API and Identity projects with your database connection strings.
3. **Apply Migrations**: Use the `dotnet ef database update` command in the Persistence and Identity projects to apply the database schema.
4. **Run the Application**: Execute `dotnet run` in the API project directory to start the application.

## Contributing

Contributions are highly appreciated! If you have suggestions for improving the ToBee System, please feel free to fork the repository, make changes, and submit a pull request. You can also open issues for bugs or feature requests.

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details.


