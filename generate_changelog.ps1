$future = $args[0]
$token = $(Get-Content "./github_token")

github_changelog_generator -u L3tum -p CPU-Benchmark --future-release $future --cache-log ./tmp --token $token
Remove-Item "./tmp"

Write-Host "Press any key to continue..."
$Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")