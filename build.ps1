$result = 0

if ($env:APPVEYOR -ne $true) {
    $env:SemVer = "1.0.0"
    $now = (Get-Date -Date ((Get-Date).DateTime) -UFormat %s)
    $env:Version = "1.0.0-local$now"
}

if ($isLinux) {
	Get-ChildItem -rec `
	| Where-Object { $_.Name -like "*.IntegrationTest.csproj" `
		   -Or $_.Name -like "*.Test.csproj" `
		 } `
	| ForEach-Object { 
		Set-Location $_.DirectoryName
		dotnet test
	
		if ($LASTEXITCODE -ne 0) {
			$result = $LASTEXITCODE
		}
	}
} else {
	Get-ChildItem -rec `
	| Where-Object { $_.Name -like "*.IntegrationTest.csproj" `
		   -Or $_.Name -like "*.Test.csproj" `
		 } `
	| ForEach-Object { 
        &('dotnet') ('test', $_.FullName, '--logger', "trx;LogFileName=$_.trx", '-c', 'Release', '/p:CollectCoverage=true', '/p:CoverletOutputFormat=cobertura', '/p:Exclude=\"[xunit.*]*')    
		if ($LASTEXITCODE -ne 0) {
			$result = $LASTEXITCODE
		}
	  }

    $merge = ""
    Get-ChildItem -rec `
    | Where-Object { $_.Name -like "coverage.cobertura.xml" } `
    | ForEach-Object { 
        $path = $_.FullName
        $merge = "$merge;$path"
    }
    Write-Host $merge
    ReportGenerator\tools\net47\ReportGenerator.exe "-reports:$merge" "-targetdir:coverage\docs" "-reporttypes:HtmlInline;Badges"
}

.\publish.ps1

exit $result
  