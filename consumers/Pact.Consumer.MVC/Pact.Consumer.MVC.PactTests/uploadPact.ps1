param(
	[string]$version,
	[string]$branch
)

if(!$Version) {
	Write-Host "No version parameter provided"
	exit -1;
}

if(!$branch) {
	Write-Host "No branch parameter provided"
	exit -1;
}

if($branch -match "master") {
	$tag = "production";
}
else {
	$tag = $branch.Replace("/", "-");
}

$baseUrl = "http://13.80.68.171"
$pacts = Get-ChildItem -Path ".\*\pacts\*.json"
foreach($pact in $pacts) {
	$pactContent = (Get-Content $pact) -join "`r`n";
	$pactObject = $pactContent | ConvertFrom-Json ;
    $provider = $pactObject.provider.name;
    $consumer = $pactObject.consumer.name

    $providerConsumerUrl = "/pacts/provider/$provider/consumer/$consumer";
	$addPactUrl = "$providerConsumerUrl/version/$version";
	$addPactUrlFull = "$baseUrl$addPactUrl";
	Invoke-RestMethod  -Uri $addPactUrlFull -Method PUT -Body $pactContent -ContentType "application/json";
	$tagPactUrl = "/pacticipants/$consumer/versions/$version/tags/$tag";
	$tagPactUrlFull = "$baseUrl$tagPactUrl";
	Invoke-RestMethod  -Uri $tagPactUrlFull -Method PUT -ContentType "application/json";
	Write-Host "Specific version of pact: $addPactUrlFull"
	Write-Host "Latest  pact on $tag tag: $baseUrl$providerConsumerUrl/latest/$tag"
}

