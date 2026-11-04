using Microsoft.AspNetCore.Http.Connections;

namespace ProjetoFinalCSharp.Models;

public class Jogo
{
    public int Id {get; set;}
    public string Titulo {get; set;} = string.Empty;
    public int anoLancamento {get; set;}
    public DateTime DataCadastro {get; set;} = DateTime.UtcNow;

    public int GeneroId {get; set;}
    public Genero Genero {get; set;} = null;

    public List<JogoPlataforma> JogoPlataformas {get; set;} = new();
    public List<Avaliacao> Avaliacoes {get; set;} = new();

    private Jogo ()
    {
        
    }

    public Jogo (string titulo, int anoLancamento, Genero genero)
    {
        this.Titulo = titulo;
        this.anoLancamento = anoLancamento;
        this.Genero = genero;
        this.GeneroId = GeneroId;
    }

    public void AdicionarPlataforma(Plataforma plataforma)
    {
        if (JogoPlataformas. Any(jp => jp.PlataformaId == plataforma.Id))
        {
            throw new InvalidOperationException("Esta plataforma já está vinculada a esse jogo!");
        }

        JogoPlataformas.Add(new JogoPlataforma(this, plataforma));
    }

    public void AdicionarAvaliacao (Avaliacao avaliacao)
    {
        if (Avaliacoes.Any(a => a.Autor == avaliacao.Autor))
        {
            throw new InvalidOperationException("Esse autor já avaliou esse jogo.");
        }

        Avaliacoes.Add(avaliacao);
    }

    public double CalcularMediaAvaliacoes()
    {
        return Avaliacoes.Count == 0 ? 0 : Avaliacoes.Average(a => a.Nota);
    }
}