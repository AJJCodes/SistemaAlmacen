using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modelo.Traslado
{
    public class Traslado_VM
    {
        public int ProductoID { get; set; }
        public string? NombreProducto { get; set; }
        public int Cantidad { get; set; }
        public int BodegaOrigenID { get; set; }
        public int BodegaDestinoID { get; set; }         
        public string? NombreBodegaOrigen { get; set; }
        public string? NombreBodegaDestino { get; set; }
        public DateTime FechaTraslado { get; set; }
    }

    //public class Traslado_VM
    //{
    //    public int BodegaOrigenID { get; set; }
    //    public int BodegaDestinoID { get; set; }
    //    public List<TrasladoProducto_VM> ProductosTrasladar { get; set; } = new();
    //}

    //public class TrasladoProducto_VM
    //{
    //    public int ProductoID { get; set; }
    //    public int Cantidad { get; set; }
    //    public string? NombreProducto { get; set; } // opcional para mostrar en la vista
    //}
}
