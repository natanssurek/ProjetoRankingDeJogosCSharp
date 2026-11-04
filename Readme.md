# Catálogo de Games

Aplicação para cadastrar jogos, organizá-los por gênero e plataforma e registrar avaliações dos usuários, com ranking dos jogos mais bem avaliados.

O projeto começou como uma aplicação de console em C# e foi migrado para uma arquitetura com **API em ASP.NET Core + Entity Framework Core** e **front-end em React + Vite**.

## Tecnologias

**Back-end**
- C# / .NET 10
- ASP.NET Core (Web API)
- Entity Framework Core 10
- SQLite

**Front-end**
- React (JavaScript)
- Vite

## Funcionalidades

- Cadastrar jogo (o gênero é criado automaticamente se ainda não existir)
- Listar jogos
- Buscar jogo por título (parte do nome)
- Remover jogo
- Avaliar jogo (nota de 0 a 10, com autor e comentário opcional)
- Ranking dos jogos pela média das avaliações

### Regras de negócio

- A nota da avaliação deve estar entre 0 e 10
- Um autor só pode avaliar o mesmo jogo uma vez
- Uma plataforma não pode ser vinculada duas vezes ao mesmo jogo
- Cada jogo possui apenas um gênero (N → 1)
- Jogos e plataformas têm relação muitos-para-muitos (N → N), pela tabela `JogoPlataforma`

## Modelo de dados

| Entidade | Descrição |
|---|---|
| `Jogo` | Título, ano de lançamento, data de cadastro e gênero |
| `Genero` | Nome do gênero; um gênero possui vários jogos |
| `Plataforma` | Nome da plataforma (PC, PlayStation, etc.) |
| `JogoPlataforma` | Tabela de ligação entre jogos e plataformas (chave composta) |
| `Avaliacao` | Nota, autor, comentário e data, vinculada a um jogo |

## Estrutura do projeto

```
ProjetoFinalCSharp/
├── Data/            # AppDbContext (configuração do EF Core)
├── Models/          # Entidades: Jogo, Genero, Plataforma, JogoPlataforma, Avaliacao
├── Services/        # CatalogoService (regras de negócio)
├── Migrations/      # Migrations geradas pelo EF Core
├── Frontend/        # Aplicação React + Vite
├── Program.cs
└── ProjetoFinalC#.csproj
```

## Pré-requisitos

- [.NET SDK 10](https://dotnet.microsoft.com/download)
- [Node.js](https://nodejs.org) versão 20.19+ ou 22.12+
- Ferramenta do EF Core:

```bash
dotnet tool install --global dotnet-ef
```

## Como executar

### 1. Clonar o repositório

```bash
git clone <url-do-repositorio>
cd ProjetoFinalCSharp
```

### 2. Back-end

```bash
dotnet restore
dotnet ef database update
dotnet run
```

O comando `dotnet ef database update` cria o arquivo `app.db` (SQLite) com todas as tabelas.

### 3. Front-end

Em outro terminal:

```bash
cd Frontend
npm install
npm run dev
```

O front sobe em `http://localhost:5173`.

## Migrations

Ao alterar alguma model, gere uma nova migration e atualize o banco:

```bash
dotnet ef migrations add NomeDaMudanca
dotnet ef database update
```

## Status do projeto

- [x] Models das entidades
- [x] `AppDbContext` e banco SQLite com migrations
- [x] `CatalogoService` com as regras de negócio
- [x] Front-end criado com Vite + React
- [ ] DTOs e controllers (endpoints da API)
- [ ] Integração do front-end com a API
- [ ] Cadastro de plataformas e vínculo com jogos

## Endpoints previstos

| Método | Rota | Descrição |
|---|---|---|
| `POST` | `/api/jogos` | Cadastrar jogo |
| `GET` | `/api/jogos` | Listar jogos |
| `GET` | `/api/jogos?busca=zelda` | Buscar jogos por título |
| `DELETE` | `/api/jogos/{id}` | Remover jogo |
| `POST` | `/api/jogos/{id}/avaliacoes` | Avaliar jogo |
| `GET` | `/api/ranking` | Ranking por média de avaliações |

## Observação sobre o nome da pasta

Evite usar o caractere `#` no caminho das pastas do projeto. O Vite e o Node tratam o `#` como início de fragmento de URL e deixam de encontrar os arquivos.