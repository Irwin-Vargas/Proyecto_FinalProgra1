# Etapa 1: build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

COPY *.csproj ./
RUN dotnet restore

COPY . ./
RUN dotnet publish -c Release -o /out

# Etapa 2: runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /out .
# Copiar explícitamente el modelo ML si no quedó en la publish
COPY MLModels/product_popularity_model.zip ./MLModels/product_popularity_model.zip

EXPOSE 80
EXPOSE 443

ENTRYPOINT ["dotnet", "Proyecto_FinalProgra1.dll"]
