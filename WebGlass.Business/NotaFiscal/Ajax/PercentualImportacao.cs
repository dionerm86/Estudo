using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Glass.Data.DAL;
using Glass.Data.Helper;

namespace WebGlass.Business.NotaFiscal.Ajax
{
    public interface IPercentualImportacao
    {
        string ObterDadosImportacao(string idProd);
    }

    internal class PercentualImportacao : IPercentualImportacao
    {
        public string ObterDadosImportacao(string idProd)
        {
            var prod = ProdutoPercentualImportacaoDAO.Instance.Obter(Utils.StrToUint(idProd));

            if (prod == null)
                return "Erro;Não existe produto com o código informado.";

            string retorno = "Prod;" + prod.IdProd + ";" + prod.AliquotaInterEstadual + ";" + prod.PercentualImportacaoAtual + ";";

            return retorno;
        }
    }
}
