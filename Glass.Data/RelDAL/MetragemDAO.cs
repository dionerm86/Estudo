using System;
using System.Collections.Generic;
using Glass.Data.RelModel;
using Glass.Data.DAL;
using GDA;
using Glass.Data.Model;
using Glass.Data.Helper;

namespace Glass.Data.RelDAL
{
    public sealed class MetragemDAO : BaseDAO<Metragem, MetragemDAO>
    {
        //private MetragemDAO() { }

        private string Sql(uint idPedido, uint idImpressao, uint idFunc, string codPedCli, uint idCliente, string nomeCliente, string dataIni, 
            string dataFim, string dataIniEnt, string dataFimEnt, int situacao, uint idSetor, bool setoresPosteriores, string idsRotas,
            uint idTurno, bool selecionar)
        {
            string campoNomeCliente = "Coalesce(cli.nome,cli.nome)";
    
            string criterio = "";
            string sql = @"
                select pp.idPedido, substring(ppp.numEtiqueta, instr(ppp.numEtiqueta, '-') + 1) as NumPeca, 
                    COALESCE(cv.Descricao, 'N/D') as Cor, p.Espessura, pp.Altura, pp.Largura, 
                    
                    /*Caso o pedido seja mão de obra o m2 da peça deve ser considerado*/
                    ROUND(if(ped.tipoPedido=3, (
                    (((50 - If(Mod(a.altura, 50) > 0, Mod(a.altura, 50), 50)) +
                    a.altura) * ((50 - If(Mod(a.largura, 50) > 0, Mod(a.largura, 50), 50)) + a.largura)) / 1000000)             
                    * a.qtde, pp.TotM2Calc)/(pp.qtde*if(ped.tipoPedido=3, a.qtde, 1)), 4) as TotM2,

                    pp.Obs, cli.id_Cli as idCliente, " + campoNomeCliente + @" as NomeCliente, '$$$' as Criterio
                from produto_pedido_producao ppp
                    inner Join produtos_pedido_espelho pp On (ppp.idProdPed=pp.idProdPed)" +
                    ((!String.IsNullOrEmpty(dataIni) || !String.IsNullOrEmpty(dataFim)) &&
                    situacao != (int)ProdutoPedidoProducao.SituacaoEnum.Perda && idSetor > 0 ?
                    " INNER JOIN leitura_producao lp ON (ppp.IdProdPedProducao=lp.IdProdPedProducao)" : "") +
                    @"
                    left join ambiente_pedido_espelho a On (pp.idAmbientePedido=a.idAmbientePedido) 
                    inner Join pedido ped On (pp.idPedido=ped.idPedido)
                    Inner Join cliente cli On (ped.idCli=cli.id_Cli)
                    inner Join produto p On (pp.idProd=p.idProd)
                    Left Join cor_vidro cv on (p.idCorVidro=cv.idCorVidro)
                    Left Join etiqueta_aplicacao apl On (pp.idAplicacao=apl.idAplicacao)
                    Left Join etiqueta_processo prc On (pp.idProcesso=prc.idProcesso)
                    Left Join leitura_producao lp1 On (ppp.idProdPedProducao=lp1.idProdPedProducao)
                where ped.situacao<>" + (int)Pedido.SituacaoPedido.Cancelado;

            ProdutoPedidoProducao temp = new ProdutoPedidoProducao();

            if (idPedido > 0)
            {
                sql += " And (ped.idPedido=" + idPedido;

                if (Glass.Configuracoes.ProducaoConfig.TipoControleReposicao == DataSources.TipoReposicaoEnum.Pedido && PedidoDAO.Instance.IsPedidoReposto(null, idPedido))
                    sql += " Or ped.IdPedidoAnterior=" + idPedido;

                if (PedidoDAO.Instance.IsPedidoExpedicaoBox(null, idPedido))
                    sql += " Or ppp.idPedidoExpedicao=" + idPedido;

                sql += ")";
                criterio += "Pedido: " + idPedido + "    ";
            }

            if (!String.IsNullOrEmpty(codPedCli))
            {
                sql += " And ped.codCliente=?codCliente";
                criterio += "Pedido Cliente: " + codPedCli + "    ";
            }

            if (idImpressao > 0)
            {
                sql += " And ppp.idProdPed In (Select idProdPed From produto_impressao Where idImpressao=" + idImpressao + ")";
                criterio += "Num. Impressão: " + idImpressao + "    ";
            }

            if (idFunc > 0)
            {
                sql += " And ppp.idProdPedProducao in (select idProdPedProducao from leitura_producao where idFuncLeitura=" + idFunc + ")";
                criterio += "Funcionário: " + FuncionarioDAO.Instance.GetNome(idFunc) + "    ";
            }

            if (idCliente > 0)
            {
                sql += " And ped.idCli=" + idCliente;
                criterio += "Cliente: " + idCliente + " - " + ClienteDAO.Instance.GetNome(idCliente) + "    ";
            }
            else if (!String.IsNullOrEmpty(nomeCliente))
            {
                string ids = ClienteDAO.Instance.GetIds(null, nomeCliente, null, 0, null, null, null, null, 0);
                sql += " And cli.id_Cli in (" + ids + ")";
                criterio += "Cliente: " + nomeCliente;
            }

            if (situacao > 0)
            {
                sql += " And ppp.Situacao=" + situacao;
                temp.Situacao = situacao;
                criterio += "Situação: " + temp.DescrSituacao + "    ";
            }

            if (idSetor > 0)
            {
                if (!setoresPosteriores)
                    sql += " And ppp.idSetor=" + idSetor;
                else
                {
                    if (setoresPosteriores)
                        sql += " And " + Utils.ObtemSetor(idSetor).NumeroSequencia +
                            " <= all (Select numSeq From setor Where idSetor=ppp.idSetor) ";
                }

                criterio += "Setor: " + Utils.ObtemSetor(idSetor).Descricao + (setoresPosteriores ? " (inclui produtos que já passaram por este setor)" : "") + "    ";
            }

            if (!String.IsNullOrEmpty(dataIni))
            {
                if (situacao == (int)ProdutoPedidoProducao.SituacaoEnum.Perda)
                {
                    sql += " And ppp.dataPerda>=?dataIni";
                    criterio += "Data perda início: " + dataIni + "    ";
                }
                else if (idSetor > 0)
                {
                    sql += " AND lp.DataLeitura>=?dataIni AND lp.IdSetor=" + idSetor;
                    criterio += "Data " + Utils.ObtemSetor(idSetor).Descricao + " início: " + dataIni + "    ";
                }
            }

            if (!String.IsNullOrEmpty(dataFim))
            {
                if (situacao == (int)ProdutoPedidoProducao.SituacaoEnum.Perda)
                {
                    sql += " And ppp.dataPerda<=?dataFim";
                    criterio += "Data perda término: " + dataFim + "    ";
                }
                else if (idSetor > 0)
                {
                    sql += " AND lp.DataLeitura<=?dataFim AND lp.IdSetor=" + idSetor;
                    criterio += "Data " + Utils.ObtemSetor(idSetor).Descricao + " término: " + dataFim + "    ";
                }
            }

            if (!String.IsNullOrEmpty(dataIniEnt))
            {
                sql += " And ped.dataEntrega>=?dataIniEnt";
                criterio += "Data Entrega início: " + dataIniEnt + "    ";
            }

            if (!String.IsNullOrEmpty(dataFimEnt))
            {
                sql += " And ped.dataEntrega<=?dataFimEnt";
                criterio += "Data Entrega término: " + dataFimEnt + "    ";
            }

            if (!String.IsNullOrEmpty(idsRotas))
            {
                sql += " And cli.Id_Cli in (select * from (select idCliente from rota_cliente where idRota in (" + idsRotas + ")) as temp)";
                criterio += "Rota(s): " + RotaDAO.Instance.ObtemDescrRotas(idsRotas) + "    ";
            }

            if (idTurno > 0)
            {
                string inicio = TurnoDAO.Instance.ObtemValorCampo<string>("inicio", "idTurno=" + idTurno);
                string termino = TurnoDAO.Instance.ObtemValorCampo<string>("termino", "idTurno=" + idTurno);
                string descricao = TurnoDAO.Instance.ObtemValorCampo<string>("descricao", "idTurno=" + idTurno);

                if (TimeSpan.Parse(inicio) <= TimeSpan.Parse(termino))
                    sql += " and lp1.idSetor=ppp.idSetor and lp1.DataLeitura>=cast(concat(date_format(lp1.DataLeitura, '%Y-%m-%d'), ' " + inicio + "') as datetime) and lp1.DataLeitura<=cast(concat(date_format(lp1.DataLeitura, '%Y-%m-%d'), ' " + termino + "') as datetime)";
                else
                    sql += " and lp1.idSetor=ppp.idSetor and (lp1.DataLeitura>=cast(concat(date_format(lp1.DataLeitura, '%Y-%m-%d'), ' " + inicio + "') as datetime) or lp1.DataLeitura<cast(concat(date_format(lp1.DataLeitura, '%Y-%m-%d'), ' " + termino + "') as datetime))";

                criterio += "Turno: " + descricao + "    ";
            }

            sql = sql.Replace("$$$", criterio.Trim());
            sql = selecionar ? sql + " group by ppp.idProdPedProducao" : "select count(*) from (" + sql + ") as temp";
            return sql;
        }

        private GDAParameter[] GetParams(string dataIni, string dataFim, string dataIniEnt, string dataFimEnt, string nomeCliente, string codPedCli)
        {
            List<GDAParameter> lstParam = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(dataIni))
                lstParam.Add(new GDAParameter("?dataIni", DateTime.Parse(dataIni + " 00:00")));

            if (!String.IsNullOrEmpty(dataFim))
                lstParam.Add(new GDAParameter("?dataFim", DateTime.Parse(dataFim + " 23:59")));

            if (!String.IsNullOrEmpty(dataIniEnt))
                lstParam.Add(new GDAParameter("?dataIniEnt", DateTime.Parse(dataIniEnt + " 00:00")));

            if (!String.IsNullOrEmpty(dataFimEnt))
                lstParam.Add(new GDAParameter("?dataFimEnt", DateTime.Parse(dataFimEnt + " 23:59")));

            if (!String.IsNullOrEmpty(nomeCliente))
                lstParam.Add(new GDAParameter("?nomeCliente", "%" + nomeCliente + "%"));

            if (!String.IsNullOrEmpty(codPedCli))
                lstParam.Add(new GDAParameter("?codCliente", codPedCli));

            return lstParam.Count > 0 ? lstParam.ToArray() : null;
        }

        public IList<Metragem> GetMetragens(uint idPedido, uint idImpressao, uint idFunc, string codPedCli, uint idCliente, string nomeCliente, string dataIni, string dataFim,
            string dataIniEnt, string dataFimEnt, int situacao, uint idSetor, bool setoresPosteriores, string idsRotas, uint idTurno,
            string sortExpression, int startRow, int pageSize)
        {
            return LoadDataWithSortExpression(Sql(idPedido, idImpressao, idFunc, codPedCli, idCliente, nomeCliente, dataIni, dataFim, dataIniEnt, 
                dataFimEnt, situacao, idSetor, setoresPosteriores, idsRotas, idTurno, true), sortExpression, startRow, pageSize, GetParams(dataIni, dataFim, 
                dataIniEnt, dataFimEnt, nomeCliente, codPedCli));
        }

        public int GetMetragensCount(uint idPedido, uint idImpressao, uint idFunc, string codPedCli, uint idCliente, string nomeCliente, string dataIni, string dataFim,
            string dataIniEnt, string dataFimEnt, int situacao, uint idSetor, bool setoresPosteriores, string idsRotas, uint idTurno)
        {
            return objPersistence.ExecuteSqlQueryCount(Sql(idPedido, idImpressao, idFunc, codPedCli, idCliente, nomeCliente, dataIni, dataFim, dataIniEnt, 
                dataFimEnt, situacao, idSetor, setoresPosteriores, idsRotas, idTurno, false), GetParams(dataIni, dataFim, dataIniEnt, dataFimEnt, nomeCliente, codPedCli));
        }

        public IList<Metragem> GetForRpt(uint idPedido, uint idImpressao, uint idFunc, string codPedCli, uint idCliente, string nomeCliente, string dataIni, string dataFim,
            string dataIniEnt, string dataFimEnt, int situacao, uint idSetor, bool setoresPosteriores, string idsRotas, uint idTurno)
        {
            return objPersistence.LoadData(Sql(idPedido, idImpressao, idFunc, codPedCli, idCliente, nomeCliente, dataIni, dataFim, dataIniEnt, dataFimEnt, 
                situacao, idSetor, setoresPosteriores, idsRotas, idTurno, true), GetParams(dataIni, dataFim, dataIniEnt, dataFimEnt, nomeCliente, codPedCli)).ToList();
        }
    }
}
