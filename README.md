# Content Config Cleaner

## Overview

This repository contains the source code for a custom application developed for a client. The purpose of the application is to process a large number of folders, each containing about 10,000 files, and search for specific content within `config.ini` files. The application then identifies identical content across these files, deletes corresponding content and lines, preserves the original content, moves the remaining files back to their original folders, and updates references in the affected folders to the original `config.ini` file. The entire process is designed to efficiently clean and organize the data.

## Features

- **Bulk Processing**: The application processes a substantial number of folders, handling about 10,000 files in each, optimizing the cleanup operation.

- **Content Comparison**: Identical content within `config.ini` files is identified across the folders for targeted removal.

- **Preservation of Original Content**: The application ensures that the original content and lines in the `config.ini` files are preserved during the cleaning process.

- **Folder Organization**: Cleaned folders are moved back to their original locations, maintaining the overall structure.

- **Efficiency**: The project boasts impressive efficiency, completing the processing of approximately 10,000 folders in around 15 minutes.


## Usage

- Provide the input folders containing the data to be processed.

- Execute the application to initiate the cleaning process.

- Monitor the progress, and upon completion, find the cleaned folders with updated `config.ini` files in their original locations.


## License

This project is licensed under the [MIT License](LICENSE). See the [LICENSE](LICENSE) file for details.
