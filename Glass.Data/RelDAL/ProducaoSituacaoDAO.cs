using System;
using System.Collections.Generic;
using Glass.Data.DAL;
using Glass.Data.RelModel;
using GDA;
using Glass.Data.Model;

namespace Glass.Data.RelDAL
{
    public sealed class ProducaoSituacaoDAO : BaseDAO<ProducaoSituacao, ProducaoSituacaoDAO>
    {
        //private ProducaoSituacaoDAO() { }

//        private string Sql(uint idFuncSetor, uint idPedido, string dataIni, string dataFim, bool selecionar, bool relatorio)
//        {
//            string campos = !selecionar ? "idPedido" : 
//                !relatorio ? @"Data, idPedido, Sum(TotM2) as TotM2, Valor, DataLiberacao, dataConf, Criterio, 
//                cast(group_concat(somaSetor) as char) as somasSetores, cast(group_concat(idSetor) as char) as idsSetores,
//                group_concat(nomeSetor) as nomesSetores, cast(group_concat(idFunc) as char) as idsFuncSetores" : 
//                @"Data, idPedido, TotM2, Valor, DataLiberacao, dataConf, Criterio, cast(somaSetor as char) as somasSetores, 
//                Cast(idSetor as char) as idsSetores, nomeSetor as nomesSetores, cast(idFunc as char) as idsFuncSetores";

//            string camposInt = !selecionar ? "p.idPedido" : 
//                @"coalesce(p.DataPedido, p.DataCad) as Data, p.idPedido, sum(pp.totM/(pp.qtde * (select count(*) + 1 from setor))) as TotM2, 
//                p.Total as Valor, lp.DataLiberacao, p.DataConf, dados.nomeSetor, dados.numSeqSetor, coalesce(dados.idFunc, '') as idFunc,
//                '$$$' as Criterio, coalesce(dados.idSetor, '') as idSetor, sum(if(ppp.situacao=" + (int)ProdutoPedidoProducao.SituacaoEnum.
//                Producao + @", if(ppp.idSetor=dados.idSetor, pp.totM/pp.qtde, 0), if(dados.idSetor is null, pp.totM/pp.qtde, 0))) as somaSetor";

//            string idsPedidos = idPedido > 0 ? "" : GetValoresCampo(@"select idPedido from pedido where coalesce(DataPedido, DataCad)>=?dataIni 
//                and coalesce(DataPedido, DataCad)<=?dataFim and situacao<>" + (int)Pedido.SituacaoPedido.Cancelado, "idPedido", GetParams(dataIni, dataFim));

//            if (String.IsNullOrEmpty(idsPedidos))
//                idsPedidos = "0";

//            string where = idPedido > 0 ? " and {0}.idPedido=" + idPedido : @" and {0}.idPedido in (" + idsPedidos + ")";
//            string whereInt = where;

//            if (idFuncSetor > 0)
//            {
//                string idsProdPedProducao = GetValoresCampo(@"select idProdPedProducao from leitura_producao 
//                    where idFuncLeitura=" + idFuncSetor, "idProdPedProducao");

//                if (String.IsNullOrEmpty(idsProdPedProducao))
//                    idsProdPedProducao = "0";

//                where += @" and ppp.idProdPedProducao in (" + idsProdPedProducao + ")";
//                whereInt += " and lp1.idFuncLeitura=" + idFuncSetor;
//            }

//            string sql = @"
//                select " + campos + @"
//                from (
//                    select " + camposInt + @"
//                    from produto_pedido_producao ppp
//                        left join produtos_pedido pp on (ppp.idProdPed=pp.idProdPedEsp)
//                        left join pedido p on (pp.idPedido=p.idPedido)
//                        left join liberarpedido lp on (p.idLiberarPedido=lp.idLiberarPedido)
//                        " + (selecionar ? @"inner join (
//                            select idPedido, idSetor" + (selecionar ? ", nomeSetor, numSeqSetor, idFunc" : "") + @"
//                            from (
//                                " + ProducaoDAO.Instance.SqlSetores(true, false, String.Format(whereInt, "ped1"), null, selecionar) + @"
//                            ) as temp1
//
//                            /* Este Group By deve ter 2 espaços entre as duas palavras, pois, ao utilizar o método LoadDataWithSortExpression
//                               é verificado a primeira ocorrência de 'Group By' e é inserido o Where antes desta ocorrência */
//    
//                            group  by idPedido, idSetor
//                        ) as dados on (p.idPedido=dados.idPedido)" : "") + @"
//                    where 1" + String.Format(where, "p") + @"
//                    group by p.idPedido" + (selecionar ? ", dados.idSetor" : "") + @"
//                    order by coalesce(p.DataPedido, p.DataCad), p.idPedido
//                ) as temp
//                ";

//            if (!relatorio)
//                sql += "group by idPedido";
//            else
//                sql += "order by data, idPedido, numSeqSetor";

//            string criterio = "Data início: " + dataIni + "    Data fim: " + dataFim;
//            if (idFuncSetor > 0)
//                criterio += "    Funcionário: " + FuncionarioDAO.Instance.GetNome(idFuncSetor);
//            if (idPedido > 0)
//                criterio += "    Pedido: " + idPedido;

//            sql = selecionar ? sql : "select count(*) from (" + sql + ") as temp";
//            return sql.Replace("$$$", criterio);
//        }

        private string Sql(uint idFuncSetor, uint idPedido, string dataIni, string dataFim, bool selecionar, bool relatorio)
        {
            string composIinternos = @"
                COALESCE(ped.DataPedido, ped.DataCad) as Data, ped.IdPedido, pedEsp.Total as Valor, pedEsp.TotM AS TotM2, ped.DataConf, libP.DataLiberacao,
                    s.descricao as NomeSetor,
                    SUM(IF(ped.TipoPedido=" + (int)Pedido.TipoPedidoEnum.MaoDeObra + @", (
                        (((50 - If(MOD(a.Altura, 50) > 0, MOD(a.Altura, 50), 50)) +
                        a.Altura) * ((50 - IF(MOD(a.Largura, 50) > 0, MOD(a.Largura, 50), 50)) + a.Largura)) / 1000000)             
                        * a.Qtde, pp.TotM2Calc)/(pp.Qtde*IF(ped.TipoPedido=" + (int)Pedido.TipoPedidoEnum.MaoDeObra + @", a.Qtde, 1))) AS TotMSetor,
                    lp.idSetor, lp.idFuncLeitura as idFuncSetor";

            string campos = "Data, IdPedido, Valor, DataConf, DataLiberacao, CAST(TotM2 as Decimal(12,2)) as TotM2," + (!relatorio ? @"
                   GROUP_CONCAT(NomeSetor) as NomesSetores, GROUP_CONCAT(TotMSetor) as SomasSetores, GROUP_CONCAT(idSetor) as IdsSetores, GROUP_CONCAT(idFuncSetor) as IdsFuncSetores" :
                   "NomeSetor as NomesSetores, CAST(TotMSetor as char) as SomasSetores, CAST(idSetor as char) as IdsSetores, CAST(idFuncSetor as char) as IdsFuncSetores");

            var sql = @"
                SELECT " + composIinternos + @"
                FROM produto_pedido_producao ppp
                    INNER JOIN produtos_pedido pp ON (ppp.IdProdPed = pp.IdProdPedEsp)
                    LEFT JOIN ambiente_pedido a ON (a.IdAmbientePedido = pp.IdAmbientePedido)
                    INNER JOIN pedido ped ON (pp.IdPedido = ped.IdPedido)
                    INNER JOIN pedido_espelho pedEsp ON (pp.IdPedido = pedEsp.IdPedido)
                    LEFT JOIN liberarpedido libP on (ped.IdLiberarPedido = libP.IdLiberarPedido)
                    LEFT JOIN leitura_producao lp ON (ppp.IdProdPedProducao = lp.IdProdPedProducao)
                    LEFT JOIN setor s ON (lp.idSetor = s.idSetor)
                WHERE s.ExibirSetores IS NOT NULL AND s.ExibirSetores=1
                    AND COALESCE(pp.InvisivelFluxo, False) = False 
                    AND ped.situacao <> " + (int)Pedido.SituacaoPedido.Cancelado + @"
                    {0}
                GROUP BY ped.IdPedido, lp.IdSetor {1}
                ORDER BY ped.IdPedido desc";


            sql = @"SELECT " + campos + @" FROM (" + sql + @") as tmp";

            if (!relatorio)
                sql += " GROUP BY IdPedido";


            string filtro = @"";
            string criterio = @"";

            if (!string.IsNullOrEmpty(dataIni))
            {
                filtro += " AND COALESCE(ped.DataPedido, ped.DataCad) >= ?dataIni";
                criterio += "Data início: " + dataIni + "   ";
            }

            if (!string.IsNullOrEmpty(dataFim))
            {
                filtro += " AND COALESCE(ped.DataPedido, ped.DataCad) <= ?dataFim";
                criterio += "Data fim: " + dataFim + "   ";
            }

            if (idFuncSetor > 0)
            {
                filtro += " AND lp.IdFuncLeitura = " + idFuncSetor;
                criterio += "Funcionário: " + FuncionarioDAO.Instance.GetNome(idFuncSetor) + "    ";
            }

            if (idPedido > 0)
            {
                filtro += " AND ped.IdPedido = " + idPedido;
                criterio += "Pedido: " + idPedido + " ";
            }

            sql = string.Format(sql, filtro, idFuncSetor > 0 || relatorio ? ", lp.IdFuncLeitura" : "");

            if (!selecionar)
                sql = "SELECT COUNT(*) FROM (" + sql + ") AS c";

            return sql;
        }

        private GDAParameter[] GetParams(string dataIni, string dataFim)
        {
            List<GDAParameter> lstParam = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(dataIni))
                lstParam.Add(new GDAParameter("?dataIni", DateTime.Parse(dataIni + " 00:00:00")));

            if (!String.IsNullOrEmpty(dataFim))
                lstParam.Add(new GDAParameter("?dataFim", DateTime.Parse(dataFim + " 23:59:59")));

            return lstParam.ToArray();
        }

        public IList<ProducaoSituacao> GetList(uint idFuncSetor, uint idPedido, string dataIni, string dataFim, string sortExpression, int startRow, int pageSize)
        {
            return LoadDataWithSortExpression(Sql(idFuncSetor, idPedido, dataIni, dataFim, true, false), sortExpression, startRow, pageSize, GetParams(dataIni, dataFim));
        }

        public int GetCount(uint idFuncSetor, uint idPedido, string dataIni, string dataFim)
        {
            return objPersistence.ExecuteSqlQueryCount(Sql(idFuncSetor, idPedido, dataIni, dataFim, false, false), GetParams(dataIni, dataFim));
        }

        public IList<ProducaoSituacao> GetForRpt(uint idFuncSetor, uint idPedido, string dataIni, string dataFim)
        {
            return objPersistence.LoadData(Sql(idFuncSetor, idPedido, dataIni, dataFim, true, true), GetParams(dataIni, dataFim)).ToList();
        }
    }
}
