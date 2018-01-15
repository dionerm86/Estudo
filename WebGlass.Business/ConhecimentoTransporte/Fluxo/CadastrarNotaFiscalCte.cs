using System;

namespace WebGlass.Business.ConhecimentoTransporte.Fluxo
{
    public class CadastrarNotaFiscalCte : BaseFluxo<CadastrarNotaFiscalCte>
    {
        private CadastrarNotaFiscalCte() { }

        /// <summary>
        /// insere dados
        /// </summary>
        /// <param name="nfeCte"></param>
        /// <returns></returns>
        public uint Insert(Entidade.NfeCte nfeCte)
        {
            using (Glass.Data.DAL.CTe.NotaFiscalCteDAO dao = Glass.Data.DAL.CTe.NotaFiscalCteDAO.Instance)
            {
                var nfeCteElement = dao.GetElement(nfeCte.IdCte, nfeCte.IdNf);
                if (nfeCteElement != null && nfeCteElement.IdCte > 0)
                    throw new Exception("Esta nota já foi associada à este CT-e.");

                return dao.Insert(Convert(nfeCte));
            }
        }

        /// <summary>
        /// converte dados da entidade na model
        /// </summary>
        /// <param name="nfeCte"></param>
        /// <returns></returns>
        internal Glass.Data.Model.Cte.NotaFiscalCte Convert(Entidade.NfeCte nfeCte)
        {
            return new Glass.Data.Model.Cte.NotaFiscalCte
            {
                IdCte = nfeCte.IdCte,
                IdNf = nfeCte.IdNf
            };
        }
    }
}
