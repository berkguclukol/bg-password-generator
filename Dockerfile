# Web version: Blazor WebAssembly compiled to static files and served by nginx.
# All code runs in the visitor's browser; the container only serves files.

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /repo
COPY Directory.Build.props ./
COPY src/BgPasswordGenerator.Core/ src/BgPasswordGenerator.Core/
COPY src/BgPasswordGenerator.Web/ src/BgPasswordGenerator.Web/
COPY src/BgPasswordGenerator/Assets/logo.svg src/BgPasswordGenerator/Assets/logo.svg
RUN dotnet publish src/BgPasswordGenerator.Web -c Release -o /publish

FROM nginxinc/nginx-unprivileged:1-alpine
COPY nginx.conf /etc/nginx/conf.d/default.conf
COPY --from=build /publish/wwwroot /usr/share/nginx/html
EXPOSE 8080
