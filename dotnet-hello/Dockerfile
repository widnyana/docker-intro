FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS builder

WORKDIR /build

COPY dotnet-hello.csproj .
RUN dotnet restore

COPY . .
RUN dotnet publish -c Release -o /app/publish

# =====================

FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine

RUN adduser -Ds /bin/sh appuser

WORKDIR /app
COPY --from=builder /app/publish .

USER appuser

EXPOSE 8000
ENV ASPNETCORE_HTTP_PORTS=8000
ENV ConnectionStrings__Default=""
ENV Redis__ConnectionString=""

ENTRYPOINT ["dotnet", "dotnet-hello.dll"]
