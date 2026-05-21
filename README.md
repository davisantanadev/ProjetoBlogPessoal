![Cabeçalho do Capsule-Render](https://capsule-render.vercel.app/api?type=waving&color=0:4B0082,100:BA55D3&height=300&section=header&text=Projeto%20Blog%20Pessoal&fontSize=70&fontColor=FFFFFF&animation=fadeIn&fontAlignY=38&desc=By%20@daviantanadev&descAlignY=60&descAlign=70)
# 📝 Blog Pessoal — Backend ASP.NET Core

API RESTful desenvolvida com ASP.NET Core 8, Entity Framework Core e MySQL, como parte de um projeto de aprendizado de desenvolvimento backend com o ecossistema .NET.

---

## 🚀 Tecnologias

| Tecnologia | Versão |
|---|---|
| C# | 12+ |
| ASP.NET Core | 8.0 |
| Entity Framework Core | 8.0.2 |
| Pomelo (MySQL) | 8.0.2 |
| Swashbuckle (Swagger) | 10.1.7 |
| JWT Bearer | 8.0.0 (em implementação) |
| MySQL | 8.x |

---

## 📁 Estrutura do Projeto

```
BlogPessoal/
│
├── Controllers/         # Gerencia as requisições HTTP
├── Services/            # Regras de negócio (em implementação)
├── Repositories/        # Interação com o banco de dados
├── Models/              # Entidades do domínio
├── DTOs/                # Objetos de transferência de dados
├── Data/                # DbContext e configuração do banco
├── Config/              # Configurações gerais
├── Middlewares/         # Middlewares customizados (em implementação)
├── Migrations/          # Migrations geradas pelo EF Core
├── appsettings.json     # Configurações da aplicação
├── Program.cs           # Ponto de entrada e configuração de serviços
└── BlogPessoal.csproj
```

---

## 🗃️ Entidades

### Usuario
| Campo | Tipo | Descrição |
|---|---|---|
| UsuarioId | int | Chave primária |
| Nome | string (80) | Nome do usuário |
| Email | string (50) | Email do usuário |
| Senha | string (20) | Senha (hash em implementação) |
| FotoURL | string (500) | URL da foto de perfil |

### Tema
| Campo | Tipo | Descrição |
|---|---|---|
| TemaID | int | Chave primária |
| Descricao | string (500) | Descrição do tema |

### Postagem
| Campo | Tipo | Descrição |
|---|---|---|
| PostagemId | int | Chave primária |
| Titulo | string (50) | Título da postagem |
| Texto | string (500) | Conteúdo da postagem |
| ImagemURL | string (500) | URL da imagem |
| Data | DateTime | Data da postagem |
| ResumoIA | string? | Resumo gerado por IA |
| TagIA | string? | Tags geradas por IA |
| CategoriaIA | string? | Categoria gerada por IA |
| Tema | Tema | Relacionamento com Tema |
| Usuario | Usuario | Relacionamento com Usuário |

---

## 🔗 Endpoints

### Usuário `/api/usuarios`
| Método | Rota | Descrição |
|---|---|---|
| POST | `/api/usuarios/cadastrar` | Cadastrar novo usuário |
| PUT | `/api/usuarios/{id}` | Atualizar usuário |
| DELETE | `/api/usuarios/{id}` | Excluir usuário |
| POST | `/api/usuarios/login` | Login e geração de token JWT *(em implementação)* |

### Tema `/api/temas`
| Método | Rota | Descrição |
|---|---|---|
| GET | `/api/temas` | Listar todos os temas |
| GET | `/api/temas/{id}` | Buscar tema por id |
| POST | `/api/temas` | Criar novo tema |
| PUT | `/api/temas/{id}` | Atualizar tema |
| DELETE | `/api/temas/{id}` | Excluir tema |

### Postagem `/api/postagens`
| Método | Rota | Descrição |
|---|---|---|
| GET | `/api/postagens` | Listar todas as postagens |
| GET | `/api/postagens/filtro?autor={id}&tema={id}` | Filtrar por autor e/ou tema |
| POST | `/api/postagens` | Criar nova postagem |
| PUT | `/api/postagens/{id}` | Atualizar postagem |
| DELETE | `/api/postagens/{id}` | Excluir postagem |

### IA `/api/ia` *(desafio — em implementação)*
| Método | Rota | Descrição |
|---|---|---|
| POST | `/api/ia/resumir` | Gerar resumo, tags e categoria via IA |

---

## ⚙️ Como rodar o projeto

### Pré-requisitos
- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [MySQL 8.x](https://dev.mysql.com/downloads/)
- Visual Studio 2022 ou VS Code

### Configuração

1. Clone o repositório:
```bash
git clone https://github.com/seu-usuario/BlogPessoal.git
cd BlogPessoal
```

2. Configure a connection string no `appsettings.json`:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=BlogPessoal;Uid=root;Pwd=suasenha;"
}
```

3. Aplique as migrations:
```bash
dotnet ef database update
```

4. Rode o projeto:
```bash
dotnet run
```

5. Acesse o Swagger:
```
https://localhost:{porta}/swagger
```

---

## 🏗️ Arquitetura

O projeto segue arquitetura em camadas:

```
Controller → Repository → Banco de Dados
```

- **Controller**: recebe a requisição HTTP, valida e retorna a resposta
- **Repository**: executa as queries no banco via EF Core
- **DTO**: controla o que entra e o que sai da API
- **Model**: representa as entidades do banco de dados

---

## 🔒 Segurança *(em implementação)*

- Autenticação JWT
- Hash de senha com PasswordHasher
- Controle de acesso com `[Authorize]`

---

## 🤖 Desafio — Integração com IA *(em implementação)*

Ao cadastrar uma postagem, a API enviará o conteúdo para uma API de IA externa que retornará:
- Resumo automático
- Palavras-chave
- Sugestão de categoria

APIs suportadas: OpenAI, Gemini, Azure AI Services.

---

## 📌 Status do Projeto

- [x] Models e entidades
- [x] Migrations e banco de dados
- [x] CRUD de Temas
- [x] CRUD de Usuários (sem login)
- [x] CRUD de Postagens
- [x] DTOs com métodos de extensão
- [x] Filtro de postagens por autor e tema
- [ ] Autenticação JWT
- [ ] Hash de senha
- [ ] Integração com IA
- [ ] Testes unitários e de integração

![Rodapé do Capsule-Render](https://capsule-render.vercel.app/api?type=waving&color=0:4B0082,100:BA55D3&height=100&section=footer)
