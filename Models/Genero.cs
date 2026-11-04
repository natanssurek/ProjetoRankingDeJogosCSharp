namespace ProjetoFinalCSharp.Models;

public class Genero
{
    public int Id {get; set;}
    public string Nome {get; set;} = string.Empty;
    public List<Jogo> Jogos  {get; set;} = new();

    public Genero()
    {
        
    }

    public Genero (string nome)
    {
        this.Nome = nome;
    }

    public bool PodeSerExcluido()
    {
        return Jogos.Count == 0;
    }
}