using Logica.TrasladoLogica;
using Microsoft.AspNetCore.Mvc;
using Modelo.Producto;
using Modelo.Traslado;

namespace Web.Controllers.Traslado
{
    public class TrasladoController : Controller
    {
        private readonly Traslado_LN ln;
        
        public TrasladoController()
        {
            ln = new Traslado_LN();
        }
        public IActionResult Index()
        {
            return View();
        }

      


        [HttpGet]
        public IActionResult ObtenerListaTraslados()
        {
            string? errorMessage = null;
            List<Traslado_VM> lista = ln.ListarTraslados(out errorMessage);

            if (errorMessage != null)
            {
                return Json(new { success = false, error = errorMessage });
            }

            return Json(new { success = true, data = lista });
        }

        public ActionResult Formulario(int? TrasladoId)
        {
            //if (TrasladoId == null)
                return PartialView("_CrearEditar");


            //string? errorMessage = null;
            //var producto = ln.ObtenerPorId(TrasladoId, ref errorMessage);

            //if (producto == null)
            //    return Json(new { error = errorMessage });

            //return PartialView("_CrearEditar", producto);
        }
    }
}
