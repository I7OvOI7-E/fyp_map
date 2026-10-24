FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY WebApplication1.csproj .
RUN dotnet restore WebApplication1.csproj
COPY . .
RUN dotnet publish WebApplication1.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://0.0.0.0:10000
EXPOSE 10000
ENTRYPOINT ["dotnet", "WebApplication1.dll"]
