FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["ToDoApp.API/ToDoApp.API.csproj", "ToDoApp.API/"]
COPY ["ToDoApp.Infrastructure/ToDoApp.Infrastructure.csproj", "ToDoApp.Infrastructure/"]
COPY ["ToDoApp.Application/ToDoApp.Application.csproj", "ToDoApp.Application/"]
COPY ["ToDoApp.Domain/ToDoApp.Domain.csproj", "ToDoApp.Domain/"]
RUN dotnet restore "ToDoApp.API/ToDoApp.API.csproj"
COPY . .
WORKDIR "/src/ToDoApp.API"
RUN dotnet build "ToDoApp.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ToDoApp.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ToDoApp.API.dll"]
