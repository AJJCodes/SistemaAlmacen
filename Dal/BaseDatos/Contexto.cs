using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Datos.BaseDatos;

public partial class Contexto : DbContext
{
    public Contexto()
    {
    }

    public Contexto(DbContextOptions<Contexto> options)
        : base(options)
    {
    }

    public virtual DbSet<Bodega> Bodega { get; set; }

    public virtual DbSet<Inventario> Inventario { get; set; }

    public virtual DbSet<Producto> Producto { get; set; }

    public virtual DbSet<Traslado> Traslado { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=sql1002.site4now.net;Database=db_abf21c_ucnproyecto;User ID=db_abf21c_ucnproyecto_admin;Password=proyectoalmacen1;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Bodega>(entity =>
        {
            entity.HasKey(e => e.BodegaId).HasName("PK__Bodega__37A29A75041FC2AD");

            entity.Property(e => e.BodegaId).HasColumnName("BodegaID");
            entity.Property(e => e.EstadoFila).HasDefaultValue(true);
            entity.Property(e => e.FechaCreacion).HasColumnType("datetime");
            entity.Property(e => e.FechaModificacion).HasColumnType("datetime");
            entity.Property(e => e.NombreBodega).HasMaxLength(100);
            entity.Property(e => e.SucursalId).HasColumnName("SucursalID");
        });

        modelBuilder.Entity<Inventario>(entity =>
        {
            entity.HasKey(e => e.InventarioId).HasName("PK__Inventar__FB8A24B713AA46C2");

            entity.HasIndex(e => new { e.ProductoId, e.BodegaId }, "UQ_Inventario").IsUnique();

            entity.Property(e => e.InventarioId).HasColumnName("InventarioID");
            entity.Property(e => e.BodegaId).HasColumnName("BodegaID");
            entity.Property(e => e.EstadoFila).HasDefaultValue(true);
            entity.Property(e => e.FechaCreacion).HasColumnType("datetime");
            entity.Property(e => e.FechaModificacion).HasColumnType("datetime");
            entity.Property(e => e.ProductoId).HasColumnName("ProductoID");

            entity.HasOne(d => d.Bodega).WithMany(p => p.Inventario)
                .HasForeignKey(d => d.BodegaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Inventario_Bodega");

            entity.HasOne(d => d.Producto).WithMany(p => p.Inventario)
                .HasForeignKey(d => d.ProductoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Inventario_Producto");
        });

        modelBuilder.Entity<Producto>(entity =>
        {
            entity.HasKey(e => e.ProductoId).HasName("PK__Producto__A430AE83540E26C1");

            entity.Property(e => e.ProductoId).HasColumnName("ProductoID");
            entity.Property(e => e.EstadoFila).HasDefaultValue(true);
            entity.Property(e => e.FechaCreacion).HasColumnType("datetime");
            entity.Property(e => e.FechaModificacion).HasColumnType("datetime");
            entity.Property(e => e.Nombre).HasMaxLength(100);
            entity.Property(e => e.Precio).HasColumnType("decimal(10, 2)");
        });

        modelBuilder.Entity<Traslado>(entity =>
        {
            entity.HasKey(e => e.TrasladoId).HasName("PK__Traslado__199FA09B0AE33964");

            entity.Property(e => e.TrasladoId).HasColumnName("TrasladoID");
            entity.Property(e => e.BodegaDestinoId).HasColumnName("BodegaDestinoID");
            entity.Property(e => e.BodegaOrigenId).HasColumnName("BodegaOrigenID");
            entity.Property(e => e.Fecha)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ProductoId).HasColumnName("ProductoID");

            entity.HasOne(d => d.BodegaDestino).WithMany(p => p.TrasladoBodegaDestino)
                .HasForeignKey(d => d.BodegaDestinoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Traslado_BodegaDestino");

            entity.HasOne(d => d.BodegaOrigen).WithMany(p => p.TrasladoBodegaOrigen)
                .HasForeignKey(d => d.BodegaOrigenId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Traslado_BodegaOrigen");

            entity.HasOne(d => d.Producto).WithMany(p => p.Traslado)
                .HasForeignKey(d => d.ProductoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Traslado_Producto");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
