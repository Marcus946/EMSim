# The following code was written largely by Chat-GPT

import os
import shutil
import sys

# Constants
DLL_NAME = r"bin\EMSimDLL.dll"
UNITY_PLUGIN_DIR = r"Simulations\Assets\Plugins\EMSimDLL.dll"

def copy_dll(solution_dir):
    # Construct full paths
    source = os.path.join(solution_dir, DLL_NAME)
    destination = os.path.join(solution_dir, UNITY_PLUGIN_DIR)

    try:
        # Ensure the source DLL exists
        if not os.path.exists(source):
            raise FileNotFoundError(f"{DLL_NAME} not found in {solution_dir}")

        # Create the Plugins folder if it doesn't exist
        os.makedirs(UNITY_PLUGIN_DIR, exist_ok=True)

        # Copy the DLL to the destination
        shutil.copy2(source, destination)
        print(f"Successfully copied {DLL_NAME} to {UNITY_PLUGIN_DIR}")

    except Exception as e:
        print(f"Error: {e}")

if __name__ == "__main__":
    if len(sys.argv) != 2:
        print("Usage: python copy_dll.py <solution_dir>")
        sys.exit(1)

    solution_dir = sys.argv[1][:-1]
    copy_dll(solution_dir)
