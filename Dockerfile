FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["ScreenlyyIDdotNetSdk/ScreenlyyIDdotNetSdk.csproj", "ScreenlyyIDdotNetSdk/"]
RUN dotnet restore "ScreenlyyIDdotNetSdk/ScreenlyyIDdotNetSdk.csproj"
COPY . .
WORKDIR "/src/ScreenlyyIDdotNetSdk"
RUN dotnet build "ScreenlyyIDdotNetSdk.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ScreenlyyIDdotNetSdk.csproj" -c Release -o /app/publish

FROM nginx:alpine AS final
WORKDIR /usr/share/nginx/html
COPY --from=publish /app/publish/wwwroot .
COPY ScreenlyyIDdotNetSdk/nginx.conf /etc/nginx/nginx.conf
