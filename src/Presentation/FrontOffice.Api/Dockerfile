FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app

EXPOSE 8443
EXPOSE 8080

USER app
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG configuration=Release
WORKDIR /src
COPY ["src/Presentation/FrontOffice.Api/CesiZen.Presentation.FrontOffice.Api.csproj", "src/Presentation/FrontOffice.Api/"]
RUN dotnet restore "src/Presentation/FrontOffice.Api/CesiZen.Presentation.FrontOffice.Api.csproj"
COPY . .
WORKDIR "/src/src/Presentation/FrontOffice.Api"
RUN dotnet build "CesiZen.Presentation.FrontOffice.Api.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "CesiZen.Presentation.FrontOffice.Api.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CesiZen.Presentation.FrontOffice.Api.dll"]
CMD []
