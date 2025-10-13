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

    public class ProductosBodega_VM:Producto_VM
    {
       public  int cantidad { get; set; }

    }

    public class Bodega_Productos_VM: Bodega_VM
    {

        public List<ProductosBodega_VM>? Productos { get; set; } = new List<ProductosBodega_VM>();
    }
}
