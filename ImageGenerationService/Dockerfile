FROM python:3.10.12

RUN python -m pip install grpcio torch protobuf
RUN python -m pip install --upgrade diffusers[torch]
RUN python -m pip install -q diffusers transformers accelerate opencv-python
RUN python -m pip install -U peft transformers

WORKDIR /app

COPY . /app/

EXPOSE 5051

CMD ["python", "imageGeneratorService.py"]