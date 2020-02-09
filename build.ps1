param(
	[parameter(Mandatory=$false)]
    [String]$v
)

$version="0.6.0"

if($PSBoundParameters.ContainsKey("v") -and $v.Length -gt 0){
    $version = $v.TrimStart("v");
}

Write-Host "Building version ${version}"

cd Benchmarker
if([System.IO.File]::Exists("bin/Release")){
    Remove-Item -Path bin/Release -Recurse
}
mkdir bin/Release
dotnet restore Benchmarker.csproj

dotnet publish Benchmarker.csproj --framework netcoreapp3.0 -c Release -o bin/Release/Benchmarker /p:VersionSuffix=$version
dotnet publish Benchmarker.csproj --framework netcoreapp3.0 -c Release -r win-x64 /p:PublishSingleFile=true /p:PublishTrimmed=true -o bin/Release/Benchmarker-win64 /p:Version=$version-win64
dotnet publish Benchmarker.csproj --framework netcoreapp3.0 -c Release -r win-x86 /p:PublishSingleFile=true /p:PublishTrimmed=true -o bin/Release/Benchmarker-win32 /p:Version=$version-win86
dotnet publish Benchmarker.csproj --framework netcoreapp3.0 -c Release -r linux-arm /p:PublishSingleFile=true /p:PublishTrimmed=true -o bin/Release/Benchmarker-linux-arm32v7 /p:Version=$version-linuxarm
dotnet publish Benchmarker.csproj --framework netcoreapp3.0 -c Release -r win-arm /p:PublishSingleFile=true /p:PublishTrimmed=true -o bin/Release/Benchmarker-win-arm32v7 /p:Version=$version-winarm
dotnet publish Benchmarker.csproj --framework netcoreapp3.0 -c Release -r win-arm64 /p:PublishSingleFile=true /p:PublishTrimmed=true -o bin/Release/Benchmarker-win-arm64 /p:Version=$version-winarm64
dotnet publish Benchmarker.csproj --framework netcoreapp3.0 -c Release -r linux-x64 /p:PublishSingleFile=true /p:PublishTrimmed=true -o bin/Release/Benchmarker-linux64 /p:Version=$version-linux64
dotnet publish Benchmarker.csproj --framework netcoreapp3.0 -c Release -r debian-x64 /p:PublishSingleFile=true /p:PublishTrimmed=true -o bin/Release/Benchmarker-debian64 /p:Version=$version-debain64
dotnet publish Benchmarker.csproj --framework netcoreapp3.0 -c Release -r ubuntu-x64 /p:PublishSingleFile=true /p:PublishTrimmed=true -o bin/Release/Benchmarker-ubuntu64 /p:Version=$version-ubuntu64
dotnet publish Benchmarker.csproj --framework netcoreapp3.0 -c Release -r osx-x64 /p:PublishSingleFile=true /p:PublishTrimmed=true -o bin/Release/Benchmarker-osx64 /p:Version=$version-osx64

Copy-Item ../README.md bin/Release/Benchmarker/README.md
Copy-Item ../README.md bin/Release/Benchmarker-win64/README.md
Copy-Item ../README.md bin/Release/Benchmarker-win32/README.md
Copy-Item ../README.md bin/Release/Benchmarker-linux-arm32v7/README.md
Copy-Item ../README.md bin/Release/Benchmarker-win-arm32v7/README.md
Copy-Item ../README.md bin/Release/Benchmarker-win-arm64/README.md
Copy-Item ../README.md bin/Release/Benchmarker-linux64/README.md
Copy-Item ../README.md bin/Release/Benchmarker-debian64/README.md
Copy-Item ../README.md bin/Release/Benchmarker-ubuntu64/README.md
Copy-Item ../README.md bin/Release/Benchmarker-osx64/README.md

Copy-Item ../LICENSE bin/Release/Benchmarker/LICENSE
Copy-Item ../LICENSE bin/Release/Benchmarker-win64/LICENSE
Copy-Item ../LICENSE bin/Release/Benchmarker-win32/LICENSE
Copy-Item ../LICENSE bin/Release/Benchmarker-linux-arm32v7/LICENSE
Copy-Item ../LICENSE bin/Release/Benchmarker-win-arm32v7/LICENSE
Copy-Item ../LICENSE bin/Release/Benchmarker-win-arm64/LICENSE
Copy-Item ../LICENSE bin/Release/Benchmarker-linux64/LICENSE
Copy-Item ../LICENSE bin/Release/Benchmarker-debian64/LICENSE
Copy-Item ../LICENSE bin/Release/Benchmarker-ubuntu64/LICENSE
Copy-Item ../LICENSE bin/Release/Benchmarker-osx64/LICENSE

mkdir bin/Release/ZIPs

Compress-Archive -Path bin/Release/Benchmarker -DestinationPath bin/Release/ZIPs/Benchmarker.zip
Compress-Archive -Path bin/Release/Benchmarker-win64 -DestinationPath bin/Release/ZIPs/Benchmarker-win64.zip
Compress-Archive -Path bin/Release/Benchmarker-win32 -DestinationPath bin/Release/ZIPs/Benchmarker-win32.zip
Compress-Archive -Path bin/Release/Benchmarker-linux-arm32v7 -DestinationPath bin/Release/ZIPs/Benchmarker-linux-arm32v7.zip
Compress-Archive -Path bin/Release/Benchmarker-win-arm32v7 -DestinationPath bin/Release/ZIPs/Benchmarker-win-arm32v7.zip
Compress-Archive -Path bin/Release/Benchmarker-win-arm64 -DestinationPath bin/Release/ZIPs/Benchmarker-win-arm64.zip
Compress-Archive -Path bin/Release/Benchmarker-linux64 -DestinationPath bin/Release/ZIPs/Benchmarker-linux64.zip
Compress-Archive -Path bin/Release/Benchmarker-debian64 -DestinationPath bin/Release/ZIPs/Benchmarker-debian64.zip
Compress-Archive -Path bin/Release/Benchmarker-ubuntu64 -DestinationPath bin/Release/ZIPs/Benchmarker-ubuntu64.zip
Compress-Archive -Path bin/Release/Benchmarker-osx64 -DestinationPath bin/Release/ZIPs/Benchmarker-osx64.zip

Remove-Item -Recurse -Path bin/Release/Benchmarker
Remove-Item -Recurse -Path bin/Release/Benchmarker-win64
Remove-Item -Recurse -Path bin/Release/Benchmarker-win32
Remove-Item -Recurse -Path bin/Release/Benchmarker-linux-arm32v7
Remove-Item -Recurse -Path bin/Release/Benchmarker-win-arm32v7
Remove-Item -Recurse -Path bin/Release/Benchmarker-win-arm64
Remove-Item -Recurse -Path bin/Release/Benchmarker-linux64
Remove-Item -Recurse -Path bin/Release/Benchmarker-debian64
Remove-Item -Recurse -Path bin/Release/Benchmarker-ubuntu64
Remove-Item -Recurse -Path bin/Release/Benchmarker-osx64