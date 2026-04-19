using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace BiciRodriguez.Api.Models;

public partial class BiciContext : DbContext
{
    public BiciContext()
    {
    }

    public BiciContext(DbContextOptions<BiciContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Bicicleta> Bicicletas { get; set; }

    public virtual DbSet<CatalogoManoObra> CatalogoManoObras { get; set; }

    public virtual DbSet<Categoria> Categorias { get; set; }

    public virtual DbSet<Cliente> Clientes { get; set; }

    public virtual DbSet<DetalleManoObra> DetalleManoObras { get; set; }

    public virtual DbSet<DetallePedido> DetallePedidos { get; set; }

    public virtual DbSet<DetalleRepuesto> DetalleRepuestos { get; set; }

    public virtual DbSet<FichasIngreso> FichasIngresos { get; set; }

    public virtual DbSet<Foto> Fotos { get; set; }

    public virtual DbSet<Pedido> Pedidos { get; set; }

    public virtual DbSet<Producto> Productos { get; set; }

    public virtual DbSet<Proveedore> Proveedores { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }
    public virtual DbSet<Balance> Balances { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Bicicleta>(entity =>
        {
            entity.HasKey(e => e.BicicletaId).HasName("PK__Vehiculo__AA0886205EB81C66");

            entity.HasIndex(e => e.NumeroMarco, "UQ_NumeroMarco").IsUnique();

            entity.Property(e => e.BicicletaId).HasColumnName("BicicletaID");
            entity.Property(e => e.ClienteId).HasColumnName("ClienteID");
            entity.Property(e => e.Color).HasMaxLength(30);
            entity.Property(e => e.CreadoPorUsuarioId).HasColumnName("CreadoPorUsuarioID");
            entity.Property(e => e.FechaRegistro)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Marca).HasMaxLength(50);
            entity.Property(e => e.Modelo).HasMaxLength(50);
            entity.Property(e => e.NumeroMarco).HasMaxLength(100);
            entity.Property(e => e.TipoBicicleta).HasMaxLength(30);
            entity.Property(e => e.UltimaModificacion).HasColumnType("datetime");

            entity.HasOne(d => d.Cliente).WithMany(p => p.Bicicleta)
                .HasForeignKey(d => d.ClienteId)
                .HasConstraintName("FK__Vehiculos__Clien__02084FDA");

            entity.HasOne(d => d.CreadoPorUsuario).WithMany(p => p.Bicicleta)
                .HasForeignKey(d => d.CreadoPorUsuarioId)
                .HasConstraintName("FK__Bicicleta__Cread__2EDAF651");
        });

        modelBuilder.Entity<CatalogoManoObra>(entity =>
        {
            entity.HasKey(e => e.ServicioId).HasName("PK__Servicio__D5AEEC22981601E1");

            entity.ToTable("CatalogoManoObra");

            entity.Property(e => e.ServicioId).HasColumnName("ServicioID");
            entity.Property(e => e.DuracionEstimada).HasMaxLength(50);
            entity.Property(e => e.Nombre).HasMaxLength(100);
            entity.Property(e => e.PrecioFijo).HasColumnType("decimal(18, 2)");
        });

        modelBuilder.Entity<Categoria>(entity =>
        {
            entity.HasKey(e => e.CategoriaId).HasName("PK__Categori__F353C1C559D2DD0C");

            entity.Property(e => e.CategoriaId).HasColumnName("CategoriaID");
            entity.Property(e => e.Descripcion).HasMaxLength(200);
            entity.Property(e => e.Nombre).HasMaxLength(50);
        });

        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.HasKey(e => e.ClienteId).HasName("PK__Clientes__71ABD0A7229251EC");

            entity.ToTable(tb => tb.HasTrigger("TRG_ValidarEmailCliente"));

            entity.HasIndex(e => e.NombreCompleto, "IX_Cliente_Nombre");

            entity.HasIndex(e => e.Email, "UQ_Cliente_Email").IsUnique();

            entity.HasIndex(e => e.Email, "UQ_Clientes_Email").IsUnique();

            entity.Property(e => e.ClienteId).HasColumnName("ClienteID");
            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.AutorizaDatos).HasDefaultValue(true);
            entity.Property(e => e.CreadoPorUsuarioId).HasColumnName("CreadoPorUsuarioID");
            entity.Property(e => e.Direccion).HasMaxLength(200);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.FechaRegistro)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.NombreCompleto).HasMaxLength(100);
            entity.Property(e => e.Telefono).HasMaxLength(20);
            entity.Property(e => e.UltimaModificacion).HasColumnType("datetime");

            entity.HasOne(d => d.CreadoPorUsuario).WithMany(p => p.Clientes)
                .HasForeignKey(d => d.CreadoPorUsuarioId)
                .HasConstraintName("FK__Clientes__Creado__2DE6D218");
        });

        modelBuilder.Entity<DetalleManoObra>(entity =>
        {
            entity.HasKey(e => e.DetalleServicioId).HasName("PK__DetalleO__938140D28AAFBA5C");

            entity.ToTable("DetalleManoObra");

            entity.Property(e => e.DetalleServicioId).HasColumnName("DetalleServicioID");
            entity.Property(e => e.FichaId).HasColumnName("FichaID");
            entity.Property(e => e.PrecioCobrado).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ServicioId).HasColumnName("ServicioID");

            entity.HasOne(d => d.Ficha).WithMany(p => p.DetalleManoObras)
                .HasForeignKey(d => d.FichaId)
                .HasConstraintName("FK__DetalleOr__Orden__6FE99F9F");

            entity.HasOne(d => d.Servicio).WithMany(p => p.DetalleManoObras)
                .HasForeignKey(d => d.ServicioId)
                .HasConstraintName("FK__DetalleOr__Servi__70DDC3D8");
        });

        modelBuilder.Entity<DetallePedido>(entity =>
        {
            entity.HasKey(e => e.DetalleId).HasName("PK__DetalleP__6E19D6FA0A1C929A");

            entity.ToTable("DetallePedido");

            entity.Property(e => e.DetalleId).HasColumnName("DetalleID");
            entity.Property(e => e.PedidoId).HasColumnName("PedidoID");
            entity.Property(e => e.PrecioCompra).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ProductoId).HasColumnName("ProductoID");

            entity.HasOne(d => d.Pedido).WithMany(p => p.DetallePedidos)
                .HasForeignKey(d => d.PedidoId)
                .HasConstraintName("FK__DetallePe__Pedid__59FA5E80");

            entity.HasOne(d => d.Producto).WithMany(p => p.DetallePedidos)
                .HasForeignKey(d => d.ProductoId)
                .HasConstraintName("FK__DetallePe__Produ__5AEE82B9");
        });

        modelBuilder.Entity<DetalleRepuesto>(entity =>
        {
            entity.HasKey(e => e.DetalleInsumoId).HasName("PK__DetalleR__55331A19D7A68C1C"); // Tu PK actual

            entity.Property(e => e.DetalleInsumoId).HasColumnName("DetalleInsumoID");
            entity.Property(e => e.FichaId).HasColumnName("FichaID");
            entity.Property(e => e.ProductoId).HasColumnName("ProductoID");

            // AGREGA ESTA LÍNEA PARA EL PRECIO HISTÓRICO
            entity.Property(e => e.PrecioVentaHistorico).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Ficha).WithMany(p => p.DetalleRepuestos)
                .HasForeignKey(d => d.FichaId)
                .HasConstraintName("FK__DetalleRe__Ficha__6EF57B66");

            entity.HasOne(d => d.Producto).WithMany(p => p.DetalleRepuestos)
                .HasForeignKey(d => d.ProductoId)
                .HasConstraintName("FK__DetalleRe__Produ__6FE99F9F");
        });

        modelBuilder.Entity<FichasIngreso>(entity =>
        {
            entity.HasKey(e => e.FichaId).HasName("PK__OrdenesS__C088A4E49BED0377");

            entity.ToTable("FichasIngreso");

            entity.HasIndex(e => e.FechaEntrada, "IX_Orden_Fecha");

            entity.Property(e => e.FichaId).HasColumnName("FichaID");
            entity.Property(e => e.BicicletaId).HasColumnName("BicicletaID");
            entity.Property(e => e.ClienteId).HasColumnName("ClienteID");
            entity.Property(e => e.CreadoPorUsuarioId).HasColumnName("CreadoPorUsuarioID");
            entity.Property(e => e.Estado)
                .HasMaxLength(20)
                .HasDefaultValue("Recibida", "DF_Fichas_Estado_Recibida");
            entity.Property(e => e.FechaEntrada)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.FechaEntrega).HasColumnType("datetime");
            entity.Property(e => e.MecanicoId).HasColumnName("MecanicoID");
            entity.Property(e => e.ModificadoPorUsuarioId).HasColumnName("ModificadoPorUsuarioID");
            entity.Property(e => e.UltimaModificacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Bicicleta).WithMany(p => p.FichasIngresos)
                .HasForeignKey(d => d.BicicletaId)
                .HasConstraintName("FK__OrdenesSe__Vehic__03F0984C");

            entity.HasOne(d => d.Cliente).WithMany(p => p.FichasIngresos)
                .HasForeignKey(d => d.ClienteId)
                .HasConstraintName("FK__OrdenesSe__Clien__4AB81AF0");

            entity.HasOne(d => d.CreadoPorUsuario).WithMany(p => p.FichasIngresoCreadoPorUsuarios)
                .HasForeignKey(d => d.CreadoPorUsuarioId)
                .HasConstraintName("FK__FichasIng__Cread__30C33EC3");

            entity.HasOne(d => d.Mecanico).WithMany(p => p.FichasIngresoMecanicos)
                .HasForeignKey(d => d.MecanicoId)
                .HasConstraintName("FK__OrdenesSe__Mecan__4BAC3F29");
        });

        modelBuilder.Entity<Foto>(entity =>
        {
            entity.HasKey(e => e.FotoId).HasName("PK__Fotos__4EA1C1793894A9A0");

            entity.Property(e => e.FotoId).HasColumnName("FotoID");
            entity.Property(e => e.EsPrincipal).HasDefaultValue(false);
            entity.Property(e => e.FechaRegistro)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.RelacionId).HasColumnName("RelacionID");
            entity.Property(e => e.TipoEntidad).HasMaxLength(20);
        });

        modelBuilder.Entity<Pedido>(entity =>
        {
            entity.HasKey(e => e.PedidoId).HasName("PK__Pedidos__09BA14102F462DD2");

            entity.Property(e => e.PedidoId).HasColumnName("PedidoID");
            entity.Property(e => e.Estado)
                .HasMaxLength(20)
                .HasDefaultValue("Pendiente");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ProveedorId).HasColumnName("ProveedorID");
            entity.Property(e => e.TotalPedido)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Proveedor).WithMany(p => p.Pedidos)
                .HasForeignKey(d => d.ProveedorId)
                .HasConstraintName("FK__Pedidos__Proveed__5441852A");
        });

        modelBuilder.Entity<Producto>(entity =>
        {
            entity.HasKey(e => e.ProductoId).HasName("PK__Producto__A430AE831FC253AC");

            entity.HasIndex(e => e.Nombre, "IX_Producto_Nombre");

            entity.Property(e => e.ProductoId).HasColumnName("ProductoID");
            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.CategoriaId).HasColumnName("CategoriaID");
            entity.Property(e => e.CreadoPorUsuarioId).HasColumnName("CreadoPorUsuarioID");
            entity.Property(e => e.FechaRegistro)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Nombre).HasMaxLength(100);
            entity.Property(e => e.PrecioCompra).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.PrecioVenta).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ProveedorId).HasColumnName("ProveedorID");
            entity.Property(e => e.StockActual).HasDefaultValue(0);
            entity.Property(e => e.StockMinimo).HasDefaultValue(5);
            entity.Property(e => e.UltimaModificacion).HasColumnType("datetime");

            entity.HasOne(d => d.Categoria).WithMany(p => p.Productos)
                .HasForeignKey(d => d.CategoriaId)
                .HasConstraintName("FK__Productos__Categ__412EB0B6");

            entity.HasOne(d => d.CreadoPorUsuario).WithMany(p => p.Productos)
                .HasForeignKey(d => d.CreadoPorUsuarioId)
                .HasConstraintName("FK__Productos__Cread__2FCF1A8A");

            entity.HasOne(d => d.Proveedor).WithMany(p => p.Productos)
                .HasForeignKey(d => d.ProveedorId)
                .HasConstraintName("FK__Productos__Prove__4222D4EF");
        });

        modelBuilder.Entity<Proveedore>(entity =>
        {
            entity.HasKey(e => e.ProveedorId).HasName("PK__Proveedo__61266BB9D0F0BCB1");

            entity.HasIndex(e => e.Nit, "UQ_Proveedor_NIT").IsUnique();

            entity.Property(e => e.ProveedorId).HasColumnName("ProveedorID");
            entity.Property(e => e.ContactoNombre).HasMaxLength(100);
            entity.Property(e => e.Nit)
                .HasMaxLength(20)
                .HasColumnName("NIT");
            entity.Property(e => e.NombreEmpresa).HasMaxLength(100);
            entity.Property(e => e.Telefono).HasMaxLength(20);
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RolId).HasName("PK__Roles__F92302D1C0EC6758");

            entity.Property(e => e.RolId)
                .ValueGeneratedNever()
                .HasColumnName("RolID");
            entity.Property(e => e.Nombre).HasMaxLength(20);
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.UsuarioId).HasName("PK__Usuarios__2B3DE79810FE251F");

            entity.HasIndex(e => e.Email, "UQ_Usuario_Email_Unico").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__Usuarios__A9D1053459797792").IsUnique();

            entity.Property(e => e.UsuarioId).HasColumnName("UsuarioID");
            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.NombreCompleto).HasMaxLength(100);
            entity.Property(e => e.RolId).HasColumnName("RolID");

            entity.HasOne(d => d.Rol).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.RolId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Usuarios__RolID__45F365D3");
        });

        modelBuilder.Entity<Balance>(entity =>
        {
            entity.ToTable(tb => tb.HasTrigger("tr_BloquearModificacionBalances"));

            entity.HasIndex(e => new { e.Fecha, e.Tipo }, "UQ_Balance_Fecha_Tipo").IsUnique();

            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.Property(e => e.IngresosRepuestos)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 2)");

            entity.Property(e => e.IngresosServicios)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 2)");

            entity.Property(e => e.Tipo).HasMaxLength(20);

            entity.Property(e => e.TotalEgresos)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 2)");

            entity.Property(e => e.UtilidadNeta)
                .HasComputedColumnSql("(([IngresosServicios]+[IngresosRepuestos])-[TotalEgresos])", false)
                .HasColumnType("decimal(20, 2)");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
