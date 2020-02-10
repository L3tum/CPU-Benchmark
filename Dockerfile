FROM --platform $TARGETPLATFORM mcr.microsoft.com/dotnet/core/runtime:3.0 AS base
ARG BUILD_VERSION
ENV VERSION=$BUILD_VERSION
WORKDIR /app

FROM --platform $BUILDPLATFORM mcr.microsoft.com/dotnet/core/sdk:3.0 AS build
WORKDIR /src
COPY ["Benchmarker/Benchmarker.csproj", "Benchmarker/"]
RUN dotnet restore "Benchmarker/Benchmarker.csproj"
COPY . .
WORKDIR "/src/Benchmarker"
RUN dotnet build "Benchmarker.csproj" --framework netcoreapp3.0 -c Release -o /app

FROM build AS publish
RUN dotnet publish "Benchmarker.csproj" --framework netcoreapp3.0 -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Benchmarker.dll"]
CMD ["--help"]