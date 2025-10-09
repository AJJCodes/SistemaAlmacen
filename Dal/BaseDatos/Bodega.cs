using System;
using System.Collections.Generic;

namespace Datos.BaseDatos;

public partial class Bodega
{
    public int BodegaId { get; set; }

    public string NombreBodega { get; set; } = null!;

    public int? SucursalId { get; set; }

    public bool? EstadoFila { get; set; }

    public DateTime? FechaCreacion { get; set; }

    public DateTime? FechaModificacion { get; set; }

    public virtual ICollection<Inventario> Inventario { get; set; } = new List<Inventario>();

    public virtual ICollection<Traslado> TrasladoBodegaDestino { get; set; } = new List<Traslado>();

    public virtual ICollection<Traslado> TrasladoBodegaOrigen { get; set; } = new List<Traslado>();
}
