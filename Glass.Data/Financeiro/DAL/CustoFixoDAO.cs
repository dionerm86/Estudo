using System;
using System.Collections.Generic;
using GDA;
using Glass.Data.Model;
using Glass.Data.Helper;

namespace Glass.Data.DAL
{
    public sealed class CustoFixoDAO : BaseCadastroDAO<CustoFixo, CustoFixoDAO>
    {
        //private CustoFixoDAO() { }

        #region Busca para listagem e relatório

        private string Sql(uint idCustoFixo, uint idLoja, uint idFornec, string nomeFornec, int diaVencIni, int diaVencFim, 
            int situacao, string descricao, bool selecionar, out string filtroAdicional)
        {
            filtroAdicional = "";
            string campos = selecionar ? "c.*, Coalesce(f.NomeFantasia, f.RazaoSocial, '') as NomeFornec, " + 
                "l.NomeFantasia as NomeLoja, Concat(g.Descricao, ' - ', p.Descricao) as DescrPlanoConta, '$$$' as Criterio" : "Count(*)";

            string criterio = String.Empty;

            string sql = "Select " + campos + @" From custo_fixo c 
                Left Join fornecedor f On (c.IdFornec=f.IdFornec) 
                Left Join loja l On (c.IdLoja=l.IdLoja)
                Left Join plano_contas p On (c.IdConta=p.IdConta) 
                Left Join grupo_conta g On (p.IdGrupo=g.IdGrupo) Where 1 ?filtroAdicional?";

            if (idCustoFixo > 0)
            {
                filtroAdicional += " And c.IdCustoFixo=" + idCustoFixo;
                criterio += "Cód. Custo Fixo: " + idCustoFixo + "    ";
            }

            if (idLoja > 0)
            {
                filtroAdicional += " And c.IdLoja=" + idLoja;
                criterio += "Loja: " + LojaDAO.Instance.GetNome(idLoja) + "    ";
            }

            if (idFornec > 0)
            {
                filtroAdicional += " And c.idFornec=" + idFornec;
                criterio += "Fornecedor: " + nomeFornec + "    ";
            }
            else if (!String.IsNullOrEmpty(nomeFornec))
            {
                string ids = FornecedorDAO.Instance.ObtemIds(nomeFornec);
                filtroAdicional += " And c.idFornec In (" + ids + ")";
                criterio += "Fornecedor: " + nomeFornec + "    ";
            }

            if (diaVencIni > 0)
            {
                filtroAdicional += " And c.diaVenc>=" + diaVencIni;
                criterio += "Dia Venc.: " + diaVencIni + " ";
            }

            if (diaVencFim > 0)
            {
                filtroAdicional += " And c.diaVenc<=" + diaVencFim;
                criterio += diaVencIni == 0 ? "Dia Venc.: até " + diaVencFim + "    " : "a " + diaVencFim;
            }

            if (!String.IsNullOrEmpty(descricao))
            {
                filtroAdicional += " And c.Descricao Like ?descr";
                criterio += "Descrição: " + descricao + "    ";
            }

            if (situacao > 0)
            {
                filtroAdicional += " And c.situacao=" + situacao;
                criterio += "Situação: " + (situacao == 1 ? "Ativos" : situacao == 2 ? "Inativos" : " N/D") + "    ";
            }

            return sql.Replace("$$$", criterio);
        }

        private string SqlPE(uint idCustoFixo, uint idLoja, uint idFornec, string nomeFornec, int diaVencIni, int diaVencFim, 
            int situacao, string descricao, bool selecionar, out string filtroAdicional)
        {
            filtroAdicional = "";
            string campos = selecionar ? "c.*, Coalesce(f.NomeFantasia, f.RazaoSocial, '') as NomeFornec, " +
                "l.NomeFantasia as NomeLoja, Concat(g.Descricao, ' - ', p.Descricao) as DescrPlanoConta, '$$$' as Criterio" : "Count(*)";

            string criterio = String.Empty;

            string sql = "Select " + campos + @" From custo_fixo c 
                Left Join fornecedor f On (c.IdFornec=f.IdFornec) 
                Left Join loja l On (c.IdLoja=l.IdLoja)
                Left Join plano_contas p On (c.IdConta=p.IdConta) 
                Left Join grupo_conta g On (p.IdGrupo=g.IdGrupo) Where 1 ?filtroAdicional?";

            if (idCustoFixo > 0)
            {
                filtroAdicional += " And c.IdCustoFixo=" + idCustoFixo;
                criterio += "Cód. Custo Fixo: " + idCustoFixo + "    ";
            }

            if (idLoja > 0)
            {
                filtroAdicional += " And c.IdLoja=" + idLoja;
                criterio += "Loja: " + LojaDAO.Instance.GetNome(idLoja) + "    ";
            }

            if (idFornec > 0)
            {
                filtroAdicional += " And c.idFornec=" + idFornec;
                criterio += "Fornecedor: " + nomeFornec + "    ";
            }
            else if (!String.IsNullOrEmpty(nomeFornec))
            {
                string ids = FornecedorDAO.Instance.ObtemIds(nomeFornec);
                filtroAdicional += " And c.idFornec In (" + ids + ")";
                criterio += "Fornecedor: " + nomeFornec + "    ";
            }

            if (diaVencIni > 0)
            {
                filtroAdicional += " And c.diaVenc>=" + diaVencIni;
                criterio += "Dia Venc.: " + diaVencIni + " ";
            }

            if (diaVencFim > 0)
            {
                filtroAdicional += " And c.diaVenc<=" + diaVencFim;
                criterio += diaVencIni == 0 ? "Dia Venc.: até " + diaVencFim + "    " : "a " + diaVencFim;
            }

            if (!String.IsNullOrEmpty(descricao))
            {
                filtroAdicional += " And c.Descricao Like ?descr";
                criterio += "Descrição: " + descricao + "    ";
            }

            if (situacao > 0)
            {
                filtroAdicional += " And c.situacao=" + situacao;
                criterio += "Situação: " + (situacao == 1 ? "Ativos" : situacao == 2 ? "Inativos" : " N/D") + "    ";
            }

            sql += " And c.PontoEquilibrio = 0";

            return sql.Replace("$$$", criterio);
        }

        private string SqlGerados(int idCustoFixo, int idLoja, int idFornec, string nomeFornec, string dataVencIni, string dataVencFim,
            int idConta, string descricao, bool centroCustoDivergente, bool selecionar, out string filtroAdicional)
        {
            filtroAdicional = "";

            var campos = selecionar ? @"
                cf.*, Coalesce(f.NomeFantasia, f.RazaoSocial, '') as NomeFornec,
                l.NomeFantasia as NomeLoja, Concat(g.Descricao, ' - ', p.Descricao) as DescrPlanoConta,
                cp.DataVenc, cp.Obs, cp.valorVenc as ValorContaGerada, cp.IdContaPg" : "COUNT(*)";

            var sql = @"
                SELECT" + campos + @"
                FROM contas_pagar cp
                    INNER JOIN custo_fixo cf ON (cp.IdCustoFixo = cf.IdCustoFixo)
                    LEFT JOIN fornecedor f ON (cf.IdFornec = f.IdFornec) 
                    LEFT JOIN loja l ON (cf.IdLoja = l.IdLoja)
                    LEFT JOIN plano_contas p ON (cf.IdConta = p.IdConta) 
                    LEFT JOIN grupo_conta g ON (p.IdGrupo = g.IdGrupo) 
                WHERE 1";


            if (idCustoFixo > 0)
                filtroAdicional += " And cf.IdCustoFixo=" + idCustoFixo;

            if (idLoja > 0)
                filtroAdicional += " And cf.IdLoja=" + idLoja;

            if (idFornec > 0)
            {
                filtroAdicional += " And cf.idFornec=" + idFornec;
            }
            else if (!String.IsNullOrEmpty(nomeFornec))
            {
                string ids = FornecedorDAO.Instance.ObtemIds(nomeFornec);
                filtroAdicional += " And cf.idFornec In (" + ids + ")";
            }

            if (!string.IsNullOrEmpty(dataVencIni))
                filtroAdicional += " And cp.dataVenc >= ?dataIni";

            if (!string.IsNullOrEmpty(dataVencFim))
                filtroAdicional += " And cp.dataVenc <= ?dataFim";

            if (!String.IsNullOrEmpty(descricao))
                filtroAdicional += " And cf.Descricao Like ?descr";

            if (idConta > 0)
                filtroAdicional += " And cf.idConta=" + idConta;

            if (centroCustoDivergente)
                filtroAdicional += " AND cp.ValorVenc <> (SELECT COALESCE(sum(valor), 0) FROM centro_custo_associado WHERE IdContaPg = cp.IdContaPg)";

            return sql;
        }


        public IList<CustoFixo> GetForRpt(uint idCustoFixo, uint idLoja, uint idFornec, string nomeFornec, int diaVencIni, int diaVencFim, string descricao, int situacao, ref decimal[] lstPrevisao)
        {
            // Sql para buscar gastos com Salários
            string sqlSalarios = "Select coalesce(Sum(coalesce(salario, 0) + coalesce(gratificacao, 0) + coalesce(auxalimentacao, 0)),0) " +
                "From funcionario Where situacao=" + (int)Situacao.Ativo;

            // Sql para buscar gasto com férias
            string sqlFerias = "Select coalesce(Sum((coalesce(salario, 0) / 3) / 12),0) From funcionario where situacao=" + (int)Situacao.Ativo;

            // Sql para buscar gasto com décimo terceiro
            string sqlDecTerc = "Select coalesce(Sum(coalesce(salario, 0) / 12),0) From funcionario where situacao=" + (int)Situacao.Ativo;

            // Sql para buscar gasto com IPVA
            string sqlIpva = "Select coalesce(Sum(Coalesce(ValorIpva, 0) / 12),0) From veiculo";

            lstPrevisao[0] = ExecuteScalar<decimal>(sqlSalarios);
            lstPrevisao[1] = ExecuteScalar<decimal>(sqlFerias);
            lstPrevisao[2] = ExecuteScalar<decimal>(sqlDecTerc);
            lstPrevisao[3] = ExecuteScalar<decimal>(sqlIpva);

            string filtroAdicional;
            string sql = Sql(idCustoFixo, idLoja, idFornec, nomeFornec, diaVencIni, diaVencFim, situacao, descricao, true, out filtroAdicional).
                Replace("?filtroAdicional?", filtroAdicional);

            return objPersistence.LoadData(sql, GetParams(nomeFornec, descricao, null, null)).ToList();
        }

        public CustoFixo GetElement(uint idCustoFixo)
        {
            return GetElement(null, idCustoFixo);
        }

        public CustoFixo GetElement(GDASession sessao, uint idCustoFixo)
        {
            string filtroAdicional;
            string sql = Sql(idCustoFixo, 0, 0, null, 0, 0, 0, null, true, out filtroAdicional).
                Replace("?filtroAdicional?", filtroAdicional);

            return objPersistence.LoadOneData(sessao, sql);
        }

        public IList<CustoFixo> GetList(uint idCustoFixo, uint idLoja, uint idFornec, string nomeFornec, int diaVencIni, int diaVencFim, 
            string descricao, int situacao, string sortExpression, int startRow, int pageSize)
        {
            string filtroAdicional;
            string sql = Sql(idCustoFixo, idLoja, idFornec, nomeFornec, diaVencIni, diaVencFim, situacao, descricao, true, out filtroAdicional).
                Replace("?filtroAdicional?", "");

            return LoadDataWithSortExpression(sql, sortExpression, startRow, pageSize, false, filtroAdicional, GetParams(nomeFornec, descricao, null, null));
        }

        public int GetCount(uint idCustoFixo, uint idLoja, uint idFornec, string nomeFornec, int diaVencIni, int diaVencFim, 
            string descricao, int situacao)
        {
            string filtroAdicional;
            string sql = Sql(idCustoFixo, idLoja, idFornec, nomeFornec, diaVencIni, diaVencFim, situacao, descricao, true, out filtroAdicional).
                Replace("?filtroAdicional?", "");

            return GetCountWithInfoPaging(sql, false, filtroAdicional, GetParams(nomeFornec, descricao, null, null));
        }

        public IList<CustoFixo> GetListSel(uint idCustoFixo, uint idLoja, uint idFornec, string nomeFornec, int diaVencIni, int diaVencFim,
            string descricao, int situacao, string sortExpression, int startRow, int pageSize)
        {
            string filtroAdicional;
            string sql = Sql(idCustoFixo, idLoja, idFornec, nomeFornec, diaVencIni, diaVencFim, situacao, descricao, true, out filtroAdicional).
                Replace("?filtroAdicional?", "");

            return LoadDataWithSortExpression(sql, sortExpression, startRow, pageSize, false, filtroAdicional, GetParams(nomeFornec, descricao, null, null));
        }

        public int GetCountSel(uint idCustoFixo, uint idLoja, uint idFornec, string nomeFornec, int diaVencIni, int diaVencFim,
            string descricao, int situacao)
        {
            string filtroAdicional;
            string sql = Sql(idCustoFixo, idLoja, idFornec, nomeFornec, diaVencIni, diaVencFim, situacao, descricao, true, out filtroAdicional).
                Replace("?filtroAdicional?", "");

            return GetCountWithInfoPaging(sql, false, filtroAdicional, GetParams(nomeFornec, descricao, null, null));
        }

        public int GetListGeradosCount(int idCustoFixo, int idLoja, int idFornec, string nomeFornec, string dataVencIni, string dataVencFim,
            string descricao, int idConta, bool centroCustoDivergente)
        {
            string filtroAdicional;
            string sql = SqlGerados(idCustoFixo, idLoja, idFornec, nomeFornec, dataVencIni, dataVencFim, idConta, descricao, centroCustoDivergente, true, out filtroAdicional).
                Replace("?filtroAdicional?", "");

            return GetCountWithInfoPaging(sql, false, filtroAdicional, GetParams(nomeFornec, descricao, dataVencIni, dataVencFim));
        }

        public IList<CustoFixo> GetListGerados(int idCustoFixo, int idLoja, int idFornec, string nomeFornec, string dataVencIni, string dataVencFim,
            string descricao, int idConta, bool centroCustoDivergente, string sortExpression, int startRow, int pageSize)
        {
            string filtroAdicional;
            string sql = SqlGerados(idCustoFixo, idLoja, idFornec, nomeFornec, dataVencIni, dataVencFim, idConta, descricao, centroCustoDivergente, true, out filtroAdicional).
                Replace("?filtroAdicional?", "");

            if (string.IsNullOrEmpty(sortExpression))
                sortExpression = "DataVenc";

            return LoadDataWithSortExpression(sql, sortExpression, startRow, pageSize, false, filtroAdicional, GetParams(nomeFornec, descricao, dataVencIni, dataVencFim));
        }

        private GDAParameter[] GetParams(string nomeFornec, string descricao, string dataIni, string dataFim)
        {
            List<GDAParameter> lstParam = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(nomeFornec))
                lstParam.Add(new GDAParameter("?nomeFornec", "%" + nomeFornec + "%"));

            if (!String.IsNullOrEmpty(descricao))
                lstParam.Add(new GDAParameter("?descr", "%" + descricao + "%"));

            if (!String.IsNullOrEmpty(dataIni))
                lstParam.Add(new GDAParameter("?dataIni", DateTime.Parse(dataIni + " 00:00")));

            if (!String.IsNullOrEmpty(dataFim))
                lstParam.Add(new GDAParameter("?dataFim", DateTime.Parse(dataFim + " 23:59")));

            return lstParam.Count > 0 ? lstParam.ToArray() : null;
        }

        #endregion

        #region Busca por lista de PKS

        /// <summary>
        /// Busca custos fixos com os ids passados
        /// </summary>
        /// <param name="listPKs">ids separados por ','. Ex.: 18,34,12,43</param>
        /// <returns></returns>
        public IList<CustoFixo> GetByPKs(string listPKs)
        {
            string sql = "Select * From custo_fixo Where idCustoFixo In (" + listPKs.TrimEnd(',') + ")";

            return objPersistence.LoadData(sql).ToList();
        }

        #endregion

        #region Atualiza valor, dia vencimento e data de geração

        public void UpdateValorDiaVenc(uint idCustoFixo, decimal valor, int diaVenc)
        {
            UpdateValorDiaVenc(null, idCustoFixo, valor, diaVenc);
        }

        /// <summary>
        /// Atualiza o valor, dia de vencimento e data de geração do custo fixo
        /// </summary>
        /// <param name="idCustoFixo"></param>
        /// <param name="valor"></param>
        /// <param name="diaVenc"></param>
        public void UpdateValorDiaVenc(GDASession sessao, uint idCustoFixo, decimal valor, int diaVenc)
        {
            string sql = "Update custo_fixo set valorvenc=" + valor.ToString().Replace(",", ".") + ", diavenc=" + diaVenc +
                ", DataUltGerado=Now() Where idCustoFixo=" + idCustoFixo;

            objPersistence.ExecuteCommand(sessao, sql);
        }

        #endregion

        #region Busca custos fixos para serem gerados

        private string SqlGerar(string mesAno, uint idLoja, uint idFornecedor)
        {
            string sql = @"
                Select c.*, Coalesce(f.NomeFantasia, f.RazaoSocial, '') as NomeFornec, l.NomeFantasia as NomeLoja, 
                    Concat(g.Descricao, ' - ', p.Descricao) as DescrPlanoConta 
                From custo_fixo c 
                    Left Join fornecedor f On (c.IdFornec=f.IdFornec) 
                    Left Join loja l On (c.IdLoja=l.IdLoja) 
                    Left Join plano_contas p On (c.IdConta=p.IdConta) 
                    Left Join grupo_conta g On (p.IdGrupo=g.IdGrupo) 
                Where c.Situacao=" + (int)CustoFixo.SituacaoEnum.Ativo;

            // Se o mês e o ano não tiverem sido especificados ou forem inválidos, não retorna nada
            if (String.IsNullOrEmpty(mesAno) || mesAno.Length != 7 || mesAno.IndexOf("/") == -1 ||
                Glass.Conversoes.StrParaInt(mesAno.Substring(0, 2)) > 31)
                return sql + " And 0>1";

            if (idLoja > 0)
                sql += " And c.IdLoja=" + idLoja;

            if (idFornecedor > 0)
                sql += " And c.idFornec=" + idFornecedor;

            string mes = mesAno.Substring(0, 2);
            string ano = mesAno.Substring(3, 4);

            // Busca custos fixos que nunca foram gerados ou que não foram gerados no mês passado por parâmetro
            sql += " And (c.DataUltGerado is null Or idCustoFixo Not In (Select 0 Union Select IdCustoFixo From contas_pagar where Month(DataVenc)=" + 
                mes + " And Year(DataVenc)=" + ano + " And IdCustoFixo is not null and idPagtoRestante is null))";

            return sql;
        }

        /// <summary>
        /// Retornar custos fixos ativos que ainda não foram gerados no mês/ano passado
        /// </summary>
        /// <param name="mesAno"></param>
        public CustoFixo[] GetToGenerate(string mesAno, uint idLoja, uint idFornec, string sortExpression)
        {
            sortExpression = !String.IsNullOrEmpty(sortExpression) ? sortExpression : "diaVenc, descricao";
            List<CustoFixo> lst = objPersistence.LoadData(SqlGerar(mesAno, idLoja, idFornec) + " Order By " + sortExpression);

            // Procura o custo fixo relacionado ao salário para alterar seu valor
            foreach (CustoFixo c in lst)
                if (c.IdConta == UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.Salario))
                {
                    c.ValorVenc = ExecuteScalar<decimal>(
                        "Select Coalesce(Sum(coalesce(salario, 0) + coalesce(gratificacao, 0) + " +
                        "coalesce(auxalimentacao, 0)), 0) From funcionario Where situacao=" + (int)Situacao.Ativo + 
                        " and idLoja=" + idLoja);

                    break;
                }

            return lst.ToArray();
        }

        #endregion

        #region Gerar custo fixo

        public class RetornoGerar
        {
            public enum TipoRetorno
            {
                OK,
                JaExiste,
                DataInvalida
            }

            public TipoRetorno Tipo;

            public string Descricao;
        }


        private static object gerarCustoFixo = new object();

        public RetornoGerar Gerar(uint idCustoFixo, string mesAno, int diaVenc, decimal valorVenc, string obs)
        {
            lock(gerarCustoFixo)
            {
                using (var transaction = new GDATransaction())
                {
                    try
                    {
                        transaction.BeginTransaction();

                        CustoFixo c = GetElement(transaction, idCustoFixo);

                        RetornoGerar retorno = new RetornoGerar();
                        retorno.Descricao = c.Descricao;

                        // Verifica se o dia escolhido para gerar o custo fixo é válido.
                        if (!FuncoesData.ValidaData(diaVenc + "/" + mesAno))
                        {
                            retorno.Tipo = RetornoGerar.TipoRetorno.DataInvalida;
                            return retorno;
                        }

                        // Verifica se este custo fixo já foi gerado para este mês
                        else if (ContasPagarDAO.Instance.ExistsCustoFixo(transaction, idCustoFixo, mesAno))
                        {
                            retorno.Tipo = RetornoGerar.TipoRetorno.JaExiste;
                            return retorno;
                        }

                        ContasPagar contaPagar = new ContasPagar();
                        contaPagar.IdCustoFixo = c.IdCustoFixo;
                        contaPagar.DataVenc = DateTime.Parse(diaVenc + "/" + mesAno);
                        contaPagar.IdConta = c.IdConta;
                        contaPagar.IdFornec = c.IdFornec;
                        contaPagar.IdLoja = c.IdLoja;
                        contaPagar.Paga = false;
                        contaPagar.ValorVenc = valorVenc;
                        contaPagar.Contabil = c.Contabil;
                        contaPagar.Obs = obs;

                        // Atualizar o valor, a data de vencimento e a data que foi gerado deste custo fixo
                        UpdateValorDiaVenc(transaction, idCustoFixo, valorVenc, diaVenc);

                        // Insere conta a pagar e atualiza o número das parcelas
                        ContasPagarDAO.Instance.Insert(transaction, contaPagar);
                        ContasPagarDAO.Instance.AtualizaNumParcCustoFixo(transaction, idCustoFixo);

                        retorno.Tipo = RetornoGerar.TipoRetorno.OK;

                        transaction.Commit();
                        transaction.Close();

                        return retorno;

                    } catch (Exception ex)
                    {
                        transaction.Rollback();
                        transaction.Close();
                        ErroDAO.Instance.InserirFromException("CustoFixoDAO(Gerar). IdCustoFixo:" + idCustoFixo, ex);
                        throw;
                    }
                }
            }
        }

        #endregion

        #region Retificar Custo Fixo

        /// <summary>
        /// Altera o valor e/ou a data de vencimento das contas geradas pelo custo fixo passado para o mês informado
        /// </summary>
        /// <param name="idCustoFixo"></param>
        /// <param name="mesAno"></param>
        /// <param name="diaVenc"></param>
        /// <param name="valor"></param>
        public void RetificarCustoFixo(uint idCustoFixo, string mesAno, int diaVenc, decimal valor)
        {
            // Se o custo fixo não existir
            if (!ContasPagarDAO.Instance.ExistsCustoFixo(idCustoFixo, mesAno))
                throw new Exception("Este Custo Fixo não foi gerado para o mês especificado.");

            // Se a conta a pagar referente ao custo fixo e ao mês passados já tiver sido paga
            if (ContasPagarDAO.Instance.IsCustoFixoPago(idCustoFixo, mesAno))
                throw new Exception("Este Custo Fixo consta como pago para o mês especificado, portanto, não pode ser alterado.");

            // Busca a conta a pagar referente ao custo fixo e ao mês passados, e atualiza a data de vencimento e o valor
            ContasPagar contaPagar = ContasPagarDAO.Instance.GetByCustoFixo(idCustoFixo, mesAno);
            contaPagar.DataVenc = DateTime.Parse(diaVenc + "/" + mesAno);
            contaPagar.ValorVenc = valor;
            ContasPagarDAO.Instance.Update(contaPagar);

            throw new Exception("Operação concluída.");
        }

        #endregion

        #region Verifica se o custo fixo existe

        /// <summary>
        /// Verifica se o custo fixo existe.
        /// </summary>
        /// <param name="idCustoFixo"></param>
        /// <returns></returns>
        public override bool Exists(uint idCustoFixo)
        {
            return CurrentPersistenceObject.ExecuteSqlQueryCount("select count(*) from custo_fixo where idCustoFixo=" + idCustoFixo) > 0;
        }

        #endregion

        #region Métodos sobrescritos

        public override int Delete(CustoFixo objDelete)
        {
            int retorno = 1;

            // Exclui contas a pagar não pagas
            ContasPagarDAO.Instance.DeleteFromCustoFixo(objDelete.IdCustoFixo);

            // Se houver alguma conta a pagar paga, apenas inativa custo fixo, caso contrário, exclui
            if (ContasPagarDAO.Instance.IsCustoFixoPago(objDelete.IdCustoFixo))
                retorno = objPersistence.ExecuteCommand("Update custo_fixo Set situacao=" + (int)CustoFixo.SituacaoEnum.Inativo + 
                    " Where idCustoFixo=" + objDelete.IdCustoFixo);
            else
                retorno = base.Delete(objDelete);

            return retorno;
        }

        #endregion

        public int SetPontoEquilibrio(uint idCustoFixo, bool valor)
        {
            string sql = "update custo_fixo set pontoEquilibrio=" + valor + " where idCustoFixo=" + idCustoFixo;
            return objPersistence.ExecuteCommand(sql);
        }

        public IList<CustoFixo> GetListPE(uint idCustoFixo, uint idLoja, uint idFornec, string nomeFornec, int diaVencIni, int diaVencFim,
            string descricao, int situacao, string sortExpression, int startRow, int pageSize)
        {
            string filtroAdicional;
            string sql = SqlPE(idCustoFixo, idLoja, idFornec, nomeFornec, diaVencIni, diaVencFim, situacao, descricao, true, out filtroAdicional).
                Replace("?filtroAdicional?", "");

            return LoadDataWithSortExpression(sql, sortExpression, startRow, pageSize, false, filtroAdicional, GetParams(nomeFornec, descricao, null, null));
        }

        public int GetCountPE(uint idCustoFixo, uint idLoja, uint idFornec, string nomeFornec, int diaVencIni, int diaVencFim,
            string descricao, int situacao)
        {
            string filtroAdicional;
            string sql = SqlPE(idCustoFixo, idLoja, idFornec, nomeFornec, diaVencIni, diaVencFim, situacao, descricao, true, out filtroAdicional).
                Replace("?filtroAdicional?", "");

            return GetCountWithInfoPaging(sql, false, filtroAdicional, GetParams(nomeFornec, descricao, null, null));
        }
    }
}