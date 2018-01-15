using GDA;
using System.Linq;

namespace WebGlass.Business.ConhecimentoTransporte.Fluxo
{
    public class CadastrarInfoCte : BaseFluxo<CadastrarInfoCte>
    {
        private CadastrarInfoCte() { }

        public uint Insert(Entidade.InfoCte infoCte)
        {
            return Insert(null, infoCte);
        }

        /// <summary>
        /// insere dados
        /// </summary>
        /// <param name="infoCte"></param>
        /// <returns></returns>
        public uint Insert(GDASession sessao, Entidade.InfoCte infoCte)
        {
            Glass.Data.DAL.CTe.InfoCteDAO.Instance.Insert(sessao, Convert(infoCte));

            foreach (var i in infoCte.ObjInfoCargaCte.ToList())
            {
                i.IdCte = infoCte.IdCte;
                CadastrarInfoCargaCte.Instance.Insert(sessao, i);
            }

            return infoCte.IdCte;
        }

        public int Update(Entidade.InfoCte infoCte)
        {
            return Update(null, infoCte);
        }

        public int Update(GDASession sessao, Entidade.InfoCte infoCte)
        {
            foreach (var i in infoCte.ObjInfoCargaCte.ToList())
                i.IdCte = infoCte.IdCte;

            CadastrarInfoCargaCte.Instance.AtualizarInfoCarga(sessao, infoCte.ObjInfoCargaCte);

            Glass.Data.DAL.CTe.InfoCteDAO.Instance.InsertOrUpdate(sessao, Convert(infoCte));

            return 1;
        }

        /// <summary>
        /// converte dados da entidade na model
        /// </summary>
        /// <param name="infoCte"></param>
        /// <returns></returns>
        internal Glass.Data.Model.Cte.InfoCte Convert(Entidade.InfoCte infoCte)
        {
            return new Glass.Data.Model.Cte.InfoCte
            {
                IdCte = infoCte.IdCte,
                ProdutoPredominante = infoCte.ProdutoPredominante,
                OutrasCaract = infoCte.OutrasCaract,
                ValorCarga = infoCte.ValorCarga
            };
        }
    }
}
