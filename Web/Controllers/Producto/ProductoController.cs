using Logica.BodegaLogica;
using Logica.ProductoLogica;
using Microsoft.AspNetCore.Mvc;
using Modelo.Producto;

namespace Web.Controllers.Producto
{
    public class ProductoController : Controller
    {

        private readonly Producto_LN ln;


        public ProductoController()
        {
            ln = new Producto_LN();
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ObtenerlistaProductos()
        {
            List<Producto_VM> ListaBodegas = new List<Producto_VM>();
            string? errorMessage = null;


            bool exito = ln.ListaProducto(ref ListaBodegas, out errorMessage);

            if (exito)
            {

                return Json(new { status = true, data = ListaBodegas });
            }
            else
            {
                return Json(new { status = false, error = errorMessage });
            }
        }

        public ActionResult Formulario(int? productoId)
        {
            if (productoId == null)
                return PartialView("_CrearEditar", new Producto_VM());


            string? errorMessage = null;
            var producto = ln.ObtenerPorId(productoId, ref errorMessage);

            if (producto == null)
                return Json(new { error = errorMessage });

            return PartialView("_CrearEditar", producto);
        }

        #region crud
        [HttpPost]
        public JsonResult Guardar(Producto_VM producto)
        {
            if (ModelState.IsValid)
            {



                //bool EsEditar = (producto.ProductoId != 0);
                bool EsEditar = (producto.ProductoId != null);
                string ErrorMessage = string.Empty;


                if (!EsEditar)
                {
                    //if (catalogosLN.Crear(catalogos, IdEmpresa, ref ErrorMessage))
                    if(ln.Crear(producto, ref ErrorMessage))
                        return Json(new { estado = true });
                 
                }
                else
                {
                        if(ln.Editar(producto, ref ErrorMessage))
                        return Json(new { estado = true });
                }

                

                return Json(new
                {
                    estado = false,
                    formErrors = ModelState.Select(kvp => new
                    {
                        key = kvp.Key,
                        errors = kvp.Value.Errors.Select(e => e.ErrorMessage)
                    })
                });
            }

            return Json(new
            {
                estado = false,
                formErrors = ModelState.Select(kvp => new
                {
                    key = kvp.Key,
                    errors = kvp.Value.Errors.Select(e => e.ErrorMessage)
                })
            });
        }


        #endregion 
    }
}
