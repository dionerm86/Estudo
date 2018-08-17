using System;
using System.Collections.Generic;
using System.Text;
using Glass.Data.Model;
using GDA;
using Glass.Data.Helper;
using Glass.Configuracoes;
using System.Linq;
using Glass.Data.Helper.Calculos;

namespace Glass.Data.DAL
{
    public sealed class OrcamentoDAO : BaseCadastroDAO<Orcamento, OrcamentoDAO>
    {
        //private OrcamentoDAO() { }

        private static readonly object _gerarOrcamentoLock = new object();

        private static object RecalcularOrcamentoLock = new object();

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

        #region Verifica se orçamento existe

        /// <summary>
        /// Verifica se orçamento existe e está em aberto
        /// </summary>
        public bool ExistsOrcamentoEmAberto(uint? idOrcamento)
        {
            return ExistsOrcamentoEmAberto(null, idOrcamento);
        }

        /// <summary>
        /// Verifica se orçamento existe e está em aberto
        /// </summary>
        public bool ExistsOrcamentoEmAberto(GDASession session, uint? idOrcamento)
        {
            if (idOrcamento == null || idOrcamento == 0)
                return false;

            return Glass.Conversoes.StrParaInt(objPersistence.ExecuteScalar(session, "Select Count(*) From orcamento Where situacao=" + (int)Orcamento.SituacaoOrcamento.EmAberto +
                " And idOrcamento=" + idOrcamento).ToString()) > 0;
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
                    l.Fax as FaxLoja, l.Site as emailLoja, c.cpf_cnpj as cpfCnpjCliente, c.rg_escinst as inscEstCliente, c.ObsNfe
                From orcamento o Left Join funcionario f On o.IdFunc=f.idFunc 
                    Left Join loja l On o.idLoja=l.idLoja 
                    Left Join cidade cidLoja On (cidLoja.idCidade=l.idCidade) 
                    Left Join cliente c On (o.idCliente=c.id_Cli)
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

        #region Recupera o total do orçamento

        /// <summary>
        /// Retorna o total do Orçamento
        /// </summary>
        /// <param name="idOrcamento"></param>
        /// <returns></returns>
        public decimal GetTotal(uint idOrcamento)
        {
            return GetTotal(null, idOrcamento);
        }

        /// <summary>
        /// Retorna o total do Orçamento
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idOrcamento"></param>
        /// <returns></returns>
        public decimal GetTotal(GDASession sessao, uint idOrcamento)
        {
            return ObtemValorCampo<decimal>(sessao, "total", "idOrcamento=" + idOrcamento);
        }

        #endregion

        #region Atualiza o peso dos produtos e do orçamento

        /// <summary>
        /// Atualiza o peso dos produtos e do orçamento.
        /// </summary>
        /// <param name="idOrcamento"></param>
        public void AtualizaPeso(GDASession sessao, uint idOrcamento)
        {
            string sql = @"
                update produtos_orcamento po
                    left join (
                        " + Utils.SqlCalcPeso(Utils.TipoCalcPeso.ProdutoOrcamento, idOrcamento, false, false, false) + @"
                    ) as peso on (po.idProd=peso.id)
                set po.peso=coalesce(peso.peso, 0)
                where po.idOrcamento={0};

                update orcamento set peso=coalesce((select sum(peso) from produtos_orcamento 
                where idOrcamento={0}), 0) where idOrcamento={0}";

            objPersistence.ExecuteCommand(sessao, String.Format(sql, idOrcamento));
        }

        #endregion

        #region Atualiza o custo e o total do orçamento

        // Variável de controle do método UpdateTotaisOrcamento
        private static Dictionary<uint, bool> _atualizando = new Dictionary<uint, bool>();

        /// <summary>
        /// Atualiza os totais do orçamento, alterando o percentual da comissão.
        /// </summary>
        /// <param name="idOrca"></param>
        public void UpdateTotaisOrcamento(uint idOrca)
        {
            UpdateTotaisOrcamento(null, GetElementByPrimaryKey(idOrca), false, false);
        }

        /// <summary>
        /// Atualiza os totais do orçamento, alterando o percentual da comissão.
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idOrca"></param>
        internal void UpdateTotaisOrcamento(GDASession sessao, uint idOrca)
        {
            UpdateTotaisOrcamento(sessao, GetElementByPrimaryKey(sessao, idOrca));
        }

        /// <summary>
        /// Atualiza os totais do orçamento, alterando o percentual da comissão.
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="orcamento"></param>
        internal void UpdateTotaisOrcamento(GDASession sessao, Orcamento orcamento)
        {
            UpdateTotaisOrcamento(sessao, orcamento, false, false);
        }

        /// <summary>
        /// Atualiza os totais do orçamento, alterando o percentual da comissão.
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="orca"></param>
        /// <param name="forcarAtualizacao"></param>
        /// <param name="alterouDesconto"></param>
        internal void UpdateTotaisOrcamento(GDASession sessao, Orcamento orca, bool forcarAtualizacao, bool alterouDesconto)
        {
            // Verifica se o usuário está atualizando o total
            if (!_atualizando.ContainsKey(UserInfo.GetUserInfo.CodUser))
                _atualizando.Add(UserInfo.GetUserInfo.CodUser, false);

            if (!forcarAtualizacao && _atualizando[UserInfo.GetUserInfo.CodUser])
                return;
            
            try
            {
                // Define que o usuário está atualizando o total
                _atualizando[UserInfo.GetUserInfo.CodUser] = true;

                var msgErro = String.Empty;
                if (!DescontoPermitido(sessao, orca, out msgErro))
                {
                    var produtosOrcamento = ProdutosOrcamentoDAO.Instance.GetByOrcamento(orca.IdOrcamento, true);
                    var removidos = new List<uint>();

                    // Remove o desconto dos produtos
                    if (RemoverDesconto(sessao, orca, produtosOrcamento))
                        removidos.AddRange(produtosOrcamento.Select(p => p.IdProd));

                    foreach (var parent in produtosOrcamento.Where(p => !p.IdProdParent.HasValue))
                    {
                        var produtos = new List<ProdutosOrcamento>();

                        if (parent.IdItemProjeto == null)
                            produtos.AddRange(produtosOrcamento.Where(p => p.IdProdParent == parent.IdProd));
                        else
                            produtos.Add(parent);

                        if (ProdutosOrcamentoDAO.Instance.RemoverDesconto(sessao, parent, orca, produtos))
                        {
                            removidos.Add(parent.IdProd);
                            removidos.AddRange(produtos.Select(p => p.IdProd));
                        }
                    }

                    var produtosAtualizar = produtosOrcamento
                        .Where(p => removidos.Contains(p.IdProd))
                        .ToList();

                    FinalizarAplicacaoComissaoAcrescimoDesconto(sessao, orca, produtosAtualizar, true);
                    objPersistence.ExecuteCommand(sessao, "update orcamento set Desconto=0 where idOrcamento=" + orca.IdOrcamento);
                }
                else if (alterouDesconto)
                {
                    var tipoDesconto = ObtemValorCampo<int>(sessao, "tipoDesconto", "idOrcamento=" + orca.IdOrcamento);
                    var percDesconto = ObtemValorCampo<decimal>(sessao, "desconto", "idOrcamento=" + orca.IdOrcamento);

                    var idFuncDesc = ObtemIdFuncDesc(sessao, orca.IdOrcamento) ?? UserInfo.GetUserInfo.CodUser;

                    if (tipoDesconto == 2)
                        percDesconto = Orcamento.GetValorPerc(1, tipoDesconto, percDesconto,
                            GetTotalSemDesconto(sessao, orca.IdOrcamento,
                            orca.Total));

                    if (percDesconto > (decimal)OrcamentoConfig.Desconto.GetDescMaxOrcamentoConfigurado)
                        Email.EnviaEmailDescontoMaior(sessao, 0, orca.IdOrcamento, idFuncDesc, (float)percDesconto,
                            OrcamentoConfig.Desconto.GetDescMaxOrcamentoConfigurado);
                }
                
                var sql = "select coalesce(sum(round(custo, 2)), 0) from produtos_orcamento where idProdParent is null and idOrcamento=" + orca.IdOrcamento;
                var custo = ExecuteScalar<decimal>(sessao, sql);

                sql = "select coalesce(sum(round(total, 2)), 0) from produtos_orcamento where idProdParent is null and idOrcamento=" + orca.IdOrcamento;
                var total = ExecuteScalar<decimal>(sessao, sql);

                sql = "update orcamento o set o.custo=?custo, o.total=(?total" + (!PedidoConfig.RatearDescontoProdutos ?
                    @"-if(TipoDesconto=1, (?total*(o.Desconto/100)), o.Desconto) " : "") + ") where idOrcamento=" + orca.IdOrcamento;

                objPersistence.ExecuteCommand(sessao, sql, new GDAParameter("?custo", custo), new GDAParameter("?total", total));
                
                string descontoRateadoImpostos = "0";
                orca.Total = GetTotal(sessao, orca.IdOrcamento);

                if (!PedidoConfig.RatearDescontoProdutos)
                {
                    var dadosAmbientes = (orca as IContainerCalculo).Ambientes.Obter(true)
                        .Cast<ProdutosOrcamento>()
                        .Select(x => new { x.IdProd, Total = x.Total.GetValueOrDefault() });

                    var formata = new Func<decimal, string>(x => x.ToString().Replace(".", "").Replace(",", "."));

                    decimal totalSemDesconto = GetTotalSemDesconto(sessao, orca.IdOrcamento, orca.Total);
                    string selectAmbientes = dadosAmbientes.Count() == 0 ? "select null as idProd, 1 as total" :
                        String.Join(" union all ", dadosAmbientes.Select(x =>
                            String.Format("select {0} as idProd, {1} as total",
                            x.IdProd, formata(x.Total))).ToArray());

                    descontoRateadoImpostos = @"(
                        if(coalesce(o.desconto, 0)=0, 0, if(o.tipoDesconto=1, o.desconto / 100, o.desconto / " + formata(totalSemDesconto) + @") * ({0}.total + coalesce({0}.valorBenef, 0)))) - (
                        if(coalesce(ppo.desconto, 0)=0, 0, if(ppo.tipoDesconto=1, ppo.desconto / 100, ppo.desconto / (select total from (" + selectAmbientes + @") as amb 
                        where idProd=ppo.idProd)) * ({0}.total + coalesce({0}.valorBenef, 0))))";
                }

                var lojaCalculaIpiPedido = false;
                var clienteCalculaIpi = false;
                var lojaCalculaIcmsStPedido = false;
                var clienteCalculaIcmsSt = false;

                if (orca.IdLoja > 0)
                {
                    lojaCalculaIpiPedido = LojaDAO.Instance.ObtemCalculaIpiPedido(sessao, orca.IdLoja.Value);
                    lojaCalculaIcmsStPedido = LojaDAO.Instance.ObtemCalculaIcmsStPedido(sessao, orca.IdLoja.Value);
                }

                if (orca.IdCliente > 0)
                {
                    clienteCalculaIpi = ClienteDAO.Instance.IsCobrarIpi(sessao, orca.IdCliente.Value);
                    clienteCalculaIcmsSt = ClienteDAO.Instance.IsCobrarIcmsSt(sessao, orca.IdCliente.Value);
                }

                var calcularIpi = lojaCalculaIpiPedido && clienteCalculaIpi;
                var calcularIcmsSt = lojaCalculaIcmsStPedido && clienteCalculaIcmsSt;

                // Calcula o valor do ICMS do orçamento
                if (calcularIcmsSt)
                {
                    var descontoRateadoMaterial = string.Format(descontoRateadoImpostos, "mip");
                    var descontoRateadoProduto = string.Format(descontoRateadoImpostos, "po");
                    var calcIcmsSt = CalculoIcmsStFactory.ObtemInstancia(sessao, (int?)orca.IdLoja ?? 0, (int?)orca.IdCliente, null, null, null, null, calcularIpi);

                    var idProd = "mip.idProd";
                    var totalProd = "mip.Total + coalesce(mip.ValorBenef, 0)";
                    var aliquotaIcmsSt = "mip.AliqIcms";

                    sql = @"
                        update material_item_projeto mip 
                            inner join produtos_orcamento ppo on (mip.idItemProjeto=ppo.idItemProjeto)
                            inner join orcamento o on (ppo.idOrcamento=o.idOrcamento)
                        {0}
                        where o.idOrcamento=" + orca.IdOrcamento;

                    // Atualiza a Alíquota ICMSST somada ao FCPST com o ajuste do MVA e do IPI. Necessário porque na tela é recuperado e salvo o valor sem FCPST.
                    objPersistence.ExecuteCommand(sessao, string.Format(sql,
                        "SET mip.AliqIcms=Round((" + calcIcmsSt.ObtemSqlAliquotaInternaIcmsSt(sessao, idProd, totalProd, descontoRateadoMaterial, aliquotaIcmsSt, null) + @"), 2)"));
                    // Atualiza o valor do ICMSST calculado com a Alíquota recuperada anteriormente.
                    objPersistence.ExecuteCommand(sessao, string.Format(sql,
                        "SET mip.ValorIcms=(" + calcIcmsSt.ObtemSqlValorIcmsSt(totalProd, descontoRateadoMaterial, aliquotaIcmsSt, null) + @")"));

                    idProd = "po.idProduto";
                    totalProd = "po.Total + coalesce(po.ValorBenef, 0)";
                    aliquotaIcmsSt = "po.AliquotaIcms";

                    sql = @"
                        update produtos_orcamento po 
                            inner join orcamento o on (po.idOrcamento=o.idOrcamento)
                            left join produtos_orcamento ppo on (po.idProdParent=ppo.idProd)
                        {0}
                        where po.idOrcamento=" + orca.IdOrcamento + " and po.idProduto is not null and po.idItemProjeto is null";

                    // Atualiza a Alíquota ICMSST somada ao FCPST com o ajuste do MVA e do IPI. Necessário porque na tela é recuperado e salvo o valor sem FCPST.
                    objPersistence.ExecuteCommand(sessao, string.Format(sql,
                        "SET po.AliquotaIcms=Round((" + calcIcmsSt.ObtemSqlAliquotaInternaIcmsSt(sessao, idProd, totalProd, descontoRateadoProduto, aliquotaIcmsSt, null) + @"), 2)"));
                    // Atualiza o valor do ICMSST calculado com a Alíquota recuperada anteriormente.
                    objPersistence.ExecuteCommand(sessao, string.Format(sql,
                        "SET po.ValorIcms=(" + calcIcmsSt.ObtemSqlValorIcmsSt(totalProd, descontoRateadoProduto, aliquotaIcmsSt, null) + @")"));

                    sql = "update produtos_orcamento po set ValorIcms=((select sum(ValorIcms) from material_item_projeto where idItemProjeto=po.idItemProjeto)), AliquotaIcms=((ValorIcms / Total) * 100) where idOrcamento=" + orca.IdOrcamento + " and idItemProjeto is not null";
                    objPersistence.ExecuteCommand(sessao, sql);

                    sql = "update orcamento set AliquotaIcms=Round((select sum(coalesce(AliquotaIcms, 0)) from produtos_orcamento where idOrcamento=" + orca.IdOrcamento + ") / " +
                        " (select Greatest(count(*), 1) from produtos_orcamento where idOrcamento=" + orca.IdOrcamento + " and AliquotaIcms>0) " +
                        ", 2) where idOrcamento=" + orca.IdOrcamento;
                    objPersistence.ExecuteCommand(sessao, sql);

                    sql = "update orcamento set ValorIcms=Round((select sum(coalesce(ValorIcms, 0)) from produtos_orcamento where IdOrcamento=" + orca.IdOrcamento + "), 2), Total=(Total + ValorIcms) where idOrcamento=" + orca.IdOrcamento;
                    objPersistence.ExecuteCommand(sessao, sql);
                }
                else
                {
                    sql = string.Format(@"UPDATE material_item_projeto mip
                            INNER JOIN item_projeto ip ON (mip.IdItemProjeto=ip.IdItemProjeto)
                        SET AliqIcms=0, ValorIcms=0 WHERE ip.IdOrcamento={0}", orca.IdOrcamento);
                    objPersistence.ExecuteCommand(sessao, sql);

                    sql = "update produtos_orcamento po set AliquotaIcms=0, ValorIcms=0 where idOrcamento=" + orca.IdOrcamento;
                    objPersistence.ExecuteCommand(sessao, sql);

                    sql = "update orcamento set AliquotaIcms=0, ValorIcms=0 where idOrcamento=" + orca.IdOrcamento;
                    objPersistence.ExecuteCommand(sessao, sql);
                }

                // Calcula o valor do IPI do orçamento
                if (calcularIpi)
                {
                    string descontoRateadoMaterial = String.Format(descontoRateadoImpostos, "mip");
                    string descontoRateadoProduto = String.Format(descontoRateadoImpostos, "po");

                    sql = @"
                        update material_item_projeto mip 
                            inner join produtos_orcamento ppo on (mip.idItemProjeto=ppo.idItemProjeto)
                            inner join orcamento o on (ppo.idOrcamento=o.idOrcamento)
                        set mip.AliquotaIpi=Round((select aliqIpi from produto where idProd=mip.idProd), 2), 
                            mip.ValorIpi=((mip.Total + coalesce(mip.ValorBenef, 0) - " + descontoRateadoMaterial + @") * 
                                (Coalesce(mip.AliquotaIpi, 0) / 100))
                        where o.idOrcamento=" + orca.IdOrcamento;

                    objPersistence.ExecuteCommand(sessao, sql);

                    sql = @"
                        update produtos_orcamento po 
                            inner join orcamento o on (po.idOrcamento=o.idOrcamento)
                            left join produtos_orcamento ppo on (po.idProdParent=ppo.idProd)
                        set po.AliquotaIpi=Round((select aliqIpi from produto where idProd=po.idProduto), 2), 
                            po.ValorIpi=((po.Total + coalesce(po.ValorBenef, 0) - " + descontoRateadoProduto + @") * 
                                (Coalesce(po.AliquotaIpi, 0) / 100))
                        where po.idOrcamento=" + orca.IdOrcamento + " and po.idProduto is not null and po.idItemProjeto is null";

                    objPersistence.ExecuteCommand(sessao, sql);

                    sql = "update produtos_orcamento po set ValorIpi=(select sum(ValorIpi) from material_item_projeto where idItemProjeto=po.idItemProjeto), AliquotaIpi=((ValorIpi / Total) * 100) where idOrcamento=" + orca.IdOrcamento + " and idItemProjeto is not null";
                    objPersistence.ExecuteCommand(sessao, sql);

                    sql = "update orcamento set AliquotaIpi=Round((select sum(coalesce(AliquotaIpi, 0)) from produtos_orcamento where idOrcamento=" + orca.IdOrcamento + ") / (select Greatest(count(*), 1) from produtos_orcamento where idOrcamento=" + orca.IdOrcamento + " and AliquotaIpi>0), 2) where idOrcamento=" + orca.IdOrcamento;
                    objPersistence.ExecuteCommand(sessao, sql);

                    sql = "update orcamento set ValorIpi=Round((select sum(coalesce(ValorIpi, 0)) from produtos_orcamento where IdOrcamento=" + orca.IdOrcamento + "), 2), Total=(Total + ValorIpi) where idOrcamento=" + orca.IdOrcamento;
                    objPersistence.ExecuteCommand(sessao, sql);
                }
                else
                {
                    sql = string.Format(@"UPDATE material_item_projeto mip
                            INNER JOIN item_projeto ip ON (mip.IdItemProjeto=ip.IdItemProjeto)
                        SET AliquotaIpi=0, ValorIpi=0 WHERE ip.IdOrcamento={0}", orca.IdOrcamento);
                    objPersistence.ExecuteCommand(sessao, sql);

                    sql = "update produtos_orcamento po set ValorIpi=0, AliquotaIpi=0 where idOrcamento=" + orca.IdOrcamento;
                    objPersistence.ExecuteCommand(sessao, sql);

                    sql = "update orcamento set AliquotaIpi=0, ValorIpi=0 where idOrcamento=" + orca.IdOrcamento;
                    objPersistence.ExecuteCommand(sessao, sql);
                }

                // Atualiza o campo ValorComissao
                sql = @"update orcamento set valorComissao=total*coalesce(percComissao,0)/100 where idOrcamento=" + orca.IdOrcamento;
                objPersistence.ExecuteCommand(sessao, sql);

                int numeroParcelas = ObtemValorCampo<int>(sessao, "numeroParcelas", "idOrcamento=" + orca.IdOrcamento);
                
                sql = "update orcamento set total=total*(1+(taxaPrazo/100)) where idOrcamento=" + orca.IdOrcamento;
                objPersistence.ExecuteCommand(sessao, sql);

                sql = "UPDATE ORCAMENTO SET total = COALESCE(total, 0) + ?frete WHERE idOrcamento=" + orca.IdOrcamento;
                objPersistence.ExecuteCommand(sessao, sql, new GDAParameter("?frete", orca.ValorEntrega));

                // Atualiza o usuário e a data da última alteração
                sql = "update orcamento set usuAlt=" + UserInfo.GetUserInfo.CodUser + ", dataAlt=now() where idOrcamento=" + orca.IdOrcamento;
                objPersistence.ExecuteCommand(sessao, sql);

                // Atualiza o peso
                AtualizaPeso(sessao, orca.IdOrcamento);
            }
            finally
            {
                // Indica que a atualização já acabou
                _atualizando[UserInfo.GetUserInfo.CodUser] = false;
            }
        }

        #endregion

        #region Verifica se o desconto é permitido

        /// <summary>
        /// Verifica se o desconto é permitido.
        /// </summary>
        /// <param name="orca"></param>
        /// <param name="msgErro"></param>
        /// <returns></returns>
        internal bool DescontoPermitido(Orcamento orca, out string msgErro)
        {
            return DescontoPermitido(null, orca, out msgErro);
        }

        /// <summary>
        /// Verifica se o desconto é permitido.
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="orca"></param>
        /// <param name="msgErro"></param>
        /// <returns></returns>
        internal bool DescontoPermitido(GDASession sessao, Orcamento orca, out string msgErro)
        {
            msgErro = "";

            var somaDesconto = "Coalesce((select sum(coalesce(valorDescontoQtde,0)" +
                (PedidoConfig.RatearDescontoProdutos ? "+coalesce(valorDesconto,0)+coalesce(valorDescontoProd,0)" :
                "") + ") from produtos_orcamento where idOrcamento=o.idOrcamento),0)";

            var idFunc = UserInfo.GetUserInfo.CodUser;
            if (Geral.ManterDescontoAdministrador)
                idFunc = ObtemIdFuncDesc(sessao, orca.IdOrcamento).GetValueOrDefault(idFunc);

            var sql = "Select Count(*) from orcamento o Where idOrcamento=" + orca.IdOrcamento + @" And (
                (tipoDesconto=1 And desconto<=" + OrcamentoConfig.Desconto.GetDescontoMaximoOrcamento(sessao, idFunc) + @") Or
                (tipoDesconto=2 And round(Coalesce(desconto/(total+" + somaDesconto + (!PedidoConfig.RatearDescontoProdutos ? "+desconto" : "") + "),0),2)<=(" +
                OrcamentoConfig.Desconto.GetDescontoMaximoOrcamento(sessao, idFunc) + @"/100))
            )";
            // Se o desconto do orçamento não estiver dentro do permitido, já retorna false
            if (ExecuteScalar<int>(sessao, sql) <= 0)
            {
                msgErro = "Desconto acima do permitido";
                return false;
            }

            // Verifica se este orçamento pode ter desconto
            if (orca.Desconto > 0 && FuncionarioDAO.Instance.ObtemIdTipoFunc(sessao, idFunc) != (uint)Utils.TipoFuncionario.Administrador)
            {
                // Obtém o maior desconto aplicado nos ambientes deste orçamento
                var maiorDescPercAmbiente = ProdutosOrcamentoDAO.Instance.ObtemMaiorDesconto(sessao, orca.IdOrcamento);

                if (PedidoConfig.Desconto.ImpedirDescontoSomativo)
                {
                    if (orca.IdCliente > 0 && DescontoAcrescimoClienteDAO.Instance.ClientePossuiDesconto(sessao, orca.IdCliente.Value,
                        orca.IdOrcamento, null, 0, null))
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

                var percDescontoOrca = new decimal();

                if (orca.TipoDesconto == 1)
                    percDescontoOrca = orca.Desconto;
                /* Chamado 22175.
                 * O total sem desconto será 0 quando o orçamento for duplicado e o primeiro projeto estiver sendo clonado,
                 * por isso, deve ser feita essa verificação. */
                else if (orca.TotalSemDesconto > 0)
                    percDescontoOrca = (orca.Desconto / orca.TotalSemDesconto) * 100;

                //if ((decimal)OrcamentoConfig.Desconto.DescontoMaximoOrcamento < percDescontoOrca + maiorDescPercAmbiente)
                if ((decimal)OrcamentoConfig.Desconto.GetDescontoMaximoOrcamento(idFunc) < percDescontoOrca + maiorDescPercAmbiente)
                {
                    msgErro = "Desconto lançado está acima do permitido. Desconto orçamento: " + percDescontoOrca.ToString("N2") +
                        "%, Desconto ambientes: " + maiorDescPercAmbiente.ToString("N2") + "%";

                    return false;
                }
            }

            return true;
        }

        #endregion

        #region Comissão, Acréscimo e Desconto

        #region Comissão

        #region Aplica a comissão no valor dos produtos

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

        #endregion

        #region Remove a comissão no valor dos produtos

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

        #endregion

        #region Recupera o valor da comissão

        public decimal GetComissaoOrcamento(uint idOrcamento)
        {
            return GetComissaoOrcamento(null, idOrcamento);
        }

        public decimal GetComissaoOrcamento(GDASession session, uint idOrcamento)
        {
            string sql = @"select coalesce(sum(coalesce(valorComissao,0)),0) from produtos_orcamento 
                where idOrcamento=" + idOrcamento;

            return ExecuteScalar<decimal>(session, sql);
        }

        #endregion

        #region Métodos de suporte

        /// <summary>
        /// Recupera o percentual de comissão de um orçamento.
        /// </summary>
        /// <param name="idOrcamento"></param>
        /// <returns></returns>
        public float RecuperaPercComissao(uint idOrcamento)
        {
            return RecuperaPercComissao(null, idOrcamento);
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
        /// <param name="idOrcamento"></param>
        public void RemovePercComissao(uint idOrcamento)
        {
            RemovePercComissao(idOrcamento, false);
        }

        /// <summary>
        /// Remove o percentual e o valor da comissão do orçamento.
        /// </summary>
        public void RemovePercComissao(uint idOrcamento, bool removerIdComissionado)
        {
            RemovePercComissao(null, idOrcamento, removerIdComissionado);
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

        #endregion

        #region Acréscimo

        #region Aplica acréscimo no valor dos produtos

        /// <summary>
        /// Aplica acréscimo no valor dos produtos e consequentemente no valor do orçamento
        /// </summary>
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

        #endregion

        #region Remove acréscimo no valor dos produtos

        /// <summary>
        /// Remove acréscimo no valor dos produtos e consequentemente no valor do orçamento
        /// </summary>
        internal bool RemoverAcrescimo(GDASession session, Orcamento orcamento, IEnumerable<ProdutosOrcamento> produtosOrcamento)
        {
            return DescontoAcrescimo.Instance.RemoverAcrescimo(session, orcamento, produtosOrcamento);
        }

        #endregion

        #region Recupera o valor do acréscimo

        public decimal GetAcrescimoProdutos(uint idOrcamento)
        {
            return GetAcrescimoProdutos(null, idOrcamento);
        }

        public decimal GetAcrescimoProdutos(GDASession session, uint idOrcamento)
        {
            var sql = @"select (
                    select coalesce(sum(coalesce(valorAcrescimoProd,0)+coalesce(valorAcrescimoCliente,0)),0)
                    from produtos_orcamento where (idProdParent is not null or idItemProjeto is not null) and idOrcamento={0}
                )+(
                    select coalesce(sum(coalesce(valorAcrescimoProd,0)),0)
                    from produto_orcamento_benef where idProd in (select * from (
                        select idProd from produtos_orcamento where idOrcamento={0}
                    ) as temp)
                )";

            return ExecuteScalar<decimal>(session, String.Format(sql, idOrcamento));
        }

        public decimal GetAcrescimoOrcamento(uint idOrcamento)
        {
            return GetAcrescimoOrcamento(null, idOrcamento);
        }

        public decimal GetAcrescimoOrcamento(GDASession session, uint idOrcamento)
        {
            string sql = @"select (
                    select coalesce(sum(coalesce(valorAcrescimo,0)),0)
                    from produtos_orcamento where idOrcamento={0}
                )+(
                    select coalesce(sum(coalesce(valorAcrescimo,0)),0)
                    from produto_orcamento_benef where idProd in (select * from (
                        select idProd from produtos_orcamento where idOrcamento={0}
                    ) as temp)
                )";

            return ExecuteScalar<decimal>(session, String.Format(sql, idOrcamento));
        }

        #endregion

        #endregion

        #region Desconto

        #region Aplica desconto no valor dos produtos

        /// <summary>
        /// Aplica desconto no valor dos produtos e consequentemente no valor do orçamento
        /// </summary>
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

        #endregion

        #region Remove desconto no valor dos produtos

        /// <summary>
        /// Remove desconto no valor dos produtos e consequentemente no valor do orçamento
        /// </summary>
        internal bool RemoverDesconto(GDASession sessao, Orcamento orcamento, IEnumerable<ProdutosOrcamento> produtosOrcamento)
        {
            return DescontoAcrescimo.Instance.RemoverDesconto(sessao, orcamento, produtosOrcamento);
        }

        #endregion

        #region Recupera o valor do desconto

        public decimal GetDescontoProdutos(uint idOrcamento)
        {
            return GetDescontoProdutos(null, idOrcamento);
        }

        public decimal GetDescontoProdutos(GDASession sessao, uint idOrcamento)
        {
            string sql;

            if (PedidoConfig.RatearDescontoProdutos)
            {
                sql = @"select (
                        select coalesce(sum(coalesce(valorDescontoProd,0)+coalesce(valorDescontoQtde,0)+coalesce(valorDescontoCliente,0)),0)
                        from produtos_orcamento po where idOrcamento={0}
                    )+(
                        select coalesce(sum(coalesce(valorDescontoProd,0)),0)
                        from produto_orcamento_benef where idProd in (select * from (
                            select idProd from produtos_orcamento po where idOrcamento={0}
                        ) as temp)
                    )";
            }
            else
            {
                sql = @"select (
                        select coalesce(sum(coalesce(po.total/pp.totalProd*pp.desconto,0)+coalesce(po.valorDescontoQtde,0)),0)
                        from produtos_orcamento po
                            inner join (
                                select pp.idProd, sum(po.total+coalesce(po.valorBenef,0)) as totalProd, 
                                    pp.desconto*if(pp.tipoDesconto=1, sum(po.total+coalesce(po.valorBenef,0))/100, 1) as desconto
                                from produtos_orcamento po
                                    inner join produtos_orcamento pp on (po.idProdParent=pp.idProd)
                                where po.idOrcamento={0} group by pp.idProd
                            ) as pp on (po.idProdParent=pp.idProd)
                        where po.idOrcamento={0}
                    )+(
                        select coalesce(sum(coalesce(pob.valor/pp.totalProd*pp.desconto,0)),0)
                        from produto_orcamento_benef pob
                            inner join produtos_orcamento po on (pob.idProd=po.idProd)
                            inner join (
                                select pp.idProd, sum(po.total+coalesce(po.valorBenef,0)) as totalProd, 
                                    pp.desconto*if(pp.tipoDesconto=1, sum(po.total+coalesce(po.valorBenef,0))/100, 1) as desconto
                                from produtos_orcamento po
                                    inner join produtos_orcamento pp on (po.idProdParent=pp.idProd)
                                where po.idOrcamento={0} group by pp.idProd
                            ) as pp on (po.idProdParent=pp.idProd)
                        where po.idOrcamento={0}
                    )";
            }

            return ExecuteScalar<decimal>(sessao, String.Format(sql, idOrcamento +(!Glass.Configuracoes.Geral.NaoVendeVidro() ? 
                " and (po.idProdParent is not null or po.idItemProjeto is not null)" : "")));
        }

        public decimal GetDescontoOrcamento(uint idOrcamento)
        {
            return GetDescontoOrcamento(null, idOrcamento);
        }

        public decimal GetDescontoOrcamento(GDASession sessao, uint idOrcamento)
        {
            string sql;

            if (PedidoConfig.RatearDescontoProdutos)
            {
                sql = @"select (
                        select coalesce(sum(coalesce(valorDesconto,0)),0)
                        from produtos_orcamento where idOrcamento={0}
                    )+(
                        select coalesce(sum(coalesce(valorDesconto,0)),0)
                        from produto_orcamento_benef where idProd in (select * from (
                            select idProd from produtos_orcamento where idOrcamento={0}
                        ) as temp)
                    )";
            }
            else
                sql = string.Format("SELECT IF(TipoDesconto=2, Desconto, (Total - ValorIcms - ValorIpi) / (1 - (Desconto / 100)) * (Desconto / 100)) FROM orcamento WHERE IdOrcamento={0}", idOrcamento);

            return ExecuteScalar<decimal>(sessao, String.Format(sql, idOrcamento + (!Glass.Configuracoes.Geral.NaoVendeVidro() ? 
                " and (idProdParent is not null or idItemProjeto is not null)" : "")));
        }

        #endregion

        #endregion

        #region Finalizar

        internal void FinalizarAplicacaoComissaoAcrescimoDesconto(GDASession sessao, Orcamento orcamento,
            IEnumerable<ProdutosOrcamento> produtosOrcamento, bool atualizar, bool manterFuncDesc = false)
        {
            if (atualizar)
            {
                var idsParentsBuscar = new List<uint>();
                var parents = new List<ProdutosOrcamento>();

                foreach (var produto in produtosOrcamento)
                {
                    ProdutosOrcamentoDAO.Instance.UpdateBase(sessao, produto, orcamento);
                    ProdutosOrcamentoDAO.Instance.AtualizaBenef(sessao, produto.IdProd, produto.Beneficiamentos, orcamento);

                    if (!produto.IdProdParent.HasValue)
                        parents.Add(produto);
                    else if (!parents.Any(p => p.IdProd == produto.IdProdParent.Value) && !idsParentsBuscar.Any(id => id == produto.IdProdParent.Value))
                        idsParentsBuscar.Add(produto.IdProdParent.Value);
                }

                if (idsParentsBuscar.Any())
                {
                    string idsBuscar = string.Join(",", idsParentsBuscar);
                    parents.AddRange(ProdutosOrcamentoDAO.Instance.GetByIds(idsBuscar));
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
                objPersistence.ExecuteCommand(
                    sessao,
                    "update orcamento set idFuncDesc=?f, dataDesc=?d where idOrcamento=" + orcamento.IdOrcamento,
                    new GDAParameter("?f", UserInfo.GetUserInfo.CodUser),
                    new GDAParameter("?d", dataDesc)
                );

                orcamento.IdFuncDesc = UserInfo.GetUserInfo.CodUser;
                //orcamento.DataDesc = dataDesc; --comentado porque campo não existe no mapeamento
            }
        }

        #endregion

        #endregion

        #region Verifica se o orçamento pode ser editado

        public bool EditEnabled(uint idOrcamento)
        {
            string sql = "select count(*) from orcamento where situacao=" + (int)Orcamento.SituacaoOrcamento.NegociadoParcialmente +
                " and idOrcamento=" + idOrcamento; 

            if (objPersistence.ExecuteSqlQueryCount(sql) == 0)
                return true;

            sql = "select count(*) from produtos_orcamento where idOrcamento=" + idOrcamento + " and idProdPed is not null";
            return objPersistence.ExecuteSqlQueryCount(sql) == 0;
        }

        public bool VerificarPodeEditarOrcamento(uint idOrcamento)
        {
            var orcamento = GetElementByPrimaryKey(idOrcamento);

            return VerificarPodeEditarOrcamento(orcamento.IdLoja, orcamento.IdFuncionario, orcamento.IdPedidoGerado, orcamento.Situacao);
        }

        public bool VerificarPodeEditarOrcamento(uint? idLoja, uint? idFuncionario, uint? idPedidoGerado, int situacao)
        { 
            if (idLoja.GetValueOrDefault(0) == 0)
                return true;

            LoginUsuario login = UserInfo.GetUserInfo;
            bool flagVendedor = true;

            // Se for vendedor, só altera pedidos da loja dele
            if (login.TipoUsuario == (uint)Utils.TipoFuncionario.Vendedor)
            {
                flagVendedor = (idLoja == login.IdLoja || login.CodUser == idFuncionario);

                // Verifica se vendedor só pode alterar orçamento dele
                if (!OrcamentoConfig.AlterarOrcamentoVendedor)
                    flagVendedor = login.CodUser == idFuncionario || idFuncionario == null;

                if (OrcamentoConfig.VendedorPodeAlterarOrcamentoQualquerLoja)
                    flagVendedor = true;
            }

            return flagVendedor && ((idPedidoGerado == null || idPedidoGerado <= 0) || situacao == 5);
        }

        #endregion

        #region Verifica se o orçamento foi negociado parcialmente

        public bool IsNegociadoParcialmente(uint idOrcamento)
        {
            return IsNegociadoParcialmente(null, idOrcamento);
        }

        public bool IsNegociadoParcialmente(GDASession session, uint idOrcamento)
        {
            string sql = "select count(*) from produtos_orcamento where idProdParent is null and idOrcamento=" + idOrcamento;
            int numeroProdutos = objPersistence.ExecuteSqlQueryCount(session, sql);

            sql += " and idProdPed is not null";
            int numeroProdutosNegociados = objPersistence.ExecuteSqlQueryCount(session, sql);

            return numeroProdutosNegociados > 0 && numeroProdutos != numeroProdutosNegociados;
        }

        #endregion

        #region Verifica se o orçamento possui pedido gerado

        /// <summary>
        /// Verifica se o orçamento possui pedido gerado
        /// </summary>
        public bool PossuiPedidoGerado(uint idOrcamento)
        {
            return PossuiPedidoGerado(null, idOrcamento);
        }

        /// <summary>
        /// Verifica se o orçamento possui pedido gerado
        /// </summary>
        public bool PossuiPedidoGerado(GDASession session, uint idOrcamento)
        {
            var sql = $"SELECT COUNT(*) FROM pedido WHERE IdOrcamento={ idOrcamento } AND Situacao <> 6";

            return objPersistence.ExecuteSqlQueryCount(session, sql) > 0;
        }

        #endregion

        #region Atualiza a data que o orçamento foi recalculado

        /// <summary>
        /// Atualiza a data que o orçamento foi recalculado.
        /// </summary>
        public void AtualizarDataRecalcular(GDASession session, uint idOrcamento, DateTime? dataRecalcular, uint idFuncRecalcular)
        {
            if (dataRecalcular != null)
            {
                objPersistence.ExecuteCommand(session, "update orcamento set dataRecalcular=?data, idFuncRecalcular=?idFunc where idOrcamento=" + idOrcamento, 
                    new GDAParameter("?data", dataRecalcular), new GDAParameter("?idFunc", idFuncRecalcular));
            }
            else
                objPersistence.ExecuteCommand(session, "update orcamento set dataRecalcular=null, idFuncRecalcular=?idFunc where idOrcamento=" + idOrcamento, 
                    new GDAParameter("?idFunc", idFuncRecalcular));
        }

        #endregion

        #region Duplicar orçamento

        /// <summary>
        /// Duplica um orçamento.
        /// </summary>
        public uint Duplicar(uint idOrcamento)
        {
            uint idOrcamentoNovo = 0;

            var ambientes = new Dictionary<uint, uint>();
            var produtos = new Dictionary<uint, uint>();
            var itensProjeto = new Dictionary<uint, uint>();

            using (var transaction = new GDA.GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    #region Duplica o orçamento

                    var orca = GetElementByPrimaryKey(transaction, idOrcamento);

                    if (orca == null)
                        throw new Exception("Orçamento não encontrado.");

                    orca.IdOrcamento = 0;
                    orca.IdPedidoGerado = null;
                    orca.IdProjeto = null;
                    orca.Situacao = (int) Orcamento.SituacaoOrcamento.EmAberto;
                    orca.UsuAlt = null;
                    orca.DataAlt = null;
                    orca.IdFuncionario = UserInfo.GetUserInfo.CodUser;
                    orca.IdOrcamentoOriginal = idOrcamento;
                    idOrcamentoNovo = Insert(transaction, orca);
                    orca.IdOrcamento = idOrcamentoNovo;

                    #endregion

                    #region Duplica os ambientes

                    if (OrcamentoConfig.AmbienteOrcamento)
                    {
                        foreach (var a in AmbienteOrcamentoDAO.Instance.GetByOrcamento(transaction, idOrcamento))
                        {
                            var idAmbiente = a.IdAmbienteOrca;

                            a.IdAmbienteOrca = 0;
                            a.IdOrcamento = idOrcamentoNovo;
                            var idAmbienteNovo = AmbienteOrcamentoDAO.Instance.Insert(transaction, a);

                            ambientes.Add(idAmbiente, idAmbienteNovo);
                        }
                    }

                    #endregion

                    #region Duplica os itens dos projetos

                    foreach (var i in ItemProjetoDAO.Instance.GetByOrcamento(transaction, idOrcamento))
                    {
                        var idItemProjetoNovo = ClonaItemProjeto(transaction, i.IdItemProjeto, orca);
                        itensProjeto.Add(i.IdItemProjeto, idItemProjetoNovo);
                    }

                    #endregion

                    #region Duplica os produtos

                    var produtosOrcamento = ProdutosOrcamentoDAO.Instance.GetByOrcamento(transaction, idOrcamento, true);
                    foreach (var p in produtosOrcamento.Where(produto => !produto.IdProdParent.HasValue))
                    {
                        var idProduto = p.IdProd;

                        p.IdProd = 0;
                        p.IdOrcamento = idOrcamentoNovo;
                        p.IdAmbienteOrca = p.IdAmbienteOrca > 0 && ambientes.ContainsKey(p.IdAmbienteOrca.Value) ? (uint?) ambientes[p.IdAmbienteOrca.Value] : null;
                        p.IdItemProjeto = p.IdItemProjeto > 0 && itensProjeto.ContainsKey(p.IdItemProjeto.Value) ? (uint?) itensProjeto[p.IdItemProjeto.Value] : null;
                        var idProdutoNovo = ProdutosOrcamentoDAO.Instance.Insert(transaction, p);

                        produtos.Add(idProduto, idProdutoNovo);

                        foreach (var po in produtosOrcamento.Where(produto => produto.IdProdParent == idProduto))
                        {
                            var idProdutoChild = po.IdProd;

                            po.IdProd = 0;
                            po.IdOrcamento = idOrcamentoNovo;
                            po.IdAmbienteOrca = po.IdAmbienteOrca > 0 && ambientes.ContainsKey(po.IdAmbienteOrca.Value) ? (uint?) ambientes[po.IdAmbienteOrca.Value] : null;
                            po.IdProdParent = idProdutoNovo;
                            var idProdutoChildNovo = ProdutosOrcamentoDAO.Instance.Insert(transaction, po);

                            produtos.Add(idProdutoChild, idProdutoChildNovo);
                        }
                    }

                    #endregion

                    // Salva o orçamento novamente para manter os dados sobre desconto, acréscimo e comissão
                    Update(transaction, orca);

                    transaction.Commit();
                    transaction.Close();

                    return idOrcamentoNovo;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    transaction.Close();

                    if(ex.ToString().Contains("BLOQUEIO_ORCAMENTO"))                                         
                        throw new Exception("Foi cadastrado um orçamento com estes mesmos dados recentemente" +
                            " sendo necessário aguardar 1 minuto ou alterar dados do orçamento");
                    else
                        throw new Exception(Glass.MensagemAlerta.FormatErrorMsg("Falha ao duplicar orçamento.", ex));
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
                ProdutosOrcamentoDAO.Instance.UpdateTotaisProdutoOrcamento(session, idProd);

            UpdateTotaisOrcamento(session, orcamento);

            #endregion

            return idItemProjetoOrca;
        }

        #endregion

        #region Obtém campos do orcamento

        public int? ObterIdParcela(GDASession session, int idOrcamento)
        {
            return ObtemValorCampo<int?>(session, "IdParcela", string.Format("IdOrcamento={0}", idOrcamento));
        }

        public int? ObterTipoVenda(GDASession session, int idOrcamento)
        {
            return ObtemValorCampo<int?>(session, "TipoVenda", string.Format("IdOrcamento={0}", idOrcamento));
        }

        /// <summary>
        /// Obtém os orçamentos que os pedidos passados possa ter sido gerados
        /// </summary>
        public string ObtemIdsOrcamento(GDASession session, string idsPedidos)
        {
            string sql = "Select cast(group_concat(Coalesce(idOrcamento, '')) as char) From pedido Where idPedido In (" + idsPedidos + ")";

            object obj = objPersistence.ExecuteScalar(session, sql);

            return obj == null ? String.Empty : obj.ToString();
        }

        public uint? ObtemIdCliente(uint idOrcamento)
        {
            return ObtemIdCliente(null, idOrcamento);
        }

        public uint? ObtemIdCliente(GDASession sessao, uint idOrcamento)
        {
            return ObtemValorCampo<uint?>(sessao, "idCliente", "idOrcamento=" + idOrcamento);
        }

        public int ObtemSituacao(uint idOrcamento)
        {
            return ObtemSituacao(null, idOrcamento);
        }

        public int ObtemSituacao(GDASession sessao, uint idOrcamento)
        {
            return ObtemValorCampo<int>(sessao, "situacao", "idOrcamento=" + idOrcamento);
        }

        public int? ObtemTipoEntrega(uint idOrcamento)
        {
            return ObtemTipoEntrega(null, idOrcamento);
        }

        public int? ObtemTipoEntrega(GDASession sessao, uint idOrcamento)
        {
            return ObtemValorCampo<int?>(sessao, "tipoEntrega", "idOrcamento=" + idOrcamento);
        }

        public int? ObtemTipoOrcamento(uint idOrcamento)
        {
            return ObtemTipoOrcamento(null, idOrcamento);
        }

        public int? ObtemTipoOrcamento(GDASession session, uint idOrcamento)
        {
            return ObtemValorCampo<int?>(session, "tipoOrcamento", "idOrcamento=" + idOrcamento);
        }

        public uint? ObtemIdFuncDesc(uint idOrcamento)
        {
            return ObtemIdFuncDesc(null, idOrcamento);
        }

        public uint? ObtemIdFuncDesc(GDASession sessao, uint idOrcamento)
        {
            return ObtemValorCampo<uint?>(sessao, "idFuncDesc", "idOrcamento=" + idOrcamento);
        }

        public void ObtemDadosComissaoDescontoAcrescimo(uint idOrcamento, out int tipoDesconto,
            out decimal desconto, out int tipoAcrescimo, out decimal acrescimo, out float percComissao,
            out uint? idComissionado)
        {
            ObtemDadosComissaoDescontoAcrescimo(null, idOrcamento, out tipoDesconto, out desconto,
                out tipoAcrescimo, out acrescimo, out percComissao, out idComissionado);
        }

        public void ObtemDadosComissaoDescontoAcrescimo(GDASession session, uint idOrcamento, out int tipoDesconto,
            out decimal desconto, out int tipoAcrescimo, out decimal acrescimo, out float percComissao,
            out uint? idComissionado)
        {
            string where = "idOrcamento=" + idOrcamento;
            tipoDesconto = ObtemValorCampo<int>(session, "tipoDesconto", where);
            desconto = ObtemValorCampo<decimal>(session, "desconto", where);
            tipoAcrescimo = ObtemValorCampo<int>(session, "tipoAcrescimo", where);
            acrescimo = ObtemValorCampo<decimal>(session, "acrescimo", where);
            percComissao = RecuperaPercComissao(session, idOrcamento);
            idComissionado = ObtemValorCampo<uint?>(session, "idComissionado", where);
        }

        internal decimal GetTotalSemDesconto(uint idOrcamento, decimal total)
        {
            return GetTotalSemDesconto(null, idOrcamento, total);
        }

        internal decimal GetTotalSemDesconto(GDASession sessao, uint idOrcamento, decimal total)
        {
            return total + OrcamentoDAO.Instance.GetDescontoProdutos(sessao, idOrcamento) +
                OrcamentoDAO.Instance.GetDescontoOrcamento(sessao, idOrcamento);
        }

        internal decimal GetTotalSemAcrescimo(uint idOrcamento, decimal total)
        {
            return GetTotalSemAcrescimo(null, idOrcamento, total);
        }

        internal decimal GetTotalSemAcrescimo(GDASession session, uint idOrcamento, decimal total)
        {
            return total - OrcamentoDAO.Instance.GetAcrescimoProdutos(session, idOrcamento) -
                OrcamentoDAO.Instance.GetAcrescimoOrcamento(session, idOrcamento);
        }

        internal decimal GetTotalSemComissao(uint idOrcamento, decimal total)
        {
            return GetTotalSemComissao(null, idOrcamento, total);
        }

        internal decimal GetTotalSemComissao(GDASession session, uint idOrcamento, decimal total)
        {
            return total - OrcamentoDAO.Instance.GetComissaoOrcamento(session, idOrcamento);
        }

        internal decimal GetTotalSemDescontoETaxaPrazo(uint idOrcamento, decimal total)
        {
            float taxaPrazo = ObtemValorCampo<float>("taxaPrazo", "idOrcamento=" + idOrcamento);
            return GetTotalSemDesconto(null, idOrcamento, total);
        }

        internal decimal GetTotalBruto(uint idOrcamento)
        {
            return GetTotalBruto(null, idOrcamento);
        }

        internal decimal GetTotalBruto(GDASession session, uint idOrcamento)
        {
            decimal total = GetTotal(session, idOrcamento);
            float taxaPrazo = ObtemValorCampo<float>(session, "taxaPrazo", "idOrcamento=" + idOrcamento);

            decimal acrescimo = total - GetTotalSemAcrescimo(session, idOrcamento, total);
            decimal desconto = GetTotalSemDesconto(session, idOrcamento, total) - total;
            decimal comissao = total - GetTotalSemComissao(session, idOrcamento, total);
            return (total - acrescimo + desconto - comissao);
        }

        public int ObtemQuantidadePecas(uint idOrcamento)
        {
            string sql = @"select cast(sum(coalesce(qtde,0)) as signed integer) from produtos_orcamento pp 
                left join produto p on (pp.idProd=p.idProd) where idOrcamento=?id and p.idGrupoProd=" + (int)Glass.Data.Model.NomeGrupoProd.Vidro;

            object o = objPersistence.ExecuteScalar(sql, new GDAParameter("?id", idOrcamento));

            return o != DBNull.Value ? Convert.ToInt32(o) : 0;
        }

        #endregion

        #region Obtém o total dos orçamentos de um período - Gráfico

        public string GetTotalOrcamentos(uint idLoja, uint idVendedor, int situacao, string dataIni, string dataFim)
        {
            string campos = @"o.idLoja, o.idFunc, cast(Sum(o.Total) as decimal(12,2)) as TotalVenda, f.Nome as NomeVendedor, coalesce(l.nomeFantasia, l.razaoSocial) as NomeLoja, 
                (Right(Concat('0', Cast(Month(o.DataCad) as char), '/', Cast(Year(o.DataCad) as char)), 7)) as DataVenda, 
                o.situacao, '$$$' as Criterio";

            string criterio = String.Empty;

            string sql = @"
                Select " + campos + @" 
                From orcamento o
                    left join funcionario f On (o.idFunc=f.idFunc)
                    left join loja l on (o.idLoja=l.idLoja)
                Where 1";

            if (idLoja > 0)
            {
                sql += " And o.IdLoja=" + idLoja;
                criterio += "Loja: " + LojaDAO.Instance.GetNome(idLoja) + "    ";
            }

            if (idVendedor > 0)
            {
                sql += " And o.idFunc=" + idVendedor;
                criterio += "Vendedor: " + BibliotecaTexto.GetTwoFirstNames(FuncionarioDAO.Instance.GetNome(idVendedor)) + "    ";
            }

            if (situacao > 0)
            {
                sql += " and o.situacao=" + situacao;
                criterio += "Situação: " + Orcamento.GetDescrSituacao(situacao) + "    ";
            }

            if (!String.IsNullOrEmpty(dataIni))
            {
                sql += " And o.DataCad>=?dataIni";
                criterio += "Data Início: " + dataIni + "    ";
            }

            if (!String.IsNullOrEmpty(dataFim))
            {
                sql += " And o.DataCad<=?dataFim";
                criterio += "Data Fim: " + dataFim + "    ";
            }

            sql = "Select sum(temp.TotalVenda) from (" + sql + ") as temp";

            Object retorno = objPersistence.ExecuteScalar(sql.Replace("$$$", criterio), GetParams(dataIni, dataFim, null, null));

            return retorno.ToString();
        }
        #endregion

        #region Remove e aplica comissão, desconto e acréscimo

        #region Remove

        /// <summary>
        /// Remove comissão, desconto e acréscimo.
        /// </summary>
        public void RemoveComissaoDescontoAcrescimo(GDASession session, Orcamento orcamento)
        {
            var produtosOrcamento = ProdutosOrcamentoDAO.Instance.GetByOrcamento(orcamento.IdOrcamento, true)
                .Where(f => !f.TemItensProdutoSession(session))
                .ToList();

            bool removido = false;

            if (RemoverComissao(session, orcamento, produtosOrcamento))
                removido = true;

            if (RemoverAcrescimo(session, orcamento, produtosOrcamento))
                removido = true;

            if (RemoverDesconto(session, orcamento, produtosOrcamento))
                removido = true;

            objPersistence.ExecuteCommand(session, @"update orcamento set desconto=0, 
                acrescimo=0 where idOrcamento=" + orcamento.IdOrcamento);

            FinalizarAplicacaoComissaoAcrescimoDesconto(session, orcamento, produtosOrcamento, removido);

            if (removido)
                UpdateTotaisOrcamento(session, orcamento);
        }

        /// <summary>
        /// Remove comissão, desconto e acréscimo.
        /// </summary>
        internal void RemoveComissaoDescontoAcrescimo(GDASession session, Orcamento antigo, Orcamento novo)
        {
            bool alterarComissao = antigo.PercComissao != novo.PercComissao;
            bool alterarAcrescimo = novo.Acrescimo != antigo.Acrescimo || novo.TipoAcrescimo != antigo.TipoAcrescimo;
            bool alterarDesconto = novo.Desconto != antigo.Desconto || novo.TipoDesconto != antigo.TipoDesconto;

            if (alterarAcrescimo || alterarComissao || alterarDesconto)
            {
                var produtosOrcamento = ProdutosOrcamentoDAO.Instance.GetByOrcamento(novo.IdOrcamento, true)
                    .Where(f => !f.TemItensProdutoSession(session))
                    .ToList();

                // Remove a comissão do orçamento
                if (alterarComissao)
                    RemoverComissao(session, novo, produtosOrcamento);

                // Remove o acréscimo do orçamento
                if (alterarAcrescimo)
                    RemoverAcrescimo(session, novo, produtosOrcamento);

                // Remove o desconto do orçamento
                if (alterarDesconto)
                    RemoverDesconto(session, novo, produtosOrcamento);

                FinalizarAplicacaoComissaoAcrescimoDesconto(session, novo, produtosOrcamento, true);
                UpdateTotaisOrcamento(session, novo);
            }
        }

        #endregion

        #region Aplica

        /// <summary>
        /// Aplica comissão, desconto e acréscimo em uma ordem pré-estabelecida.
        /// </summary>
        public void AplicaComissaoDescontoAcrescimo(GDASession session, Orcamento orcamento, uint? idComissionado,
            float percComissao, int tipoAcrescimo, decimal acrescimo, int tipoDesconto,
            decimal desconto, bool manterFuncDesc)
        {
            orcamento.IdComissionado = idComissionado;
            orcamento.PercComissao = percComissao;
            orcamento.TipoAcrescimo = tipoAcrescimo;
            orcamento.Acrescimo = acrescimo;
            orcamento.TipoDesconto = tipoDesconto;
            orcamento.Desconto = desconto;

            if (percComissao > 0 || acrescimo > 0 || desconto > 0)
            {
                var produtosOrcamento = ProdutosOrcamentoDAO.Instance.GetByOrcamento(orcamento.IdOrcamento, true)
                    .Where(f => !f.TemItensProdutoSession(session))
                    .ToList();

                AplicarAcrescimo(session, orcamento, produtosOrcamento);
                AplicarDesconto(session, orcamento, produtosOrcamento);
                AplicarComissao(session, orcamento, produtosOrcamento);

                objPersistence.ExecuteCommand(
                    session,
                    @"update orcamento set idComissionado=?id, percComissao=?pc, tipoDesconto=?td, desconto=?d, 
                        tipoAcrescimo=?ta, acrescimo=?a where idOrcamento=" + orcamento.IdOrcamento,
                    new GDAParameter("?id", idComissionado),
                    new GDAParameter("?pc", percComissao),
                    new GDAParameter("?td", tipoDesconto),
                    new GDAParameter("?d", desconto),
                    new GDAParameter("?ta", tipoAcrescimo),
                    new GDAParameter("?a", acrescimo)
                );

                FinalizarAplicacaoComissaoAcrescimoDesconto(session, orcamento, produtosOrcamento, true, manterFuncDesc);
                UpdateTotaisOrcamento(session, orcamento);
            }
        }

        /// <summary>
        /// Aplica comissão, desconto e acréscimo em uma ordem pré-estabelecida.
        /// </summary>
        internal void AplicaComissaoDescontoAcrescimo(GDASession session, Orcamento antigo, Orcamento novo)
        {
            bool alterarAcrescimo = novo.Acrescimo != antigo.Acrescimo || novo.TipoAcrescimo != antigo.TipoAcrescimo;
            bool alterarDesconto = novo.Desconto != antigo.Desconto || novo.TipoDesconto != antigo.TipoDesconto;
            bool alterarComissao = antigo.PercComissao != novo.PercComissao;

            if (alterarAcrescimo || alterarComissao || alterarDesconto)
            {
                var produtosOrcamento = ProdutosOrcamentoDAO.Instance.GetByOrcamento(novo.IdOrcamento, true)
                    .Where(f => !f.TemItensProdutoSession(session))
                    .ToList();

                // Aplica o acréscimo no orçamento
                if (alterarAcrescimo)
                    AplicarAcrescimo(session, novo, produtosOrcamento);

                // Aplica o desconto no orçamento
                if (alterarDesconto)
                    AplicarDesconto(session, novo, produtosOrcamento);

                // Aplica a comissão no orçamento
                if (alterarComissao)
                    AplicarComissao(session, novo, produtosOrcamento);

                FinalizarAplicacaoComissaoAcrescimoDesconto(session, novo, produtosOrcamento, true);
                UpdateTotaisOrcamento(session, novo);
            }
        }

        #endregion

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
                if (objUpdate.Situacao != ObtemSituacao(session, objUpdate.IdOrcamento))
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
                    uint? idComissionado;

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

                if (PossuiPedidoGerado(session, objUpdate.IdOrcamento) && orcaAntigo.Situacao == (int)Orcamento.SituacaoOrcamento.Negociado)
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
        /// Gerar um orçamento para o projeto passado, retornando o idOrcamento
        /// </summary>
        public uint GerarOrcamento(uint idProjeto, bool parceiro)
        {
            lock (_gerarOrcamentoLock)
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

                        var produtosOrcamento = ProdutosOrcamentoDAO.Instance.GetByOrcamento(transaction, orca.IdOrcamento, true)
                            .Where(f => !f.TemItensProdutoSession(transaction))
                            .ToList();

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

                            uint? idAmbienteOrca = null;
                            if (OrcamentoConfig.AmbienteOrcamento)
                            {
                                AmbienteOrcamento ambiente = new AmbienteOrcamento();
                                ambiente.Ambiente = item.Ambiente;
                                ambiente.Descricao = descricao;
                                ambiente.IdOrcamento = idOrca;

                                idAmbienteOrca = AmbienteOrcamentoDAO.Instance.Insert(transaction, ambiente);
                            }

                            prodOrca = new ProdutosOrcamento();
                            prodOrca.IdOrcamento = idOrca;
                            prodOrca.IdAmbienteOrca = idAmbienteOrca;
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
                        UpdateTotaisOrcamento(transaction, orca);

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
                                mip.Valor = ProdutoDAO.Instance.GetValorTabela(transaction, (int)mip.IdProd, tipoEntrega, idCliente, false, item.Reposicao, 0, null, null, (int?)idOrcamentoParam);
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
                                ProdutosOrcamentoDAO.Instance.UpdateTotaisProdutoOrcamento(transaction, idProd);

                            UpdateTotaisOrcamento(transaction, idOrcamento.Value);
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
                    uint? idAmbienteOrca = AmbienteOrcamentoDAO.Instance.GetIdByItemProjeto(id);

                    var orcamento = GetElementByPrimaryKey(sessao, idOrcamento.Value);
                    ProdutosOrcamentoDAO.Instance.InsereAtualizaProdProj(sessao, orcamento, idAmbienteOrca, itemProj);
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
        
        public void RecalcularOrcamentoComTransacao(int idOrcamento, int? tipoEntregaNovo, int? idClienteNovo, out int tipoDesconto, out decimal desconto, out int tipoAcrescimo,
            out decimal acrescimo, out uint? idComissionado, out float percComissao, out Dictionary<uint, KeyValuePair<KeyValuePair<int, decimal>, KeyValuePair<int, decimal>>> dadosProd,
            Orcamento orcamento)
        {
            lock (RecalcularOrcamentoLock)
            {
                using (var transaction = new GDATransaction())
                {
                    try
                    {
                        transaction.BeginTransaction();
                        
                        RecalcularOrcamento(transaction, orcamento, tipoEntregaNovo, idClienteNovo, out tipoDesconto, out desconto, out tipoAcrescimo, out acrescimo, out idComissionado,
                            out percComissao, out dadosProd);

                        transaction.Commit();
                        transaction.Close();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        transaction.Close();

                        ErroDAO.Instance.InserirFromException(string.Format("RecalcularOrcamento - IdOrcamento: {0}", idOrcamento), ex);
                        throw ex;
                    }
                }
            }
        }

        public void RecalcularOrcamento(GDASession session, Orcamento orcamento, int? tipoEntregaNovo, int? idClienteNovo,
            out Dictionary<uint, KeyValuePair<KeyValuePair<int, decimal>, KeyValuePair<int, decimal>>> dadosProd)
        {
            int tipoDesconto;
            decimal desconto;
            int tipoAcrescimo;
            decimal acrescimo;
            uint? idComissionado;
            float percComissao;

            RecalcularOrcamento(session, orcamento, tipoEntregaNovo, idClienteNovo, out tipoDesconto, out desconto, out tipoAcrescimo, out acrescimo, out idComissionado, out percComissao,
                out dadosProd);
        }

        private void RecalcularOrcamento(GDASession session, Orcamento orcamento, int? tipoEntregaNovo, int? idClienteNovo, out int tipoDesconto, out decimal desconto, out int tipoAcrescimo,
            out decimal acrescimo, out uint? idComissionado, out float percComissao, out Dictionary<uint, KeyValuePair<KeyValuePair<int, decimal>, KeyValuePair<int, decimal>>> dadosProd)
        {
            tipoDesconto = orcamento.TipoDesconto;
            desconto = orcamento.Desconto;
            tipoAcrescimo = orcamento.TipoAcrescimo;
            acrescimo = orcamento.Acrescimo;
            idComissionado = orcamento.IdComissionado;
            percComissao = orcamento.PercComissao;

            dadosProd = new Dictionary<uint, KeyValuePair<KeyValuePair<int, decimal>, KeyValuePair<int, decimal>>>();

            orcamento.IdCliente = (uint?)idClienteNovo ?? orcamento.IdCliente;
            orcamento.TipoEntrega = tipoEntregaNovo ?? orcamento.TipoEntrega;
            orcamento.PercComissao = 0;
            orcamento.Desconto = 0;
            orcamento.Acrescimo = 0;

            RemovePercComissao(session, orcamento.IdOrcamento, true);
            RemoveComissaoDescontoAcrescimo(session, orcamento);
            
            var produtosOrcamento = ProdutosOrcamentoDAO.Instance.GetByOrcamento(session, orcamento.IdOrcamento, true);

            foreach (var p in produtosOrcamento.Where(p => !p.IdProdParent.HasValue))
            {
                if (p.Desconto <= 0 && p.Acrescimo <= 0)
                    continue;

                var dadosDesconto = new KeyValuePair<int, decimal>(p.TipoDesconto, p.Desconto);
                var dadosAcrescimo = new KeyValuePair<int, decimal>(p.TipoAcrescimo, p.Acrescimo);
                var dados = new KeyValuePair<KeyValuePair<int, decimal>, KeyValuePair<int, decimal>>(dadosDesconto, dadosAcrescimo);
                dadosProd.Add(p.IdProd, dados);

                var produtos = new List<ProdutosOrcamento>();

                if (p.IdItemProjeto == null)
                    produtos.AddRange(ProdutosOrcamentoDAO.Instance.GetByProdutoOrcamento(session, p.IdProd));
                else
                    produtos.Add(p);

                ProdutosOrcamentoDAO.Instance.RemoverDesconto(session, p, orcamento, produtos);
                ProdutosOrcamentoDAO.Instance.RemoverAcrescimo(session, p, orcamento, produtos);
                ProdutosOrcamentoDAO.Instance.FinalizarAplicacaoAcrescimoDesconto(session, orcamento, p, produtos, true);
            }

            //Necessário buscar os produtos atualizados; após o método FinalizarAplicacaoAcrescimoDesconto os produtos em memória não estão atualizados
            produtosOrcamento = ProdutosOrcamentoDAO.Instance.GetByOrcamento(session, orcamento.IdOrcamento, true);

            foreach (var p in produtosOrcamento.Where(p => p.IdProdParent.HasValue))
            {
                if (p.IdProduto > 0)
                {
                    ProdutosOrcamentoDAO.Instance.RecalcularValores(session, p, false, orcamento);
                    ProdutosOrcamentoDAO.Instance.UpdateBase(session, p, orcamento);
                }

                else if (p.IdItemProjeto > 0)
                {
                    foreach (var mip in MaterialItemProjetoDAO.Instance.GetByItemProjeto(session, p.IdItemProjeto.Value))
                    {
                        MaterialItemProjetoDAO.Instance.RecalcularValores(session, mip, orcamento);
                        MaterialItemProjetoDAO.Instance.Update(session, mip, orcamento);
                    }

                    #region Update Total Item Projeto

                    ItemProjetoDAO.Instance.UpdateTotalItemProjeto(session, p.IdItemProjeto.Value);

                    var idProd = ProdutosOrcamentoDAO.Instance.ObtemIdProdutoPorIdItemProjeto(session, p.IdItemProjeto.Value);
                    if (idProd > 0)
                        ProdutosOrcamentoDAO.Instance.UpdateTotaisProdutoOrcamento(session, idProd);

                    #endregion
                }

                if (p.IdProduto.GetValueOrDefault() == 0 && !string.IsNullOrEmpty(p.Ambiente) && !ProdutoOrcamentoPossuiFilhos(session, (int)p.IdProd))
                {
                    p.ValorProd = 0M;
                    p.Total = 0M;
                    ProdutosOrcamentoDAO.Instance.UpdateBase(session, p, orcamento);
                }
            }
            
            /* Chamado 58815. */
            UpdateTotaisOrcamento(session, orcamento);
        }
 
        /// <summary>
        /// Finaliza o recálculo do orçamento.
        /// </summary>
        public void FinalizarRecalcularComTransacao(int idOrcamento, int tipoDesconto, decimal desconto, int tipoAcrescimo, decimal acrescimo, int? idComissionado, float percComissao, string dadosAmbientes,
            Orcamento orcamento)
        {
            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    FinalizarRecalcular(transaction, orcamento, tipoDesconto, desconto, tipoAcrescimo, acrescimo, idComissionado, percComissao, dadosAmbientes, true);

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
        public void FinalizarRecalcular(GDASession session, Orcamento orcamento, int tipoDesconto, decimal desconto, int tipoAcrescimo, decimal acrescimo, int? idComissionado, float percComissao,
            string dadosAmbientes, bool atualizarOrcamento)
        {
            // Remove o percentual de comissão dos beneficiamentos do orçamento
            // Para que não sejam aplicados 2 vezes (se o cálculo do valor for feito com o percentual aplicado)
            ProdutoOrcamentoBenefDAO.Instance.RemovePercComissaoBenef(session, orcamento.IdOrcamento, percComissao);

            var produtosSemFilhos = (orcamento as IContainerCalculo).Ambientes.Obter()
                .Cast<ProdutosOrcamento>();

            foreach (var po in produtosSemFilhos)
                ProdutosOrcamentoDAO.Instance.UpdateTotaisProdutoOrcamento(session, po);

            var ambientes = dadosAmbientes.TrimEnd('|').Split('|');

            foreach (var dados in ambientes)
            {
                if (string.IsNullOrEmpty(dados))
                    continue;

                var dadosProd = dados.Split(',');

                var idProd = dadosProd[0].StrParaUint();
                var tipoDescontoProd = dadosProd[1].StrParaInt();
                var descontoProd = dadosProd[2].Replace(".", ",").StrParaDecimal();
                var tipoAcrescimoProd = dadosProd[3].StrParaInt();
                var acrescimoProd = dadosProd[4].Replace(".", ",").StrParaDecimal();

                var produto = produtosSemFilhos.FirstOrDefault(p => p.IdProd == idProd)
                    ?? ProdutosOrcamentoDAO.Instance.GetElementByPrimaryKey(idProd);

                produto.TipoDesconto = tipoDescontoProd;
                produto.Desconto = descontoProd;
                produto.TipoAcrescimo = tipoAcrescimoProd;
                produto.Acrescimo = acrescimoProd;

                if (descontoProd > 0 || acrescimoProd > 0)
                {
                    var produtos = new List<ProdutosOrcamento>();

                    if (produto.IdItemProjeto == null)
                        produtos.AddRange(ProdutosOrcamentoDAO.Instance.GetByProdutoOrcamento(session, produto.IdProd));
                    else
                        produtos.Add(produto);

                    ProdutosOrcamentoDAO.Instance.AplicarAcrescimo(session, produto, orcamento, produtos);
                    ProdutosOrcamentoDAO.Instance.AplicarDesconto(session, produto, orcamento, produtos);
                    ProdutosOrcamentoDAO.Instance.FinalizarAplicacaoAcrescimoDesconto(session, orcamento, produto, produtos, true);
                }
            }

            UpdateTotaisOrcamento(session, orcamento);

            AplicaComissaoDescontoAcrescimo(session, orcamento, (uint?)idComissionado, percComissao, tipoAcrescimo, acrescimo, tipoDesconto, desconto,
                Geral.ManterDescontoAdministrador);

            AtualizarDataRecalcular(session, orcamento.IdOrcamento, DateTime.Now, UserInfo.GetUserInfo.CodUser);

            if (atualizarOrcamento)
            {
                Update(session, orcamento);
            }
        }

        /// <summary>
        /// Recupera os dados dos ambientes do orçamento.
        /// </summary>
        public string ObterDadosOrcamentoRecalcular(int tipoDesconto, decimal desconto, int tipoAcrescimo, decimal acrescimo, uint? idComissionado, float percComissao,
            Dictionary<uint, KeyValuePair<KeyValuePair<int, decimal>, KeyValuePair<int, decimal>>> dadosProdutosOrcamento)
        {
            var dadosOrcamentoRecalcular = string.Empty;

            foreach (var idProd in dadosProdutosOrcamento.Keys)
            {
                var dadosDesconto = dadosProdutosOrcamento[idProd].Key;
                var dadosAcrescimo = dadosProdutosOrcamento[idProd].Value;

                dadosOrcamentoRecalcular += string.Format("{0},{1},{2},{3},{4}|",
                    idProd, dadosDesconto.Key, dadosDesconto.Value.ToString().Replace(",", "."), dadosAcrescimo.Key, dadosAcrescimo.Value.ToString().Replace(",", "."));
            }

            var retorno = string.Format("Ok;{0};{1};{2};{3};{4};{5};{6}",
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