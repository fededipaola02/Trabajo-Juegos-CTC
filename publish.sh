#!/bin/bash
# Descargar e instalar .NET SDK en el servidor de Render
wget https://dot.net/v1/dotnet-install.sh -O dotnet-install.sh
chmod +x ./dotnet-install.sh
./dotnet-install.sh --channel 8.0

# Agregar dotnet al PATH
export PATH="$PATH:$HOME/.dotnet"

# Publicar el proyecto
dotnet publish -c Release -o release
