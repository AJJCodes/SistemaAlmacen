using Datos.BaseDatos;
using Modelo.Bodega;

namespace Logica.Bodega
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
        #endregion

        #region CRUD
        #endregion
    }
}
