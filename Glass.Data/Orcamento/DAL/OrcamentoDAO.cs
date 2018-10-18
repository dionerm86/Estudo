// <copyright file="OrcamentoDAO.cs" company="Sync Softwares">
// Copyright (c) Sync Softwares. Todos os direitos reservados.
// </copyright>

using GDA;
using Glass.Configuracoes;
using Glass.Data.Helper;
using Glass.Data.Helper.Calculos;
using Glass.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Data.DAL
{
    public sealed class OrcamentoDAO : BaseCadastroDAO<Orcamento, OrcamentoDAO>
    {
        // Variável de controle do método UpdateTotaisOrcamento.
        private static Dictionary<uint, bool> atualizando = new Dictionary<uint, bool>();

        #region Busca orçamentos

        private string Sql(uint idOrca, uint idLoja, uint idCliente, string nomeCliente, uint idFunc, string telefone,
            uint idCidade, string endereco, string complemento, string bairro, IEnumerable<int> situacao, DateTime? dataInicio, DateTime? dataFim,
            bool selecionar, out bool temFiltro)
        {
            temFiltro = false;

            StringBuilder campos = new StringBuilder();
            if (!selecionar)
                campos.Append("Count(*)");
            else
                campos.Append(@"o.*, f.Nome as NomeFuncionario, l.NomeFantasia as NomeLoja, c.Nome as NomeComissionado,
                    fCad.nome as descrUsuCad, fAlt.nome as descrUsuAlt, (select cast(group_concat(distinct idItemProjeto) as char)
                    as idItensProjeto from produtos_orcamento where idOrcamento=o.idOrcamento) as idItensProjeto");

            StringBuilder sql = new StringBuilder("Select ");
            sql.Append(campos);
            sql.Append(@" From orcamento o
                    Left Join cliente cl ON (o.idCliente=cl.id_cli)
                    Left Join funcionario f On o.IdFunc=f.idFunc
                    Left Join funcionario fCad On (o.usuCad=fCad.idFunc)
                    Left Join funcionario fAlt On (o.usuAlt=fAlt.idFunc)
                    LEFT JOIN comissionado c ON (o.IdComissionado=c.IdComissionado)
                    LEFT JOIN loja l ON (o.IdLoja=l.IdLoja)
                Where 1 ");

            if (idOrca > 0)
            {
                sql.Append(" And o.IdOrcamento=");
                sql.Append(idOrca);
                temFiltro = true;
            }

            if (idLoja > 0)
            {
                sql.Append(" and o.IdLoja=" + idLoja);
                temFiltro = true;
            }

            if (idCliente > 0)
            {
                sql.Append(" And o.idCliente=" + idCliente);
                temFiltro = true;
            }

            else if (!String.IsNullOrEmpty(nomeCliente))
            {
                sql.Append(" And o.NomeCliente Like ?cliente ");
                temFiltro = true;
            }

            if (situacao != null && situacao.Count() > 0 &&
                (situacao.ToList()[0] > 0 || situacao.Count() > 1))
            {
                sql.Append(string.Format(" And o.Situacao IN ({0})", string.Join(",",situacao)));
                temFiltro = true;
            }

            if (idFunc > 0)
            {
                sql.Append(" And f.idFunc=");
                sql.Append(idFunc);
                temFiltro = true;
            }

            if (!String.IsNullOrEmpty(telefone))
            {
                sql.Append(" And o.TelCliente Like ?telefone ");
                temFiltro = true;
            }

            if (idCidade > 0)
            {
                sql.Append(" And o.idCliente In (Select id_cli From cliente Where idCidade=" + idCidade + ")");
                temFiltro = true;
            }

            if (!String.IsNullOrEmpty(endereco))
            {
                sql.Append(" And o.Endereco Like ?endereco ");
                temFiltro = true;
            }

            if (!string.IsNullOrEmpty(complemento))
            {
                sql.Append(" AND cl.Compl LIKE ?complemento");
                temFiltro = true;
            }

            if (!String.IsNullOrEmpty(bairro))
            {
                sql.Append(" And o.Bairro Like ?bairro ");
                temFiltro = true;
            }

            if (OrcamentoConfig.DadosOrcamento.ListaApenasOrcamentosVendedor && UserInfo.GetUserInfo.TipoUsuario == (uint)Utils.TipoFuncionario.Vendedor)
            {
                sql.Append(" And o.IdFunc=");
                sql.Append(UserInfo.GetUserInfo.CodUser);
                temFiltro = true;
            }

            if (dataInicio > DateTime.MinValue)
            {
                sql.Append(" AND o.DataCad >=?dataInicio");
                temFiltro = true;
            }

            if (dataFim > DateTime.MinValue)
            {
                sql.Append(" AND o.DataCad <=?dataFim");
                temFiltro = true;
            }

            return sql.ToString();
        }

        public Orcamento GetElement(uint idOrca)
        {
            return GetElement(null, idOrca);
        }

        public Orcamento GetElement(GDASession session, uint idOrca)
        {
            bool temFiltro;
            return objPersistence.LoadOneData(session, Sql(idOrca, 0, 0, null, 0, null, 0, null, null, null, null,null, null, true, out temFiltro));
        }

        public IList<Orcamento> GetList(uint idOrca, uint idLoja, uint idCliente, string nomeCliente, uint idFunc, string telefone,
            uint idCidade, string endereco, string complemento, string bairro, IEnumerable<int> situacao, DateTime? dataInicio, DateTime? dataFim, string sortExpression, int startRow, int pageSize)
        {
            sortExpression = String.IsNullOrEmpty(sortExpression) ? "o.IdOrcamento desc" : sortExpression;

            bool temFiltro;
            return LoadDataWithSortExpression(Sql(idOrca, idLoja, idCliente, nomeCliente, idFunc, telefone, idCidade, endereco,
                complemento, bairro, situacao, dataInicio, dataFim, true, out temFiltro), sortExpression, startRow, pageSize, temFiltro,
                GetParameters(nomeCliente, telefone, endereco, complemento, bairro, dataInicio, dataFim));
        }

        public int GetCount(uint idOrca, uint idLoja, uint idCliente, string nomeCliente, uint idFunc, string telefone,
            uint idCidade, string endereco, string complemento, string bairro, IEnumerable<int> situacao, DateTime? dataInicio, DateTime? dataFim)
        {
            return GetCount(null, idOrca, idLoja, idCliente, nomeCliente, idFunc, telefone, idCidade, endereco,
                complemento, bairro, situacao, dataInicio, dataFim);
        }

        public int GetCount(GDASession session, uint idOrca, uint idLoja, uint idCliente, string nomeCliente, uint idFunc, string telefone,
            uint idCidade, string endereco, string complemento, string bairro, IEnumerable<int> situacao, DateTime? dataInicio, DateTime? dataFim)
        {
            bool temFiltro;
            return GetCountWithInfoPaging(session, Sql(idOrca, idLoja, idCliente, nomeCliente, idFunc, telefone, idCidade, endereco, complemento,
                bairro, situacao, dataInicio, dataFim, true, out temFiltro), temFiltro, null, GetParameters(nomeCliente, telefone, endereco, complemento, bairro, dataInicio, dataFim));
        }

        public uint GetIdLoja(GDASession sessao, uint idOrca)
        {
            return ObtemValorCampo<uint>(sessao, "idLoja", "idOrcamento=" + idOrca);
        }

        public int GetCount(uint idOrca)
        {
            return GetCount(null, idOrca);
        }

        public int GetCount(GDASession session, uint idOrca)
        {
            return GetCount(session, idOrca, 0, 0, null, 0, null, 0, null, null, null, null,null, null);
        }

        /// <summary>
        /// Verifica se o produto orcamento possui filhos
        /// </summary>
        public bool ProdutoOrcamentoPossuiFilhos(GDASession session, int idProd)
        {
            return ExecuteScalar<int>(session, string.Format("SELECT COUNT(*) FROM produtos_orcamento WHERE IdProdParent = {0}", idProd)) > 0;
        }

        private GDAParameter[] GetParameters(string cliente, string telefone, string endereco, string complemento, string bairro, DateTime? dataInicio, DateTime? dataFim)
        {
            List<GDAParameter> lstParam = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(cliente))
                lstParam.Add(new GDAParameter("?cliente", "%" + cliente + "%"));

            if (!String.IsNullOrEmpty(telefone))
                lstParam.Add(new GDAParameter("?telefone", "%" + telefone + "%"));

            if (!String.IsNullOrEmpty(endereco))
                lstParam.Add(new GDAParameter("?endereco", "%" + endereco + "%"));

            if (!string.IsNullOrEmpty(complemento))
                lstParam.Add(new GDAParameter("?complemento", "%" + complemento + "%"));

            if (!String.IsNullOrEmpty(bairro))
                lstParam.Add(new GDAParameter("?bairro", "%" + bairro + "%"));

            if (dataInicio > DateTime.MinValue)
                lstParam.Add(new GDAParameter("?dataInicio",  dataInicio));

            if(dataFim > DateTime.MinValue)
                lstParam.Add(new GDAParameter("?dataFim", dataFim.Value.AddDays(1).AddSeconds(-1)));

            return lstParam.Count == 0 ? null : lstParam.ToArray();
        }

        #endregion

        #region Busca orçamento pelo idMedicao

        /// <summary>
        /// Retorna o orçamento relacionado ao idMedicao passado se existir,
        /// senão retorna um novo objeto do tipo orçamento
        /// </summary>
        public Orcamento GetByMedicao(uint idMedicao)
        {
            return GetByMedicao(null, idMedicao);
        }

        /// <summary>
        /// Retorna o orçamento relacionado ao idMedicao passado se existir,
        /// senão retorna um novo objeto do tipo orçamento
        /// </summary>
        public Orcamento GetByMedicao(GDASession session, uint idMedicao)
        {
            string sql = "Select * From orcamento Where IdMedicao=" + idMedicao;

            List<Orcamento> lst = objPersistence.LoadData(session, sql);

            return lst.Count == 0 ? new Orcamento() : lst[0];
        }

        #endregion

        #region Busca orçamentos utilizando filtros de medição

        private string SqlOrcamentosMedicao(uint idMedicao, uint idMedidor, string nomeMedidor, IEnumerable<int> situacao, string dataFinIni, string dataFinFim, string nomeCli, bool selecionar)
        {
            string campos = selecionar ? @"o.*, f.Nome as NomeFuncionario, fm.Nome as NomeMedidor, l.NomeFantasia as NomeLoja, group_concat(m.IDMEDICAO) as idsMedicao" : "Count(*)";

            string sql = "Select " + campos + @" From orcamento o
                Inner Join medicao m On (o.idorcamento=m.idorcamento)
                Left Join funcionario f On o.IdFunc=f.idFunc
                Left Join funcionario fm On (m.idFuncMed=fm.idfunc)
                Left Join loja l On o.idLoja=l.idLoja Where m.idorcamento > 0";

            if (idMedicao > 0)
                sql += " And (m.Idmedicao=" + idMedicao + ")";

            if (situacao != null && situacao.Count() > 0 &&
                (situacao.ToList()[0] > 0 || situacao.Count() > 1))
                sql += string.Format(" And o.Situacao IN ({0})", string.Join(",", situacao));

            if (!String.IsNullOrEmpty(dataFinIni))
                sql += " And DataMedicao>=?dataIni";

            if (!String.IsNullOrEmpty(dataFinFim))
                sql += " And DataMedicao<=?dataFim";

            if (!String.IsNullOrEmpty(nomeCli))
                sql += " And o.NomeCliente Like ?nomeCli";

            if (idMedidor > 0)
                sql += " And m.IdFuncMed=" + idMedidor;
            else if (!String.IsNullOrEmpty(nomeMedidor))
                sql += " And fm.Nome Like ?nomeMedidor";

            sql += selecionar ? " group by o.idorcamento" : string.Empty;

            return sql;
        }

        public IList<Orcamento> GetOrcamentosMedicao(uint idMedicao, uint idMedidor, string nomeMedidor, IEnumerable<int> situacao, string dataFinIni, string dataFinFim, string nomeCli, string sortExpression, int startRow, int pageSize)
        {
            sortExpression = String.IsNullOrEmpty(sortExpression) ? "DataMedicao desc" : sortExpression;

            return LoadDataWithSortExpression(SqlOrcamentosMedicao(idMedicao, idMedidor, nomeMedidor, situacao, dataFinIni, dataFinFim, nomeCli, true), sortExpression, startRow, pageSize, GetParams(dataFinIni, dataFinFim, nomeCli, nomeMedidor));
        }

        public int GetOrcamentosMedicaoCount(uint idMedicao, uint idMedidor, string nomeMedidor, IEnumerable<int> situacao, string dataFinIni, string dataFinFim, string nomeCli)
        {
            return objPersistence.ExecuteSqlQueryCount(SqlOrcamentosMedicao(idMedicao, idMedidor, nomeMedidor, situacao, dataFinIni, dataFinFim, nomeCli, false), GetParams(dataFinIni, dataFinFim, nomeCli, nomeMedidor));
        }

        private GDAParameter[] GetParams(string dataFinIni, string dataFinFim, string nomeCli, string nomeMedidor)
        {
            List<GDAParameter> lstParam = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(dataFinIni))
                lstParam.Add(new GDAParameter("?dataIni", DateTime.Parse(dataFinIni)));

            if (!String.IsNullOrEmpty(dataFinFim))
                lstParam.Add(new GDAParameter("?dataFim", DateTime.Parse(dataFinFim)));

            if (!String.IsNullOrEmpty(nomeCli))
                lstParam.Add(new GDAParameter("?nomeCli", "%" + nomeCli + "%"));

            if (!String.IsNullOrEmpty(nomeMedidor))
                lstParam.Add(new GDAParameter("?nomeMedidor", "%" + nomeMedidor + "%"));

            return lstParam.Count > 0 ? lstParam.ToArray() : null;
        }

        #endregion

        #region Busca orçamentos criados por medição em campo

        private string SqlOrcamentoCalculo(uint idMedicao, uint idMedidor, string nomeMedidor, string dataFinIni, string dataFinFim, string nomeCli, bool selecionar)
        {
            string campos = selecionar ? "o.*, f.Nome as NomeFuncionario, medidor.Nome as NomeMedidor, l.NomeFantasia as NomeLoja" : "Count(*)";

            string sql = "Select " + campos + " From orcamento o " +
                "Inner Join medicao m On (o.idOrcamento=m.idOrcamento) " +
                "Left Join funcionario f On o.IdFunc=f.idFunc " +
                "Left Join funcionario medidor On (m.idFuncMed=medidor.idfunc) " +
                "Left Join loja l On o.idLoja=l.idLoja Where 1 ";

            if (idMedicao > 0)
                sql += " And m.idMedicao=" + idMedicao;

            if (!String.IsNullOrEmpty(dataFinIni))
                sql += " And DataMedicao>=?dataIni";

            if (!String.IsNullOrEmpty(dataFinFim))
                sql += " And DataMedicao<=?dataFim";

            if (!String.IsNullOrEmpty(nomeCli))
                sql += " And o.NomeCliente Like ?nomeCli";

            if (idMedidor > 0)
                sql += " And m.IdFuncMed=" + idMedidor;
            else if (!String.IsNullOrEmpty(nomeMedidor))
                sql += " And medidor.Nome Like ?nomeMedidor";

            return sql;
        }

        public IList<Orcamento> GetOrcamentoCalculo(uint idMedicao, uint idMedidor, string nomeMedidor, string dataFinIni, string dataFinFim, string nomeCli, string sortExpression, int startRow, int pageSize)
        {
            sortExpression = String.IsNullOrEmpty(sortExpression) ? "DataMedicao desc" : sortExpression;

            return LoadDataWithSortExpression(SqlOrcamentoCalculo(idMedicao, idMedidor, nomeMedidor, dataFinIni, dataFinFim, nomeCli, true), sortExpression, startRow, pageSize, GetCalculoParams(dataFinIni, dataFinFim, nomeCli, nomeMedidor));
        }

        public int GetOrcamentoCalculoCount(uint idMedicao, uint idMedidor, string nomeMedidor, string dataFinIni, string dataFinFim, string nomeCli)
        {
            return objPersistence.ExecuteSqlQueryCount(SqlOrcamentoCalculo(idMedicao, idMedidor, nomeMedidor, dataFinIni, dataFinFim, nomeCli, false), GetCalculoParams(dataFinIni, dataFinFim, nomeCli, nomeMedidor));
        }

        private GDAParameter[] GetCalculoParams(string dataFinIni, string dataFinFim, string nomeCli, string nomeMedidor)
        {
            List<GDAParameter> lstParam = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(dataFinIni))
                lstParam.Add(new GDAParameter("?dataIni", DateTime.Parse(dataFinIni)));

            if (!String.IsNullOrEmpty(dataFinFim))
                lstParam.Add(new GDAParameter("?dataFim", DateTime.Parse(dataFinFim)));

            if (!String.IsNullOrEmpty(nomeCli))
                lstParam.Add(new GDAParameter("?nomeCli", "%" + nomeCli + "%"));

            if (!String.IsNullOrEmpty(nomeMedidor))
                lstParam.Add(new GDAParameter("?nomeMedidor", "%" + nomeMedidor + "%"));

            return lstParam.Count > 0 ? lstParam.ToArray() : null;
        }

        #endregion

        #region Busca orçamentos para seleção

        private string SqlSel(uint idOrca, string cliente, uint idFunc, string telefone, string endereco, string bairro, int situacao, bool selecionar)
        {
            string campos = selecionar ? "o.*, f.Nome as NomeFuncionario, l.NomeFantasia as NomeLoja" : "Count(*)";

            string sql = "Select " + campos + " From orcamento o " +
                "Left Join funcionario f On o.IdFunc=f.idFunc " +
                "Left Join loja l On o.idLoja=l.idLoja Where 1 ";

            if (idOrca > 0)
                sql += " And o.IdOrcamento=" + idOrca;

            if (situacao > 0)
                sql += " And o.Situacao=" + situacao;

            if (idFunc > 0)
                sql += " And f.idFunc=" + idFunc;

            if (!String.IsNullOrEmpty(cliente))
                sql += " And o.NomeCliente Like ?cliente ";

            if (!String.IsNullOrEmpty(telefone))
                sql += " And o.TelCliente Like ?telefone ";

            if (!String.IsNullOrEmpty(endereco))
                sql += " And o.Endereco Like ?endereco ";

            if (!String.IsNullOrEmpty(bairro))
                sql += " And o.Bairro Like ?bairro ";

            return sql;
        }

        public IList<Orcamento> GetListSel(uint idOrca, string cliente, uint idFunc, string telefone,
            string endereco, string bairro, int situacao, string sortExpression, int startRow, int pageSize)
        {
            sortExpression = String.IsNullOrEmpty(sortExpression) ? "o.DataCad desc" : sortExpression;

            bool temFiltro;
            return LoadDataWithSortExpression(Sql(idOrca, 0, 0, cliente, idFunc, telefone, 0, endereco, null, bairro,
                 new int[] { situacao }, null, null, true, out temFiltro), sortExpression, startRow, pageSize, temFiltro,
                GetParameters(cliente, telefone, endereco, null, bairro, null, null));
        }

        public int GetCountSel(uint idOrca, string cliente, uint idFunc, string telefone, string endereco,
            string bairro, int situacao)
        {
            bool temFiltro;
            return GetCountWithInfoPaging(Sql(idOrca, 0, 0, cliente, idFunc, telefone, 0, endereco, null, bairro,
                new int[] { situacao }, null, null, true, out temFiltro), temFiltro, GetParameters(cliente, telefone, endereco, null, bairro, null,null));
        }

        #endregion

        #region Relatório Orçamento

        /// <summary>
        /// Busca orçamentos pelo nome do cliente e/ou pelo seu endereco
        /// </summary>
        /// <param name="cliente"></param>
        /// <param name="endereco"></param>
        /// <returns></returns>
        public IList<Orcamento> GetForRpt(string cliente, string endereco)
        {
            string sql = "Select o.*, f.Nome as NomeFuncionario, f.Email as EmailFuncionario, l.NomeFantasia as NomeLoja" +
                "From orcamento o " +
                "Left Join funcionario f On o.IdFunc=f.idFunc " +
                "Left Join loja l On o.idLoja=l.idLoja Where 1>0 ";

            var lstParam = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(cliente))
            {
                sql += " And o.NomeCliente Like ?cliente ";
                lstParam.Add(new GDAParameter("?cliente", cliente));
            }

            if (!String.IsNullOrEmpty(endereco))
            {
                sql += " And o.Endereco Like ?endereco ";
                lstParam.Add(new GDAParameter("?endereco", endereco));
            }

            return objPersistence.LoadData(sql, lstParam.Count > 0 ? lstParam.ToArray() : null).ToList();
        }

        /// <summary>
        /// Retorna orçamento para ser exibido no relatório, com campo DataLocal
        /// </summary>
        /// <param name="idOrca"></param>
        /// <returns></returns>
        public Orcamento GetForRpt(uint idOrca)
        {
            string sql = @"
                Select o.*, f.Nome as NomeFuncionario, f.Email as EmailFuncionario, f.ramal as ramalFunc,
                    l.NomeFantasia as NomeLoja, l.InscEst as InscEstLoja, l.Cnpj as CnpjLoja, l.Endereco as LogradouroLoja,
                    l.Compl as ComplLoja, l.Bairro as BairroLoja, cidLoja.NomeCidade as CidadeLoja, cidLoja.NomeUf as UfLoja, l.Cep as CepLoja,
                    l.numero as numeroLoja, concat(l.Telefone, if(length(l.telefone2)>0, concat(' / ', l.telefone2), '')) as TelefoneLoja,
                    l.Fax as FaxLoja, l.Site as emailLoja, c.cpf_cnpj as cpfCnpjCliente, c.rg_escinst as inscEstCliente, c.ObsNfe, vend.Nome as VendedorCliente
                From orcamento o Left Join funcionario f On o.IdFunc=f.idFunc
                    Left Join loja l On o.idLoja=l.idLoja
                    Left Join cidade cidLoja On (cidLoja.idCidade=l.idCidade)
                    Left Join cliente c On (o.idCliente=c.id_Cli)
                    Left Join funcionario vend On (c.idFunc=vend.idFunc)
                Where IdOrcamento=" + idOrca;

            Orcamento orca = objPersistence.LoadOneData(sql);
            orca.DataLocal = LojaDAO.Instance.GetElement(orca.IdLoja != null ? orca.IdLoja.Value : UserInfo.GetUserInfo.IdLoja).Cidade + ", " + Formatacoes.DataExtenso(DateTime.Now);

            return orca;
        }

        #endregion

        #region Relatório Lista de Orçamentos

        private string SqlRptLista(uint idLoja, uint idVendedor, IEnumerable<int> situacao, string dataIniSit, string dataFimSit,
            string dtIni, string dtFim, bool selecionar)
        {
            string criterio = String.Empty, where = String.Empty;

            List<GDAParameter> lstParam = new List<GDAParameter>();

            if (idLoja > 0)
            {
                where += " And o.IdLoja=" + idLoja;
                criterio += "Loja: " + LojaDAO.Instance.GetNome(idLoja) + "    ";
            }
            else
                criterio += "Loja: Todas    ";

            if (idVendedor > 0)
            {
                where += " And o.IdFunc=" + idVendedor;
                criterio += "Vendedor: " + BibliotecaTexto.GetTwoFirstNames(FuncionarioDAO.Instance.GetNome(idVendedor)) + "    ";
            }
            else
                criterio += "Vendedor: Todos    ";

            if (!String.IsNullOrEmpty(dtIni))
            {
                where += " And o.DataCad>=?dtIni";
                criterio += "Data Início: " + dtIni + "    ";
            }

            if (!String.IsNullOrEmpty(dtFim))
            {
                where += " And o.DataCad<=?dtFim";
                criterio += "Data Fim: " + dtFim + "    ";
            }

            if (!String.IsNullOrEmpty(dataIniSit))
            {
                where += " And o.DataAlt>=?dtIniSit ";
                criterio += "Data Situação: a partir de " + dataIniSit + "    ";
            }

            if (!String.IsNullOrEmpty(dataFimSit))
            {
                where += " And o.DataAlt<=?dtFimSit ";
                criterio += !String.IsNullOrEmpty(dataIniSit) ? " até " + dataIniSit + "    " : "Data Situação: até " + dataFimSit + "    ";
            }


            if (situacao != null && situacao.Count() > 0 &&
                (situacao.ToList()[0] > 0 || situacao.Count() > 1))
            {
                where += (string.Format(" And o.Situacao IN ({0})", string.Join(",", situacao)));

                if (situacao.Contains((int)Orcamento.SituacaoOrcamento.EmAberto))
                    criterio += "Situação: Em Aberto    ";
                if (situacao.Contains((int)Orcamento.SituacaoOrcamento.Negociado))
                    criterio += "Situação: Negociado    ";
                if (situacao.Contains((int)Orcamento.SituacaoOrcamento.NaoNegociado))
                    criterio += "Situação: Não Negociado    ";
                if (situacao.Contains((int)Orcamento.SituacaoOrcamento.EmNegociacao))
                    criterio += "Situação: Em Negociação    ";

            }

            string campos = selecionar ? "o.*, f.Nome as NomeFuncionario, l.NomeFantasia as NomeLoja, " +
                "'" + criterio + @"' as Criterio,
                    (SELECT SUM(IF(po.iditemprojeto>0, (Select Sum(mip.totm) From material_item_projeto mip Where mip.idItemProjeto=po.iditemprojeto), po.totm))
                    FROM produtos_orcamento po WHERE idOrcamento=o.idOrcamento) as TotM" : "Count(*)";

            return "Select " + campos + " From orcamento o " +
                "Left Join funcionario f On (o.IdFunc=f.IdFunc) " +
                "Left Join loja l On (o.IdLoja = l.IdLoja) " +
                "Where 1 " + where;
        }

        public IList<Orcamento> GetForRptLista(uint idLoja, uint idVendedor, IEnumerable<int> situacao, string dataIniSit, string dataFimSit,
            string dtIni, string dtFim)
        {
            string sql = SqlRptLista(idLoja, idVendedor, situacao, dataIniSit, dataFimSit, dtIni, dtFim, true) + " Order By o.DataCad Asc";

            return objPersistence.LoadData(sql, GetParamRptLista(dtIni, dtFim, dataIniSit, dataFimSit)).ToList();
        }

        public IList<Orcamento> GetForLista(uint idLoja, uint idVendedor, IEnumerable<int> situacao, string dataIniSit, string dataFimSit,
            string dtIni, string dtFim, string sortExpression, int startRow, int pageSize)
        {
            sortExpression = String.IsNullOrEmpty(sortExpression) ? "o.DataCad Asc" : sortExpression;

            return LoadDataWithSortExpression(SqlRptLista(idLoja, idVendedor, situacao, dataIniSit, dataFimSit, dtIni, dtFim, true),
                sortExpression, startRow, pageSize, GetParamRptLista(dtIni, dtFim, dataIniSit, dataFimSit));
        }

        public int GetCountRptLista(uint idLoja, uint idVendedor, IEnumerable<int> situacao, string dataIniSit, string dataFimSit, string dtIni,
            string dtFim)
        {
            return objPersistence.ExecuteSqlQueryCount(SqlRptLista(idLoja, idVendedor, situacao, dataIniSit, dataFimSit, dtIni, dtFim,
                false), GetParamRptLista(dtIni, dtFim, dataIniSit, dataFimSit));
        }

        private GDAParameter[] GetParamRptLista(string dtIni, string dtFim, string dataIniSit, string dataFimSit)
        {
            List<GDAParameter> lstParam = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(dtIni))
                lstParam.Add(new GDAParameter("?dtIni", DateTime.Parse(dtIni + " 00:00")));

            if (!String.IsNullOrEmpty(dtFim))
                lstParam.Add(new GDAParameter("?dtFim", DateTime.Parse(dtFim + " 23:59")));

            if (!String.IsNullOrEmpty(dataIniSit))
                lstParam.Add(new GDAParameter("?dtIniSit", DateTime.Parse(dataIniSit + " 00:00")));

            if (!String.IsNullOrEmpty(dataFimSit))
                lstParam.Add(new GDAParameter("?dtFimSit", DateTime.Parse(dataFimSit + " 23:59")));

            return lstParam.Count > 0 ? lstParam.ToArray() : null;
        }

        #endregion

        #region Atualiza o custo e o total do orçamento

        /// <summary>
        /// Atualiza os totais do orçamento, alterando o percentual da comissão.
        /// </summary>
        /// <param name="sessao">sessao.</param>
        /// <param name="orcamento">orcamento.</param>
        /// <param name="forcarAtualizacao">forcarAtualizacao.</param>
        /// <param name="alterouDesconto">alterouDesconto.</param>
        public void UpdateTotaisOrcamento(GDASession sessao, Orcamento orcamento, bool forcarAtualizacao, bool alterouDesconto)
        {
            // Verifica se o usuário está atualizando o total.
            if (!atualizando.ContainsKey(UserInfo.GetUserInfo.CodUser))
            {
                atualizando.Add(UserInfo.GetUserInfo.CodUser, false);
            }

            if (!forcarAtualizacao && atualizando[UserInfo.GetUserInfo.CodUser])
            {
                return;
            }

            try
            {
                // Define que o usuário está atualizando o total.
                atualizando[UserInfo.GetUserInfo.CodUser] = true;
                var sql = string.Empty;
                var msgErro = string.Empty;
                decimal totalOrcamento = 0;

                this.AtualizarCustoOrcamento(sessao, (int)orcamento.IdOrcamento);

                // Atualiza total do orçamento.
                sql = $@"UPDATE orcamento o SET o.Total =
                        (SELECT SUM(po.Total + COALESCE(po.ValorBenef, 0))
                        FROM produtos_orcamento po
                        WHERE po.IdOrcamento = o.IdOrcamento AND
                            po.IdProduto > 0 AND
                            (IdProdOrcamentoParent IS NULL OR IdProdOrcamentoParent = 0))
                    WHERE o.IdOrcamento = { orcamento.IdOrcamento }";

                this.objPersistence.ExecuteCommand(sessao, sql);

                if (!PedidoConfig.RatearDescontoProdutos)
                {
                    // Atualiza total do orçamento.
                    sql = $@"UPDATE orcamento o SET o.Total = ROUND(o.Total - IF(o.TipoDesconto = 1, (o.Total * (o.Desconto / 100)), o.Desconto)-
                            COALESCE((SELECT SUM(IF(poa.TipoDesconto = 1,
                                ((SELECT SUM(po.Total + COALESCE(po.ValorBenef, 0)) 
                                FROM produtos_orcamento po
                                WHERE po.IdProdParent = poa.IdProd AND  
                                    po.IdProduto > 0 AND
                                    (po.IdProdOrcamentoParent IS NULL OR po.IdProdOrcamentoParent = 0)) * (poa.Desconto / 100)), poa.Desconto)) 
                                FROM produtos_orcamento poa 
                                WHERE poa.IdOrcamento = o.IdOrcamento AND
                                    (poa.IdProdParent IS NULL OR poa.IdProdParent = 0)), 0), 2) 
                        WHERE o.IdOrcamento = { orcamento.IdOrcamento }";

                    this.objPersistence.ExecuteCommand(sessao, sql);
                }

                if (!this.DescontoPermitido(sessao, orcamento, out msgErro))
                {
                    var produtosOrcamento = ProdutosOrcamentoDAO.Instance.ObterProdutosOrcamento(sessao, (int)orcamento.IdOrcamento, null);
                    this.FinalizarAplicacaoComissaoAcrescimoDesconto(sessao, orcamento, produtosOrcamento, true);
                    this.objPersistence.ExecuteCommand(sessao, $"UPDATE orcamento SET Desconto = 0 WHERE IdOrcamento = {orcamento.IdOrcamento}");
                }
                else if (alterouDesconto)
                {
                    var tipoDesconto = this.ObterTipoDesconto(sessao, (int)orcamento.IdOrcamento);
                    var percDesconto = this.ObterDesconto(sessao, (int)orcamento.IdOrcamento);
                    var idFuncDesc = (uint?)this.ObterIdFuncDesc(sessao, (int)orcamento.IdOrcamento) ?? UserInfo.GetUserInfo.CodUser;

                    if (tipoDesconto == 2)
                    {
                        totalOrcamento = this.ObterTotal(sessao, (int)orcamento.IdOrcamento);
                        var totalSemDesconto = this.ObterTotalSemDesconto(sessao, (int)orcamento.IdOrcamento, totalOrcamento);
                        percDesconto = Orcamento.GetValorPerc(1, tipoDesconto, percDesconto, totalSemDesconto);
                    }

                    if (percDesconto > (decimal)OrcamentoConfig.Desconto.GetDescMaxOrcamentoConfigurado)
                    {
                        Email.EnviaEmailDescontoMaior(sessao, 0, orcamento.IdOrcamento, idFuncDesc, (float)percDesconto, OrcamentoConfig.Desconto.GetDescMaxOrcamentoConfigurado);
                    }
                }

                var sqlRateioDesconto = !PedidoConfig.RatearDescontoProdutos ? $" - IF(TipoDesconto = 1, (o.Total * (o.Desconto / 100)), o.Desconto)" : string.Empty;

                sql = $@"UPDATE orcamento o
                    SET o.Total = (o.Total{sqlRateioDesconto})
                    WHERE IdOrcamento = {orcamento.IdOrcamento}";

                this.objPersistence.ExecuteCommand(sessao, sql);

                orcamento.Total = this.ObterTotal(sessao, (int)orcamento.IdOrcamento);

                // Calcula os impostos dos produtos do pedido
                var impostos = CalculadoraImpostoHelper.ObterCalculadora<Model.Orcamento>()
                    .Calcular(sessao, orcamento);

                // Salva os dados dos impostos calculados
                impostos.Salvar(sessao);

                this.AtualizarValorComissao(sessao, (int)orcamento.IdOrcamento);
                this.AtualizarValorEntrega(sessao, (int)orcamento.IdOrcamento, orcamento.ValorEntrega);
                this.AtualizarDataUltimaAlteracao(sessao, (int)orcamento.IdOrcamento, (int)UserInfo.GetUserInfo.CodUser);
                this.AtualizarPeso(sessao, (int)orcamento.IdOrcamento);
            }
            finally
            {
                // Indica que a atualização já acabou
                atualizando[UserInfo.GetUserInfo.CodUser] = false;
            }
        }

        #endregion

        #region Comissão, Acréscimo e Desconto

        #region Comissão

        /// <summary>
        /// Aplica o percentual da comissão do orçamento.
        /// </summary>
        internal bool AplicarComissao(GDASession session, Orcamento orcamento, IEnumerable<ProdutosOrcamento> produtosOrcamento)
        {
            if (!PedidoConfig.Comissao.ComissaoAlteraValor)
            {
                return false;
            }

            return DescontoAcrescimo.Instance.AplicarComissao(
                session,
                orcamento,
                orcamento.PercComissao,
                produtosOrcamento
            );
        }

        /// <summary>
        /// Remove o percentual da comissão do orçamento.
        /// </summary>
        internal bool RemoverComissao(GDASession session, Orcamento orcamento, IEnumerable<ProdutosOrcamento> produtosOrcamento)
        {
            if (!PedidoConfig.Comissao.ComissaoAlteraValor)
            {
                return false;
            }

            return DescontoAcrescimo.Instance.RemoverComissao(session, orcamento, produtosOrcamento);
        }
        
        public decimal GetComissaoOrcamento(GDASession session, uint idOrcamento)
        {
            string sql = @"select coalesce(sum(coalesce(valorComissao,0)),0) from produtos_orcamento
                where idOrcamento=" + idOrcamento;

            return ExecuteScalar<decimal>(session, sql);
        }
        
        /// <summary>
        /// Recupera o percentual de comissão de um orçamento.
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idOrcamento"></param>
        /// <returns></returns>
        public float RecuperaPercComissao(GDASession sessao, uint idOrcamento)
        {
            return ObtemValorCampo<float>(sessao, "percComissao", "idOrcamento=" + idOrcamento);
        }
        
        /// <summary>
        /// Remove o percentual e o valor da comissão do orçamento.
        /// </summary>
        public void RemovePercComissao(GDASession session, uint idOrcamento, bool removerIdComissionado)
        {
            string sql = "update orcamento set percComissao=0, valorComissao=0" + (removerIdComissionado ? ", idComissionado=null" : "") +
                " where idOrcamento=" + idOrcamento;

            objPersistence.ExecuteCommand(session, sql);
        }

        #endregion

        #region Acréscimo

        /// <summary>
        /// Aplica acréscimo no valor dos produtos e consequentemente no valor do orçamento.
        /// </summary>
        /// <param name="session">session.</param>
        /// <param name="orcamento">orcamento.</param>
        /// <param name="produtosOrcamento">produtosOrcamento.</param>
        /// <returns>True: acréscimo aplicado;
        /// False: acréscimo não aplicado.</returns>
        internal bool AplicarAcrescimo(GDASession session, Orcamento orcamento, IEnumerable<ProdutosOrcamento> produtosOrcamento)
        {
            return DescontoAcrescimo.Instance.AplicarAcrescimo(
                session,
                orcamento,
                orcamento.TipoAcrescimo,
                orcamento.Acrescimo,
                produtosOrcamento
            );
        }

        /// <summary>
        /// Remove acréscimo no valor dos produtos e consequentemente no valor do orçamento.
        /// </summary>
        /// <param name="session">session.</param>
        /// <param name="orcamento">orcamento.</param>
        /// <param name="produtosOrcamento">produtosOrcamento.</param>
        /// <returns>True: acréscimo removido;
        /// False: acréscimo não removido.</returns>
        internal bool RemoverAcrescimo(GDASession session, Orcamento orcamento, IEnumerable<ProdutosOrcamento> produtosOrcamento)
        {
            return DescontoAcrescimo.Instance.RemoverAcrescimo(session, orcamento, produtosOrcamento);
        }

        /// <summary>
        /// Obtém o valor total de acréscimo dos produtos do orçamento informado.
        /// </summary>
        /// <param name="session">session.</param>
        /// <param name="idOrcamento">idOrcamento.</param>
        /// <returns>Retorna o valor total de acréscimo dos produtos do orçamento.</returns>
        public decimal ObterAcrescimoProdutos(GDASession session, int idOrcamento)
        {
            var sql = $@"SELECT
                    (SELECT COALESCE(SUM(COALESCE(po.ValorAcrescimoProd, 0) + COALESCE(po.ValorAcrescimoCliente, 0)), 0) FROM produtos_orcamento po
                    WHERE (po.IdProdOrcamentoParent IS NULL OR po.IdProdOrcamentoParent = 0) AND
                        po.IdOrcamento = {idOrcamento} {"{0}"})
                    +
                    (SELECT COALESCE(SUM(COALESCE(pob.ValorAcrescimoProd, 0)), 0) FROM produto_orcamento_benef pob
                        INNER JOIN produtos_orcamento po ON (pob.IdProd = po.IdProd)
                    WHERE (po.IdProdOrcamentoParent IS NULL OR po.IdProdOrcamentoParent = 0) AND
                        po.IdOrcamento = {idOrcamento} {"{0}"})
                AS temp";

            var sqlNaoVendeVidro = Geral.NaoVendeVidro() ? string.Empty : " AND po.IdProdParent > 0";

            return this.ExecuteScalar<decimal>(session, string.Format(sql, sqlNaoVendeVidro));
        }

        /// <summary>
        /// Obtém o valor total de acréscimo do orçamento informado.
        /// </summary>
        /// <param name="session">session.</param>
        /// <param name="idOrcamento">idOrcamento.</param>
        /// <returns>Retorna o valor total de acréscimo do orçamento.</returns>
        public decimal ObterAcrescimoOrcamento(GDASession session, int idOrcamento)
        {
            var sql = $@"SELECT
                    (SELECT COALESCE(SUM(COALESCE(po.ValorAcrescimo, 0)), 0) FROM produtos_orcamento po
                    WHERE (po.IdProdOrcamentoParent IS NULL OR po.IdProdOrcamentoParent = 0) AND
                        po.IdOrcamento = {idOrcamento})
                    +
                    (SELECT COALESCE(SUM(COALESCE(pob.ValorAcrescimo, 0)), 0) FROM produto_orcamento_benef pob
                        INNER JOIN produtos_orcamento po ON (pob.IdProd = po.IdProd)
                    WHERE (po.IdProdOrcamentoParent IS NULL OR po.IdProdOrcamentoParent = 0) AND
                        po.IdOrcamento = {idOrcamento})
                AS temp";

            return this.ExecuteScalar<decimal>(session, sql);
        }

        #endregion

        #region Desconto

        /// <summary>
        /// Aplica desconto no valor dos produtos e consequentemente no valor do orçamento.
        /// </summary>
        /// <param name="session">session.</param>
        /// <param name="orcamento">orcamento.</param>
        /// <param name="produtosOrcamento">produtosOrcamento.</param>
        /// <returns>True: desconto aplicado;
        /// False: desconto não aplicado.</returns>
        internal bool AplicarDesconto(GDASession session, Orcamento orcamento, IEnumerable<ProdutosOrcamento> produtosOrcamento)
        {
            return DescontoAcrescimo.Instance.AplicarDesconto(
                session,
                orcamento,
                orcamento.TipoDesconto,
                orcamento.Desconto,
                produtosOrcamento
            );
        }

        /// <summary>
        /// Remove desconto no valor dos produtos e consequentemente no valor do orçamento.
        /// </summary>
        /// <param name="sessao">sessao.</param>
        /// <param name="orcamento">orcamento.</param>
        /// <param name="produtosOrcamento">produtosOrcamento.</param>
        /// <returns>True: desconto removido;
        /// False: desconto não removido.</returns>
        internal bool RemoverDesconto(GDASession sessao, Orcamento orcamento, IEnumerable<ProdutosOrcamento> produtosOrcamento)
        {
            return DescontoAcrescimo.Instance.RemoverDesconto(sessao, orcamento, produtosOrcamento);
        }

        /// <summary>
        /// Obtém o valor total de desconto dos produtos do orçamento informado.
        /// </summary>
        /// <param name="sessao">sessao.</param>
        /// <param name="idOrcamento">idOrcamento.</param>
        /// <returns>Retorna o valor total de desconto dos produtos do orçamento.</returns>
        public decimal ObterDescontoProdutos(GDASession sessao, int idOrcamento)
        {
            var sql = string.Empty;

            if (PedidoConfig.RatearDescontoProdutos)
            {
                sql = $@"SELECT
                    (SELECT COALESCE(SUM(COALESCE(ValorDescontoProd, 0) + COALESCE(ValorDescontoQtde, 0) + COALESCE(ValorDescontoCliente, 0)), 0)
                    FROM produtos_orcamento po
                    WHERE (po.IdProdOrcamentoParent IS NULL OR po.IdProdOrcamentoParent = 0) AND
                        po.IdOrcamento = {idOrcamento} {"{0}"})
                    +
                    (SELECT COALESCE(SUM(COALESCE(pob.ValorDescontoProd, 0)), 0)
                    FROM produto_orcamento_benef pob
                        INNER JOIN produtos_orcamento po ON (pob.IdProd = po.IdProd)
                    WHERE (po.IdProdOrcamentoParent IS NULL OR po.IdProdOrcamentoParent = 0) AND
                        po.IdOrcamento = {idOrcamento} {"{0}"}) AS temp))";
            }
            else
            {
                sql = $@"SELECT
                    (SELECT COALESCE(SUM(COALESCE(((po.Total / pp.TotalProd) * pp.Desconto), 0) + COALESCE(po.ValorDescontoQtde, 0)), 0)
                    FROM produtos_orcamento po
                        INNER JOIN
                            (SELECT pp.IdProd, SUM(po.Total + COALESCE(po.ValorBenef, 0)) AS TotalProd, 
                                pp.Desconto * IF(pp.TipoDesconto = 1, SUM(po.Total + COALESCE(po.ValorBenef, 0)) / 100, 1) AS Desconto
                            FROM produtos_orcamento po
                                INNER JOIN produtos_orcamento pp ON (po.IdProdParent = pp.IdProd)
                            WHERE (po.IdProdOrcamentoParent IS NULL OR po.IdProdOrcamentoParent = 0) AND
                                po.IdOrcamento = {idOrcamento} {"{0}"}
                            GROUP BY pp.IdProd) AS pp ON (po.IdProdParent = pp.IdProd)
                    WHERE (po.IdProdOrcamentoParent IS NULL OR po.IdProdOrcamentoParent = 0) AND
                        po.IdOrcamento = {idOrcamento} {"{0}"})
                    +
                    (SELECT COALESCE(SUM(COALESCE(((pob.Valor / pp.TotalProd) * pp.Desconto), 0)), 0)
                    FROM produto_orcamento_benef pob
                        INNER JOIN produtos_orcamento po ON (pob.IdProd = po.IdProd)
                        INNER JOIN
                            (SELECT pp.IdProd, SUM(po.Total + COALESCE(po.ValorBenef, 0)) AS TotalProd, 
                                pp.Desconto * IF(pp.TipoDesconto = 1, SUM(po.Total + COALESCE(po.ValorBenef, 0)) / 100, 1) AS Desconto
                            FROM produtos_orcamento po
                                INNER JOIN produtos_orcamento pp ON (po.IdProdParent = pp.IdProd)
                            WHERE (po.IdProdOrcamentoParent IS NULL OR po.IdProdOrcamentoParent = 0) AND
                                po.IdOrcamento = {idOrcamento} {"{0}"}
                            GROUP BY pp.IdProd) AS pp ON (po.IdProdParent = pp.IdProd)
                    WHERE (po.IdProdOrcamentoParent IS NULL OR po.IdProdOrcamentoParent = 0) AND
                        po.IdOrcamento = {idOrcamento} {"{0}"})";
            }

            var sqlNaoVendeVidro = Geral.NaoVendeVidro() ? string.Empty : " AND po.IdProdParent > 0";

            return this.ExecuteScalar<decimal>(sessao, string.Format(sql, sqlNaoVendeVidro));
        }

        /// <summary>
        /// Obtém o valor total de desconto do orçamento informado.
        /// </summary>
        /// <param name="sessao">sessao.</param>
        /// <param name="idOrcamento">idOrcamento.</param>
        /// <returns>Retorna o valor total de desconto do orçamento.</returns>
        public decimal ObterDescontoOrcamento(GDASession sessao, int idOrcamento)
        {
            var sql = string.Empty;

            if (PedidoConfig.RatearDescontoProdutos)
            {
                sql = $@"SELECT
                    (SELECT COALESCE(SUM(COALESCE(po.ValorDesconto, 0)), 0)
                    FROM produtos_orcamento po WHERE (po.IdProdOrcamentoParent IS NULL OR po.IdProdOrcamentoParent = 0) AND
                        po.IdOrcamento = {idOrcamento})
                    +
                    (SELECT COALESCE(SUM(COALESCE(pob.ValorDesconto, 0)), 0)
                    FROM produto_orcamento_benef pob
                        INNER JOIN produtos_orcamento po ON (pob.IdProd = po.IdProd)
                    WHERE (po.IdProdOrcamentoParent IS NULL OR po.IdProdOrcamentoParent = 0) AND
                        po.IdOrcamento = {idOrcamento}) AS temp))";
            }
            else
            {
                sql = $@"SELECT IF(o.TipoDesconto = 2, o.Desconto, ((o.Total - o.ValorIcms) - o.ValorIpi) / (1 - (o.Desconto / 100)) * (o.Desconto / 100))
                    FROM orcamento o
                    WHERE o.IdOrcamento = {idOrcamento}";
            }

            return this.ExecuteScalar<decimal>(sessao, sql);
        }

        #endregion

        #region Finalizar

        /// <summary>
        /// Finaliza a aplicação de comissão, desconto e acréscimo.
        /// </summary>
        /// <param name="sessao">sessao.</param>
        /// <param name="orcamento">orcamento.</param>
        /// <param name="produtosOrcamento">produtosOrcamento.</param>
        /// <param name="atualizar">atualizar.</param>
        /// <param name="manterFuncDesc">manterFuncDesc.</param>
        internal void FinalizarAplicacaoComissaoAcrescimoDesconto(
            GDASession sessao,
            Orcamento orcamento,
            IEnumerable<ProdutosOrcamento> produtosOrcamento,
            bool atualizar,
            bool manterFuncDesc = false)
        {
            if (atualizar)
            {
                var idsParentsBuscar = new List<int>();
                var parents = new List<ProdutosOrcamento>();

                foreach (var produto in produtosOrcamento)
                {
                    ProdutosOrcamentoDAO.Instance.UpdateBase(sessao, produto, orcamento);
                    ProdutosOrcamentoDAO.Instance.AtualizarBenef(sessao, (int)produto.IdProd, produto.Beneficiamentos);

                    if (!produto.IdProdParent.HasValue)
                    {
                        parents.Add(produto);
                    }
                    else if (!parents.Any(p => p.IdProd == produto.IdProdParent.Value) && !idsParentsBuscar.Any(id => id == produto.IdProdParent.Value))
                    {
                        idsParentsBuscar.Add((int)produto.IdProdParent.Value);
                    }
                }

                if (idsParentsBuscar.Any())
                {
                    parents.AddRange(ProdutosOrcamentoDAO.Instance.ObterProdutosOrcamentoPorIdsProdOrcamento(sessao, idsParentsBuscar));
                }

                foreach (var prodParent in parents)
                {
                    ProdutosOrcamentoDAO.Instance.UpdateTotaisProdutoOrcamento(sessao, prodParent);
                }
            }

            // A data do desconto não pode ser alterada caso o pedido esteja sendo gerado.
            if (!manterFuncDesc)
            {
                var dataDesc = DateTime.Now;
                this.objPersistence.ExecuteCommand(
                    sessao,
                    "update orcamento set idFuncDesc=?f, dataDesc=?d where idOrcamento=" + orcamento.IdOrcamento,
                    new GDAParameter("?f", UserInfo.GetUserInfo.CodUser),
                    new GDAParameter("?d", dataDesc)
                );

                orcamento.IdFuncDesc = UserInfo.GetUserInfo.CodUser;
            }
        }

        #endregion

        #endregion

        #region Duplicar orçamento

        /// <summary>
        /// Duplica o orçamento informado.
        /// </summary>
        /// <param name="idOrcamento">idOrcamento.</param>
        /// <returns>Retorna o ID do novo orçamento.</returns>
        public uint Duplicar(uint idOrcamento)
        {
            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    var produtosOrcamentoOriginalNovo = new Dictionary<int, int>();
                    var produtosComposicaoOriginalNovo = new Dictionary<int, int>();
                    var itensProjetoOriginalNovo = new Dictionary<uint, uint>();
                    var materiaistemProjetoOriginalNovo = new Dictionary<int, int>();

                    var orcamento = this.GetElementByPrimaryKey(transaction, idOrcamento);

                    if (orcamento == null)
                    {
                        throw new Exception("Orçamento não encontrado.");
                    }

                    orcamento.IdOrcamento = 0;
                    orcamento.IdPedidoGerado = null;
                    orcamento.IdProjeto = null;
                    orcamento.Situacao = (int)Orcamento.SituacaoOrcamento.EmAberto;
                    orcamento.UsuAlt = null;
                    orcamento.DataAlt = null;
                    orcamento.IdFuncionario = UserInfo.GetUserInfo.CodUser;
                    orcamento.IdOrcamentoOriginal = idOrcamento;
                    orcamento.IdOrcamento = Instance.Insert(transaction, orcamento);

                    ProdutosOrcamentoDAO.Instance.InserirProdutosOrcamentoPelosIdsItemProjeto(transaction, (int)idOrcamento);

                    foreach (var i in ItemProjetoDAO.Instance.GetByOrcamento(transaction, idOrcamento))
                    {
                        var idItemProjetoNovo = this.ClonaItemProjeto(transaction, i.IdItemProjeto, orcamento);
                        itensProjetoOriginalNovo.Add(i.IdItemProjeto, idItemProjetoNovo);
                    }

                    var produtosOrcamentoAmbiente = ProdutosOrcamentoDAO.Instance.ObterProdutosAmbienteOrcamento(transaction, (int)idOrcamento);

                    foreach (var produtoOrcamentoAmbiente in produtosOrcamentoAmbiente)
                    {
                        var idAmbienteOriginal = produtoOrcamentoAmbiente.IdProd;

                        // Recupera os beneficiamentos.
                        produtoOrcamentoAmbiente.Beneficiamentos = produtoOrcamentoAmbiente.Beneficiamentos;
                        produtoOrcamentoAmbiente.IdProd = 0;
                        produtoOrcamentoAmbiente.IdOrcamento = orcamento.IdOrcamento;
                        produtoOrcamentoAmbiente.IdItemProjeto = produtoOrcamentoAmbiente.IdItemProjeto > 0 && itensProjetoOriginalNovo.ContainsKey(produtoOrcamentoAmbiente.IdItemProjeto.Value) ?
                            (uint?)itensProjetoOriginalNovo[produtoOrcamentoAmbiente.IdItemProjeto.Value] : null;
                        produtoOrcamentoAmbiente.IdProd = ProdutosOrcamentoDAO.Instance.Insert(transaction, produtoOrcamentoAmbiente);

                        var produtosOrcamento = ProdutosOrcamentoDAO.Instance.ObterProdutosOrcamento(transaction, (int)produtoOrcamentoAmbiente.IdOrcamento, (int)idOrcamento);

                        // A ordenação deve ser feita por esse campo, para que todos os produtos pais sejam inseridos antes dos produtos filhos.
                        foreach (var produtoOrcamento in produtosOrcamento.OrderBy(f => f.IdProdOrcamentoParent))
                        {
                            var idProdutoOrcamentoOriginal = produtoOrcamento.IdProd;

                            // Recupera os beneficiamentos.
                            produtoOrcamento.Beneficiamentos = produtoOrcamento.Beneficiamentos;
                            produtoOrcamento.IdProd = 0;
                            produtoOrcamento.IdOrcamento = orcamento.IdOrcamento;
                            produtoOrcamento.IdProdParent = produtoOrcamentoAmbiente.IdProd;
                            produtoOrcamento.IdItemProjeto = produtoOrcamentoAmbiente.IdItemProjeto;
                            produtoOrcamento.IdProd = ProdutosOrcamentoDAO.Instance.Insert(transaction, produtoOrcamento);

                            if (produtoOrcamento.IdProdOrcamentoParent > 0 && produtosOrcamentoOriginalNovo.ContainsKey((int)produtoOrcamento.IdProdOrcamentoParent))
                            {
                                produtoOrcamento.IdProdOrcamentoParent = produtosOrcamentoOriginalNovo[(int)produtoOrcamento.IdProdOrcamentoParent];
                            }

                            produtosOrcamentoOriginalNovo.Add((int)idProdutoOrcamentoOriginal, (int)produtoOrcamento.IdProd);
                        }
                    }

                    // Salva o orçamento novamente para manter os dados sobre desconto, acréscimo e comissão.
                    this.Update(transaction, orcamento);

                    transaction.Commit();
                    transaction.Close();

                    return orcamento.IdOrcamento;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    transaction.Close();

                    if (ex.ToString().Contains("BLOQUEIO_ORCAMENTO"))
                    {
                        throw new Exception("Foi cadastrado um orçamento com estes mesmos dados recentemente sendo necessário aguardar 1 minuto ou alterar dados do orçamento.");
                    }
                    else
                    {
                        throw new Exception(MensagemAlerta.FormatErrorMsg("Falha ao duplicar orçamento.", ex));
                    }
                }
            }
        }

        #endregion

        #region Clona item projeto para o orçamento passado

        /// <summary>
        /// Clona item projeto para o orçamento passado
        /// </summary>
        internal uint ClonaItemProjeto(GDASession session, uint idItemProjeto, Orcamento orcamento)
        {
            uint idItemProjetoOrca = 0;

            // Clona item projeto
            var itemProj = ItemProjetoDAO.Instance.GetElement(session, idItemProjeto);
            itemProj.IdOrcamento = orcamento.IdOrcamento;
            itemProj.IdProjeto = null;
            itemProj.IdPedido = null;
            itemProj.IdPedidoEspelho = null;
            idItemProjetoOrca = ItemProjetoDAO.Instance.Insert(session, itemProj);

            // Clona medidas
            MedidaItemProjetoDAO.Instance.DeleteByItemProjeto(session, idItemProjetoOrca);
            foreach (var mip in MedidaItemProjetoDAO.Instance.GetListByItemProjeto(session, idItemProjeto))
            {
                mip.IdMedidaItemProjeto = 0;
                mip.IdItemProjeto = idItemProjetoOrca;
                MedidaItemProjetoDAO.Instance.Insert(session, mip);
            }

            // Clona peças
            PecaItemProjetoDAO.Instance.DeleteByItemProjeto(session, idItemProjetoOrca);
            foreach (var pip in PecaItemProjetoDAO.Instance.GetByItemProjeto(session, idItemProjeto, itemProj.IdProjetoModelo))
            {
                // Importa beneficiamentos
                pip.Beneficiamentos = pip.Beneficiamentos;

                pip.IdPecaItemProj = 0;
                pip.IdItemProjeto = idItemProjetoOrca;
                PecaItemProjetoDAO.Instance.Insert(session, pip);
            }

            // Armazena quais idPecaItemProj foram usados nos materiais para que a peça seja referenciada por apenas um material
            var idsPecaUsados = string.Empty;

            // Clona materiais
            MaterialItemProjetoDAO.Instance.DeleteByItemProjeto(session, idItemProjetoOrca);
            foreach (var mip in MaterialItemProjetoDAO.Instance.GetByItemProjeto(session, idItemProjeto, false))
            {
                // Importa beneficiamentos
                mip.Beneficiamentos = mip.Beneficiamentos;

                mip.IdMaterItemProj = 0;
                mip.IdItemProjeto = idItemProjetoOrca;
                mip.IdPecaItemProj = mip.IdPecaItemProj > 0 ? PecaItemProjetoDAO.Instance.ObtemIdPecaNova(session, mip.IdPecaItemProj.Value, idItemProjetoOrca, idsPecaUsados) : null;

                if (mip.IdPecaItemProj > 0)
                    idsPecaUsados += mip.IdPecaItemProj + ",";

                mip.IdMaterItemProj = MaterialItemProjetoDAO.Instance.InsertBase(session, mip, orcamento);
            }

            #region Update Total Item Projeto

            ItemProjetoDAO.Instance.UpdateTotalItemProjeto(session, idItemProjetoOrca);

            var idProd = ProdutosOrcamentoDAO.Instance.ObtemValorCampo<uint>(session, "idProd", "idItemProjeto=" + idItemProjetoOrca);

            if (idProd > 0)
            {
                ProdutosOrcamentoDAO.Instance.UpdateTotaisProdutoOrcamento(session, ProdutosOrcamentoDAO.Instance.GetElementByPrimaryKey(session, (int)idProd));
            }

            this.UpdateTotaisOrcamento(session, orcamento, false, false);

            #endregion

            return idItemProjetoOrca;
        }

        #endregion

        #region Atualiza campos do orçamento

        /// <summary>
        /// Atualizar valor do custo do orçamento.
        /// </summary>
        /// <param name="session">session.</param>
        /// <param name="idOrcamento">idOrcamento.</param>
        public void AtualizarCustoOrcamento(GDASession session, int idOrcamento)
        {
            if (idOrcamento == 0)
            {
                return;
            }

            // Atualiza valor do custo do orçamento.
            var sql = $@"UPDATE orcamento o
                SET o.Custo = (SELECT ROUND(SUM(po.Custo), 2)
                    FROM produtos_orcamento po
                    WHERE po.IdOrcamento = o.IdOrcamento AND
                        po.IdProduto > 0 AND
                        (po.IdProdOrcamentoParent IS NULL OR po.IdProdOrcamentoParent = 0))
                WHERE o.IdOrcamento = {idOrcamento}";

            this.objPersistence.ExecuteCommand(session, sql);
        }

        /// <summary>
        /// Atualiza o peso dos produtos e do orçamento.
        /// </summary>
        /// <param name="sessao">sessao.</param>
        /// <param name="idOrcamento">idOrcamento.</param>
        public void AtualizarPeso(GDASession sessao, int idOrcamento)
        {
            if (idOrcamento == 0)
            {
                return;
            }

            var sqlCalculoPeso = Utils.SqlCalcPeso(Utils.TipoCalcPeso.ProdutoOrcamento, (uint)idOrcamento, false, false, false);

            var sql = $@"UPDATE produtos_orcamento po
                    LEFT JOIN ({sqlCalculoPeso}) AS peso ON (po.IdProd = peso.Id)
                    INNER JOIN produto prod ON (po.IdProduto = prod.IdProd)
                    LEFT JOIN subgrupo_prod sgp ON (prod.IdSubGrupoProd = sgp.IdSubGrupoProd)
                    LEFT JOIN
                    (
                        SELECT po1.IdProdOrcamentoParent, SUM(po1.Peso) AS Peso
                        FROM produtos_orcamento po1
                        WHERE (po1.IdProdOrcamentoParent IS NULL OR po1.IdProdOrcamentoParent = 0) AND po1.IdOrcamento = {idOrcamento}
                        GROUP BY po1.IdProdOrcamentoParent
                    ) AS pesoFilhos ON (po.IdProd = pesoFilhos.IdProdOrcamentoParent)
                    SET po.Peso = COALESCE(IF(sgp.TipoSubgrupo IN ({(int)TipoSubgrupoProd.VidroDuplo}, {(int)TipoSubgrupoProd.VidroLaminado}), pesoFilhos.Peso * po.Qtde, peso.Peso), 0)
                WHERE (po.IdProdOrcamentoParent IS NULL OR po.IdProdOrcamentoParent = 0) AND
                    po.IdOrcamento = {idOrcamento};

                UPDATE orcamento o
                    SET o.Peso = COALESCE((SELECT SUM(po.Peso) FROM produtos_orcamento po
                        WHERE (po.IdProdOrcamentoParent IS NULL OR po.IdProdOrcamentoParent = 0) AND
                            po.IdOrcamento = {idOrcamento}), 0)
                WHERE o.IdOrcamento = {idOrcamento}";

            this.objPersistence.ExecuteCommand(sessao, sql);
        }

        /// <summary>
        /// Atualiza a data da última alteração feita no orçamento.
        /// </summary>
        /// <param name="session">session.</param>
        /// <param name="idOrcamento">idOrcamento.</param>
        /// <param name="idFunc">idFunc.</param>
        internal void AtualizarDataUltimaAlteracao(GDASession session, int idOrcamento, int idFunc)
        {
            if (idOrcamento == 0 || idFunc == 0)
            {
                return;
            }

            // Atualiza o usuário e a data da última alteração.
            var sql = $@"UPDATE orcamento o
                SET o.UsuAlt = {idFunc},
                    o.DataAlt = NOW()
                WHERE o.IdOrcamento = {idOrcamento}";

            this.objPersistence.ExecuteCommand(session, sql);
        }

        /// <summary>
        /// Atualiza o valor de entrega (frete) do orçamento.
        /// </summary>
        /// <param name="session">session.</param>
        /// <param name="idOrcamento">idOrcamento.</param>
        /// <param name="valorEntrega">valorEntrega.</param>
        internal void AtualizarValorEntrega(GDASession session, int idOrcamento, decimal valorEntrega)
        {
            if (idOrcamento == 0)
            {
                return;
            }

            var sql = $@"UPDATE orcamento o
                SET o.Total = COALESCE(o.Total, 0) + ?frete
                WHERE o.IdOrcamento = {idOrcamento}";

            this.objPersistence.ExecuteCommand(session, sql, new GDAParameter("?frete", valorEntrega));
        }

        /// <summary>
        /// Atualiza o valor de comissão do orçamento.
        /// </summary>
        /// <param name="session">session.</param>
        /// <param name="idOrcamento">idOrcamento.</param>
        internal void AtualizarValorComissao(GDASession session, int idOrcamento)
        {
            if (idOrcamento == 0)
            {
                return;
            }

            // Atualiza o campo ValorComissao.
            var sql = $@"UPDATE orcamento o
                SET o.ValorComissao = ((o.Total * COALESCE(o.PercComissao, 0)) / 100)
                WHERE o.IdOrcamento = {idOrcamento}";

            this.objPersistence.ExecuteCommand(session, sql);
        }

        /// <summary>
        /// Atualiza a data que o orçamento foi recalculado.
        /// </summary>
        /// <param name="session">session.</param>
        /// <param name="idOrcamento">idOrcamento.</param>
        /// <param name="dataRecalcular">dataRecalcular.</param>
        /// <param name="idFuncRecalcular">idFuncRecalcular.</param>
        public void AtualizarDataRecalcular(GDASession session, uint idOrcamento, DateTime? dataRecalcular, uint idFuncRecalcular)
        {
            if (idOrcamento == 0)
            {
                return;
            }

            if (dataRecalcular != null)
            {
                var sqlPreencherDataRecalcular = $@"UPDATE orcamento
                    SET DataRecalcular = ?data,
                        IdFuncRecalcular = ?idFunc
                    WHERE IdOrcamento = {idOrcamento}";
                var parametrosPreencherDataRecalcular = new List<GDAParameter>
                {
                    new GDAParameter("?data", dataRecalcular),
                    new GDAParameter("?idFunc", idFuncRecalcular),
                };

                this.objPersistence.ExecuteCommand(session, sqlPreencherDataRecalcular, parametrosPreencherDataRecalcular.ToArray());
            }
            else
            {
                var sqlZerarDataRecalcular = $@"UPDATE orcamento
                    SET DataRecalcular = NULL,
                        IdFuncRecalcular = ?idFunc
                    WHERE IdOrcamento = {idOrcamento}";
                var parametrosZerarDataRecalcular = new List<GDAParameter>
                {
                    new GDAParameter("?idFunc", idFuncRecalcular),
                };

                this.objPersistence.ExecuteCommand(session, sqlZerarDataRecalcular, parametrosZerarDataRecalcular.ToArray());
            }
        }

        /// <summary>
        /// Atualiza os valores de impostos associados com a instancia informada.
        /// </summary>
        /// <param name="sessao">sessao.</param>
        /// <param name="orcamento">orcamento.</param>
        /// <param name="atualizarTotal">atualizarTotal.</param>
        public void AtualizarImpostos(GDASession sessao, Orcamento orcamento, bool atualizarTotal)
        {
            if ((orcamento?.IdOrcamento).GetValueOrDefault() == 0)
            {
                return;
            }

            // Relação das propriedades que devem ser atualizadas
            var propriedades = new List<string>
            {
                nameof(Orcamento.ValorIpi),
                nameof(Orcamento.ValorIcms),
                nameof(Orcamento.AliquotaIpi),
                nameof(Orcamento.AliquotaIcms),
            };

            this.objPersistence.Update(sessao, orcamento, string.Join(",", propriedades), DirectionPropertiesName.Inclusion);

            if (atualizarTotal)
            {
                var sqlAtualizaTotal = "UPDATE orcamento SET Total = ?total WHERE IdOrcamento = ?id";
                var parametrosAtualizaTotal = new List<GDAParameter>
                {
                    new GDAParameter("?total", orcamento.Total),
                    new GDAParameter("?id", orcamento.IdOrcamento),
                };

                this.objPersistence.ExecuteCommand(sessao, sqlAtualizaTotal, parametrosAtualizaTotal.ToArray());
            }
        }

        #endregion

        #region Verifica campos do orçamento

        /// <summary>
        /// Verifica se orçamento existe e está em aberto.
        /// </summary>
        /// <param name="session">session.</param>
        /// <param name="idOrcamento">idOrcamento.</param>
        /// <returns>True: o orçamento existe e está em aberto;
        /// False: o orçamento não existe ou não está em aberto.</returns>
        public bool ExistsOrcamentoEmAberto(GDASession session, int? idOrcamento)
        {
            if (idOrcamento.GetValueOrDefault() == 0)
            {
                return false;
            }

            var sqlOrcamentoExisteEmAberto = $@"SELECT COUNT(*) FROM orcamento
                WHERE Situacao = {(int)Orcamento.SituacaoOrcamento.EmAberto}
                    AND IdOrcamento = {idOrcamento}";

            return this.objPersistence.ExecuteScalar(session, sqlOrcamentoExisteEmAberto)?.ToString()?.StrParaInt() > 0;
        }

        /// <summary>
        /// Verifica se o orçamento possui pedido gerado
        /// </summary>
        /// <param name="session">session.</param>
        /// <param name="idOrcamento">idOrcamento.</param>
        /// <returns>True: o orçamento possui pedido gerado;
        /// False: o orçamento não possui pedido gerado.</returns>
        public bool VerificarPossuiPedidoGerado(GDASession session, int idOrcamento)
        {
            if (idOrcamento == 0 || !this.Exists(session, idOrcamento))
            {
                return false;
            }

            var sqlOrcamentoPossuiPedidoGerado = $@"SELECT COUNT(*) FROM pedido
                WHERE Situacao <> {(int)Pedido.SituacaoPedido.Cancelado} AND
                    IdOrcamento = {idOrcamento}";

            return this.objPersistence.ExecuteSqlQueryCount(session, sqlOrcamentoPossuiPedidoGerado) > 0;
        }

        /// <summary>
        /// Verifica se o orçamento pode ser editado.
        /// </summary>
        /// <param name="idOrcamento">idOrcamento.</param>
        /// <returns>True: o orçamento pode ser editado;
        /// False: o orçamento não pode ser editado.</returns>
        public bool EditEnabled(uint idOrcamento)
        {
            if (idOrcamento == 0)
            {
                return false;
            }

            var sql = $@"SELECT COUNT(*) FROM orcamento
                WHERE Situacao = {(int)Orcamento.SituacaoOrcamento.NegociadoParcialmente} AND
                    IdOrcamento = {idOrcamento}";

            if (this.objPersistence.ExecuteSqlQueryCount(sql) == 0)
            {
                return true;
            }

            sql = $@"SELECT COUNT(*) FROM produtos_orcamento
                WHERE IdOrcamento = { idOrcamento } AND
                    IdAmbientePedido IS NOT NULL AND IdAmbientePedido > 0";

            return this.objPersistence.ExecuteSqlQueryCount(sql) == 0;
        }

        /// <summary>
        /// Verifica se o orçamento pode ser editado.
        /// </summary>
        /// <param name="idOrcamento">idOrcamento.</param>
        /// <returns>True: o orçamento pode ser editado
        /// False: o orçamento não pode ser editado.</returns>
        public bool VerificarPodeEditarOrcamento(uint idOrcamento)
        {
            if (idOrcamento == 0 || !this.Exists(idOrcamento))
            {
                return false;
            }

            var orcamento = this.GetElementByPrimaryKey(idOrcamento);

            return this.VerificarPodeEditarOrcamento(
                orcamento.IdLoja,
                orcamento.IdFuncionario,
                orcamento.IdPedidoGerado,
                orcamento.Situacao);
        }

        /// <summary>
        /// Verifica se o orçamento pode ser editado.
        /// </summary>
        /// <param name="idLoja">idLoja.</param>
        /// <param name="idFuncionario">idFuncionario.</param>
        /// <param name="idPedidoGerado">idPedidoGerado.</param>
        /// <param name="situacao">situacao.</param>
        /// <returns>True: o orçamento pode ser editado
        /// False: o orçamento não pode ser editado.</returns>
        public bool VerificarPodeEditarOrcamento(uint? idLoja, uint? idFuncionario, uint? idPedidoGerado, int situacao)
        {
            if (idLoja.GetValueOrDefault() == 0)
            {
                return true;
            }

            var login = UserInfo.GetUserInfo;
            var flagVendedor = true;

            // Se for vendedor, só altera pedidos da loja dele
            if (login.TipoUsuario == (uint)Utils.TipoFuncionario.Vendedor)
            {
                flagVendedor = idLoja == login.IdLoja || login.CodUser == idFuncionario;

                // Verifica se vendedor só pode alterar orçamento dele
                if (!OrcamentoConfig.AlterarOrcamentoVendedor)
                {
                    flagVendedor = login.CodUser == idFuncionario || idFuncionario == null;
                }

                if (OrcamentoConfig.VendedorPodeAlterarOrcamentoQualquerLoja)
                {
                    flagVendedor = true;
                }
            }

            return flagVendedor &&
                ((idPedidoGerado == null || idPedidoGerado <= 0) ||
                situacao == (int)Orcamento.SituacaoOrcamento.NegociadoParcialmente);
        }

        /// <summary>
        /// Verifica se o desconto é permitido.
        /// </summary>
        /// <param name="sessao">sessao.</param>
        /// <param name="orca">orca.</param>
        /// <param name="msgErro">msgErro.</param>
        /// <returns>True: o desconto aplicado no orçamento é permitiro;
        /// False: o desconto aplicado no orçamento é permitido.</returns>
        internal bool DescontoPermitido(GDASession sessao, Orcamento orca, out string msgErro)
        {
            msgErro = string.Empty;

            if ((orca?.IdOrcamento).GetValueOrDefault() == 0)
            {
                return false;
            }

            msgErro = string.Empty;
            var somaDesconto = $@"COALESCE((SELECT SUM(COALESCE(po.ValorDescontoQtde, 0){(PedidoConfig.RatearDescontoProdutos ? " + COALESCE(po.ValorDesconto, 0) + COALESCE(po.ValorDescontoProd, 0)" : string.Empty)})
                FROM produtos_orcamento po
                WHERE (po.IdProdOrcamentoParent IS NULL OR po.IdProdOrcamentoParent = 0) AND
                    po.IdProdParent > 0 AND
                    po.IdOrcamento = o.IdOrcamento), 0)";

            var idFunc = UserInfo.GetUserInfo.CodUser;

            if (Geral.ManterDescontoAdministrador)
            {
                idFunc = (uint)this.ObterIdFuncDesc(sessao, (int)orca.IdOrcamento).GetValueOrDefault((int)idFunc);
            }

            var descontoMaximoOrcamento = OrcamentoConfig.Desconto.GetDescontoMaximoOrcamento(sessao, idFunc);

            var sql = $@"SELECT COUNT(*) FROM orcamento o
                WHERE o.IdOrcamento = {orca.IdOrcamento} AND
                    ((o.TipoDesconto = 1 AND o.Desconto <= {descontoMaximoOrcamento}) OR
                    (o.TipoDesconto = 2 AND ROUND(COALESCE(o.Desconto / (o.Total + {somaDesconto}{(!PedidoConfig.RatearDescontoProdutos ? " + o.Desconto" : string.Empty)}), 0), 2) <=
                ({descontoMaximoOrcamento} / 100)))";

            // Se o desconto do orçamento não estiver dentro do permitido, já retorna false
            if (this.ExecuteScalar<int>(sessao, sql) <= 0)
            {
                msgErro = "Desconto acima do permitido";
                return false;
            }

            // Verifica se este orçamento pode ter desconto
            if (orca.Desconto > 0 && FuncionarioDAO.Instance.ObtemIdTipoFunc(sessao, idFunc) != (uint)Utils.TipoFuncionario.Administrador)
            {
                // Obtém o maior desconto aplicado nos ambientes deste orçamento
                var maiorDescPercAmbiente = ProdutosOrcamentoDAO.Instance.ObterMaiorDesconto(sessao, orca.IdOrcamento);

                if (PedidoConfig.Desconto.ImpedirDescontoSomativo)
                {
                    if (orca.IdCliente > 0 && DescontoAcrescimoClienteDAO.Instance.ClientePossuiDesconto(sessao, orca.IdCliente.Value, orca.IdOrcamento, null, 0, null))
                    {
                        msgErro = "O cliente já possui desconto por grupo/subgrupo, não é permitido lançar outro desconto.";
                        return false;
                    }

                    if (maiorDescPercAmbiente > 0)
                    {
                        msgErro = "O cliente já possui desconto aplicado nos ambientes deste orçamento, não é permitido lançar outro desconto.";
                        return false;
                    }
                }

                var percDescontoOrca = default(decimal);

                if (orca.TipoDesconto == 1)
                {
                    percDescontoOrca = orca.Desconto;
                }
                else if (orca.TotalSemDesconto > 0)
                {
                    /* Chamado 22175.
                     * O total sem desconto será 0 quando o orçamento for duplicado e o primeiro projeto estiver sendo clonado,
                     * por isso, deve ser feita essa verificação. */
                    percDescontoOrca = (orca.Desconto / orca.TotalSemDesconto) * 100;
                }

                if ((decimal)OrcamentoConfig.Desconto.GetDescontoMaximoOrcamento(idFunc) < percDescontoOrca + maiorDescPercAmbiente)
                {
                    msgErro = $"Desconto lançado está acima do permitido. Desconto orçamento: {percDescontoOrca.ToString("N2")}%, Desconto ambientes: {maiorDescPercAmbiente.ToString("N2")}%";

                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Verifica se o orçamento possui produtos.
        /// </summary>
        /// <param name="session">session.</param>
        /// <param name="idOrcamento">idOrcamento.</param>
        /// <returns>True: o orçamento possui produtos;
        /// False: o orçamento não possui produtos.</returns>
        public bool VerificarOrcamentoPossuiProdutos(GDASession session, int idOrcamento)
        {
            if (idOrcamento == 0)
            {
                return false;
            }

            var sqlVerificarOrcamentoPossuiProduto = $@"SELECT COUNT(*) > 0 FROM produtos_orcamento po
                WHERE po.IdProdParent IS NOT NULL AND
                    po.IdProdParent > 0 AND
                    po.IdOrcamento = {idOrcamento};";

            return this.ExecuteScalar<bool>(session, sqlVerificarOrcamentoPossuiProduto);
        }

        /// <summary>
        /// Verifica se o orçamento possui produtos a serem negociados.
        /// </summary>
        /// <param name="session">session.</param>
        /// <param name="idOrcamento">idOrcamento.</param>
        /// <returns>True: o orçamento possui produtos a serem negociados;
        /// False: o orçamento não possui produtos a serem negociados.</returns>
        public bool VerificarOrcamentoPossuiProdutosANegociar(GDASession session, int idOrcamento)
        {
            if (idOrcamento == 0)
            {
                return false;
            }

            var sqlVerificarOrcamentoPossuiProdutoANegociar = $@"SELECT COUNT(*) > 0 FROM produtos_orcamento po
                WHERE po.IdProdParent IS NOT NULL AND
                    po.IdProdParent > 0 AND
                    (po.IdAmbientePedido IS NULL OR po.IdAmbientePedido = 0) AND
                    po.Negociar AND
                    po.IdOrcamento = {idOrcamento};";

            return this.ExecuteScalar<bool>(session, sqlVerificarOrcamentoPossuiProdutoANegociar);
        }

        /// <summary>
        /// Verifica se o orçamento está negociado parcialmente.
        /// </summary>
        /// <param name="session">session.</param>
        /// <param name="idOrcamento">idOrcamento.</param>
        /// <returns>True: o orçamento está negociado parcialmente;
        /// False: o orçamento não está negociado parcialmente.</returns>
        public bool VerificarNegociadoParcialmente(GDASession session, int idOrcamento)
        {
            if (idOrcamento == 0)
            {
                return false;
            }

            var sql = $@"SELECT COUNT(*) FROM produtos_orcamento po
                WHERE (po.IdProdParent IS NULL OR po.IdProdParent = 0) AND
                    po.IdOrcamento = {idOrcamento}";
            var numeroProdutos = this.objPersistence.ExecuteSqlQueryCount(session, sql);

            sql += @" AND po.IdAmbientePedido IS NOT NULL AND
                po.IdAmbientePedido > 0;";
            var numeroProdutosNegociados = this.objPersistence.ExecuteSqlQueryCount(session, sql);

            return numeroProdutosNegociados > 0 && numeroProdutos != numeroProdutosNegociados;
        }

        #endregion

        #region Obtém campos do orcamento

        /// <summary>
        /// Obtém o ID da parcela selecionada no orçamento.
        /// </summary>
        /// <param name="session">session.</param>
        /// <param name="idOrcamento">idOrcamento.</param>
        /// <returns>Retorna o ID da parcela selecionada no orçamento.</returns>
        public int? ObterIdParcela(GDASession session, int idOrcamento)
        {
            return this.ObtemValorCampo<int?>(session, "IdParcela", $"IdOrcamento = {idOrcamento}");
        }

        /// <summary>
        /// Obtém o tipo de venda selecionado no orçamento.
        /// </summary>
        /// <param name="session">session.</param>
        /// <param name="idOrcamento">idOrcamento.</param>
        /// <returns>Retorna o tipo de venda selecionado no orçamento.</returns>
        public int? ObterTipoVenda(GDASession session, int idOrcamento)
        {
            return this.ObtemValorCampo<int?>(session, "TipoVenda", $"IdOrcamento = {idOrcamento}");
        }

        /// <summary>
        /// Obtém uma lista com o ID dos pedidos associados ao orçamento.
        /// </summary>
        /// <param name="session">session.</param>
        /// <param name="idOrcamento">idOrcamento.</param>
        /// <returns>Retorna uma lista com o ID de cada pedido associado ao orçamento.</returns>
        public List<int> ObterIdsPedidoGerado(GDASession session, int idOrcamento)
        {
            if (idOrcamento == 0 || !this.Exists(session, idOrcamento))
            {
                return new List<int>();
            }

            var sqlIdsPedidoGeradoOrcamento = $@"SELECT DISTINCT(p.IdPedido) FROM pedido p
                WHERE p.Situacao <> {(int)Pedido.SituacaoPedido.Cancelado} AND
                    p.IdOrcamento = {idOrcamento}";

            return this.ExecuteMultipleScalar<int>(session, sqlIdsPedidoGeradoOrcamento);
        }

        /// <summary>
        /// Obtém o valor total do orçamento.
        /// </summary>
        /// <param name="sessao">sessao.</param>
        /// <param name="idOrcamento">idOrcamento.</param>
        /// <returns>Retorna o valor total do orçamento.</returns>
        public decimal ObterTotal(GDASession sessao, int idOrcamento)
        {
            if (idOrcamento == 0)
            {
                return 0;
            }

            return this.ObtemValorCampo<decimal>(sessao, "Total", $"IdOrcamento = {idOrcamento}");
        }

        /// <summary>
        /// Obtém a loja do orçamento.
        /// </summary>
        /// <param name="sessao">sessao.</param>
        /// <param name="idOrcamento">idOrcamento.</param>
        /// <returns>Retorna o ID da loja associada ao orçamento.</returns>
        public int ObterIdLoja(GDASession sessao, int idOrcamento)
        {
            if (idOrcamento == 0)
            {
                return 0;
            }

            return this.ObtemValorCampo<int>(sessao, "IdLoja", $"IdOrcamento = {idOrcamento}");
        }

        /// <summary>
        /// Obtém o comissionado do orçamento.
        /// </summary>
        /// <param name="sessao">sessao.</param>
        /// <param name="idOrcamento">idOrcamento.</param>
        /// <returns>Retorna o ID do comissionado associado ao orçamento.</returns>
        public int? ObterIdComissionado(GDASession sessao, int idOrcamento)
        {
            if (idOrcamento == 0)
            {
                return null;
            }

            return this.ObtemValorCampo<int?>(sessao, "IdComissionado", $"IdOrcamento = {idOrcamento}");
        }

        /// <summary>
        /// Obtém o percentual de comissão do orçamento.
        /// </summary>
        /// <param name="sessao">sessao.</param>
        /// <param name="idOrcamento">idOrcamento.</param>
        /// <returns>Retorna o percentual de comissão do orçamento.</returns>
        public float ObterPercentualComissao(GDASession sessao, int idOrcamento)
        {
            if (idOrcamento == 0)
            {
                return 0;
            }

            return this.ObtemValorCampo<float>(sessao, "PercComissao", $"IdOrcamento = {idOrcamento}");
        }

        /// <summary>
        /// Obtém a quantidade de peças dentro do orçamento.
        /// </summary>
        /// <param name="session">session.</param>
        /// <param name="idOrcamento">idOrcamento.</param>
        /// <returns>Retorna a quantidade de peças asspcoadas ao orçamento informado.</returns>
        public int ObterQuantidadePecas(GDASession session, int idOrcamento)
        {
            if (idOrcamento == 0)
            {
                return 0;
            }

            var sql = $@"SELECT CAST(SUM(COALESCE(po.Qtde, 0)) AS SIGNED INTEGER) FROM produtos_orcamento po
                    LEFT JOIN produto p ON (po.IdProduto = p.IdProd)
                WHERE po.IdOrcamento=?id AND p.IdGrupoProd = {(int)NomeGrupoProd.Vidro}";

            return this.ExecuteScalar<int>(session, sql, new GDAParameter("?id", idOrcamento));
        }

        /// <summary>
        /// Obtém o ID de cada orçamento associado aos pedidos informados.
        /// </summary>
        /// <param name="session">session.</param>
        /// <param name="idsPedidos">s.</param>
        /// <returns>Retorna uma string separada por vírgula, onde cada ID é referente
        /// a um ID de orçamento associado a pelo menos um pedido informado por padrâmetro.</returns>
        public string ObterIdsOrcamento(GDASession session, string idsPedidos)
        {
            if (string.IsNullOrWhiteSpace(idsPedidos))
            {
                return string.Empty;
            }

            return this.ExecuteScalar<string>(session, $"SELECT CAST(GROUP_CONCAT(COALESCE(IdOrcamento, '')) AS CHAR) FROM pedido WHERE IdPedido IN ({idsPedidos})");
        }

        /// <summary>
        /// Obtém o ID do cliente associado ao orçamento
        /// </summary>
        /// <param name="sessao">sessao.</param>
        /// <param name="idOrcamento">idOrcamento.</param>
        /// <returns>Retorna o ID do cliente associado ao orçamento informado.</returns>
        public int ObterIdCliente(GDASession sessao, int idOrcamento)
        {
            if (idOrcamento == 0)
            {
                return 0;
            }

            return this.ObtemValorCampo<int>(sessao, "IdCliente", $"IdOrcamento = {idOrcamento}");
        }

        /// <summary>
        /// Obtém a situação do orçamento.
        /// </summary>
        /// <param name="sessao">sessao.</param>
        /// <param name="idOrcamento">idOrcamento.</param>
        /// <returns>Retorna a situação do orçamento.</returns>
        public int ObterSituacao(GDASession sessao, int idOrcamento)
        {
            if (idOrcamento == 0)
            {
                return 0;
            }

            return this.ObtemValorCampo<int>(sessao, "Situacao", $"IdOrcamento = {idOrcamento}");
        }

        /// <summary>
        /// Retorna o valor da propriedade que define se os produtos do orçamento devem ser impressos ou não.
        /// </summary>
        /// <param name="session">session.</param>
        /// <param name="idOrcamento">idOrcamento.</param>
        /// <returns>True: o orçamento está marcado para imprimir os produtos;
        /// False: o orçamento não está marcado para imprimir os produtos</returns>
        public bool ObterImprimirProdutosOrcamento(GDASession session, int idOrcamento)
        {
            if (idOrcamento == 0)
            {
                return false;
            }

            return this.ObtemValorCampo<bool>(session, "ImprimirProdutosOrcamento", $"IdOrcamento = {idOrcamento}");
        }

        /// <summary>
        /// Obtém o tipo de entrega do orçamento.
        /// </summary>
        /// <param name="sessao">sessao.</param>
        /// <param name="idOrcamento">idOrcamento.</param>
        /// <returns>Retorna o tipo de entrega do orçamento.</returns>
        public int ObterTipoEntrega(GDASession sessao, int idOrcamento)
        {
            if (idOrcamento == 0)
            {
                return 0;
            }

            return this.ObtemValorCampo<int>(sessao, "TipoEntrega", $"IdOrcamento = {idOrcamento}");
        }

        /// <summary>
        /// Obtém o tipo de orçamento do orçamento informado.
        /// </summary>
        /// <param name="session">session.</param>
        /// <param name="idOrcamento">idOrcamento.</param>
        /// <returns>Retorna o tipo de orçamento do orçamento informado.</returns>
        public int? ObterTipoOrcamento(GDASession session, int idOrcamento)
        {
            if (idOrcamento == 0)
            {
                return 0;
            }

            return this.ObtemValorCampo<int?>(session, "TipoOrcamento", $"IdOrcamento = {idOrcamento}");
        }

        /// <summary>
        /// Obtém o funcionário de desconto do orçamento informado.
        /// </summary>
        /// <param name="sessao">sessao.</param>
        /// <param name="idOrcamento">idOrcamento.</param>
        /// <returns>Retorna o ID do funcionário de desconto.</returns>
        public int? ObterIdFuncDesc(GDASession sessao, int idOrcamento)
        {
            if (idOrcamento == 0)
            {
                return 0;
            }

            return this.ObtemValorCampo<int?>(sessao, "IdFuncDesc", $"IdOrcamento = {idOrcamento}");
        }

        /// <summary>
        /// Obtém o ID de orçamento associado à medição definitiva informada.
        /// </summary>
        /// <param name="session">session.</param>
        /// <param name="idMedicao">idMedicao.</param>
        /// <returns>Retorna o ID do orçamento associado à medição informada.</returns>
        public int? ObterIdOrcamentoPelaMedicaoDefinitiva(GDASession session, int idMedicao)
        {
            if (idMedicao == 0)
            {
                return 0;
            }

            return this.ObtemValorCampo<int?>(session, "IdOrcamento", $"IdMedicaoDefinitiva = {idMedicao}");
        }

        /// <summary>
        /// Obtém o percentual de taxa à praxo do orçamento.
        /// </summary>
        /// <param name="session">session</param>
        /// <param name="idOrcamento">idOrcamento</param>
        /// <returns>Retorna o percentual de taxa à praxo do orçamento.</returns>
        public float ObterTaxaPrazo(GDASession session, int idOrcamento)
        {
            if (idOrcamento == 0)
            {
                return 0;
            }

            return this.ObtemValorCampo<float>(session, "TaxaPrazo", $"IdOrcamento = {idOrcamento}");
        }

        /// <summary>
        /// Obtém o número de parcelas do orçamento.
        /// </summary>
        /// <param name="session">session.</param>
        /// <param name="idOrcamento">idOrcamento.</param>
        /// <returns>Retorna o número de parcelas do orçamento.</returns>
        public int ObterNumeroParcelas(GDASession session, int idOrcamento)
        {
            if (idOrcamento == 0)
            {
                return 0;
            }

            return this.ObtemValorCampo<int>(session, "NumeroParcelas", $"IdOrcamento = {idOrcamento}");
        }

        /// <summary>
        /// Obtém o tipo de desconto do orçamento.
        /// </summary>
        /// <param name="session">session.</param>
        /// <param name="idOrcamento">idOrcamento.</param>
        /// <returns>Retorna o tipo de desconto definido no cadastro do orçamento.</returns>
        public int ObterTipoDesconto(GDASession session, int idOrcamento)
        {
            if (idOrcamento == 0)
            {
                return 0;
            }

            return this.ObtemValorCampo<int>(session, "TipoDesconto", $"IdOrcamento = {idOrcamento}");
        }

        /// <summary>
        /// Obtém o desconto do orçamento.
        /// </summary>
        /// <param name="session">session.</param>
        /// <param name="idOrcamento">idOrcamento.</param>
        /// <returns>Retorna o desconto definido no cadastro do orçamento.</returns>
        public decimal ObterDesconto(GDASession session, int idOrcamento)
        {
            if (idOrcamento == 0)
            {
                return 0;
            }

            return this.ObtemValorCampo<decimal>(session, "Desconto", $"IdOrcamento = {idOrcamento}");
        }

        /// <summary>
        /// Obtém o tipo de acréscimo do orçamento.
        /// </summary>
        /// <param name="session">session.</param>
        /// <param name="idOrcamento">idOrcamento.</param>
        /// <returns>Retorna o tipo de acréscimo definido no cadastro do orçamento.</returns>
        public int ObterTipoAcrescimo(GDASession session, int idOrcamento)
        {
            if (idOrcamento == 0)
            {
                return 0;
            }

            return this.ObtemValorCampo<int>(session, "TipoAcrescimo", $"IdOrcamento = {idOrcamento}");
        }

        /// <summary>
        /// Obtém o acréscimo do orçamento.
        /// </summary>
        /// <param name="session">session.</param>
        /// <param name="idOrcamento">idOrcamento.</param>
        /// <returns>Retorna o acréscimo definido no cadastro do orçamento.</returns>
        public decimal ObterAcrescimo(GDASession session, int idOrcamento)
        {
            if (idOrcamento == 0)
            {
                return 0;
            }

            return this.ObtemValorCampo<decimal>(session, "Acrescimo", $"IdOrcamento = {idOrcamento}");
        }

        /// <summary>
        /// Obtém o total do orçamento sem desconto.
        /// </summary>
        /// <param name="sessao">sessao.</param>
        /// <param name="idOrcamento">idOrcamento.</param>
        /// <param name="total">total.</param>
        /// <returns>Retorna o total do orçamento sem desconto.</returns>
        public decimal ObterTotalSemDesconto(GDASession sessao, int idOrcamento, decimal total)
        {
            if (idOrcamento == 0)
            {
                return 0;
            }

            var descontoOrcamento = this.ObterDescontoOrcamento(sessao, idOrcamento);
            var descontoProdutosOrcamento = this.ObterDescontoProdutos(sessao, idOrcamento);

            return total + descontoProdutosOrcamento + descontoOrcamento;
        }

        /// <summary>
        /// Obtém o total do orçamento sem acréscimo.
        /// </summary>
        /// <param name="session">session.</param>
        /// <param name="idOrcamento">idOrcamento.</param>
        /// <param name="total">total.</param>
        /// <returns>Retorna o total do orçamento sem acréscimo.</returns>
        internal decimal ObterTotalSemAcrescimo(GDASession session, int idOrcamento, decimal total)
        {
            if (idOrcamento == 0)
            {
                return 0;
            }

            var acrescimoOrcamento = this.ObterAcrescimoOrcamento(session, idOrcamento);
            var acrescimoProdutosOrcamento = this.ObterAcrescimoProdutos(session, idOrcamento);

            return total - acrescimoProdutosOrcamento - acrescimoOrcamento;
        }

        /// <summary>
        /// Obtém o total do orçamento sem comissão.
        /// </summary>
        /// <param name="session">session.</param>
        /// <param name="idOrcamento">idOrcamento.</param>
        /// <param name="total">total.</param>
        /// <returns>Retorna o total do orçamento sem comissão.</returns>
        internal decimal ObterTotalSemComissao(GDASession session, int idOrcamento, decimal total)
        {
            if (idOrcamento == 0)
            {
                return 0;
            }

            var valorComissaoOrcamento = this.ObterComissaoOrcamento(session, idOrcamento);

            return total - valorComissaoOrcamento;
        }

        /// <summary>
        /// Obtém o valor total do orçamento sem 
        /// </summary>
        /// <param name="session">session.</param>
        /// <param name="idOrcamento">idOrcamento.</param>
        /// <param name="total">total.</param>
        /// <returns>Retorna o total do orçamento sem desconto e sem taxa à prazo.</returns>
        internal decimal ObterTotalSemDescontoETaxaPrazo(GDASession session, int idOrcamento, decimal total)
        {
            if (idOrcamento == 0)
            {
                return 0;
            }

            var totalSemDesconto = this.ObterTotalSemDesconto(session, idOrcamento, total);
            var taxaPrazo = (decimal)this.ObterTaxaPrazo(session, idOrcamento);
            var valorPrazo = totalSemDesconto * (taxaPrazo / 100);

            return totalSemDesconto - valorPrazo;
        }

        /// <summary>
        /// Obtém o total bruto do orçamento
        /// </summary>
        /// <param name="session">session.</param>
        /// <param name="idOrcamento">idOrcamento.</param>
        /// <returns>Retorna o valor total bruto do orçamento</returns>
        internal decimal ObterTotalBruto(GDASession session, int idOrcamento)
        {
            if (idOrcamento == 0)
            {
                return 0;
            }

            var total = this.ObterTotal(session, idOrcamento);
            var taxaPrazo = (decimal)this.ObterTaxaPrazo(session, idOrcamento);
            var valorPrazo = total * (taxaPrazo / 100);
            var acrescimo = total - this.ObterTotalSemAcrescimo(session, idOrcamento, total);
            var desconto = this.ObterTotalSemDesconto(session, idOrcamento, total) - total;
            var comissao = total - this.ObterTotalSemComissao(session, idOrcamento, total);

            return total - acrescimo + desconto - comissao - valorPrazo;
        }

        /// <summary>
        /// Obtém o valor de comissão do orçamento.
        /// </summary>
        /// <param name="session">session.</param>
        /// <param name="idOrcamento">idOrcamento.</param>
        /// <returns>Retorna o valor de comissão do orçamento.</returns>
        public decimal ObterComissaoOrcamento(GDASession session, int idOrcamento)
        {
            if (idOrcamento == 0)
            {
                return 0;
            }

            var sqlComissaoOrcamento = $@"SELECT COALESCE(SUM(COALESCE(ValorComissao, 0)), 0)
                FROM produtos_orcamento
                WHERE IdProdParent > 0 AND
                    IdOrcamento = {idOrcamento}";

            return this.ExecuteScalar<decimal>(session, sqlComissaoOrcamento);
        }

        /// <summary>
        /// Obtém os dados de comissão, desconto e acréscimo do orçamento.
        /// </summary>
        /// <param name="session">session.</param>
        /// <param name="idOrcamento">idOrcamento.</param>
        /// <param name="tipoDesconto">tipoDesconto.</param>
        /// <param name="desconto">desconto.</param>
        /// <param name="tipoAcrescimo">tipoAcrescimo.</param>
        /// <param name="acrescimo">acrescimo.</param>
        /// <param name="percComissao">percComissao.</param>
        /// <param name="idComissionado">idComissionado.</param>
        public void ObterDadosComissaoDescontoAcrescimo(
            GDASession session,
            int idOrcamento,
            out int tipoDesconto,
            out decimal desconto,
            out int tipoAcrescimo,
            out decimal acrescimo,
            out float percComissao,
            out int? idComissionado)
        {
            tipoDesconto = 0;
            desconto = 0;
            tipoAcrescimo = 0;
            acrescimo = 0;
            percComissao = 0;
            idComissionado = 0;

            if (idOrcamento == 0)
            {
                return;
            }

            tipoDesconto = this.ObterTipoDesconto(session, idOrcamento);
            desconto = this.ObterDesconto(session, idOrcamento);
            tipoAcrescimo = this.ObterTipoAcrescimo(session, idOrcamento);
            acrescimo = this.ObterAcrescimo(session, idOrcamento);
            percComissao = this.ObterPercentualComissao(session, idOrcamento);
            idComissionado = this.ObterIdComissionado(session, idOrcamento);
        }

        #endregion

        #region Remove e aplica comissão, desconto e acréscimo

        /// <summary>
        /// Remove comissão, desconto e acréscimo.
        /// </summary>
        /// <param name="session">session.</param>
        /// <param name="orcamento">orcamento.</param>
        public void RemoveComissaoDescontoAcrescimo(GDASession session, Orcamento orcamento)
        {
            var produtosOrcamento = ProdutosOrcamentoDAO.Instance.ObterProdutosOrcamento(session, (int)orcamento.IdOrcamento, null);
            var removido = false;

            if (this.RemoverComissao(session, orcamento, produtosOrcamento))
            {
                removido = true;
            }

            if (this.RemoverAcrescimo(session, orcamento, produtosOrcamento))
            {
                removido = true;
            }

            if (this.RemoverDesconto(session, orcamento, produtosOrcamento))
            {
                removido = true;
            }

            this.objPersistence.ExecuteCommand(session, $"UPDATE orcamento SET Desconto = 0, Acrescimo = 0 WHERE IdOrcamento = {orcamento.IdOrcamento}");

            this.FinalizarAplicacaoComissaoAcrescimoDesconto(session, orcamento, produtosOrcamento, removido);

            if (removido)
            {
                this.UpdateTotaisOrcamento(session, orcamento, false, false);
            }
        }

        /// <summary>
        /// Remove comissão, desconto e acréscimo.
        /// </summary>
        /// <param name="session">session.</param>
        /// <param name="antigo">antigo.</param>
        /// <param name="novo">novo.</param>
        internal void RemoveComissaoDescontoAcrescimo(GDASession session, Orcamento antigo, Orcamento novo)
        {
            var alterarComissao = antigo.PercComissao != novo.PercComissao;
            var alterarAcrescimo = novo.Acrescimo != antigo.Acrescimo || novo.TipoAcrescimo != antigo.TipoAcrescimo;
            var alterarDesconto = novo.Desconto != antigo.Desconto || novo.TipoDesconto != antigo.TipoDesconto;

            if (alterarAcrescimo || alterarComissao || alterarDesconto)
            {
                var produtosOrcamento = ProdutosOrcamentoDAO.Instance.ObterProdutosOrcamento(session, (int)novo.IdOrcamento, null)
                    .Where(f => !f.TemItensProdutoSession(session))
                    .ToList();

                // Remove a comissão do orçamento.
                if (alterarComissao)
                {
                    this.RemoverComissao(session, novo, produtosOrcamento);
                }

                // Remove o acréscimo do orçamento.
                if (alterarAcrescimo)
                {
                    this.RemoverAcrescimo(session, novo, produtosOrcamento);
                }

                // Remove o desconto do orçamento.
                if (alterarDesconto)
                {
                    this.RemoverDesconto(session, novo, produtosOrcamento);
                }

                this.FinalizarAplicacaoComissaoAcrescimoDesconto(session, novo, produtosOrcamento, true);
                this.UpdateTotaisOrcamento(session, novo, false, false);
            }
        }

        /// <summary>
        /// Aplica comissão, desconto e acréscimo em uma ordem pré-estabelecida.
        /// </summary>
        /// <param name="session">session.</param>
        /// <param name="orcamento">orcamento.</param>
        /// <param name="idComissionado">idComissionado.</param>
        /// <param name="percComissao">percComissao.</param>
        /// <param name="tipoAcrescimo">tipoAcrescimo.</param>
        /// <param name="acrescimo">acrescimo.</param>
        /// <param name="tipoDesconto">tipoDesconto.</param>
        /// <param name="desconto">desconto.</param>
        /// <param name="manterFuncDesc">manterFuncDesc.</param>
        public void AplicaComissaoDescontoAcrescimo(
            GDASession session,
            Orcamento orcamento,
            uint? idComissionado,
            float percComissao,
            int tipoAcrescimo,
            decimal acrescimo,
            int tipoDesconto,
            decimal desconto,
            bool manterFuncDesc)
        {
            orcamento.IdComissionado = idComissionado;
            orcamento.PercComissao = percComissao;
            orcamento.TipoAcrescimo = tipoAcrescimo;
            orcamento.Acrescimo = acrescimo;
            orcamento.TipoDesconto = tipoDesconto;
            orcamento.Desconto = desconto;

            if (percComissao > 0 || acrescimo > 0 || desconto > 0)
            {
                var produtosOrcamento = ProdutosOrcamentoDAO.Instance.ObterProdutosOrcamento(session, (int)orcamento.IdOrcamento, null);

                this.AplicarAcrescimo(session, orcamento, produtosOrcamento);
                this.AplicarDesconto(session, orcamento, produtosOrcamento);
                this.AplicarComissao(session, orcamento, produtosOrcamento);

                var sqlAtualizarComissaoDescontoAcrescimo = $@"UPDATE orcamento
                    SET IdComissionado = ?idComissionado,
                        PercComissao = ?percComissao,
                        TipoDesconto = ?tipoDesconto,
                        Desconto = ?desconto,
                        TipoAcrescimo = ?tipoAcrescimo,
                        Acrescimo = ?acrescimo
                    WHERE IdOrcamento = {orcamento.IdOrcamento};";

                var parametrosAtualizarComissaoDescontoAcrescimo = new List<GDAParameter>
                {
                    new GDAParameter("?idComissionado", idComissionado),
                    new GDAParameter("?percComissao", percComissao),
                    new GDAParameter("?tipoDesconto", tipoDesconto),
                    new GDAParameter("?desconto", desconto),
                    new GDAParameter("?tipoAcrescimo", tipoAcrescimo),
                    new GDAParameter("?acrescimo", acrescimo),
                };

                this.objPersistence.ExecuteCommand(
                    session,
                    sqlAtualizarComissaoDescontoAcrescimo,
                    parametrosAtualizarComissaoDescontoAcrescimo.ToArray());

                this.FinalizarAplicacaoComissaoAcrescimoDesconto(session, orcamento, produtosOrcamento, true, manterFuncDesc);
                this.UpdateTotaisOrcamento(session, orcamento, false, false);
            }
        }

        /// <summary>
        /// Aplica comissão, desconto e acréscimo em uma ordem pré-estabelecida.
        /// </summary>
        /// <param name="session">session.</param>
        /// <param name="antigo">antigo.</param>
        /// <param name="novo">novo.</param>
        internal void AplicaComissaoDescontoAcrescimo(GDASession session, Orcamento antigo, Orcamento novo)
        {
            var alterarAcrescimo = novo.Acrescimo != antigo.Acrescimo || novo.TipoAcrescimo != antigo.TipoAcrescimo;
            var alterarDesconto = novo.Desconto != antigo.Desconto || novo.TipoDesconto != antigo.TipoDesconto;
            var alterarComissao = antigo.PercComissao != novo.PercComissao;

            if (alterarAcrescimo || alterarComissao || alterarDesconto)
            {
                var produtosOrcamento = ProdutosOrcamentoDAO.Instance.ObterProdutosOrcamento(session, (int)novo.IdOrcamento, null);

                // Aplica o acréscimo no orçamento.
                if (alterarAcrescimo)
                {
                    this.AplicarAcrescimo(session, novo, produtosOrcamento);
                }

                // Aplica o desconto no orçamento.
                if (alterarDesconto)
                {
                    this.AplicarDesconto(session, novo, produtosOrcamento);
                }

                // Aplica a comissão no orçamento.
                if (alterarComissao)
                {
                    this.AplicarComissao(session, novo, produtosOrcamento);
                }

                this.FinalizarAplicacaoComissaoAcrescimoDesconto(session, novo, produtosOrcamento, true);
                this.UpdateTotaisOrcamento(session, novo, false, false);
            }
        }

        #endregion

        #region Métodos sobrescritos

        public override uint Insert(Orcamento objInsert)
        {
            return Insert(null, objInsert);
        }

        public override uint Insert(GDASession session, Orcamento objInsert)
        {
            if (Geral.ConsiderarLojaClientePedidoFluxoSistema && objInsert.IdCliente > 0)
            {
                var idLojaCliente = ClienteDAO.Instance.ObtemIdLoja(session, objInsert.IdCliente.Value);

                if (idLojaCliente > 0)
                    objInsert.IdLoja = idLojaCliente;
            }

            if (objInsert.IdFuncionario != null && objInsert.IdLoja.GetValueOrDefault() == 0)
            {
                objInsert.IdLoja = FuncionarioDAO.Instance.ObtemIdLoja(session, objInsert.IdFuncionario.Value);
            }

            if (objInsert.IdCliente > 0)
            {
                var situacao = ClienteDAO.Instance.GetSituacao(session, objInsert.IdCliente.Value);

                if (situacao == (int)SituacaoCliente.Inativo && !OrcamentoConfig.TelaCadastro.PermitirInserirClienteInativoBloqueado)
                    throw new Exception("O cliente informado está inativo.");

                if (situacao == (int)SituacaoCliente.Bloqueado && !OrcamentoConfig.TelaCadastro.PermitirInserirClienteInativoBloqueado)
                    throw new Exception("O cliente informado está bloqueado.");

                if (situacao == (int)SituacaoCliente.Cancelado)
                    throw new Exception("O cliente informado está cancelado.");
            }

            if (!PedidoConfig.Comissao.AlterarPercComissionado)
            {
                if (objInsert.IdComissionado > 0)
                    objInsert.PercComissao = ComissionadoDAO.Instance.ObtemValorCampo<float>(session, "percentual", "idComissionado=" + objInsert.IdComissionado.Value);
                else
                    objInsert.PercComissao = 0;
            }

            objInsert.MostrarTotal = true;
            objInsert.MostrarTotalProd = true;

            objInsert.Usucad = UserInfo.GetUserInfo.CodUser;
            objInsert.DataCad = DateTime.Now;

            objInsert.ImprimirProdutosOrcamento = OrcamentoConfig.ExibirItensProdutosRelatorio;

            if (objInsert.IdCliente > 0)
                objInsert.NomeCliente = ClienteDAO.Instance.GetNome(session, objInsert.IdCliente.Value);

            uint retorno = base.Insert(session, objInsert);
            if (objInsert.NumeroParcelas > 0)
                objPersistence.ExecuteCommand(session, "update orcamento set numeroParcelas=" + objInsert.NumeroParcelas + " where idOrcamento=" + retorno);

            // Associa textos de orçamentos padrões
            TextoOrcamentoDAO.Instance.AssociaTextoOrcamentoPadrao(session, retorno);

            return retorno;
        }

        private static object _atualizarOrcamentoLock = new object();

        public int UpdateComTransacao(Orcamento objUpdate)
        {
            lock(_atualizarOrcamentoLock)
            {
                using (var transaction = new GDATransaction())
                {
                    try
                    {
                        transaction.BeginTransaction();

                        var retorno = Update(transaction, objUpdate);

                        transaction.Commit();
                        transaction.Close();

                        return retorno;
                    }
                    catch
                    {
                        transaction.Rollback();
                        transaction.Close();
                        throw;
                    }
                }
            }
        }

        public override int Update(Orcamento objUpdate)
        {
            return Update(null, objUpdate);
        }

        public override int Update(GDASession session, Orcamento objUpdate)
        {
            FilaOperacoes.AtualizarOrcamento.AguardarVez();

            try
            {
                if (objUpdate.IdCliente > 0)
                {
                    var situacao = ClienteDAO.Instance.GetSituacao(session, objUpdate.IdCliente.Value);

                    if (situacao == (int)SituacaoCliente.Inativo && !OrcamentoConfig.TelaCadastro.PermitirInserirClienteInativoBloqueado)
                        throw new Exception("O cliente informado está inativo.");

                    else if (situacao == (int)SituacaoCliente.Bloqueado && !OrcamentoConfig.TelaCadastro.PermitirInserirClienteInativoBloqueado)
                        throw new Exception("O cliente informado está bloqueado.");

                    else if (situacao == (int)SituacaoCliente.Cancelado)
                        throw new Exception("O cliente informado está cancelado.");
                }

                // Busca o orçamento atual antes de ser atualizado
                Orcamento orcaAntigo = GetElementByPrimaryKey(session, objUpdate.IdOrcamento);
                // Salva no objeto atual o valor total do orçamento porque ao verificar se o desconto aplicado era permitido ou não
                // o valor total estava zerado e estava ocorrendo um erro de tentativa de divisão por zero.
                objUpdate.Total = orcaAntigo.Total;

                bool orcaCobrouAreaMinima = objPersistence.ExecuteSqlQueryCount(session, @"
                    select count(*) from produtos_orcamento po inner join produto p on (po.idProduto=p.idProd) where
                    p.ativarAreaMinima=true and po.idOrcamento=" + objUpdate.IdOrcamento + " and po.totM != po.totMCalc and po.totMCalc=p.areaMinima") > 0;

                if (!PedidoConfig.Comissao.AlterarPercComissionado)
                {
                    if (objUpdate.IdComissionado > 0)
                        objUpdate.PercComissao = ComissionadoDAO.Instance.ObtemValorCampo<float>(session, "percentual", "idComissionado=" + objUpdate.IdComissionado.Value);
                    else
                        objUpdate.PercComissao = 0;
                }

                if (objUpdate.IdFuncionario != null && !OrcamentoConfig.AlterarLojaOrcamento)
                    objUpdate.IdLoja = FuncionarioDAO.Instance.ObtemIdLoja(session, objUpdate.IdFuncionario.Value);

                /* Chamado 16722.
                 * Para que os filtros da tela funcionem corretamente, sempre que a situação for alterada a data de alteração deve ser salva. */
                if (objUpdate.Situacao != ObterSituacao(session, (int)objUpdate.IdOrcamento))
                    objUpdate.DataSituacao = DateTime.Now;

                if (objUpdate.Desconto != orcaAntigo.Desconto)
                {
                    objUpdate.IdFuncDesc = null;
                    objPersistence.ExecuteCommand(session, "Update orcamento Set idFuncDesc=Null Where idOrcamento=" + objUpdate.IdOrcamento);
                }

                string msgErro = String.Empty;
                if (!DescontoPermitido(session, objUpdate, out msgErro))
                    throw new Exception(msgErro);

                // Se a empresa não permite retirar comissionado do pedido, recupera o idComissionado do orçamento direto do banco,
                // pois pode acontecer do usuário deixar duas telas abertas, em uma delas insere o comissionado e imprime o orcamento,
                // na outra mantém o orcamento sem comissionado atualizando esta última para tirar o comissionado
                if (!objUpdate.ExibirLimparComissionado && objUpdate.IdComissionado.GetValueOrDefault(0) == 0)
                    objUpdate.IdComissionado = orcaAntigo.IdComissionado;

                if (objUpdate.IdCliente > 0)
                    objUpdate.NomeCliente = ClienteDAO.Instance.GetNome(session, objUpdate.IdCliente.Value);

                if (objUpdate.IdPedidoGerado.GetValueOrDefault() == 0)
                    objUpdate.IdPedidoGerado = orcaAntigo.IdPedidoGerado;

                // Chamado 57404: Manter hora e minuto de cadastro do orçamento
                if (objUpdate.DataCad.Hour == 0)
                    objUpdate.DataCad.AddHours(orcaAntigo.DataCad.Hour).AddMinutes(orcaAntigo.DataCad.Minute);

                #region Atualiza os valores do orçamento

                /* Chamado 58815. */
                if (objUpdate.IdCliente != orcaAntigo.IdCliente || objUpdate.TipoEntrega != orcaAntigo.TipoEntrega ||
                    objUpdate.Acrescimo != orcaAntigo.Acrescimo || objUpdate.TipoAcrescimo != orcaAntigo.TipoAcrescimo ||
                    objUpdate.Desconto != orcaAntigo.Desconto || objUpdate.TipoDesconto != orcaAntigo.TipoDesconto ||
                    objUpdate.IdComissionado != orcaAntigo.IdComissionado || objUpdate.PercComissao != orcaAntigo.PercComissao ||
                    objUpdate.TipoOrcamento != orcaAntigo.TipoOrcamento ||
                    (orcaCobrouAreaMinima && (!TipoClienteDAO.Instance.CobrarAreaMinima(session, objUpdate.IdCliente.GetValueOrDefault()) ||
                    !TipoClienteDAO.Instance.CobrarAreaMinima(session, orcaAntigo.IdCliente.GetValueOrDefault()))))
                {
                    Dictionary<uint, KeyValuePair<KeyValuePair<int, decimal>, KeyValuePair<int, decimal>>> dadosProd;

                    int tipoDesconto, tipoAcrescimo;
                    decimal desconto, acrescimo;
                    float percComissao;
                    int? idComissionado;

                    RecalcularOrcamento(session, objUpdate, objUpdate.TipoEntrega, (int?)objUpdate.IdCliente, out tipoDesconto,
                        out desconto, out tipoAcrescimo, out acrescimo, out idComissionado, out percComissao, out dadosProd);

                    string dadosAmbientes = ObterDadosOrcamentoRecalcular(tipoDesconto, desconto, tipoAcrescimo, acrescimo,
                        idComissionado, percComissao, dadosProd).Split(';')[7];

                    FinalizarRecalcular(session, objUpdate, tipoDesconto, desconto, tipoAcrescimo, acrescimo, (int?)idComissionado,
                        percComissao, dadosAmbientes, false);

                    /* Chamado 61744. */
                    if (objUpdate.Desconto > 0 && (objUpdate.Desconto != orcaAntigo.Desconto || objUpdate.TipoDesconto != orcaAntigo.TipoDesconto))
                        objPersistence.ExecuteCommand(session, string.Format("UPDATE orcamento SET IdFuncDesc=?idFuncDesc, DataDesc=?dataDesconto WHERE IdOrcamento={0}", objUpdate.IdOrcamento),
                            new GDAParameter("?idFuncDesc", UserInfo.GetUserInfo.CodUser), new GDAParameter("?dataDesconto", DateTime.Now));
                }

                base.Update(session, objUpdate);

                if (VerificarPossuiPedidoGerado(session, (int)objUpdate.IdOrcamento) && orcaAntigo.Situacao == (int)Orcamento.SituacaoOrcamento.Negociado)
                    throw new Exception("Nenhuma alteração pode ser efetuada caso o orçamento possua um pedido gerado.");

                UpdateTotaisOrcamento(session, objUpdate, true, orcaAntigo.Desconto != objUpdate.Desconto || orcaAntigo.TipoDesconto != objUpdate.TipoDesconto);

                #endregion

                LogAlteracaoDAO.Instance.LogOrcamento(session, orcaAntigo, GetElement(session, objUpdate.IdOrcamento), LogAlteracaoDAO.SequenciaObjeto.Atual);

                return 1;
            }
            finally
            {
                FilaOperacoes.AtualizarOrcamento.ProximoFila();
            }
        }

        #endregion

        #region Gerar Orçamento

        /// <summary>
        /// Gerar um orçamento para o projeto passado, retornando o idOrcamento.
        /// </summary>
        public uint GerarOrcamento(uint idProjeto, bool parceiro)
        {
            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    // Busca o projeto
                    Projeto projeto = ProjetoDAO.Instance.GetElementByPrimaryKey(transaction, idProjeto);

                    uint idOrca;
                    Orcamento orca;

                    // Se tiver sido informado o id de algum orçamento no projeto, utiliza-o
                    if (projeto.IdOrcamento > 0)
                    {
                        idOrca = projeto.IdOrcamento.Value;

                        // Se este orçamento já possuir um id de projeto e não o id deste projeto, não permite gerar em cima dele
                        orca = GetElement(transaction, idOrca);
                        if (orca.IdProjeto > 0 && orca.IdProjeto != idProjeto)
                            throw new Exception("O orçamento selecionado já pertence à outro projeto.");

                        // Salva o id do projeto no orçamento escolhido
                        objPersistence.ExecuteCommand(transaction, "Update orcamento set idProjeto=" + idProjeto + " where idorcamento=" + idOrca);

                        // Exclui todos os ambiente/produtos/benef gerados por projeto deste orçamento
                        ProdutosOrcamentoDAO.Instance.DeleteFromProjeto(transaction, projeto.IdOrcamento.Value);

                        // Para cada item projeto
                        foreach (ItemProjeto ip in ItemProjetoDAO.Instance.GetByOrcamento(transaction, projeto.IdOrcamento.Value))
                            ItemProjetoDAO.Instance.Delete(transaction, ip);
                    }
                    else
                    {
                        orca = new Orcamento();
                        orca.NomeCliente = projeto.NomeCliente;
                        orca.IdFuncionario = projeto.IdFunc;
                        orca.IdProjeto = idProjeto;
                        orca.TipoEntrega = projeto.TipoEntrega;
                        orca.Validade = OrcamentoConfig.DadosOrcamento.ValidadeOrcamento;
                        orca.PrazoEntrega = OrcamentoConfig.DadosOrcamento.PrazoEntregaOrcamento;
                        orca.FormaPagto = OrcamentoConfig.DadosOrcamento.FormaPagtoOrcamento;
                        orca.Situacao = 1;
                        orca.Custo = projeto.CustoTotal;
                        orca.IdCliente = projeto.IdCliente;
                        orca.TipoOrcamento = (int)Orcamento.TipoOrcamentoEnum.Venda;

                        // Busca o cliente do projeto
                        if (projeto.IdCliente > 0)
                        {
                            Cliente cliProj = ClienteDAO.Instance.GetElementByPrimaryKey(transaction, projeto.IdCliente.Value);

                            orca.Bairro = cliProj.Bairro;
                            orca.Cidade = CidadeDAO.Instance.GetNome(transaction, (uint?)cliProj.IdCidade);
                            orca.Endereco = cliProj.Endereco + (!String.IsNullOrEmpty(cliProj.Numero) ? ", " + cliProj.Numero : String.Empty);
                            orca.TelCliente = !String.IsNullOrEmpty(cliProj.TelCont) ? cliProj.TelCont : cliProj.TelRes;
                            orca.CelCliente = cliProj.TelCel;
                            orca.Email = cliProj.Email != null ? cliProj.Email.Split(';')[0] : null;
                        }

                        idOrca = Insert(transaction, orca);
                        orca.IdOrcamento = idOrca;
                    }

                    var produtosOrcamento = ProdutosOrcamentoDAO.Instance.ObterProdutosOrcamento(transaction, (int)orca.IdOrcamento, null);

                    bool removido = RemoverComissao(transaction, orca, produtosOrcamento);
                    FinalizarAplicacaoComissaoAcrescimoDesconto(transaction, orca, produtosOrcamento, removido);

                    // Busca os itens_projeto
                    List<ItemProjeto> lstItemProj = ItemProjetoDAO.Instance.GetForOrcamento(transaction, idProjeto);
                    ProdutosOrcamento prodOrca = new ProdutosOrcamento();

                    var possuiVidro = false;

                    // Para cada item_projeto será inserido um produto orçamento
                    foreach (ItemProjeto item in lstItemProj)
                    {
                        // Se o produto for um cálculo de projeto, faz uma cópia para o pedido
                        uint idItemProjeto = ClonaItemProjeto(transaction, item.IdItemProjeto, orca);

                        var materiais = MaterialItemProjetoDAO.Instance.GetByItemProjeto(transaction, idItemProjeto, false);

                        if (parceiro && !possuiVidro && materiais.Any(f => ProdutoDAO.Instance.IsVidro(transaction, (int)f.IdProd)))
                            possuiVidro = true;

                        // Carrega a descrição do orçamento
                        string descricao = UtilsProjeto.FormataTextoOrcamento(transaction, item);

                        prodOrca = new ProdutosOrcamento();
                        prodOrca.IdOrcamento = idOrca;
                        prodOrca.IdItemProjeto = idItemProjeto;
                        prodOrca.Ambiente = item.Ambiente;
                        prodOrca.Descricao = descricao;
                        prodOrca.Qtde = item.Qtde;
                        prodOrca.Total = ItemProjetoDAO.Instance.GetTotalItemProjetoAluminio(transaction, item.IdItemProjeto);
                        prodOrca.ValorProd = prodOrca.Total / (decimal)item.Qtde;
                        prodOrca.Custo = item.CustoTotal;
                        prodOrca.Espessura = item.EspessuraVidro;
                        ProdutosOrcamentoDAO.Instance.Insert(transaction, prodOrca);
                    }

                    /* Chamado 44650. */
                    if (parceiro)
                    {
                        var idLoja = possuiVidro ? ProjetoConfig.TelaCadastroParceiros.IdLojaPorTipoPedidoComVidro : ProjetoConfig.TelaCadastroParceiros.IdLojaPorTipoPedidoComVidro;

                        if (idLoja > 0)
                        {
                            objPersistence.ExecuteCommand(transaction, string.Format("UPDATE projeto SET IdLoja={0} WHERE IdProjeto={1}", idLoja.Value, idProjeto));
                            objPersistence.ExecuteCommand(transaction, string.Format("UPDATE orcamento SET IdLoja={0} WHERE IdOrcamento={1}", idLoja.Value, idOrca));
                        }
                    }

                    // Atualiza o total do orçamento
                    this.UpdateTotaisOrcamento(transaction, orca, false, false);

                    // Salva o id do orçamento gerado no projeto
                    if (projeto.IdOrcamento == null && idOrca > 0)
                        objPersistence.ExecuteCommand(transaction, "Update projeto set idOrcamento=" + idOrca + " Where idProjeto=" + idProjeto);

                    transaction.Commit();
                    transaction.Close();

                    return idOrca;
                }
                catch
                {
                    transaction.Rollback();
                    transaction.Close();
                    throw;
                }
            }
        }

        #endregion

        #region Altera a cor de itens de projeto

        /// <summary>
        /// Altera a cor de itens de projeto.
        /// </summary>
        public void AlteraCorItens(uint? idItemProjetoParam, uint? idOrcamentoParam, uint? idPedidoParam, uint? idPedidoEspelhoParam,
            uint? idCorVidro, uint? idCorAluminio, uint? idCorFerragem, int? tipoEntrega, uint? idCliente, params uint[] idsItensProjeto)
        {
            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    foreach (uint idItemProjeto in idsItensProjeto)
                    {
                        ItemProjeto item = ItemProjetoDAO.Instance.GetElementByPrimaryKey(transaction, idItemProjeto);

                        // Atualiza a cor do vidro no item do projeto
                        if (idCorVidro > 0)
                        {
                            var ids = ProjetoModeloDAO.Instance.ObtemIdsCorVidroArr(transaction, item.IdProjetoModelo);

                            if (ids.Count() > 0 && !ids.Contains(idCorVidro.Value))
                            {
                                var cor = CorVidroDAO.Instance.GetNome(transaction, idCorVidro.Value);
                                var projeto = ProjetoModeloDAO.Instance.ObtemCodigo(transaction, item.IdProjetoModelo);
                                throw new Exception(string.Format("Não é possivel utilizar a cor: {0} no projeto {1}, pois ela não esta vinculada ao mesmo.", cor, projeto));
                            }

                            if (item.IdCorVidro != idCorVidro)
                                item.IdCorVidro = idCorVidro.Value;
                            else
                                idCorVidro = null;
                        }

                        // Atualiza a cor do alumínio no item do projeto
                        if (idCorAluminio > 0)
                        {
                            if (item.IdCorAluminio != idCorAluminio)
                                item.IdCorAluminio = idCorAluminio.Value;
                            else
                                idCorAluminio = null;
                        }

                        // Atualiza a cor da ferragem no item do projeto
                        if (idCorFerragem > 0)
                        {
                            if (item.IdCorFerragem != idCorFerragem)
                                item.IdCorFerragem = idCorFerragem.Value;
                            else
                                idCorFerragem = null;
                        }

                        // Percorre todos os materiais do item
                        foreach (MaterialItemProjeto mip in MaterialItemProjetoDAO.Instance.GetByItemProjeto(transaction, idItemProjeto))
                        {
                            // Variável de controle de alteração
                            bool alterado = false;

                            // Verifica se há alteração da cor do vidro
                            if (idCorVidro > 0 && bool.Parse(mip.IsVidro) && mip.IdPecaItemProj > 0)
                            {
                                // Recupera o produto associado à cor
                                PecaItemProjeto pip = PecaItemProjetoDAO.Instance.GetElementByPrimaryKey(transaction, mip.IdPecaItemProj.Value);
                                uint? idProd = ProdutoProjetoConfigDAO.Instance.GetIdProdVidro(pip.Tipo,
                                    ProjetoModeloDAO.Instance.IsBoxPadrao(item.IdProjetoModelo),
                                    item.EspessuraVidro, idCorVidro.Value);

                                if (idProd == null)
                                    continue;

                                var idsLojaSubgrupoProd = SubgrupoProdDAO.Instance.ObterIdsLojaPeloProduto(transaction, (int)idProd);

                                if (idsLojaSubgrupoProd.Any())
                                {
                                    var idOrcamentoValidacaoLoja = idOrcamentoParam ?? item.IdOrcamento;
                                    var idPedidoValidacaoLoja = idPedidoParam ?? item.IdPedido;
                                    var idPedidoEspelhoValidacaoLoja = idPedidoEspelhoParam ?? item.IdPedidoEspelho;
                                    var idProjetoValidacaoLoja = item.IdProjeto;

                                    var idLoja = idOrcamentoValidacaoLoja > 0 ? GetIdLoja(transaction, idOrcamentoValidacaoLoja.Value) :
                                        idPedidoValidacaoLoja > 0 || idPedidoEspelhoValidacaoLoja > 0 ?
                                            PedidoDAO.Instance.ObtemIdLoja(transaction, (idPedidoValidacaoLoja ?? idPedidoEspelhoValidacaoLoja).GetValueOrDefault()) :
                                        idProjetoValidacaoLoja > 0 ? (uint?)ProjetoDAO.Instance.ObterIdLoja(transaction, (int)idProjetoValidacaoLoja.Value) : 0;

                                    if (idLoja > 0 && !idsLojaSubgrupoProd.Contains((int)idLoja))
                                        continue;
                                    /* Chamado 48322. */
                                    else if (idLoja == 0 && idProjetoValidacaoLoja > 0)
                                        ProjetoDAO.Instance.AtualizarIdLojaProjeto(transaction, (int)idProjetoValidacaoLoja, (int)idsLojaSubgrupoProd.First());
                                }

                                // Altera a cor na peça
                                pip.IdProd = idProd;
                                PecaItemProjetoDAO.Instance.Update(transaction, pip);

                                // Altera a cor no material
                                mip.IdProd = idProd.Value;
                                alterado = true;
                            }

                            // Verifica se há alteração na cor do alumínio
                            else if (idCorAluminio > 0 && bool.Parse(mip.IsAluminio) && mip.IdMaterProjMod > 0)
                            {
                                // Recupera o produto associado à cor
                                MaterialProjetoModelo mpm = MaterialProjetoModeloDAO.Instance.GetElement(mip.IdMaterProjMod.Value);
                                uint? idProd = ProdutoProjetoConfigDAO.Instance.GetIdProd(transaction, mpm.IdProdProj, ProdutoProjeto.TipoProduto.Aluminio, idCorAluminio.Value);
                                if (idProd == null)
                                    continue;

                                var idsLojaSubgrupoProd = SubgrupoProdDAO.Instance.ObterIdsLojaPeloProduto(transaction, (int)idProd);

                                if (idsLojaSubgrupoProd.Any())
                                {
                                    var idOrcamentoValidacaoLoja = idOrcamentoParam ?? item.IdOrcamento;
                                    var idPedidoValidacaoLoja = idPedidoParam ?? item.IdPedido;
                                    var idPedidoEspelhoValidacaoLoja = idPedidoEspelhoParam ?? item.IdPedidoEspelho;
                                    var idProjetoValidacaoLoja = item.IdProjeto;

                                    var idLoja = idOrcamentoValidacaoLoja > 0 ? GetIdLoja(transaction, idOrcamentoValidacaoLoja.Value) :
                                        idPedidoValidacaoLoja > 0 || idPedidoEspelhoValidacaoLoja > 0 ?
                                            PedidoDAO.Instance.ObtemIdLoja(transaction, (idPedidoValidacaoLoja ?? idPedidoEspelhoValidacaoLoja).GetValueOrDefault()) :
                                        idProjetoValidacaoLoja > 0 ? (uint?)ProjetoDAO.Instance.ObterIdLoja(transaction, (int)idProjetoValidacaoLoja.Value) : 0;

                                    if (idLoja > 0 && !idsLojaSubgrupoProd.Contains((int)idLoja))
                                        continue;
                                    /* Chamado 48322. */
                                    else if (idLoja == 0 && idProjetoValidacaoLoja > 0)
                                        ProjetoDAO.Instance.AtualizarIdLojaProjeto(transaction, (int)idProjetoValidacaoLoja, (int)idsLojaSubgrupoProd.First());
                                }

                                // Altera a cor no material
                                mip.IdProd = idProd.Value;
                                alterado = true;
                            }

                            // Verifica se há aleração na cor da ferragem
                            else if (idCorFerragem > 0 && bool.Parse(mip.IsFerragem) && mip.IdMaterProjMod > 0)
                            {
                                // Recupera o produto associado à cor
                                MaterialProjetoModelo mpm = MaterialProjetoModeloDAO.Instance.GetElement(mip.IdMaterProjMod.Value);
                                uint? idProd = ProdutoProjetoConfigDAO.Instance.GetIdProd(transaction, mpm.IdProdProj, ProdutoProjeto.TipoProduto.Ferragem, idCorFerragem.Value);
                                if (idProd == null)
                                    continue;

                                var idsLojaSubgrupoProd = SubgrupoProdDAO.Instance.ObterIdsLojaPeloProduto(transaction, (int)idProd);

                                if (idsLojaSubgrupoProd.Any())
                                {
                                    var idOrcamentoValidacaoLoja = idOrcamentoParam ?? item.IdOrcamento;
                                    var idPedidoValidacaoLoja = idPedidoParam ?? item.IdPedido;
                                    var idPedidoEspelhoValidacaoLoja = idPedidoEspelhoParam ?? item.IdPedidoEspelho;
                                    var idProjetoValidacaoLoja = item.IdProjeto;

                                    var idLoja = idOrcamentoValidacaoLoja > 0 ? GetIdLoja(transaction, idOrcamentoValidacaoLoja.Value) :
                                        idPedidoValidacaoLoja > 0 || idPedidoEspelhoValidacaoLoja > 0 ?
                                            PedidoDAO.Instance.ObtemIdLoja(transaction, (idPedidoValidacaoLoja ?? idPedidoEspelhoValidacaoLoja).GetValueOrDefault()) :
                                        idProjetoValidacaoLoja > 0 ? (uint?)ProjetoDAO.Instance.ObterIdLoja(transaction, (int)idProjetoValidacaoLoja.Value) : 0;

                                    if (idLoja > 0 && !idsLojaSubgrupoProd.Contains((int)idLoja))
                                        continue;
                                    /* Chamado 48322. */
                                    else if (idLoja == 0 && idProjetoValidacaoLoja > 0)
                                        ProjetoDAO.Instance.AtualizarIdLojaProjeto(transaction, (int)idProjetoValidacaoLoja, (int)idsLojaSubgrupoProd.First());
                                }

                                // Altera a cor no material
                                mip.IdProd = idProd.Value;
                                alterado = true;
                            }

                            // Atualiza o material se houve alteração do produto
                            if (alterado)
                            {
                                mip.Valor = ProdutoDAO.Instance.GetValorTabela(transaction, (int)mip.IdProd, tipoEntrega, idCliente, false, item.Reposicao, 0, null, null, (int?)idOrcamentoParam, mip.Altura);
                                MaterialItemProjetoDAO.Instance.Update(transaction, mip);
                            }
                        }

                        // Atualiza o item, recalculando o total
                        ItemProjetoDAO.Instance.Update(transaction, item);

                        #region Update Total Item Projeto

                        ItemProjetoDAO.Instance.UpdateTotalItemProjeto(transaction, idItemProjeto);

                        uint? idProjeto = ItemProjetoDAO.Instance.GetIdProjeto(transaction, idItemProjeto);
                        uint? idOrcamento = ItemProjetoDAO.Instance.GetIdOrcamento(transaction, idItemProjeto);

                        if (idProjeto > 0)
                            ProjetoDAO.Instance.UpdateTotalProjeto(transaction, idProjeto.Value);
                        else if (idOrcamento > 0)
                        {
                            uint idProd = ProdutosOrcamentoDAO.Instance.ObtemValorCampo<uint>(transaction, "idProd", "idItemProjeto=" + idItemProjeto);
                            if (idProd > 0)
                                ProdutosOrcamentoDAO.Instance.UpdateTotaisProdutoOrcamento(transaction, ProdutosOrcamentoDAO.Instance.GetElementByPrimaryKey(transaction, (int)idProd));

                            this.UpdateTotaisOrcamento(transaction, this.GetElementByPrimaryKey(transaction, (int)idOrcamento.Value), false, false);
                        }

                        #endregion
                    }

                    AtualizarOrcaPedido(transaction, idsItensProjeto, idItemProjetoParam, idOrcamentoParam, idPedidoParam, idPedidoEspelhoParam);

                    transaction.Commit();
                    transaction.Close();
                }
                catch(Exception ex)
                {
                    transaction.Rollback();
                    transaction.Close();
                    throw ex;
                }
            }
        }

        private void AtualizarOrcaPedido(GDASession sessao, uint[] idsItensProjetos, uint? idItemProjeto, uint? idOrcamento, uint? idPedido, uint? idPedidoEspelho)
        {
            if (idItemProjeto != null && ItemProjetoDAO.Instance.Exists(sessao, idItemProjeto.Value))
            {
                // Deve ser getelement para buscar o texto do orçamento e não apagar o texto no produto/ambiente
                var item = ItemProjetoDAO.Instance.GetElement(sessao, idItemProjeto.Value);
                AtualizarOrcaPedido(sessao, idsItensProjetos, null, item.IdOrcamento, item.IdPedido, item.IdPedidoEspelho);
            }
            else if (idOrcamento != null)
            {
                foreach (uint id in idsItensProjetos)
                {
                    // Deve ser getelement para buscar o texto do orçamento e não apagar o texto no produto/ambiente
                    var itemProj = ItemProjetoDAO.Instance.GetElement(sessao, id);

                    var orcamento = GetElementByPrimaryKey(sessao, idOrcamento.Value);
                    ProdutosOrcamentoDAO.Instance.InsereAtualizaProdProj(sessao, (int)orcamento.IdOrcamento, null, itemProj, true);
                }
            }
            else if (idPedido != null)
            {
                var pedido = PedidoDAO.Instance.GetElementByPrimaryKey(sessao, idPedido.Value);

                /* Chamado 51998.
                 * Remove e aplica acréscimo/desconto/comissão no pedido somente uma vez.
                 * Antes essa atualização estava demorando muito porque era feita para cada ambiente. */
                #region Remove acréscimo/desconto/comissão do pedido

                var idsAmbientePedido = new List<uint>();

                // Remove acréscimo, desconto e comissão.
                objPersistence.ExecuteCommand(sessao, "UPDATE PEDIDO SET IdComissionado=NULL WHERE IdPedido=" + idPedido);
                PedidoDAO.Instance.RemoveComissaoDescontoAcrescimo(sessao, pedido);

                #endregion

                foreach (uint id in idsItensProjetos)
                {
                    // Deve ser getelement para buscar o texto do orçamento e não apagar o texto no produto/ambiente
                    var itemProj = ItemProjetoDAO.Instance.GetElement(sessao, id);
                    var idAmbientePedido = AmbientePedidoDAO.Instance.GetIdByItemProjeto(id);

                    if (idAmbientePedido > 0)
                        idsAmbientePedido.Add(idAmbientePedido.Value);

                    ProdutosPedidoDAO.Instance.InsereAtualizaProdProj(sessao, pedido, idAmbientePedido, itemProj, true, false, false);
                }

                #region Aplica acréscimo/desconto/comissão do pedido

                // Aplica acréscimo, desconto e comissão.
                PedidoDAO.Instance.AplicaComissaoDescontoAcrescimo(sessao, pedido, Geral.ManterDescontoAdministrador);

                // Aplica acréscimo e desconto no ambiente.
                if (OrcamentoConfig.Desconto.DescontoAcrescimoItensOrcamento && idsAmbientePedido.Count > 0)
                    foreach (var idAmbientePedido in idsAmbientePedido)
                    {
                        var acrescimoAmbiente = AmbientePedidoDAO.Instance.ObterAcrescimo(sessao, idAmbientePedido);
                        var descontoAmbiente = AmbientePedidoDAO.Instance.ObterAcrescimo(sessao, idAmbientePedido);

                        if (acrescimoAmbiente == 0 && descontoAmbiente == 0)
                            continue;

                        var produtosPedido = ProdutosPedidoDAO.Instance.GetByAmbiente(sessao, idAmbientePedido);

                        if (acrescimoAmbiente > 0)
                            AmbientePedidoDAO.Instance.AplicarAcrescimo(sessao, pedido, idAmbientePedido, AmbientePedidoDAO.Instance.ObterTipoAcrescimo(sessao, idAmbientePedido), acrescimoAmbiente, produtosPedido);

                        if (descontoAmbiente > 0)
                            AmbientePedidoDAO.Instance.AplicarDesconto(sessao, pedido, idAmbientePedido, AmbientePedidoDAO.Instance.ObterTipoDesconto(sessao, idAmbientePedido), descontoAmbiente, produtosPedido);

                        AmbientePedidoDAO.Instance.FinalizarAplicacaoAcrescimoDesconto(sessao, pedido, produtosPedido, true);
                    }

                // Atualiza o total do pedido.
                PedidoDAO.Instance.UpdateTotalPedido(sessao, pedido);

                #endregion
            }
            else if (idPedidoEspelho != null)
            {
                foreach (uint id in idsItensProjetos)
                {
                    // Deve ser getelement para buscar o texto do orçamento e não apagar o texto no produto/ambiente
                    var itemProj = ItemProjetoDAO.Instance.GetElement(sessao, id);
                    uint? idAmbientePedidoEspelho = AmbientePedidoEspelhoDAO.Instance.GetIdByItemProjeto(id);
                    ProdutosPedidoEspelhoDAO.Instance.InsereAtualizaProdProj(sessao, idPedidoEspelho.Value, idAmbientePedidoEspelho, itemProj, true);
                }
            }
        }

        #endregion

        #region Recalcular orçamento

        /// <summary>
        /// Recalcula o valor dos produtos do orçamento e o valor do orçamento, considerando descontos, acréscimos e comissão.
        /// </summary>
        /// <param name="idOrcamento">idOrcamento.</param>
        /// <param name="tipoEntregaNovo">tipoEntregaNovo.</param>
        /// <param name="idClienteNovo">idClienteNovo.</param>
        /// <param name="tipoDesconto">tipoDesconto.</param>
        /// <param name="desconto">desconto.</param>
        /// <param name="tipoAcrescimo">tipoAcrescimo.</param>
        /// <param name="acrescimo">acrescimo.</param>
        /// <param name="idComissionado">idComissionado.</param>
        /// <param name="percComissao">percComissao.</param>
        /// <param name="dadosProd">dadosProd.</param>
        public void RecalcularOrcamentoComTransacao(
            int idOrcamento,
            int? tipoEntregaNovo,
            int? idClienteNovo,
            out int tipoDesconto,
            out decimal desconto,
            out int tipoAcrescimo,
            out decimal acrescimo,
            out int? idComissionado,
            out float percComissao,
            out Dictionary<uint, KeyValuePair<KeyValuePair<int, decimal>, KeyValuePair<int, decimal>>> dadosProd)
        {
            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    this.RecalcularOrcamento(
                        transaction,
                        this.GetElementByPrimaryKey(transaction, idOrcamento),
                        tipoEntregaNovo,
                        idClienteNovo,
                        out tipoDesconto,
                        out desconto,
                        out tipoAcrescimo,
                        out acrescimo,
                        out idComissionado,
                        out percComissao,
                        out dadosProd);

                    transaction.Commit();
                    transaction.Close();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    transaction.Close();

                    ErroDAO.Instance.InserirFromException($"RecalcularOrcamento - IdOrcamento: {idOrcamento}", ex);
                    throw ex;
                }
            }
        }

        /// <summary>
        /// Recalcula o valor dos produtos do orçamento e o valor do orçamento, considerando descontos, acréscimos e comissão.
        /// </summary>
        /// <param name="session">session.</param>
        /// <param name="idOrcamento">idOrcamento.</param>
        /// <param name="tipoEntregaNovo">tipoEntregaNovo.</param>
        /// <param name="idClienteNovo">idClienteNovo.</param>
        /// <param name="tipoDesconto">tipoDesconto.</param>
        /// <param name="desconto">desconto.</param>
        /// <param name="tipoAcrescimo">tipoAcrescimo.</param>
        /// <param name="acrescimo">acrescimo.</param>
        /// <param name="idComissionado">idComissionado.</param>
        /// <param name="percComissao">percComissao.</param>
        /// <param name="dadosProd">dadosProd.</param>
        public void RecalcularOrcamento(
            GDASession session,
            Orcamento orcamento,
            int? tipoEntregaNovo,
            int? idClienteNovo,
            out int tipoDesconto,
            out decimal desconto,
            out int tipoAcrescimo,
            out decimal acrescimo,
            out int? idComissionado,
            out float percComissao,
            out Dictionary<uint, KeyValuePair<KeyValuePair<int, decimal>, KeyValuePair<int, decimal>>> dadosProd)
        {
            tipoDesconto = 0;
            desconto = 0;
            tipoAcrescimo = 0;
            acrescimo = 0;
            idComissionado = null;
            percComissao = 0;
            dadosProd = new Dictionary<uint, KeyValuePair<KeyValuePair<int, decimal>, KeyValuePair<int, decimal>>>();

            idClienteNovo = idClienteNovo ?? this.ObterIdCliente(session, (int)orcamento.IdOrcamento);
            tipoEntregaNovo = tipoEntregaNovo ?? this.ObterTipoEntrega(session, (int)orcamento.IdOrcamento);

            ProdutosOrcamentoDAO.Instance.InserirProdutosOrcamentoPelosIdsItemProjeto(session, (int)orcamento.IdOrcamento);

            this.ObterDadosComissaoDescontoAcrescimo(
                session,
                (int)orcamento.IdOrcamento,
                out tipoDesconto,
                out desconto,
                out tipoAcrescimo,
                out acrescimo,
                out percComissao,
                out idComissionado);

            this.RemovePercComissao(session, orcamento.IdOrcamento, true);
            this.RemoveComissaoDescontoAcrescimo(session, orcamento);

            var produtosOrcamentoAmbiente = ProdutosOrcamentoDAO.Instance.ObterProdutosAmbienteOrcamento(session, (int)orcamento.IdOrcamento);
            var produtosOrcamento = ProdutosOrcamentoDAO.Instance.ObterProdutosOrcamento(session, (int)orcamento.IdOrcamento, null);
            var produtosOrcamentoAtualizar = new List<ProdutosOrcamento>();

            produtosOrcamentoAtualizar.AddRange(produtosOrcamentoAmbiente);
            produtosOrcamentoAtualizar.AddRange(produtosOrcamento);

            foreach (var produtoOrcamentoAmbiente in produtosOrcamentoAmbiente)
            {
                if (produtoOrcamentoAmbiente.Desconto <= 0 &&
                    produtoOrcamentoAmbiente.Acrescimo <= 0 &&
                    produtoOrcamentoAmbiente.IdItemProjeto == null)
                {
                    continue;
                }

                var dadosDesconto = new KeyValuePair<int, decimal>(produtoOrcamentoAmbiente.TipoDesconto, produtoOrcamentoAmbiente.Desconto);
                var dadosAcrescimo = new KeyValuePair<int, decimal>(produtoOrcamentoAmbiente.TipoAcrescimo, produtoOrcamentoAmbiente.Acrescimo);
                var dados = new KeyValuePair<KeyValuePair<int, decimal>, KeyValuePair<int, decimal>>(dadosDesconto, dadosAcrescimo);
                dadosProd.Add(produtoOrcamentoAmbiente.IdProd, dados);

                var produtosDoAmbiente = ProdutosOrcamentoDAO.Instance.ObterProdutosOrcamento(session, (int)orcamento.IdOrcamento, (int)produtoOrcamentoAmbiente.IdProd);

                ProdutosOrcamentoDAO.Instance.RemoverDescontoAmbiente(session, produtoOrcamentoAmbiente, orcamento, produtosDoAmbiente);
                ProdutosOrcamentoDAO.Instance.RemoverAcrescimoAmbiente(session, produtoOrcamentoAmbiente, orcamento, produtosDoAmbiente);
                ProdutosOrcamentoDAO.Instance.FinalizarAplicacaoAcrescimoDescontoAmbiente(session, orcamento, produtoOrcamentoAmbiente, produtosDoAmbiente, true);
            }

            foreach (var produtoOrcamento in produtosOrcamentoAtualizar)
            {
                if (produtoOrcamento.TemItensProdutoSession(session))
                {
                    ProdutosOrcamentoDAO.Instance.UpdateTotaisProdutoOrcamento(session, ProdutosOrcamentoDAO.Instance.GetElementByPrimaryKey(session, (int)produtoOrcamento.IdProd));
                    continue;
                }

                ProdutosOrcamentoDAO.Instance.RecalcularValores(session, produtoOrcamento, false, orcamento);
                ProdutosOrcamentoDAO.Instance.UpdateBase(session, produtoOrcamento, orcamento);

                if (produtoOrcamento.IdItemProjeto > 0)
                {
                    var materiaisItemProjeto = MaterialItemProjetoDAO.Instance.GetByItemProjeto(session, produtoOrcamento.IdItemProjeto.Value);

                    foreach (var materialItemProjeto in materiaisItemProjeto)
                    {
                        MaterialItemProjetoDAO.Instance.RecalcularValores(session, materialItemProjeto, orcamento);
                        MaterialItemProjetoDAO.Instance.Update(session, materialItemProjeto);
                    }

                    #region Update Total Item Projeto

                    ItemProjetoDAO.Instance.UpdateTotalItemProjeto(session, produtoOrcamento.IdItemProjeto.Value);

                    var idProjeto = ItemProjetoDAO.Instance.GetIdProjeto(session, produtoOrcamento.IdItemProjeto.Value);
                    var idOrcamentoProjeto = ItemProjetoDAO.Instance.GetIdOrcamento(session, produtoOrcamento.IdItemProjeto.Value);

                    if (idProjeto > 0)
                    {
                        ProjetoDAO.Instance.UpdateTotalProjeto(session, idProjeto);
                    }
                    else if (idOrcamentoProjeto > 0)
                    {
                        var idProd = ProdutosOrcamentoDAO.Instance.ObterIdProdOrcamentoPeloIdItemProjeto(session, (int)produtoOrcamento.IdItemProjeto.Value);

                        if (idProd > 0)
                        {
                            ProdutosOrcamentoDAO.Instance.UpdateTotaisProdutoOrcamento(session, ProdutosOrcamentoDAO.Instance.GetElementByPrimaryKey(session, (uint)idProd));
                        }

                        this.UpdateTotaisOrcamento(session, orcamento, false, false);
                    }

                    #endregion
                }
            }
        }

        /// <summary>
        /// Finaliza o recálculo do orçamento.
        /// </summary>
        /// <param name="idOrcamento">idOrcamento.</param>
        /// <param name="tipoDesconto">tipoDesconto.</param>
        /// <param name="desconto">desconto.</param>
        /// <param name="tipoAcrescimo">tipoAcrescimo.</param>
        /// <param name="acrescimo">acrescimo.</param>
        /// <param name="idComissionado">idComissionado.</param>
        /// <param name="percComissao">percComissao.</param>
        /// <param name="dadosAmbientes">dadosAmbientes.</param>
        public void FinalizarRecalcularComTransacao(
            int idOrcamento,
            int tipoDesconto,
            decimal desconto,
            int tipoAcrescimo,
            decimal acrescimo,
            int? idComissionado,
            float percComissao,
            string dadosAmbientes)
        {
            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    this.FinalizarRecalcular(
                        transaction,
                        this.GetElementByPrimaryKey(transaction, idOrcamento),
                        tipoDesconto,
                        desconto,
                        tipoAcrescimo,
                        acrescimo,
                        idComissionado,
                        percComissao,
                        dadosAmbientes,
                        true);

                    transaction.Commit();
                    transaction.Close();
                }
                catch
                {
                    transaction.Rollback();
                    transaction.Close();
                    throw;
                }
            }
        }

        /// <summary>
        /// Finaliza o recálculo do orçamento.
        /// </summary>
        /// <param name="session">session.</param>
        /// <param name="idOrcamento">idOrcamento.</param>
        /// <param name="tipoDesconto">tipoDesconto.</param>
        /// <param name="desconto">desconto.</param>
        /// <param name="tipoAcrescimo">tipoAcrescimo.</param>
        /// <param name="acrescimo">acrescimo.</param>
        /// <param name="idComissionado">idComissionado.</param>
        /// <param name="percComissao">percComissao.</param>
        /// <param name="dadosAmbientes">dadosAmbientes.</param>
        /// <param name="atualizarOrcamento">atualizarOrcamento.</param>
        public void FinalizarRecalcular(
            GDASession session,
            Orcamento orcamento,
            int tipoDesconto,
            decimal desconto,
            int tipoAcrescimo,
            decimal acrescimo,
            int? idComissionado,
            float percComissao,
            string dadosAmbientes,
            bool atualizarOrcamento)
        {
            // Remove o percentual de comissão dos beneficiamentos do orçamento
            // Para que não sejam aplicados 2 vezes (se o cálculo do valor for feito com o percentual aplicado)
            ProdutoOrcamentoBenefDAO.Instance.RemovePercComissaoBenef(session, orcamento.IdOrcamento, percComissao);
            var produtosAmbienteOrcamento = ProdutosOrcamentoDAO.Instance.ObterProdutosAmbienteOrcamento(session, (int)orcamento.IdOrcamento);

            foreach (var produtoAmbienteOrcamento in produtosAmbienteOrcamento)
            {
                ProdutosOrcamentoDAO.Instance.UpdateTotaisProdutoOrcamento(session, produtoAmbienteOrcamento);
            }

            this.UpdateTotaisOrcamento(session, orcamento, false, false);

            var ambientes = dadosAmbientes.TrimEnd('|').Split('|');

            foreach (var dados in ambientes)
            {
                if (string.IsNullOrWhiteSpace(dados))
                {
                    continue;
                }

                var dadosProd = dados.Split(',');
                var idProd = dadosProd[0].StrParaUint();
                var tipoDescontoProd = dadosProd[1].StrParaInt();
                var descontoProd = dadosProd[2].Replace(".", ",").StrParaDecimal();
                var tipoAcrescimoProd = dadosProd[3].StrParaInt();
                var acrescimoProd = dadosProd[4].Replace(".", ",").StrParaDecimal();
            }

            this.AplicaComissaoDescontoAcrescimo(
                session,
                orcamento,
                (uint?)idComissionado,
                percComissao,
                tipoAcrescimo,
                acrescimo,
                tipoDesconto,
                desconto,
                Geral.ManterDescontoAdministrador);
            this.AtualizarDataRecalcular(session, orcamento.IdOrcamento, DateTime.Now, UserInfo.GetUserInfo.CodUser);

            if (atualizarOrcamento)
            {
                orcamento = this.GetElementByPrimaryKey(session, orcamento.IdOrcamento);
                this.Update(session, orcamento);
            }
        }

        /// <summary>
        /// Recupera os dados dos ambientes do orçamento.
        /// </summary>
        /// <param name="tipoDesconto">tipoDesconto.</param>
        /// <param name="desconto">desconto.</param>
        /// <param name="tipoAcrescimo">tipoAcrescimo.</param>
        /// <param name="acrescimo">acrescimo.</param>
        /// <param name="idComissionado">idComissionado.</param>
        /// <param name="percComissao">percComissao.</param>
        /// <param name="dadosProdutosOrcamento">dadosProdutosOrcamento.</param>
        /// <returns>Retorna uma string com os dados de acréscimo/desconto do orçamento e de seus produtos.</returns>
        public string ObterDadosOrcamentoRecalcular(
            int tipoDesconto,
            decimal desconto,
            int tipoAcrescimo,
            decimal acrescimo,
            int? idComissionado,
            float percComissao,
            Dictionary<uint, KeyValuePair<KeyValuePair<int, decimal>, KeyValuePair<int, decimal>>> dadosProdutosOrcamento)
        {
            var dadosOrcamentoRecalcular = string.Empty;

            foreach (var idProd in dadosProdutosOrcamento.Keys)
            {
                var dadosDesconto = dadosProdutosOrcamento[idProd].Key;
                var dadosAcrescimo = dadosProdutosOrcamento[idProd].Value;

                dadosOrcamentoRecalcular += string.Format(
                    "{0},{1},{2},{3},{4}|",
                    idProd,
                    dadosDesconto.Key,
                    dadosDesconto.Value.ToString().Replace(",", "."),
                    dadosAcrescimo.Key,
                    dadosAcrescimo.Value.ToString().Replace(",", "."));
            }

            var retorno = string.Format(
                "Ok;{0};{1};{2};{3};{4};{5};{6}",
                tipoDesconto,
                desconto.ToString().Replace(",", "."),
                tipoAcrescimo,
                acrescimo.ToString().Replace(",", "."),
                idComissionado > 0 ? idComissionado.ToString() : string.Empty,
                percComissao.ToString().Replace(",", "."),
                dadosOrcamentoRecalcular.TrimEnd('|'));

            return retorno;
        }

        #endregion
    }
}
