using System;
using Glass.Data.DAL;

namespace WebGlass.Business.Cheque.Ajax
{
    public interface IBuscarEValidar
    {
        string GetCheques(string numero);
    }

    internal class BuscarEValidar : IBuscarEValidar
    {
        public string GetCheques(string numero)
        {
            try
            {
                string cheques = "ok\t";

                foreach (var c in ChequesDAO.Instance.GetForDeposito(Glass.Conversoes.StrParaInt(numero)))
                {
                    cheques += c.IdCheque + ";" + c.Num + ";" + c.Titular.Replace("\t", "") + ";" + c.Banco + ";" + c.Agencia + ";" + c.Conta + ";" +
                        c.Valor.ToString("C") + ";" + (c.DataVenc != null ? c.DataVenc.Value.ToString("dd/MM/yyyy") : String.Empty) + ";" + c.Obs + "|";
                }

                return cheques.Trim('|');
            }
            catch (Exception ex)
            {
                return Glass.MensagemAlerta.FormatErrorMsg("Erro\t", ex);
            }
        }
    }
}
