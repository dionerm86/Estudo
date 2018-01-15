using GDA;

namespace WebGlass.Business.ConhecimentoTransporte.Fluxo
{
    public class CadastrarEntregaCte : BaseFluxo<CadastrarEntregaCte>
    {
        private CadastrarEntregaCte() { }

        public uint Insert(Entidade.EntregaCte entregaCte)
        {
            return Insert(null, entregaCte);
        }

        /// <summary>
        /// insere dados
        /// </summary>
        /// <param name="entregaCte"></param>
        /// <returns></returns>
        public uint Insert(GDASession sessao, Entidade.EntregaCte entregaCte)
        {
            return Glass.Data.DAL.CTe.EntregaCteDAO.Instance.Insert(sessao, Convert(entregaCte));
        }

        public int Update(Entidade.EntregaCte entregaCte)
        {
            return Update(null, entregaCte);
        }

        /// <summary>
        /// atualiza dados
        /// </summary>
        /// <param name="entregaCte"></param>
        /// <returns></returns>
        public int Update(GDASession sessao, Entidade.EntregaCte entregaCte)
        {
            return Glass.Data.DAL.CTe.EntregaCteDAO.Instance.Update(sessao, Convert(entregaCte));
        }

        /// <summary>
        /// Converte dados da entidade na model
        /// </summary>
        /// <param name="entregaCte"></param>
        /// <returns></returns>
        internal Glass.Data.Model.Cte.EntregaCte Convert(Entidade.EntregaCte entregaCte)
        {
            return new Glass.Data.Model.Cte.EntregaCte
            {
                DataHoraFim = entregaCte.DataHoraFim,
                DataHoraIni = entregaCte.DataHoraIni,
                DataHoraProg = entregaCte.DataHoraProg,
                IdCte = entregaCte.IdCte,
                TipoPeriodoData = entregaCte.TipoPeriodoData,
                TipoPeriodoHora = entregaCte.TipoPeriodoHora
            };
        }
    }
}
