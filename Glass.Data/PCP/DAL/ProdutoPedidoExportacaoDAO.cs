using System.Collections.Generic;
using Glass.Data.Model;
using GDA;

namespace Glass.Data.DAL
{
    public sealed class ProdutoPedidoExportacaoDAO : BaseDAO<ProdutoPedidoExportacao, ProdutoPedidoExportacaoDAO>
    {
        //private ProdutoPedidoExportacaoDAO() { }

        public List<ProdutoPedidoExportacao> ObterProdutosExportados(uint idPedido)
        {
            string sql = @"select ppe.*, p.CodInterno, p.Descricao, pp.Altura, pp.Largura, pp.TotM, pp.Total
                            from produtos_pedido_exportacao ppe
                            inner join pedido ped on(ped.IdPedido=ppe.IdPedido)
                            inner join produto p on(p.IdProd=ppe.IdProd)
                            inner join produtos_pedido pp on(pp.IdPedido=ppe.IdPedido and pp.IdProd=ppe.IdProd)
                            where ppe.IdPedido=?idPedido order by p.Descricao asc";

            return objPersistence.LoadData(sql, new GDAParameter("?idPedido", idPedido));
        }

        public List<ProdutoPedidoExportacao> ObterProdutosExportadosPeloIdExportacao(uint idExportacao)
        {
            string sql = @"select ppe.*, p.CodInterno, p.Descricao, pp.Altura, pp.Largura, pp.TotM, pp.Total
                            from produtos_pedido_exportacao ppe
                            inner join pedido ped on(ped.IdPedido=ppe.IdPedido)                            
                            inner join produtos_pedido pp on(pp.IdPedido=ppe.IdPedido and pp.IdProdPed=ppe.IdProd)
                            inner join produto p on(p.IdProd=pp.IdProd)
                            where ppe.IdExportacao=?idExportacao order by p.Descricao asc";

            return objPersistence.LoadData(sql, new GDAParameter("?idExportacao", idExportacao));
        }

        public string ObtemIdsPedidoPeloIdExportacao(uint idExportacao)
        {
            string ids = string.Empty;

            return GetValoresCampo("Select distinct(idPedido) From produtos_pedido_exportacao Where idExportacao=?idExportacao",
                "idPedido", new GDAParameter("?idExportacao", idExportacao));
        }

        public void InserirExportado(GDASession session, uint idExportacao, uint idPedido, uint idProd)
        {
            ProdutoPedidoExportacao model = new ProdutoPedidoExportacao();
            model.IdExportacao = idExportacao;
            model.IdPedido = idPedido;
            model.IdProduto = idProd;
            Insert(session, model);
        }
    }
}
