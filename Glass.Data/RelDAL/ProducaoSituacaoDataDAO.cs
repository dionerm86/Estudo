using System;
using System.Collections.Generic;
using Glass.Data.DAL;
using Glass.Data.RelModel;
using GDA;
using Glass.Data.Model;
using Glass.Data.Helper;

namespace Glass.Data.RelDAL
{
    public sealed class ProducaoSituacaoDataDAO : BaseDAO<ProducaoSituacaoData, ProducaoSituacaoDataDAO>
    {
        //private ProducaoSituacaoDataDAO() { }

        private string SqlData(string idSetor, string idPedido)
        {
            string sqlBase = @"
                select max(Data)
                from (
                    select lp1.idSetor, lp1.dataLeitura as Data, pp1.idProdPed, pp1.idPedido
                    from leitura_producao lp1
                        left join produto_pedido_producao ppp1 on (lp1.idProdPedProducao=ppp1.idProdPedProducao)
                        left join produtos_pedido pp1 on (ppp1.idProdPed=pp1.idProdPedEsp)
                    where lp1.dataLeitura is not null and ppp1.situacao=" + (int)ProdutoPedidoProducao.SituacaoEnum.Producao + @"
                ) as temp3
                where temp3.idSetor={0}
                    and temp3.idPedido={1}
                having count(*)=(select sum(qtde) from produtos_pedido where idPedido={1})";

            return String.Format(sqlBase, idSetor, idPedido);
        }

        private string Sql(string dataIni, string dataFim, uint idPedido, uint idCliente, string nomeCliente, uint idRota, uint idLoja, bool somenteLiberados, bool selecionar)
        {
            string where = " AND {0}.situacao<>" + (int)Pedido.SituacaoPedido.Cancelado;

            if (idPedido > 0)
                where += " AND {0}.IdPedido = " + idPedido;

            if (!string.IsNullOrEmpty(dataIni))
                where += " AND coalesce({0}.DataPedido, {0}.DataCad)>=?dataIni";

            if (!string.IsNullOrEmpty(dataFim))
                where += " AND coalesce({0}.DataPedido, {0}.DataCad)<=?dataFim";

            if (idCliente > 0)
                where += " AND {0}.IdCli = " + idCliente;
            else if (!string.IsNullOrEmpty(nomeCliente))
                where += " AND {0}.IdCli IN (" + ClienteDAO.Instance.GetIds(null, nomeCliente, null, 0, null, null, null, null, 0) + ")";

            if(idRota>0)
                where += " AND {0}.IdCli IN (SELECT IdCliente FROM rota_cliente WHERE IdRota = " + idRota + ")";

            if (idLoja > 0)
                where += " AND {0}.IdLoja = " + idLoja;

            //if (somenteLiberados)
            //    where += " AND lp.IdLiberarPedido IS NOT NULL";



//            string where = idPedido > 0 ? " and {0}.idPedido=" + idPedido :
//                @" and {0}.idPedido in (
//                    select p.idPedido
//                    from pedido p
//                        inner join cliente c on (p.idCli=c.id_Cli)
//                    where coalesce(p.DataPedido, p.DataCad)>=?dataIni 
//                        and coalesce(p.DataPedido, p.DataCad)<=?dataFim
//                        and p.situacao<>" + (int)Pedido.SituacaoPedido.Cancelado + @"
//                        " + (
//                            idCliente > 0 ? "and p.idCli=" + idCliente :
//                                !String.IsNullOrEmpty(nomeCliente) ? "and c.id_Cli In (" +
//                                ClienteDAO.Instance.GetIds(null, nomeCliente, null, 0, null, null, null, null, 0) + ") " : ""
//                        ) + @"
//                )";

            string sql = @"
                select data, idPedido, totM2, valor, dataLiberacao, dataConf" + (selecionar ? @", group_concat(nomeSetor) as nomesSetores, 
                    cast(group_concat(dataSetor) as char) as datasSetores, cast(group_concat(idSetor) as char) as idsSetores, criterio" : "") + @"
                from (
                    select coalesce(p.DataPedido, p.DataCad) as Data, p.idPedido, round(sum(pp.totM/(pp.qtde*if(p.tipoPedido=" + (int)Pedido.TipoPedidoEnum.MaoDeObra +
                        @", a.qtde, 1))), 4) as TotM2, p.Total as Valor, lp.DataLiberacao, p.DataConf" + (selecionar ? @", dados.nomeSetor, dados.numSeqSetor,
                        coalesce(max(dados.dataSetor), '') as dataSetor, '$$$' as Criterio, coalesce(dados.idSetor, '') as idSetor" : "") + @"
                    from produto_pedido_producao ppp
                        left join produtos_pedido_espelho pp on (ppp.idProdPed=pp.idProdPed)
                        left join ambiente_pedido_espelho a on (pp.idAmbientePedido=a.idAmbientePedido)
                        left join pedido p on (pp.idPedido=p.idPedido)
                        left join liberarpedido lp on (p.idLiberarPedido=lp.idLiberarPedido)
                        inner join (
                            select idPedido, idSetor" + (selecionar ? ", nomeSetor, max(dataSetor) as dataSetor, numSeqSetor" : "") + @"
                            from (
                                " + ProducaoDAO.Instance.SqlSetores(true, false, String.Format(where, "ped1"), GetParams(dataIni, dataFim, null), selecionar) + @"
                            ) as temp1

                            /* Este Group By deve ter 2 espaços entre as duas palavras, pois, ao utilizar o método LoadDataWithSortExpression
                               é verificado a primeira ocorrência de 'Group By' e é inserido o Where antes desta ocorrência */

                            group  by idPedido, idSetor
                        ) as dados on (p.idPedido=dados.idPedido)
                    where 1" + (somenteLiberados ? " AND lp.IdLiberarPedido IS NOT NULL" : "") + String.Format(where, "p") + @"

                    /* Este Group By deve ter 2 espaços entre as duas palavras, pois, ao utilizar o método LoadDataWithSortExpression
                       é verificado a primeira ocorrência de 'Group By' e é inserido o Where antes desta ocorrência */

                    group  by p.idPedido, dados.idSetor
                ) as temp
                group by idPedido";

            string criterio = "Data início: " + dataIni + "    Data fim: " + dataFim + "    ";

            if (!selecionar)
                sql = "select count(*) from (" + sql + ") as temp";

            return sql.Replace("$$$", criterio);
        }

        private GDAParameter[] GetParams(string dataIni, string dataFim, string nomeCliente)
        {
            List<GDAParameter> lstParam = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(dataIni))
                lstParam.Add(new GDAParameter("?dataIni", DateTime.Parse(dataIni + " 00:00:00")));

            if (!String.IsNullOrEmpty(dataFim))
                lstParam.Add(new GDAParameter("?dataFim", DateTime.Parse(dataFim + " 23:59:59")));

            if (!String.IsNullOrEmpty(nomeCliente))
                lstParam.Add(new GDAParameter("?nomeCliente", "%" + nomeCliente + "%"));

            return lstParam.ToArray();
        }

        internal DateTime? GetDataByPedido(uint idPedido, uint idSetor)
        {
            string sql = ProducaoSituacaoDataDAO.Instance.SqlData(idSetor.ToString(), idPedido.ToString());
            object retorno = objPersistence.ExecuteScalar(sql);

            DateTime? data = retorno != null ? (DateTime?)retorno : null;
            return data;
        }

        public IList<ProducaoSituacaoData> GetList(string dataIni, string dataFim, uint idPedido, uint idCliente, string nomeCliente, 
            string sortExpression, int startRow, int pageSize)
        {
            var result =  LoadDataWithSortExpression(Sql(dataIni, dataFim, idPedido, idCliente, nomeCliente, 0, 0, false, true), null, startRow, pageSize,
                GetParams(dataIni, dataFim, nomeCliente));

            return result;
        }

        public int GetCount(string dataIni, string dataFim, uint idPedido, uint idCliente, string nomeCliente)
        {
            return objPersistence.ExecuteSqlQueryCount(Sql(dataIni, dataFim, idPedido, idCliente, nomeCliente, 0, 0, false, false), 
                GetParams(dataIni, dataFim, nomeCliente));
        }

        public ProducaoSituacaoData[] GetForRpt(string dataIni, string dataFim, uint idPedido, uint idCliente, string nomeCliente)
        {
            List<ProducaoSituacaoData> itens = objPersistence.LoadData(Sql(dataIni, dataFim, idPedido, idCliente, nomeCliente, 0, 0, false, true), 
                GetParams(dataIni, dataFim, nomeCliente));

            List<ProducaoSituacaoData> retorno = new List<ProducaoSituacaoData>();

            foreach (ProducaoSituacaoData item in itens)
            {
                for (int i = 0; i < item.TextoSetor.Length; i++)
                {
                    ProducaoSituacaoData novo = new ProducaoSituacaoData();
                    novo.Criterio = item.Criterio;
                    novo.Data = item.Data;
                    novo.DataConf = item.DataConf;
                    novo.DataLiberacao = item.DataLiberacao;
                    novo.DatasSetores = item.TextoSetor[i] != null ? item.TextoSetor[i].Replace("<br />", "\n") : null;
                    novo.IdPedido = item.IdPedido;
                    novo.IdsSetores = i < Utils.GetSetores.Length ? Utils.GetSetores[i].IdSetor.ToString() : null;
                    novo.NomesSetores = item.NomeSetor[i];
                    novo.TempoTotal = item.TempoTotal;
                    novo.TempoTotalHoras = item.TempoTotalHoras;
                    novo.TotM2 = item.TotM2;
                    novo.Valor = item.Valor;

                    retorno.Add(novo);
                }
            }

            return retorno.ToArray();
        }

        #region TempoLiberacao

        public ProducaoSituacaoData[] GetForRptTempoLiberacao(string dataIni, string dataFim, uint idPedido, uint idCliente, string nomeCliente, uint idLoja, uint idRota)
        {
            return objPersistence.LoadData(Sql(dataIni, dataFim, idPedido, idCliente, nomeCliente, idRota, idLoja, true, true),
                GetParams(dataIni, dataFim, nomeCliente)).ToList().ToArray();
        }

        public IList<ProducaoSituacaoData> GetListTempoLiberacao(string dataIni, string dataFim, uint idPedido, uint idCliente, string nomeCliente,
            uint idLoja, uint idRota,
            string sortExpression, int startRow, int pageSize)
        {
            var result = LoadDataWithSortExpression(Sql(dataIni, dataFim, idPedido, idCliente, nomeCliente, idRota, idLoja, true, true), null, startRow, pageSize,
                GetParams(dataIni, dataFim, nomeCliente));

            return result;
        }

        public int GetCountTempoLiberacao(string dataIni, string dataFim, uint idPedido, uint idCliente, string nomeCliente, uint idLoja, uint idRota)
        {
            return objPersistence.ExecuteSqlQueryCount(Sql(dataIni, dataFim, idPedido, idCliente, nomeCliente, idRota, idLoja, true, false),
                GetParams(dataIni, dataFim, nomeCliente));
        }

        #endregion
    }
}
