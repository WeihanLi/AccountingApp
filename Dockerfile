# mcr.microsoft.com/dotnet/core/sdk:2.2-alpine for later use
FROM microsoft/dotnet:2.2-sdk-alpine AS build-env
WORKDIR /app

# copy everything and build
COPY . .
RUN dotnet restore && \
    dotnet publish -c Release -o out

# build runtime image
# mcr.microsoft.com/dotnet/core/aspnet:2.2-alpine for later use
FROM microsoft/dotnet:2.2-aspnetcore-runtime-alpine
LABEL Maintainer="WeihanLi"
WORKDIR /app
COPY --from=build-env /app/AccountingApp/out .
EXPOSE 80
ENTRYPOINT ["dotnet", "AccountingApp.dll"]
