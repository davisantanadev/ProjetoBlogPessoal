$base = 'http://localhost:5119'
Write-Host "Base URL: $base"

function Invoke-Json {
    param($Method, $Url, $Body=$null, $Token=$null)
    $headers = @{}
    if ($Token) { $headers['Authorization'] = "Bearer $Token" }
    $json = $null
    if ($Body -ne $null) { $json = $Body | ConvertTo-Json -Depth 10 }
    try {
        $resp = Invoke-WebRequest -Method $Method -Uri $Url -Headers $headers -ContentType 'application/json' -Body $json -UseBasicParsing -ErrorAction Stop
        Write-Host "$Method $Url -> $($resp.StatusCode)"
        if ($resp.Content) { Write-Host $resp.Content }
        return @{ StatusCode = $resp.StatusCode; Content = $resp.Content }
    } catch {
        Write-Host "ERROR $Method $Url -> $($_.Exception.Message)"
        try {
            $errResp = $_.Exception.Response
            if ($errResp -ne $null) {
                $stream = $errResp.GetResponseStream()
                $reader = New-Object System.IO.StreamReader($stream)
                $body = $reader.ReadToEnd()
                Write-Host "Response body:"; Write-Host $body
                return @{ StatusCode = $errResp.StatusCode; Content = $body }
            }
        } catch {
            Write-Host "Não foi possível ler o corpo da resposta de erro."
        }
        return $null
    }
}

# Login
$loginPayload = @{ Email = 'apitest+1@example.com'; Senha = 'Senha123!' }
$login = Invoke-Json -Method 'Post' -Url "$base/api/usuarios/login" -Body $loginPayload
Write-Host 'Tentando criar usuário de teste (pode retornar conflito)'
$regPayload = @{ Nome = 'API Test'; Email = 'apitest+1@example.com'; Senha = 'Senha123!'; FotoURL = '' }
Invoke-Json -Method 'Post' -Url "$base/api/usuarios/cadastrar" -Body $regPayload

# Agora efetua login
$login = Invoke-Json -Method 'Post' -Url "$base/api/usuarios/login" -Body $loginPayload
if ($login -eq $null) { Write-Host 'Login falhou, abortando.'; exit 1 }
$token = ($login.Content | ConvertFrom-Json).Token
$userId = ($login.Content | ConvertFrom-Json).UsuarioId
Write-Host "Token obtained, userId: $userId"

# Criar tema e postagem de teste
$temaPayload = @{ Descricao = 'Debug Tema' }
$tema = Invoke-Json -Method 'Post' -Url "$base/api/temas" -Body $temaPayload -Token $token
$temaId = $null
if ($tema -ne $null -and $tema.Content) { $temaId = ($tema.Content | ConvertFrom-Json).TemaID }
if (-not $temaId) {
    $temas = Invoke-Json -Method 'Get' -Url "$base/api/temas" -Token $token
    if ($temas -ne $null -and $temas.Content) { $temaId = (($temas.Content | ConvertFrom-Json)[0]).TemaID }
}

# Criar postagem de teste
$postPayload = @{ Titulo = 'Debug Post'; Texto = 'Conteúdo debug'; ImagemURL = 'img.png'; Data = (Get-Date).ToString('s'); TemaId = $temaId; UsuarioId = $userId }
$post = Invoke-Json -Method 'Post' -Url "$base/api/postagens" -Body $postPayload -Token $token

# Get last post
$postsResp = Invoke-Json -Method 'Get' -Url "$base/api/postagens" -Token $token
$posts = $postsResp.Content | ConvertFrom-Json
$last = $posts | Sort-Object postagemId -Descending | Select-Object -First 1
Write-Host "Última postagem id: $($last.postagemId)"

# Prepare update payload
$putPost = @{ Titulo = ($last.titulo + ' (edit-debug)'); Texto = $last.texto; ImagemURL = $last.imagemURL; Data = $last.data; TemaId = $last.tema.TemaID; UsuarioId = $last.usuario.usuarioId }
Write-Host "PUT payload for post:"; $putPost | ConvertTo-Json -Depth 6

# Call PUT
Invoke-Json -Method 'Put' -Url "$base/api/postagens/$($last.postagemId)" -Body $putPost -Token $token

# Prepare update user payload
$putUser = @{ Nome = 'API Test Edit Debug'; Email = 'apitest+1@example.com'; Senha = 'Senha123!'; FotoURL = '' }
Write-Host "PUT payload for user:"; $putUser | ConvertTo-Json -Depth 5
Invoke-Json -Method 'Put' -Url "$base/api/usuarios/$userId" -Body $putUser -Token $token
