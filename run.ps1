Param (
    [Parameter(Position = 0)]
    [System.String]$paramService = "all",

    [Parameter(Position = 1)]
    [ValidateSet('down', 'up', 'build', 'recreate', 'clean-build')]
    [System.String]$paramCommand = 'up'
)

$endpoints = $(
    "Server"
)

$backends = $(
); 

$clean = 0
$command = "up -d"

if ($paramCommand.Equals('recreate')) {
    $command = "up -dV";
} elseif ($paramCommand.Equals('down')) {
    $command = "down";
} elseif ($paramCommand.Equals('build')) {
    $command = 'build';	
} elseif ($paramCommand.Equals('clean-build')) {
    $command = 'build --no-cache';	
}

$services = $();
if ($paramService.Equals("base")) {
    $services = $("base");
}
elseif ($paramService.Equals("all")) {
    $services = $backends + $endpoints;
}
else {
    $services = @($paramService);
}


$summaries = @{};

Foreach ($service in $services) {
    if ($service.Equals("base")) {
        $composeFileParams = "-f base/docker-compose.yml -f base/docker-compose.override.yml"
    } else {
        $dockerComposeDirectory = "";
        if ($backends -contains $service) {
		    $dockerComposeDirectory = "domains/$service";
        } elseif ($endpoints -contains $service) {
		    $dockerComposeDirectory = "endpoints/$service";
        }
        $composeFileParams = "-f $dockerComposeDirectory/docker-compose.yml -f $dockerComposeDirectory/docker-compose.override.yml"
    }

    if ($paramCommand.Equals('build')) {
        $cmd = "docker-compose -p hive $composeFileParams $command";
    } else {
        if ($clean.Equals(1)) {
            $cmd = "docker-compose -p hive $composeFileParams rm -s -f";
            Write-Output "Executing: $cmd";
            Invoke-Expression $cmd;
            
            $cmd = "docker-compose -p hive $composeFileParams pull";
            Write-Output "Executing: $cmd";
            Invoke-Expression $cmd;
        }
        $cmd = "docker-compose -p hive $composeFileParams $command";
    }

    Write-Output "Executing: $cmd";
    Invoke-Expression $cmd;
    $summaries[$service] = $LASTEXITCODE -eq 0;
}

Foreach ($service in $summaries.keys) {
    if($summaries[$service]) {
        Write-Host "$paramCommand of $service succeeded" -ForegroundColor Green;
    } else {
        Write-Host "$paramCommand of $service failed" -ForegroundColor Red;
    }
}