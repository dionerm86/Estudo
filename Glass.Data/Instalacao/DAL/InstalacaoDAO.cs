using System;
using System.Collections.Generic;
using System.Linq;
using GDA;
using Glass.Data.Model;
using Glass.Data.Helper;
using Glass.Configuracoes;

namespace Glass.Data.DAL
{
    public sealed class InstalacaoDAO : BaseCadastroDAO<Instalacao, InstalacaoDAO>
    {
        //private InstalacaoDAO() { }

        private const string SEPARADOR_EQUIPES = " - ";

        #region Variáveis locais

        private static readonly object _instalacaoLock = new object();

        #endregion

        #region Busca Instalações

        private void PreencheValorInstalado(ref Instalacao[] lstInst)
        {
            // Calcula o valor instalado somente se for exibir o relatório
            string sqlValorProd = @"select sum(((pp.total+coalesce(pp.valorBenef,0){0})/pp.qtde*if(?emAndamento, 
                    pp.qtde - coalesce(pi.qtdeInstalada, 0), pi.qtdeInstalada)))
                from produtos_instalacao pi 
                    left join produtos_pedido pp on (pi.idProdPed=pp.idProdPed)
                    left join ambiente_pedido a on (pp.idAmbientePedido=a.idAmbientePedido)
                    left join produtos_pedido_espelho ppe on (pp.idProdPedEsp=ppe.idProdPed)
                    left join ambiente_pedido_espelho ae on (ppe.idAmbientePedido=ae.idAmbientePedido)
                    left join pedido ped on (pp.idPedido=ped.idPedido)
                    left join pedido_espelho pedEsp on (pp.idPedido=pedEsp.idPedido)
                where pi.idInstalacao=?idInstalacao";

            if (!PedidoConfig.RatearDescontoProdutos)
            {
                string format = @" - coalesce((if(coalesce({0}.desconto, {1}.desconto)=0, 0, coalesce({0}.desconto, {1}.desconto) *
                    if(coalesce({0}.tipoDesconto, {1}.tipoDesconto)=2, 1, (select sum(total + coalesce(valorBenef, 0)) from produtos_pedido
                    where idPedido=?idPedido And coalesce(invisivelFluxo,false)=false and {2}=coalesce({0}.{2}, {1}.{2}))/100)) /
                    (select count(*) from produtos_pedido where idPedido=?idPedido And coalesce(invisivelFluxo,false)=false and {2}=coalesce({0}.{2}, {1}.{2}))), 0)";

                sqlValorProd = String.Format(sqlValorProd, String.Format(format, "pedEsp", "ped", "idPedido") +
                    String.Format(format, "ae", "a", "idAmbientePedido"));
            }
            else
                sqlValorProd = String.Format(sqlValorProd, "");

            foreach (Instalacao inst in lstInst)
            {
                string sqlExec = sqlValorProd
                    .Replace("?idInstalacao", inst.IdInstalacao.ToString())
                    .Replace("?idPedido", inst.IdPedido.ToString())
                    .Replace("?emAndamento", (inst.Situacao == (int)Instalacao.SituacaoInst.EmAndamento).ToString());

                inst.ValorProdutosInstalados = ExecuteScalar<decimal>(sqlExec);
            }
        }

        private string Sql(uint idInstalacao, string idPedido, uint idOrdemInstalacao, uint idOrcamento, uint idEquipe, string tiposInstalacao, string situacoes,
            string dataIni, string dataFim, string dataIniEnt, string dataFimEnt, string dataIniOrdemInst, string dataFimOrdemInst, uint idLoja,
            string idsCliente, string telefone, string observacao, bool incluirJoin, bool finalizadas, bool selecionar)
        {
            return Sql(null, idInstalacao, idPedido, idOrdemInstalacao, idOrcamento, idEquipe, tiposInstalacao, situacoes, dataIni, dataFim, dataIniEnt, dataFimEnt,
                dataIniOrdemInst, dataFimOrdemInst, idLoja, idsCliente, telefone, observacao, incluirJoin, finalizadas, selecionar);
        }

        private string Sql(GDASession session, uint idInstalacao, string idPedido, uint idOrdemInstalacao, uint idOrcamento, uint idEquipe, string tiposInstalacao, string situacoes, 
            string dataIni, string dataFim, string dataIniEnt, string dataFimEnt, string dataIniOrdemInst, string dataFimOrdemInst, uint idLoja,
            string idsCliente, string telefone, string observacao, bool incluirJoin, bool finalizadas, bool selecionar)
        {
            string campos = selecionar ? "i.*, " + (incluirJoin ? "group_concat(e.Nome separator '" + SEPARADOR_EQUIPES + "') as NomesEquipes, " +
                "c.id_Cli as idCliente, c.Nome as NomeCliente, l.NomeFantasia as NomeLoja, " : "") + "p1.DataConf as DataConfPedido, Concat(p1.EnderecoObra, ' - ', p1.BairroObra, ' - ', " +
                @"p1.CidadeObra) as LocalObra, '$$$' as Criterio, p1.total as ValorPedido, p1.idOrcamento" : "count(*)";

            string criterio = String.Empty;

            string sql = "Select " + campos + @" From instalacao i
                Left Join pedido p1 On (i.idPedido=p1.idPedido) ";

            if (incluirJoin || selecionar)
                sql += @"
                    Left Join equipe_instalacao ei On (i.idInstalacao=ei.idInstalacao) 
                    Left Join equipe e On (ei.idEquipe=e.IdEquipe) 
                    Left Join loja l On (p1.idLoja=l.idLoja) 
                    Left Join cliente c On (p1.idCli=c.id_Cli) ";
            
            sql += "Where 1";

            Instalacao temp = new Instalacao();

            if (idInstalacao > 0)
                sql += " and i.idInstalacao=" + idInstalacao;

            if (!String.IsNullOrEmpty(idPedido) && idPedido != "0")
            {
                sql += " And i.idPedido=" + idPedido;
                criterio += "Pedido: " + idPedido + "    ";
            }

            if (idOrdemInstalacao > 0)
            {
                sql += " And i.idOrdemInstalacao=" + idOrdemInstalacao;
                criterio += "Ordem Inst.: " + idOrdemInstalacao + "    ";
            }

            if(idOrcamento > 0)
            {
                sql += " AND p1.idOrcamento=" + idOrcamento;
                criterio += "Orçamento: " + idOrcamento + "    ";
            }

            if (idEquipe > 0)
            {
                sql += " And e.idEquipe=" + idEquipe;
                criterio += "Equipe: " + EquipeDAO.Instance.ObtemNome(session, idEquipe) + "    ";
            }

            if (!string.IsNullOrEmpty(tiposInstalacao) && tiposInstalacao != "0")
            {
                var descricaoTiposInstalacao = new List<string>();
                foreach (var tipoInstalacao in tiposInstalacao.Split(','))
                {
                    temp.TipoInstalacao = tipoInstalacao.StrParaInt();
                    descricaoTiposInstalacao.Add(temp.DescrTipoInstalacao);
            }

                sql += string.Format(" AND i.TipoInstalacao IN ({0})", tiposInstalacao);
                criterio += string.Format("Tipo(s) Colocação: {0}    ", string.Join(", ", descricaoTiposInstalacao.ToArray()));
            }

            if (!string.IsNullOrEmpty(situacoes) && situacoes != "0")
            {
                var descricaoSituacoes = new List<string>();
                foreach (var situacao in situacoes.Split(','))
                {
                    temp.Situacao = situacao.StrParaInt();
                    descricaoSituacoes.Add(temp.DescrSituacao);
            }

                sql += string.Format(" AND i.Situacao IN ({0})", situacoes);
                criterio += string.Format("Situação(ões): {0}    ", string.Join(", ", descricaoSituacoes.ToArray()));
            }

            if (!String.IsNullOrEmpty(idsCliente))
            {
                sql += " And c.id_Cli in (" + idsCliente + ") ";

                foreach (string s in idsCliente.Split(','))
                    temp.NomeCliente += ClienteDAO.Instance.GetNome(session, Glass.Conversoes.StrParaUint(s)) + ", ";

                criterio += "Clientes: " + temp.NomeCliente.TrimEnd(' ', ',') + "    ";
            }

            if (!String.IsNullOrEmpty(dataIni))
            {
                // Se tiver filtro por situação finalizada, busca o periodo que foi finalizada
                if (!(string.IsNullOrEmpty(situacoes)) && situacoes.Contains("3"))
                    sql += " And i.DataFinal>=?dataIni";
                else
                    sql += " And i.DataInstalacao>=?dataIni";

                criterio += "Data Início: " + dataIni + "    ";
            }

            if (!String.IsNullOrEmpty(dataFim)) 
            {
                // Se tiver filtro por situação finalizada, busca o periodo que foi finalizada
                if (!(string.IsNullOrEmpty(situacoes)) && situacoes.Contains("3"))
                    sql += " And i.DataFinal<=?dataFim";
                else
                    sql += " And i.DataInstalacao<=?dataFim";

                criterio += "Data Fim: " + dataFim + "    ";
            }

            if (!String.IsNullOrEmpty(dataIniEnt))
            {
                sql += " And i.DataEntrega>=?dataIniEnt";
                criterio += "Data Início Entrega: " + dataIniEnt + "    ";
            }

            if (!String.IsNullOrEmpty(dataFimEnt))
            {
                sql += " And i.DataEntrega<=?dataFimEnt";
                criterio += "Data Fim Entrega: " + dataFimEnt + "    ";
            }

            if (!String.IsNullOrEmpty(dataIniOrdemInst))
            {
                sql += " And i.DataOrdemInstalacao>=?dataIniOrdemInst";
                criterio += "Data Início Ordem Instalação: " + dataIniOrdemInst + "    ";
            }

            if (!String.IsNullOrEmpty(dataFimOrdemInst))
            {
                sql += " And i.DataOrdemInstalacao<=?dataFimOrdemInst";
                criterio += "Data Fim Ordem Instalação: " + dataFimOrdemInst + "    ";
            }

            if (idLoja > 0)
            {
                sql += " And p1.idLoja=" + idLoja;
                criterio += "Loja: " + LojaDAO.Instance.GetNome(session, idLoja) + "    ";
            }

            if (!String.IsNullOrEmpty(telefone))
            {
                sql += " And (c.tel_cel like ?telefone Or c.tel_cont like ?telefone Or c.tel_res like ?telefone)";
                criterio += "Telefone: " + telefone + "    ";
            }

            if (!String.IsNullOrEmpty(observacao))
            {
                sql += " And i.obs like '%" + observacao + "%'";
                criterio += "Observação: " + observacao + "    ";
            }

            var  login = UserInfo.GetUserInfo;

            if (login != null &&
                !Config.PossuiPermissao(Config.FuncaoMenuInstalacao.ControleInstalacaoComum) &&
                !Config.PossuiPermissao(Config.FuncaoMenuInstalacao.ControleInstalacaoTemperado) &&
                !Config.PossuiPermissao(Config.FuncaoMenuFinanceiro.ControleFinanceiroRecebimento) &&
                !Config.PossuiPermissao(Config.FuncaoMenuCaixaDiario.ControleCaixaDiario))
                sql += " And p1.idFunc=" + login.CodUser;

            if (finalizadas)
                sql += " and i.tipoInstalacao<>" + (int)Instalacao.TipoInst.Entrega;

            if (selecionar)
                sql += " group by i.idInstalacao";

            return sql.Replace("$$$", criterio);
        }

        public Instalacao GetElement(uint idInstalacao)
        {
            uint? idOrdemInst = ObtemValorCampo<uint?>("IdOrdemInstalacao", "idInstalacao=" + idInstalacao);

            return objPersistence.LoadOneData(Sql(idInstalacao, null, idOrdemInst != null ? idOrdemInst.Value : 0, 0, 0, null, null, null, null, null, null, 
                null, null, 0, null, null, null, true, false, true));
        }

        /// <summary>
        /// Busca instalações que pertencem à uma Ordem de Instalação
        /// </summary>
        /// <param name="idOrdemInst"></param>
        /// <returns></returns>
        public IList<Instalacao> GetByOrdemInst(uint idOrdemInst)
        {
            return objPersistence.LoadData(Sql(0, null, idOrdemInst, 0, 0, null, "2", null, null, null, null, null, null, 0, null, null, null, true, false,
                true)).ToList();
        }

        public IList<Instalacao> GetForRpt(uint idPedido, uint idOrdemInstalacao, uint idOrcamento, uint idEquipe, string tiposInstalacao, string situacoes, string dataIni, 
            string dataFim, string dataIniEnt, string dataFimEnt, string dataIniOrdemInst, string dataFimOrdemInst, uint idLoja, string nomeCliente, 
            string telefone, string observacao)
        {
            string idsClientes = null;
            if (!String.IsNullOrEmpty(nomeCliente))
                idsClientes = ClienteDAO.Instance.GetIds(null, nomeCliente, null, 0, null, null, null, null, 0);

            var lstInst = objPersistence.LoadData(Sql(0, idPedido.ToString(), idOrdemInstalacao, idOrcamento, idEquipe, tiposInstalacao, situacoes, dataIni, dataFim, 
                dataIniEnt, dataFimEnt, dataIniOrdemInst, dataFimOrdemInst, idLoja, idsClientes, telefone, observacao, true, false, true) +
                " Order By i.idInstalacao", GetParam(dataIni, dataFim, dataIniEnt, dataFimEnt, null, dataIniOrdemInst, dataFimOrdemInst, telefone)).ToList().ToArray();

            PreencheValorInstalado(ref lstInst);

            return lstInst;
        }

        public IList<Instalacao> GetList(uint idPedido, uint idOrdemInstalacao, uint idOrcamento, uint idEquipe, string tiposInstalacao, string situacoes,
            string dataIni, string dataFim, string dataIniEnt, string dataFimEnt, string dataIniOrdemInst, string dataFimOrdemInst, uint idLoja,
            string nomeCliente, string telefone, string observacao, string sortExpression, int startRow, int pageSize)
        {            
            string idsClientes = null;
            if (!String.IsNullOrEmpty(nomeCliente))
                idsClientes = ClienteDAO.Instance.GetIds(null, nomeCliente, null, 0, null, null, null, null, 0);
            
            string sql = Sql(0, idPedido.ToString(), idOrdemInstalacao, idOrcamento, idEquipe, tiposInstalacao, situacoes, dataIni, dataFim, dataIniEnt, dataFimEnt, 
                dataIniOrdemInst, dataFimOrdemInst, idLoja, idsClientes, telefone, observacao, true, false, true);

            string sort = String.IsNullOrEmpty(sortExpression) ? "i.idInstalacao" : sortExpression;

            return LoadDataWithSortExpression(sql, sort, startRow, pageSize, GetParam(dataIni, dataFim, dataIniEnt, dataFimEnt, null,
                dataIniOrdemInst, dataFimOrdemInst, telefone));
        }

        public int GetCount(uint idPedido, uint idOrdemInstalacao, uint idOrcamento, uint idEquipe, string tiposInstalacao, string situacoes, string dataIni, string dataFim, 
            string dataIniEnt, string dataFimEnt, string dataIniOrdemInst, string dataFimOrdemInst, uint idLoja, string nomeCliente, string telefone, 
            string observacao)
        {
            string idsClientes = null;
            if (!String.IsNullOrEmpty(nomeCliente))
                idsClientes = ClienteDAO.Instance.GetIds(null, nomeCliente, null, 0, null, null, null, null, 0);
            
            return objPersistence.ExecuteSqlQueryCount(Sql(0, idPedido.ToString(), idOrdemInstalacao, idOrcamento, idEquipe, tiposInstalacao, situacoes, dataIni, 
                dataFim, dataIniEnt, dataFimEnt, dataIniOrdemInst, dataFimOrdemInst, idLoja, idsClientes, telefone, observacao, true, false, false), 
                GetParam(dataIni, dataFim, dataIniEnt, dataFimEnt, null, dataIniOrdemInst, dataFimOrdemInst, telefone));
        }

        private GDAParameter[] GetParam(string dataIni, string dataFim, string dataIniEnt, string dataFimEnt, string nomeCli,
            string dataIniOrdemInst, string dataFimOrdemInst, string telefone)
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

            if (!String.IsNullOrEmpty(nomeCli))
                lstParam.Add(new GDAParameter("?nomeCli", "%" + nomeCli + "%"));

            if (!String.IsNullOrEmpty(dataIniOrdemInst))
                lstParam.Add(new GDAParameter("?dataIniOrdemInst", DateTime.Parse(dataIniOrdemInst + " 00:00")));

            if (!String.IsNullOrEmpty(dataFimOrdemInst))
                lstParam.Add(new GDAParameter("?dataFimOrdemInst", DateTime.Parse(dataFimOrdemInst + " 23:59")));

            if (!String.IsNullOrEmpty(telefone))
                lstParam.Add(new GDAParameter("?telefone", "%" + telefone + "%"));

            return lstParam.Count > 0 ? lstParam.ToArray() : null;
        }

        #endregion

        #region Busca Instalações abertas e canceladas para serem adicionadas

        private string SqlAbertas(uint idCli, uint idPedido, string nomeCli, string dataIniConf, string dataFimConf, uint idLoja, bool selecionar)
        {
            string campos = selecionar ? "i.*, c.Nome as NomeCliente, l.NomeFantasia as NomeLoja, p.DataConf as DataConfPedido, " +
                "Concat(p.EnderecoObra, ' - ', p.BairroObra, ' - ', p.CidadeObra) as LocalObra, '$$$' as Criterio" : "Count(*)";

            string criterio = String.Empty;

            string sql = "Select " + campos + " From instalacao i " +
                "Left Join pedido p On (i.idPedido=p.idPedido) " +
                "Left Join loja l On (p.idLoja=l.idLoja) " +
                "Left Join cliente c On (p.idCli=c.id_Cli) Where (i.Situacao=" + (int)Instalacao.SituacaoInst.Aberta + " Or i.Situacao=" + 
                    (int)Instalacao.SituacaoInst.Cancelada + " Or i.Situacao=" + (int)Instalacao.SituacaoInst.Agendar + " Or i.Situacao=" + ")";

            if (idCli > 0)
                sql += " And p.idCli=" + idCli;
            else if (!String.IsNullOrEmpty(nomeCli))
            {
                string ids = ClienteDAO.Instance.GetIds(null, nomeCli, null, 0, null, null, null, null, 0);
                sql += " And c.id_Cli in (" + ids + ")";
            }

            if (idPedido > 0)
                sql += " And i.idPedido=" + idPedido;
            
            if (!String.IsNullOrEmpty(dataIniConf))
                sql += " And i.DataEntrega>=?dataIni";

            if (!String.IsNullOrEmpty(dataFimConf))
                sql += " And i.DataEntrega<=?dataFim";

            if (idLoja > 0)
                sql += " And p.idLoja=" + idLoja;

            bool instComum = Config.PossuiPermissao(Config.FuncaoMenuInstalacao.ControleInstalacaoComum);
            bool instTemp = Config.PossuiPermissao(Config.FuncaoMenuInstalacao.ControleInstalacaoTemperado);

            if (instComum && !instTemp)
                sql += " And i.TipoInstalacao=1";

            return sql.Replace("$$$", criterio);
        }

        /// <summary>
        /// Método utilizado ao buscar Instalação diretamente pelo idPedido, nas telas Nova Ordem de Instalação
        /// e Retificar Ordem de Instalação
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public IList<Instalacao> GetAbertasByPedido(uint idPedido)
        {
            return objPersistence.LoadData(SqlAbertas(0, idPedido, null, null, null, 0, true)).ToList();
        }

        public IList<Instalacao> GetListAbertas(uint idCli, uint idPedido, string nomeCli, string dataIniConf, string dataFimConf, uint idLoja, string sortExpression, int startRow, int pageSize)
        {
            string sql = SqlAbertas(idCli, idPedido, nomeCli, dataIniConf, dataFimConf, idLoja, true);
            string sort = String.IsNullOrEmpty(sortExpression) ? "i.tipoInstalacao desc, i.DataCad" : sortExpression;

            return LoadDataWithSortExpression(sql, sort, startRow, pageSize, GetParam(dataIniConf, dataFimConf, null, null, nomeCli, null, null, null));
        }

        public int GetCountAbertas(uint idCli, uint idPedido, string nomeCli, string dataIniConf, string dataFimConf, uint idLoja)
        {
            return objPersistence.ExecuteSqlQueryCount(SqlAbertas(idCli, idPedido, nomeCli, dataIniConf, dataFimConf, idLoja, false),
                GetParam(dataIniConf, dataFimConf, null, null, nomeCli, null, null, null));
        }

        #endregion

        #region Busca Instalações em andamento para serem finalizadas

        private string SqlEmAndamento(uint idPedido, uint idOrdemInstalacao, uint idEquipe, string dataIni, string dataFim,
            bool confirmadasPda, uint idCliente, string nomeCliente, bool selecionar)
        {
            string campos = selecionar ? "i.*, group_concat(e.Nome separator '" + SEPARADOR_EQUIPES + "') as NomesEquipes, c.Nome as NomeCliente, l.NomeFantasia as NomeLoja, " +
                "p.DataConf as DataConfPedido, Concat(p.EnderecoObra, ' - ', p.BairroObra, ' - ', p.CidadeObra) as LocalObra, " +
                "'$$$' as Criterio" : "i.idInstalacao";

            string sql = "Select " + campos + " From instalacao i " +
                "Left Join equipe_instalacao ei On (i.idInstalacao=ei.IdInstalacao) " +
                "Left Join equipe e On (ei.idEquipe=e.IdEquipe) " +
                "Left Join pedido p On (i.idPedido=p.idPedido) " +
                "Left Join loja l On (p.idLoja=l.idLoja) " +
                "Left Join cliente c On (p.idCli=c.id_Cli) Where 1 ";

            if (!confirmadasPda)
                sql += " And i.Situacao=" + (int)Instalacao.SituacaoInst.EmAndamento;
            else
                sql += " And (i.Situacao=" + (int)Instalacao.SituacaoInst.Finalizada + " And i.latitude < 0)";

            if (idPedido > 0)
                sql += " And i.idPedido=" + idPedido;

            if (idOrdemInstalacao > 0)
                sql += " And i.idOrdemInstalacao=" + idOrdemInstalacao;

            if (idEquipe > 0)
                sql += " And e.idEquipe=" + idEquipe;

            if (!String.IsNullOrEmpty(dataIni))
                sql += " And i.DataInstalacao>=?dataIni";

            if (!String.IsNullOrEmpty(dataFim))
                sql += " And i.DataInstalacao<=?dataFim";

            if (idCliente > 0)
                sql += " And p.idCli=" + idCliente;
            else if (!String.IsNullOrEmpty(nomeCliente))
            {
                string ids = ClienteDAO.Instance.GetIds(null, nomeCliente, null, 0, null, null, null, null, 0);
                sql += " And c.id_Cli in (" + ids + ")";
            }

            LoginUsuario login = UserInfo.GetUserInfo;

            bool instComum = Config.PossuiPermissao(Config.FuncaoMenuInstalacao.ControleInstalacaoComum);
            bool instTemp = Config.PossuiPermissao(Config.FuncaoMenuInstalacao.ControleInstalacaoTemperado);

            if (instComum && !instTemp)
                sql += " And i.TipoInstalacao in (" + (int)Instalacao.TipoInst.Comum + ", " + (int)Instalacao.TipoInst.Entrega + ")";
            else if (!instTemp)
                sql += " And i.TipoInstalacao in (0)";

            sql += " group by i.idInstalacao";
            if (!selecionar)
                sql = "select count(*) from (" + sql + ") as temp";

            return sql;
        }

        public IList<Instalacao> GetListEmAndamento(uint idPedido, uint idOrdemInstalacao, uint idEquipe, string dataIni, string dataFim, 
            bool confirmadasPda, uint idCliente, string nomeCliente, string sortExpression, int startRow, int pageSize)
        {
            string sql = SqlEmAndamento(idPedido, idOrdemInstalacao, idEquipe, dataIni, dataFim, confirmadasPda, idCliente, nomeCliente, true);
            string sort = String.IsNullOrEmpty(sortExpression) ? "i.DataCad" : sortExpression;

            return LoadDataWithSortExpression(sql, sort, startRow, pageSize, GetParam(dataIni, dataFim, null, null, nomeCliente, null, null, null));
        }

        public int GetCountEmAndamento(uint idPedido, uint idOrdemInstalacao, uint idEquipe, string dataIni, string dataFim, 
            bool confirmadasPda, uint idCliente, string nomeCliente)
        {
            return objPersistence.ExecuteSqlQueryCount(SqlEmAndamento(idPedido, idOrdemInstalacao, idEquipe, dataIni, dataFim,
                confirmadasPda, idCliente, nomeCliente, false), GetParam(dataIni, dataFim, null, null, nomeCliente, null, null, null));
        }

        #endregion

        #region Busca instalação comum para ser liberada e utilizada por equipes temperado

        /// <summary>
        /// Busca instalação comum que possa ser liberada para ser instalada por equipes temperado
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public Instalacao BuscarParaLiberacao(uint idPedido)
        {
            if (idPedido == 0)
                return null;

            string sql = "Select i.*, group_concat(e.Nome separator '" + SEPARADOR_EQUIPES + "') as NomesEquipes, c.Nome as NomeCliente, l.NomeFantasia as NomeLoja, " + 
                "p.DataConf as DataConfPedido, Concat(p.EnderecoObra, ' - ', p.BairroObra, ' - ', p.CidadeObra) as LocalObra " + 
                "From instalacao i " +
                "Left Join equipe_instalacao ei On (i.idInstalacao=ei.IdInstalacao) " +
                "Left Join equipe e On (ei.idEquipe=e.IdEquipe) " +
                "Left Join pedido p On (i.idPedido=p.idPedido) " +
                "Left Join loja l On (p.idLoja=l.idLoja) " +
                "Left Join cliente c On (p.idCli=c.id_Cli) Where i.TipoInstalacao=" + (int)Instalacao.TipoInst.Comum +
                " And i.idPedido=" + idPedido + " group by i.idInstalacao";

            List<Instalacao> lstInst = objPersistence.LoadData(sql);

            if (lstInst.Count == 0)
                throw new Exception("Nenhuma Instalação Comum encontrada para o pedido passado.");

            if (lstInst[0].Situacao != 1 && lstInst[0].Situacao != 4)
                throw new Exception("Apenas instalações comum abertas e canceladas podem ser liberadas para equipes de instalação temperado.");

            return lstInst[0];
        }

        #endregion

        #region Libera instalação para ser executada por equipes de temperado

        /// <summary>
        /// Libera a instalação para ser executada por equipes de temperado
        /// </summary>
        /// <param name="idInstalacao"></param>
        public void LiberaInstalacao(uint idInstalacao)
        {
            // Verifica se instalação já foi liberada
            string sqlVerify = "Select Count(*) From instalacao Where LiberarTemperado=1 And idInstalacao=" + idInstalacao;
            if (objPersistence.ExecuteSqlQueryCount(sqlVerify) == 1)
                throw new Exception("Esta instalação já foi liberada.");

            string sql = "Update instalacao Set LiberarTemperado=1 Where idInstalacao=" + idInstalacao;

            objPersistence.ExecuteCommand(sql);
        }

        #endregion

        #region Busca instalações que o pedido passado possa ter gerado

        /// <summary>
        /// Busca instalações que o pedido passado possa ter gerado
        /// </summary>
        public IList<Instalacao> GetByPedido(uint idPedido)
        {
            return GetByPedido(null, idPedido);
        }

        /// <summary>
        /// Busca instalações que o pedido passado possa ter gerado
        /// </summary>
        public IList<Instalacao> GetByPedido(GDASession session, uint idPedido)
        {
            return GetByVariosPedidos(session, idPedido.ToString());
        }

        public IList<Instalacao> GetByVariosPedidos(string idsPedidos)
        {
            return GetByVariosPedidos(null, idsPedidos);
        }

        public IList<Instalacao> GetByVariosPedidos(GDASession session, string idsPedidos)
        {
            string sql = "Select * From instalacao where idPedido in (" + idsPedidos.Trim(' ', ',') + ")";
            return objPersistence.LoadData(session, sql).ToList();
        }

        #endregion

        #region Verifica se a Ordem de Instalação existe

        /// <summary>
        /// Verifica se a Ordem de Instalação existe
        /// </summary>
        /// <param name="idOrdemInst"></param>
        public bool OrdemInstExists(uint idOrdemInst)
        {
            string sql = "Select Count(*) From instalacao Where idOrdemInstalacao=" + idOrdemInst;

            return objPersistence.ExecuteSqlQueryCount(sql, null) > 0;
        }

        #endregion

        #region Insere uma nova instalação

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// Insere uma nova instalação
        /// </summary>
        /// <param name="idPedido"></param>
        /// <param name="tipoInstalacao">Verificar propriedade TipoInstalacao na model</param>
        /// <param name="ignorarConfig">A instalação deve ser gerada, mesmo automaticamente (ignorando o PedidoConfig.Instalacao.GerarInstalacaoAutomaticamente)?</param>
        public uint NovaInstalacao(uint idPedido, DateTime dataEntrega, int tipoInstalacao, bool ignorarConfig)
        {
            return NovaInstalacao(null, idPedido, dataEntrega, tipoInstalacao, ignorarConfig);
        }

        /// <summary>
        /// Insere uma nova instalação
        /// </summary>
        /// <param name="idPedido"></param>
        /// <param name="tipoInstalacao">Verificar propriedade TipoInstalacao na model</param>
        /// <param name="ignorarConfig">A instalação deve ser gerada, mesmo automaticamente (ignorando o PedidoConfig.Instalacao.GerarInstalacaoAutomaticamente)?</param>
        public uint NovaInstalacao(GDASession sessao, uint idPedido, DateTime dataEntrega, int tipoInstalacao, bool ignorarConfig)
        {
            if (!PedidoConfig.Instalacao.GerarInstalacaoAutomaticamente && !ignorarConfig)
                return 0;
            
            Instalacao inst = new Instalacao();
            inst.IdPedido = idPedido;
            inst.TipoInstalacao = tipoInstalacao;
            inst.Situacao = (int)Instalacao.SituacaoInst.Aberta;
            inst.LiberarTemperado = false;
            inst.DataEntrega = dataEntrega;

            return Insert(sessao, inst);
        }

        #endregion

        #region Nova Ordem de Instalação

        /// <summary>
        /// Cria uma nova Ordem de Instalação
        /// </summary>
        /// <param name="idsInstalacao"></param>
        /// <param name="dataInstalacao"></param>
        /// <param name="idEquipe"></param>
        public uint NovaOrdemInstalacao(string idsInstalacao, DateTime dataInstalacao, int tipoInstalacao, string idsEquipes, string idsProdutos, string obs)
        {
            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    LoginUsuario login = UserInfo.GetUserInfo;

                    if (!Config.PossuiPermissao(Config.FuncaoMenuInstalacao.ControleInstalacaoComum) && !Config.PossuiPermissao(Config.FuncaoMenuInstalacao.ControleInstalacaoTemperado))
                        throw new Exception("Você não tem permissão para criar ordens de instalação.");

                    if (PedidoConfig.Instalacao.UsarAmbienteInstalacao && String.IsNullOrEmpty(idsProdutos))
                        throw new Exception("Selecione os produtos antes de gerar a ordem de instalação.");

                    // Altera o tipo das instalações para o tipo selecionado
                    if (tipoInstalacao > 0)
                        objPersistence.ExecuteCommand(transaction, "Update instalacao Set tipoInstalacao=" + tipoInstalacao + " Where idInstalacao In (" + idsInstalacao.TrimEnd(',') + ")");

                    // Verifica se há alguma instalação temperada
                    string sqlInstalacaoTemperada = "select count(*) from instalacao where tipoInstalacao=2 and idInstalacao in (" + idsInstalacao.TrimEnd(',') + ")";
                    if (objPersistence.ExecuteSqlQueryCount(transaction, sqlInstalacaoTemperada) > 0)
                        foreach (string e in idsEquipes.Split(','))
                            if (EquipeDAO.Instance.ObtemValorCampo<int>(transaction, "tipo", "idEquipe=" + e) == 1)
                                throw new Exception("Apenas equipes do tipo 'Colocação Temperado' podem fazer instalações de colocação temperada.");

                    // Obtém um novo idOrdemInstalacao
                    string sqlNovoIdOrdemInst = "Select Coalesce(Max(idOrdemInstalacao)+1, 1) From instalacao";
                    uint novoIdOrdemInstalacao = Glass.Conversoes.StrParaUint(objPersistence.ExecuteScalar(transaction, sqlNovoIdOrdemInst, null).ToString());

                    string sqlNovaOrdem = "Update instalacao set situacao=" + (int)Instalacao.SituacaoInst.EmAndamento + ", obs=?obs, idOrdemInstalacao=" + novoIdOrdemInstalacao +
                        ", DataInstalacao=?dataInst, dataOrdemInstalacao=now() Where idInstalacao In (" + idsInstalacao.TrimEnd(',') + ")";

                    objPersistence.ExecuteCommand(transaction, sqlNovaOrdem, new GDAParameter("?dataInst", dataInstalacao), new GDAParameter("?obs", obs));

                    foreach (string i in idsInstalacao.TrimEnd(',').Split(','))
                        foreach (string e in idsEquipes.TrimEnd(',').Split(','))
                        {
                            if (String.IsNullOrEmpty(i) || String.IsNullOrEmpty(e))
                                continue;

                            EquipeInstalacao ei = new EquipeInstalacao();
                            ei.IdOrdemInstalacao = novoIdOrdemInstalacao;
                            ei.IdInstalacao = Glass.Conversoes.StrParaUint(i);
                            ei.IdEquipe = Glass.Conversoes.StrParaUint(e);

                            EquipeInstalacaoDAO.Instance.Insert(transaction, ei);
                        }

                    // Apaga os produtos de uma instalação
                    ProdutosInstalacaoDAO.Instance.DeleteByInstalacoes(transaction, idsInstalacao.TrimEnd(','));

                    // Recupera todos os ambientes, se a empresa não trabalhar com ambiente na instalação
                    if (!PedidoConfig.Instalacao.UsarAmbienteInstalacao)
                    {
                        idsProdutos = "";
                        foreach (Instalacao i in GetByOrdemInst(novoIdOrdemInstalacao))
                        {
                            List<string> produtosAmbiente = new List<string>();
                            foreach (ProdutosPedido p in ProdutosPedidoDAO.Instance.GetByPedidoLite(transaction, i.IdPedido))
                                produtosAmbiente.Add(p.IdProdPed.ToString());

                            idsProdutos += i.IdInstalacao + ";" + i.IdPedido + ";" + String.Join(",", produtosAmbiente.ToArray()) + "|";
                        }
                    }

                    // Cadastra os produtos para a instalação
                    string[] dados = idsProdutos.TrimEnd('|').Split('|');
                    foreach (string s in dados)
                    {
                        if (String.IsNullOrEmpty(s))
                            continue;

                        uint idInstalacao = Glass.Conversoes.StrParaUint(s.Split(';')[0]);
                        uint idPedido = Glass.Conversoes.StrParaUint(s.Split(';')[1]);
                        string[] produtos = s.Split(';')[2].TrimEnd(',').Split(',');

                        if (PedidoConfig.Instalacao.UsarAmbienteInstalacao && (produtos.Length == 0 || produtos.Any(f => string.IsNullOrEmpty(f))))
                            throw new Exception(string.Format("Nenhum produto foi selecionado para o pedido {0}", idPedido));

                        foreach (string p in produtos)
                        {
                            if (String.IsNullOrEmpty(p))
                                continue;

                            ProdutosInstalacao novo = new ProdutosInstalacao();
                            novo.IdInstalacao = idInstalacao;
                            novo.IdPedido = idPedido;
                            novo.IdProdPed = Glass.Conversoes.StrParaUint(p);

                            ProdutosInstalacaoDAO.Instance.Insert(transaction, novo);
                        }
                    }

                    transaction.Commit();
                    transaction.Close();

                    return novoIdOrdemInstalacao;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    transaction.Close();

                    ErroDAO.Instance.InserirFromException("NovaOrdemInstalacao", ex);
                    throw ex;
                }
            }
        }

        #endregion

        #region Retificar Ordem de Instalação

        public void RetificarOrdemInst(uint idOrdemInst, string idsInstalacao, string idsEquipes, DateTime dataInstalacao, string obs)
        {
            // Retira as Instalações da Ordem de Instalação que está sendo retificada
            objPersistence.ExecuteCommand("Update instalacao set situacao=" + (int)Instalacao.SituacaoInst.Aberta + ", idOrdemInstalacao=null," +
                " DataInstalacao=null, UsuFinal=null, DataFinal=null, dataOrdemInstalacao=null Where situacao<>" + (int)Instalacao.SituacaoInst.Finalizada + 
                " And idOrdemInstalacao=" + idOrdemInst + "; delete from equipe_instalacao where idOrdemInstalacao=" + idOrdemInst, null);

            // Coloca Instalações na Ordem de Instalação que está sendo retificada
            // Retira as Instalações da Ordem de Instalação que está sendo retificada
            objPersistence.ExecuteCommand("Update instalacao set situacao=" + (int)Instalacao.SituacaoInst.EmAndamento + ", idOrdemInstalacao=" + idOrdemInst +
                ", DataInstalacao=?dataInst, dataOrdemInstalacao=now(), obs=?obs Where idInstalacao In (" + idsInstalacao.TrimEnd(',') + ")",
                new GDAParameter("?dataInst", dataInstalacao), new GDAParameter("?obs", obs));

            foreach (string i in idsInstalacao.TrimEnd(',').Split(','))
                foreach (string e in idsEquipes.TrimEnd(',').Split(','))
                {
                    if (String.IsNullOrEmpty(i) || String.IsNullOrEmpty(e))
                        continue;

                    EquipeInstalacao ei = new EquipeInstalacao();
                    ei.IdOrdemInstalacao = idOrdemInst;
                    ei.IdInstalacao = Glass.Conversoes.StrParaUint(i);
                    ei.IdEquipe = Glass.Conversoes.StrParaUint(e);

                    EquipeInstalacaoDAO.Instance.Insert(ei);
                }
        }

        #endregion

        #region Cancelar Instalação

        /// <summary>
        /// Cancela a instalação
        /// </summary>
        public void Cancelar(uint idInstalacao)
        {
            Cancelar(null, idInstalacao);
        }

        /// <summary>
        /// Cancela a instalação
        /// </summary>
        public void Cancelar(GDASession session, uint idInstalacao)
        {
            lock (_instalacaoLock)
            {
                string sql = "Update instalacao set situacao=" + (int)Instalacao.SituacaoInst.Cancelada + ", idOrdemInstalacao=null, " +
                    "DataInstalacao=null, UsuFinal=null, DataFinal=null Where idInstalacao=" + idInstalacao + "; " +
                    "delete from equipe_instalacao where idInstalacao=" + idInstalacao;

                objPersistence.ExecuteCommand(session, sql);

            	// Atualiza a situação da produção do pedido
            	PedidoDAO.Instance.AtualizaSituacaoProducao(session, ObtemIdPedido(session, idInstalacao), null, DateTime.Now);

            	LogCancelamentoDAO.Instance.LogInstalacao(session, GetElementByPrimaryKey(session, idInstalacao), "Cancelamento de instalação", true);

            	// Atualiza a situação da produção do pedido
            	PedidoDAO.Instance.AtualizaSituacaoProducao(session, ObtemIdPedido(session, idInstalacao), null, DateTime.Now);
			}
        }

        /// <summary>
        /// Cancela instalação informando observação
        /// </summary>
        /// <param name="idInstalacao"></param>
        /// <param name="obs"></param>
        public void Cancelar(uint idInstalacao, string obs)
        {
            lock (_instalacaoLock)
            {
                LoginUsuario login = UserInfo.GetUserInfo;

                if (!Config.PossuiPermissao(Config.FuncaoMenuInstalacao.ControleInstalacaoComum) &&
                    !Config.PossuiPermissao(Config.FuncaoMenuInstalacao.ControleInstalacaoTemperado))
                    throw new Exception("Você não tem permissão para cancelar instalações.");

                try
                {
                    // Altera situação da instalação para continuada
                    objPersistence.ExecuteCommand("Update instalacao Set situacao=" + (int)Instalacao.SituacaoInst.Cancelada +
                        ", obs=?obs Where idInstalacao=" + idInstalacao, new GDAParameter[] { new GDAParameter("?obs", obs) });
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }

                PedidoDAO.Instance.AtualizaSituacaoProducao(null, ObtemIdPedido(idInstalacao), null, DateTime.Now);

                LogCancelamentoDAO.Instance.LogInstalacao(GetElementByPrimaryKey(idInstalacao), "Cancelamento de instalação", true);
            }
        }

        public bool PossuiIntalacoesNaoCanceladas(GDASession sessao, uint idLiberarPedido)
        {
            return objPersistence.ExecuteSqlQueryCount(sessao, $@"SELECT COUNT(*)
                FROM instalacao 
                WHERE 
                    Situacao<>{(int)Instalacao.SituacaoInst.Cancelada}
                    AND IdPedido IN (SELECT IdPedido 
                                     FROM produtos_liberar_pedido 
                                     WHERE IdLiberarPedido={idLiberarPedido} AND QtdeCalc>0);") > 0;
        }


        /// <summary>
        /// Cancela instalação que já foi finalizada informando observação
        /// </summary>
        /// <param name="idInstalacao"></param>
        /// <param name="obs"></param>
        public void CancelarFinalizada(uint idInstalacao, string obs)
        {
            lock (_instalacaoLock)
            {
                LoginUsuario login = UserInfo.GetUserInfo;

                if (!Config.PossuiPermissao(Config.FuncaoMenuInstalacao.ControleInstalacaoComum) &&
                    !Config.PossuiPermissao(Config.FuncaoMenuInstalacao.ControleInstalacaoTemperado))
                    throw new Exception("Você não tem permissão para cancelar instalações.");

                try
                {
                    //Registra log de cancelamento
                    LogCancelamentoDAO.Instance.LogFinalizacaoInstalacao(InstalacaoDAO.Instance.GetElement(idInstalacao), obs, true);

                    // Altera situação da instalação para cencelada
                    objPersistence.ExecuteCommand("Update instalacao Set situacao=" + (int)Instalacao.SituacaoInst.Cancelada +
                        ", obs=?obs, usuFinal=Null, dataFinal=Null Where idInstalacao=" + idInstalacao, new GDAParameter[] { new GDAParameter("?obs", obs) });
                    objPersistence.ExecuteCommand("Delete From produtos_instalacao Where idInstalacao=" + idInstalacao);
                    objPersistence.ExecuteCommand("Delete From func_instalacao Where idInstalacao=" + idInstalacao);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }

                PedidoDAO.Instance.AtualizaSituacaoProducao(null, ObtemIdPedido(idInstalacao), null, DateTime.Now);
            }
        }

        #endregion

        #region Continuar/Finalizar Instalação

        /// <summary>
        /// Continua/Finaliza a instalação passada.
        /// </summary>
        public void ContinuarFinalizar(int idInstalacao, List<ProdutosInstalacao> lstProdInst, string obs)
        {
            lock (_instalacaoLock)
            {
                using (var transaction = new GDATransaction())
                {
                    
                    try
                    {
                        transaction.BeginTransaction();

                        // Insere cada um dos produtos marcados como instalados.
                        ProdutosInstalacaoDAO.Instance.InsereProdutoInstalado(transaction, idInstalacao, lstProdInst);

                        if (ProdutosInstalacaoDAO.Instance.VerificaFinalizarInst(transaction, lstProdInst.Select(f => f.IdPedido).ToList()[0]))
                            Finalizar(transaction, idInstalacao);
                        else
                            Continuar(transaction, idInstalacao, obs);

                        transaction.Commit();
                        transaction.Close();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        transaction.Close();
                        ErroDAO.Instance.InserirFromException(string.Format("Finalizar/Continuar - Instalação: {0}", idInstalacao), ex);
                        throw;
                    }
                }
            }
        }

        #region Finalizar

        /// <summary>
        /// Continua a instalação passada.
        /// </summary>
        public void FinalizarComTransacao(int idInstalacao)
        {
            lock (_instalacaoLock)
            {
                using (var transaction = new GDATransaction())
                {
                    try
                    {
                        transaction.BeginTransaction();
                        // Busca os produtos com as quantidades já instaladas
                        var lstProdPed = ProdutosPedidoDAO.Instance.GetListInst(transaction, (uint)idInstalacao);

                        var lstProdInst = new List<ProdutosInstalacao>();
                        ProdutosInstalacao prodInst;

                        // Adiciona produto à uma lista temporária para ser inserido depois
                        foreach (var pp in lstProdPed)
                        {
                            prodInst = new ProdutosInstalacao();
                            prodInst.IdInstalacao = (uint)idInstalacao;
                            prodInst.IdPedido = pp.IdPedido;
                            prodInst.IdProdPed = pp.IdProdPed;
                            prodInst.QtdeInstalada = (int)pp.Qtde - Convert.ToInt32(pp.QtdeInstalada);
                            lstProdInst.Add(prodInst);
                        }

                        // Insere produtos instalacao, com a quantidade que foram instaladas
                        // (neste caso todos os produtos que ainda não foram instalados)
                        ProdutosInstalacaoDAO.Instance.InsereProdutoInstalado(transaction, idInstalacao, lstProdInst);

                        Finalizar(transaction, idInstalacao);

                        transaction.Commit();
                        transaction.Close();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        transaction.Close();
                        ErroDAO.Instance.InserirFromException(string.Format("Finalizar - Instalação: {0}", idInstalacao), ex);
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// Finaliza a instalação passada.
        /// </summary>
        public void Finalizar(GDASession session, int idInstalacao)
        {
            LoginUsuario login = UserInfo.GetUserInfo;

            if (!Config.PossuiPermissao(Config.FuncaoMenuInstalacao.ControleInstalacaoComum) &&
                !Config.PossuiPermissao(Config.FuncaoMenuInstalacao.ControleInstalacaoTemperado))
                throw new Exception("Você não tem permissão para finalizar instalações.");

            DateTime dataFinal = DateTime.Now;
            objPersistence.ExecuteCommand(session, @"update instalacao set usuFinal=?usu, dataFinal=?data, situacao=?s
                where idInstalacao=" + idInstalacao, new GDAParameter("?usu", login.CodUser),
                    new GDAParameter("?data", dataFinal), new GDAParameter("?s", (int)Instalacao.SituacaoInst.Finalizada));
            
            // Apaga, se houver, historico de funcionario para esta instalacao, para evitar duplicacao
            objPersistence.ExecuteCommand(session, "Delete From func_instalacao Where idInstalacao=" + idInstalacao);

            // Monta SQL para salvar os funcionários da equipe desta instalação na tabela de histórico
            var sqlFuncHist = @"Insert Into func_instalacao (IDFUNC, IDINSTALACAO) 
                (Select distinct fe.IdFunc, " + idInstalacao + @" From func_equipe fe Where fe.idEquipe In 
                (Select IdEquipe From equipe_instalacao Where idInstalacao=" + idInstalacao + "))";            

            objPersistence.ExecuteCommand(session, sqlFuncHist);

            // Atualiza a situação da produção do pedido
            PedidoDAO.Instance.AtualizaSituacaoProducao(session, ObtemIdPedido(session, (uint)idInstalacao), null, dataFinal, true);
        }

        #endregion

        #region Continuar

        /// <summary>
        /// Continua a instalação passada.
        /// </summary>
        public void ContinuarComTransacao(int idInstalacao, string obs)
        {
            lock (_instalacaoLock)
            {
                using (var transaction = new GDATransaction())
                {
                    try
                    {
                        transaction.BeginTransaction();
                        Continuar(transaction, idInstalacao, obs);
                        transaction.Commit();
                        transaction.Close();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        transaction.Close();
                        ErroDAO.Instance.InserirFromException(string.Format("Continuar - Instalação: {0}", idInstalacao), ex);
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// Continua a instalação passada.
        /// </summary>
        public void Continuar(GDASession session, int idInstalacao, string obs)
        {
            LoginUsuario login = UserInfo.GetUserInfo;

            if (!Config.PossuiPermissao(Config.FuncaoMenuInstalacao.ControleInstalacaoComum) &&
                !Config.PossuiPermissao(Config.FuncaoMenuInstalacao.ControleInstalacaoTemperado))
                throw new Exception("Você não tem permissão para dar continuidade à instalações.");

            // Busca instalação que está sendo continuada
            uint idPedido = ObtemIdPedido(session, (uint)idInstalacao);
            DateTime dataEntrega = ObtemValorCampo<DateTime>(session, "dataEntrega", "idInstalacao=" + idInstalacao);
            int tipoInstalacao = ObtemValorCampo<int>(session, "tipoInstalacao", "idInstalacao=" + idInstalacao);

                // Cria uma nova instalação a partir da instalação que está sendo continuada
            uint idNovaInst = NovaInstalacao(session, idPedido, dataEntrega, tipoInstalacao, true);

            // Se a nova instalação não tiver sido criada, tenta mais uma vez
            if (idNovaInst == 0)
            idNovaInst = NovaInstalacao(session, idPedido, dataEntrega, tipoInstalacao, true);

            if (idNovaInst == 0)
                throw new Exception("Falha ao cadastrar nova instalação. Banco de dados ocupado, tente novamente.");

            // Altera situação da instalação para continuada
            int retorno = objPersistence.ExecuteCommand(session, "Update instalacao Set situacao=" + (int)Instalacao.SituacaoInst.Continuada +
                ", obs=?obs Where idInstalacao=" + idInstalacao, new GDAParameter[] { new GDAParameter("?obs", obs) });
        }

        #endregion

        #endregion

        #region Busca instalações finalizadas por pedido

        private string SqlFinalizadas(uint idCli, string idPedido, string idsPedidos, string nomeCli, string dataIniConf, string dataFimConf,
            uint idLoja, bool incluirJoin, bool incluirContinuadas, bool selecionar)
        {
            return SqlFinalizadas(null, idCli, idPedido, idsPedidos, nomeCli, dataIniConf, dataFimConf, idLoja, incluirJoin,
                incluirContinuadas, selecionar);
        }

        private string SqlFinalizadas(GDASession session, uint idCli, string idPedido, string idsPedidos, string nomeCli, string dataIniConf,
            string dataFimConf, uint idLoja, bool incluirJoin, bool incluirContinuadas, bool selecionar)
        {
            string campos = selecionar ? "i.*, c.Nome as NomeCliente, l.NomeFantasia as NomeLoja, p1.DataConf as DataConfPedido, " +
                "Concat(p1.EnderecoObra, ' - ', p1.BairroObra, ' - ', p1.CidadeObra) as LocalObra, '$$$' as Criterio" : "Count(*)";

            string criterio = String.Empty;

            string sql = "Select " + campos + " From instalacao i ";

            if (incluirJoin || selecionar)
                sql += "Left Join pedido p1 On (i.idPedido=p1.idPedido) " +
                    "Left Join loja l On (p1.idLoja=l.idLoja) " +
                    "Left Join cliente c On (p1.idCli=c.id_Cli) ";
            
            sql += "Where i.Situacao in (" + (int)Instalacao.SituacaoInst.Finalizada + (incluirContinuadas ? "," + (int)Instalacao.SituacaoInst.Continuada : "") + @")
                 and i.tipoInstalacao<>" + (int)Instalacao.TipoInst.Entrega;

            if (idCli > 0)
                sql += " And p.idCli=" + idCli;
            else if (!String.IsNullOrEmpty(nomeCli))
            {
                string ids = ClienteDAO.Instance.GetIds(session, null, nomeCli, null, 0, null, null, null, null, 0);
                sql += " And c.id_Cli in (" + ids + ")";
            }

            if (!String.IsNullOrEmpty(idPedido) && idPedido != "0")
                sql += " And i.idPedido=" + idPedido;
            else if (!String.IsNullOrEmpty(idsPedidos))
                sql += " and i.idPedido in (" + idsPedidos + ")";

            if (!String.IsNullOrEmpty(dataIniConf))
                sql += " And i.DataEntrega>=?dataIni";

            if (!String.IsNullOrEmpty(dataFimConf))
                sql += " And i.DataEntrega<=?dataFim";

            if (idLoja > 0)
                sql += " And p1.idLoja=" + idLoja;

            LoginUsuario login = UserInfo.GetUserInfo;

            bool instComum = Config.PossuiPermissao(Config.FuncaoMenuInstalacao.ControleInstalacaoComum);
            bool instTemp = Config.PossuiPermissao(Config.FuncaoMenuInstalacao.ControleInstalacaoTemperado);

            if (instComum && !instTemp)
                sql += " And i.TipoInstalacao=1";
            //else if (login.TipoUsuario == (uint)Utils.TipoFuncionario.SupervisorColocacaoTemperado)
            //    sql += " And (i.TipoInstalacao=2 Or i.LiberarTemperado=1)";

            return sql.Replace("$$$", criterio);
        }

        public IList<Instalacao> GetFinalizadasByPedido(uint idPedido)
        {
            return objPersistence.LoadData(SqlFinalizadas(0, idPedido.ToString(), null, null, null, null, 0, true, false, true)).ToList();
        }

        public int GetCountFinalizadasByPedido(uint idPedido)
        {
            return objPersistence.ExecuteSqlQueryCount(SqlFinalizadas(0, idPedido.ToString(), null, null, null, null, 0, true, false, false));
        }

        public IList<Instalacao> GetFinalizadasByVariosPedidos(string idsPedidos)
        {
            return objPersistence.LoadData(SqlFinalizadas(0, null, idsPedidos, null, null, null, 0, true, false, true)).ToList();
        }

        internal string SqlFinalizadaByPedido(string idPedido)
        {
            return SqlFinalizadaByPedido(null, idPedido, true);
        }

        internal string SqlFinalizadaByPedido(GDASession session, string idPedido, bool incluirSelect)
        {
            string sqlCount = Sql(session, 0, idPedido, 0, 0, 0, null, null, null, null, null, null, null, null, 0, null, null, null, false, true, false);
            string sqlFinalizadas = SqlFinalizadas(session, 0, idPedido, null, null, null, null, 0, false, true, false);

            var sql = "if((" + sqlCount + ")>0, (" + sqlCount + ")=(" + sqlFinalizadas + "), false)";
            return incluirSelect ? "select " + sql : sql;
        }

        public bool IsFinalizadaByPedido(uint idPedido)
        {
            return IsFinalizadaByPedido(null, idPedido);
        }

        public bool IsFinalizadaByPedido(GDASession session, uint idPedido)
        {
            object retorno = objPersistence.ExecuteScalar(session, SqlFinalizadaByPedido(session, idPedido.ToString(), true));
            return retorno != null & retorno.ToString() != "" ? Convert.ToBoolean(retorno) : false;
        }

        #endregion

        #region Atualiza data de entrega

        public int AtualizaDataEntregaSituacao(Instalacao objUpdate)
        {
            string sql = "Update instalacao Set dataEntrega=?dataEntrega";

            if (objUpdate.TipoInstalacao > 0)
                sql += ", tipoInstalacao=" + objUpdate.TipoInstalacao;
            
            sql += " Where idInstalacao=" + objUpdate.IdInstalacao;

            return objPersistence.ExecuteCommand(sql, new GDAParameter[] { new GDAParameter("?dataEntrega", objUpdate.DataEntrega) });
        }

        #endregion

        #region Verifica se a instalação já está cadastrada

        /// <summary>
        /// Verifica se há alguma instalação cadastrada para um pedido/tipo.
        /// </summary>
        /// <param name="idPedido"></param>
        /// <param name="tipoInstalacao"></param>
        /// <returns></returns>
        public bool ExisteAbertaByPedidoTipo(uint idPedido, int tipoInstalacao)
        {
            string sql = "select count(*) from instalacao where idPedido=" + idPedido + " and tipoInstalacao=" + tipoInstalacao + " and situacao in (1)";
            return objPersistence.ExecuteSqlQueryCount(sql) > 0;
        }

        #endregion

        public uint ObtemIdPedido(uint idInstalacao)
        {
            return ObtemIdPedido(null, idInstalacao);
        }

        public uint ObtemIdPedido(GDASession session, uint idInstalacao)
        {
            return ObtemValorCampo<uint>(session, "idPedido", "idInstalacao=" + idInstalacao);
        }
    }
}
