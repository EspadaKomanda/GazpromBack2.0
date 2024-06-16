import io
from PIL import Image, ImageOps
import cv2
import grpc
import numpy as np
import imageProccessor_pb2 as pb2
import imageProccessor_pb2_grpc as pb2_grpc
from concurrent import futures

def replace_colors(image, allowed_colors):
    image_data = image.load()
    for x in range(image.width):
        for y in range(image.height):
            pixel_color = image_data[x, y][:3]  # Получаем RGB цвет пикселя
            alpha = image_data[x, y][3]  # Получаем значение альфа-канала
            if pixel_color not in allowed_colors:
                closest_color = min(allowed_colors, key=lambda c: sum(
                    (c_i - pixel_color_i) ** 2 for c_i, pixel_color_i in zip(c, pixel_color)))
                new_pixel = closest_color + (alpha,)  # Сохраняем значение альфа-канала
                image_data[x, y] = new_pixel

    return image


def remove_most_common_color(image, delta=5):
    pixels = image.getdata()

    # Подсчет количества встречающихся цветов
    color_count = {}
    for pixel in pixels:
        if pixel not in color_count:
            color_count[pixel] = 1
        else:
            color_count[pixel] += 1

    # Находим цвет, который встречается наиболее часто
    most_common_color = max(color_count, key=color_count.get)

    if color_count[most_common_color] < image.width * image.height / 100:
        return remove_background(image)

    # Создаем изображение с прозрачным фоном
    image_with_transparent_bg = Image.new("RGBA", image.size, (0, 0, 0, 0))

    # Заменяем наиболее часто встречающийся цвет на прозрачный
    for x in range(image.width):
        for y in range(image.height):
            pixel_color = image.getpixel((x, y))
            if all(most_common_color[i] - delta <= pixel_color[i] <= most_common_color[i] + delta for i in range(3)):
                image_with_transparent_bg.putpixel((x, y), (0, 0, 0, 0))  # Прозрачный цвет
            else:
                image_with_transparent_bg.putpixel((x, y), pixel_color)

    return image_with_transparent_bg


def remove_background(image):

    image_np = np.array(image)
    image = cv2.cvtColor(image_np, cv2.COLOR_RGB2BGR)
    if image is None:
        print("Не удалось загрузить изображение")
        return None

    mask = np.zeros(image.shape[:2], np.uint8)

    background_model = np.zeros((1, 65), np.float64)
    foreground_model = np.zeros((1, 65), np.float64)

    rectangle = (50, 50, image.shape[1] - 50, image.shape[0] - 50)  # Прямоугольник, ограничивающий объект

    cv2.grabCut(image, mask, rectangle, background_model, foreground_model, 5, cv2.GC_INIT_WITH_RECT)

    mask2 = np.where((mask == 2) | (mask == 0), 0, 1).astype('uint8')
    image_without_background = image * mask2[:, :, np.newaxis]

    # Создание изображения с 4 каналами (RGB + Alpha)
    image_rgba = cv2.cvtColor(image_without_background, cv2.COLOR_BGR2BGRA)
    alpha_mask = np.where((mask2 == 1), 255, 0).astype('uint8')
    image_rgba[:, :, 3] = alpha_mask

    # Сохранение изображения и конвертация в PIL Image с правильными цветами
    image_path_output = r"C:\Users\Max\PycharmProjects\hakaton\data\process.png"
    cv2.imwrite(image_path_output, image_rgba)

    image_pil = Image.open(image_path_output)

    return image_pil


def increase_resolution(image, new_resolution, resolution_pos="center", background_color=(0, 0, 0, 0)):
    # Получаем размеры исходного изображения
    original_width, original_height = image.size

    # Создаем новое изображение большего размера
    new_width, new_height = new_resolution
    new_image = Image.new('RGBA', (new_width, new_height))

    # Размещаем исходное изображение в новом изображении
    # Варианты: left_top, left_center, left_bottom, right_top, right_center, right_bottom, center_top, center_center, center_bottom, resize
    if resolution_pos == "resize":
        image = image.resize((new_width, new_height))
        new_image.paste(image, (0, 0))
    else:
        switcher = {
            'left_top': (0, 0),
            'left_center': (0, (new_height - original_height) // 2),
            'left_bottom': (0, new_height - original_height),

            'right_top': (new_width - original_width, 0),
            'right_center': (new_width - original_width, (new_height - original_height) // 2),
            'right_bottom': (new_width - original_width, new_height - original_height),

            'center_top': ((new_width - original_width) // 2, 0),
            'center_center': ((new_width - original_width) // 2, (new_height - original_height) // 2),
            'center_bottom': ((new_width - original_width) // 2, new_height - original_height),
        }

        position = switcher.get(resolution_pos)
        new_image.paste(image, position)

    # Заменяем цвет фона нового изображения
    if background_color[3] != 0:
        new_image = set_background(new_image, background_color)

    return new_image


def set_background(image, background_color):
    image_data = image.getdata()

    r, g, b, a = background_color

    new_data = []
    for pixel in image_data:
        r_pixel, g_pixel, b_pixel, a_pixel = pixel
        if a_pixel == 0:
            # Если альфа-канал равен 0, заменяем цвет пикселя на цвет фона
            new_data.append((r, g, b, a))
        else:
            new_data.append((r_pixel, g_pixel, b_pixel, a_pixel))

    new_image = Image.new(image.mode, image.size)
    new_image.putdata(new_data)

    return new_image


def check_colors(image, allowed_colors):
    max_colors = 256
    colors = image.getcolors(max_colors)

    if colors is None:
        image = image.convert('P')
        colors = image.getcolors(max_colors)

    if colors is None:
        return False

    for color in colors:
        if color[1] not in allowed_colors:
            return False

    return True
def process(byte_image, allowed_colors_str, background="transparent", width=512, height=512, resolution_pos="center", text=None, font=None,
            should_check_colors=False):
    print("A"*1000)
    resolution = (width, height)
    allowed_colors = []
    for color in allowed_colors_str:
        r, g, b = map(int, color.split(', '))
        allowed_colors.append((r, g, b))

    # image
    image = Image.open(io.BytesIO(byte_image))

    if background != "saved":
        image = remove_most_common_color(image, 20)

        # background
        if background != "transparent":
            r, g, b, a = map(int, background.split(', '))
            rgba_color_background = [r, g, b, a]
            image = set_background(image, rgba_color_background)

    # resolution
    if image.size != resolution:
        if image.size[0] < resolution[0] and image.size[1] < resolution[1]:
            rgba_color_background = [0, 0, 0, 0]
            if background != "transparent":
                r, g, b, a = map(int, background.split(', '))
                rgba_color_background = [r, g, b, a]
            image = increase_resolution(image, resolution, resolution_pos, rgba_color_background)
        else:
            image.resize(resolution)

    # colors
    if should_check_colors:
        image = replace_colors(image, allowed_colors)

    # return bytes
    img_byte_array = io.BytesIO()
    image.save(img_byte_array, format='PNG')
    img_byte_array = img_byte_array.getvalue()
    
    print("B"*1000+"\n"+img_byte_array)
    return img_byte_array
class GExchange(pb2_grpc.ImageProcessorServicer):
   def VerifyImage(self, request, context):
       try:
         print(request.request.allowed_colors_str)
         print(request.request.background)
         print(request.request.width)
         print(request.request.height)
         print(request.request.resolution_pos)
         print(request.request.text)
         print(request.request.font)
         print(request.request.should_check_colors)
         image = process(request.byte_image, request.allowed_colors_str, request.background, request.width, request.height, request.resolution_pos, request.text, request.font, request.should_check_colors)
         return pb2.ImageResponse(error="", image_byte_array=image)
       except Exception as e:
         return pb2.ImageResponse(error=str(e), image_byte_array=[])
       
def serve():
   server = grpc.server(futures.ThreadPoolExecutor(max_workers=100))
   pb2_grpc.add_ImageProcessorServicer_to_server(GExchange(), server)
   server.add_insecure_port("[::]:5050")
   server.start()
   server.wait_for_termination()
serve()