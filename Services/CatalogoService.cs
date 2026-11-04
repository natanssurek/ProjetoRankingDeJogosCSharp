using Microsoft.EntityFrameworkCore;
using ProjetoFinalCSharp.Data;
using ProjetoFinalCSharp.Models;

namespace ProjetoFinalCSharp.Services;

public class CatalogoService
{
    private readonly AppDbContext _context;

    public CatalogoService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Genero> ObterOuCriarGenero(string nome)
    {
        var genero = await _context.Generos.FirstOrDefaultAsync(g => g.Nome == nome);

        if (genero == null)
        {
            genero = new Genero(nome);
            _context.Generos.Add(genero);
            await _context.SaveChangesAsync();
        }
        
        return genero;
    }

    public async Task<Jogo> CadastrarJogo(string titulo, int ano, string nomeGenero)
    {
        var genero = await ObterOuCriarGenero(nomeGenero);

        var jogo = new Jogo (titulo, ano, genero);
        _context.Jogos.Add(jogo);
        await _context.SaveChangesAsync();

        return jogo;
    }

    public async Task<List<Jogo>> ListarJogos()
    {
        return await _context.Jogos.Include(j => j.Genero).ToListAsync();
    }

    public async Task<List<Jogo>> BuscarJogo(string busca)
    {
        return await _context.Jogos
        .Include(j => j.Genero)
        .Where(j => j.Titulo.Contains(busca))
        .ToListAsync();
    }

    public async Task<Jogo?> BuscarPorId(int id)
    {
        return await _context.Jogos
        .Include(j => j.Genero)
        .Include (j => j.Avaliacoes)
        .FirstOrDefaultAsync(j => j.Id == id);
    }

    public async Task<bool> RemoverJogo(int id)
    {
        var jogo = await _context.Jogos.FirstOrDefaultAsync(j => j.Id == id );

        if (jogo == null)
        {
            return false;
        }

        _context.Jogos.Remove(jogo);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task AvaliarJogo(int idJogo, string autor, int nota, string comentario)
    {
        var jogo = await _context.Jogos
        .Include(j => j.Avaliacoes)
        .FirstOrDefaultAsync(j => j.Id == idJogo);

        if (jogo == null)
        {
            throw new ArgumentException("Jogo não encontrado!!");
        }

        var avaliacao = new Avaliacao (jogo, nota, autor);
        avaliacao.Comentario = comentario;

        jogo.AdicionarAvaliacao(avaliacao);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Jogo>> ObterRanking()
    {
        var jogos = await _context.Jogos
        .Include(j => j.Genero)
        .Include(j => j.Avaliacoes)
        .ToListAsync();

        return jogos.OrderByDescending(j => j.CalcularMediaAvaliacoes()).ToList();
    }

}