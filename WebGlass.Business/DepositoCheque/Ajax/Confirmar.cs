using System;
using Glass.Data.DAL;

namespace WebGlass.Business.DepositoCheque.Ajax
{
    public interface IConfirmar
    {
        string ConfirmarDeposito(string idsCheque, string idContaBanco, string dataDeposito, string taxaAntecip,
            string valorDeposito, string obs, string noCache);
    }

    internal class Confirmar : IConfirmar
    {
        public string ConfirmarDeposito(string idsCheque, string idContaBanco, string dataDeposito, string taxaAntecip,
            string valorDeposito, string obs, string noCache)
        {
            try
            {
                decimal valorDepositoSingle = decimal.Parse(valorDeposito.Replace("R$", "").Replace(" ", "").Replace(".", ""), System.Globalization.NumberStyles.AllowDecimalPoint);
                decimal taxaAntecipSingle = String.IsNullOrEmpty(taxaAntecip) ? 0 : decimal.Parse(taxaAntecip.Replace('.', ','));

                uint idDeposito = DepositoChequeDAO.Instance.EfetuarDeposito(idsCheque, Glass.Conversoes.StrParaUint(idContaBanco), DateTime.Parse(dataDeposito),
                    valorDepositoSingle, taxaAntecipSingle, obs);

                return "ok\t" + idDeposito;
            }
            catch (Exception ex)
            {
                return Glass.MensagemAlerta.FormatErrorMsg("Erro\t", ex);
            }
        }
    }
}
