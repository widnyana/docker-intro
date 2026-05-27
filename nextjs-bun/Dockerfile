FROM oven/bun:1.3.14 AS base

WORKDIR /app

# ---
FROM base AS builder

COPY package.json bun.lockb ./

RUN bun install --frozen-lockfile \
  && bun next telemetry disable

COPY . . 
RUN bun run build

RUN bun install --production --frozen-lockfile


# ---
FROM base AS final

RUN addgroup --system --gid 1001 nodejs
RUN adduser --system --uid 1001 nextjs
USER nextjs

COPY --chown=nextjs:nextjs . .
COPY --from=builder --chown=nextjs:nextjs /app/public       ./public
COPY --from=builder --chown=nextjs:nextjs /app/.next/static ./.next/static
COPY --from=builder --chown=nextjs:nextjs /app/node_modules ./node_modules/

EXPOSE 3000
CMD ["bun", "run", "start"]
