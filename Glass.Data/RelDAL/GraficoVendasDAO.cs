using System;
using System.Collections.Generic;
using Glass.Data.RelModel;
using Glass.Data.DAL;
using Glass.Data.Model;
using Glass.Data.Helper;
using GDA;
using Glass.Configuracoes;

namespace Glass.Data.RelDAL
{
    public sealed class GraficoVendasDAO : BaseDAO<GraficoVendas, GraficoVendasDAO>
    {
        //private GraficoVendasDAO() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idLoja"></param>
        /// <param name="idVendedor"></param>
        /// <param name="dataIni"></param>
        /// <param name="dataFim"></param>
        /// <param name="agrupar"></param>
        /// <param name="series">Se verdadeiro, busca apenas séries (do gráfico), senão busca todos os dados</param>
        /// <returns></returns>
        public GraficoVendas[] GetVendas(uint idLoja, int tipoFunc, uint idVendedor, uint idCli, string nomeCliente, 
            string dataIni, string dataFim, int agrupar, bool series)
        {
            string data = PedidoConfig.LiberarPedido ? "DataLiberacao" : "DataConf";

            bool temFiltro;
            string filtroAdicional;

            var login = UserInfo.GetUserInfo;
            var cliente = login.IsCliente;
            var administrador = login.IsAdministrador;
            var emitirGarantiaReposicao = Config.PossuiPermissao(Config.FuncaoMenuPedido.EmitirPedidoGarantiaReposicao);
            var emitirPedidoFuncionario = Config.PossuiPermissao(Config.FuncaoMenuPedido.EmitirPedidoFuncionario);

            string sql = PedidoDAO.Instance.SqlRptSit(0, "", 0, null, null, idCli.ToString(), nomeCliente, 0, (idLoja > 0 ? idLoja.ToString() : null), 
                (int)Pedido.SituacaoPedido.Confirmado + "," + (int)Pedido.SituacaoPedido.LiberadoParcialmente, dataIni, dataFim, null, 
                null, null, null, 0, 0, null, 0, 0, 0, null, null, 0, null, null, false, false, false, null, null, 0, null, null, 0, 0, null, null, 
                null, null, false, 0, 0, true, false, false, true, out temFiltro, out filtroAdicional, 0, null, 0, true, 0, null, 
                 cliente, administrador, emitirGarantiaReposicao, emitirPedidoFuncionario).Replace("?filtroAdicional?", filtroAdicional);

            string criterio = String.Empty;

            sql = @"
                Select p.idLoja, p.idFunc" + (tipoFunc == 0 ? "" : "Cliente") + @" as idFunc, cast(Sum(p.Total) as decimal(12,2)) as TotalVenda, 
                    NomeFunc" + (tipoFunc == 0 ? "" : "Cliente") + " as NomeVendedor, NomeLoja, (Right(Concat('0', Cast(Month(p." + data +
                    ") as char), '/', Cast(Year(p." + data + @") as char)), 7)) as DataVenda, p.idCli as idCliente, NomeCliente, '$$$' as Criterio
                From (" + sql + @") as p
                Where 1";

            if (idLoja > 0)
                criterio += "Loja: " + LojaDAO.Instance.GetNome(idLoja) + "    ";

            if (idVendedor > 0)
            {
                sql += " And p.idFunc" + (tipoFunc == 0 ? "" : "Cliente") + "=" + idVendedor;
                criterio += (tipoFunc == 0 ? "Emissor" : "Vendedor") + ": " + BibliotecaTexto.GetTwoFirstNames(FuncionarioDAO.Instance.GetNome(idVendedor)) + "    ";
            }

            if (idCli > 0)
                criterio += "Cliente: " + ClienteDAO.Instance.GetNome(idCli) + "    ";
            else if (!String.IsNullOrEmpty(nomeCliente))
                criterio += "Cliente: " + nomeCliente + "    ";

            if (!String.IsNullOrEmpty(dataIni))
                criterio += "Data Início: " + dataIni + "    ";

            if (!String.IsNullOrEmpty(dataFim))
                criterio += "Data Fim: " + dataFim + "    ";

            // Agrupar por loja 
            if (agrupar == 1)
            {
                sql += " Group By p.idLoja";

                if (!series)
                    sql += ", (Right(Concat('0', Cast(Month(p." + data + ") as char), '/', Cast(Year(p." + data + ") as char)), 7))";

                sql += " Order By NomeLoja";
            }
            // Agrupar por vendedor
            else if (agrupar == 2)
            {
                sql += " Group By p.idFunc" + (tipoFunc == 0 ? "" : "Cliente");

                if (!series)
                    sql += ", (Right(Concat('0', Cast(Month(p." + data + ") as char), '/', Cast(Year(p." + data + ") as char)), 7))";

                sql += " Having sum(p.Total) > 500";

                sql += " Order By NomeFunc" + (tipoFunc == 0 ? "" : "Cliente");
            }
            else
            // Nenhum
            {
                if (!series)
                    sql += " Group By (Right(Concat('0', Cast(Month(p." + data + ") as char), '/', Cast(Year(p." + data + ") as char)), 7))";
            }


            return objPersistence.LoadData(sql.Replace("$$$", criterio), GetParams(dataIni, dataFim, nomeCliente)).ToList().ToArray();
        }

        private GDAParameter[] GetParams(string dataIni, string dataFim, string nomeCliente)
        {
            List<GDAParameter> lstParams = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(dataIni))
                lstParams.Add(new GDAParameter("?dtIniSit", DateTime.Parse(dataIni + " 00:00:00")));

            if (!String.IsNullOrEmpty(dataFim))
                lstParams.Add(new GDAParameter("?dtFimSit", DateTime.Parse(dataFim + " 23:59:59")));

            if (!String.IsNullOrEmpty(nomeCliente))
                lstParams.Add(new GDAParameter("?nomeCliente", "%" + nomeCliente + "%"));

            return lstParams.ToArray();
        }
    }
}
