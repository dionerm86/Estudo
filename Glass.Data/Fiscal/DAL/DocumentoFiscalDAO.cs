using System.Collections.Generic;
using Glass.Data.Model;

namespace Glass.Data.DAL
{
    public sealed class DocumentoFiscalDAO : BaseDAO<DocumentoFiscal, DocumentoFiscalDAO>
    {
        //private DocumentoFiscalDAO() { }

        private string Sql(uint idNf, bool selecionar)
        {
            string campos = selecionar ? @"df.*, l.nomeFantasia as nomeLoja, c.nome as nomeCliente,
                f.nomeFantasia as nomeFornec, t.nome as nomeTransp, a.nome as nomeAdminCartao, nf.ChaveAcesso" : "count(*)";

            string sql = "select " + campos + @"
                from documento_fiscal df
                    LEFT JOIN nota_fiscal nf ON (df.IdNf=nf.IdNf)
                    left join loja l on (df.idLoja=l.idLoja)
                    left join cliente c on (df.id_Cli=c.id_Cli)
                    left join fornecedor f on (df.idFornec=f.idFornec)
                    left join transportador t on (df.idTransportador=t.idTransportador)
                    left join administradora_cartao a on (df.idAdminCartao=a.idAdminCartao)
                where 1";

            if (idNf > 0)
                sql += " and nf.IdNf=" + idNf;

            return sql;
        }

        public IList<DocumentoFiscal> GetList(uint idNf, string sortExpression, int startRow, int pageSize)
        {
            if (GetCountReal(idNf) == 0)
                return new DocumentoFiscal[] { new DocumentoFiscal() };

            return LoadDataWithSortExpression(Sql(idNf, true), sortExpression, startRow, pageSize);
        }

        public int GetCount(uint idNf)
        {
            int retorno = GetCountReal(idNf);
            return retorno > 0 ? retorno : 1;
        }

        public int GetCountReal(uint idNf)
        {
            return objPersistence.ExecuteSqlQueryCount(Sql(idNf, false));
        }

        public IList<DocumentoFiscal> GetForEFD(uint idNf)
        {
            return objPersistence.LoadData(Sql(idNf, true)).ToList();
        }
    }
}
