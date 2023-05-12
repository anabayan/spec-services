FROM mcr.microsoft.com/dotnet/sdk:7.0 as builder
LABEL stage=builder

WORKDIR /src

COPY ./.editorconfig ./
COPY ./global.json ./

COPY ./src/Directory.Build.props ./
COPY ./src/Directory.Build.targets ./
COPY ./src/Directory.Packages.props ./
COPY ./src/Packages.props ./
COPY ./src/Services/AppliedAIService/Directory.Build.props ./Services/AppliedAIService/

# TODO: Using wildcard to copy all files in the directory.
COPY ./src/BuildingBlocks/BuildingBlocks.Abstractions/BuildingBlocks.Abstractions.csproj ./BuildingBlocks/BuildingBlocks.Abstractions/
COPY ./src/BuildingBlocks/BuildingBlocks.Caching/BuildingBlocks.Caching.csproj ./BuildingBlocks/BuildingBlocks.Caching/
COPY ./src/BuildingBlocks/BuildingBlocks.Core/BuildingBlocks.Core.csproj ./BuildingBlocks/BuildingBlocks.Core/
COPY ./src/BuildingBlocks/BuildingBlocks.Dapr/BuildingBlocks.Dapr.csproj ./BuildingBlocks/BuildingBlocks.Dapr/
COPY ./src/BuildingBlocks/BuildingBlocks.HealthCheck/BuildingBlocks.HealthCheck.csproj ./BuildingBlocks/BuildingBlocks.HealthCheck/
COPY ./src/BuildingBlocks/BuildingBlocks.Logging/BuildingBlocks.Logging.csproj ./BuildingBlocks/BuildingBlocks.Logging/
COPY ./src/BuildingBlocks/BuildingBlocks.OpenTelemetry/BuildingBlocks.OpenTelemetry.csproj ./BuildingBlocks/BuildingBlocks.OpenTelemetry/
COPY ./src/BuildingBlocks/BuildingBlocks.Resiliency/BuildingBlocks.Resiliency.csproj ./BuildingBlocks/BuildingBlocks.Resiliency/
COPY ./src/BuildingBlocks/BuildingBlocks.Swagger/BuildingBlocks.Swagger.csproj ./BuildingBlocks/BuildingBlocks.Swagger/
COPY ./src/BuildingBlocks/BuildingBlocks.Validation/BuildingBlocks.Validation.csproj ./BuildingBlocks/BuildingBlocks.Validation/
COPY ./src/BuildingBlocks/BuildingBlocks.Web/BuildingBlocks.Web.csproj ./BuildingBlocks/BuildingBlocks.Web/

COPY ./src/Services/AppliedAIService/JCR.Services.AppliedAIService/JCR.Services.AppliedAIService.csproj ./Services/AppliedAIService/JCR.Services.AppliedAIService/
COPY ./src/Services/AppliedAIService/JCR.Services.AppliedAIService.Api/JCR.Services.AppliedAIService.Api.csproj ./Services/AppliedAIService/JCR.Services.AppliedAIService.Api/

RUN ls

# restore nuget packages
RUN dotnet restore ./Services/AppliedAIService/JCR.Services.AppliedAIService.Api/JCR.Services.AppliedAIService.Api.csproj

# Copy project files
COPY ./src/BuildingBlocks/ ./BuildingBlocks/
COPY ./src/Services/AppliedAIService/JCR.Services.AppliedAIService.Api/  ./Services/AppliedAIService/JCR.Services.AppliedAIService.Api/
COPY ./src/Services/AppliedAIService/JCR.Services.AppliedAIService/  ./Services/AppliedAIService/JCR.Services.AppliedAIService/

# Build project with Release configuration
# and no restore, as we did it already
RUN dotnet build -c Release --no-restore  ./Services/AppliedAIService/JCR.Services.AppliedAIService.Api/JCR.Services.AppliedAIService.Api.csproj --runtime linux-x64 --self-contained

WORKDIR /src/Services/AppliedAIService/JCR.Services.AppliedAIService.Api/
# Publish project to output folder
# and no build, as we did it already
RUN ls
RUN dotnet publish -c Release --no-build -o out --self-contained --runtime linux-x64

FROM mcr.microsoft.com/dotnet/aspnet:7.0
# Setup working directory for the project
WORKDIR /app
COPY --from=builder /src/Services/AppliedAIService/JCR.Services.AppliedAIService.Api/out  .

EXPOSE 80
ENV ASPNETCORE_URLS http://*:80
ENV ASPNETCORE_ENVIRONMENT docker
ENTRYPOINT ["dotnet", "JCR.Services.AppliedAIService.Api.dll"]
