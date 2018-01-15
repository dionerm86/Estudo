namespace WebGlass.Business.ConhecimentoTransporte.Fluxo
{
    public class DeletarNotaFiscalCte : BaseFluxo<DeletarNotaFiscalCte>
    {
        private DeletarNotaFiscalCte() { }

        /// <summary>
        /// Apaga dados de nota fiscal associada a cte
        /// </summary>
        /// <param name="nfeCte"></param>
        /// <returns></returns>
        public int Delete(Entidade.NfeCte nfeCte)
        {
            using (Glass.Data.DAL.CTe.NotaFiscalCteDAO dao = Glass.Data.DAL.CTe.NotaFiscalCteDAO.Instance)
            {
                return dao.Delete(new Glass.Data.Model.Cte.NotaFiscalCte { IdCte = nfeCte.IdCte, IdNf = nfeCte.IdNf });
            }
        }
    }
}
