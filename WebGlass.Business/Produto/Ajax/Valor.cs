using System;
using Glass.Data.DAL;
using Glass.Configuracoes;
using Glass;

namespace WebGlass.Business.Produto.Ajax
{
    public interface IValor
    {
        string GetValorMinimoOrca(string codInterno, string tipoEntrega, string idCliente, string revenda,
            string idProdOrcaStr, string percDescontoQtdeStr, string idOrcamento, string alturaStr);
        string GetValorMinimoPedido(string codInterno, string tipoPedido, string tipoEntrega, string tipoVenda,
            string idCliente, string revenda, string idProdPedStr, string percDescontoQtdeStr, string idPedido, string alturaStr);
        string GetValorMinimoPcp(string codInterno, string tipoPedido, string tipoEntrega, string idCliente,
            string revenda, string reposicao, string idProdPedStr, string percDescontoQtdeStr, string idPedido, string alturaStr);
        string AtualizaPreco(string idProd, string tipoPreco, string preco);
    }

    internal class Valor : IValor
    {
        public string GetValorMinimoOrca(string codInterno, string tipoEntrega, string idCliente, string revenda,
            string idProdOrcaStr, string percDescontoQtdeStr, string idOrcamento, string alturaStr)
        {
            float percDescontoQtde = !String.IsNullOrWhiteSpace(percDescontoQtdeStr) ? float.Parse(percDescontoQtdeStr.Replace(".", ",")) : 0;
            float altura = !string.IsNullOrWhiteSpace(alturaStr) ? float.Parse(alturaStr.Replace(".", ",")) : 0;
            uint idProdOrca;

            if (uint.TryParse(idProdOrcaStr, out idProdOrca) && idProdOrca > 0 )
                return ProdutoDAO.Instance.GetValorMinimo(idProdOrca, ProdutoDAO.TipoBuscaValorMinimo.ProdutoOrcamento,
                    revenda.ToLower() == "true", percDescontoQtde, null, null, idOrcamento.StrParaIntNullable(), altura).ToString();
            else
            {
                var prod = ProdutoDAO.Instance.GetByCodInterno(codInterno);

                // Recupera o valor mínimo do produto
                int? tipoEntr = !string.IsNullOrWhiteSpace(tipoEntrega) ? (int?)Glass.Conversoes.StrParaInt(tipoEntrega) : null;
                uint? idCli = !string.IsNullOrWhiteSpace(idCliente) ? (uint?)Glass.Conversoes.StrParaUint(idCliente) : null;
                return ProdutoDAO.Instance.GetValorMinimo(prod.IdProd, tipoEntr, idCli, revenda.ToLower() == "true", false, percDescontoQtde, null, null, idOrcamento.StrParaIntNullable(), altura).ToString();
            }
        }

        public string GetValorMinimoPedido(string codInterno, string tipoPedido, string tipoEntrega, string tipoVenda, string idCliente,
            string revenda, string idProdPedStr, string percDescontoQtdeStr, string idPedido, string alturaStr)
        {
            float percDescontoQtde = !string.IsNullOrWhiteSpace(percDescontoQtdeStr) ? float.Parse(percDescontoQtdeStr.Replace(".", ",")) : 0;
            float altura = !string.IsNullOrWhiteSpace(alturaStr) ? float.Parse(alturaStr.Replace(".", ",")) : 0;

            uint idProdPed;

            if (uint.TryParse(idProdPedStr, out idProdPed) && idProdPed > 0)
            {
                return (Glass.Conversoes.StrParaInt(tipoVenda) == (int)Glass.Data.Model.Pedido.TipoVendaPedido.Reposição ?
                    ProdutoDAO.Instance.ObtemCustoCompra((int)ProdutosPedidoDAO.Instance.ObtemIdProd(null, idProdPed)) :
                    ProdutoDAO.Instance.GetValorMinimo(idProdPed, ProdutoDAO.TipoBuscaValorMinimo.ProdutoPedido,
                    revenda.ToLower() == "true", percDescontoQtde, idPedido.StrParaIntNullable(), null, null, altura)).ToString();
            }
            else
            {
                var prod = ProdutoDAO.Instance.GetByCodInterno(codInterno);

                // Recupera o valor mínimo do produto
                int? tipoEntr = !string.IsNullOrWhiteSpace(tipoEntrega) ? (int?)Glass.Conversoes.StrParaInt(tipoEntrega) : null;
                uint? idCli = !string.IsNullOrWhiteSpace(idCliente) ? (uint?)Glass.Conversoes.StrParaUint(idCliente) : null;
                return ProdutoDAO.Instance.GetValorMinimo(prod.IdProd, tipoEntr, idCli, revenda == "true",
                    Glass.Conversoes.StrParaInt(tipoVenda) == (int)Glass.Data.Model.Pedido.TipoVendaPedido.Reposição,
                    percDescontoQtde, idPedido.StrParaIntNullable(), null, null, altura).ToString();
            }
        }

        public string GetValorMinimoPcp(string codInterno, string tipoPedido, string tipoEntrega, string idCliente, string revenda,
            string reposicao, string idProdPedStr, string percDescontoQtdeStr, string idPedido, string alturaStr)
        {
            float percDescontoQtde = !string.IsNullOrEmpty(percDescontoQtdeStr) ? float.Parse(percDescontoQtdeStr.Replace(".", ",")) : 0;
            float altura = !string.IsNullOrWhiteSpace(alturaStr) ? float.Parse(alturaStr.Replace(".", ",")) : 0;
            uint idProdPed;

            if (uint.TryParse(idProdPedStr, out idProdPed) && idProdPed > 0)
            {
                if (!PedidoConfig.DadosPedido.AlterarValorUnitarioProduto)
                {
                    decimal valorVendido = ProdutosPedidoEspelhoDAO.Instance.ObtemValorVendido(idProdPed);
                    decimal valMin = ProdutoDAO.Instance.GetValorMinimo(idProdPed, ProdutoDAO.TipoBuscaValorMinimo.ProdutoPedidoEspelho,
                        revenda == "true", percDescontoQtde, idPedido.StrParaIntNullable(), null, null, altura);

                    return Math.Min(valMin, valorVendido).ToString();
                }
                else
                    return ProdutoDAO.Instance.GetValorMinimo(idProdPed, ProdutoDAO.TipoBuscaValorMinimo.ProdutoPedidoEspelho,
                        revenda == "true", percDescontoQtde, idPedido.StrParaIntNullable(), null, null, altura).ToString();
            }
            else
            {
                var prod = ProdutoDAO.Instance.GetByCodInterno(codInterno);

                // Recupera o valor mínimo do produto
                int? tipoEntr = !string.IsNullOrWhiteSpace(tipoEntrega) ? (int?)Glass.Conversoes.StrParaInt(tipoEntrega) : null;
                uint? idCli = !string.IsNullOrWhiteSpace(idCliente) ? (uint?)Glass.Conversoes.StrParaUint(idCliente) : null;
                return ProdutoDAO.Instance.GetValorMinimo(prod.IdProd, tipoEntr, idCli, revenda == "true", reposicao.ToLower() == "true",
                    percDescontoQtde, idPedido.StrParaIntNullable(), null, null, altura).ToString();
            }
        }

        public string AtualizaPreco(string idProd, string tipoPreco, string preco)
        {
            try
            {
                ProdutoDAO.Instance.AtualizaPreco(Glass.Conversoes.StrParaUint(idProd), Glass.Conversoes.StrParaInt(tipoPreco), Glass.Conversoes.StrParaFloat(preco));
                return "Ok";
            }
            catch (Exception ex)
            {
                return "Erro\t" + Glass.MensagemAlerta.FormatErrorMsg("Falha ao atualizar preço.", ex);
            }
        }
    }
}
