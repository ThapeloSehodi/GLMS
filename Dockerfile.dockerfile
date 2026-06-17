FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["GLMS/GLMS.csproj", "GLMS/"]
RUN dotnet restore "GLMS/GLMS.csproj"
COPY . .
WORKDIR "/src/GLMS"
RUN dotnet publish "GLMS.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "GLMS.dll"]