FROM mcr.microsoft.com/dotnet/sdk:10.0 AS builder

WORKDIR /build

COPY dotnet-hello.csproj .
RUN dotnet restore

COPY . .
RUN dotnet publish -c Release -o /app/publish

# =====================

FROM mcr.microsoft.com/dotnet/aspnet:10.0

RUN useradd --create-home appuser

WORKDIR /app
COPY --from=builder /app/publish .

USER appuser

EXPOSE 8000
ENV ASPNETCORE_HTTP_PORTS=8000

ENTRYPOINT ["dotnet", "dotnet-hello.dll"]
