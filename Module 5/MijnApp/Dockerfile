FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80

ENV ASPNETCORE_URLS=http://+:80

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG configuration=Release
WORKDIR /src
COPY ["MijnApp.csproj", "./"]
RUN dotnet restore "MijnApp.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "MijnApp.csproj" -c $configuration -o /app/build

#FROM build
ARG configuration=Release
RUN dotnet publish "MijnApp.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "MijnApp.dll"]
