using GDA;

namespace WebGlass.Business.ConhecimentoTransporte.Fluxo
{
    public class CadastrarCobrancaDuplCte : BaseFluxo<CadastrarCobrancaDuplCte>
    {
        private CadastrarCobrancaDuplCte() { }

        public uint Insert(Entidade.CobrancaDuplCte cobrancaDuplCte)
        {
            return Insert(null, cobrancaDuplCte);
        }

        /// <summary>
        /// Insere dados de cobrança de duplicata do cte
        /// </summary>
        /// <param name="cobrancaDuplCte"></param>
        /// <returns></returns>
        public uint Insert(GDASession sessao, Entidade.CobrancaDuplCte cobrancaDuplCte)
        {
            return Glass.Data.DAL.CTe.CobrancaDuplCteDAO.Instance.Insert(sessao, Convert(cobrancaDuplCte));
        }

        /// <summary>
        /// Atualiza dados
        /// </summary>
        /// <param name="cobrancaDuplCte"></param>
        /// <returns></returns>
        public int Update(Entidade.CobrancaDuplCte cobrancaDuplCte)
        {
            return Glass.Data.DAL.CTe.CobrancaDuplCteDAO.Instance.Update(Convert(cobrancaDuplCte));
        }

        /// <summary>
        /// Converte dados da entidade para model
        /// </summary>
        /// <param name="cobrancaDuplCte"></param>
        /// <returns></returns>
        internal Glass.Data.Model.Cte.CobrancaDuplCte Convert(Entidade.CobrancaDuplCte cobrancaDuplCte)
        {
            return new Glass.Data.Model.Cte.CobrancaDuplCte
            {
                DataVenc = cobrancaDuplCte.DataVenc,
                IdCte = cobrancaDuplCte.IdCte,
                NumeroDupl = cobrancaDuplCte.NumeroDupl,
                ValorDupl = cobrancaDuplCte.ValorDupl
            };
        }
    }
}
