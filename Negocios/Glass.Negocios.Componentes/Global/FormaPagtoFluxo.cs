using System.Linq;

namespace Glass.Global.Negocios.Componentes
{
    /// <summary>
    /// Implementação do fluxo das formas de pagamento.
    /// </summary>
    public class FormaPagtoFluxo : Entidades.IProvedorFormaPagtoCliente
    {
        #region IProvedorFormaPagtoCliente Members

        /// <summary>
        /// Recupera a identificação da forma de pagamento do cliente.
        /// </summary>
        /// <param name="idFormaPagto"></param>
        /// <returns></returns>
        string Entidades.IProvedorFormaPagtoCliente.ObtemIdentificacao(int idFormaPagto)
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.FormaPagto>()
                .Where("IdFormaPagto=?id")
                .Add("?id", idFormaPagto)
                .Select("Descricao")
                .Execute()
                .Select(f => f.GetString(0))
                .FirstOrDefault();
        }

        #endregion
    }
}
