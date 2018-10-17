using Glass.Data.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Data.RelDAL
{
    public class MovEstoquesLogDAO : BaseDAO<RelModel.MovEstoquesLog, MovEstoquesLogDAO>
    {
        /// <summary>
        /// Recupera as movimentações no estoque real e fiscal referentes à nota passada
        /// </summary>
        public IList<RelModel.MovEstoquesLog> ObtemMovEstoquesLogNota(uint idNf)
        {
            var sql = string.Format(@"select
                                            COALESCE(tmp.IDPRODNF, tmp2.IDPRODNF) AS IDPRODNF,
                                            COALESCE(tmp.CODPROD, tmp2.CODPROD) AS CODPROD,
                                            COALESCE(tmp.DESCRICAOPROD, tmp2.DESCRICAOPROD) AS DESCRICAOPROD,
                                            tmp.MOVESTOQUEFISCAL,
                                            tmp.TIPOMOVFISCAL,
                                            tmp.QTDEMOVFISCAL,
                                            tmp.DATAMOVFISCAL,
                                            tmp2.MOVESTOQUEREAL,
                                            tmp2.TIPOMOVREAL,
                                            tmp2.QTDEMOVREAL,
                                            tmp2.DATAMOVREAL
                                        from
	                                    (SELECT pnf.IDPRODNF, p.CODINTERNO AS CODPROD, p.DESCRICAO AS DESCRICAOPROD,
		                                    mef.IDMOVESTOQUEFISCAL AS MOVESTOQUEFISCAL, (CASE mef.TIPOMOV WHEN 1 THEN 'ENTRADA' ELSE 'SAIDA' END) As TIPOMOVFISCAL,
			                                    mef.QTDEMOV AS QTDEMOVFISCAL, mef.DATAMOV AS DATAMOVFISCAL
		                                    FROM produtos_nf pnf
		                                    LEFT JOIN produto p ON pnf.IDPROD=p.IDPROD
		                                    INNER JOIN mov_estoque_fiscal mef ON pnf.IDNF=mef.IDNF AND (pnf.IDPROD=mef.IDPROD OR p.IDPRODBASE=mef.IDPROD)
		                                    WHERE pnf.IDNF = {0}
		                                    group by pnf.IDPRODNF) tmp
	                                     LEFT JOIN
		                                    (SELECT pnf.IDPRODNF, p.CODINTERNO AS CODPROD, p.DESCRICAO AS DESCRICAOPROD,
                                            me.IDMOVESTOQUE AS MOVESTOQUEREAL, (CASE me.TIPOMOV WHEN 1 THEN 'ENTRADA' ELSE 'SAIDA' END) As TIPOMOVREAL,
			                                    me.QTDEMOV AS QTDEMOVREAL, me.DATAMOV AS DATAMOVREAL
		                                    FROM produtos_nf pnf
		                                    LEFT JOIN produto p ON pnf.IDPROD=p.IDPROD
		                                    INNER JOIN mov_estoque me ON pnf.IDNF=me.IDNF AND (pnf.IDPROD=me.IDPROD OR p.IDPRODBASE=me.IDPROD)
		                                    WHERE pnf.IDNF = {0}
		                                    group by pnf.IDPRODNF) tmp2 ON tmp.IDPRODNF=tmp2.IDPRODNF
                                            group by tmp2.MOVESTOQUEREAL
	                                    UNION
                                        select
                                            COALESCE(tmp.IDPRODNF, tmp2.IDPRODNF) AS IDPRODNF,
                                            COALESCE(tmp.CODPROD, tmp2.CODPROD) AS CODPROD,
                                            COALESCE(tmp.DESCRICAOPROD, tmp2.DESCRICAOPROD) AS DESCRICAOPROD,
                                            tmp.MOVESTOQUEFISCAL,
                                            tmp.TIPOMOVFISCAL,
                                            tmp.QTDEMOVFISCAL,
                                            tmp.DATAMOVFISCAL,
                                            tmp2.MOVESTOQUEREAL,
                                            tmp2.TIPOMOVREAL,
                                            tmp2.QTDEMOVREAL,
                                            tmp2.DATAMOVREAL
                                        from
	                                    (SELECT pnf.IDPRODNF, p.CODINTERNO AS CODPROD, p.DESCRICAO AS DESCRICAOPROD,
		                                    mef.IDMOVESTOQUEFISCAL AS MOVESTOQUEFISCAL, (CASE mef.TIPOMOV WHEN 1 THEN 'ENTRADA' ELSE 'SAIDA' END) As TIPOMOVFISCAL,
			                                    mef.QTDEMOV AS QTDEMOVFISCAL, mef.DATAMOV AS DATAMOVFISCAL
		                                    FROM produtos_nf pnf
		                                    LEFT JOIN produto p ON pnf.IDPROD=p.IDPROD
		                                    INNER JOIN mov_estoque_fiscal mef ON pnf.IDNF=mef.IDNF AND (pnf.IDPROD=mef.IDPROD OR p.IDPRODBASE=mef.IDPROD)
		                                    WHERE pnf.IDNF = {0}
		                                    group by pnf.IDPRODNF) tmp
	                                     RIGHT JOIN
		                                    (SELECT pnf.IDPRODNF, p.CODINTERNO AS CODPROD, p.DESCRICAO AS DESCRICAOPROD,
                                            me.IDMOVESTOQUE AS MOVESTOQUEREAL, (CASE me.TIPOMOV WHEN 1 THEN 'ENTRADA' ELSE 'SAIDA' END) As TIPOMOVREAL,
			                                    me.QTDEMOV AS QTDEMOVREAL, me.DATAMOV AS DATAMOVREAL
		                                    FROM produtos_nf pnf
		                                    LEFT JOIN produto p ON pnf.IDPROD=p.IDPROD
		                                    INNER JOIN mov_estoque me ON pnf.IDNF=me.IDNF AND (pnf.IDPROD=me.IDPROD OR p.IDPRODBASE=me.IDPROD)
		                                    WHERE pnf.IDNF = {0}
		                                    group by pnf.IDPRODNF) tmp2 ON tmp.IDPRODNF=tmp2.IDPRODNF
                                            group by tmp2.MOVESTOQUEREAL", idNf);

            var retorno = objPersistence.LoadData(sql).ToList();

            return retorno;
        }
    }
}
