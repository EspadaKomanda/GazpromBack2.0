import grpc
from concurrent import futures
import imageGenerator_pb2 as pb2
import imageGenerator_pb2_grpc as pb2_grpc
import PIL.Image as Image
import numpy as np
from diffusers import DiffusionPipeline
import torch
import io

def generateImage(prompt):
   pipe_id = "runwayml/stable-diffusion-v1-5"
   pipe = DiffusionPipeline.from_pretrained(pipe_id, torch_dtype=torch.float16).to("cuda")
   pipe.load_lora_weights("./Lora", weight_name="Espada_v2-23.safetensors")

   lora_scale = 0.9
   image = pipe(
      prompt, num_inference_steps=30, cross_attention_kwargs={"scale": lora_scale}, generator=torch.manual_seed(0)
   ).images[0]
   img_byte_array = io.BytesIO()
   image.save(img_byte_array, format='PNG')
   img_byte_array = img_byte_array.getvalue()
   return img_byte_array
   
class GExchange(pb2_grpc.ImageGeneratorServicer):
   def GenerateImage(self, request, context):
       try:
         image = generateImage(request.prompt)
         return pb2.GenerateImageResponse(image_name="1234", template=request.template, image_byte_array=image,error="")
       except:
         return pb2.GenerateImageResponse(image_name="1234", template=request.template, image_byte_array=None, error="Error generating image")
       
def serve():
   server = grpc.server(futures.ThreadPoolExecutor(max_workers=10))
   pb2_grpc.add_ImageGeneratorServicer_to_server(GExchange(), server)
   server.add_insecure_port("[::]:5051")
   server.start()
   server.wait_for_termination()
serve()
