using System;
using Glass.Data.DAL;

namespace WebGlass.Business.Produto.Ajax
{
    public interface IDuplicar
    {
        string DuplicarProduto(string idsProd, string idNovoGrupo, string idNovoSubgrupo, string codInternoRemover,
            string codInternoSubstituir, string descricaoRemover, string descricaoSubstituir);
    }

    internal class Duplicar : IDuplicar
    {
        public string DuplicarProduto(string idsProd, string idNovoGrupo, string idNovoSubgrupo, string codInternoRemover,
            string codInternoSubstituir, string descricaoRemover, string descricaoSubstituir)
        {
            try
            {
                if (string.IsNullOrEmpty(idsProd))
                    return "Erro##Produto não encontrado. Atualize a tela e tente novamente.";

                if (string.IsNullOrEmpty(idNovoGrupo))
                    return "Erro##Informe o grupo do produto.";

                if (string.IsNullOrEmpty(idNovoSubgrupo) || idNovoSubgrupo == "0")
                    return "Erro##Informe o subgrupo do produto.";

                ProdutoDAO.Instance.Duplicar(idsProd.Trim(' ', ','), Glass.Conversoes.StrParaUint(idNovoGrupo), Glass.Conversoes.StrParaUintNullable(idNovoSubgrupo),
                    codInternoRemover, codInternoSubstituir, descricaoRemover, descricaoSubstituir);

                return "Ok##alert('Produtos duplicados com sucesso!'); redirectUrl(window.location.href);\n";
            }
            catch (Exception ex)
            {
                return "Erro##" + Glass.MensagemAlerta.FormatErrorMsg("Falha ao duplicar produtos.", ex);
            }
        }
    }
}
