using System;
using System.Collections.Generic;
using Glass.Data.RelModel;
using Glass.Data.DAL;
using GDA;

namespace Glass.Data.RelDAL
{
    public sealed class ProdutoEntradaEstoqueDAO : BaseDAO<ProdutoEntradaEstoque, ProdutoEntradaEstoqueDAO>
    {
        //private ProdutoEntradaEstoqueDAO() { }

        private string Sql(uint idFornec, string nomeFornec, uint idCompra, uint numeroNFe, string dataIni, string dataFim, bool selecionar)
        {
            string criterio = "", where1 = "", where2 = "";
            string campos = selecionar ? @"pee.*, p.codInterno, p.descricao as descrProduto, p.idGrupoProd, p.idSubgrupoProd, '$$$' as criterio" : "sum(numero)";

            string campos1 = selecionar ? @"pc.idCompra, null as numeroNFe, pc.idProd, pc.qtde, pc.qtdeEntrada, pc.altura, pc.largura, 
                pc.totM, pc.valor as valorUnit, pc.total, pc.valorBenef, c.idFornec, coalesce(f.nomeFantasia, f.razaoSocial) as nomeFornec" : 
                "count(*) as numero";

            string campos2 = selecionar ? @"null as idCompra, nf.numeroNFe, pnf.idProd, pnf.qtde, pnf.qtdeEntrada, pnf.altura, pnf.largura,
                pnf.totM, pnf.valorUnitario as valorUnit, pnf.total, null as valorBenef, nf.idFornec, coalesce(f.nomeFantasia, f.razaoSocial) as nomeFornec" : 
                "count(*) as numero";

            string sql = "select " + campos + @"
                from (
                    select " + campos1 + @"
                    from produtos_compra pc
                        inner join compra c on (pc.idCompra=c.idCompra)
                        inner join fornecedor f on (c.idFornec=f.idFornec)
                    where pc.qtde>pc.qtdeEntrada{0}
                    
                    union select " + campos2 + @"
                    from produtos_nf pnf
                        inner join nota_fiscal nf on (pnf.idNf=nf.idNf)
                        inner join fornecedor f on (nf.idFornec=f.idFornec)
                    where pnf.qtde>pnf.qtdeEntrada and nf.tipoDocumento=3{1}
                ) as pee
                " + (!selecionar ? "" :
                    @"inner join produto p on (pee.idProd=p.idProd)") + @"
                where 1";

            if (idFornec > 0)
            {
                where1 += " and c.idFornec=" + idFornec;
                where2 += " and nf.idFornec=" + idFornec;
                criterio += "Fornecedor: " + FornecedorDAO.Instance.GetNome(idFornec) + "    ";
            }
            else if (!String.IsNullOrEmpty(nomeFornec))
            {
                where1 = " and f.nome like ?nomeFornec";
                where2 = " and f.nome like ?nomeFornec";
                criterio += "Fornecedor: " + nomeFornec;
            }

            if (idCompra > 0)
            {
                where1 += " and pc.idCompra=" + idCompra;
                where2 += "";
                criterio += "Compra: " + idCompra + "    ";
            }

            if (numeroNFe > 0)
            {
                where1 += "";
                where2 += " and nf.numeroNFe=" + numeroNFe;
                criterio += "NFe: " + numeroNFe + "    ";
            }

            if (!String.IsNullOrEmpty(dataIni))
            {
                where1 += " and c.dataCad>=?dataIni";
                where2 += " and nf.dataEmissao>=?dataIni";
                criterio += "Data início: " + dataIni + "    ";
            }

            if (!String.IsNullOrEmpty(dataFim))
            {
                where1 += " and c.dataCad<=?dataFim";
                where2 += " and nf.dataEmissao<=?dataFim";
                criterio += "Data fim: " + dataFim + "    ";
            }

            sql = String.Format(sql, where1, where2);
            return sql.Replace("$$$", criterio);
        }

        private GDAParameter[] GetParams(string nomeFornec, string dataIni, string dataFim)
        {
            List<GDAParameter> lst = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(nomeFornec))
                lst.Add(new GDAParameter("?nomeFornec", "%" + nomeFornec + "%"));

            if (!String.IsNullOrEmpty(dataIni))
                lst.Add(new GDAParameter("?dataIni", DateTime.Parse(dataIni + " 00:00")));

            if (!String.IsNullOrEmpty(dataFim))
                lst.Add(new GDAParameter("?dataFim", DateTime.Parse(dataFim + " 23:59")));

            return lst.ToArray();
        }

        public IList<ProdutoEntradaEstoque> GetList(uint idFornec, string nomeFornec, uint idCompra, uint numeroNFe, string dataIni, string dataFim,
            string sortExpression, int startRow, int pageSize)
        {
            return LoadDataWithSortExpression(Sql(idFornec, nomeFornec, idCompra, numeroNFe, dataIni, dataFim, true),
                sortExpression, startRow, pageSize, GetParams(nomeFornec, dataIni, dataFim));
        }

        public int GetCount(uint idFornec, string nomeFornec, uint idCompra, uint numeroNFe, string dataIni, string dataFim)
        {
            return objPersistence.ExecuteSqlQueryCount(Sql(idFornec, nomeFornec, idCompra, numeroNFe, dataIni, dataFim, false),
                GetParams(nomeFornec, dataIni, dataFim));
        }

        public IList<ProdutoEntradaEstoque> GetForRpt(uint idFornec, string nomeFornec, uint idCompra, uint numeroNFe, string dataIni, string dataFim)
        {
            return objPersistence.LoadData(Sql(idFornec, nomeFornec, idCompra, numeroNFe, dataIni, dataFim, true),
                GetParams(nomeFornec, dataIni, dataFim)).ToList();
        }
    }
}
