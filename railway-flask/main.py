import os

from flask import Flask, jsonify

app = Flask(__name__)


@app.route("/")
def index():
    return jsonify({"x": "(ง'̀-'́)ง", "hello": "welkam ~"})


@app.route("/health")
def health():
    return jsonify({"sehat?": "sehaaaaaaat ᵔ ᴥ ᵔ"})


if __name__ == "__main__":
    app.run(debug=True, port=os.getenv("PORT", default=5000))
