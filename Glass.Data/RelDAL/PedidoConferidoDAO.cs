using System;
using System.Collections.Generic;
using Glass.Data.DAL;
using Glass.Data.RelModel;
using GDA;
using Glass.Data.Helper;

namespace Glass.Data.RelDAL
{
    public sealed class PedidoConferidoDAO : BaseDAO<PedidoConferido, PedidoConferidoDAO>
    {
        //private PedidoConferidoDAO() { }

        private string Sql(uint idPedido, uint idLoja, uint idConferente, uint idVendedor, int situacao, string dataIni, string dataFim, bool selecionar)
        {
            var criterio = String.Empty;

            var campos = selecionar ? @"pe.idPedido, Concat(cast(p.idCli as char), ' - ', c.nome) as nomeCliente, f.nome as conferente,
                p.total as totalPedidoOriginal, pe.total as totalPedidoConferido, pe.situacao as situacaoEspelho, pe.dataEspelho, '$$$' as criterio, 
                if((Select Count(*) from contas_receber Where idPedido=pe.idPedido And
                idConta=" +  UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.ValorExcedente) + ") > 0, 'Sim', 'Não') as GerouExcedente" :
                "Count(*)";

            var sql = @"
                Select " + campos + @" From pedido_espelho pe 
                    Inner Join pedido p On (pe.idPedido=p.idPedido) 
                    left Join funcionario f On (pe.idFuncConf=f.idFunc)
                    Inner Join cliente c on (p.idCli=c.id_Cli) 
                Where 1";

            if (idPedido > 0)
            {
                sql += " And pe.idPedido=" + idPedido;
                criterio += "Pedido: " + idPedido + "    ";
            }

            if (idLoja > 0)
            {
                sql += " And p.idLoja=" + idLoja;
                criterio += "Loja: " + LojaDAO.Instance.GetNome(idLoja) + "    ";
            }

            if (idVendedor > 0)
            {
                sql += " And p.idFunc=" + idVendedor;
                criterio += "Vendedor: " + FuncionarioDAO.Instance.GetNome(idVendedor) + "    ";
            }

            if (idConferente > 0)
            {
                sql += " And pe.idFuncConf=" + idConferente;
                criterio += "Conferente: " + FuncionarioDAO.Instance.GetNome(idConferente) + "    ";
            }

            if (situacao > 0)
            {
                sql += " And pe.situacao=" + situacao;
                criterio += "Situação: " + (situacao == 1 ? "Aberto" : situacao == 2 ? "Finalizado" : situacao == 3 ? "Impresso" : situacao == 4 ? "Impresso Comum" : "");
            }

            if (!String.IsNullOrEmpty(dataIni))
            {
                sql += " And pe.dataEspelho>=?dataIni";
                criterio += "Data Início: " + dataIni + "    ";
            }

            if (!String.IsNullOrEmpty(dataFim))
            {
                sql += " And pe.dataEspelho<=?dataFim";
                criterio += "Data Fim: " + dataFim + "    ";
            }

            return sql.Replace("$$$", criterio);
        }

        public IList<PedidoConferido> GetList(uint idPedido, uint idLoja, uint idConferente, uint idVendedor,
            int situacao, string dataIni, string dataFim, string sortExpression, int startRow, int pageSize)
        {
            return objPersistence.LoadDataWithSortExpression(Sql(idPedido, idLoja, idConferente, idVendedor, situacao,
                dataIni, dataFim, true), new InfoSortExpression(sortExpression), new InfoPaging(startRow, pageSize),
                GetParam(dataIni, dataFim)).ToList();
        }

        public int GetCount(uint idPedido, uint idLoja, uint idConferente, uint idVendedor, int situacao, string dataIni, string dataFim)
        {
            return objPersistence.ExecuteSqlQueryCount(Sql(idPedido, idLoja, idConferente, idVendedor, situacao, dataIni, dataFim, false),
                GetParam(dataIni, dataFim));
        }

        public IList<PedidoConferido> GetForRpt(uint idPedido, uint idLoja, uint idConferente, uint idVendedor, int situacao, string dataIni, string dataFim)
        {
            return objPersistence.LoadData(Sql(idPedido, idLoja, idConferente, idVendedor, situacao, dataIni, dataFim, true), GetParam(dataIni, dataFim)).ToList();
        }

        private GDAParameter[] GetParam(string dataIni, string dataFim)
        {
            List<GDAParameter> lstParam = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(dataIni))
                lstParam.Add(new GDAParameter("?dataIni", DateTime.Parse(dataIni + " 00:00")));

            if (!String.IsNullOrEmpty(dataFim))
                lstParam.Add(new GDAParameter("?dataFim", DateTime.Parse(dataFim + " 23:59")));

            return lstParam.Count > 0 ? lstParam.ToArray() : null;
        }
    }
}