# =============================================================================
# Stage 1 — build
# SDK image: compiles and publishes FinTrack.Server along with FinTrack.Client
# (Blazor WebAssembly) and FinTrack.Shared.
# =============================================================================
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /source

# -- Layer 1: copy project files (.csproj) ------------------------------
# Copying project files before source allows Docker to cache the NuGet
# restore layer; it is only invalidated when a .csproj changes.
COPY src/FinTrack.Shared/FinTrack.Shared.csproj  src/FinTrack.Shared/
COPY src/FinTrack.Client/FinTrack.Client.csproj  src/FinTrack.Client/
COPY src/FinTrack.Server/FinTrack.Server.csproj  src/FinTrack.Server/

# -- Layer 2: restore NuGet packages -------------------------------------
RUN dotnet restore src/FinTrack.Server/FinTrack.Server.csproj

# -- Layer 3: copy source code -------------------------------------------
COPY src/FinTrack.Shared/ src/FinTrack.Shared/
COPY src/FinTrack.Client/ src/FinTrack.Client/
COPY src/FinTrack.Server/ src/FinTrack.Server/

# -- Layer 4: publish in Release mode ------------------------------------
# Publishing FinTrack.Server automatically builds and bundles FinTrack.Client
# static web assets into wwwroot/
RUN dotnet publish src/FinTrack.Server/FinTrack.Server.csproj \
      --configuration Release \
      --no-restore \
      --output /app/publish

# =============================================================================
# Stage 2 — runtime
# Minimal ASP.NET Core runtime image; contains no SDK or build tooling.
# Serves the ASP.NET Core API and Blazor WebAssembly static assets.
# =============================================================================
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

# Render injects PORT at container startup. Default to 8080 so the image
# also works with plain `docker run` locally.
ENV PORT=8080
EXPOSE 8080

# ---------------------------------------------------------------------------
# Sensitive values are supplied at runtime via Render environment variables:
#   ASPNETCORE_ENVIRONMENT=Production
#   ConnectionStrings__DefaultConnection=<MonsterASP SQL Server conn string>
#   Jwt__Secret=<production JWT signing key>
#   Jwt__Issuer=FinTrackServer
#   Jwt__Audience=FinTrackClient
# ---------------------------------------------------------------------------
ENTRYPOINT ["sh", "-c", "ASPNETCORE_URLS=http://+:${PORT} exec dotnet FinTrack.Server.dll"]
