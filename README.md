# EuroSkills Düsseldorf 2027 | Hungary | Round 1
Solution by Bori Krisztián Sándor

[AI Statement](./ai_statement.md)

## Prerequisites
- Download and install [Node.js](https://nodejs.org/en/download)
- Download and install [.NET SDK 8](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- Clone the repository
  ```sh
   git clone https://github.com/b-krisz/es2027-s09-r1-172.git
   ```
- Open a terminal/command-line/shell in the repository directory

## Installation

### JSON-SERVER

1. Change the working directory to the JSON-SERVER
    ```sh
    cd ./JSON-SERVER/
    ```
2. Install JSON-SERVER and its dependencies
    ```sh
    npm install
    ```
## Usage

### JSON-SERVER

1. Change the working directory to the JSON-SERVER
    ```sh
    cd ./JSON-SERVER/
    ```
2. Start the JSON-SERVER with the database
    ```sh
    npx json-server <database_file>
    ```
    **e.g. ```npx json-server ./database.json```**

    *Defaultly the JSON-SERVER runs on port **3000***

### Command Line Tool

1. Change the working directory to the Command Line Tool
    ```sh
    cd ./CommandLineTool/
    ```
    
2. *It is possible to run the program both with the executable manually or with the dotnet command*
    - Executable
        ```sh
        cd ./bin/Release/net8.0/
        ./CommandLineTool.exe <folder_path> <json_server_url>
        ```
        **e.g. ```./CommandLineTool.exe  C:/Users/user/Desktop/Gurum http://localhost:3000/projects```**
    - Dotnet
        ```sh
        dotnet run <folder_path> <json_server_url>
        ```
        **e.g. ```dotnet run C:/Users/user/Desktop/Gurum http://localhost:3000/projects```**

### REST API

1. Change the working directory to the REST API
    ```sh
    cd ./RESTAPI/
    ```
    
2. *It is possible to run the program both with the executable manually or with the dotnet command*
    - Executable
        ```sh
        cd ./bin/Release/net8.0/
        ./RESTAPI.exe --urls <rest_api_url>
        ```
        **e.g. ```./RESTAPI.exe --urls http://localhost:3001```**
    - Dotnet
        ```sh
        dotnet run --urls <rest_api_url>
        ```
        **e.g. ```dotnet run --urls http://localhost:3001```**