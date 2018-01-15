namespace WebGlass.Business.ConhecimentoTransporte.Fluxo
{
    public class CadastrarSeguradora : BaseFluxo<CadastrarSeguradora>
    {
        private CadastrarSeguradora() { }
        
        /// <summary>
        /// insere dados
        /// </summary>
        /// <param name="seguradora"></param>
        /// <returns></returns>
        public uint Insert(Entidade.Seguradora seguradora)
        {
            return Glass.Data.DAL.CTe.SeguradoraDAO.Instance.Insert(Convert(seguradora));
        }

        /// <summary>
        /// converte dados da entidade na model
        /// </summary>
        /// <param name="seguradora"></param>
        /// <returns></returns>
        internal Glass.Data.Model.Cte.Seguradora Convert(Entidade.Seguradora seguradora)
        {
            return new Glass.Data.Model.Cte.Seguradora
            {
                NomeSeguradora = seguradora.NomeSeguradora
            };
        }
    }
}
