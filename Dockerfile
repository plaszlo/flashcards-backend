# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:10000
EXPOSE 10000

# Build image
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["FlashcardBackend.csproj", "./"]
RUN dotnet restore "FlashcardBackend.csproj"

COPY . .
RUN dotnet publish "FlashcardBackend.csproj" -c Release -o /app/publish

# Final image
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "FlashcardBackend.dll"]