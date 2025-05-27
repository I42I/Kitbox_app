FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 50097

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["kitboxAPI.csproj", "."]
RUN dotnet restore "kitboxAPI.csproj"
COPY . .
WORKDIR "/src"
RUN dotnet build "kitboxAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "kitboxAPI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "kitboxAPI.dll"]