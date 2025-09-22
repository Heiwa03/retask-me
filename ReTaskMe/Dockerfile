FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /src

COPY "*.csproj" .
RUN dotnet restore

COPY . .

RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final

WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8181

ENV ASPNETCORE_URLS=http://0.0.0.0:8181

ENTRYPOINT ["dotnet", "ReTaskMe.dll"]
