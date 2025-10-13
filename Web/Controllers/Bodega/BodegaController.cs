using Logica.BodegaLogica;
using Logica.ProductoLogica;
using Microsoft.AspNetCore.Mvc;
using Modelo.Bodega;
using Modelo.Producto;

namespace Web.Controllers.Bodega
{
    public class BodegaController : Controller
    {


        private readonly Bodega_LN ln;
        private readonly Producto_LN ln2;

        public BodegaController()
        {
            ln = new Bodega_LN();
            ln2 = new Producto_LN();
        }

        public IActionResult Index()
        {

            return View();
        }

        public ActionResult FormularioEditarYConsulta(int Bodega,int Visualizar)
        {
            string? errorMessage = null;

            Bodega_Productos_VM? BodeConProductos = ln.ProporcionarListaBodegasYProductos(Bodega, out errorMessage);

            if (BodeConProductos == null)
                return Json(new { error = errorMessage });


            if (Visualizar == 1)
            {
                ViewBag.Visualizar = Visualizar;
                return PartialView("_EditaryInfo", BodeConProductos);
            }
            else
            {
                ViewBag.Visualizar = Visualizar;
                return PartialView("_EditaryInfo", BodeConProductos);
            }
        }
        #region Consultas


        [HttpGet]
        public JsonResult ObtenerListaProductos()
        {
            string? errormsg = null;
            List<Producto_VM> listaProductos = new List<Producto_VM>();

            if (ln2.ListaProducto(ref listaProductos, out errormsg))
            {
                return Json(new { data = listaProductos });
            }
            else
            {
                return Json(new { data = new List<Producto_VM>(), error = errormsg });
            }
        }


        [HttpPost]
        public IActionResult ObtenerlistaBodegas()
        {
            List<Bodega_VM> ListaBodegas = new List<Bodega_VM>();
            string? errorMessage = null;


            bool exito = ln.ProporcionarListaBodegas(ref ListaBodegas, out errorMessage);

            if (exito)
            {

                return Json(new { data = ListaBodegas });
            }
            else
            {
                return Json(new { error = errorMessage });
            }
        }

        [HttpPost]
        public IActionResult ObtenerBodegayProductos()
        {
            List<Bodega_VM> ListaBodegas = new List<Bodega_VM>();
            string? errorMessage = null;


            bool exito = ln.ProporcionarListaBodegas(ref ListaBodegas, out errorMessage);

            if (exito)
            {

                return Json(new { data = ListaBodegas });
            }
            else
            {
                return Json(new { error = errorMessage });
            }
        }

        #endregion
        #region CRUD
        [HttpPost]
        public IActionResult AgregarBodegaYProductos(Bodega_Productos_VM BodegaProductos)
        {
            string? errorMessage = null;
            bool resultado = ln.AgregarBodegaYproducto(BodegaProductos, out errorMessage);

            if (resultado)
            {
                return Json(new { success = true });
            }
            else
            {
                return Json(new { success = false, error = errorMessage });
            }
        }
        #endregion
    }
}
