using System;
using Glass.Data.DAL;

namespace WebGlass.Business.NotaFiscal.Ajax
{
    public interface IProdutoFornecedor
    {
        string AssociaProduto(string codProd, string codFornec, string idFornec);
    }

    internal class ProdutoFornecedor : IProdutoFornecedor
    {
        public string AssociaProduto(string codProd, string codFornec, string idFornec)
        {
            var prod = ProdutoDAO.Instance.GetByCodInterno(codProd);

            if (prod == null)
                return "#ERRO#";

            try
            {
                // Desassocia o produto caso já esteja associado
                ProdutoFornecedorDAO.Instance.DesassociaProduto(Glass.Conversoes.StrParaUint(idFornec), codFornec);

                // Associa o produto do fornecedor ao novo produto selecionado
                var prodFornec = new Glass.Data.Model.ProdutoFornecedor();
                prodFornec.IdProd = (int)prod.IdProd;
                prodFornec.IdFornec = Glass.Conversoes.StrParaInt(idFornec);
                prodFornec.CodFornec = codFornec;
                ProdutoFornecedorDAO.Instance.Insert(prodFornec);
            }
            catch (Exception ex)
            {
                throw new Exception(Glass.MensagemAlerta.FormatErrorMsg("Falha ao associar produtos do fornecedor com os produtos do sistema.", ex));
            }

            return prod.CodInterno + "#" + prod.Descricao;
        }
    }
}
