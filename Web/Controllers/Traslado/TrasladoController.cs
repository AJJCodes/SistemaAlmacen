using Logica.BodegaLogica;
using Logica.TrasladoLogica;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Modelo.Bodega;
using Modelo.Producto;
using Modelo.Traslado;

namespace Web.Controllers.Traslado
{
    public class TrasladoController : Controller
    {
        private readonly Traslado_LN ln;
        private readonly Bodega_LN ln2;
        public TrasladoController()
        {
            ln = new Traslado_LN();
            ln2 = new Bodega_LN();
        }
        public IActionResult Index()
        {
            string? ErrorMsg = null;
            List<Bodega_VM> ListaBodegas = new List<Bodega_VM>();
            if (ln2.ProporcionarListaBodegas(ref ListaBodegas, out ErrorMsg))
            {
                ViewBag.ListaBodegas = ListaBodegas;
            }
            else
            {
                ViewBag.ListaBodegas = ListaBodegas;
            }
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

            return PartialView("_CrearEditar");
        }

        #region Consultas

        [HttpGet]
        public IActionResult ObtenerBodegas()
        {
            string? errorMessage;
            List<Bodega_VM> listaBodegas = new List<Bodega_VM>();

            bool resultado = ln2.ProporcionarListaBodegas(ref listaBodegas, out errorMessage);

            if (resultado && listaBodegas != null)
            {
                return Json(new { success = true, data = listaBodegas });
            }

            return Json(new { success = false, error = errorMessage ?? "No se pudieron obtener las bodegas." });
        }


        [HttpGet]
        public IActionResult ObtenerProductosPorBodega(int idBodega)
        {
            List<Bodega_Productos_VM> Lista = new List<Bodega_Productos_VM>();
            string? errorMessage;

            bool resultado = ln.ProporcionarListaBodegasYProductos(ref Lista, idBodega, out errorMessage);

            if (!resultado) 
            {
                return Json(new { success = false, error = errorMessage ?? "Error desconocido" });
            }


            return Json(new
            {
                success = true,
                data = new
                {
                    productos = Lista.FirstOrDefault()?.Productos
                }
            });
        }


        #endregion

        #region CRUD
        [HttpPost]
        public IActionResult RegistrarTraslado([FromBody] List<Traslado_VM> traslados)
        {
            if (traslados == null || traslados.Count == 0)
                return Json(new { success = false, error = "No se recibieron productos para trasladar." });

            string? errorMessage;
            bool resultado = ln.RegistrarTraslado(traslados, out errorMessage);

            if (resultado)
                return Json(new { success = true });
            else
                return Json(new { success = false, error = errorMessage ?? "Error desconocido al registrar el traslado." });
        }

        #endregion
    }
}
