using System.Collections.Generic;

namespace Glass.Global.UI.Web.Process.Funcionarios
{
    /// <summary>
    /// Classe com métodos que auxiliam no cadastro do funcionário.
    /// </summary>
    public class CadastroFuncionario
    {
        #region Tipo Pedido

        /// <summary>
        /// Recupera os tipos de pedidos compatíveis.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Colosoft.IEntityDescriptor> ObtemTiposPedido()
        {
            var tiposPedidos = new List<Glass.Data.Model.Pedido.TipoPedidoEnum>();
            foreach (var i in Colosoft.Translator.GetTranslates<Glass.Data.Model.Pedido.TipoPedidoEnum>())
            {
                var tipoPedido = (Glass.Data.Model.Pedido.TipoPedidoEnum)i.Key;

                switch (tipoPedido)
                {
                    case Data.Model.Pedido.TipoPedidoEnum.MaoDeObra:
                    case Data.Model.Pedido.TipoPedidoEnum.Producao:
                    case Data.Model.Pedido.TipoPedidoEnum.Selecione:
                        continue;
                    case Data.Model.Pedido.TipoPedidoEnum.MaoDeObraEspecial:
                        break;
                }

                yield return new Colosoft.EntityDescriptor((int)(Data.Model.Pedido.TipoPedidoEnum)i.Key, i.Translation);
            }
        }

        #endregion
    }
}
