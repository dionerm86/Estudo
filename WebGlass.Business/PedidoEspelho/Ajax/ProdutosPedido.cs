using System;
using Glass.Data.DAL;

namespace WebGlass.Business.PedidoEspelho.Ajax
{
    public interface IProdutosPedido
    {
        string Desmembrar(string idProdPedStr, string qtdeStr);
    }

    internal class ProdutosPedido : IProdutosPedido
    {
        public string Desmembrar(string idProdPedStr, string qtdeStr)
        {
            try
            {
                uint idProdPed = Glass.Conversoes.StrParaUint(idProdPedStr);
                int qtde = Glass.Conversoes.StrParaInt(qtdeStr);
                ProdutosPedidoEspelhoDAO.Instance.Desmembrar(idProdPed, qtde);
                return "Ok;Produtos separados com sucesso!";
            }
            catch (Exception ex)
            {
                return "Erro;" + Glass.MensagemAlerta.FormatErrorMsg("Falha ao separar produtos.", ex);
            }
        }
    }
}
