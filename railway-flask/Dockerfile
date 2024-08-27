FROM python:3.11-alpine

RUN adduser -Ds /bin/bash  abdul


USER abdul

WORKDIR /home/abdul/app
COPY . /home/abdul/app

RUN python -m pip install --user --upgrade pip \
  && python -m pip install --user -r requirements.txt


EXPOSE 8000
ENTRYPOINT ["/home/abdul/app/start.sh"]