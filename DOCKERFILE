FROM microsoft/dotnet:2.1-sdk-alpine AS build-env
WORKDIR /app

# copy everything and build
COPY . .
RUN dotnet restore 
RUN dotnet publish -c Release -o out

# build runtime image
FROM microsoft/dotnet:2.1-runtime-alpine
LABEL Maintainer="WeihanLi"
WORKDIR /app
COPY --from=build-env /app/AccountingApp/out .
EXPOSE 80
ENTRYPOINT ["dotnet", "AccountingApp.dll"]
