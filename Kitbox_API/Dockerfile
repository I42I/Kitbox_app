FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copier les fichiers csproj et restaurer les dépendances
COPY ["*.csproj", "./"]
RUN dotnet restore

# Copier tout le reste et construire
COPY . .
RUN dotnet publish -c Release -o /app/publish

# Build de l'image finale
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Exposer le port 50097
EXPOSE 50097

# Définir le point d'entrée
ENTRYPOINT ["dotnet", "Kitbox_API.dll"]