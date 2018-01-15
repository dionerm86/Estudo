using GDA;

namespace WebGlass.Business.ConhecimentoTransporte.Fluxo
{
    public class CadastrarEfdCte : BaseFluxo<CadastrarEfdCte>
    {
        private CadastrarEfdCte() { }

        public uint Insert(Entidade.EfdCte efdCte)
        {
            return Insert(null, efdCte);
        }

        /// <summary>
        /// insere dados
        /// </summary>
        /// <param name="efdCte"></param>
        /// <returns></returns>
        public uint Insert(GDASession sessao, Entidade.EfdCte efdCte)
        {
            return Glass.Data.DAL.CTe.EfdCteDAO.Instance.Insert(sessao, efdCte._efdCte);
        }

        public void Update(Entidade.EfdCte efdCte)
        {
            Update(null, efdCte);
        }

        /// <summary>
        /// atualiza dados
        /// </summary>
        /// <param name="efdCte"></param>
        /// <returns></returns>
        public void Update(GDASession sessao, Entidade.EfdCte efdCte)
        {
            Glass.Data.DAL.CTe.EfdCteDAO.Instance.InsertOrUpdate(sessao, efdCte._efdCte);
        }
    }
}
