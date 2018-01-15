using System.Collections.Generic;

namespace WebGlass.Business.ConhecimentoTransporte.Fluxo
{
    public class BuscarLogCte : BaseFluxo<BuscarLogCte>
    {
        private BuscarLogCte() { }

        /// <summary>
        /// Busca lista de logs pelo idCte
        /// </summary>
        /// <param name="idCte"></param>
        /// <param name="sortExpression"></param>
        /// <param name="startRow"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public Entidade.LogCte[] GetList(uint idCte, string sortExpression, int startRow, int pageSize)
        {
            var retorno = new List<Entidade.LogCte>();
            using (Glass.Data.DAL.CTe.LogCteDAO dao = Glass.Data.DAL.CTe.LogCteDAO.Instance)
            {
                var listaLogCte = dao.GetList(idCte, sortExpression, startRow, pageSize);

                foreach (var item in listaLogCte)
                    retorno.Add(new Entidade.LogCte(item));
            }
            return retorno.ToArray();
        }

        /// <summary>
        /// Retorna quantidade de logs cadastrados
        /// </summary>
        /// <param name="idCte"></param>
        /// <returns></returns>
        public int GetCount(uint idCte)
        {
            using (Glass.Data.DAL.CTe.LogCteDAO dao = Glass.Data.DAL.CTe.LogCteDAO.Instance)
            {
                return dao.GetCount(idCte);
            }
        }

        /// <summary>
        /// Obtém último log
        /// </summary>
        /// <param name="idCte"></param>
        /// <returns></returns>
        public int ObtemUltimoCodigo(uint idCte)
        {
            using (Glass.Data.DAL.CTe.LogCteDAO dao = Glass.Data.DAL.CTe.LogCteDAO.Instance)
            {
                return dao.ObtemUltimoCodigo(idCte);
            }
        }
    }
}
