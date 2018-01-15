using System;
using System.Collections.Generic;
using Glass.Data.RelModel;
using GDA;
using Glass.Data.DAL;
using Glass.Data.Model;
using Glass.Configuracoes;

namespace Glass.Data.RelDAL
{
    public sealed class VendasDAO : BaseDAO<Vendas, VendasDAO>
    {
        //private VendasDAO() { }

        private string SqlBase(uint idCliente, string nomeCliente, uint idRota, string idsRotas, bool revenda, uint idComissionado, string nomeComissionado,
            int mesInicio, int anoInicio, int mesFim, int anoFim, string tipoMedia, int tipoVendas, string idsFuncionario,
            string nomeFuncionario, string idsFuncAssociaCliente, int situacaoCliente, int tipoFiscalCliente, uint idLoja, bool lojaCliente, string tipoCliente)
        {
            var criterio = "";
            var isLiberarPedido = PedidoConfig.LiberarPedido;
            var total = PedidoDAO.Instance.SqlCampoTotalLiberacao(isLiberarPedido, "valor", "p", "pe", "ap", "plp", false);

            var campoData = (isLiberarPedido ? "lp.DataLiberacao, " : "");

            var sqlIdsPedido = @"
                Select distinct Cast(p.idPedido as char) From pedido p
                    Left Join cliente c On (p.idCli=c.id_Cli)
                    Left Join produtos_liberar_pedido plp on (p.idPedido=plp.idPedido)
                    Left Join liberarpedido lp on (plp.idLiberarPedido=lp.idLiberarPedido)
                Where" +
                      (!string.IsNullOrEmpty(idsFuncAssociaCliente) && idsFuncAssociaCliente != int.MaxValue.ToString()
                          ? " c.idFunc IN (" + idsFuncAssociaCliente + ") And "
                          : (!string.IsNullOrEmpty(idsFuncAssociaCliente) ? " c.idFunc is null And " : "")) + @"
                    p.Situacao In (" + (isLiberarPedido ? (int)Pedido.SituacaoPedido.LiberadoParcialmente + "," : "")
                      + (int)Pedido.SituacaoPedido.Confirmado + @")
                    And p.TipoPedido In (" + (int)Pedido.TipoPedidoEnum.Venda + "," +
                      (int)Pedido.TipoPedidoEnum.Revenda + "," +
                      (int)Pedido.TipoPedidoEnum.MaoDeObraEspecial + "," +
                      (int)Pedido.TipoPedidoEnum.MaoDeObra + @")
                    And p.TipoVenda In (" + (int)Pedido.TipoVendaPedido.AVista + "," +
                      (int)Pedido.TipoVendaPedido.APrazo + "," +
                      (int)Pedido.TipoVendaPedido.Obra + @")" +
                      (isLiberarPedido ? " And lp.situacao =" + (int)LiberarPedido.SituacaoLiberarPedido.Liberado : "") +
                      @"
                    And " + (tipoVendas == 0 ? "p.IdCli" : "p.IdComissionado") +
                      (idCliente > 0 ? "=" + idCliente + " " : " Is Not Null ") +
                      (revenda ? "And c.Revenda=1" : "") + @" and ((Month(Coalesce(" + campoData +
                      "p.DataPedido, p.DataCad))>=" + mesInicio +
                      " And Year(Coalesce(" + campoData + "p.DataPedido, p.DataCad))=" + anoInicio + @")
                    Or Year(Coalesce(" + campoData + "p.DataPedido, p.DataCad))>" + anoInicio + @")
                    And (( Month(Coalesce(" + campoData + "p.DataPedido, p.DataCad))<=" + mesFim + @"
                    and Year(Coalesce(" + campoData + "p.DataPedido, p.DataCad))=" + anoFim + @")
                    or Year(Coalesce(" + campoData + "p.DataPedido, p.DataCad))<" + anoFim + ")";

            // Recupera os ids dos pedidos para que o método principal fique mais leve
            var idsPedido = string.Join(",", ExecuteMultipleScalar<string>(sqlIdsPedido).ToArray());

            if (string.IsNullOrEmpty(idsPedido))
                idsPedido = "0";

            var sql =
                string.Format(@"
                    Select IdCliente, NomeCliente, ValorMediaIni, ValorMediaFim, IdComissionado, NomeComissionado, IdFuncionario, NomeFuncionario, IdFuncPed, NomeFuncPed, 
                        MesVenda, AnoVenda, Sum(Valor) as Valor, Criterio, idTipoCLiente, idLoja, SUM(totM2) as totM2, SUM(totalItens) as totalItens
                    From (
                        Select IdCliente, NomeCliente, ValorMediaIni, valorMediaFim, idComissionado, IdFuncionario, idFuncPed, NomeFuncionario, NomeFuncPed, 
                            NomeComissionado, MesVenda, AnoVenda, Sum(Valor) as Valor, Criterio, SituacaoCliente, TipoFiscalCliente, IdTipoCliente, IdLoja, 
                            Sum(totM2) as totM2, Sum(totalItens) as totalItens
                        From (
                            Select p.IdPedido, p.IdCli As IdCliente, COALESCE(c.NomeFantasia, c.Nome) As NomeCliente, c.valorMediaIni, c.valorMediaFim, p.idComissionado, c.IdFunc As IdFuncionario, p.idFunc as idFuncPed,
                            f.Nome As NomeFuncionario, fped.Nome As NomeFuncPed, cm.Nome As NomeComissionado, Month(Coalesce({0} p.DataPedido, p.DataCad)) As MesVenda, 
                            Year(Coalesce({0} p.DataPedido, p.DataCad)) As AnoVenda, {1}, '$$$' As Criterio, c.situacao as situacaoCliente,
                            c.tipoFiscal as tipoFiscalCliente, c.idTipoCliente, {2} as idLoja,
                            pp.totM as totM2,
                            If(prod.IdGrupoProd={3}, pp.qtde, 0) as totalItens
                        From pedido p
                            Left Join produtos_pedido pp On (pp.IdPedido=p.IdPedido)
                            Left Join produto prod On (pp.IdProd=prod.IdProd) 
                            {4}
                            Left Join cliente c On (p.IdCli=c.Id_Cli)
                            Left Join funcionario f On (c.IdFunc=f.IdFunc) 
                            Left Join funcionario fped On (p.IdFunc=fped.IdFunc) 
                            Left Join comissionado cm On (p.IdComissionado=cm.IdComissionado)
                            Where {5} And p.idPedido In ({6}) {7} And coalesce(pp.invisivelFluxo, 0)=0 group by pp.idProdPed
                        ) as tbl Group By idpedido
                    ) As vendas1 
                    Where 1", campoData, total, (lojaCliente ? "c.id_loja" : "p.idLoja"), (int)NomeGrupoProd.Vidro,
                    (isLiberarPedido ? @"
                        Left Join pedido_espelho pe On (p.idPedido=pe.idPedido)
                        Left Join ambiente_pedido ap On (pp.IdAmbientePedido=ap.IdAmbientePedido)
                        Left Join produtos_liberar_pedido plp On (pp.IdProdPed=plp.IdProdPed)
                        Left Join liberarpedido lp On (plp.IdLiberarPedido = lp.IdLiberarPedido)" : string.Empty),
                    isLiberarPedido ? string.Format("lp.Situacao={0}", (int)LiberarPedido.SituacaoLiberarPedido.Liberado) : "1",
                    idsPedido,
                    string.Format(
                        @"AND ((MONTH(COALESCE({0}p.DataPedido, p.DataCad)) >= {1}
                        AND YEAR(COALESCE({0}p.DataPedido, p.DataCad)) = {2})
                        OR YEAR(COALESCE({0}p.DataPedido, p.DataCad)) > {2})
                        AND ((MONTH(COALESCE({0}p.DataPedido, p.DataCad)) <= {3}
                        AND YEAR(COALESCE({0}p.DataPedido, p.DataCad)) = {4})
                        OR YEAR(COALESCE({0}p.DataPedido, p.DataCad)) < {4})",
                        campoData, mesInicio, anoInicio, mesFim, anoFim));

            criterio += "Mês/ano início: " + mesInicio + "/" + anoInicio + "    Mês/ano fim: " + mesFim + "/" + anoFim + "    ";

            if (tipoVendas == 0)
            {
                if (idCliente > 0)
                {
                    sql += " and IdCliente=" + idCliente;
                    criterio += "Cliente: " + idCliente + " - " + ClienteDAO.Instance.GetNome(idCliente) + "    ";
                }
                else if (!String.IsNullOrEmpty(nomeCliente))
                {
                    string ids = ClienteDAO.Instance.GetIds(null, nomeCliente, null, 0, null, null, null, null, 0);
                    sql += " And idCliente in (" + ids + ")";
                    criterio += "Cliente: " + nomeCliente + "    ";
                }

                if (idRota > 0)
                {
                    sql += " And idCliente in (Select idCliente from rota_cliente Where idRota = " + idRota + ")";
                    criterio += "Rota: " + RotaDAO.Instance.ObtemDescrRota(idRota) + "    ";
                }
                else if (!string.IsNullOrWhiteSpace(idsRotas) && idsRotas != "0")
                {
                    sql += " And idCliente in (Select idCliente from rota_cliente Where idRota IN (" + idsRotas + "))";
                    criterio += "Rota(s): " + RotaDAO.Instance.ObtemDescrRotas(idsRotas) + "    ";
                }

                if (!String.IsNullOrEmpty(idsFuncionario))
                {
                    sql += " And IdFuncPed in (?idsFuncionario)";
                    var criterioFunc = String.Empty;

                    foreach (var id in idsFuncionario.Split(','))
                        criterioFunc += id + " - " + FuncionarioDAO.Instance.GetNome(Glass.Conversoes.StrParaUint(id)) + "    ";

                    criterio += "Funcionario: " + criterioFunc;
                }
                else if (!String.IsNullOrEmpty(nomeFuncionario))
                {
                    sql += " and NomeFuncPed LIKE ?nomeFuncionario";
                    criterio += "Funcionario: " + nomeFuncionario + "    ";
                }

                if (!String.IsNullOrEmpty(tipoMedia))
                {
                    if (tipoMedia.Contains("1"))
                        criterio += "Acima da média de compra, ";

                    if (tipoMedia.Contains("2"))
                        criterio += "Dentro da média de compra, ";

                    if (tipoMedia.Contains("3"))
                        criterio += "Abaixo da média de compra, ";

                    criterio += "   ";
                }

                if (situacaoCliente > 0)
                {
                    sql += " and situacaoCliente=" + situacaoCliente;
                    Cliente cli = new Cliente();
                    cli.Situacao = situacaoCliente;
                    criterio += "Situação Cliente: " + cli.DescrSituacao + "    ";
                }

                if (tipoFiscalCliente > 0)
                {
                    sql += " and tipoFiscalCliente=" + tipoFiscalCliente;
                    criterio += "Tipo Fiscal Cliente: " + (tipoFiscalCliente == 1 ? "Consumidor final" : "Revenda") + "    ";
                }

                if (idLoja > 0)
                {
                    sql += " AND idLoja=" + idLoja;
                    criterio += "Loja: " + LojaDAO.Instance.GetNome(idLoja) + "   ";
                }

                if (!string.IsNullOrEmpty(tipoCliente))
                {
                    sql += " AND idTipoCliente IN (" + tipoCliente + ")";
                    criterio += "Tipo do Cliente: " + TipoClienteDAO.Instance.GetNomes(tipoCliente) + "     ";
                }
            }
            else
            {
                if (idComissionado > 0)
                {
                    sql += " and IdComissionado=" + idComissionado;
                    criterio += "Comissionado: " + idComissionado + " - " + ComissionadoDAO.Instance.GetNome(idComissionado) + "    ";
                }
                else if (!String.IsNullOrEmpty(nomeComissionado))
                {
                    sql += " and NomeComissionado LIKE ?nomeComissionado";
                    criterio += "Comissionado: " + nomeComissionado + "    ";
                }
            }

            return sql.Replace("$$$", criterio.Trim()) + " group by " + (tipoVendas == 0 ? "IdCliente" : "IdComissionado") +
                ", MesVenda, AnoVenda order by " + (tipoVendas == 0 ? "IdCliente" : "IdComissionado") + ", MesVenda, AnoVenda";
        }

        private string Sql(uint idCliente, string nomeCliente, uint idRota, string idsRotas, bool revenda, uint idComissionado, string nomeComissionado, int mesInicio,
            int anoInicio, int mesFim, int anoFim, int ordenar, string tipoMedia, int tipoVendas, string idsFuncionario, string nomeFuncionario, string idsFuncAssociaCliente, decimal valorMinimo,
            decimal valorMaximo, int situacaoCliente, int tipoFiscalCliente, uint idLoja, bool lojaCliente, string tipoCliente, bool selecionar)
        {
            string sql = @"Select * From (select IdCliente, NomeCliente, IdComissionado, NomeComissionado, IdFuncionario, NomeFuncionario, valorMediaIni, valorMediaFim,
                    Group_Concat(Concat(Cast(MesVenda As Char), Concat('/', Cast(AnoVenda As Char)))) As MesesVenda, Cast(Group_Concat(Valor) As Char) As ValoresVenda,
                Cast(Sum(Valor) As Decimal(12,2)) As Total, Criterio, Sum(totM2) as totM2, SUM(totalItens) as totalItens
                From (" + SqlBase(idCliente, nomeCliente, idRota, idsRotas, revenda, idComissionado, nomeComissionado, mesInicio, anoInicio, mesFim, anoFim, tipoMedia,
                        tipoVendas, idsFuncionario, nomeFuncionario, idsFuncAssociaCliente, situacaoCliente, tipoFiscalCliente, idLoja, lojaCliente, tipoCliente) +
                ") As vendas ";

            sql += " Group By " + (tipoVendas == 0 ? "IdCliente" : "IdComissionado");

            if (ordenar > 0)
            {
                sql += " Order By ";
                switch (ordenar)
                {
                    case 1:
                        sql += "Total ASC";
                        break;
                    case 2:
                        sql += "Total DESC";
                        break;
                    case 3:
                        sql += tipoVendas == 0 ? "NomeCliente ASC" : "NomeComissionado ASC";
                        break;
                    case 4:
                        sql += tipoVendas == 0 ? "NomeCliente DESC" : "NomeComissionado DESC";
                        break;
                    default:
                        sql += tipoVendas == 0 ? "IdCliente" : "IdComissionado";
                        break;
                }
            }

            sql += ") As temp Where 1 ";

            if (valorMinimo > 0 || valorMaximo > 0)
            {
                sql += " having 1";
            }

            if (valorMinimo > 0)
            {
                sql += " And Total >= Cast('" + valorMinimo + "' As Decimal(12,2))";
            }

            if (valorMaximo > 0)
            {
                sql += " And Total <= Cast('" + valorMaximo + "' As Decimal(12,2))";
            }

            if (!String.IsNullOrEmpty(tipoMedia))
            {
                string sqlTipoMedia = " And (";

                if (tipoMedia.Contains("1"))
                    sqlTipoMedia += "(Total>valorMediaFim And valorMediaFim>0)";

                if (tipoMedia.Contains("2"))
                    sqlTipoMedia += (tipoMedia.Contains("1") ? " Or " : "") + "Total Between valorMediaIni And valorMediaFim";

                if (tipoMedia.Contains("3"))
                    sqlTipoMedia += (tipoMedia.Contains("1") || tipoMedia.Contains("2") ? " Or " : "") + "Total<valorMediaIni";

                sql += sqlTipoMedia + ")";
            }

            return selecionar ? sql : "Select count(*) From (" + sql + ") As temp1";
        }

        private GDAParameter[] GetParams(string nomeCliente, string nomeComissionado, string idsFuncionario,
            string idsFuncionarioAssocCliente, string nomeFuncionario)
        {
            List<GDAParameter> retorno = new List<GDAParameter>();
            
            if (!String.IsNullOrEmpty(nomeCliente))
                retorno.Add(new GDAParameter("?nomeCliente", "%" + nomeCliente + "%"));

            if (!String.IsNullOrEmpty(nomeComissionado))
                retorno.Add(new GDAParameter("?nomeComissionado", "%" + nomeComissionado + "%"));

            if (!String.IsNullOrEmpty(idsFuncionario))
                retorno.Add(new GDAParameter("?idsFuncionario", idsFuncionario));

            if (!String.IsNullOrEmpty(idsFuncionarioAssocCliente))
                retorno.Add(new GDAParameter("?idsFuncionarioAssocCliente", idsFuncionarioAssocCliente));

            if (!String.IsNullOrEmpty(nomeFuncionario))
                retorno.Add(new GDAParameter("?nomeFuncionario", "%" + nomeFuncionario + "%"));

            return retorno.ToArray();
        }

        public IList<Vendas> GetList(uint idCliente, string nomeCliente, string idsRota, bool revenda, uint idComissionado, string nomeComissionado, int mesInicio,
            int anoInicio, int mesFim, int anoFim, int ordenar, string tipoMedia, int tipoVendas, string idsFuncionario, string nomeFuncionario, string idsFuncAssociaCliente,
            decimal valorMinimo, decimal valorMaximo, int situacaoCliente, int tipoFiscalCliente, uint idLoja, bool lojaCliente, string tipoCliente)
        {
            return LoadDataWithSortExpression(Sql(idCliente, nomeCliente, 0, idsRota, revenda, idComissionado, nomeComissionado, mesInicio,
                anoInicio, mesFim, anoFim, ordenar, tipoMedia, tipoVendas, idsFuncionario, nomeFuncionario, idsFuncAssociaCliente, valorMinimo, valorMaximo, 
                situacaoCliente, tipoFiscalCliente, idLoja, lojaCliente, tipoCliente, true), null, 0, int.MaxValue,
                GetParams(nomeCliente, nomeComissionado, idsFuncionario, idsFuncAssociaCliente, nomeFuncionario));
        }

        public IList<Vendas> GetList(uint idCliente, string nomeCliente, string idsRotas, bool revenda, int mesInicio, int anoInicio,
            int mesFim, int anoFim, int ordenar, string tipoMedia, string idsFuncionario, string nomeFuncionario, string idsFuncAssociaCliente, decimal valorMinimo,
            decimal valorMaximo, int situacaoCliente, int tipoFiscalCliente, uint idLoja, bool lojaCliente, string tipoCliente,
            string sortExpression, int startRow, int pageSize)
        {
            return LoadDataWithSortExpression(Sql(idCliente, nomeCliente, 0, idsRotas, revenda, 0, null, mesInicio, anoInicio, mesFim, anoFim,
                ordenar, tipoMedia, 0, idsFuncionario, nomeFuncionario, idsFuncAssociaCliente, valorMinimo, valorMaximo, situacaoCliente, tipoFiscalCliente,
                idLoja, lojaCliente, tipoCliente, true), sortExpression, startRow, pageSize,
                GetParams(nomeCliente, null, idsFuncionario, idsFuncAssociaCliente, nomeFuncionario));
        }

        public int GetListCount(uint idCliente, string nomeCliente, string idsRotas, bool revenda, int mesInicio, int anoInicio,
            int mesFim, int anoFim, int ordenar, string tipoMedia, string idsFuncionario, string nomeFuncionario, string idsFuncAssociaCliente, decimal valorMinimo,
            decimal valorMaximo, int situacaoCliente, int tipoFiscalCliente, uint idLoja, bool lojaCliente, string tipoCliente)
        {
            return objPersistence.ExecuteSqlQueryCount(Sql(idCliente, nomeCliente, 0, idsRotas, revenda, 0, null, mesInicio, anoInicio, mesFim, anoFim,
                ordenar, tipoMedia, 0, idsFuncionario, nomeFuncionario, idsFuncAssociaCliente, valorMinimo, valorMaximo, situacaoCliente, tipoFiscalCliente,
                idLoja, lojaCliente, tipoCliente, false), GetParams(nomeCliente, null, idsFuncionario, idsFuncAssociaCliente, nomeFuncionario));
        }

        public IList<Vendas> GetListComissionado(uint idCliente, string nomeCliente, uint idComissionado, string nomeComissionado, int mesInicio,
            int anoInicio, int mesFim, int anoFim, int ordenar, int tipoVendas, string idsFuncionario, string nomeFuncionario,
            decimal valorMinimo, decimal valorMaximo, string sortExpression, int startRow, int pageSize)
        {
            return LoadDataWithSortExpression(Sql(idCliente, nomeCliente, 0, null, false, idComissionado, nomeComissionado, mesInicio,
                anoInicio, mesFim, anoFim, ordenar, null, tipoVendas, idsFuncionario, nomeFuncionario, null, valorMinimo, valorMaximo, 0, 0, 0, false, null, true),
                sortExpression, startRow, pageSize, GetParams(nomeCliente, nomeComissionado, idsFuncionario, null, nomeFuncionario));
        }

        public int GetListCountComissionado(uint idCliente, string nomeCliente, uint idComissionado, string nomeComissionado, int mesInicio,
            int anoInicio, int mesFim, int anoFim, int ordenar, int tipoVendas, string idsFuncionario, string nomeFuncionario,
            decimal valorMinimo, decimal valorMaximo)
        {
            return objPersistence.ExecuteSqlQueryCount(Sql(idCliente, nomeCliente, 0, null, false, idComissionado, nomeComissionado, mesInicio,
                anoInicio, mesFim, anoFim, ordenar, null, tipoVendas, idsFuncionario, nomeFuncionario, null, valorMinimo, valorMaximo, 0, 0, 0, false, null, false),
                GetParams(nomeCliente, nomeComissionado, idsFuncionario, null, nomeFuncionario));
        }

        public string[] GetMesesVenda(uint idCliente, string nomeCliente, string idsRota, bool revenda, uint idComissionado, string nomeComissionado,
            int mesInicio, int anoInicio, int mesFim, int anoFim, string tipoMedia, int tipoVendas, string idsFuncionario, string nomeFuncionario, 
            uint idLoja, bool lojaCliente, string tipoCliente, int situacaoCliente)
        {
            string sql = "select group_concat(concat(cast(MesVenda as char), concat('/', cast(AnoVenda as char)))) from (select distinct MesVenda, AnoVenda from (" +
                SqlBase(idCliente, nomeCliente, 0, idsRota, revenda, idComissionado, nomeComissionado, mesInicio, anoInicio, mesFim, anoFim, tipoMedia,
                tipoVendas, idsFuncionario, nomeFuncionario, null, situacaoCliente, 0, idLoja, lojaCliente, tipoCliente) + ") as temp1 order by AnoVenda, MesVenda) as temp";

            object retorno = objPersistence.ExecuteScalar(sql, GetParams(nomeCliente, nomeComissionado, idsFuncionario, null, nomeFuncionario));
            if (retorno == null || retorno.ToString() == String.Empty)
                return new string[0];

            List<string> lista = new List<string>();
            foreach (string s in retorno.ToString().Split(','))
                if (!lista.Contains(s))
                    lista.Add(s);

            return lista.ToArray();
        }
    }
}