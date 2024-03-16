from fastapi import FastAPI
import pandas as pd
import numpy as np
import pickle
from pydantic import BaseModel


PREDICT_MIN_VAL = -0.2
PREDICT_MAX_VAL = 0.8


with open("models/scaler.pkl", "rb") as file_1:
    scaler = pickle.load(file_1)

with open("models/model_knn.pkl", "rb") as file_2:
    model_knn = pickle.load(file_2)


class PredictResponse(BaseModel):
    result: str


class PredictRequest(BaseModel):
    limit_balance: float
    pay_1: float
    pay_2: float
    pay_3: float
    pay_4: float
    pay_5: float
    pay_6: float


def __do_predict(req: PredictRequest) -> PredictResponse:

    df_inf = pd.DataFrame(
        {
            "limit_balance": req.limit_balance,
            "pay_1": req.pay_1,
            "pay_2": req.pay_2,
            "pay_3": req.pay_3,
            "pay_4": req.pay_4,
            "pay_5": req.pay_5,
            "pay_6": req.pay_6,
        },
        index=[0],
    )
    df_inf_num = df_inf[["limit_balance"]]
    df_inf_cat = df_inf[["pay_1", "pay_2", "pay_3", "pay_4", "pay_5", "pay_6"]]

    df_inf_num_scaled = scaler.transform(df_inf_num)
    df_inf_num_scaled = pd.DataFrame(df_inf_num_scaled)

    df_inf_final = np.concatenate([df_inf_num_scaled, df_inf_cat], axis=1)

    y_pred_inf = model_knn.predict(df_inf_final)
    if y_pred_inf == 0:
        result = "Nasabah Terprediksi bisa membayar"
    else:
        result = "Nasabah Terprediksi tidak bisa membayar"

    return PredictResponse(result=result)


# ====================================
# Begin REST API Server
# ====================================

app = FastAPI()


@app.get("/")
async def root():
    return {"message": "Hello World"}


@app.post("/predict")
async def predict(payload: PredictRequest):
    result = __do_predict(payload)

    return result
