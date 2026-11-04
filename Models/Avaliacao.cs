namespace ProjetoFinalCSharp.Models;

public class Avaliacao
{
    public int Id {get; set;}
    public int JogoId {get; set;}
    public Jogo Jogo {get; set;} = null!;
    public int Nota {get; private set;}
    public string Autor {get; set;} = string.Empty;
    public string Comentario {get; set;} = string.Empty;
    public DateTime DataAvaliacao {get; set;} = DateTime.UtcNow;

    private Avaliacao()
    {
        
    }

    public Avaliacao(Jogo jogo, int nota, string autor)
    {
        if (nota < 0 || nota > 10)
            throw new ArgumentException("A nota deve estar entre 0 e 10");

        Jogo = jogo;
        JogoId = jogo.Id;
        Nota = nota;
        Autor = autor;
    }

}