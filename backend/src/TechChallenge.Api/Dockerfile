FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY src/TechChallenge.Api/TechChallenge.Api.csproj TechChallenge.Api/
COPY src/TechChallenge.Application/TechChallenge.Application.csproj TechChallenge.Application/
COPY src/TechChallenge.Domain/TechChallenge.Domain.csproj TechChallenge.Domain/
COPY src/TechChallenge.Infra/TechChallenge.Infra.csproj TechChallenge.Infra/
COPY src/TechChallenge.JwtService/TechChallenge.JwtService.csproj TechChallenge.JwtService/

RUN dotnet restore TechChallenge.Api/TechChallenge.Api.csproj

COPY src/ ./ 

WORKDIR /src/TechChallenge.Api
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "TechChallenge.Api.dll"]