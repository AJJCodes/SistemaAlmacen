using Datos.BaseDatos;
using Microsoft.EntityFrameworkCore;
using Modelo.Producto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logica.ProductoLogica
{
    public class Producto_LN
    {

        private readonly Contexto _bd;

        public Producto_LN()
        {
            _bd = new Contexto();
        }


        #region crud
        public bool ListaProducto(ref List<Producto_VM> ListaProductos, out string? errorMessage)
        {

            
            try
            {

                ListaProductos = _bd.Producto.FromSqlRaw("EXEC SP_Producto_Listar").AsEnumerable().Select(prod => new Producto_VM
                {
                    ProductoId = prod.ProductoId,
                    Nombre = prod.Nombre,
                    Precio = prod.Precio,
                    EstadoFila = prod.EstadoFila,
                    FechaCreacion = prod.FechaCreacion
                }).ToList();
                //.FromSqlRaw("EXEC ObtenerProductosPorCategoria @CategoriaId = {0}", categoriaId)
                //.ToListAsync();
                //ListaProductos = (from prod in _bd.Producto
                //                  where prod.EstadoFila == true
                //                  select new Producto_VM
                //                  {
                //                      ProductoId = prod.ProductoId,
                //                      Nombre = prod.Nombre,
                //                      Precio = prod.Precio,
                //                      EstadoFila = prod.EstadoFila,
                //                      FechaCreacion = prod.FechaCreacion
                //                  }
                //                 ).ToList();
                errorMessage = null;
                return true;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return false;
            }
        }


        public bool Crear(Producto_VM producto, ref string ErrorMessage)
        {

            try
            {
                //Producto nuevoProducto = new Producto()
                //{

                //}
                //_bd.Producto.Add(new Producto()
                //{
                //    Nombre = producto.Nombre,
                //    Precio = producto.Precio,
                //    EstadoFila = true,
                //    FechaCreacion = DateTime.Now
                //});

                //_bd.SaveChanges();

                // Ejecutamos el SP directamente desde el contexto
                _bd.Database.ExecuteSqlRaw(
                    "EXEC SP_Producto_Insertar @Nombre = {0}, @Precio = {1}",
                    producto.Nombre,
                    producto.Precio
                );
                return true;
            }
            catch (Exception ex)
            {

                ErrorMessage = ex.Message;
                return false;
            }


        }

        public bool Editar(Producto_VM producto, ref string ErrorMessage)
        {

            try
            {
               
                var dbProducto = _bd.Producto.Find(producto.ProductoId);

                if (dbProducto == null)
                {

                    ErrorMessage = "Producto No Encontrado";
                    return false;
                }


                dbProducto.Nombre = producto.Nombre;
                dbProducto.Precio = producto.Precio;
                dbProducto.FechaModificacion = DateTime.Now;
                _bd.Entry(dbProducto).State = EntityState.Modified;


                try
                {
                    _bd.SaveChanges();
                    return true;
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (!ProductExists(producto.ProductoId))
                    {
                        ErrorMessage = ex.Message;
                        return false;
                    }
                    else
                    {
                        ErrorMessage = ex.Message;
                        return false;
                    }
                }

            }
            catch (Exception ex)
            {

                ErrorMessage = ex.Message;
                return false;
            }


        }

        public bool Eliminar(int id, ref string ErrorMessage)
        {
            try
            {
                var dbProducto = _bd.Producto.Find(id);

                if (dbProducto == null)
                {
                    ErrorMessage = "Producto no encontrado.";
                    return false;
                }

                // 🔹 Marcamos el producto como inactivo (eliminación lógica)
                dbProducto.EstadoFila = false;
                dbProducto.FechaModificacion = DateTime.Now;

                _bd.Entry(dbProducto).State = EntityState.Modified;

                _bd.SaveChanges();
                return true;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                ErrorMessage = "Error de concurrencia al eliminar el producto: " + ex.Message;
                return false;
            }
            catch (Exception ex)
            {
                ErrorMessage = "Error al eliminar el producto: " + ex.Message;
                return false;
            }
        }


        #endregion


        public Producto_VM? ObtenerPorId(int? productoId, ref string? ErrorMessage)
        {
            try
            {
                var producto = _bd.Producto
                    .Find(productoId);

                if (producto == null)
                {
                    ErrorMessage = "Producto no encontrado.";
                    return null;
                }

                return new Producto_VM
                {
                    ProductoId = producto.ProductoId,
                    Nombre = producto.Nombre,
                    Precio = producto.Precio,
                    //EstadoFila = producto.EstadoFila,
                    //FechaCreacion = producto.FechaCreacion,
                    //FechaModificacion = producto.FechaModificacion
                };
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                return null;
            }
        }

        private bool ProductExists(int? id)
        {
            return (_bd.Producto?.Any(e => e.ProductoId == id)).GetValueOrDefault();
        }
    }
}
