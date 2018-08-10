using System;
using System.Collections.Generic;
using Glass.Data.RelModel;
using Glass.Data.DAL;
using GDA;
using Glass.Data.Helper;
using Glass.Data.Model;
using Glass.Configuracoes;

namespace Glass.Data.RelDAL
{
    public sealed class PontoEquilibrioDAO : BaseDAO<PontoEquilibrio, PontoEquilibrioDAO>
    {
        //private PontoEquilibrioDAO() { }

        public List<PontoEquilibrio> GetPontoEquilibrio(string dtIni, string dtFim, LoginUsuario login)
        {
            List<PontoEquilibrio> lista = new List<PontoEquilibrio>();

            if (!string.IsNullOrEmpty(dtIni) && !string.IsNullOrEmpty(dtFim))
            {
                PontoEquilibrio venda = GetVendas(0, 0, 0, 0, null, dtIni, dtFim, 0, login); //GetVendas(dtIni, dtFim);
                venda.Percentual = "100%";

                if (venda.Valor > 0)
                {
                    PontoEquilibrio custo = GetCustoProdutosVendidos(dtIni, dtFim);
                    custo.Percentual = custo.Valor > 0 ? string.Format("{0:N2}", (custo.Valor * 100 / venda.Valor)) + "%"  : 0 + "%";

                    PontoEquilibrio despVariavel = new PontoEquilibrio() { Indice = 3, Item = "Despesas Variáveis" };
                    List<PontoEquilibrio> despVariaveis = GetDespesasVariaveis(dtIni, dtFim);
                    despVariavel.subItens = despVariaveis;
                    foreach (PontoEquilibrio i in despVariaveis)
                    {
                        i.Percentual = string.Format("{0:N2}", (i.Valor * 100 / venda.Valor)) + "%";
                        despVariavel.Valor += i.Valor;
                    }
                    despVariavel.Percentual = string.Format("{0:N2}", (despVariavel.Valor * 100 / venda.Valor)) + "%";

                    PontoEquilibrio despFixa = GetDespesaFixa(dtIni, dtFim);
                    despFixa.Percentual = string.Format("{0:N2}", (despFixa.Valor * 100 / venda.Valor)) + "%";

                    PontoEquilibrio imc = new PontoEquilibrio()
                    {
                        Indice = 4,
                        Item = "Margem de Contribuição",
                        Valor = venda.Valor - custo.Valor - despVariavel.Valor,
                    };
                    imc.Percentual = string.Format("{0:N2}", (imc.Valor * 100 / venda.Valor)) + "%";

                    PontoEquilibrio lucro = new PontoEquilibrio()
                    {
                        Indice = 6,
                        Item = "Lucro Líquido",
                        Valor = imc.Valor - despFixa.Valor
                    };
                    lucro.Percentual = string.Format("{0:N2}", (lucro.Valor * 100 / venda.Valor)) + "%";

                    decimal pePerc = decimal.Round((imc.Valor * 100) / venda.Valor, 2) / 100;

                    PontoEquilibrio pe = new PontoEquilibrio()
                    {
                        Indice = 7,
                        Item = "Ponto de Equilíbrio",
                        Valor = despFixa.Valor != 0 ? despFixa.Valor / pePerc : 0
                    };
                    pe.Percentual = string.Format("{0:N2}", (pe.Valor * 100 / venda.Valor)) + "%";

                    lista.Add(venda);
                    lista.Add(custo);
                    lista.Add(despVariavel);
                    lista.Add(imc);
                    lista.Add(despFixa);
                    lista.Add(lucro);
                    lista.Add(pe);
                }
            }

            return lista;
        }

        private PontoEquilibrio GetVendas(uint idLoja, int tipoFunc, uint idVendedor, uint idCliente, string nomeCliente, string dataIni, 
            string dataFim, int agrupar, LoginUsuario login)
        {
            string data = PedidoConfig.LiberarPedido ? "DataLiberacao" : "DataConf";

            bool temFiltro;
            string filtroAdicional;

            // Mesmos filtros utilizados no relatório de pedidos
            string tipoPedido = "1,2,3";
            string tipoVenda = "1,2,5";

            var cliente = login == null ? false: login.IsCliente;
            var administrador = login == null ? true: login.IsAdministrador;
            var emitirGarantia = login == null ? true : Config.PossuiPermissao((int)login.CodUser, Config.FuncaoMenuPedido.EmitirPedidoGarantia);
            var emitirReposicao = login == null ? true : Config.PossuiPermissao((int)login.CodUser, Config.FuncaoMenuPedido.EmitirPedidoReposicao);
            var emitirPedidoFuncionario = login == null ? true : Config.PossuiPermissao((int)login.CodUser, Config.FuncaoMenuPedido.EmitirPedidoFuncionario);
            
            string sql = PedidoDAO.Instance.SqlRptSit(0, "", 0, null, null, (idCliente > 0 ? idCliente.ToString() : null), nomeCliente, 0,
                (idLoja > 0 ? idLoja.ToString() : null), (int)Pedido.SituacaoPedido.Confirmado + "," + (int)Pedido.SituacaoPedido.LiberadoParcialmente,
                dataIni, dataFim, null, null, null, null, 0, 0, tipoPedido, 0, 0, 0, null, tipoVenda, 0, null, null, false, false, false, null, null, 0, null,
                null, 0, 0, null, null, null, null, false, 0, 0, true, false, false, true, out temFiltro, out filtroAdicional, 0, null, 0, true, 0, null,
                cliente, administrador, emitirGarantia, emitirReposicao, emitirPedidoFuncionario).Replace("?filtroAdicional?", filtroAdicional);

            sql = string.Format(@"SELECT 1 AS Indice, 'Vendas' AS Item, CAST(SUM(p.Total) AS DECIMAL(12,2)) AS Valor, CAST(GROUP_CONCAT(p.IdProd) AS CHAR) AS Produtos FROM ({0}) AS p WHERE 1",
                sql.Replace("SELECT", "SELECT plp.IdProdPed AS IdProd, "));

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
            //else // Nenhum
            //    sql += " Group By (Right(Concat('0', Cast(Month(p." + data + ") as char), '/', Cast(Year(p." + data + ") as char)), 7))";

            sql += " limit 0,15";

            return objPersistence.LoadOneData(sql, GetParams(dataIni, dataFim));
        }

        private PontoEquilibrio GetCustoProdutosVendidos(string dtIni, string dtFim)
        {
            string filtro = PedidoConfig.LiberarPedido ? " And lp.dataLiberacao>=?dtIni And lp.dataLiberacao<=?dtFim and lp.situacao=1" :
                "And pd.DataConf >=?dtIni And pd.DataConf <=?dtFim and pd.Situacao=5";

            /*string custo = PedidoDAO.Instance.SqlCampoCustoLiberacao(PedidoConfig.LiberarPedido, 
                "sum(pp.custoProd+coalesce((select sum(custo) from produto_pedido_benef where idProdPed=pp.idProdPed),0))", 
                "Valor", "pd", "a", "pl");*/

            string custo = "(select sum(pv.totalCusto) from (" + ProdutoDAO.Instance.ObterVendaPontoEquilibrio(dtIni, dtFim) + ") as pv) as Valor";

            string sql = @"Select distinct 2 As Indice, 'Custo das Mercadorias Vendidas' As Item, " + custo + @"
                From produtos_pedido pp
                    left join ambiente_pedido a on (pp.idAmbientePedido=a.idAmbientePedido)
                    left join produtos_liberar_pedido pl on (pl.IdProdPed=pp.IdProdPed)
                    left join liberarpedido lp on (lp.IdLiberarPedido=pl.IdLiberarPedido)
                    inner join pedido pd on (pd.IdPedido=pp.IdPedido) 
                Where 1 " + filtro;

            return objPersistence.LoadOneData(sql, new GDAParameter("?dtIni", DateTime.Parse(dtIni + " 00:00:00")), new GDAParameter("?dtFim", DateTime.Parse(dtFim + " 23:59:59")));
        }

        private List<PontoEquilibrio> GetDespesasVariaveis(string dtIni, string dtFim)
        {
            string sql = @"select sum(case cp.paga when 1 then cp.valorPago else cp.valorVenc end) As Valor,
                gc.Descricao As Item, 3 As Parent from contas_pagar cp
                inner join plano_contas pc on(cp.idConta=pc.idConta)
                inner join grupo_conta gc on(gc.idGrupo=pc.idGrupo)
                where gc.PontoEquilibrio=true and if(cp.paga=true, cp.dataPagto, cp.dataVenc)>= ?dataIni
                and  if(cp.paga=true, cp.dataPagto, cp.dataVenc) <= ?dataFim and cp.idCustoFixo is null  group by gc.Descricao";

            List<PontoEquilibrio> lista = objPersistence.LoadData(sql, new GDAParameter("?dataIni", DateTime.Parse(dtIni + " 00:00:00")), new GDAParameter("?dataFim", DateTime.Parse(dtFim + " 23:59:59")));

            return lista;
        }

        private PontoEquilibrio GetDespesaFixa(string dtIni, string dtFim)
        {
            string sql = @"select sum(case cp.paga when 1 then cp.valorPago else cp.valorVenc end) As Valor,
                 'Despesas Fixas' As Item, 5 As Indice from contas_pagar cp
                inner join custo_fixo cf on(cp.idCustoFixo=cf.idCustoFixo)
                where cf.PontoEquilibrio=true and  if(cp.paga=true, cp.dataPagto, cp.dataVenc) >= ?dataIni
                and  if(cp.paga=true, cp.dataPagto, cp.dataVenc) <= ?dataFim and cp.idCustoFixo is not null";

            return objPersistence.LoadOneData(sql, new GDAParameter("?dataIni", DateTime.Parse(dtIni + " 00:00:00")), new GDAParameter("?dataFim", DateTime.Parse(dtFim + " 23:59:59")));
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
    }
}
