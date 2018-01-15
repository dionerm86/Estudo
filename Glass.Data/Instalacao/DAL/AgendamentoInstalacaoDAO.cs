using System;
using System.Collections.Generic;
using Glass.Data.Model;
using GDA;

namespace Glass.Data.DAL
{
    public sealed class AgendamentoInstalacaoDAO : BaseDAO<AgendamentoInstalacao, AgendamentoInstalacaoDAO>
    {
        //private AgendamentoInstalacaoDAO() { }

        public List<AgendamentoInstalacao> ObterListaPorEquipe(uint idEquipe, string dataInicial, string dataFinal)
        {
            string query = String.Empty;
            string criterio = String.Empty;

            if (idEquipe > 0)
            {
                query += " and ei.IDEQUIPE=?id";
                criterio += "Equipe: " + idEquipe + "    ";
            }

            if (!string.IsNullOrEmpty(dataInicial) && !string.IsNullOrEmpty(dataFinal))
            {
                query += " and i.DATAINSTALACAO >= ?ini and i.DATAINSTALACAO <= ?fim";
                criterio += "Período: " + dataInicial + " a " + dataFinal + "    ";
            }

            string sql = @"select e.IDEQUIPE as IdEquipeInstalacao, e.NOME as NomeEquipe, 
                            i.DATAINSTALACAO as DataInstalacao, 
                            COUNT(ei.IDINSTALACAO) as QuantidadeInstalacao, '$$$' as Criterio
                            from equipe e
                            inner join equipe_instalacao ei ON(e.IDEQUIPE=ei.IDEQUIPE)
                            inner join instalacao i on(ei.IDINSTALACAO=i.IDINSTALACAO)
                            inner join pedido p on(i.IDPEDIDO=p.IDPEDIDO)
                            inner join cliente c on(p.IDCLI=c.ID_CLI)
                            where 1 and i.Situacao not in (3,4,5) " + query + " group by i.DATAINSTALACAO order by i.DATAINSTALACAO asc";

            return objPersistence.LoadData(sql.Replace("$$$", criterio), new GDAParameter("?id", idEquipe), new GDAParameter("?ini", DateTime.Parse(dataInicial + " 00:00")), new GDAParameter("?fim", DateTime.Parse(dataFinal + " 23:59")));
        }

        public List<AgendamentoInstalacao> ObterListaPorCliente(uint idCliente, string dataInicial, string dataFinal)
        {
            string query = String.Empty;
            string criterio = String.Empty;

            if (idCliente > 0)
            {
                query += " and c.ID_CLI=?id";
                criterio += "Cliente: " + ClienteDAO.Instance.GetNome(idCliente) + "    ";
            }

            if (!string.IsNullOrEmpty(dataInicial) && !string.IsNullOrEmpty(dataFinal))
            {
                query += " and i.DATAINSTALACAO >= ?ini and i.DATAINSTALACAO <= ?fim";
                criterio += "Período: " + dataInicial + " a " + dataFinal + "    ";
            }

            string sql = @"select e.IDEQUIPE as IdEquipeInstalacao, e.NOME as NomeEquipe, 
                            i.DATAINSTALACAO as DataInstalacao, c.Nome as Cliente,'$$$' as Criterio
                            from equipe e
                            inner join equipe_instalacao ei ON(e.IDEQUIPE=ei.IDEQUIPE)
                            inner join instalacao i on(ei.IDINSTALACAO=i.IDINSTALACAO)
                            inner join pedido p on(i.IDPEDIDO=p.IDPEDIDO)
                            inner join cliente c on(p.IDCLI=c.ID_CLI)
                            where 1 " + query + " group by i.DATAINSTALACAO order by i.DATAINSTALACAO asc";

            return objPersistence.LoadData(sql.Replace("$$$", criterio), new GDAParameter("?id", idCliente), new GDAParameter("?ini", DateTime.Parse(dataInicial + " 00:00")), new GDAParameter("?fim", DateTime.Parse(dataFinal + " 23:59")));
        }

    }
}
