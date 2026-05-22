$base = 'http://localhost:5119'
Write-Host "Base URL: $base"

function Safe-Invoke {
    param($Method, $Url, $Body=$null, $Token=$null)
    try {
        $headers = @{}
        if ($Token) { $headers['Authorization'] = "Bearer $Token" }
        if ($Body -ne $null) {
            return Invoke-RestMethod -Method $Method -Uri $Url -Headers $headers -ContentType 'application/json' -Body ($Body | ConvertTo-Json -Depth 10)
        } else {
            return Invoke-RestMethod -Method $Method -Uri $Url -Headers $headers
        }
    } catch {
        Write-Host "ERROR $Method $Url -> $($_.Exception.Message)"
        return $null
    }
}

# 1) Registrar usuário
$userPayload = @{ Nome = 'API Test'; Email = 'apitest+1@example.com'; Senha = 'Senha123!'; FotoURL = '' }
$reg = Safe-Invoke -Method 'Post' -Url "$base/api/usuarios/cadastrar" -Body $userPayload
Write-Host "Register result:"; $reg | ConvertTo-Json -Depth 5

# 2) Login
$loginPayload = @{ Email = 'apitest+1@example.com'; Senha = 'Senha123!' }
$login = Safe-Invoke -Method 'Post' -Url "$base/api/usuarios/login" -Body $loginPayload
if ($login -eq $null) { Write-Host 'Login falhou, abortando testes.'; exit 1 }
$token = $login.Token
Write-Host "Login token:"; $token

# 3) Criar tema
$temaPayload = @{ Descricao = 'Culinária' }
$tema = Safe-Invoke -Method 'Post' -Url "$base/api/temas" -Body $temaPayload -Token $token
Write-Host "Tema criado:"; $tema | ConvertTo-Json -Depth 5

# 4) Listar temas (GET)
$temas = Safe-Invoke -Method 'Get' -Url "$base/api/temas" -Token $token
Write-Host "Temas listados:"; $temas | ConvertTo-Json -Depth 5

# Determine tema id
$temaId = $null
if ($tema -ne $null -and $tema.TemaID) { $temaId = $tema.TemaID } elseif ($temas -ne $null -and $temas.Count -gt 0) { $temaId = $temas[0].TemaID }
Write-Host "Usando temaId: $temaId"

# 5) Criar postagem
$postPayload = @{ Titulo = 'Receita de onigiri'; Texto = 'Hoje vou explicar como fazer um onigiri simples com arroz japonês, recheio de atum e alga nori.'; ImagemURL = 'https://exemplo.com/onigiri.jpg'; Data = '2026-05-21T20:40:00'; TemaId = $temaId; UsuarioId = $login.UsuarioId }
$post = Safe-Invoke -Method 'Post' -Url "$base/api/postagens" -Body $postPayload -Token $token
Write-Host "Post criado:"; $post | ConvertTo-Json -Depth 6

# 6) Listar postagens (GET)
$postagens = Safe-Invoke -Method 'Get' -Url "$base/api/postagens" -Token $token
Write-Host "Postagens listadas:"; $postagens | ConvertTo-Json -Depth 6

# 7) Filtro (por autor e tema)
$filtro = Safe-Invoke -Method 'Get' -Url "$base/api/postagens/filtro?autor=$($login.UsuarioId)&tema=$temaId" -Token $token
Write-Host "Resultado filtro:"; $filtro | ConvertTo-Json -Depth 6

# 8) Chamar IA (resumir)
$iaReq = @{ Titulo = 'Teste IA'; Texto = 'Texto de teste para gerar resumo e tags via IA.' }
$iaRes = Safe-Invoke -Method 'Post' -Url "$base/api/ia/resumir" -Body $iaReq -Token $token
Write-Host "IA resposta:"; $iaRes | ConvertTo-Json -Depth 5

# 9) Atualizar postagem (PUT)
if ($post -ne $null -and $post.PostagemId) {
    $postId = $post.PostagemId
    $postUpdate = @{ Titulo = 'Receita de onigiri (editada)'; Texto = $post.Texto; ImagemURL = $post.ImagemURL; Data = $post.Data; TemaId = $temaId; UsuarioId = $login.UsuarioId }
    $upd = Safe-Invoke -Method 'Put' -Url "$base/api/postagens/$postId" -Body $postUpdate -Token $token
    Write-Host "Post atualizado:"; $upd | ConvertTo-Json -Depth 6
}

# 10) Deletar postagem
if ($post -ne $null -and $post.PostagemId) {
    $postId = $post.PostagemId
    $del = Safe-Invoke -Method 'Delete' -Url "$base/api/postagens/$postId" -Token $token
    Write-Host "Post deletado:"; $del | ConvertTo-Json -Depth 6
}

# 11) Atualizar tema (PUT)
if ($temaId -ne $null) {
    $temaPut = @{ TemaID = $temaId; Descricao = 'Culinária (editada)' }
    $updTema = Safe-Invoke -Method 'Put' -Url "$base/api/temas/$temaId" -Body $temaPut -Token $token
    Write-Host "Tema atualizado:"; $updTema | ConvertTo-Json -Depth 5
}

# 12) Deletar tema
if ($temaId -ne $null) {
    $delTema = Safe-Invoke -Method 'Delete' -Url "$base/api/temas/$temaId" -Token $token
    Write-Host "Tema deletado:"; $delTema | ConvertTo-Json -Depth 5
}

# 13) Atualizar usuário (PUT)
$userPut = @{ Nome = 'API Test Edit'; Email = 'apitest+1@example.com'; Senha = 'Senha123!'; FotoURL = '' }
$updUser = Safe-Invoke -Method 'Put' -Url "$base/api/usuarios/$($login.UsuarioId)" -Body $userPut -Token $token
Write-Host "Usuário atualizado:"; $updUser | ConvertTo-Json -Depth 5

# 14) Deletar usuário
$delUser = Safe-Invoke -Method 'Delete' -Url "$base/api/usuarios/$($login.UsuarioId)" -Token $token
Write-Host "Usuário deletado:"; $delUser | ConvertTo-Json -Depth 5

Write-Host 'Todos os testes concluídos.'
