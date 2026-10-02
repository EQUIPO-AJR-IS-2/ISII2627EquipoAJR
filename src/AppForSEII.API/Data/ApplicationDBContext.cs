using AppForSEII.API.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using AppForSEII.API.DTOs.ApplicationUserDTO;

namespace AppForSEII.API.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
{
    protected override void OnModelCreating(ModelBuilder builder)
    {

        base.OnModelCreating(builder);


    }


    public DbSet<ApplicationUser> ApplicationUsers { get; set; }
    public DbSet<TipoDeporte> TiposDeporte { get; set; }
    public DbSet<ClaseDeportiva> ClasesDeportivas { get; set; }
    public DbSet<Competicion> Competiciones { get; set; }
    public DbSet<TipoMaterial> TiposMaterial { get; set; }
    public DbSet<Material> Materiales { get; set; }
    public DbSet<MaterialAlquilado> MaterialesAlquilados { get; set; }
    public DbSet<Alquiler> Alquileres { get; set; }
    public DbSet<ClaseInscrita> ClasesInscritas { get; set; }
    public DbSet<InscripcionClaseDeportiva> InscripcionesClasesDeportivas { get; set; }
    public DbSet<CompeticionInscrita> CompeticionesInscritas { get; set; }

}