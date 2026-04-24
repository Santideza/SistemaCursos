FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["SistemaCursos.csproj", "./"]
RUN dotnet restore "SistemaCursos.csproj"

COPY . .
RUN dotnet publish "SistemaCursos.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 10000

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "SistemaCursos.dll"]
