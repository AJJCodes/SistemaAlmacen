using System;
using System.Collections.Generic;

namespace Datos.BaseDatos;

public partial class Inventario
{
    public int InventarioId { get; set; }

    public int ProductoId { get; set; }

    public int BodegaId { get; set; }

    public int Cantidad { get; set; }

    public bool? EstadoFila { get; set; }

    public DateTime? FechaCreacion { get; set; }

    public DateTime? FechaModificacion { get; set; }

    public virtual Bodega Bodega { get; set; } = null!;

    public virtual Producto Producto { get; set; } = null!;
}
