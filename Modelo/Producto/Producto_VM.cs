using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modelo.Producto
{
    public  class Producto_VM
    {
        public int? ProductoId { get; set; }

        public string Nombre { get; set; } = null!;
        [DataType(DataType.Currency)]
        [Display(Name = "Precio")]
        [Range(0, double.MaxValue, ErrorMessage = "El precio debe ser un valor positivo.")]
        public decimal Precio { get; set; }

        public bool? EstadoFila { get; set; }

        public DateTime? FechaCreacion { get; set; }

        public DateTime? FechaModificacion { get; set; }
    }
}
