
using Microsoft.EntityFrameworkCore;
using pimFinal.Models;

namespace pimFinal.Data;

public class PedieDbContext : DbContext
{
    public PedieDbContext(
        DbContextOptions<PedieDbContext> options)
        : base(options)
    {
    }

    public DbSet<Categoria> Categorias {get;set;}
    public DbSet<Produto> Produtos {get;set;}
}