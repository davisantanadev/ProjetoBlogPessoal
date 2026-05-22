$base = 'http://localhost:5119'
$creds = @{ Email = 'apitest+1@example.com'; Senha = 'Senha123!' }
$login = Invoke-RestMethod -Method Post -Uri "$base/api/usuarios/login" -Body ($creds | ConvertTo-Json) -ContentType 'application/json'
$token = $login.Token
$postId = 6
$json = '{"Titulo":"ASCII Test","Texto":"Simple text","ImagemURL":"img.png","Data":"2026-05-21T20:56:00","TemaId":6,"UsuarioId":8}'
Write-Host "Token: $token"
Write-Host "Sending PUT for post $postId with body:"; Write-Host $json
try {
    $resp = Invoke-RestMethod -Method Put -Uri "$base/api/postagens/$postId" -Body $json -ContentType 'application/json' -Headers @{ Authorization = "Bearer $token" } -ErrorAction Stop
    Write-Host "PUT success:"; $resp | ConvertTo-Json -Depth 6
} catch {
    Write-Host "PUT failed:"; $_.Exception | Format-List -Force
    try { $body = $_.Exception.Response | Select-Object -ExpandProperty Content; Write-Host "Response content:"; Write-Host $body } catch {}
}
