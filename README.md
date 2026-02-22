# School Attendance Tracker - Full Stack

A comprehensive school attendance management system with QR code functionality, built with ASP.NET Core and Vue.js.

![License](https://img.shields.io/badge/license-MIT-blue.svg)
![.NET](https://img.shields.io/badge/.NET-8.0-purple.svg)
![Vue.js](https://img.shields.io/badge/Vue.js-3.0-green.svg)

## Features

### Admin Panel
- **Real-time Dashboard** - Live statistics with interactive charts
- **Student Management** - Full CRUD operations with QR code generation
- **Attendance Logs** - Comprehensive attendance tracking with filters
- **Advanced Reports** - Weekly, monthly, department-wise, and student-level analytics
- **CSV Export** - Export attendance data for external analysis

### Student Portal
- **QR Code Scanner** - Quick check-in/check-out functionality
- **Personal Dashboard** - View attendance stats and personal QR code
- **Attendance History** - Complete attendance record with duration tracking
- **Mobile-Friendly** - Responsive design optimized for all devices

### 🔧 Technical Features
- RESTful API with Swagger documentation
- QR code generation and scanning
- Real-time statistics and analytics
- SQLite database with EF Core
- Unit testing with xUnit
- Docker containerization
- CORS configuration for frontend integration

## 🛠️ Tech Stack

### Backend
- **Framework:** ASP.NET Core 8.0
- **ORM:** Entity Framework Core
- **Database:** SQLite
- **QR Generation:** QRCoder
- **Testing:** xUnit
- **Documentation:** Swagger/OpenAPI

### Frontend
- **Framework:** Vue.js 3 (Composition API)
- **Build Tool:** Vite
- **Styling:** Tailwind CSS
- **Charts:** Chart.js + vue-chartjs
- **Icons:** Heroicons
- **HTTP Client:** Axios
- **Router:** Vue Router 4

## Quick Start

### Prerequisites
- .NET 8.0 SDK
- Node.js 18+ and npm
- Docker (optional)

### Backend Setup
```bash
# Navigate to API directory
cd SchoolAttendance.API

# Restore dependencies
dotnet restore

# Run migrations
dotnet ef database update

# Run the API
dotnet run
```

API will be available at: `https://localhost:5000` (or check terminal for port)

### Frontend Setup
```bash
# Navigate to frontend directory
cd frontend

# Install dependencies
npm install

# Run development server
npm run dev
```

Frontend will be available at: `http://localhost:5174`

### Docker Setup
```bash
# Build and run with Docker Compose
docker-compose up -d
```

- API: `http://localhost:5000`
- Frontend: Configure in docker-compose.yml

## API Endpoints

### Students
```
GET    /api/student              - Get all students
GET    /api/student/{id}         - Get student by ID
GET    /api/student/qr/{qrCode}  - Get student by QR code
GET    /api/student/{id}/qrcode  - Get student QR code image
POST   /api/student              - Create new student
PUT    /api/student/{id}         - Update student
DELETE /api/student/{id}         - Delete student
```

### Attendance
```
POST   /api/attendance/checkin          - Check-in student
POST   /api/attendance/checkout         - Check-out student
GET    /api/attendance/today            - Today's attendance
GET    /api/attendance/date/{date}      - Attendance by date
GET    /api/attendance/student/{id}     - Student attendance history
```

### Statistics
```
GET    /api/statistics/dashboard        - Dashboard summary
GET    /api/statistics/weekly           - Weekly statistics
GET    /api/statistics/monthly          - Monthly statistics
GET    /api/statistics/student/{id}     - Student statistics
GET    /api/statistics/department/{name} - Department statistics
```

## Usage

### For Administrators

1. **Access Admin Panel:** Navigate to `/admin`
2. **View Dashboard:** See real-time attendance statistics and charts
3. **Manage Students:** Add, edit, or remove students with QR code generation
4. **Track Attendance:** Monitor check-ins/check-outs in real-time
5. **Generate Reports:** Analyze attendance patterns with comprehensive reports
6. **Export Data:** Download attendance records as CSV

### For Students

1. **Access Student Portal:** Navigate to `/student`
2. **View QR Code:** Display your unique QR code on the home screen
3. **Check-in/Check-out:** Use the scanner or enter student number manually
4. **View History:** Track your attendance record and statistics

## Database Schema

### Students
- ID, Student Number, First Name, Last Name, Email, Department, QR Code, Created At

### Teachers
- ID, Employee Number, First Name, Last Name, Email, Department, Created At

### Attendance Records
- ID, Student ID, Check-in Time, Check-out Time, Location, Status

## Testing
```bash
# Run all tests
dotnet test

# Run with coverage
dotnet test /p:CollectCoverage=true
```

## Use Cases

- **Schools & Universities** - Student attendance tracking
- **Corporate Offices** - Employee check-in systems
- **Events** - Participant attendance management
- **Gyms & Studios** - Member visit tracking

## Screenshots

### Admin Dashboard
![Dashboard](docs/dashboard.png)

### Student Portal
![Student Portal](docs/student-portal.png)

### QR Code Scanner
![Scanner](docs/scanner.png)

## Future Enhancements

- [ ] Authentication & Authorization (JWT)
- [ ] Email notifications for absences
- [ ] Mobile app (React Native)
- [ ] Facial recognition integration
- [ ] SMS notifications
- [ ] Parent portal
- [ ] Multi-language support
- [ ] Advanced analytics with AI predictions

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Author

**Burak Ercikti**
- GitHub: [@erciktiburak](https://github.com/erciktiburak)

## Acknowledgments

- ASP.NET Core Team
- Vue.js Community
- Tailwind CSS
- All contributors and supporters


If you find this project helpful, please give it a star!
