# School Attendance Tracker API

ASP.NET Core Web API for school attendance tracking with QR code support.

## Features

- Student management with unique QR codes
- Check-in/Check-out tracking
- Real-time attendance statistics
- Dashboard analytics (daily, weekly, monthly)
- Department-wise reports
- QR code generation for students
- Swagger API documentation
- Docker support

## Key Features Implemented
- RESTful API design with proper HTTP verbs
- Entity Framework Core with code-first migrations
- Repository pattern and dependency injection
- Comprehensive Swagger/OpenAPI documentation
- Unit testing with xUnit and in-memory database
- Docker containerization for easy deployment
- CORS configuration for frontend integration

## Statistics & Analytics
- Real-time dashboard metrics
- Weekly/monthly attendance reports
- Individual student performance tracking
- Department-wise analytics

## Quick Start

### Prerequisites
- .NET 8.0 SDK
- Docker (optional)

### Run Locally
```bash
cd SchoolAttendance.API
dotnet restore
dotnet run
```

Visit: `https://localhost:7xxx/swagger`

### Run with Docker
```bash
docker-compose up -d
```

Visit: `http://localhost:5000/swagger`

## API Endpoints

### Students
- `GET /api/student` - Get all students
- `GET /api/student/{id}` - Get student by ID
- `GET /api/student/qr/{qrCode}` - Get student by QR code
- `GET /api/student/{id}/qrcode` - Get student QR code image
- `POST /api/student` - Create new student
- `PUT /api/student/{id}` - Update student
- `DELETE /api/student/{id}` - Delete student

### Attendance
- `POST /api/attendance/checkin` - Check-in student
- `POST /api/attendance/checkout` - Check-out student
- `GET /api/attendance/today` - Today's attendance
- `GET /api/attendance/date/{date}` - Attendance by date
- `GET /api/attendance/student/{id}` - Student attendance history

### Statistics
- `GET /api/statistics/dashboard` - Dashboard summary
- `GET /api/statistics/weekly` - Weekly statistics
- `GET /api/statistics/monthly` - Monthly statistics
- `GET /api/statistics/student/{id}` - Student statistics
- `GET /api/statistics/department/{name}` - Department statistics

## Tech Stack

- ASP.NET Core 8.0
- Entity Framework Core
- SQLite
- QRCoder
- Swagger/OpenAPI

## Database

SQLite database is automatically created on first run with seed data:
- 2 sample students
- 1 sample teacher

## Use Cases

- School attendance management
- University class tracking
- Corporate office check-ins
- Event attendance tracking

## License

MIT License

## Author

[GitHub](https://github.com/erciktiburak)