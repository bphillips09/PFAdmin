function CreateDocker() {
    [string] $Path = $PSScriptRoot
    Write-Host "`n`nChecking Credentials...`n`n" -ForegroundColor Green
    $CredentialsFile = $Path + "\Credentials"
    $values = [pscustomobject](Get-Content ($CredentialsFile) -Raw | ConvertFrom-StringData)
    $Repo = $values.Repo
    $User = $values.User
    $Pass = $values.Pass
    Set-Location ($Path + "\image")
    Write-Host "`n"
    $ServerTag = Read-Host -Prompt 'Enter a server name'
    Write-Host "`n"
    $PortNumber = Read-Host -Prompt 'Enter Port Number'
    $ImageTag = $Repo + "/" + $ServerTag
    Write-Host "`n`nBegin Build...`n`n" -ForegroundColor Green
    docker build -t $ImageTag .
    $portArgTCP = "-p " + $PortNumber + ":" + $PortNumber
    $portArgUDP = "-p " + $PortNumber + ":" + $PortNumber + "/udp"
    $cmd = "docker run -dit " + ($portArgTCP, $portArgUDP, $ImageTag) -join " "
    Write-Host "`n`nBegin Running...`n`n" -ForegroundColor Green
	Write-Host "`n`nRemoving Credentials File...`n`n" -ForegroundColor Green
    if (Test-Path $CredentialsFile) {
        Remove-Item $CredentialsFile
    }
    Write-Host "`n`nWaiting for container to run...`n`n" -ForegroundColor Green
    Invoke-Expression $cmd
    Write-Host "`n`nWaiting for Docker response...`n`n" -ForegroundColor Green
    Start-Sleep -s 5
    Write-Host "`n`nGetting Build ID...`n`n" -ForegroundColor Green
    $BuildID = docker ps -q
    Write-Host "`n`nCommitting changes to Docker...`n`n" -ForegroundColor Green
    docker commit $BuildID $ImageTag | Out-Null
    Write-Host "`n`nLogging into Azure Repo...`n`n" -ForegroundColor Green
    [string] $LoginResult = Write-Output "$Pass" | docker login -u $User --password-stdin $Repo
    if ($LoginResult -match "Login Succeeded") {
        Write-Host $LoginResult -ForegroundColor White
    } else {
        Write-Host "`n`nAzure Login Failed!`n`n" -ForegroundColor Red
        return;
    }
    Write-Host "`n`nPushing to Azure Repo...`n`n" -ForegroundColor Green
    docker push $ImageTag
    Write-Host "`n`nStopping running container...`n`n" -ForegroundColor Green
    docker stop $BuildID | Out-Null
    Write-Host "`n`nFINISHED!`n" -ForegroundColor Green
    Write-Host "`nReturn to the Admin app to submit the build!`n`n" -ForegroundColor Magenta
}

$ErrorActionPreference = "Stop"

Write-Host "`nStarting Up..."
CreateDocker;