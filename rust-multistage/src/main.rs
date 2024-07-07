use actix_web::{get, post, App, HttpResponse, HttpServer, Responder};
use awc::Client;
use log::{error, info};
use mime;
use openssl::ssl::{SslConnector, SslMethod};
use std::time::Instant;

const URL: &str = "https://reqres.in/api/users?page=1";

fn tls_web_client() -> Client {
    let ssl = SslConnector::builder(SslMethod::tls()).unwrap().build();
    let conn = awc::Connector::new().openssl(ssl);
    awc::Client::builder().connector(conn).finish()
}

#[get("/")]
async fn hello() -> impl Responder {
    HttpResponse::Ok().body("hello world!")
}

#[post("/echo")]
async fn echo(req_body: String) -> impl Responder {
    HttpResponse::Ok().body(req_body)
}

#[get("/sample")]
async fn fetch_json() -> HttpResponse {
    let start = Instant::now();
    let hc = tls_web_client();

    let mut res = hc.get(URL).send().await.unwrap();

    if !res.status().is_success() {
        error!("Server did not return expected result");
        return HttpResponse::InternalServerError().finish();
    }

    let payload = res.body().await.unwrap();

    info!(
        "it took {}ms to download image to memory",
        start.elapsed().as_millis()
    );

    HttpResponse::Ok()
        .content_type(mime::APPLICATION_JSON)
        .body(payload)
}

#[actix_web::main]
async fn main() -> std::io::Result<()> {
    env_logger::init_from_env(env_logger::Env::new().default_filter_or("info"));

    HttpServer::new(|| {
        App::new()
        .service(hello)
        .service(echo)
        .service(fetch_json)
    })
        .bind("0.0.0.0:8080")?
        .run()
        .await
}
