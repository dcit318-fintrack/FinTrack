# =============================================================================
# Stage 1 — build
# SDK image: compiles and publishes FinTrack.Server and its only dependency,
# FinTrack.Shared. FinTrack.Client and tests are never referenced here.
# =============================================================================
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /source

# -- Layer 1: copy only the .csproj files --------------------------------
# Copying project files before source allows Docker to cache the NuGet
# restore layer; it is only invalidated when a .csproj changes.
COPY src/FinTrack.Shared/FinTrack.Shared.csproj  src/FinTrack.Shared/
COPY src/FinTrack.Server/FinTrack.Server.csproj  src/FinTrack.Server/

# -- Layer 2: restore NuGet packages for the Server project only ----------
# Restoring by project (not by solution) avoids any dependency on
# FinTrack.Client.csproj or FinTrack.Tests.csproj being present.
RUN dotnet restore src/FinTrack.Server/FinTrack.Server.csproj

# -- Layer 3: copy source code -------------------------------------------
COPY src/FinTrack.Shared/ src/FinTrack.Shared/
COPY src/FinTrack.Server/ src/FinTrack.Server/

# -- Layer 4: publish in Release mode ------------------------------------
RUN dotnet publish src/FinTrack.Server/FinTrack.Server.csproj \
      --configuration Release \
      --no-restore \
      --output /app/publish

# =============================================================================
# Stage 2 — runtime
# Minimal ASP.NET Core runtime image; contains no SDK or build tooling.
# Only the published output from Stage 1 is copied in.
# =============================================================================
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

# Render injects PORT at container startup. Default to 8080 so the image
# also works with plain `docker run` locally.
# CMD uses shell form so that $PORT is expanded at startup, not at build time.
ENV PORT=8080
EXPOSE 8080

# ---------------------------------------------------------------------------
# All sensitive values are supplied at runtime via Render environment vars.
# Nothing secret is baked into this image. Required Render env vars:
#   ASPNETCORE_ENVIRONMENT=Production
#   ConnectionStrings__DefaultConnection=<MonsterASP SQL Server conn string>
#   Jwt__Secret=<production JWT signing key>
#   Jwt__Issuer=FinTrackServer
#   Jwt__Audience=FinTrackClient
#   Cors__AllowedOrigins__0=https://<your-netlify-app>.netlify.app
# ---------------------------------------------------------------------------
CMD ASPNETCORE_URLS="http://+:${PORT}" dotnet FinTrack.Server.dll
