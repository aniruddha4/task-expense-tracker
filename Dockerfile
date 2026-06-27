FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY TaskExpenseTracker.Domain/TaskExpenseTracker.Domain.csproj TaskExpenseTracker.Domain/
COPY TaskExpenseTracker.Application/TaskExpenseTracker.Application.csproj TaskExpenseTracker.Application/
COPY TaskExpenseTracker.Infrastructure/TaskExpenseTracker.Infrastructure.csproj TaskExpenseTracker.Infrastructure/
COPY TaskExpenseTracker.API/TaskExpenseTracker.API.csproj TaskExpenseTracker.API/
COPY TaskExpenseTracker.Tests/TaskExpenseTracker.Tests.csproj TaskExpenseTracker.Tests/

RUN dotnet restore TaskExpenseTracker.API/TaskExpenseTracker.API.csproj

COPY . .

RUN dotnet publish TaskExpenseTracker.API/TaskExpenseTracker.API.csproj -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "TaskExpenseTracker.API.dll"]
