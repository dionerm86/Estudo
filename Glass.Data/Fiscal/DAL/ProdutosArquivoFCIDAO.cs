using System;
using System.Collections.Generic;
using System.Linq;
using Glass.Data.Model;
using GDA;

namespace Glass.Data.DAL
{
    public sealed class ProdutosArquivoFCIDAO : BaseDAO<ProdutosArquivoFCI, ProdutosArquivoFCIDAO>
    {
        public string Sql(uint idArquivoFCI, bool selecionar)
        {
            string campos = selecionar ? "pafci.*, p.codInterno, p.descricao as DescrProduto, p.ncm, p.gtinProduto, um.codigo as unidade" :
                "COUNT(*)";

            string sql = @"
                SELECT " + campos + @"
                FROM produtos_arquivo_fci pafci
                INNER JOIN produto p ON (pafci.idProd = p.idProd)
                LEFT JOIN unidade_medida um ON (p.idUnidadeMedida = um.idUnidadeMedida)
                WHERE 1";

            if (idArquivoFCI > 0)
                sql += " AND idArquivoFci=" + idArquivoFCI;

            return sql;
        }

        /// <summary>
        /// Recupera os produtos de um arquivo da FCI.
        /// </summary>
        /// <param name="idArquivoFci"></param>
        /// <returns></returns>
        public List<ProdutosArquivoFCI> GetByIdArquivoFci(uint idArquivoFci)
        {
            return objPersistence.LoadData(Sql(idArquivoFci, true));
        }

        /// <summary>
        /// Recupera os produtos de um arquivo FCI
        /// </summary>
        /// <param name="idArquivoFci"></param>
        /// <param name="sortExpression"></param>
        /// <param name="startRow"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IList<ProdutosArquivoFCI> ObterLista(uint idArquivoFci, string sortExpression, int startRow, int pageSize)
        {
            return LoadDataWithSortExpression(Sql(idArquivoFci, true), sortExpression, startRow, pageSize);
        }

        /// <summary>
        /// Recupera a quantidade de produtos de um arquivo FCI
        /// </summary>
        /// <param name="idArquivoFCI"></param>
        /// <returns></returns>
        public int ObterListaCount(uint idArquivoFCI)
        {
            return objPersistence.ExecuteSqlQueryCount(Sql(idArquivoFCI, false));
        }

        /// <summary>
        /// Deleta todos os produtos de um arquivo FCI
        /// </summary>
        /// <param name="idArquivoFCI"></param>
        public void DeleteByArquivoFCI(uint idArquivoFCI)
        {
            string sql = "DELETE FROM produtos_arquivo_fci WHERE idArquivoFCI=" + idArquivoFCI;
            objPersistence.ExecuteCommand(sql);
        }

        /// <summary>
        /// Atualiza o numero de controle do produto da fci
        /// </summary>
        /// <param name="idProdArquivoFci"></param>
        /// <param name="numControleFci"></param>
        public void AtualizaNumControleFci(uint idProdArquivoFci, string numControleFci)
        {
            objPersistence.ExecuteCommand("UPDATE produtos_arquivo_fci SET numControleFci=?num WHERE idProdArquivoFci=" + idProdArquivoFci,
                new GDAParameter("?num", new Guid(numControleFci).ToByteArray()));
        }

        /// <summary>
        /// Recupera o numero de controle do produto da FCI
        /// </summary>
        /// <param name="idProd"></param>
        /// <param name="conteudoImportacao"></param>
        /// <returns></returns>
        public string ObtemNumControleFci(uint idProd, decimal conteudoImportacao)
        {
            string sql = @"
                SELECT pafci.idProdArquivoFci
                FROM produtos_arquivo_fci pafci
                WHERE pafci.idProd = " + idProd + @"
                    AND pafci.numControleFci is not null
                    AND IF(?perc > 0 AND ?perc <= 40, pafci.conteudoImportacao > 0 AND pafci.conteudoImportacao <= 40,
                                IF(?perc > 40 AND ?perc <= 70, pafci.conteudoImportacao > 40 AND pafci.conteudoImportacao <= 70,
                                    IF(?perc > 70, pafci.conteudoImportacao > 70, 0)))
                ORDER BY pafci.idProdArquivoFci DESC limit 0,1";

            var idProdArqFci = objPersistence.LoadResult(sql, new GDAParameter("?perc", conteudoImportacao)).Select(f => f.GetUInt32(0)).ToList(); 

            ProdutosArquivoFCI prodArqFci = null;

            if (idProdArqFci.Count == 0)
                return "";

            prodArqFci = GetElementByPrimaryKey(idProdArqFci[0]);

            return prodArqFci.NumControleFciStr + ";" + prodArqFci.ParcelaImportada + ";" + prodArqFci.SaidaInterestadual 
                + ";" + prodArqFci.ConteudoImportacao;
        }
    }
}
