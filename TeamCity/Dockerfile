FROM jetbrains/teamcity-agent:latest

# Instalacja zależności systemowych
USER root
RUN apt-get update && apt-get install -y \
    wget \
    curl \
    apt-transport-https \
    ca-certificates \
    gnupg

# Pobranie repozytorium .NET 8
RUN wget https://packages.microsoft.com/config/ubuntu/$(lsb_release -rs)/packages-microsoft-prod.deb -O packages-microsoft-prod.deb \
    && dpkg -i packages-microsoft-prod.deb \
    && rm packages-microsoft-prod.deb

# Instalacja .NET 8 SDK
RUN apt-get update && apt-get install -y dotnet-sdk-8.0

# Ustawienie ścieżki do .NET
ENV DOTNET_ROOT=/usr/share/dotnet
ENV PATH="$PATH:/usr/share/dotnet"

USER buildagent