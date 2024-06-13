def remove_short_lines(file_path):
    with open(file_path, 'r') as file:
        lines = file.readlines()
    
    filtered_lines = [line for line in lines if len(line.strip()) >= 8]
    
    with open(file_path, 'w') as file:
        file.writelines(filtered_lines)

# Usage example
file_path = '/home/weirdcat/Desktop/BackGazprom/AuthService/Data/Security/rockyou.txt'
remove_short_lines(file_path)
