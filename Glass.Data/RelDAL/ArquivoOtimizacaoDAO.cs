using System;
using System.Collections.Generic;
using Glass.Data.RelModel;
using Glass.Data.DAL;

namespace Glass.Data.RelDAL
{
    public sealed class ArquivoOtimizacaoDAO : BaseDAO<ArquivoOtimizacao, ArquivoOtimizacaoDAO>
    {
        //private ArquivoOtimizacaoDAO() { }

        public IList<ArquivoOtimizacao> GetForArquivoOtimizacao(string dadosEtiquetas)
        {
            if (String.IsNullOrEmpty(dadosEtiquetas))
                return new ArquivoOtimizacao[0];

            dadosEtiquetas = dadosEtiquetas.TrimEnd('|');
            string[] etiquetas = dadosEtiquetas.Split('|');

            string idsProdPed = "";
            const string BASE_QTDE = "if(idProdPed={0}, '{1}', {2})";
            string sqlQtdeImp = BASE_QTDE;

            for (int i = 0; i < etiquetas.Length; i++)
            {
                string[] etiqueta = etiquetas[i].Split(';');
                idsProdPed += "," + etiqueta[0];

                string complQtdeImp = i < (etiquetas.Length - 1) ? BASE_QTDE : "''";
                sqlQtdeImp = String.Format(sqlQtdeImp, etiqueta[0], etiqueta[1], complQtdeImp);
            }

            idsProdPed = idsProdPed.TrimStart(',');
            if (idsProdPed.Length == 0)
                return new ArquivoOtimizacao[0];

            string sql = @"
                select group_concat(concat(cast(idProdPed as char), ';', cast(QtdeImp as char), ';') SEPARATOR '|') as Etiquetas,
                    Espessura, CorVidro
                from (
                    select ppe.idProdPed, p.codOtimizacao, " + sqlQtdeImp + @" as QtdeImp, p.Espessura, cv.Descricao as CorVidro
                    from produtos_pedido_espelho ppe 
                        left join produto p on (ppe.idProd=p.idProd)
                        left join cor_vidro cv on (p.idCorVidro=cv.idCorVidro)
                    where idProdPed in (" + idsProdPed + @")
                ) as temp
                group by codOtimizacao";

            return objPersistence.LoadData(sql).ToList();
        }
    }
}
