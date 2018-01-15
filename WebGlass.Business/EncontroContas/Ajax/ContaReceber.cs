using System;

namespace WebGlass.Business.EncontroContas.Ajax
{
    public interface IContaReceber
    {
        string AddContaR(string idEncontroContas, string idContaR);
    }

    internal class ContaReceber : IContaReceber
    {
        #region IContaPagar Members

        /// <summary>
        /// Adiciona uma conta recebida ao encontro de contas
        /// </summary>
        /// <param name="idContaR"></param>
        public string AddContaR(string idEncontroContas, string idContaR)
        {
            try
            {
                if (string.IsNullOrEmpty(idEncontroContas))
                    throw new Exception("Informe o encontro de contas a pagar/receber.");

                if (string.IsNullOrEmpty(idContaR))
                    throw new Exception("Informe a conta a receber.");

                Fluxo.ContaReceber.Instance.AddContaR(Glass.Conversoes.StrParaUint(idEncontroContas), Glass.Conversoes.StrParaUint(idContaR));

                return "Ok;";
            }
            catch (Exception ex)
            {
                return "Erro;" + ex.Message;
            }
        }

        #endregion
    }
}
