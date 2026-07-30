import os
import re
import json

def snake_to_pascal(snake_str):
    """Convert snake_case string to PascalCase"""
    components = snake_str.split('_')
    return ''.join(component.capitalize() for component in components)

def convert_keys_to_pascal_case(content):
    """Convert snake_case keys to PascalCase in JSON/TSV content"""
    # Pattern to match keys in JSON format: "key_name": 
    # This regex looks for quoted strings followed by a colon
    pattern = r'"([a-z][a-z0-9_]*[a-z0-9])"\s*:'
    
    def replace_key(match):
        original_key = match.group(1)
        # Convert to PascalCase
        pascal_key = snake_to_pascal(original_key)
        return f'"{pascal_key}":'
    
    # Apply the replacement
    converted_content = re.sub(pattern, replace_key, content)
    return converted_content

def process_json_to_tsv():
    """Convert JSON files to TSV files"""
    current_dir = os.getcwd()
    print(f"Processing files in directory: {current_dir}")
    
    # Find all JSON files
    json_files = [f for f in os.listdir(current_dir) if f.endswith('.json')]
    
    print(f"Found {len(json_files)} JSON files")
    
    converted_files = []
    
    for filename in json_files:
        json_filepath = os.path.join(current_dir, filename)
        # Create TSV filename by replacing .json extension with .tsv
        tsv_filename = filename[:-5] + '.tsv'
        tsv_filepath = os.path.join(current_dir, tsv_filename)
        
        print(f"Converting {filename} to {tsv_filename}")
        
        try:
            # Rename/convert JSON to TSV (simply change extension)
            if os.path.exists(tsv_filepath):
                print(f"TSV file {tsv_filename} already exists, will overwrite")
            
            # Read the JSON file content
            with open(json_filepath, 'r', encoding='utf-8') as file:
                content = file.read()
            
            # Convert snake_case keys to PascalCase
            converted_content = convert_keys_to_pascal_case(content)
            
            # Write to TSV file
            with open(tsv_filepath, 'w', encoding='utf-8') as file:
                file.write(converted_content)
                
            # Add to list of converted files
            converted_files.append((json_filepath, tsv_filename))
            print(f"Successfully converted: {filename} -> {tsv_filename}")
            
        except Exception as e:
            print(f"Error converting {filename}: {str(e)}")
    
    # Delete original JSON files after successful conversion
    for json_filepath, tsv_filename in converted_files:
        try:
            os.remove(json_filepath)
            print(f"Deleted original JSON file: {os.path.basename(json_filepath)}")
        except Exception as e:
            print(f"Error deleting {os.path.basename(json_filepath)}: {str(e)}")
    
    return len(converted_files)

def process_tsv_files():
    """Process all TSV files in the current directory"""
    current_dir = os.getcwd()
    print(f"Processing files in directory: {current_dir}")
    
    # Find all TSV files
    tsv_files = [f for f in os.listdir(current_dir) if f.endswith('.tsv')]
    
    print(f"Found {len(tsv_files)} TSV files")
    
    for filename in tsv_files:
        filepath = os.path.join(current_dir, filename)
        print(f"Processing file: {filename}")
        
        try:
            # Read the file content
            with open(filepath, 'r', encoding='utf-8') as file:
                content = file.read()
            
            # Convert snake_case keys to PascalCase
            converted_content = convert_keys_to_pascal_case(content)
            
            # Write back to the file
            with open(filepath, 'w', encoding='utf-8') as file:
                file.write(converted_content)
                
            print(f"Successfully converted: {filename}")
            
        except Exception as e:
            print(f"Error processing {filename}: {str(e)}")

if __name__ == "__main__":
    # Step 1: Check for JSON files and convert them to TSV
    converted_count = process_json_to_tsv()
    
    # Step 2 & 3: Process all TSV files (including newly converted ones)
    process_tsv_files()
    
    print(f"All files have been processed. Converted {converted_count} JSON files to TSV and deleted originals.")