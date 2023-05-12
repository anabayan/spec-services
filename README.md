# spec-services


1. Run projects in tye
  `tye run ./deployments/tye/tye.yaml`
2.  Install Dapr
  1. Steps
3. docker run -d -p 10000:10000 -p 10001:10001 -p 10002:10002 \
   -v ~/Documents/Code/azurite:/data mcr.microsoft.com/azure-storage/azurite
4. macos colima better to run dapr
5.
6.
7. PH=$(echo 'password-1' | docker run --rm -i datalust/seq config hash)

mkdir -p ~/Documents/Code/logs/seq

docker run \
--name seq \
-d \
--restart unless-stopped \
-e ACCEPT_EULA=Y \
-e SEQ_FIRSTRUN_ADMINPASSWORDHASH="$PH" \
-v ~/Documents/Code/logs/seq:/data \
-p 80:80 \
-p 5341:5341 \
datalust/seq

seq user is admin


Issue with routing with versions dpar subscription(https://github.com/dapr/dotnet-sdk/issues/791)


Building docker images


```dotnet publish --os linux --arch x64 -p:PublishProfile=DefaultContainer -p:ContainerImageName=appliedai-services```
