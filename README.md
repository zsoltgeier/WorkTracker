# WorkTracker WPF Application

WorkTracker is a Windows Presentation Foundation (WPF) application designed to help you manage employee activities efficiently. This project provides a user-friendly interface for tracking and summarizing work-related activities. It follows the Model-View-ViewModel (MVVM) architecture to separate concerns and enhance maintainability.

## Features

- **Employee Management**: Easily add and manage employees.
- **Activity Logging**: Log work activities with details such as title, description, duration, and comments.
- **Activity Summaries**: View and export summaries of logged activities.
- **User-Friendly UI**: A clean and intuitive user interface for seamless interaction.

## Technologies Used

- C# and .NET Framework for application logic.
- Entity Framework Core for database interaction.
- CSV export functionality using CsvHelper.
- MVVM architecture for clean separation of concerns.
- Microsoft Toolkit MVVM framework for data binding and commanding.

## Getting Started

To get started with the WorkTracker application, follow these steps:

### **Clone the Repository**:

```bash
git clone https://github.com/zsoltgeier/WorkTracker.git
```

### **Build the Project**:
Open the solution in Visual Studio and build the project.
### **Run Database Migrations**:
Before running the application, execute the following command in the Package Manager Console to apply database migrations:

```bash
Update-Database
```

This will set up the database schema required for the application.

### **Run the Application**:
Start the application, and you'll be greeted with a user-friendly interface for managing employees and logging activities.

## Usage

- **Add Employees**: Use the "Dolgozó felvétele" tab to add employees to the database.
- **Log Activities**: Log work activities in the "Munkavégzés rögzítése" tab, providing details about the work done.
- **View Summaries**: Check the "Összegzés" tab to view summaries of activities for selected employees and export data to a CSV file.
