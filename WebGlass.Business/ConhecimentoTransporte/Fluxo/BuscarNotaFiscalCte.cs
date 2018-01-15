using System.Collections.Generic;

namespace WebGlass.Business.ConhecimentoTransporte.Fluxo
{
    public class BuscarNotaFiscalCte : BaseFluxo<BuscarNotaFiscalCte>
    {
        private BuscarNotaFiscalCte() { }

        /// <summary>
        /// Busca nota fiscal associada ao cte pelo idCte e pelo idNf
        /// </summary>
        /// <param name="idCte"></param>
        /// <param name="idNf"></param>
        /// <returns></returns>
        public Entidade.NfeCte GetNfeCte(uint idCte, uint idNf)
        {
            using (Glass.Data.DAL.CTe.NotaFiscalCteDAO dao = Glass.Data.DAL.CTe.NotaFiscalCteDAO.Instance)
            {
                return new Entidade.NfeCte(dao.GetElement(idCte, idNf));
            }
        }

        /// <summary>
        /// Busca lista de notas fiscais associadas ao cte pelo idCte
        /// </summary>
        /// <param name="idCte"></param>
        /// <param name="sortExpression"></param>
        /// <param name="startRow"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public Entidade.NfeCte[] GetList(uint idCte, string sortExpression, int startRow, int pageSize)
        {
            var retorno = new List<Entidade.NfeCte>();
            using (Glass.Data.DAL.CTe.NotaFiscalCteDAO dao = Glass.Data.DAL.CTe.NotaFiscalCteDAO.Instance)
            {
                if (dao.GetCount(idCte) == 0)
                {
                    var nfeCte = new Entidade.NfeCte(new Glass.Data.Model.Cte.NotaFiscalCte());
                    nfeCte.ObjNotaFiscal = new Glass.Data.Model.NotaFiscal();
                    var ListanfCte = new Entidade.NfeCte[] 
                    { 
                        nfeCte
                    };

                    return ListanfCte;
                }

                var listaNfCte = dao.GetList(idCte, sortExpression, startRow, pageSize);
                foreach (var item in listaNfCte)
                {
                    var nfCte = new Entidade.NfeCte(item);
                    var notaFiscal = Glass.Data.DAL.NotaFiscalDAO.Instance.GetElement(item.IdNf);

                    // Caso a nota associada não exista mais, exclui do cte
                    if (notaFiscal == null)
                    {
                        dao.Delete(item);
                        continue;
                    }

                    nfCte.ObjNotaFiscal = notaFiscal;

                    retorno.Add(nfCte);
                }
            }
            return retorno.ToArray();
        }

        public Entidade.NfeCte[] GetForRpt(uint idCte)
        {
            return GetList(idCte, null, 0, int.MaxValue);
        }

        /// <summary>
        /// Retorna quantidade de registros cadastrados
        /// </summary>
        /// <returns></returns>
        public int GetCount()
        {
            using (Glass.Data.DAL.CTe.ConhecimentoTransporteDAO dao = Glass.Data.DAL.CTe.ConhecimentoTransporteDAO.Instance)
            {
                return dao.GetCount();
            }
        }
    }
}
