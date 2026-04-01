FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app

USER app
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG configuration=Release
WORKDIR /src
COPY ["src/Presentation/BackOffice/CesiZen.Presentation.BackOffice.csproj", "src/Presentation/BackOffice/"]
RUN dotnet restore "src/Presentation/BackOffice/CesiZen.Presentation.BackOffice.csproj"
COPY . .
WORKDIR "/src/src/Presentation/BackOffice"
RUN dotnet build "CesiZen.Presentation.BackOffice.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "CesiZen.Presentation.BackOffice.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CesiZen.Presentation.BackOffice.dll"]
