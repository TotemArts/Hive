Param (
    [Parameter(Position = 0)]
    [System.String]$paramService = "all",

    [Parameter(Position = 1)]
    [ValidateSet('down', 'up', 'build', 'recreate', 'upimg', 'clean-build')]
    [System.String]$paramCommand = 'up'
)

$endpoints = $(
    "Server"
)

$backends = $(
); 

$clean = 0
$command = "up -d"

$services = $();

if ($paramCommand.Equals('recreate')) {
    $command = "up -dV --remove-orphans";
}
elseif ($paramCommand.Equals('down')) {
    $command = "down";
}
elseif ($paramCommand.Equals('build')) {
    $command = 'build';	
}
elseif ($paramCommand.Equals('clean-build')) {
    $command = 'build --no-cache';	
}

if ($paramService.Equals("base")) {
    $services = $("base");
}
elseif (!$paramService.Equals("all")) {
    $services = @($paramService);
}
else {
    $services = $backends + $endpoints;
}

Foreach ($service in $services) {
    $dockerComposeDirectory = $service;
	$name = $service;
    if ($backends -contains $service) {
		$dockerComposeDirectory = "domains/Hive.$service";
    }
    if ($endpoints -contains $service) {
		$dockerComposeDirectory = "endpoints/$service";
    }

    if ($service.Equals("base")) {
        $composeFileParams = "-f base/docker-compose.yml -f base/docker-compose.override.yml"
    }
    else {
        $composeFileParams = "--project-directory $dockerComposeDirectory/ -f $dockerComposeDirectory/docker-compose.yml -f $dockerComposeDirectory/docker-compose.override.yml"
    }

    if ($clean.Equals(1)) {
        Write-Output "Cleaning..."
		
        $cmd = "docker-compose $composeFileParams -p Hive.$service stop";
        Write-Output "Executing: $cmd";
        Invoke-Expression $cmd;
		
        $cmd = "docker-compose $composeFileParams -p Hive.$service rm -f";
        Write-Output "Executing: $cmd";
        Invoke-Expression $cmd;
		
        $cmd = "docker-compose $composeFileParams -p Hive.$service pull";
        Write-Output "Executing: $cmd";
        Invoke-Expression $cmd;
    }
	
    $cmd = "docker-compose $composeFileParams -p Hive.$service $command";

    Write-Output "Executing: $cmd";
    Invoke-Expression $cmd;
}