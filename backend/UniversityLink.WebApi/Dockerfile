FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["UniversityLink.WebApi/UniversityLink.WebApi.csproj", "UniversityLink.WebApi/"]
RUN dotnet restore "UniversityLink.WebApi/UniversityLink.WebApi.csproj"
COPY . .
WORKDIR "/src/UniversityLink.WebApi"
RUN dotnet build "./UniversityLink.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./UniversityLink.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "UniversityLink.WebApi.dll"]