FROM rust:1-bookworm AS builder

WORKDIR /build

COPY . .

RUN cargo build --release

# ===================== 

FROM debian:bookworm-slim

WORKDIR /app

RUN DEBIAN_FRONTEND=noninteractive apt-get update \
  && apt-get install -y \
    ca-certificates \
    openssl \
  && apt-get clean autoclean \
  && apt-get autoremove --yes \
  && rm -rf /var/lib/{apt,dpkg,cache,log}/

COPY --from=builder /build/target/release/rust-multistage /app/rust-multistage


EXPOSE 8080

CMD ["/app/rust-multistage"]