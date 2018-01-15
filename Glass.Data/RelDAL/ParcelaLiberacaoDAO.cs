using System.Collections.Generic;
using Glass.Data.Model;
using Glass.Data.DAL;

namespace Glass.Data.RelDAL
{
    public sealed class ParcelaLiberacaoDAO : BaseDAO<ParcelaLiberacao, ParcelaLiberacaoDAO>
    {
        //private ParcelaLiberacaoDAO() { }

        /// <summary>
        /// Obtém as parcelas da liberação.
        /// </summary>
        /// <param name="idLiberarPedido"></param>
        /// <returns></returns>
        public List<ParcelaLiberacao> ObtemParcelasLiberacao(uint idLiberarPedido)
        {
            var parcelas = ContasReceberDAO.Instance.GetByLiberacaoPedido(idLiberarPedido, true);
            var lstParcLib = new List<ParcelaLiberacao>();

            foreach (ContasReceber parc in parcelas)
            {
                var parcLib = new ParcelaLiberacao();
                parcLib.ValorParcela = parc.ValorVec;
                parcLib.DataParcela = parc.DataVec;
                lstParcLib.Add(parcLib);
            }

            return lstParcLib;
        }
    }
}
