Virtual-Plant-Care-Assistant-Database-Project
🌿 Virtual Plant Care Assistant
License: MIT .NET MySQL

A modern desktop application built to simplify indoor plant management. This project focuses on providing a seamless user experience for tracking plant health, scheduling maintenance tasks, and maintaining a digital history of your botanical collection.

✨ Key Features
🔐 Secure User Accounts: Full authentication system with secure login and registration.
🪴 Interactive Dashboard: A centralized view to manage all plants in your collection at a glance.
📅 Smart Care Scheduling: Automated tracking for watering, fertilizing, and repotting tasks.
📜 Maintenance History: Comprehensive logging system to monitor growth trends and health over time.
🎨 Modern Botanical UI: A custom-designed XAML interface featuring a professional botanical aesthetic, responsive layout, and custom plant image support.
🗄️ Relational Database: Optimized MySQL schema for persistent data storage and efficient querying.
🛠️ Built With
Frontend: C# WPF (Windows Presentation Foundation)
Backend Logic: .NET Core / C#
Database: MySQL 8.0
UI Design Principles: HCI (Human-Computer Interaction) focused layout
🚀 Installation & Setup
Prerequisites
.NET SDK (6.0 or higher)
MySQL Server
Setup Instructions
Clone the Project:

git clone https://github.com/yourusername/VirtualPlantCareAssistant.git
cd VirtualPlantCareAssistant
Initialize Database:

Execute the following SQL scripts in your MySQL environment to create the schema and optionally add sample data:
SOURCE setup_database.sql;
SOURCE sample_data.sql;  -- Optional: Adds a test user and sample plants
Configure Connection:

Ensure your database credentials are correctly set in the DatabaseHelper.cs file.
Launch Application:

dotnet run --project VirtualPlantCareAssistantVSC
OR Press F5 to run the project

📸 Interface Preview
Feature	Preview
Login System	image
Sign-up	image
Dashboard	image
Add Plant	image
Care History	image
Care Schedule	image
Reminders	image
Delete Plant	image
📁 Project Architecture
VirtualPlantCareAssistantVSC/
├── DB VSC Project Images/ # User-uploaded plant images
├── Assets/
│   └── Images/         # UI resources, backgrounds, and icons
├── Models/             # Business logic and data structures
├── Views/              # XAML UI Components and Windows
├── Utilities/          # Database connectivity and Session management
├── Data/               # Data access layer
└── App.xaml            # Global styles and application entry
Developed by Zaneera Bint E Zahid 🌿
