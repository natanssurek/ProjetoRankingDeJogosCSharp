using Microsoft.EntityFrameworkCore;
using ProjetoFinalCSharp.Data;
using ProjetoFinalCSharp.Services;
using ProjetoFinalCSharp.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(o => o.UseSqlite("Data Source=app.db"));
builder.Services.AddScoped<CatalogoService>();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
//Lista os jogos
app.MapGet("/api/jogos", async (string? busca, CatalogoService service) =>
{
    var jogos = string.IsNullOrWhiteSpace(busca)
    ? await service.ListarJogos()
    : await service.BuscarJogo(busca);

    var lista = jogos.Select(j => new
    {
        j.Id,
        j.Titulo,
        AnoLancamento = j.anoLancamento,
        Gnero = j.Genero.Nome,
        j.DataCadastro
    }).ToList();
    return Results.Ok(lista);
});

//Buscar algum jogo
app.MapGet("/api/jogos/{id:int}", async (int id, CatalogoService service) =>
{
    var jogo = await service.BuscarPorId(id);
    if (jogo==null)
    {
        return Results.NotFound(new { erro = "Jogo não encontrado"});
    }
        var avaliacao = jogo.Avaliacoes.Select(a => new
        {
            a.Autor,
            a.Nota,
            a.Comentario,
            a.DataAvaliacao
        }). ToList();

        return Results.Ok(new
        {
            jogo.Id,
            jogo.Titulo,
            AnoLancamento = jogo.anoLancamento,
            Gnero = jogo.Genero.Nome,
            jogo.DataCadastro,
            MediaAvaliacao = Math.Round(jogo.CalcularMediaAvaliacoes(), 2),
            Avaliacoes = avaliacao
        });
    });

    
    //Cadatrar um jogo
    app.MapPost("/api/jogos", async (NovoJogo novo, CatalogoService service) =>
    {
       if (string.IsNullOrWhiteSpace(novo.Titulo) || string.IsNullOrWhiteSpace(novo.Genero))
       return Results.BadRequest(new {erro = "Titulo e Gênero são obrigatórios"});

       if (novo.AnoLancamento < 1900 || novo.AnoLancamento > 2100)
       return Results.BadRequest(new {erro = "Ano de lançamento invalido"});

       var jogo = await service.CadastrarJogo(novo.Titulo.Trim(), novo.AnoLancamento, novo.Genero.Trim());

       return Results.Created($"/api/jogos/{jogo.Id}", new
       {
           jogo.Id,
              jogo.Titulo,
              AnoLancamento = jogo.anoLancamento,
              Gnero = jogo.Genero.Nome,
       });
    });

    // Remover jogo
    app.MapDelete("/api/jogos/{id:int}", async (int id, CatalogoService service) =>
    {
        var removido = await service.RemoverJogo(id);
        if (!removido)
           return Results.NotFound(new { erro = "Jogo não encontrado"});
           
           return Results.NoContent();
    });

    // Avaliar algum jogo
    app.MapPost("/api/jogos/{id:int}/avaliacoes", async (int id, NovaAvaliacao pedido, CatalogoService service) =>
    {
        if (string.IsNullOrWhiteSpace(pedido.Autor))
        return Results.BadRequest(new { erro = "o Autor é obrigatório"});

        var jogo = await service.BuscarPorId(id);

        if (jogo == null)
        return Results.NotFound(new { erro = "Jogo não encontrado"});

        try

        {
            await service.AvaliarJogo(id, pedido.Autor.Trim(), pedido.Nota, pedido.Comentario ?? "");
            return Results.Ok(new { mensagem = "Avaliação adicionada com sucesso"});
        }
        catch (ArgumentException ex)
        {
            return Results.BadRequest(new { erro = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Results.BadRequest(new { erro = ex.Message });
        }

    
    });

//Ranking
app.MapGet("/api/ranking", async (CatalogoService service) =>
{
        var jogos = await service.ObterRanking();

        var ranking = jogos.Select((j, i) => new 
        
        {
            Posicao = i + 1,
            j.Id,
            j.Titulo,
            AnoLancamento = j.anoLancamento,
            Gnero = j.Genero.Nome,
            MediaAvaliacao = Math.Round(j.CalcularMediaAvaliacoes(), 2)
        }).ToList();

        return Results.Ok(ranking);
});






app.Run();

