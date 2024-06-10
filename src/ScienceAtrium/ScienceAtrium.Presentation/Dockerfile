#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:9.0-preview-nanoserver-1809 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0-preview-nanoserver-1809 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/ScienceAtrium/ScienceAtrium.Presentation/ScienceAtrium.Presentation.csproj", "src/ScienceAtrium/ScienceAtrium.Presentation/"]
COPY ["src/ScienceAtrium/ScienceAtrium.Application/ScienceAtrium.Application.csproj", "src/ScienceAtrium/ScienceAtrium.Application/"]
COPY ["src/ScienceAtrium/ScienceAtrium.Domain/ScienceAtrium.Domain.csproj", "src/ScienceAtrium/ScienceAtrium.Domain/"]
COPY ["src/ScienceAtrium/ScienceAtrium.Infrastructure/ScienceAtrium.Infrastructure.csproj", "src/ScienceAtrium/ScienceAtrium.Infrastructure/"]
RUN dotnet restore "./src/ScienceAtrium/ScienceAtrium.Presentation/ScienceAtrium.Presentation.csproj"
COPY . .
WORKDIR "/src/src/ScienceAtrium/ScienceAtrium.Presentation"
RUN dotnet build "ScienceAtrium.Presentation.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "ScienceAtrium.Presentation.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ScienceAtrium.Presentation.dll"]