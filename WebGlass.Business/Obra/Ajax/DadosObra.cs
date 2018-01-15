using System;
using Glass.Data.DAL;
using Glass.Configuracoes;

namespace WebGlass.Business.Obra.Ajax
{
    public interface IDadosObra
    {
        string IsObraCliente(string idObra, string idCliente);
        string IsProdutoObra(string idPedido, string codInterno);
        string IsProdutoObraPcp(string idPedido, string codInterno);
        string GetTamanhoMaximoProduto(string idPedido, string codInterno, string totM2Produto, string idProdPed);
        string GetTamanhoMaximoProdutoPcp(string idPedido, string codInterno, string totM2Produto, string idProdPed);
    }

    internal class DadosObra : IDadosObra
    {
        public string IsObraCliente(string idObra, string idCliente)
        {
            if (String.IsNullOrEmpty(idObra))
                return null;

            return (ObraDAO.Instance.GetNomeCliente(Glass.Conversoes.StrParaUint(idObra), false) == ClienteDAO.Instance.GetNome(Glass.Conversoes.StrParaUint(idCliente))).ToString();
        }

        public string IsProdutoObra(string idPedido, string codInterno)
        {
            uint? idObra = PedidoDAO.Instance.GetIdObra(Glass.Conversoes.StrParaUint(idPedido));
            if (idObra > 0)
            {
                ProdutoObraDAO.DadosProdutoObra retorno = ProdutoObraDAO.Instance.IsProdutoObra(idObra.Value, codInterno);
                if (!retorno.ProdutoValido)
                    return "Erro;" + retorno.MensagemErro;
                else
                    return "Ok;" + retorno.ValorUnitProduto + ";" + retorno.M2Produto + ";" + retorno.AlterarValorUnitario.ToString().ToLower();
            }

            return "Ok;0;0;" + PedidoConfig.DadosPedido.AlterarValorUnitarioProduto.ToString().ToLower();
        }

        public string IsProdutoObraPcp(string idPedido, string codInterno)
        {
            uint? idObra = PedidoDAO.Instance.GetIdObra(Glass.Conversoes.StrParaUint(idPedido));
            if (idObra > 0 && PedidoConfig.DadosPedido.UsarControleNovoObra)
            {
                ProdutoObraDAO.DadosProdutoObra retorno = ProdutoObraDAO.Instance.IsProdutoObra(idObra.Value, codInterno, Glass.Conversoes.StrParaUintNullable(idPedido));
                if (!retorno.ProdutoValido)
                    return "Erro;" + retorno.MensagemErro;
                else
                    return "Ok;" + retorno.ValorUnitProduto + ";" + retorno.M2Produto + ";" + retorno.AlterarValorUnitario.ToString().ToLower();
            }

            return "Ok;0;0;" + PedidoConfig.DadosPedido.AlterarValorUnitarioProduto.ToString().ToLower();
        }

        public string GetTamanhoMaximoProduto(string idPedido, string codInterno, string totM2Produto, string idProdPed)
        {
            uint? idObra = PedidoDAO.Instance.GetIdObra(Glass.Conversoes.StrParaUint(idPedido));
            if (idObra > 0 && PedidoConfig.DadosPedido.UsarControleNovoObra)
            {
                var prod = ProdutoObraDAO.Instance.GetByCodInterno(idObra.Value, codInterno);
                if (prod == null)
                    return "Erro;Esse produto não está cadastrado no pagamento antecipado.";

                // Calcula o total de m² já inserido no pedido, desconsiderando o m² do produto passado (em caso de edição de produto)
                float tamanhoProdutos = ProdutosPedidoDAO.Instance.TotalMedidasObra(idObra.Value, codInterno, null) -
                    ProdutosPedidoDAO.Instance.ObtemTotM(Glass.Conversoes.StrParaUint(idProdPed));

                // Se for edição de produto, não deve considerar o m² do mesmo
                float tamanhoProduto = Glass.Conversoes.StrParaUint(idProdPed) == 0 ? float.Parse(totM2Produto.Replace(".", ",")) : 0;

                // Calcula a metragem máxima restante
                float tamanhoMaximoRestante = (prod.TamanhoMaximo - tamanhoProdutos + tamanhoProduto);

                // Se a metragem máxima restante for 0 e o produto obra tiver tamanho máximo, retorna 0.1, para que entre no método
                // de validação no javascript
                if (prod.TamanhoMaximo > 0 && tamanhoMaximoRestante == 0)
                    tamanhoMaximoRestante = 0.01f;

                return "Ok;" + tamanhoMaximoRestante;
            }

            return "Ok;0";
        }

        public string GetTamanhoMaximoProdutoPcp(string idPedido, string codInterno, string totM2Produto, string idProdPed)
        {
            uint? idObra = PedidoDAO.Instance.GetIdObra(Glass.Conversoes.StrParaUint(idPedido));
            if (idObra > 0 && PedidoConfig.DadosPedido.UsarControleNovoObra)
            {
                var prod = ProdutoObraDAO.Instance.GetByCodInterno(idObra.Value, codInterno);
                if (prod == null)
                    return "Erro;Esse produto não está cadastrado no pagamento antecipado.";

                // Calcula o total de m² já inserido no pedido, desconsiderando o m² do produto passado (em caso de edição de produto)
                float tamanhoProdutos = ProdutosPedidoDAO.Instance.TotalMedidasObra(idObra.Value, codInterno, Glass.Conversoes.StrParaUintNullable(idProdPed)) -
                    ProdutosPedidoEspelhoDAO.Instance.ObtemTotM(Glass.Conversoes.StrParaUint(idProdPed));

                // Se for edição de produto, não deve considerar o m² do mesmo
                float tamanhoProduto = Glass.Conversoes.StrParaUint(idProdPed) == 0 ? float.Parse(totM2Produto.Replace(".", ",")) : 0;

                // Calcula a metragem máxima restante
                float tamanhoMaximoRestante = (prod.TamanhoMaximo - tamanhoProdutos + tamanhoProduto);

                // Se a metragem máxima restante for 0 e o produto obra tiver tamanho máximo, retorna 0.1, para que entre no método
                // de validação no javascript
                if (prod.TamanhoMaximo > 0 && tamanhoMaximoRestante == 0)
                    tamanhoMaximoRestante = 0.01f;

                return "Ok;" + tamanhoMaximoRestante;
            }

            return "Ok;0";
        }
    }
}
