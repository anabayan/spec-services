#!/bin/bash 
 
echo "on-create start" >> ~/status
 
# install dapr cli
wget -q https://raw.githubusercontent.com/dapr/cli/master/install/install.sh -O - | /bin/bash

dotnet tool install -g Microsoft.Tye --version "0.11.0-alpha.22111.1" | /bin/bash  

dotnet dev-certs https

# initialize dapr
dapr init

#dotnet dev certs
 
echo "on-create complete" >> ~/status