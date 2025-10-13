using Datos.BaseDatos;
using Modelo.Bodega;
using System.Diagnostics.Contracts;

namespace Logica.BodegaLogica
{
    public class Bodega_LN
    {

        private readonly Contexto bd;

        public Bodega_LN()
        {
            bd = new Contexto();
        }


        #region Consultas
        public bool ProporcionarListaBodegas(ref List<Bodega_VM> ListaBodegas, out string? errorMessage)
        {
            try
            {

                ListaBodegas = (from Bod in bd.Bodega
                                where Bod.EstadoFila == true
                                      select new Bodega_VM
                                      {
                                          BodegaId = Bod.BodegaId,
                                          NombreBodega = Bod.NombreBodega,
                                      }).ToList();
                errorMessage = null;  
                return true;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return false;
            }
        }

        public Bodega_Productos_VM? ProporcionarListaBodegasYProductos(int IdBodega, out string? errorMessage)
        {
            try
            {
                // Validar que el ID no sea 0
                if (IdBodega == 0)
                {
                    errorMessage = "El ID de bodega no puede ser 0.";
                    return null;
                }

                // Obtener la bodega específica
                var bodega = bd.Bodega
                    .FirstOrDefault(b => b.BodegaId == IdBodega && b.EstadoFila == true);

                if (bodega == null)
                {
                    errorMessage = "Bodega no encontrada.";
                    return null;
                }

                // Obtener los productos de esa bodega (puede estar vacía)
                var productos = bd.Inventario
                    .Where(i => i.BodegaId == IdBodega && i.EstadoFila == true)
                    .Select(i => new ProductosBodega_VM
                    {
                        ProductoId = i.ProductoId,
                        cantidad = i.Cantidad,
                        Nombre = i.Producto.Nombre,
                        Precio = i.Producto.Precio,
                        EstadoFila = i.Producto.EstadoFila,
                        FechaCreacion = i.Producto.FechaCreacion,
                        FechaModificacion = i.Producto.FechaModificacion
                    }).ToList();

                // Crear el objeto de respuesta - la lista puede estar vacía
                var resultado = new Bodega_Productos_VM
                {
                    BodegaId = bodega.BodegaId,
                    NombreBodega = bodega.NombreBodega,
                    Productos = productos.Any() ? productos : new List<ProductosBodega_VM>() // Lista vacía en lugar de null
                };

                errorMessage = null;
                return resultado;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return null;
            }
        }

        public bool ListarProductosRegistrados(ref List<Bodega_VM> ListaBodegas, out string? errorMessage)
        {
            try
            {

                ListaBodegas = (from Bod in bd.Bodega
                                where Bod.EstadoFila == true
                                select new Bodega_VM
                                {
                                    BodegaId = Bod.BodegaId,
                                    NombreBodega = Bod.NombreBodega,
                                }).ToList();
                errorMessage = null;
                return true;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return false;
            }
        }
        #endregion

        #region CRUD
        public bool AgregarBodegaYproducto(Bodega_Productos_VM Datos, out string? errorMessage)
        {
            using (var transaction = bd.Database.BeginTransaction())
            {
                try
                {
                    // Insertar en la tabla Colaboradores
                    var NuevaBodega = new Bodega
                    {
                        NombreBodega = Datos.NombreBodega,
                        EstadoFila = true,
                        FechaCreacion = DateTime.Now
                    };
                    bd.Bodega.Add(NuevaBodega);
                    bd.SaveChanges();

                    int IdBodega = NuevaBodega.BodegaId;

                    // Validar si hay productos para agregar al inventario
                    if (Datos.Productos != null && Datos.Productos.Any())
                    {
                        // Insertar en la tabla Inventario solo si hay productos
                        foreach (var producto in Datos.Productos)
                        {
                            if (producto.ProductoId.HasValue && producto.cantidad > 0)
                            {
                                var nuevoInventario = new Inventario
                                {
                                    ProductoId = producto.ProductoId.Value,
                                    BodegaId = IdBodega,
                                    Cantidad = producto.cantidad,
                                    EstadoFila = true,
                                    FechaCreacion = DateTime.Now
                                };
                                bd.Inventario.Add(nuevoInventario);
                            }
                        }
                        bd.SaveChanges();
                    }

                    transaction.Commit();

                    errorMessage = null; 
                    return true;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    errorMessage = ex.Message;
                    return false;
                }
            }
        }
        #endregion
    }
}
