using Microsoft.EntityFrameworkCore;
using ProjetoFinalCSharp.Data;
using ProjetoFinalCSharp.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(o => o.UseSqlite("Data Source=app.db"));
builder.Services.AddScoped<CatalogoService>();

// Libera o front (Vite/React) a chamar a API
builder.Services.AddCors(o => o.AddPolicy("front", p =>
    p.WithOrigins("http://localhost:5173")
     .AllowAnyHeader()
     .AllowAnyMethod()));

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

//app.UseHttpsRedirection();
app.UseCors("front");

// Listar jogos (ou buscar pelo título com ?busca=)
app.MapGet("/api/jogos", async (string? busca, CatalogoService service) =>
{
    var jogos = string.IsNullOrWhiteSpace(busca)
        ? await service.ListarJogos()
        : await service.BuscarJogo(busca);

    var lista = jogos.Select(j => new
    {
        j.Id,
        j.Titulo,
        j.anoLancamento,
        Genero = j.Genero.Nome,
        j.DataCadastro
    }).ToList();

    return Results.Ok(lista);
});

// Detalhes de um jogo
app.MapGet("/api/jogos/{id:int}", async (int id, CatalogoService service) =>
{
    var jogo = await service.BuscarPorId(id);

    if (jogo == null)
        return Results.NotFound(new { erro = "Jogo não encontrado" });

    var avaliacoes = jogo.Avaliacoes.Select(a => new
    {
        a.Autor,
        a.Nota,
        a.Comentario,
        a.DataAvaliacao
    }).ToList();

    return Results.Ok(new
    {
        jogo.Id,
        jogo.Titulo,
        jogo.anoLancamento,
        Genero = jogo.Genero.Nome,
        jogo.DataCadastro,
        MediaAvaliacao = Math.Round(jogo.CalcularMediaAvaliacoes(), 2),
        Avaliacoes = avaliacoes
    });
});

// Cadastrar jogo
app.MapPost("/api/jogos", async (NovoJogo novo, CatalogoService service) =>
{
    if (string.IsNullOrWhiteSpace(novo.Titulo) || string.IsNullOrWhiteSpace(novo.Genero))
        return Results.BadRequest(new { erro = "Título e gênero são obrigatórios" });

    if (novo.AnoLancamento < 1900 || novo.AnoLancamento > 2100)
        return Results.BadRequest(new { erro = "Ano de lançamento inválido" });

    var jogo = await service.CadastrarJogo(novo.Titulo.Trim(), novo.AnoLancamento, novo.Genero.Trim());

    return Results.Created($"/api/jogos/{jogo.Id}", new
    {
        jogo.Id,
        jogo.Titulo,
        jogo.anoLancamento,
        Genero = jogo.Genero.Nome
    });
});

// Remover jogo
app.MapDelete("/api/jogos/{id:int}", async (int id, CatalogoService service) =>
{
    var removido = await service.RemoverJogo(id);

    if (!removido)
        return Results.NotFound(new { erro = "Jogo não encontrado" });

    return Results.NoContent();
});

// Avaliar jogo
app.MapPost("/api/jogos/{id:int}/avaliacoes", async (int id, NovaAvaliacao pedido, CatalogoService service) =>
{
    if (string.IsNullOrWhiteSpace(pedido.Autor))
        return Results.BadRequest(new { erro = "O autor é obrigatório" });

    var jogo = await service.BuscarPorId(id);

    if (jogo == null)
        return Results.NotFound(new { erro = "Jogo não encontrado" });

    try
    {
        await service.AvaliarJogo(id, pedido.Autor.Trim(), pedido.Nota, pedido.Comentario ?? "");
        return Results.Ok(new { mensagem = "Avaliação adicionada com sucesso" });
    }
    catch (ArgumentException ex)
    {
        // Nota fora de 0 a 10
        return Results.BadRequest(new { erro = ex.Message });
    }
    catch (InvalidOperationException ex)
    {
        // Autor já avaliou este jogo
        return Results.Conflict(new { erro = ex.Message });
    }
});

// Ranking por média de avaliações
app.MapGet("/api/ranking", async (CatalogoService service) =>
{
    var jogos = await service.ObterRanking();

    var ranking = jogos.Select((j, i) => new
    {
        Posicao = i + 1,
        j.Id,
        j.Titulo,
        j.anoLancamento,
        Genero = j.Genero.Nome,
        MediaAvaliacao = Math.Round(j.CalcularMediaAvaliacoes(), 2)
    }).ToList();

    return Results.Ok(ranking);
});

app.Run();

// Objetos que recebem o JSON enviado pelo front (DTOs de entrada)
record NovoJogo(string Titulo, int AnoLancamento, string Genero);
record NovaAvaliacao(string Autor, int Nota, string? Comentario);