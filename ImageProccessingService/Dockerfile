FROM python:3.10.12

RUN apt-get update && apt-get install -y --no-install-recommends libgl1-mesa-glx

RUN python -m pip install --upgrade pip
RUN python -m pip install pillow opencv-python protobuf grpcio
RUN python -m pip install numpy==1.26.4 # Specify the desired version here

WORKDIR /app

COPY . /app/

EXPOSE 5051

CMD ["python", "image_processor.py"]