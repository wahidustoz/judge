FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env
WORKDIR /App

COPY ./ ./
RUN dotnet restore
RUN dotnet publish src/Ilmhub.Judge.Api/Ilmhub.Judge.Api.csproj -c Release -o out

FROM ubuntu:18.04

COPY ./src/Ilmhub.Judge.Api/dependencies.sh .
RUN chmod +x dependencies.sh && ./dependencies.sh && rm -f dependencies.sh

WORKDIR /App
COPY --from=build-env /App/out .
EXPOSE 5000
ENV COMPlus_EnableDiagnostics=0
ENV ASPNETCORE_URLS=http://*:5000
ENTRYPOINT ["dotnet", "Ilmhub.Judge.Api.dll"]