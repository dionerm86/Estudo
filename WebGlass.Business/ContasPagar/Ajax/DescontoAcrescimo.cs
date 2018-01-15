using System;
using Glass.Data.DAL;

namespace WebGlass.Business.ContasPagar.Ajax
{
    public interface IDescontoAcrescimo
    {
        string AplicarDescontoAcrescimo(string idContaPg, string valorString, string descontoString,
            string acrescimoString, string motivo);
    }

    internal class DescontoAcrescimo : IDescontoAcrescimo
    {
        public string AplicarDescontoAcrescimo(string idContaPg, string valorString, string descontoString,
            string acrescimoString, string motivo)
        {
            try
            {
                Single desconto = !String.IsNullOrEmpty(descontoString) ? Single.Parse(descontoString.Replace('.', ','),
                    System.Globalization.NumberStyles.AllowDecimalPoint) : 0;

                Single acrescimo = !String.IsNullOrEmpty(acrescimoString) ? Single.Parse(acrescimoString.Replace('.', ','),
                    System.Globalization.NumberStyles.AllowDecimalPoint) : 0;

                Single valor = Single.Parse(valorString.Replace('.', ','), System.Globalization.NumberStyles.AllowDecimalPoint);
                motivo = motivo.Length > 200 ? motivo.Substring(0, 200) : motivo;

                // O desconto dado não pode ser superior ao valor da parcela
                if (desconto > valor)
                    return "Erro\tO desconto não pode ser superior ao valor da conta a pagar.";

                ContasPagarDAO.Instance.DescontaAcrescentaContaPagar(Glass.Conversoes.StrParaUint(idContaPg), desconto, acrescimo, motivo);

                return "ok\tDesconto/acréscimo aplicado.";
            }
            catch (Exception ex)
            {
                return Glass.MensagemAlerta.FormatErrorMsg("Erro\tFalha ao descontar valor da conta a pagar.", ex);
            }
        }
    }
}
