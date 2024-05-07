FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["PokemonLookup.Web/PokemonLookup.Web.csproj", "PokemonLookup.Web/"]
RUN dotnet restore "PokemonLookup.Web/PokemonLookup.Web.csproj"
COPY ./PokemonLookup.Web ./PokemonLookup.Web
COPY ./TestProject1 ./TestProject1
COPY ./PokemonLookup.sln ./PokemonLookup.sln
WORKDIR "/src/PokemonLookup.Web"
RUN dotnet build "PokemonLookup.Web.csproj" -c ${BUILD_CONFIGURATION} -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "PokemonLookup.Web.csproj" -c ${BUILD_CONFIGURATION} -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PokemonLookup.Web.dll"]
