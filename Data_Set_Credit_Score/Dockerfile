FROM python:3.11-alpine AS base

ENV VIRTUAL_ENV=/app/venv
ENV PATH="$VIRTUAL_ENV/bin:$PATH"

RUN python3 -m venv $VIRTUAL_ENV

WORKDIR /app

# =================

FROM base AS deployment

RUN apk update \
  && apk add --no-cache \
    libgomp \
    libstdc++

# =================

FROM base AS setup

RUN python3 -m ensurepip \
    && python3 -m pip install --upgrade pip \
    && python3 -m pip install poetry==1.8.1

RUN apk update \
  && apk add --no-cache \
      musl \
      build-base

COPY pyproject.toml poetry.lock /app/
RUN poetry install --without dev

# why twice? because we want to practice black-magic in here
COPY . /app
RUN poetry install --without dev

# =================

# Final image without setup dependencies and other garbage
FROM deployment AS final

COPY --from=setup /app /app

EXPOSE 8000
CMD [ "/app/serve-api.sh" ]