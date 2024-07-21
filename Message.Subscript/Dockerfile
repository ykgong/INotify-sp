#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Message.Subscript/Message.Subscript.Server.csproj", "Message.Subscript/"]
COPY ["Common.Notify/Common.Notify.csproj", "Common.Notify/"]
RUN dotnet restore "./Message.Subscript/./Message.Subscript.Server.csproj"
COPY . .
WORKDIR "/src/Message.Subscript"
RUN dotnet build "./Message.Subscript.Server.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Message.Subscript.Server.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Message.Subscript.Server.dll","--urls","http://*:8080"]