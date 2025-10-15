using Datos.BaseDatos;
using Modelo.Traslado;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logica.TrasladoLogica
{
    public class Traslado_LN
    {
        private readonly Contexto bd;

        public Traslado_LN()
        {
            bd = new Contexto();
        }

        public List<Traslado_VM> ListarTraslados(out string? errorMessage)
        {
            try
            {
                var traslados = (from t in bd.Traslado
                                 join p in bd.Producto on t.ProductoId equals p.ProductoId
                                 join bOrigen in bd.Bodega on t.BodegaOrigenId equals bOrigen.BodegaId
                                 join bDestino in bd.Bodega on t.BodegaDestinoId equals bDestino.BodegaId
                                 orderby t.Fecha descending
                                 select new Traslado_VM
                                 {
                                     ProductoID = t.ProductoId,
                                     NombreProducto = p.Nombre,
                                     Cantidad = t.Cantidad,
                                     BodegaOrigenID = t.BodegaOrigenId,
                                     BodegaDestinoID = t.BodegaDestinoId,
                                     NombreBodegaOrigen = bOrigen.NombreBodega,
                                     NombreBodegaDestino = bDestino.NombreBodega,
                                     FechaTraslado = t.Fecha
                                 }).ToList();

                errorMessage = null;
                return traslados;
            }
            catch (Exception ex)
            {
                errorMessage = "Error al consultar los traslados: " + ex.Message;
                return new List<Traslado_VM>();
            }
        }

    }
}
