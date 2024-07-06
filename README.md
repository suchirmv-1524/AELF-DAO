# AElf DevContainer (BETA)

To use this devcontainer, click "Use this template" > "Create a new repository".

## Pre-requisites

- Docker: https://www.docker.com/
- VS Code: https://code.visualstudio.com/

## Create a new project

Ensure that you have Docker installed and running.

Clone your new repository to your local, then open the folder in VS Code.

Press `F1` in VS Code and type/choose `Dev Containers: Reopen in Container`.

## Scaffolding a new project

To scaffold a new project, use:

```bash
dotnet new aelf -n HelloWorld
```

Note that if you change `HelloWorld` to some other name, you will need to amend your `.github/workflows/main.yml`:
