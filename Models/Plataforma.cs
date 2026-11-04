namespace ProjetoFinalCSharp.Models;

public class Plataforma
{
    public int Id {get; set;}
    public string Nome {get; set;} = string.Empty;
    public List<JogoPlataforma> JogoPlataformas {get; set;} = new();

    public Plataforma()
    {
        
    }

    public Plataforma (string nome)
    {
        this.Nome = nome;
    }

    public bool PodeSerExcluido()
    {
        return JogoPlataformas.Count == 0;
    }
}