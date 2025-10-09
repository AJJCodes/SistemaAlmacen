using System;
using System.Collections.Generic;

namespace Datos.BaseDatos;

public partial class Producto
{
    public int ProductoId { get; set; }

    public string Nombre { get; set; } = null!;

    public decimal Precio { get; set; }

    public bool? EstadoFila { get; set; }

    public DateTime? FechaCreacion { get; set; }

    public DateTime? FechaModificacion { get; set; }

    public virtual ICollection<Inventario> Inventario { get; set; } = new List<Inventario>();

    public virtual ICollection<Traslado> Traslado { get; set; } = new List<Traslado>();
}
