import streamlit as st
import pandas as pd
import numpy as np
import pickle
import ast


def run():
    st.header("Model Prediction")
    with open("./models/scaler.pkl", "rb") as file_1:
        scaler = pickle.load(file_1)

    with open("./models/model_knn.pkl", "rb") as file_2:
        model_knn = pickle.load(file_2)

    limit_balance = st.number_input(label="Limit balance nasabah")
    pay_1 = st.selectbox(
        label="Delay Payment on September 2015",
        options=[-2.0, -1.0, 0.0, 1.0, 2.0, 3.0, 4.0, 5.0, 6.0, 7.0, 8.0],
    )
    pay_2 = st.selectbox(
        label="Delay Payment on Agustus 2015",
        options=[-2.0, -1.0, 0.0, 1.0, 2.0, 3.0, 4.0, 5.0, 6.0, 7.0],
    )
    pay_3 = st.selectbox(
        label="Delay Payment on Juli 2015",
        options=[-2.0, -1.0, 0.0, 2.0, 3.0, 4.0, 5.0, 6.0, 7.0],
    )
    pay_4 = st.selectbox(
        label="Delay Payment on Juni 2015",
        options=[-2.0, -1.0, 0.0, 2.0, 3.0, 4.0, 5.0, 6.0, 7.0, 8.0],
    )
    pay_5 = st.selectbox(
        label="Delay Payment on May 2015",
        options=[-2.0, -1.0, 0.0, 2.0, 3.0, 4.0, 5.0, 6.0, 7.0],
    )
    pay_6 = st.selectbox(
        label="Delay Payment on April 2015",
        options=[-2.0, -1.0, 0.0, 2.0, 3.0, 4.0, 6.0, 7.0],
    )

    df_inf = pd.DataFrame(
        {
            "limit_balance": limit_balance,
            "pay_1": pay_1,
            "pay_2": pay_2,
            "pay_3": pay_3,
            "pay_4": pay_4,
            "pay_5": pay_5,
            "pay_6": pay_6,
        },
        index=[0],
    )

    st.table(df_inf)

    if st.button(label="predict"):
        # define data bedasarkan numerik dan kategori
        df_inf_num = df_inf[["limit_balance"]]
        df_inf_cat = df_inf[["pay_1", "pay_2", "pay_3", "pay_4", "pay_5", "pay_6"]]

        df_inf_num_scaled = scaler.transform(df_inf_num)
        df_inf_num_scaled = pd.DataFrame(df_inf_num_scaled)

        df_inf_final = np.concatenate([df_inf_num_scaled, df_inf_cat], axis=1)

        y_pred_inf = model_knn.predict(df_inf_final)

        st.write(y_pred_inf[0])
        if y_pred_inf == 0:
            st.write("Nasabah Terprediksi bisa membayar")
        else:
            st.write("Nasabah Terprediksi tidak bisa membayar")
