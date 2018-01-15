using GDA;
using System.Collections.Generic;

namespace WebGlass.Business.ConhecimentoTransporte.Fluxo
{
    public class CadastrarImpostoCte : BaseFluxo<CadastrarImpostoCte>
    {
        private CadastrarImpostoCte() { }

        public void Insert(IList<Entidade.ImpostoCte> impostoCte)
        {
            Insert(null, impostoCte);
        }

        /// <summary>
        /// insere dados
        /// </summary>
        /// <param name="impostoCte"></param>
        /// <returns></returns>
        public void Insert(GDASession sessao, IList<Entidade.ImpostoCte> impostoCte)
        {
            foreach (var i in impostoCte)
                Glass.Data.DAL.CTe.ImpostoCteDAO.Instance.Insert(sessao, Convert(i));
        }

        public void Update(IList<Entidade.ImpostoCte> impostoCte)
        {
            Update(null, impostoCte);
        }

        /// <summary>
        /// atualiza dados
        /// </summary>
        /// <param name="impostoCte"></param>
        /// <returns></returns>
        public void Update(GDASession sessao, IList<Entidade.ImpostoCte> impostoCte)
        {
            foreach (var i in impostoCte)
                Glass.Data.DAL.CTe.ImpostoCteDAO.Instance.InsertOrUpdate(sessao, Convert(i));
        }

        /// <summary>
        /// Converte dados da entidade na model
        /// </summary>
        /// <param name="impostoCte"></param>
        /// <returns></returns>
        internal Glass.Data.Model.Cte.ImpostoCte Convert(Entidade.ImpostoCte impostoCte)
        {
            return new Glass.Data.Model.Cte.ImpostoCte
            {
                IdCte = impostoCte.IdCte,
                TipoImposto = impostoCte.TipoImposto,
                Aliquota = impostoCte.Aliquota,
                AliquotaStRetido = impostoCte.AliquotaStRetido,
                BaseCalc = impostoCte.BaseCalc,
                BaseCalcStRetido = impostoCte.BaseCalcStRetido,
                Cst = impostoCte.Cst,
                PercRedBaseCalc = impostoCte.PercRedBaseCalc,
                Valor = impostoCte.Valor,
                ValorCred = impostoCte.ValorCred,
                ValorStRetido = impostoCte.ValorStRetido
            };
        }
    }
}
