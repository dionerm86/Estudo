using GDA;

namespace WebGlass.Business.ConhecimentoTransporte.Fluxo
{
    public class CadastrarValePedagioCteRod : BaseFluxo<CadastrarValePedagioCteRod>
    {
        private CadastrarValePedagioCteRod() { }

        public uint Insert(Entidade.ValePedagioCteRod valePedagio)
        {
            return Insert(null, valePedagio);
        }

        /// <summary>
        /// insere dados
        /// </summary>
        /// <param name="valePedagio"></param>
        /// <returns></returns>
        public uint Insert(GDASession sessao, Entidade.ValePedagioCteRod valePedagio)
        {
            return Glass.Data.DAL.CTe.ValePedagioCteRodDAO.Instance.Insert(sessao, Convert(valePedagio));
        }

        /// <summary>
        /// atualiza dados
        /// </summary>
        /// <param name="valePedagio"></param>
        /// <returns></returns>
        public int Update(Entidade.ValePedagioCteRod valePedagio)
        {
            return Glass.Data.DAL.CTe.ValePedagioCteRodDAO.Instance.Update(Convert(valePedagio));
        }

        /// <summary>
        /// converte dados da entidade na model
        /// </summary>
        /// <param name="valePedagio"></param>
        /// <returns></returns>
        internal Glass.Data.Model.Cte.ValePedagioCteRod Convert(Entidade.ValePedagioCteRod valePedagio)
        {
            return new Glass.Data.Model.Cte.ValePedagioCteRod
            {
                CnpjComprador = valePedagio.CnpjComprador,
                IdCte = valePedagio.IdCte,
                IdFornec = valePedagio.IdFornec,
                NumeroCompra = valePedagio.NumeroCompra
            };
        }
    }
}
