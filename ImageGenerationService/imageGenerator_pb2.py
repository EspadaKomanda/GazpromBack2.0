# -*- coding: utf-8 -*-
# Generated by the protocol buffer compiler.  DO NOT EDIT!
# source: imageGenerator.proto
# Protobuf Python Version: 5.26.1
"""Generated protocol buffer code."""
from google.protobuf import descriptor as _descriptor
from google.protobuf import descriptor_pool as _descriptor_pool
from google.protobuf import symbol_database as _symbol_database
from google.protobuf.internal import builder as _builder
# @@protoc_insertion_point(imports)

_sym_db = _symbol_database.Default()




DESCRIPTOR = _descriptor_pool.Default().AddSerializedFile(b'\n\x14imageGenerator.proto\x12\x0eimagegenerator\"8\n\x14GenerateImageRequest\x12\x0e\n\x06prompt\x18\x01 \x01(\t\x12\x10\n\x08template\x18\x02 \x01(\t\"f\n\x15GenerateImageResponse\x12\x12\n\nimage_name\x18\x01 \x01(\t\x12\x10\n\x08template\x18\x02 \x01(\t\x12\x18\n\x10image_byte_array\x18\x03 \x01(\x0c\x12\r\n\x05\x65rror\x18\x04 \x01(\t2n\n\x0eImageGenerator\x12\\\n\rGenerateImage\x12$.imagegenerator.GenerateImageRequest\x1a%.imagegenerator.GenerateImageResponseb\x06proto3')

_globals = globals()
_builder.BuildMessageAndEnumDescriptors(DESCRIPTOR, _globals)
_builder.BuildTopDescriptorsAndMessages(DESCRIPTOR, 'imageGenerator_pb2', _globals)
if not _descriptor._USE_C_DESCRIPTORS:
  DESCRIPTOR._loaded_options = None
  _globals['_GENERATEIMAGEREQUEST']._serialized_start=40
  _globals['_GENERATEIMAGEREQUEST']._serialized_end=96
  _globals['_GENERATEIMAGERESPONSE']._serialized_start=98
  _globals['_GENERATEIMAGERESPONSE']._serialized_end=200
  _globals['_IMAGEGENERATOR']._serialized_start=202
  _globals['_IMAGEGENERATOR']._serialized_end=312
# @@protoc_insertion_point(module_scope)