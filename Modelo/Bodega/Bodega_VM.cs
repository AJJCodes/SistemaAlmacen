using Modelo.Producto;

namespace Modelo.Bodega
{
    public class Bodega_VM
    {
        public int BodegaId { get; set; }

        public string NombreBodega { get; set; } = null!;

        public int? SucursalId { get; set; }

        public bool? EstadoFila { get; set; }

        public DateTime? FechaCreacion { get; set; }

        public DateTime? FechaModificacion { get; set; }
    }

    public class Bodega_Productos_VM
    {
        public int BodegaId { get; set; }
        public string NombreBodegaConProductos { get; set; } = null!;

        public List<Producto_VM> Productos { get; set; } = new List<Producto_VM>();
    }
}
