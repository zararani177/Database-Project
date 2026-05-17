# Virtual-Plant-Care-Assistant-Database-Project
# 🌿 Virtual Plant Care Assistant

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![.NET](https://img.shields.io/badge/.NET-6.0%2B-blueviolet)](https://dotnet.microsoft.com/)
[![MySQL](https://img.shields.io/badge/MySQL-8.0-orange)](https://www.mysql.com/)

A modern desktop application built to simplify indoor plant management. This project focuses on providing a seamless user experience for tracking plant health, scheduling maintenance tasks, and maintaining a digital history of your botanical collection.

---

## ✨ Key Features

- **🔐 Secure User Accounts**: Full authentication system with secure login and registration.
- **🪴 Interactive Dashboard**: A centralized view to manage all plants in your collection at a glance.
- **📅 Smart Care Scheduling**: Automated tracking for watering, fertilizing, and repotting tasks.
- **📜 Maintenance History**: Comprehensive logging system to monitor growth trends and health over time.
- **🎨 Modern Botanical UI**: A custom-designed XAML interface featuring a professional botanical aesthetic, responsive layout, and **custom plant image support**.
- **🗄️ Relational Database**: Optimized MySQL schema for persistent data storage and efficient querying.

---

## 🛠️ Built With

- **Frontend**: C# WPF (Windows Presentation Foundation)
- **Backend Logic**: .NET Core / C#
- **Database**: MySQL 8.0
- **UI Design Principles**: HCI (Human-Computer Interaction) focused layout

---

## 🚀 Installation & Setup

### Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/download) (6.0 or higher)
- [MySQL Server](https://dev.mysql.com/downloads/installer/)

### Setup Instructions

1. **Clone the Project**:

   ```bash
   git clone https://github.com/yourusername/VirtualPlantCareAssistant.git
   cd VirtualPlantCareAssistant
   ```
2. **Initialize Database**:

   - Execute the following SQL scripts in your MySQL environment to create the schema and optionally add sample data:
     ```sql
     SOURCE setup_database.sql;
     SOURCE sample_data.sql;  -- Optional: Adds a test user and sample plants
     ```
3. **Configure Connection**:

   - Ensure your database credentials are correctly set in the `DatabaseHelper.cs` file.
4. **Launch Application**:

   ```bash
   dotnet run --project VirtualPlantCareAssistantVSC
   ```
**OR** Press F5 to run the project

---

## 📸 Interface Preview

| Feature                | Preview                                                                          |
| :--------------------- | :------------------------------------------------------------------------------- |
| **Login System**    | <img width="865" height="460" alt="image" src="https://github.com/user-attachments/assets/0ea3cde1-a4c3-4e71-baf8-03fff97b7cb6" />
| **Sign-up**         | <img width="859" height="604" alt="image" src="https://github.com/user-attachments/assets/976824fe-63a3-49bb-a690-16f763406664" /> |
| **Dashboard**       | <img width="1914" height="1007" alt="image" src="https://github.com/user-attachments/assets/58861e3d-6225-473b-a792-802b521cf7b4" /> |
| **Add Plant**       | <img width="900" height="635" alt="image" src="https://github.com/user-attachments/assets/c7cecb7d-c874-4ffc-b43b-1bc8c724f4a9" /> |
| **Care History**    | <img width="900" height="636" alt="image" src="https://github.com/user-attachments/assets/13ba6958-f97d-447d-ba7c-9c65c496f48f" /> |
| **Care Schedule**   | <img width="900" height="631" alt="image" src="https://github.com/user-attachments/assets/6eb685c0-17ee-444f-b8dd-05163740b09e" /> |
| **Reminders**       | <img width="900" height="632" alt="image" src="https://github.com/user-attachments/assets/9ade9ff2-42ca-4d3d-88d3-92d16237f8c4" /> |
| **Delete Plant**    | <img width="900" height="634" alt="image" src="https://github.com/user-attachments/assets/f199eba7-688c-4dd7-b1a9-d8ec2351455e" /> |

---

## 📁 Project Architecture

```text
VirtualPlantCareAssistantVSC/
├── DB VSC Project Images/ # User-uploaded plant images
├── Assets/
│   └── Images/         # UI resources, backgrounds, and icons
├── Models/             # Business logic and data structures
├── Views/              # XAML UI Components and Windows
├── Utilities/          # Database connectivity and Session management
├── Data/               # Data access layer
└── App.xaml            # Global styles and application entry
```

---
**Developed by Zara Rani** 🌿
