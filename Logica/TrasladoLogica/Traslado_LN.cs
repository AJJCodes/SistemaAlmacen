using Datos.BaseDatos;
using Microsoft.EntityFrameworkCore;
using Modelo.Bodega;
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


        #region Consultas
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


        public bool ProporcionarListaBodegasYProductos(ref List<Bodega_Productos_VM> Lista, int IdBodega, out string? errorMessage)
        {
            try
            {
                // Validar ID
                if (IdBodega <= 0)
                {
                    errorMessage = "El ID de la bodega no puede ser 0 o negativo.";
                    return false;
                }

                // Buscar la bodega activa
                var bodega = bd.Bodega
                    .FirstOrDefault(b => b.BodegaId == IdBodega && b.EstadoFila == true);

                if (bodega == null)
                {
                    errorMessage = "No se encontró la bodega especificada.";
                    return false;
                }

                // Obtener productos asociados a la bodega (si existen)
                var productos = bd.Inventario
                    .Where(i => i.BodegaId == IdBodega && i.EstadoFila == true)
                    .Select(i => new ProductosBodega_VM
                    {
                        ProductoId = i.ProductoId,
                        Nombre = i.Producto.Nombre,
                        Precio = i.Producto.Precio,
                        cantidad = i.Cantidad,
                        EstadoFila = i.Producto.EstadoFila,
                        FechaCreacion = i.Producto.FechaCreacion,
                        FechaModificacion = i.Producto.FechaModificacion
                    })
                    .ToList();

                // Crear el objeto completo
                var bodegaConProductos = new Bodega_Productos_VM
                {
                    BodegaId = bodega.BodegaId,
                    NombreBodega = bodega.NombreBodega,
                    SucursalId = bodega.SucursalId,
                    EstadoFila = bodega.EstadoFila,
                    FechaCreacion = bodega.FechaCreacion,
                    FechaModificacion = bodega.FechaModificacion,
                    Productos = productos // lista puede venir vacía sin problema
                };

                // Limpiar lista ref y agregar el resultado
                Lista.Clear();
                Lista.Add(bodegaConProductos);

                errorMessage = null;
                return true;
            }
            catch (Exception ex)
            {
                errorMessage = $"Error al obtener productos de la bodega: {ex.Message}";
                return false;
            }
        }


        #endregion

        #region CRUD
        public bool RegistrarTraslado(List<Traslado_VM> traslados, out string? errorMessage)
        {
            using (var transaction = bd.Database.BeginTransaction())
            {
                try
                {
                    foreach (var t in traslados)
                    {
                        // 🔹 Validación básica
                        if (t.Cantidad <= 0)
                        {
                            errorMessage = $"Cantidad inválida para el producto {t.NombreProducto}";
                            transaction.Rollback();
                            return false;
                        }

                        // 🔹 Buscar inventario de la bodega origen
                        var inventarioOrigen = bd.Inventario
                            .FirstOrDefault(i => i.BodegaId == t.BodegaOrigenID && i.ProductoId == t.ProductoID && i.EstadoFila == true);

                        if (inventarioOrigen == null || inventarioOrigen.Cantidad < t.Cantidad)
                        {
                            errorMessage = $"Stock insuficiente en la bodega {t.NombreBodegaOrigen} para el producto {t.NombreProducto}.";
                            transaction.Rollback();
                            return false;
                        }
                        // Restar del inventario origen
                        inventarioOrigen.Cantidad -= t.Cantidad;
                        inventarioOrigen.FechaModificacion = DateTime.Now;
                        bd.Entry(inventarioOrigen).State = EntityState.Modified;


                        // Buscar inventario de la bodega destino
                        var inventarioDestino = bd.Inventario
                            .FirstOrDefault(i => i.BodegaId == t.BodegaDestinoID && i.ProductoId == t.ProductoID && i.EstadoFila == true);

                        if (inventarioDestino != null)
                        {
                            // Ya existe el producto → sumar cantidad
                            inventarioDestino.Cantidad += t.Cantidad;
                            inventarioDestino.FechaModificacion = DateTime.Now;
                            bd.Entry(inventarioDestino).State = EntityState.Modified;
                        }
                        else
                        {
                            // No existe → crear nuevo registro
                            var nuevoInventario = new Inventario
                            {
                                ProductoId = t.ProductoID,
                                BodegaId = t.BodegaDestinoID,
                                Cantidad = t.Cantidad,
                                EstadoFila = true,
                                FechaCreacion = DateTime.Now
                            };
                            bd.Inventario.Add(nuevoInventario);
                        }

                        // Registrar traslado en tabla "Traslados"
                        var nuevoTraslado = new Traslado
                        {
                            ProductoId = t.ProductoID,
                            Cantidad = t.Cantidad,
                            BodegaOrigenId = t.BodegaOrigenID,
                            BodegaDestinoId = t.BodegaDestinoID,
                            Fecha = DateTime.Now,
                        };
                        bd.Traslado.Add(nuevoTraslado);
                    }

                    bd.SaveChanges();
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
