using System.Collections.Generic;
using GDA;

namespace WebGlass.Business.ConhecimentoTransporte.Fluxo
{
    public class CadastrarOrdemColetaCteRod : BaseFluxo<CadastrarOrdemColetaCteRod>
    {
        private CadastrarOrdemColetaCteRod() { }

        public uint Insert(Entidade.OrdemColetaCteRod ordemColeta)
        {
            return Insert(null, ordemColeta);
        }

        /// <summary>
        /// insere dados
        /// </summary>
        /// <param name="ordemColeta"></param>
        /// <returns></returns>
        public uint Insert(GDASession sessao, Entidade.OrdemColetaCteRod ordemColeta)
        {
            return Glass.Data.DAL.CTe.OrdemColetaCteRodDAO.Instance.Insert(sessao, Convert(ordemColeta));
        }

        /// <summary>
        /// insere lista de ordens de coleta do cte
        /// </summary>
        /// <param name="listaOrdensColetaCte"></param>
        /// <returns></returns>
        public List<uint> Insert(List<Entidade.OrdemColetaCteRod> listaOrdensColetaCte)
        {
            var lista = new List<uint>();
            foreach (var i in listaOrdensColetaCte)
                lista.Add(Glass.Data.DAL.CTe.OrdemColetaCteRodDAO.Instance.Insert(Convert(i)));

            return lista;
        }

        /// <summary>
        /// atualiza lista de ordens de coleta
        /// </summary>
        /// <param name="listaOrdensColetaCte"></param>
        /// <returns></returns>
        public List<uint> Update(List<Entidade.OrdemColetaCteRod> listaOrdensColetaCte)
        {
            var lista = new List<uint>();
            foreach (var i in listaOrdensColetaCte)
            {
                Glass.Data.DAL.CTe.OrdemColetaCteRodDAO.Instance.Delete(i.IdCte);
                lista.Add(Insert(i));
            }

            return lista;
        }

        /// <summary>
        /// atualiza ordem de coleta
        /// </summary>
        /// <param name="ordemColeta"></param>
        /// <returns></returns>
        public int Update(Entidade.OrdemColetaCteRod ordemColeta)
        {
            return Glass.Data.DAL.CTe.OrdemColetaCteRodDAO.Instance.Update(Convert(ordemColeta));
        }

        /// <summary>
        /// converte dados da entidade na model
        /// </summary>
        /// <param name="ordemColeta"></param>
        /// <returns></returns>
        internal Glass.Data.Model.Cte.OrdemColetaCteRod Convert(Entidade.OrdemColetaCteRod ordemColeta)
        {
            return new Glass.Data.Model.Cte.OrdemColetaCteRod
                {
                    DataEmissao = ordemColeta.DataEmissao,
                    IdCte = ordemColeta.IdCte,
                    IdTransportador = ordemColeta.IdTransportador,
                    Numero = ordemColeta.Numero,
                    Serie = ordemColeta.Serie
                };
        }
    }
}
