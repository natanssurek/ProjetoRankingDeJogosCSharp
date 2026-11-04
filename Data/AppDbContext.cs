using Microsoft.EntityFrameworkCore;
using ProjetoFinalCSharp.Models;

namespace ProjetoFinalCSharp.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base (options)
    {
        
    } 

    public DbSet<Jogo> Jogos => Set<Jogo>();
    public DbSet<Genero> Generos => Set<Genero>();
    public DbSet<Plataforma> Plataformas => Set<Plataforma>();
    public DbSet<JogoPlataforma> JogoPlataformas => Set<JogoPlataforma>();
    public DbSet<Avaliacao> Avaliacoes => Set<Avaliacao>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<JogoPlataforma>()
        .HasKey(jp => new {jp.JogoId, jp.PlataformaId});
    }
}


