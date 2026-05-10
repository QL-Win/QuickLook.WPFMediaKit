dotnet build "..\Source\WPF MediaKit.csproj" -c Release
nuget pack Package.nuspec -OutputDirectory .
@pause
