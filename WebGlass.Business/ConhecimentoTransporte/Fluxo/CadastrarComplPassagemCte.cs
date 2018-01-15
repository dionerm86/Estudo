using GDA;

namespace WebGlass.Business.ConhecimentoTransporte.Fluxo
{
    public class CadastrarComplPassagemCte : BaseFluxo<CadastrarComplPassagemCte>
    {
        private CadastrarComplPassagemCte() { }

        public uint Insert(Entidade.ComplPassagemCte complPassagemCte)
        {
            return Insert(null, complPassagemCte);
        }

        /// <summary>
        /// insere dados
        /// </summary>
        /// <param name="complPassagemCte"></param>
        /// <returns></returns>
        public uint Insert(GDASession sessao, Entidade.ComplPassagemCte complPassagemCte)
        {
            return Glass.Data.DAL.CTe.ComplPassagemCteDAO.Instance.Insert(sessao, Convert(complPassagemCte));
        }

        public int Update(Entidade.ComplPassagemCte complPassagemCte)
        {
            return Update(null, complPassagemCte);
        }

        /// <summary>
        /// Atualiza dados
        /// </summary>
        /// <param name="complPassagemCte"></param>
        /// <returns></returns>
        public int Update(GDASession sessao, Entidade.ComplPassagemCte complPassagemCte)
        {
            return Glass.Data.DAL.CTe.ComplPassagemCteDAO.Instance.Update(sessao, Convert(complPassagemCte));
        }

        /// <summary>
        /// Converte dados da entidade na model
        /// </summary>
        /// <param name="complPassagemCte"></param>
        /// <returns></returns>
        internal Glass.Data.Model.Cte.ComplPassagemCte Convert(Entidade.ComplPassagemCte complPassagemCte)
        {
            return new Glass.Data.Model.Cte.ComplPassagemCte
            {
                IdCte = complPassagemCte.IdCte,
                NumSeqPassagem = complPassagemCte.NumSeqPassagem,
                SiglaPassagem = complPassagemCte.SiglaPassagem
            };
        }
    }
}
