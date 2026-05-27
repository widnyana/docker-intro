# dotnet-hello

Minimal .NET 10 web API returning the container's hostname.

## Build

```sh
docker build -t dotnet-hello .
```

## Run

```sh
docker run -p 8000:8000 --rm dotnet-hello
```

## Test

```sh
curl http://localhost:8000/
```
