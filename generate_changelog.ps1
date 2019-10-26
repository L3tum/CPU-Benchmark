$future = "v0.3.0"

if($future -eq $null -or $future -eq ""){
    github_changelog_generator -u L3tum -p CPU-Benchmark --cache-log ./tmp --token $(Get-Content "./github_token") --no-pr-wo-labels --no-issues --pr-label "**Updates:**"
} else{
    github_changelog_generator -u L3tum -p CPU-Benchmark --cache-log ./tmp --future-release $future --token $(Get-Content "./github_token") --no-pr-wo-labels --no-issues --pr-label "**Updates:**"
}
Remove-Item "./tmp"

Write-Host "Press any key to continue..."
$Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")