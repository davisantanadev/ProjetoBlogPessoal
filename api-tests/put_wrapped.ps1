$base = 'http://localhost:5119'
$creds = @{ Email = 'apitest+1@example.com'; Senha = 'Senha123!' }
$login = Invoke-RestMethod -Method Post -Uri "$base/api/usuarios/login" -Body ($creds | ConvertTo-Json) -ContentType 'application/json'
$token = $login.Token
$postId = 8
$body = @{ dto = @{ Titulo='Wrapped Test'; Texto='Wrapped text'; ImagemURL='img.png'; Data='2026-05-21T21:01:19'; TemaId=8; UsuarioId=8 } }
$json = $body | ConvertTo-Json -Depth 10
Write-Host "Sending JSON:"; Write-Host $json
try {
    $resp = Invoke-RestMethod -Method Put -Uri "$base/api/postagens/$postId" -Body $json -ContentType 'application/json' -Headers @{ Authorization = "Bearer $token" } -ErrorAction Stop
    Write-Host "PUT success:"; $resp | ConvertTo-Json -Depth 6
} catch {
    Write-Host "PUT failed:"; $_.Exception | Format-List -Force
    try { $stream = $_.Exception.Response.GetResponseStream(); $reader = New-Object System.IO.StreamReader($stream); $bodyErr = $reader.ReadToEnd(); Write-Host "Response body:"; Write-Host $bodyErr } catch {}
}
