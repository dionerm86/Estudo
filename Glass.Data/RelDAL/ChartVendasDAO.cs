﻿using System;
using System.Collections.Generic;
using Glass.Data.RelModel;
using Glass.Data.DAL;
using Glass.Data.Model;
using Glass.Data.Helper;
using GDA;
using Glass.Configuracoes;
using System.Linq;

namespace Glass.Data.RelDAL
{
    public sealed class ChartVendasDAO : BaseDAO<ChartVendas, ChartVendasDAO>
    {
        public Dictionary<uint, List<ChartVendas>> GetForRpt(uint idLoja, uint idVendedor, int tipoFunc, uint idCliente, string nomeCliente, string tipoPedido, uint idRota, string dataIni,
            string dataFim, int agrupar, out string criterio)
        {
            var ids = new List<int>();
            var tipoAgrupar = "nenhum";
            criterio = string.Empty;

            switch (agrupar)
            {
                case 0: //nenhum
                    tipoAgrupar = "nenhum";
                    ids.Add(1);
                    break;

                case 1: //loja
                    tipoAgrupar = "loja";
                    var lojas = idLoja == 0 ? LojaDAO.Instance.GetAll() : new Loja[] { LojaDAO.Instance.GetElementByPrimaryKey(idLoja) };

                    foreach (Loja l in lojas)
                        ids.Add((int)l.IdLoja);

                    break;

                case 2: //emissor
                    tipoAgrupar = "emissor";
                    Funcionario[] funcs;

                    if (idVendedor == 0)
                    {
                        List<Funcionario> list = new List<Funcionario>(FuncionarioDAO.Instance.GetVendedoresComVendas(idLoja, tipoFunc == 1, dataIni, dataFim));
                        list.RemoveAt(0);
                        funcs = list.ToArray();
                    }
                    else
                        funcs = new Funcionario[] { FuncionarioDAO.Instance.GetElementByPrimaryKey(idVendedor) };

                    foreach (Funcionario f in funcs)
                        ids.Add(f.IdFunc);

                    break;

                case 3: // cliente
                    tipoAgrupar = "cliente";
                    Cliente[] cli;

                    if (idCliente == 0)
                        cli = ClienteDAO.Instance.GetClientesVendas(idLoja, dataIni, dataFim);
                    else
                        cli = new Cliente[] { ClienteDAO.Instance.GetElementByPrimaryKey(idCliente) };

                    foreach (Cliente c in cli)
                        ids.Add(c.IdCli);

                    break;

                case 4: // tipoPedido
                    tipoAgrupar = "tipoPedido";
                    foreach(string tp in tipoPedido.Split(','))
                        ids.Add(Glass.Conversoes.StrParaInt(tp));

                    break;
                case 5: // rota
                    tipoAgrupar = "rota";

                    Rota[] rotas;
                    if (idRota == 0)
                        rotas = RotaDAO.Instance.ObterRotas().ToArray();
                    else
                        rotas = new[] { RotaDAO.Instance.GetElement((uint)idRota) };

                    foreach (var r in rotas)
                        ids.Add(r.IdRota);

                    break;
                default:
                    break;
            }

            return GetVendasForChart(idLoja, tipoFunc, idVendedor, idCliente, nomeCliente, idRota, dataIni, dataFim, tipoPedido, agrupar, tipoAgrupar, ids.Select(x => (uint)x).ToList(), out criterio);
        }
        
        public Dictionary<uint, List<ChartVendas>> GetVendasForChart(uint idLoja, int tipoFunc, uint idVendedor, uint idCliente, string nomeCliente, 
            uint idRota, string dataIni, string dataFim, string tipoPedido, int agrupar, string tipoAgrupar, List<uint> ids, out string criterio)
        {
            DateTime periodoIni = DateTime.Parse(dataIni);
            DateTime periodoFim = DateTime.Parse(dataFim).AddDays(1);
            Dictionary<uint,List<ChartVendas>> dictVendas = new Dictionary<uint,List<ChartVendas>>();
            criterio = string.Empty;

            foreach (uint u in ids)
            {
                dictVendas.Add(u, new List<ChartVendas>());
            }

            int count = 1;
            while (periodoIni < periodoFim)
            {
                foreach (uint u in ids)
                {
                    switch (tipoAgrupar)
                    {
                        case "loja":
                            idLoja = u;
                            break;
                        case "emissor":
                            idVendedor = u;
                            break;
                        case "cliente":
                            idCliente = u;
                            break; ;
                        case "tipoPedido":
                            tipoPedido = u.ToString();
                            break;
                        case "rota":
                            idRota = u;
                            break;
                    }

                    ChartVendas[] serie = GetVendas((int?)idLoja, tipoFunc, (int?)idVendedor, (int?)idCliente, (int?)idRota, nomeCliente, periodoIni.ToString("dd/MM/yyyy"), periodoIni.AddMonths(1).AddDays(-1).ToString("dd/MM/yyyy"), tipoPedido, agrupar);

                    if (string.IsNullOrEmpty(criterio) && serie.Any(f => !string.IsNullOrEmpty(f.Criterio)))
                        criterio = serie.FirstOrDefault(f => !string.IsNullOrEmpty(f.Criterio)).Criterio;

                    foreach (ChartVendas s in serie)
                    {
                        s.Periodo = periodoIni.ToString("MMM-yy");
                        switch (tipoAgrupar)
                        {
                            case "nenhum":
                                dictVendas[1].Add(s);
                                break;
                            case "loja":
                                dictVendas[s.IdLoja].Add(s);
                                break;
                            case "emissor":
                                if (ids.Contains(s.IdFunc))
                                    dictVendas[s.IdFunc].Add(s);
                                break;
                            case "cliente":
                                if (ids.Contains(s.IdCliente))
                                    dictVendas[s.IdCliente].Add(s);
                                break;
                            case "tipoPedido":
                                if (ids.Contains((uint)s.TipoPedido))
                                    dictVendas[(uint)s.TipoPedido].Add(s);
                                break;
                            case "rota":
                                if (ids.Contains((uint)s.IdRota))
                                    dictVendas[(uint)s.IdRota].Add(s);
                                break;
                        }
                    }

                    ChartVendas cv;
                    foreach (KeyValuePair<uint, List<ChartVendas>> entry in dictVendas)
                    {
                        if (entry.Key == u && entry.Value.Count < count)
                        {
                            cv = new ChartVendas();
                            cv.Periodo = periodoIni.ToString("MMM-yy");

                            switch (tipoAgrupar)
                            {
                                case "loja":
                                    cv.IdLoja = entry.Key;
                                    cv.NomeLoja =  LojaDAO.Instance.GetNome(entry.Key);
                                    cv.Agrupar = 1;
                                    break;
                                case "emissor":
                                    cv.IdFunc = entry.Key;
                                    cv.NomeVendedor = FuncionarioDAO.Instance.GetNome(entry.Key);
                                    cv.Agrupar = 2;
                                    break;
                                case "cliente":
                                    cv.IdCliente = entry.Key;
                                    cv.NomeCliente = ClienteDAO.Instance.GetNome(entry.Key);
                                    cv.Agrupar = 3;
                                    break;
                                case "tipoPedido":
                                    cv.TipoPedido = (int)entry.Key;
                                    cv.Agrupar = 4;
                                    break;
                                case "rota":
                                    cv.IdRota = (int)entry.Key;
                                    cv.DescricaoRota = RotaDAO.Instance.ObterDescricaoRota(null, idRota);
                                    cv.Agrupar = 5;
                                    break;
                            }

                            entry.Value.Add(cv);
                        }
                    }
                }

                periodoIni = periodoIni.AddMonths(+1);
                count++;
            }

            return dictVendas;
        }
        
        public ChartVendas[] GetVendas(int? idLoja, int tipoFunc, int? idVendedor, int? idCliente, int? idRota, string nomeCliente,
            string dataIni, string dataFim, string tipoPedido, int agrupar)
        {
            string data = PedidoConfig.LiberarPedido ? "DataLiberacao" : "DataConf";

            bool temFiltro;
            string filtroAdicional;

            // Mesmos filtros utilizados no relatório de pedidos
            tipoPedido = !string.IsNullOrEmpty(tipoPedido) ? tipoPedido : "1,2,3";

            var sql = PedidoDAO.Instance.SqlGraficoVendas(dataFim, dataIni, out filtroAdicional, idCliente,
                tipoFunc == 0 ? idVendedor : 0, idLoja, tipoFunc == 0 ? 0 : idVendedor, idRota, nomeCliente, out temFiltro, tipoPedido).Replace("?filtroAdicional?", filtroAdicional);

            sql = @"
                Select p.idLoja, p.idFunc" + (tipoFunc == 0 ? "" : "Cliente") + @" as idFunc, cast(Sum(TotalReal) as decimal(12,2)) as TotalVenda, 
                    NomeFunc" + (tipoFunc == 0 ? "" : "Cliente") + @" as NomeVendedor, NomeLoja, IdCli as IdCliente, NomeCliente,
                    DATE_FORMAT(p." + data + @", '%d/%m/%Y') as Periodo, p.tipoPedido, p.IdRota, p.DescricaoRota, Criterio
                From (" + sql + @") as p
                Where 1";

            // Agrupar por loja 
            if (agrupar == 1)
            {
                sql += " Group By p.idLoja, date_format(STR_TO_DATE(periodo, '%d/%m/%Y'), '%m/%Y')";
                sql += " Order By NomeLoja";
            }
            // Agrupar por vendedor
            else if (agrupar == 2)
            {
                sql += " Group By p.idFunc" + (tipoFunc == 0 ? "" : "Cliente");
                sql += " Having sum(p.Total) > 500";
                sql += " Order By sum(p.Total) desc, NomeFunc" + (tipoFunc == 0 ? "" : "Cliente");
            }
            // Agrupar por cliente
            else if (agrupar == 3)
            {
                sql += " Group By p.idCli";
                sql += " Having sum(p.Total) > 500";
                sql += " Order By sum(p.Total) desc, NomeCliente";
            }
            // Agrupar por tipo pedido
            else if (agrupar == 4)
            {
                sql += " Group By p.tipoPedido, date_format(STR_TO_DATE(periodo, '%d/%m/%Y'), '%m/%Y')";
                sql += " Order By p.tipoPedido";
            }
            // Agrupar por Rota
            else if(agrupar == 5)
            {
                sql += " GROUP BY p.IdRota";
                sql += " ORDER BY p.IdRota";
            }

            sql += " limit 0,15";

            List<ChartVendas> retorno = objPersistence.LoadData(sql, GetParams(dataIni, dataFim));

            foreach (ChartVendas c in retorno)
            {
                c.Agrupar = agrupar;
            }

            return retorno.ToArray();
        }

        public List<ChartVendas> GetListaVendas(uint idLoja, int tipoFunc, uint idVendedor, uint idCliente, string nomeCliente,
            string dataIni, string dataFim, string tipoPedido, int agrupar)
        {
            string data = PedidoConfig.LiberarPedido ? "DataLiberacao" : "DataConf";

            bool temFiltro;
            string filtroAdicional;

            // Mesmos filtros utilizados no relatório de pedidos
            tipoPedido = !String.IsNullOrEmpty(tipoPedido) ? tipoPedido : "1,2,3";
            string tipoVenda = "1,2,5";

            var login = UserInfo.GetUserInfo;
            var cliente = login.IsCliente;
            var administrador = login.IsAdministrador;
            var emitirGarantia = Config.PossuiPermissao(Config.FuncaoMenuPedido.EmitirPedidoGarantia);
            var emitirReposicao = Config.PossuiPermissao(Config.FuncaoMenuPedido.EmitirPedidoReposicao);
            var emitirPedidoFuncionario = Config.PossuiPermissao(Config.FuncaoMenuPedido.EmitirPedidoFuncionario);

            string sql = PedidoDAO.Instance.SqlRptSit(0, "", 0, null, null, (idCliente > 0 ? idCliente.ToString() : null), nomeCliente, 0,
                (idLoja > 0 ? idLoja.ToString() : null), (int)Pedido.SituacaoPedido.Confirmado + "," + (int)Pedido.SituacaoPedido.LiberadoParcialmente, 
                dataIni, dataFim, null, null, null, null, 0, 0, tipoPedido, 0, 0, 0, null, tipoVenda, 0, null, null, false, false, false, null, null, 0, 
                null, null, 0, 0, null, null, null, null, false, 0, 0, true, false, false, true, out temFiltro, out filtroAdicional, 0, null, 0, true, 0, 
                null, cliente, administrador, emitirGarantia, emitirReposicao, emitirPedidoFuncionario).Replace("?filtroAdicional?", filtroAdicional);

            sql = @"
                Select p.idLoja, p.idFunc" + (tipoFunc == 0 ? "" : "Cliente") + @" as idFunc, Sum(TotalReal) as TotalVenda, 
                    NomeFunc" + (tipoFunc == 0 ? "" : "Cliente") + @" as NomeVendedor, NomeLoja, IdCli as IdCliente, 
                    NomeCliente, DATE_FORMAT(p." + data + @", '%d/%m/%Y') as Periodo, Criterio
                From (" + sql + @") as p
                Where 1";

            if (idVendedor > 0)
                sql += " And p.idFunc" + (tipoFunc == 0 ? "" : "Cliente") + "=" + idVendedor;

            // Agrupar por loja 
            if (agrupar == 1)
            {
                sql += " Group By p.idLoja, date_format(STR_TO_DATE(periodo, '%d/%m/%Y'), '%m/%Y')";
                sql += " Order By NomeLoja";
            }
            // Agrupar por vendedor
            else if (agrupar == 2)
            {
                sql += " Group By date_format(STR_TO_DATE(periodo, '%d/%m/%Y'), '%m/%Y'), p.idFunc" + (tipoFunc == 0 ? "" : "Cliente");
                sql += " Having sum(p.Total) > 500";
                sql += " Order By sum(p.Total) desc, NomeFunc" + (tipoFunc == 0 ? "" : "Cliente");
            }
            // Agrupar por cliente
            else if (agrupar == 3)
            {
                sql += " Group By date_format(STR_TO_DATE(periodo, '%d/%m/%Y'), '%m/%Y'), p.idCli";
                sql += " Having sum(p.Total) > 500";
                sql += " Order By sum(p.Total) desc, NomeCliente";
            }
            else // Nenhum
                sql += " Group By (Right(Concat('0', Cast(Month(p." + data + ") as char), '/', Cast(Year(p." + data + ") as char)), 7))";

            sql += " limit 0,15";

            List<ChartVendas> retorno = objPersistence.LoadData(sql, GetParams(dataIni, dataFim));
            foreach (ChartVendas c in retorno)
                c.Agrupar = agrupar;

            return retorno;
        }
        
        private GDAParameter[] GetParams(string dataIni, string dataFim)
        {
            List<GDAParameter> lstParams = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(dataIni))
                lstParams.Add(new GDAParameter("?dtIniSit", DateTime.Parse(dataIni + " 00:00:00")));

            if (!String.IsNullOrEmpty(dataFim))
                lstParams.Add(new GDAParameter("?dtFimSit", DateTime.Parse(dataFim + " 23:59:59")));

            return lstParams.ToArray();
        }

        public ChartVendasImagem[] ObterChartVendaItem()
        {
            return new ChartVendasImagem[0];
        }
    }
}