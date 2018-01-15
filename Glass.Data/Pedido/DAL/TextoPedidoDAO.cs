using GDA;
using Glass.Data.Model;

namespace Glass.Data.DAL
{
    public sealed class TextoPedidoDAO : BaseDAO<TextoPedido, TextoPedidoDAO>
	{
        //private TextoPedidoDAO() { }

        /// <summary>
        /// Busca todos os textos selecionados para o orçamento passado
        /// </summary>
        /// <param name="idOrcamento"></param>
        /// <returns></returns>
        public TextoPedido[] GetByPedido(uint idPedido)
        {
            string sql = @"
                Select tor.*, tio.titulo, tio.descricao 
                From texto_pedido tor 
                    Inner Join texto_impr_pedido tio On (tor.idTextoImprPedido=tio.idTextoImprPedido) 
                Where idPedido=" + idPedido + @"
                Order By tor.idTextoPedido";

            return objPersistence.LoadData(sql).ToList().ToArray();
        }

        /// <summary>
        /// Associa textos de orçamento padrão aos orçamentos
        /// </summary>
        /// <param name="idOrcamento"></param>
        public void AssociaTextoPedidoPadrao(GDASession sessao, uint idPedido)
        {
            // Se não houver textos de orçamento padrão, não associa textos.
            if (objPersistence.ExecuteSqlQueryCount(sessao, "Select Count(*) From texto_impr_pedido Where buscarSempre=1") == 0)
                return;

            string sql = @"
                Insert Into texto_pedido (IdPedido, IdTextoImprPedido) 
                    (
                        Select " + idPedido + @", idTextoImprPedido From texto_impr_pedido 
                        Where buscarSempre=1
                    )
                ";

            objPersistence.ExecuteCommand(sessao, sql);
        }

        /// <summary>
        /// Delete com log de cancelamento.
        /// </summary>
        public override int Delete(TextoPedido objDelete)
        {
            var textoPedido = GetElementByPrimaryKey(objDelete.IdTextoPedido);
            var descricaoTextoPedido =
                TextoImprPedidoDAO.Instance.ObtemValorCampo<string>("Descricao",
                    string.Format("IdTextoImprPedido={0}", textoPedido.IdTextoImprPedido));
            descricaoTextoPedido = descricaoTextoPedido.Length > 182 ? descricaoTextoPedido.Substring(0, 182) : descricaoTextoPedido;

            LogCancelamentoDAO.Instance.LogTextoPedido(null, textoPedido,
                string.Format("Remoção do texto: {0}", descricaoTextoPedido), true);

            return base.Delete(textoPedido);
        }
    }
}