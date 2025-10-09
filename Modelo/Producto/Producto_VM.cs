using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modelo.Producto
{
    public  class Producto_VM
    {
        public int ProductoId { get; set; }

        public string Nombre { get; set; } = null!;

        public decimal Precio { get; set; }

        public bool? EstadoFila { get; set; }

        public DateTime? FechaCreacion { get; set; }

        public DateTime? FechaModificacion { get; set; }
    }
}
