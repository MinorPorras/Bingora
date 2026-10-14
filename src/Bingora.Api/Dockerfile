FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Bingora.Api/Bingora.Api.csproj", "src/Bingora.Api/"]
COPY ["src/Bingora.Application/Bingora.Application.csproj", "src/Bingora.Application/"]
COPY ["src/Bingora.Domain/Bingora.Domain.csproj", "src/Bingora.Domain/"]
COPY ["src/Bingora.Infraestructure/Bingora.Infraestructure.csproj", "src/Bingora.Infraestructure/"]
RUN dotnet restore "src/Bingora.Api/Bingora.Api.csproj"
COPY . .
WORKDIR "/src/src/Bingora.Api"
RUN dotnet build "./Bingora.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Bingora.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Bingora.Api.dll"]
