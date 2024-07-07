# Rust Multistage

A demo on how to perform docker multi-stage build on Rust with OpenSSL.

## Build

```sh
docker buildx build -t rust-multistage:latest .
```

## Running

```sh
docker run -p8080:8080 --rm -e RUST_BACKTRACE=1 rust-multistage
```