using System;

namespace WebGlass.Business.EncontroContas.Ajax
{
    public interface IBuscarEValidar
    {
        string ValidaClienteFornecedor(string idCli, string idFornec);
    }

    internal class BuscarEValidar : IBuscarEValidar
    {
        public string ValidaClienteFornecedor(string idCli, string idFornec)
        {
            try
            {
                if (string.IsNullOrEmpty(idCli))
                    throw new Exception("Informe o cliente.");

                if (string.IsNullOrEmpty(idFornec))
                    throw new Exception("Informe o fornecedor.");

                Fluxo.BuscarEValidar.Instance.ValidaClienteFornecedor(Glass.Conversoes.StrParaUint(idCli), Glass.Conversoes.StrParaUint(idFornec));

                return "Ok;";
            }
            catch (Exception ex)
            {
                return "Erro;" + ex.Message;
            }
        }
    }
}
