using GDA;
using System.Linq;

namespace WebGlass.Business.ConhecimentoTransporte.Fluxo
{
    public class CadastrarCobrancaCte : BaseFluxo<CadastrarCobrancaCte>
    {
        private CadastrarCobrancaCte() { }

        public uint Insert(Entidade.CobrancaCte cobrancaCte)
        {
            return Insert(null, cobrancaCte);
        }

        /// <summary>
        /// insere dados de cobrança do cte
        /// </summary>
        /// <param name="cobrancaCte"></param>
        /// <returns></returns>
        public uint Insert(GDASession sessao, Entidade.CobrancaCte cobrancaCte)
        {
            Glass.Data.DAL.CTe.CobrancaCteDAO.Instance.Insert(sessao, Convert(cobrancaCte));

            //insere dados de cobrança de duplicata que é propriedade de CobrancaCte
            foreach (var i in cobrancaCte.ObjCobrancaDuplCte)
            {                
                i.IdCte = cobrancaCte.IdCte;
                CadastrarCobrancaDuplCte.Instance.Insert(sessao, i);
            }

            return cobrancaCte.IdCte;
        }

        public int Update(Entidade.CobrancaCte cobrancaCte)
        {
            return Update(null, cobrancaCte);
        }

        /// <summary>
        /// Atualiza cobranca cte
        /// </summary>
        /// <param name="cobrancaCte"></param>
        /// <returns></returns>
        public int Update(GDASession sessao, Entidade.CobrancaCte cobrancaCte)
        {
            //Verifica se CobrancaDuplCte, propriedade de CobrancaCte, possui dados para atualização
            if (cobrancaCte.ObjCobrancaDuplCte.Select(f => !string.IsNullOrEmpty(f.NumeroDupl) || !string.IsNullOrEmpty(f.DataVenc.ToString())
                || f.ValorDupl != 0).FirstOrDefault())
            {
                //Apaga dados antigos e insere os novos
                Glass.Data.DAL.CTe.CobrancaDuplCteDAO.Instance.Delete(sessao, cobrancaCte.IdCte);
                foreach (var i in cobrancaCte.ObjCobrancaDuplCte)
                {
                    i.IdCte = cobrancaCte.IdCte;
                    CadastrarCobrancaDuplCte.Instance.Insert(sessao, i);
                }
            }

            if (cobrancaCte.ObjCobrancaDuplCte.Select(f => string.IsNullOrEmpty(f.NumeroDupl) && string.IsNullOrEmpty(f.DataVenc.ToString())
                && f.ValorDupl == 0).FirstOrDefault())
            {
                Glass.Data.DAL.CTe.CobrancaDuplCteDAO.Instance.Delete(sessao, cobrancaCte.IdCte);
            }

            Glass.Data.DAL.CTe.CobrancaCteDAO.Instance.InsertOrUpdate(sessao, Convert(cobrancaCte));

            return 1;
        }

        /// <summary>
        /// Converte dados da entidade para model
        /// </summary>
        /// <param name="cobrancaCte"></param>
        /// <returns></returns>
        internal Glass.Data.Model.Cte.CobrancaCte Convert(Entidade.CobrancaCte cobrancaCte)
        {
            return new Glass.Data.Model.Cte.CobrancaCte
            {
                DescontoFatura = cobrancaCte.DescontoFatura,
                IdCte = cobrancaCte.IdCte,
                NumeroFatura = cobrancaCte.NumeroFatura,
                ValorLiquidoFatura = cobrancaCte.ValorLiquidoFatura,
                ValorOrigFatura = cobrancaCte.ValorOrigFatura,
                GerarContasPagar = cobrancaCte.GerarContasPagar,
                IdConta = cobrancaCte.IdConta
            };
        }
    }
}
