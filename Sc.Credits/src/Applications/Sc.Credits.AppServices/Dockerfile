FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-bionic AS base
WORKDIR /app
RUN apt-get update -qq && apt-get -y install libgdiplus libc6-dev tzdata
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["Build/dependencies.props", "build/"]
COPY ["Sc.Credits/src/Applications/Sc.Credits.AppServices/Sc.Credits.AppServices.csproj", "Sc.Credits/src/Applications/Sc.Credits.AppServices/"]
COPY ["Sc.Credits/src/Applications/Sc.Credits.AppServices/nuget.config", "Sc.Credits/src/Applications/Sc.Credits.AppServices/"]
COPY ["Sc.Credits/src/Domain/Sc.Credits.Domain.Managment/Sc.Credits.Domain.Managment.csproj", "Sc.Credits/src/Domain/Sc.Credits.Domain.Managment/"]
COPY ["Sc.Credits/src/Domain/Sc.Credits.Domain.UseCase/Sc.Credits.Domain.UseCase.csproj", "Sc.Credits/src/Domain/Sc.Credits.Domain.UseCase/"]
COPY ["Sc.Credits/src/Domain/Sc.Credits.Domain.Model/Sc.Credits.Domain.Model.csproj", "Sc.Credits/src/Domain/Sc.Credits.Domain.Model/"]
COPY ["Sc.Credits/src/Infrastructure/Helpers/Sc.Credits.Helpers.ObjectsUtils/Sc.Credits.Helpers.ObjectsUtils.csproj", "Sc.Credits/src/Infrastructure/Helpers/Sc.Credits.Helpers.ObjectsUtils/"]
COPY ["Sc.Credits/src/Infrastructure/Helpers/Sc.Credits.Helpers.Commons/Sc.Credits.Helpers.Commons.csproj", "Sc.Credits/src/Infrastructure/Helpers/Sc.Credits.Helpers.Commons/"]
COPY ["Sc.Credits/src/Infrastructure/DrivenAdapters/Sc.Credits.DrivenAdapters.AzureStorage/Sc.Credits.DrivenAdapters.AzureStorage.csproj", "Sc.Credits/src/Infrastructure/DrivenAdapters/Sc.Credits.DrivenAdapters.AzureStorage/"]
COPY ["Sc.Credits/src/Infrastructure/DrivenAdapters/Sc.Credits.DrivenAdapters.ServiceBus/Sc.Credits.DrivenAdapters.ServiceBus.csproj", "Sc.Credits/src/Infrastructure/DrivenAdapters/Sc.Credits.DrivenAdapters.ServiceBus/"]
COPY ["Sc.Credits/src/Infrastructure/DrivenAdapters/Sc.Credits.DrivenAdapters.SqlServer/Sc.Credits.DrivenAdapters.SqlServer.csproj", "Sc.Credits/src/Infrastructure/DrivenAdapters/Sc.Credits.DrivenAdapters.SqlServer/"]
COPY ["Sc.Credits/src/Infrastructure/EntryPoints/Sc.Credits.EntryPoints.ReactiveWeb/Sc.Credits.EntryPoints.ReactiveWeb.csproj", "Sc.Credits/src/Infrastructure/EntryPoints/Sc.Credits.EntryPoints.ReactiveWeb/"]
COPY ["Sc.Credits/src/Infrastructure/EntryPoints/Sc.Credits.EntryPoints.ServicesBus/Sc.Credits.EntryPoints.ServicesBus.csproj", "Sc.Credits/src/Infrastructure/EntryPoints/Sc.Credits.EntryPoints.ServicesBus/"]
COPY ["Sc.Credits/src/Applications/Sc.Credits.AppServices/nuget.config", "Sc.Credits/src/Applications/Sc.Credits.AppServices/"]
RUN dotnet restore "Sc.Credits/src/Applications/Sc.Credits.AppServices/Sc.Credits.AppServices.csproj" --configfile "Sc.Credits/src/Applications/Sc.Credits.AppServices/nuget.config" --packages packages
COPY . .
WORKDIR "/src/Sc.Credits/src/Applications/Sc.Credits.AppServices"
RUN dotnet build "Sc.Credits.AppServices.csproj" -c Release -o /app --configfile "nuget.config" --packages packages

FROM build AS publish
RUN dotnet publish "Sc.Credits.AppServices.csproj" -c Release -o /app --configfile "nuget.config" --packages packages


#Set timezone
RUN echo "America/Bogota" > /etc/timezone
RUN dpkg-reconfigure -f noninteractive tzdata


FROM base AS final
WORKDIR /app
COPY --from=publish /app .
RUN chmod 755 /app/Infrastructure/HtmlToPdf/Linux/wkhtmltopdf
ENTRYPOINT ["dotnet", "Sc.Credits.Applications.AppServices.dll"]