using System;

namespace WebGlass.Business.InventarioEstoque.Ajax
{
    public interface IConfirmar
    {
        string ConfirmarInventario(string codigoInventario);
    }

    internal class Confirmar : IConfirmar
    {
        public string ConfirmarInventario(string codigoInventario)
        {
            try
            {
                Fluxo.Confirmar.Instance.ConfirmarInventario(Glass.Conversoes.StrParaUint(codigoInventario));
                return "Ok";
            }
            catch (Exception ex)
            {
                return "Erro|" + Glass.MensagemAlerta.FormatErrorMsg("", ex);
            }
        }
    }
}
