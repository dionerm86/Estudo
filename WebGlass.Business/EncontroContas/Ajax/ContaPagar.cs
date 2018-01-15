using System;

namespace WebGlass.Business.EncontroContas.Ajax
{
    public interface IContaPagar
    {
        string AddContaPg(string idEncontroContas, string idContaPg);
    }

    internal class ContaPagar : IContaPagar
    {
        #region IContaPagar Members

        public string AddContaPg(string idEncontroContas, string idContaPg)
        {
            try
            {
                if (string.IsNullOrEmpty(idEncontroContas))
                    throw new Exception("Informe o encontro de contas a pagar/receber.");

                if (string.IsNullOrEmpty(idContaPg))
                    throw new Exception("Informe a conta a pagar.");

                Fluxo.ContaPagar.Instance.AddContaPg(Glass.Conversoes.StrParaUint(idEncontroContas), Glass.Conversoes.StrParaUint(idContaPg));

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
