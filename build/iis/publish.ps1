$config = Import-PowershellDataFile $PSScriptRoot\config.psd1
$appsettings = $null
$stopFile = "$($config.applicationFolder)\app_offline.htm"
$buildFolder = $PSScriptRoot
$projectFolder = "$PSScriptRoot\..\..\src\UI"

try {
    New-Item $stopFile -ItemType file -ErrorAction SilentlyContinue

    Remove-Item "$($config.backupFolder)\*" -Force -Recurse -Confirm:$false

    Get-ChildItem -Path $config.applicationFolder | `
        Copy-Item -Destination $config.backupFolder -Recurse -Container

    $checkAppsettings = Test-Path -Path "$($config.applicationFolder)\appsettings.json" -PathType Leaf

    if ($checkAppsettings) {
        $appsettings = Get-Content "$($config.applicationFolder)\appsettings.json" -Raw
    }

    Remove-Item "$($config.applicationFolder)\*" -Force -Recurse -Confirm:$false

    Push-Location $projectFolder

    Exec {
        dotnet publish `
            --configuration Release `
            --no-self-contained `
            --output $config.applicationFolder
    }

    if (!$checkAppsettings) {
        $appsettings = Get-Content "$($config.applicationFolder)\appsettings.json" -Raw
    }

    Move-Item "$($config.applicationFolder)\appsettings.json" "$($config.applicationFolder)\appsettings.json.build"

    $appsettings | Out-File "$($config.applicationFolder)\appsettings.json" -Force
}
finally {
    Remove-Item $stopFile -Force -Confirm:$false -ErrorAction SilentlyContinue
    Pop-Location
}

# $config | ForEach-Object {
#     $apps = $_.Values
#     $apps | ForEach-Object {
#         try {
#             Write-Host "Publishing $($_.name) to $($_.location)" -ForegroundColor Magenta

#             Push-Location "$PSScriptRoot\..\..\src\$($_.name)"

#             $checkAppsettings = Test-Path -Path "$($_.location)\appsettings.json" -PathType Leaf

#             if ($checkAppsettings) {
#                 $appsettings = Get-Content "$($_.location)\appsettings.json" -Raw
#                 Remove-Item "$($_.location)\*" -Exclude appsettings.json.tmp
#             }

#             Exec {
#                 dotnet publish `
#                     --configuration Release `
#                     --no-self-contained `
#                     --runtime win10-x64 `
#                     --output $_.location
#             }

#             if (!$checkAppsettings) {
#                 $appsettings = Get-Content "$($_.location)\appsettings.json" -Raw
#             }

#             Move-Item "$($_.location)\appsettings.json" "$($_.location)\appsettings.json.build"
#         }
#         finally {
#             $appsettings | Out-File "$($_.location)\appsettings.json" -Force
#             Pop-Location
#         }
#     }
# }