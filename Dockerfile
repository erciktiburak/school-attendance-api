FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

RUN apt-get update && apt-get install -y libgdiplus && rm -rf /var/lib/apt/lists/*

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["SchoolAttendance.API/SchoolAttendance.API.csproj", "SchoolAttendance.API/"]
RUN dotnet restore "SchoolAttendance.API/SchoolAttendance.API.csproj"
COPY . .
WORKDIR "/src/SchoolAttendance.API"
RUN dotnet build "SchoolAttendance.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SchoolAttendance.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SchoolAttendance.API.dll"]