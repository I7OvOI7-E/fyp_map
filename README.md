# Map API: Render and Neon

## First deployment with an empty Neon database

1. In Neon, open **Connect** for your project and select the existing database
   (`fyp_map`) and its owner role. No manual CREATE TABLE statements are needed.
2. Get the .NET / Npgsql connection details. Use a connection string in this format:

   ```text
   Host=YOUR_NEON_HOST;Port=5432;Database=fyp_map;Username=YOUR_ROLE;Password=YOUR_PASSWORD;SSL Mode=VerifyFull;
   ```

   Use your actual host and database details. Keep credentials out of Git and chat.
3. In Render, select this repository and branch `main`, root directory
   `WebApplication1`, Docker runtime, Dockerfile `./Dockerfile`, and Free instance.
4. Add environment variables:
   - `ASPNETCORE_ENVIRONMENT`: `Production`
   - `ConnectionStrings__MapConnection`: the Npgsql connection string above
5. Set the health check path to `/health` and deploy.

On startup the API connects to Neon and uses Entity Framework to create `Devices`,
`IncidentReports`, `IncidentImages`, and `Charts`, including IDs, indexes, and foreign
keys. The database must already exist and the configured role needs schema creation
permissions. No sample rows are inserted. Existing RDS data is not migrated.

Check deployment logs for `Database initialized`, then open `/health` and `/Map`.
On an empty database `/Map` should return `[]`. In Neon, refresh the Tables view
for the same branch and database to see the four tables.

## Important behavior

- Startup fails if configuration or the database connection is invalid; check Render logs.
- Initialization uses `EnsureCreatedAsync`: it creates tables only if the database
  contains no tables. It does not repair partial schemas or apply model changes to
  existing tables. Adopt EF migrations before making future schema changes; do not
  delete a populated database to fix a schema mismatch.
- Server-generated dates are UTC. Send UTC timestamps with a `Z` suffix if supplying
  timestamps in API requests.
- Schema follows the EF model, including its current 255-character limits and
  cascade / set-null relationships, rather than the previous SQL Server SQL file.
- Build just the API with `dotnet build WebApplication1/WebApplication1.csproj`.
  The existing HospitalApp.Tests project references unrelated hospital models.
