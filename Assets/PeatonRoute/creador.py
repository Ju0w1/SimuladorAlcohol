import pygame

# Initialize Pygame
pygame.init()

# Constants
WIDTH = 800
HEIGHT = 600
IMAGE_PATH = "scenary.png"  # Replace with the path to your image

IMAGE_POS = [-823.2800000000104, -320.5799992000021]
IMAGE_SIZE = 1.3

# Colors
BLACK = (0, 0, 0)

READ_FROM = "points.txt"
WRITE_TO = "new_points.txt"

def read_points():
    with open(READ_FROM) as file:
        content = file.read()
    parsed = [[y for y in x.split(' ')] for x in content.split('\n')]
    ciclos = []
    one_way_jumps = []
    a = 0
    for i, x in enumerate(parsed):
        if len(x) == 1 and len(x[0]) == 0:
            ciclos.append([[float(k) for k in y] for y in parsed[a:i]])
            a = i + 1
        elif len(x) == 4:
            one_way_jumps.append([int(y) for y in x])
    return ciclos, one_way_jumps

ciclos, one_way_jumps = read_points()
def write_points(ciclos, one_way_jumps):
    with open(WRITE_TO, 'w') as file:
        for ciclo in ciclos:
            for point in ciclo:
                file.write(' '.join([str(p) for p in point]) + '\n')
            file.write('\n')
        first = True
        for jump in one_way_jumps:
            if first:
                first = False
            else:
                file.write('\n')
            file.write(' '.join([str(p) for p in jump]))
write_points(ciclos, one_way_jumps)

offset = [0, 0]
zoom = 5
deleting_radius = 1

def get_actual_position(pos, height = 0):
    new_pos = [x * zoom for x in pos]
    return (new_pos[0] + offset[0], HEIGHT - (new_pos[1] + offset[1]) - height * zoom)

images = {}
def get_image_with_size(image, size):
    if not size in images:
        images[size] = pygame.transform.scale(image, size)
    return images[size]

def get_inverse_position(pos, height = 0):
    new_pos = [pos[0] - offset[0], HEIGHT - pos[1] - height * zoom - offset[1]]
    original_pos = [x / zoom for x in new_pos]
    return original_pos

def calc_distance2(a, b):
    x1, y1 = a
    x2, y2 = b
    distance = (x2 - x1)**2 + (y2 - y1)**2
    return distance

def delete_ciclo(ciclo_index):
    global ciclos, one_way_jumps
    ciclos.remove(ciclos[ciclo_index])
    to_delete = []
    for dos, jump in enumerate(one_way_jumps):
        mark_for_deletion = False
        for i in (0, 2):
            if jump[i] == ciclo_index:
                mark_for_deletion = True
            if jump[i] > ciclo_index:
                one_way_jumps[dos][i] -= 1
        if mark_for_deletion:
            mark_for_deletion = False
            to_delete.append(jump)
    for t in to_delete:
        one_way_jumps.remove(t)
def delete_current(ciclo_index, point_index):
    global ciclos, one_way_jumps
    ciclos[ciclo_index].remove(ciclos[ciclo_index][point_index])
    to_delete = []
    for dos, jump in enumerate(one_way_jumps):
        mark_for_deletion = False
        for i in (0, 2):
            j = i + 1
            if jump[i] == ciclo_index and jump[j] == point_index:
                mark_for_deletion = True
            if jump[i] == ciclo_index and jump[j] >= point_index:
                one_way_jumps[dos][j] -= 1
        if mark_for_deletion:
            mark_for_deletion = False
            to_delete.append(jump)
    for t in to_delete:
        one_way_jumps.remove(t)
def rotate_ciclo(ciclo_index):
    global ciclos, one_way_jumps
    new_ciclo = [ciclos[ciclo_index][x - 1] for x in range(len(ciclos[ciclo_index]))]
    ciclos[ciclo_index] = new_ciclo
    for dos, jump in enumerate(one_way_jumps):
        for i in (0, 2):
            j = i + 1
            if jump[i] == ciclo_index:
                one_way_jumps[dos][j] += 1
                if one_way_jumps[dos][j] > len(ciclos[ciclo_index]) - 1:
                    one_way_jumps[dos][j] = 0

# Create the main function
def main():
    # Create the screen
    screen = pygame.display.set_mode((WIDTH, HEIGHT))
    pygame.display.set_caption("Image Drawing Example")

    # Load the image
    image = pygame.image.load(IMAGE_PATH)

    # Clock for controlling the frame rate
    clock = pygame.time.Clock()

    # Initial time
    current_time = pygame.time.get_ticks()

    ciclo_seleccionado = 0
    point_seleccionado = -1
    floating_seleccionado = -1
    mode = 'EXTENDING'
    # Main game loop
    running = True
    while running:
        global zoom
        new_time = pygame.time.get_ticks()
        delta_time = (new_time - current_time) / 1000.0  # Convert to seconds
        current_time = new_time

        keys = pygame.key.get_pressed()
        for event in pygame.event.get():
            if event.type == pygame.QUIT:
                running = False
            elif event.type == pygame.KEYUP:
                if event.key == pygame.K_COMMA:
                    zoom *= 0.5
                elif event.key == pygame.K_PERIOD:
                    zoom *= 2
                elif event.key == pygame.K_SLASH:
                    if mode == 'DELETING':
                        delete_current(ciclo_seleccionado, point_seleccionado)
                        if len(ciclos[ciclo_seleccionado]) == 0:
                            delete_ciclo(ciclo_seleccionado)
                    else:
                        mode = 'DELETING'
                elif event.key == pygame.K_h:
                    if mode == 'FLOATING':
                        mode = ''
                    else:
                        mouse_pos = get_inverse_position(pygame.mouse.get_pos())
                        distance = 1000000
                        element_index = 0
                        point_index = 0
                        for i, ciclo in enumerate(ciclos):
                            for j, point in enumerate(ciclo):
                                pot = calc_distance2(mouse_pos, point)
                                if pot < distance:
                                    distance = pot
                                    element_index = i
                                    point_index = j
                        ciclos.append([[y for y in x] for x in ciclos[element_index]])
                        mode = 'FLOATING'
                        floating_seleccionado = len(ciclos) - 1
                elif event.key == pygame.K_e:
                    if mode == 'EXTENDING':
                        if -1 < ciclo_seleccionado < len(ciclos):
                            ciclos[ciclo_seleccionado].append(get_inverse_position(pygame.mouse.get_pos()))
                    else:
                        mode == 'EXTENDING'
                elif event.key == pygame.K_c:
                    mouse_pos = get_inverse_position(pygame.mouse.get_pos())
                    distance = 1000000
                    element_index = 0
                    point_index = 0
                    for i, ciclo in enumerate(ciclos):
                        for j, point in enumerate(ciclo):
                            pot = calc_distance2(mouse_pos, point)
                            if pot < distance:
                                distance = pot
                                element_index = i
                                point_index = j
                    mode = 'EXTENDING'
                    ciclo_seleccionado = element_index
                    while True:
                        distance = 1000000
                        for j, point in enumerate(ciclos[element_index]):
                            pot = calc_distance2(mouse_pos, point)
                            if pot < distance:
                                distance = pot
                                point_index = j
                        if point_index == 0:
                            break
                        rotate_ciclo(ciclo_seleccionado)
                elif event.key == pygame.K_x:
                    ciclos.append([get_inverse_position(pygame.mouse.get_pos())])
                elif event.key == pygame.K_s:
                    write_points(ciclos, one_way_jumps)
                elif event.key == pygame.K_j:
                    mouse_pos = get_inverse_position(pygame.mouse.get_pos())
                    distance = 1000000
                    element_index = 0
                    point_index = 0
                    for i, ciclo in enumerate(ciclos):
                        for j, point in enumerate(ciclo):
                            pot = calc_distance2(mouse_pos, point)
                            if pot < distance:
                                distance = pot
                                element_index = i
                                point_index = j
                    if mode == 'JUMPING':
                        one_way_jumps.append([ciclo_seleccionado, point_seleccionado, element_index, point_index])
                        mode = ''
                    else:
                        mode = 'JUMPING'
                        ciclo_seleccionado = element_index
                        point_seleccionado = point_index

        # Clear the screen
        screen.fill(BLACK)

        # Draw the image on the screen
        screen.blit(get_image_with_size(image, (zoom * image.get_width() * IMAGE_SIZE, zoom * image.get_height() * IMAGE_SIZE)), get_actual_position(IMAGE_POS, image.get_height() * IMAGE_SIZE))

        # Draw lines
        for i, ciclo in enumerate(ciclos):
            if len(ciclo) == 1:
                pygame.draw.circle(screen, (0, 255, 0), get_actual_position(ciclo[0]), 10)
                if i == ciclo_seleccionado:
                    pygame.draw.circle(screen, (255, 255, 255), get_actual_position(ciclo[0]), 5)
                    if mode == 'EXTENDING':
                        pygame.draw.line(screen, (0, 255, 0), get_actual_position(ciclo[-1]), pygame.mouse.get_pos())
                    if mode == 'JUMPING':
                        pygame.draw.line(screen, (0, 0, 255), get_actual_position(ciclo[point_seleccionado]), pygame.mouse.get_pos())
            elif i == ciclo_seleccionado:
                pygame.draw.lines(screen, (255, 255, 0), True, [get_actual_position(point) for point in ciclo], 2)
                pygame.draw.lines(screen, (0, 255, 0), False, [get_actual_position(point) for point in ciclo], 2)
                if mode == 'EXTENDING':
                    pygame.draw.line(screen, (0, 255, 0), get_actual_position(ciclo[-1]), pygame.mouse.get_pos())
                if mode == 'JUMPING':
                    pygame.draw.line(screen, (0, 0, 255), get_actual_position(ciclo[point_seleccionado]), pygame.mouse.get_pos())
            else:
                pygame.draw.lines(screen, (0, 255, 0), True, [get_actual_position(point) for point in ciclo])
        for jump in one_way_jumps:
            a = ciclos[jump[0]][jump[1]]
            b = ciclos[jump[2]][jump[3]]
            pygame.draw.line(screen, (0, 0, 255), get_actual_position(a), get_actual_position(b))
        if mode == 'DELETING':
            pygame.draw.circle(screen, (255, 0, 0), pygame.mouse.get_pos(), deleting_radius * zoom)
            mouse_pos = get_inverse_position(pygame.mouse.get_pos())
            distance = 1000000
            element_index = 0
            point_index = 0
            for i, ciclo in enumerate(ciclos):
                for j, point in enumerate(ciclo):
                    pot = calc_distance2(mouse_pos, point)
                    if pot < distance:
                        distance = pot
                        element_index = i
                        point_index = j
            pygame.draw.circle(screen, (255, 0, 0), get_actual_position(ciclos[element_index][point_index]), 1 * zoom)
            ciclo_seleccionado = element_index
            point_seleccionado = point_index

        if mode == 'FLOATING':
            mouse_pos = get_inverse_position(pygame.mouse.get_pos())
            middle_point = [0, 0]
            for i, point in enumerate(ciclos[floating_seleccionado]):
                middle_point = [middle_point[j] + point[j] for j in range(2)]
            middle_point[0] /= len(ciclos[floating_seleccionado])
            middle_point[1] /= len(ciclos[floating_seleccionado])
            offsets = []
            for i, point in enumerate(ciclos[floating_seleccionado]):
                offsets.append([point[j] - middle_point[j] for j in range(2)])
            for i, point in enumerate(ciclos[floating_seleccionado]):
                point[0] = mouse_pos[0] + offsets[i][0]
                point[1] = mouse_pos[1] + offsets[i][1]

        if keys[pygame.K_UP]:
            offset[1] -= 1000 * delta_time
        if keys[pygame.K_DOWN]:
            offset[1] += 1000 * delta_time
        if keys[pygame.K_LEFT]:
            offset[0] += 1000 * delta_time
        if keys[pygame.K_RIGHT]:
            offset[0] -= 1000 * delta_time
        # if keys[pygame.K_w]:
        #     IMAGE_POS[1] += 10 * delta_time
        # if keys[pygame.K_s]:
        #     IMAGE_POS[1] -= 10 * delta_time
        # if keys[pygame.K_a]:
        #     IMAGE_POS[0] -= 10 * delta_time
        # if keys[pygame.K_d]:
        #     IMAGE_POS[0] += 10 * delta_time
        # if keys[pygame.K_p]:
        #     print(IMAGE_POS)


        # Update the display
        pygame.display.flip()

    # Quit Pygame
    pygame.quit()

# Call the main function when the script is run
if __name__ == "__main__":
    main()
    write_points(ciclos, one_way_jumps)
