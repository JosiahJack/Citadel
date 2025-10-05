#!/bin/bash

# Check if a file is provided as an argument
if [ $# -ne 1 ]; then
    echo "Usage: $0 <savegame_file>"
    exit 1
fi

SAVE_FILE="$1"

# Check if the file exists
if [ ! -f "$SAVE_FILE" ]; then
    echo "Error: File '$SAVE_FILE' not found."
    exit 1
fi

# Extract keys (text before ':') from entries containing ':', remove array indices, remove duplicates, and print each on a new line
cat "$SAVE_FILE" | tr '|' '\n' | grep ':' | cut -d':' -f1 | sed 's/\[[0-9]*\]//' | sort | uniq | while read -r key; do
    echo "$key"
done

# Count unique keys
unique_count=$(cat "$SAVE_FILE" | tr '|' '\n' | grep ':' | cut -d':' -f1 | sed 's/\[[0-9]*\]//' | sort | uniq | wc -l)
echo "Total unique keys: $unique_count"
