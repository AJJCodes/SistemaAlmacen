using Logica.Bodega;
using Microsoft.AspNetCore.Mvc;
using Modelo.Bodega;

namespace Web.Controllers.Bodega
{
    public class BodegaController : Controller
    {


        private readonly Bodega_LN ln;

        public BodegaController()
        {
            ln = new Bodega_LN();
        }

        public IActionResult Index()
        {
            return View();
        }
        #region Consultas
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
        #endregion
        #region CRUD
        #endregion
    }
}
