using System;
using System.Collections.Generic;

namespace Datos.BaseDatos;

public partial class Traslado
{
    public int TrasladoId { get; set; }

    public int ProductoId { get; set; }

    public int Cantidad { get; set; }

    public int BodegaOrigenId { get; set; }

    public int BodegaDestinoId { get; set; }

    public DateTime Fecha { get; set; }

    public virtual Bodega BodegaDestino { get; set; } = null!;

    public virtual Bodega BodegaOrigen { get; set; } = null!;

    public virtual Producto Producto { get; set; } = null!;
}
