FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ERP_Placement/ERP_Placement.csproj ERP_Placement/
RUN dotnet restore ERP_Placement/ERP_Placement.csproj

COPY . .
RUN dotnet publish ERP_Placement/ERP_Placement.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://0.0.0.0:10000
EXPOSE 10000

ENTRYPOINT ["dotnet", "ERP_Placement.dll"]