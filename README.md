
# 🛡️ Smart Tourist Safety Monitoring & Incident Response System

### AI-Powered Tourist Protection | Geo-Fencing | Digital Identity | Emergency Response

A web-based tourist safety monitoring system developed using **C# and ASP.NET Core MVC**. The application helps monitor tourist locations, manage emergency reports, detect geo-fencing violations, and verify digital identities.

---

## 🌟 Project Overview

Tourist safety is an important concern in the travel and tourism industry. This project provides a centralized platform to improve tourist safety through location monitoring, emergency reporting, virtual safety zones, and blockchain-inspired digital identity verification.

The system provides an interactive dashboard where administrators can monitor tourist activities and respond to safety incidents.

---

## 🚀 Key Features

### 1. 👤 Tourist Registration
- Register new tourists.
- Generate a unique Tourist ID.
- Create a digital identity record.
- Generate SHA-256 identity hashes.

### 2. 📊 Safety Dashboard
- View total registered tourists.
- Monitor emergency reports.
- Track pending incidents.
- View geo-fencing alerts.
- Monitor digital identities.
- Automatically refresh dashboard information.

### 3. 🆘 SOS Emergency System
- Submit emergency reports.
- Generate emergency notifications.
- Display incident information.
- Resolve reported emergencies.

### 4. 📍 Live Location Tracking
- Access browser-based location services.
- Capture tourist latitude and longitude.
- Store location history.
- Display tourist locations on an interactive map.

### 5. 🗺️ Geo-Fencing System
- Create virtual safety zones.
- Define zone names and geographical coordinates.
- Calculate distances using the Haversine formula.
- Detect when tourists move outside designated safety zones.
- Generate geo-fencing alerts.

### 6. 🔐 Digital Identity Verification
- Generate SHA-256 identity hashes.
- Maintain a previous-hash reference.
- Create a GENESIS starting record.
- Verify identity data integrity.
- Demonstrate tampering detection.

### 7. 🔔 Notification System
- Real-time dashboard notifications.
- SOS emergency notifications.
- Geo-fencing alert notifications.
- Optional alert sounds.

---

## 🛠️ Technologies Used

| Technology | Purpose |
|---|---|
| C# | Backend programming |
| ASP.NET Core MVC | Web application framework |
| .NET 10 | Application platform |
| Entity Framework Core | Database operations |
| SQLite | Database |
| HTML5 | Web structure |
| CSS3 | Styling |
| JavaScript | Interactive functionality |
| Leaflet.js | Interactive maps |
| SHA-256 | Digital identity integrity |
| Docker | Application containerization |
| Render | Cloud deployment |

---

## 🏗️ Project Architecture

```text
                 TOURIST SAFETY SYSTEM
                           |
                  ASP.NET CORE MVC
                           |
          +----------------+----------------+
          |                |                |
      Controllers       Models            Views
          |                |                |
          +----------------+----------------+
                           |
                Entity Framework Core
                           |
                        SQLite
                           |
          +----------------+----------------+
          |                |                |
      Registration     Emergency         Location
          |                |                |
      Digital ID       SOS Alerts       Geo-Fencing
          |
      SHA-256 Hash
          |
    Identity Verification
```

---

## 📂 Project Structure

```text
TouristSafetySystem/
│
├── Controllers/
├── Models/
├── Data/
├── Views/
├── wwwroot/
│
├── Migrations/
├── Program.cs
├── appsettings.json
├── TouristSafetySystem.csproj
├── Dockerfile
├── .dockerignore
└── README.md
```

---

## 💻 Installation and Setup

### Prerequisites

- .NET 10 SDK
- Visual Studio Code or Visual Studio
- Git
- Docker (optional)

### Step 1: Clone the Repository

```bash
git clone https://github.com/chandramoulikandregula2468-ctrl/CSE253---Dotnet-Programming.git
```

### Step 2: Navigate to the Project

```bash
cd CSE253---Dotnet-Programming
```

### Step 3: Restore Dependencies

```bash
dotnet restore
```

### Step 4: Run the Application

```bash
dotnet run
```

### Step 5: Open in Browser

Open the localhost URL displayed in your terminal.

Example:

```text
http://localhost:5277
```

---

## 🐳 Docker Deployment

This project supports Docker-based deployment.

### Build Docker Image

```bash
docker build -t tourist-safety-system .
```

### Run Docker Container

```bash
docker run -p 10000:10000 -e PORT=10000 tourist-safety-system
```

Open:

```text
http://localhost:10000
```

---

## ☁️ Deploy on Render

1. Push your project to GitHub.
2. Open https://render.com/
3. Select **New → Web Service**.
4. Connect your GitHub repository.
5. Select Docker as the runtime.
6. Set Dockerfile Path to `./Dockerfile`.
7. Configure persistent storage at `/var/data` for SQLite.
8. Deploy the application.

---

## 🔒 Security and Data Integrity

The project demonstrates several security-related concepts:

- Unique tourist identification.
- SHA-256 hashing.
- Hash-chain-inspired digital identity records.
- Identity integrity verification.
- Emergency incident monitoring.
- Geographical safety monitoring.

**Note:** The digital identity implementation is a blockchain-inspired hash-chain demonstration, not a distributed blockchain network.

---

## 🎯 Project Objectives

- Improve tourist safety monitoring.
- Provide a centralized emergency response platform.
- Track tourist geographical locations.
- Identify tourists outside designated safety zones.
- Demonstrate digital identity integrity.
- Apply C# and ASP.NET Core concepts in a real-world application.

---

## 🔮 Future Enhancements

- AI-based tourist risk prediction.
- Automated emergency notifications.
- Mobile application integration.
- Multi-language support.
- Advanced tourist activity analytics.
- Integration with government emergency services.
- Distributed blockchain-based identity verification.

---

## 👨‍💻 Developer

**Chandramouli Kandregula**

B.Tech – Computer Science and Engineering  
Specialization: Artificial Intelligence and Robotics

**GitHub:**  
https://github.com/chandramoulikandregula2468-ctrl

---

## 📜 License

This project is developed for academic and educational purposes as part of the CSE253 – Dotnet Programming course.

---

⭐ If you find this project interesting, consider giving the repository a star!
