using System;
using System.Collections.Generic;
using GDA;
using Glass.Data.Model;
using Glass.Data.Helper;
using Glass.Configuracoes;

namespace Glass.Data.DAL
{
    public sealed class MedicaoDAO : BaseCadastroDAO<Medicao, MedicaoDAO>
    {
        //private MedicaoDAO() { }

        #region Variáveis locais

        private static readonly object _medicaoDefinitivaLock = new object();

        private static readonly object _medicaoLock = new object();

        #endregion

        #region Busca medições

        private string Sql(uint idMedicao, uint idOrcamento, uint idPedido, uint idMedidor, string nomeMedidor, uint idVendedor,
            uint situacao, string dataIni, string dataFim, string dataEfetuar, int idCliente, string nomeCli, string bairro, string endereco,
            string telefone, uint idLoja, string obs, bool selecionar)
        {
            string campos = selecionar ? "m.*, l.NomeFantasia as NomeLoja, f.Nome as NomeMedidor, vend.Nome as NomeVendedor, fCad.Nome AS NomeFuncCad, '$*[' as criterio" : "Count(*)";
            string criterio = String.Empty;

            string sql = @"
                Select " + campos + @" From medicao m 
                    Left Join funcionario f On (m.idFuncMed=f.idfunc) 
                    Left Join funcionario vend On (m.idFunc=vend.idfunc) 
                    Left Join funcionario fCad On (m.UsuCad=fCad.idfunc) 
                    Left Join loja l On (m.idLoja=l.idLoja) 
                Where 1 ";

            if (idMedicao > 0)
            {
                sql += " And idMedicao=" + idMedicao;
                criterio += "Num Medicao: " + idMedicao + "    ";
            }

            if (idOrcamento > 0)
            {
                sql += string.Format(@" AND (m.IdOrcamento={0})", idOrcamento);
                criterio += "Num. Orçamento: " + idOrcamento + "    ";
            }

            if (idPedido > 0)
            {
                sql += string.Format(" AND (m.IdPedido={0} OR m.IdMedicao IN " +
                    "(SELECT IdMedicao FROM orcamento WHERE IdOrcamento IN " +
                    "(SELECT IdOrcamento FROM pedido WHERE IdPedido={0})))", idPedido);
                criterio += string.Format("Num. Pedido: {0}    ", idPedido);
            }

            if (idVendedor > 0)
            {
                sql += "And vend.idFunc=" + idVendedor;
                criterio += "Vendedor: " + FuncionarioDAO.Instance.GetNome(idVendedor) + "    ";
            }

            if (situacao > 0)
            {
                sql += " And m.situacao=" + (uint)situacao;
                criterio += "Situação: " + GetSituacao(situacao) + "    ";
            }

            if (!String.IsNullOrEmpty(dataIni))
            {
                sql += " And DataMedicao>=?dataIni";
                criterio += "Data Início: " + dataIni + "    ";
            }

            if (!String.IsNullOrEmpty(dataFim))
            {
                sql += " And DataMedicao<=?dataFim";
                criterio += "Data Fim: " + dataFim + "    ";
            }

            if (!String.IsNullOrEmpty(dataEfetuar))
                sql += " And DataEfetuar>=?dataEfetuar And DataEfetuar<=?dataEfetuar1 ";

            if (idCliente > 0)
            {
                sql += " And m.IdCliente=" + idCliente;
                criterio += "Cliente: " + ClienteDAO.Instance.GetElementByPrimaryKey(idCliente).IdNome + "    ";
            }

            else if (!String.IsNullOrEmpty(nomeCli))
            {
                sql += " And m.NomeCliente Like ?nomeCli";
                criterio += "Cliente: " + nomeCli + "    ";
            }

            if (!String.IsNullOrEmpty(bairro))
            {
                sql += " And m.Bairro Like ?bairro";
                criterio += "Bairro: " + bairro + "    ";
            }

            if (!String.IsNullOrEmpty(endereco))
            {
                sql += " And m.Endereco Like ?endereco";
                criterio += "Endereço: " + endereco + "    ";
            }

            if (!String.IsNullOrEmpty(telefone))
            {
                sql += " And (telCliente like ?telefone Or celCliente like ?telefone)";
                criterio += "Telefone: " + telefone + "    ";
            }

            if (idLoja > 0)
            {
                sql += " And m.idLoja=" + idLoja;
                criterio += "Loja: " + LojaDAO.Instance.GetNome(idLoja) + "    ";
            }

            if (idMedidor > 0)
                sql += " And m.IdFuncMed=" + idMedidor;
            else if (!String.IsNullOrEmpty(nomeMedidor))
                sql += " And f.Nome Like ?nomeMedidor";

            if (!String.IsNullOrEmpty(nomeMedidor))
                criterio += "Medidor: " + nomeMedidor + "    ";

            if (!string.IsNullOrEmpty(obs))
            {
                sql += " And m.ObsMedicao Like ?obs";
            }

            return sql.Replace("$*[", criterio);
        }

        public IList<Medicao> GetListForRpt(uint idMedicao, uint idOrcamento, uint idPedido, uint idMedidor, string nomeMedidor,
            uint idVendedor, uint situacao, string dataIni, string dataFim, string dataEfetuar, int idCliente, string nomeCli, string bairro,
            string endereco, string telefone, uint idLoja, string obs, int ordenarPor)
        {
            var orderBy = ordenarPor == 0 ? MedicaoConfig.RelatorioListaMedicoes.OrdenarPeloIdMedicao ? " Order by idMedicao desc" : " Order By NumSeq" : ordenarPor == 1 ? "  Order By idMedicao" : "  Order By idMedicao desc";

            return objPersistence.LoadData(Sql(idMedicao, idOrcamento, idPedido, idMedidor, nomeMedidor, idVendedor, situacao, dataIni,
                dataFim, dataEfetuar, idCliente, nomeCli, bairro, endereco, telefone, idLoja, obs, true) + orderBy, GetParams(dataIni, dataFim,
                dataEfetuar, nomeCli, nomeMedidor, bairro, endereco, telefone, obs)).ToList();
        }

        public IList<Medicao> GetList(uint idMedicao, uint idOrcamento, uint idPedido, uint idMedidor, string nomeMedidor, uint idVendedor,
            uint situacao, string dataIni, string dataFim, string dataEfetuar, int idCliente, string nomeCli, string bairro, string endereco,
            string telefone, uint idLoja, string obs, int ordenarPor, string sortExpression, int startRow, int pageSize)
        {
            var orderBy = !String.IsNullOrEmpty(sortExpression) ? sortExpression : ordenarPor == 0 ?
                MedicaoConfig.RelatorioListaMedicoes.OrdenarPeloIdMedicao ? " idMedicao desc" : "DataMedicao desc, idMedicao desc" : ordenarPor == 1 ? " idMedicao" : " idMedicao desc";

            return LoadDataWithSortExpression(Sql(idMedicao, idOrcamento, idPedido, idMedidor, nomeMedidor, idVendedor, situacao, dataIni,
                dataFim, dataEfetuar, idCliente, nomeCli, bairro, endereco, telefone, idLoja, obs, true), orderBy, startRow, pageSize,
                GetParams(dataIni, dataFim, dataEfetuar, nomeCli, nomeMedidor, bairro, endereco, telefone, obs));
        }

        public int GetCount(uint idMedicao, uint idOrcamento, uint idPedido, uint idMedidor, string nomeMedidor, uint idVendedor,
            uint situacao, string dataIni, string dataFim, string dataEfetuar, int idCliente, string nomeCli, string bairro, string endereco,
            string telefone, uint idLoja, string obs, int ordenarPor)
        {
            return objPersistence.ExecuteSqlQueryCount(Sql(idMedicao, idOrcamento, idPedido, idMedidor, nomeMedidor, idVendedor, situacao,
                dataIni, dataFim, dataEfetuar, idCliente, nomeCli, bairro, endereco, telefone, idLoja, obs, false),
                GetParams(dataIni, dataFim, dataEfetuar, nomeCli, nomeMedidor, bairro, endereco, telefone, obs));
        }

        private GDAParameter[] GetParams(string dataIni, string dataFim, string dataEfetuar, string nomeCli, string nomeMedidor, string bairro, 
            string endereco, string telefone, string obs)
        {
            List<GDAParameter> lstParam = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(dataIni))
                lstParam.Add(new GDAParameter("?dataIni", DateTime.Parse(dataIni)));

            if (!String.IsNullOrEmpty(dataFim))
                lstParam.Add(new GDAParameter("?dataFim", DateTime.Parse(dataFim)));

            if (!String.IsNullOrEmpty(dataEfetuar))
            {
                lstParam.Add(new GDAParameter("?dataEfetuar", DateTime.Parse(dataEfetuar + " 00:00")));
                lstParam.Add(new GDAParameter("?dataEfetuar1", DateTime.Parse(dataEfetuar + " 23:59")));
            }

            if (!String.IsNullOrEmpty(nomeCli))
                lstParam.Add(new GDAParameter("?nomeCli", "%" + nomeCli + "%"));

            if (!String.IsNullOrEmpty(nomeMedidor))
                lstParam.Add(new GDAParameter("?nomeMedidor", "%" + nomeMedidor + "%"));

            if (!String.IsNullOrEmpty(bairro))
                lstParam.Add(new GDAParameter("?bairro", "%" + bairro + "%"));

            if (!String.IsNullOrEmpty(endereco))
                lstParam.Add(new GDAParameter("?endereco", "%" + endereco + "%"));

            if (!String.IsNullOrEmpty(telefone))
                lstParam.Add(new GDAParameter("?telefone", "%" + telefone + "%"));

            if (!String.IsNullOrEmpty(obs))
                lstParam.Add(new GDAParameter("?obs", "%" + obs + "%"));

            return lstParam.Count > 0 ? lstParam.ToArray() : null;
        }

        #endregion

        #region Busca medições para tela Efetuar Medição

        private string SqlSelecionarMedicao(string dataIni, string dataFim, string nomeCli, uint idLoja, bool selecionar)
        {
            var sql = string.Format("SELECT {0} FROM medicao m WHERE Situacao={1} ", selecionar ? "m.*" : "COUNT(*)", (int)Medicao.SituacaoMedicao.Aberta);

            if (!string.IsNullOrEmpty(dataIni))
                sql += " AND DataMedicao>=?dataIni";

            if (!string.IsNullOrEmpty(dataFim))
                sql += " AND DataMedicao<=?dataFim";
            
            if (!string.IsNullOrEmpty(nomeCli))
                sql += " And m.NomeCliente LIKE ?nomeCli";

            if (idLoja > 0)
                sql += " AND m.IdLoja=" + idLoja;

            return sql;
        }

        public IList<Medicao> PesquisarMedicoesSelecionarMedicao(string dataIni, string dataFim, string nomeCli, uint idLoja, string sortExpression, int startRow, int pageSize)
        {
            var orderBy = !string.IsNullOrEmpty(sortExpression) ? sortExpression : MedicaoConfig.RelatorioListaMedicoes.OrdenarPeloIdMedicao ? "idMedicao desc" : "DataMedicao desc, idMedicao desc";

            return LoadDataWithSortExpression(SqlSelecionarMedicao(dataIni, dataFim, nomeCli, idLoja, true), orderBy, startRow, pageSize,
                ObterParametrosPesquisarMedicoesSelecionarMedicao(dataIni, dataFim, nomeCli));
        }

        public int PesquisarMedicoesSelecionarMedicaoCount(string dataIni, string dataFim, string nomeCli, uint idLoja)
        {
            return objPersistence.ExecuteSqlQueryCount(SqlSelecionarMedicao(dataIni, dataFim, nomeCli, idLoja, false), ObterParametrosPesquisarMedicoesSelecionarMedicao(dataIni, dataFim, nomeCli));
        }

        private GDAParameter[] ObterParametrosPesquisarMedicoesSelecionarMedicao(string dataIni, string dataFim, string nomeCli)
        {
            var parametros = new List<GDAParameter>();

            if (!string.IsNullOrEmpty(dataIni))
                parametros.Add(new GDAParameter("?dataIni", DateTime.Parse(dataIni)));

            if (!string.IsNullOrEmpty(dataFim))
                parametros.Add(new GDAParameter("?dataFim", DateTime.Parse(dataFim)));

            if (!string.IsNullOrEmpty(nomeCli))
                parametros.Add(new GDAParameter("?nomeCli", "%" + nomeCli + "%"));

            return parametros.Count > 0 ? parametros.ToArray() : null;
        }

        #endregion

        #region Busca para relatório individual

        /// <summary>
        /// Busca medição para impressão de relatório padrão único
        /// </summary>
        /// <param name="idMedicao"></param>
        /// <returns></returns>
        public Medicao GetForRptUnico(uint idMedicao)
        {
            string sql = "Select m.*, l.NomeFantasia as NomeLoja, if(l.idCidade is null, l.cidade, cid.NomeCidade) as cidadeLoja, " + 
                "l.endereco as enderecoLoja, l.numero as numeroLoja, l.Bairro as bairroLoja, l.Telefone as telLoja, " + 
                "f.Nome as NomeMedidor, vend.Nome as NomeVendedor From medicao m " +
                "Left Join funcionario f On (m.idFuncMed=f.idfunc) " +
                "Left Join funcionario vend On (m.idFunc=vend.idfunc) " +
                "Left Join loja l On (m.idLoja=l.idLoja) " + 
                "Left Join cidade cid On (cid.idCidade=l.idCidade) Where idMedicao=" + idMedicao;

            return objPersistence.LoadOneData(sql);
        }

        #endregion

        #region Finaliza Medição

        /// <summary>
        /// Finaliza a medição passada e gera um orçamento para a mesma, retornando o id deste
        /// </summary>
        public uint FinalizarMedicao(uint idMedicao)
        {
            using (var transaction = new GDA.GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    var login = UserInfo.GetUserInfo;

                    if (!Config.PossuiPermissao(Config.FuncaoMenuMedicao.EfetuarMedicao))
                        throw new Exception("Apenas o Aux. Escritório Medição pode finalizar medições.");

                    var medicao = GetMedicao(transaction, idMedicao);

                    //variavel utilizada apenas para o log, para não ter que ir no banco novamente buscar a medição.
                    var medicaoLog = medicao;

                    if (medicao.Situacao == (int)Medicao.SituacaoMedicao.Finalizada)
                        throw new Exception("Medição já finalizada.");

                    medicao.Situacao = (int)Medicao.SituacaoMedicao.Finalizada;
                    medicao.IdFuncConf = login.CodUser;
                    medicao.DataConf = DateTime.Now;
                    
                    uint idOrca = 0;

                    /* Chamado 50817. */
                    // Caso a medição esteja associada a um pedido ou a um orçamento, não gera o orçamento ou
                    // retorna o ID do orçamento com medição definitiva.
                    if (medicao.IdPedido.GetValueOrDefault() == 0 && medicao.IdOrcamento.GetValueOrDefault() == 0)
                    {
                        Orcamento orca = new Orcamento();
                            orca.Bairro = medicao.Bairro;
                            orca.Cidade = medicao.Cidade;
                            orca.Endereco = medicao.Endereco;
                            orca.IdFuncionario = medicao.IdFunc == 0 ? null : medicao.IdFunc;
                            orca.NomeCliente = medicao.NomeCliente;
                            orca.TelCliente = medicao.TelCliente;
                            orca.CelCliente = medicao.CelCliente;
                            orca.Email = medicao.EmailCliente;
                            orca.TipoEntrega = (int?)Orcamento.TipoEntregaOrcamento.Temperado;
                            orca.Validade = OrcamentoConfig.DadosOrcamento.ValidadeOrcamento;
                            orca.PrazoEntrega = OrcamentoConfig.DadosOrcamento.PrazoEntregaOrcamento;

                            orca.FormaPagto = !string.IsNullOrEmpty(medicao.FormaPagto) ? medicao.FormaPagto :
                                OrcamentoConfig.DadosOrcamento.FormaPagtoOrcamento;

                            orca.Situacao = (int)Orcamento.SituacaoOrcamento.EmAberto;

                            idOrca = OrcamentoDAO.Instance.Insert(transaction, orca);

                        medicao.IdOrcamento = idOrca;
                    }

                    LogAlteracaoDAO.Instance.LogMedicao(GetMedicao(idMedicao), medicao);
                    Update(transaction, medicao);

                    transaction.Commit();
                    transaction.Close();

                    return idOrca;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    transaction.Close();

                    throw ex;
                }
            }
        }

        #endregion

        #region Cancela Medição

        /// <summary>
        /// Cancela uma medição.
        /// </summary>
        /// <param name="idMedicao"></param>
        public void CancelarMedicao(uint idMedicao, string obs)
        {
            try
            {
                LogCancelamentoDAO.Instance.LogMedicao(null, GetMedicao(idMedicao), obs, true);

                objPersistence.ExecuteCommand("update medicao set obsMedicao=?obs, situacao=" + (int)Medicao.SituacaoMedicao.Cancelada +
                    " where idMedicao=" + idMedicao, new GDAParameter("?obs", obs));

                //Sala o log de cancelamento da medição
                
            }
            catch (Exception ex)
            {
                throw new Exception(Glass.MensagemAlerta.FormatErrorMsg("Falha ao cancelar medição.", ex));
            }
        }

        #endregion

        #region Efetua medições

        /// <summary>
        /// Atribui medicoes passadas ao medidor passado
        /// </summary>
        /// <param name="idMedidor"></param>
        /// <param name="idMedicoes"></param>
        /// <param name="dataEfetuar"></param>
        public void SetMedicoesForMedidor(uint idMedidor, string idMedicoes, DateTime dataEfetuar)
        {
            if (!Config.PossuiPermissao(Config.FuncaoMenuMedicao.EfetuarMedicao))
                throw new Exception("Apenas Aux. Escritório Medição pode efetuar medições.");

            string[] idsMed = idMedicoes.Split(',');
            string sqlExec = String.Empty;
            string sql = "Update medicao Set dataEfetuar=?dataEfetuar, numSeq=$numSeq, situacao=" + (int)Medicao.SituacaoMedicao.EmAndamento + 
                ", idFuncMed=" + idMedidor + " Where idMedicao=$idMed";

            GDAParameter[] param = new GDAParameter[] { new GDAParameter("?dataEfetuar", dataEfetuar) };

            // Para cada medição
            for (int i = 0; i < idsMed.Length; i++)
            {
                sqlExec = sql.Replace("$numSeq", (i + 1).ToString()).Replace("$idMed", idsMed[i].ToString());
                objPersistence.ExecuteCommand(sqlExec, param);
            }
        }

        #endregion

        #region Situação da medição

        /// <summary>
        /// Retorna a descrição da situação da medição baseado no código passado
        /// </summary>
        /// <param name="situacao"></param>
        /// <returns></returns>
        private string GetSituacao(uint situacao)
        {
            switch (situacao)
            {
                case 1:
                    return "Aberta";
                case 2:
                    return "Em andamento";
                case 3:
                    return "Finalizada";
                case 4:
                    return "Remarcada";
                case 5:
                    return "Cancelada";
                default:
                    return "";
            }
        }

        #endregion

        #region Busca medição se existir

        public Medicao GetMedicao(uint idMedicao)
        {
            return GetMedicao(null, idMedicao);
        }

        public Medicao GetMedicao(GDASession session, uint idMedicao)
        {
            string sql = @"Select m.*
                From medicao m where idMedicao=" + idMedicao;

            List<Medicao> lst = objPersistence.LoadData(session, sql);

            return lst.Count == 0 ? new Medicao() : lst[0];
        }

        #endregion

        #region Busca Medidor

        public uint? GetMedidor(uint idMedicao)
        {
            return GetMedidor(null, idMedicao);
        }

        public uint? GetMedidor(GDASession session, uint idMedicao)
        {
            string sql = @"SELECT idfuncmed FROM medicao WHERE idmedicao=" + idMedicao;

            return ExecuteScalar<uint?>(session, sql);
        }

        #endregion

        #region Limite de medições

        public int GetNumMedicoes(DateTime dia)
        {
            return GetNumMedicoes(dia, null);
        }

        public int GetNumMedicoes(DateTime dia, uint? idMedicao)
        {
            string filtro = idMedicao != null ? "idMedicao<>" + idMedicao.Value + " and " : "";
            string sql = "select count(*) from medicao where " + filtro + "dataMedicao>=?dataIni and dataMedicao<=?dataFim " +
                "and situacao in (" + (int)Medicao.SituacaoMedicao.Aberta + "," + (int)Medicao.SituacaoMedicao.EmAndamento + "," +
                (int)Medicao.SituacaoMedicao.Remarcada + ")";

            return Glass.Conversoes.StrParaInt(objPersistence.ExecuteScalar(sql, new GDAParameter("?dataIni", dia.Date), new GDAParameter("?dataFim", 
                dia.Date.AddDays(1).AddMilliseconds(-1))).ToString());
        }

        public DateTime? GetMedicaoDay(Medicao medicao)
        {
            return GetMedicaoDay(medicao.DataMedicao.Value, medicao.IdMedicao); 
        }

        public DateTime? GetMedicaoDay(DateTime dataInicio, uint? idMedicao)
        {
            DateTime retorno = dataInicio;
            if (retorno < DateTime.Now)
                retorno = DateTime.Now;

            int i = 0;
            while (true)
            {
                if (IsMedicaoDay(retorno, idMedicao))
                    break;

                retorno = retorno.AddDays(1);
                if (retorno.DayOfWeek == DayOfWeek.Sunday || retorno.Feriado())
                    retorno = retorno.AddDays(1);

                i++;
            }

            return retorno;
        }

        public bool IsMedicaoDay(DateTime data, uint? idMedicao)
        {
            return GetNumMedicoes(data, idMedicao) + 1 <= OrcamentoConfig.LimiteDiarioMedicoes;
        }

        #endregion

        #region Medição definitiva gerada de orçamento

        /// <summary>
        /// Gera uma medição definitiva para um orçamento.
        /// </summary>
        /// <param name="idOrca"></param>
        public uint GerarMedicaoDefinitivaOrca(uint idOrca)
        {
            lock (_medicaoDefinitivaLock)
            {
                using (var transaction = new GDA.GDATransaction())
                {
                    try
                    {
                        transaction.BeginTransaction();

                        Orcamento orca = OrcamentoDAO.Instance.GetElementByPrimaryKey(transaction, idOrca);

                        if (orca.IdMedicaoDefinitiva.GetValueOrDefault() > 0 &&
                            objPersistence.ExecuteSqlQueryCount("SELECT IdMedicao FROM medicao WHERE Situacao=" +
                                (int)Medicao.SituacaoMedicao.Aberta + " AND IdMedicao=" + orca.IdMedicaoDefinitiva.Value) > 0)
                            throw new Exception("Este orçamento já possui uma medição definitiva, cancele-a para gerar outra.");

                        Medicao medicao = new Medicao();
                        medicao.Bairro = orca.BairroObra != null ? orca.BairroObra : orca.Bairro;
                        medicao.Cidade = orca.CidadeObra != null ? orca.CidadeObra : orca.Cidade;
                        medicao.Endereco = orca.EnderecoObra != null ? orca.EnderecoObra : orca.Endereco;
                        medicao.IdFunc = orca.IdFuncionario;
                        medicao.NomeCliente = orca.NomeCliente;
                        medicao.TelCliente = orca.TelCliente;
                        medicao.EmailCliente = orca.Email;
                        medicao.Situacao = (int)Medicao.SituacaoMedicao.Aberta;
                        medicao.Turno = 1;
                        medicao.IdLoja = orca.IdLoja.GetValueOrDefault(LojaDAO.Instance.ObtemValorCampo<uint>(transaction, "idLoja", null));
                        medicao.MedicaoDefinitiva = true;
                        /* Chamado 51781. */
                        medicao.IdOrcamento = idOrca;

                        uint idMedicao = Insert(transaction, medicao);
                        
                        //objPersistence.ExecuteCommand(transaction, "update orcamento set idMedicaoDefinitiva=" + idMedicao +
                        //    " where idOrcamento=" + idOrca);
                        
                        transaction.Commit();
                        transaction.Close();

                        return idMedicao;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        transaction.Close();

                        throw ex;
                    }
                }
            }
        }

        /// <summary>
        /// Verifica se é uma medição definitiva gerada de orçamento.
        /// </summary>
        /// <param name="idMedicao"></param>
        /// <returns></returns>
        public bool IsMedicaoDefinitivaOrca(uint idMedicao)
        {
            return objPersistence.ExecuteSqlQueryCount("select count(*) from orcamento where idMedicaoDefinitiva=" + idMedicao) > 0;
        }

        #endregion

        #region Obtém campos da medição

        /// <summary>
        /// Obtém o ID do orçamento associado à medição.
        /// </summary>
        public int? ObterIdOrcamento(GDASession session, int idMedicao)
        {
            return ExecuteScalar<int?>(session, string.Format("SELECT IdOrcamento FROM medicao WHERE IdMedicao={0}", idMedicao));
        }

        /// <summary>
        /// Obtém os ID's das medições associadas ao orçamento.
        /// </summary>
        public string ObterIdsMedicaoPeloIdOrcamento(GDASession session, int idOrcamento)
        {
            var idsMedicao = ExecuteMultipleScalar<string>(session, string.Format("SELECT IdMedicao FROM medicao WHERE IdOrcamento={0}", idOrcamento));
            return idsMedicao != null && idsMedicao.Count > 0 ? string.Join(",", idsMedicao) : string.Empty;
        }

        #endregion

        #region Métodos sobrescritos

        public override uint Insert(Medicao objInsert)
        {
            return Insert((GDASession)null, objInsert);
        }

        public override uint Insert(GDASession session, Medicao objInsert)
        {
            lock (_medicaoLock)
            {
                if (objInsert.IdFunc == 0)
                    objInsert.IdFunc = null;

                objInsert.Usucad = UserInfo.GetUserInfo.CodUser;
                objInsert.DataCad = DateTime.Now;

                return base.Insert(session, objInsert);
            }
        }

        public int UpdateMedicao(Medicao objUpdate)
        {
            if (objUpdate == null)
                throw new Exception("Não foi possivel atualizar a medição");

            if (objUpdate.IdOrcamento.GetValueOrDefault() >0 && objUpdate.Situacao != 5)
            {
                //Obtem caso exista a medição definitiva do orçamento em questão
                var idMed = ObterMedicaoDefinitivaPeloIdOrcamento((int)objUpdate.IdOrcamento);

                //Caso o orçamento já possua medição definitiva não permite que salve outra medição definitiva
                if (idMed > 0 && idMed != objUpdate.IdMedicao && objUpdate.MedicaoDefinitiva)
                    throw new Exception(string.Format("O orçamento {0} vinculado à essa medição já possui uma medição definitiva. Med. Def.:{1}", objUpdate.IdOrcamento, idMed));
            }

            LogAlteracaoDAO.Instance.LogMedicao(GetMedicao(objUpdate.IdMedicao), objUpdate);

            return Update(objUpdate);
        }

        public override int Update(Medicao objUpdate)
        {
            lock (_medicaoLock)
            {
                if (objUpdate.IdFunc == 0)
                    objUpdate.IdFunc = null;

                return base.Update(objUpdate);
            }
        }

        /// <summary>
        /// Altera a situação da medição para cancelada ao invés de excluir
        /// </summary>
        /// <param name="objDelete"></param>
        public override int Delete(Medicao objDelete)
        {
            lock (_medicaoLock)
            {
                objPersistence.ExecuteCommand("Update medicao Set Situacao=5 Where idMedicao=" + objDelete.IdMedicao);

                return 1;
            }
        }

        #endregion

        /// <summary>
        /// Obtém o ID do orçamento pela medição definitiva dele.
        /// </summary>
        public int? ObterIdOrcamentoPelaMedicaoDefinitiva(GDASession session, int idMedicao)
        {
            return ObtemValorCampo<int?>(session, "IdOrcamento", "MedicaoDefinitiva AND IdMedicao=" + idMedicao);
        }

        /// <summary>
        /// Obtém o ID da medição caso o orcamento possua alguma medição definitiva
        /// </summary>
        public uint? ObterMedicaoDefinitivaPeloIdOrcamento(int idOrcamento)
        {
            return ObtemValorCampo<uint?>("Idmedicao", "MedicaoDefinitiva=1 AND Situacao<>5 AND IdOrcamento =" + idOrcamento);
        }
    }
}