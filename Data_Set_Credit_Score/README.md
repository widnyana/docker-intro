# Credit Score prediction API

Creating API and Dockerizing [kennethv1706/Data_Set_Credit_Score](https://huggingface.co/kennethv1706/Data_Set_Credit_Score)

```
Graded Challenge 5
Membuat Model dimana untuk mempredik apakah user bisa atau tidak membayar untuk bulan depan berdasarkan history data pembayaran user pada bulan-bulan sebelumnya
Nama  : Kenneth Vincentius
Batch : HCK-007
```

## Dependencies

Make sure you have `docker` or `podman` as the container runtime, and `python3.11` or newer. for detailed dependency breakdown, see `pyproject.toml`

## Running with Docker

**Build Container image:**

```sh
docker buildx build -t <OCI_NAME>:<OCI_VERSION> .
```

**Run the Container image:**

```sh
docker run <OCI_NAME>:<OCI_VERSION> -p 8000:8000
```

The API can be accessed on `http://localhost:8000` if no error occured.

---

Pre-build container image is available with name [widnyana/docker-intro:credit-score-0.1.0](https://hub.docker.com/r/widnyana/docker-intro/tags) with size `~461MB`.

Pull it with `docker pull widnyana/docker-intro:credit-score-0.1.0`


## Running WITHOUT using Docker

Execute command below on your terminal emulator of choice:

```sh
poetry shell
poetry install

./serve-api.sh
```


## Hitting the API

The API only have 2 endpoints, `GET /` and `POST /predict`.

Payload for `POST /predict`:

```json
{
    "limit_balance": 0.20,
    "pay_1": -1.0,
    "pay_2": 2.0,
    "pay_3": -0.2,
    "pay_4": -0.2,
    "pay_5": -0.2,
    "pay_6": 0.0
}
```