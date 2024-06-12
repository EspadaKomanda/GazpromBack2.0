import grpc
from concurrent import futures
import imageGenerator_pb2 as pb2
import imageGenerator_pb2_grpc as pb2_grpc

class GExchange(pb2_grpc.GExchangeServicer):
   def GenerateImage(self, request, context):
       ##logic
       return pb2.generateImageResponse(image_name=request.prompt, template=request.template, error="No error", image_byte_array=None)
def serve():
   server = grpc.server(futures.ThreadPoolExecutor(max_workers=10))
   pb2_grpc.add_GExchangeServicer_to_server(GExchange(), server)
   server.add_insecure_port("[::]:50051")
   server.start()
   server.wait_for_termination()
