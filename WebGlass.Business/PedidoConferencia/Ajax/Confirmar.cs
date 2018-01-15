using System;
using Glass.Data.DAL;

namespace WebGlass.Business.PedidoConferencia.Ajax
{
    public interface IConfirmar
    {
        string ConfirmarPedido(string idConferente, string idsPedido, string dataEfetuar, 
            string retificar, string idsRetificar);
    }

    internal class Confirmar : IConfirmar
    {
        public string ConfirmarPedido(string idConferente, string idsPedido, string dataEfetuar, 
            string retificar, string idsRetificar)
        {
            try
            {
                if (retificar == "false")
                {
                    PedidoConferenciaDAO.Instance.EfetuarConferencia(Glass.Conversoes.StrParaUint(idConferente), idsPedido.TrimEnd(','),
                        DateTime.Parse(dataEfetuar));

                    return "ok\tConferências associadas ao conferente.";
                }
                else
                {
                    PedidoConferenciaDAO.Instance.RetificarConferencia(Glass.Conversoes.StrParaUint(idConferente), idsPedido.TrimEnd(','),
                        DateTime.Parse(dataEfetuar), idsRetificar.TrimEnd(','));

                    return "ok\tConferências retificadas.";
                }
            }
            catch (Exception ex)
            {
                return Glass.MensagemAlerta.FormatErrorMsg("Erro\tFalha ao associar conferências ao conferente.", ex);
            }
        }
    }
}
