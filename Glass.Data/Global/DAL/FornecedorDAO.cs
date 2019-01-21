using System;
using System.Collections.Generic;
using GDA;
using Glass.Data.Model;
using Glass.Data.Helper;
using System.Linq;
using Glass.Configuracoes;
using GDA;

namespace Glass.Data.DAL
{
    public sealed class FornecedorDAO : BaseCadastroDAO<Fornecedor, FornecedorDAO>
    {
        //private FornecedorDAO() { }

        #region Listagem padrão de Fornecedores

        private string Sql(uint idFornec, string nomeFornec, int situacao, string cnpj, bool comCredito, bool selecionar)
        {
            string campos = selecionar ? @"f.*, if(f.idCidade is null, f.cidade, cid.NomeCidade) as nomeCidade, cid.NomeUf as uf, " +
                "pa.Descricao As Parcela, pc.Descricao as DescrPlanoConta, p.NomePais as nomePais, '$$$' as criterio" : "Count(*)";

            string criterio = String.Empty;

            string sql = @"
                Select " + campos + @" From fornecedor f
                    Left Join cidade cid On (cid.idCidade=f.idCidade)
                    Left Join plano_contas pc On (f.idConta=pc.idConta)
                    Left Join pais p On (f.idPais=p.idPais)
                    Left Join parcelas pa On(pa.IdParcela=f.TipoPagto)
                Where 1";

            if (idFornec > 0)
            {
                sql += " And f.IdFornec=" + idFornec;
                criterio += "Fornecedor: " + GetNome(idFornec) + "    ";
            }

            if (!String.IsNullOrEmpty(nomeFornec))
            {
                sql += " And (f.NomeFantasia Like ?nomeFornec Or f.RazaoSocial Like ?nomeFornec)";
                criterio += "Fornecedor: " + nomeFornec + "    ";
            }

            if (!String.IsNullOrEmpty(cnpj))
            {
                sql += " And Replace(Replace(Replace(f.cpfCnpj, '.', ''), '-', ''), '/', '') Like ?cnpj";
                criterio += "CPF/CNPJ: " + cnpj + "    ";
            }

            if (comCredito)
                sql += " And f.credito>0";

            if (situacao > 0)
                sql += " And f.situacao=" + situacao;

            return sql.Replace("$$$", criterio);    
        }

        /// <summary>
        /// Retorna um fornecedor.
        /// </summary>
        /// <param name="idFornec">O identificador do fornecedor.</param>
        /// <returns>A model de fornecedor preenchida.</returns>
        public Fornecedor GetElement(uint idFornec)
        {
            using (var sessao = new GDATransaction())
            {
                return this.GetElement(sessao, idFornec);
            }
        }

        /// <summary>
        /// Retorna um fornecedor.
        /// </summary>
        /// <param name="sessao">A sessão atual.</param>
        /// <param name="idFornec">O identificador do fornecedor.</param>
        /// <returns>A model de fornecedor preenchida.</returns>
        public Fornecedor GetElement(GDASession sessao, uint idFornec)
        {
            return this.objPersistence.LoadOneData(sessao, this.Sql(idFornec, null, 0, null, false, true));
        }

        public IList<Fornecedor> GetList(uint idFornec, string nomeFornec, int situacao, string cnpj, bool comCredito, string sortExpression, int startRow, int pageSize)
        {
            return LoadDataWithSortExpression(Sql(idFornec, nomeFornec, situacao, cnpj, comCredito, true), sortExpression, startRow, pageSize, GetParam(nomeFornec, cnpj));
        }

        public int GetCount(uint idFornec, string nomeFornec, int situacao, string cnpj, bool comCredito)
        {
            return objPersistence.ExecuteSqlQueryCount(Sql(idFornec, nomeFornec, situacao, cnpj, comCredito, false), GetParam(nomeFornec, cnpj));
        }

        private GDAParameter[] GetParam(string nomeFornec, string cnpj)
        {
            List<GDAParameter> lstParam = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(nomeFornec))
                lstParam.Add(new GDAParameter("?nomeFornec", "%" + nomeFornec + "%"));

            if (!String.IsNullOrEmpty(cnpj))
                lstParam.Add(new GDAParameter("?cnpj", "%" + cnpj.Replace(".", "").Replace("-", "").Replace("/", "") + "%"));

            return lstParam.Count > 0 ? lstParam.ToArray() : null;
        }

        public IList<Fornecedor> GetOrdered()
        {
            return objPersistence.LoadData("Select * From fornecedor f Order By " + FornecedorDAO.Instance.GetNomeFornecedor("f")).ToList();
        }


        public IList<Fornecedor> GetListCredito(uint idFornec, string nome, string cpfCnpj, string sortExpression, int startRow, int pageSize)
        {
            if (string.IsNullOrEmpty(sortExpression))
                sortExpression = "NomeFantasia";

            string sql = Sql(idFornec, nome, 0, cpfCnpj, true, true);

            return LoadDataWithSortExpression(sql, sortExpression, startRow, pageSize, GetParam(nome, cpfCnpj));
        }

        public int GetCountListCredito(uint idFornec, string nome, string cpfCnpj)
        {
            string sql = Sql(idFornec, nome, 0, cpfCnpj, true, false);

            return objPersistence.ExecuteSqlQueryCount(sql, GetParam(nome, cpfCnpj));
        }

        public Fornecedor[] GetListCreditoRpt(uint idFornec, string nome, string cpfCnpj)
        {
            string sql = Sql(idFornec, nome, 0, cpfCnpj, true, true);

            return objPersistence.LoadData(sql, GetParam(nome, cpfCnpj)).ToArray();
        }

        #endregion

        #region Busca Fornecedores utilizando filtros

        public string SqlFiltro(string idFornec, string nome, string cnpj, bool selecionar)
        {
            string campos = selecionar ? "f.*, if(f.idCidade is null, f.cidade, cid.NomeCidade) as nomeCidade, cid.NomeUf as uf" : "Count(*)";

            string sql = @"
                Select " + campos + @" From fornecedor f 
                    Left Join cidade cid On (cid.idCidade=f.idCidade) 
                Where 1 And situacao=" + (int)SituacaoFornecedor.Ativo;

            if (!String.IsNullOrEmpty(nome))
                sql += " And (NomeFantasia Like ?nome Or RazaoSocial Like ?nome)";

            if (!String.IsNullOrEmpty(idFornec))
                sql += " And idFornec=?idFornec";

            if (!String.IsNullOrEmpty(cnpj))
                sql += " And Replace(Replace(Replace(f.cpfCnpj, '.', ''), '-', ''), '/', '') Like ?cnpj";

            return sql;
        }

        public IList<Fornecedor> GetFilter(string idFornec, string nome, string cnpj, string sortExpression, int startRow, int pageSize)
        {
            var gdaParam = new List<GDAParameter>();

            if (idFornec != null && idFornec != "")
                gdaParam.Add(new GDAParameter("?idFornec", idFornec));

            if (nome != null && nome != "")
                gdaParam.Add(new GDAParameter("?nome", "%" + nome + "%"));

            if (!String.IsNullOrEmpty(cnpj))
                gdaParam.Add(new GDAParameter("?cnpj", "%" + cnpj.Replace(".", "").Replace("-", "").Replace("/", "") + "%"));

            if (gdaParam.Count == 0)
                gdaParam = null;

            return LoadDataWithSortExpression(SqlFiltro(idFornec, nome, cnpj, true), sortExpression, startRow, pageSize, gdaParam != null ? gdaParam.ToArray() : null);
        }

        public int GetCountFilter(string idFornec, string nome, string cnpj)
        {
            List<GDAParameter> gdaParam = new List<GDAParameter>();

            if (idFornec != null && idFornec != "")
                gdaParam.Add(new GDAParameter("?idFornec", idFornec));

            if (nome != null && nome != "")
                gdaParam.Add(new GDAParameter("?nome", "%" + nome + "%"));

            if (!String.IsNullOrEmpty(cnpj))
                gdaParam.Add(new GDAParameter("?cnpj", "%" + cnpj.Replace(".", "").Replace("-", "").Replace("/", "") + "%"));

            if (gdaParam.Count == 0)
                gdaParam = null;

            return objPersistence.ExecuteSqlQueryCount(SqlFiltro(idFornec, nome, cnpj, false), gdaParam != null ? gdaParam.ToArray() : null);
        }

        /// <summary>
        /// Método para recuperar o id do fornecedor pelo seu CPF ou CNPJ.
        /// </summary>
        /// <param name="CpfOuCnpj">CPF ou CNPJ, formatado ou não.</param>
        /// <returns></returns>
        public string GetFornecedorByCPFCNPJ(string CpfOuCnpj)
        {
            string cpfCnpj = CpfOuCnpj.Replace(".", "").Replace("/", "").Replace("-", "");

            string sql = "Select Cast(Group_Concat(IdFornec) As char) From fornecedor Where Replace(Replace(Replace(cpfcnpj, '.', ''), '-', ''), '/', '')=?cpfCnpj";
            object retorno = objPersistence.ExecuteScalar(sql, new GDAParameter("?cpfCnpj", cpfCnpj));

            return retorno != null && retorno != DBNull.Value ? retorno.ToString() : null;
        }

        public uint? ObterIdPorCpfCnpj(GDASession sessao, string cpfCnpj)
        {
            if (string.IsNullOrEmpty(cpfCnpj))
                return null;

            cpfCnpj = cpfCnpj.Replace("/", "").Replace("-", "").Replace(" ", "").Replace(".", "");

            string sql = "Select IdFornec From fornecedor Where Replace(Replace(Replace(cpfcnpj, '.', ''), '-', ''), '/', '')=?cpfCnpj";
            return ExecuteScalar<uint?>(sessao, sql, new GDAParameter("?cpfCnpj", cpfCnpj));
        }

        #endregion

        #region Busca fornecedores para relatório

        private string SqlRpt(int tipoPessoa, uint idFornec, string nomeFornec, int situacao, string cnpj, bool comCredito, bool selecionar)
        {
            string campos = selecionar ? "f.*, if(f.idCidade is null, f.cidade, cid.NomeCidade) as nomeCidade, cid.NomeUf as uf, " +
                "pa.Descricao As Parcela, pc.Descricao as DescrPlanoConta, p.NomePais as nomePais, '^^^' as Criterio" : "Count(*)";

            string criterio = " ";

            string sql = @"
                Select " + campos + @" From fornecedor f 
                    Left Join cidade cid On (cid.idCidade=f.idCidade)
                    Left Join plano_contas pc On (f.idConta=pc.idConta)
                    Left Join pais p On (f.idPais=p.idPais)
                    Left Join parcelas pa On(pa.IdParcela=f.TipoPagto)
                Where 1 ";

            if (tipoPessoa > 0)
            {
                sql += " And f.TipoPessoa='" + (tipoPessoa == 1 ? "F" : "J") + "'";
                criterio += "Tipo Pessoa: " + (tipoPessoa == 1 ? "Física" : "Jurídica") + "    ";
            }

            if (idFornec > 0)
                sql += " And f.IdFornec=" + idFornec;
            else
            {
                if (!String.IsNullOrEmpty(nomeFornec))
                    sql += " And (f.NomeFantasia Like ?nomeFornec Or f.RazaoSocial Like ?nomeFornec)";
            }

            if (situacao > 0)
            {
                sql += " And f.situacao=" + situacao;
                criterio += "Situação: " + (situacao == (int)SituacaoFornecedor.Ativo ? "Ativo" : "Inativo") + "    ";
            }

            if (!String.IsNullOrEmpty(cnpj))
            {
                sql += " And Replace(Replace(Replace(f.cpfCnpj, '.', ''), '-', ''), '/', '') like ?cnpj";
                criterio += "CNPJ: " + cnpj + "    ";
            }

            if (comCredito)
            {
                sql += " And f.credito>0";
                criterio += "Fornecedores com crédito    ";
            }

            return sql.Replace("^^^", criterio);
        }

        public IList<Fornecedor> GetForRpt(int tipoPessoa, uint idFornec, string nomeFornec, int situacao, string cnpj, bool comCredito)
        {
            string sort = " Order By f.NomeFantasia, f.RazaoSocial";
            return objPersistence.LoadData(SqlRpt(tipoPessoa, idFornec, nomeFornec, situacao, cnpj, comCredito, true) + sort, GetParam(nomeFornec, cnpj)).ToList();
        }

        public IList<Fornecedor> GetForListaRpt(int tipoPessoa, uint idFornec, string nomeFornec, int situacao, string cnpj, bool comCredito, 
            string sortExpression, int startRow, int pageSize)
        {
            string orderBy = String.IsNullOrEmpty(sortExpression) ? "f.NomeFantasia, f.RazaoSocial" : sortExpression;
            return LoadDataWithSortExpression(SqlRpt(tipoPessoa, idFornec, nomeFornec, situacao, cnpj, comCredito, true), orderBy, startRow, pageSize, 
                GetParam(nomeFornec, cnpj));
        }

        public int GetRptCount(int tipoPessoa, uint idFornec, string nomeFornec, int situacao, string cnpj, bool comCredito)
        {
            return objPersistence.ExecuteSqlQueryCount(SqlRpt(tipoPessoa, idFornec, nomeFornec, situacao, cnpj, comCredito, false), GetParam(nomeFornec, cnpj));
        }

        #endregion

        #region Busca fornecedores para EFD

        /// <summary>
        /// Busca fornecedores para montar arquivo EFD
        /// </summary>
        /// <param name="idLoja"></param>
        /// <param name="dataInicio"></param>
        /// <param name="dataFim"></param>
        /// <returns></returns>
        public Fornecedor GetForEFD(uint idFornec)
        {
            return objPersistence.LoadOneData(@"select f.*, cid.codIbgeUf, cid.codIbgeCidade, cid.nomeUF as UF
                from fornecedor f left join cidade cid on (cid.idCidade=f.idCidade)
                where f.idFornec=" + idFornec);
        }

        #endregion
        
        #region Busca fornecedores para geração de compra

        /// <summary>
        /// Busca fornecedores para geração de compra de produtos associados à beneficiamentos.
        /// </summary>
        /// <returns>Retorna a lista de todos os fornecedores ativos, ordenados pelo nome fantasia.</returns>
        public List<Fornecedor> GetForCompraProdBenef()
        {
            return objPersistence.LoadData(
                @"Select f.* From fornecedor f
                Where f.situacao=" + (int)SituacaoFornecedor.Ativo +
                " Order By Coalesce(f.nomeFantasia, f.razaoSocial)");
        }

        #endregion

        #region Atualiza data da última compra

        /// <summary>
        /// Atualiza a data da última compra do fornecedor para hoje
        /// </summary>
        /// <param name="idFornec"></param>
        /// <param name="dataUltCompra"></param>
        public void AtualizaDataUltCompra(GDASession sessao, uint idFornec)
        {
            string sql = "Update fornecedor set DtUltCompra=Now() Where idFornec=" + idFornec;

            objPersistence.ExecuteCommand(sessao, sql);
        }

        #endregion

        #region Verifica se fornecedor já existe

        /// <summary>
        /// Verifica se já existe um fornecedor cadastrado com o CPF/CNPJ cadastrado
        /// </summary>
        /// <param name="cpfCnpj"></param>
        /// <returns></returns>
        public bool CheckIfExists(string cpfCnpj)
        {
            if (String.IsNullOrEmpty(cpfCnpj))
                return false;

            string sql = "Select Count(*) From fornecedor Where " +
                "Replace(Replace(Replace(CpfCnpj, '.', ''), '-', ''), '/', '')='" + cpfCnpj.Replace(".", "").Replace("-", "").Replace("/", "") + "'";

            return Glass.Conversoes.StrParaUint(objPersistence.ExecuteSqlQueryCount(sql).ToString()) > 0;
        }

        #endregion

        #region Situação do fornecedor

        /// <summary>
        /// Altera a situação de um fornecedor, ativando ou inativando o mesmo.
        /// </summary>
        /// <param name="idFornecedor"></param>
        public void AlteraSituacao(uint idFornecedor)
        {
            Fornecedor fornec = GetElementByPrimaryKey(idFornecedor);
            fornec.Situacao = fornec.Situacao == SituacaoFornecedor.Ativo ? SituacaoFornecedor.Inativo : SituacaoFornecedor.Ativo;
            Update(fornec);
        }

        public void InativaFornecedor(uint idFornecedor)
        {
            Fornecedor fornec = GetElementByPrimaryKey(idFornecedor);
            if (fornec.Situacao != SituacaoFornecedor.Inativo)
            {
                fornec.Situacao = SituacaoFornecedor.Inativo;
                Update(fornec);
            }
        }

        #endregion

        #region Verifica Vigência da tabela de preços

        /// <summary>
        /// Verifica se a vigência da tabela de preço do fornecedor
        /// esta expirada.
        /// </summary>
        /// <param name="idFornec"></param>
        /// <returns></returns>
        public bool VigenciaPrecoExpirada(uint idFornec)
        {
            DateTime? vigencia = ObtemDtVigenciaPreco(idFornec);

            if (vigencia.HasValue && vigencia.Value.Date < DateTime.Now.Date)
            {
                InativaFornecedor(idFornec);
                return true;
            }
            else
                return false;
        }

        #endregion

        #region Retorna Crédito

        /// <summary>
        /// Retorna o crédito do fornecedor
        /// </summary>
        public decimal GetCredito(uint idFornec)
        {
            return GetCredito(null, idFornec);
        }

        /// <summary>
        /// Retorna o crédito do fornecedor
        /// </summary>
        public decimal GetCredito(GDASession session, uint idFornec)
        {
            return ObtemValorCampo<decimal>(session, "credito", "idFornec=" + idFornec);
        }

        #endregion

        #region Debita Crédito

        public void DebitaCredito(GDASession sessao, uint idFornec, decimal valor)
        {
            string sql = "Update fornecedor Set credito=coalesce(credito, 0)-" + valor.ToString().Replace(',', '.') + " Where idFornec=" + idFornec;

            if (objPersistence.ExecuteCommand(sessao, sql, null) < 1)
                throw new Exception("Falha ao debitar crédito do fornecedor. Atualização afetou 0 registros.");
        }

        #endregion

        #region Credita Crédito

        public void CreditaCredito(GDASession sessao, uint idFornec, decimal valor)
        {
            string sql = "Update fornecedor Set credito=coalesce(credito, 0)+" + valor.ToString().Replace(',', '.') + " Where idFornec=" + idFornec;

            if (objPersistence.ExecuteCommand(sessao, sql, null) < 1)
                throw new Exception("Falha ao creditar crédito do fornecedor. Atualização afetou 0 registros.");
        }

        #endregion

        #region Verifica se o fornecedor é a própria empresa

        /// <summary>
        /// Verifica se o fornecedor é a própria empresa.
        /// </summary>
        /// <param name="idFornec"></param>
        /// <returns></returns>
        public bool IsFornecedorProprio(uint idFornec)
        {
            string formato = "replace(replace(replace({0}, '.', ''), '/', ''), '-', '')";
            string sql = "select count(*) from loja where " + String.Format(formato, "cnpj") + "=(select " + 
                String.Format(formato, "cpfCnpj") + " from fornecedor where idFornec=" + idFornec + ")";

            return objPersistence.ExecuteSqlQueryCount(sql) > 0;
        }

        #endregion

        #region Obtém ids dos fornecedores

        public string ObtemIds(string nomeFornec)
        {
            return ObtemIds(null, nomeFornec);
        }

        public string ObtemIds(GDASession session, string nomeFornec)
        {
            string sql = "Select f.* From fornecedor f Where 1 ";

            if (!String.IsNullOrEmpty(nomeFornec))
                sql += " And (f.RazaoSocial LIKE ?nomeFornec or f.NomeFantasia LIKE ?nomeFornec)";

            List<uint> ids = objPersistence.LoadResult(session, sql, GetParam(nomeFornec, null)).Select(f => f.GetUInt32(0))
                       .ToList(); ;

            if (ids.Count == 0)
                return "0";

            return String.Join(",", Array.ConvertAll<uint, string>(ids.ToArray(), new Converter<uint, string>(
                delegate(uint x)
                {
                    return x.ToString();
                }
            )));
        }

        #endregion

        #region Obtem dados do fornecedor

        public string GetNome(uint idFornec)
        {
            return GetNome(null, idFornec);
        }

        /// <summary>
        /// Retorna o nome do fornecedor
        /// </summary>
        /// <param name="idFornec"></param>
        /// <returns></returns>
        public string GetNome(GDASession session, uint idFornec)
        {
            string sql = "Select Coalesce(nomeFantasia, razaoSocial) From fornecedor Where idFornec=" + idFornec;

            object nome = objPersistence.ExecuteScalar(session, sql);

            return nome != null ? nome.ToString() : String.Empty;
        }

        /// <summary>
        /// Retorna o regime (CRT) do fornecedor
        /// 1-Regime Normal
        /// 2-Simples Nacional
        /// </summary>
        public int BuscaRegimeFornec(uint idNf)
        {
            return BuscaRegimeFornec(null, idNf);
        }

        /// <summary>
        /// Retorna o regime (CRT) do fornecedor
        /// 1-Regime Normal
        /// 2-Simples Nacional
        /// </summary>
        public int BuscaRegimeFornec(GDASession session, uint idNf)
        {
            string sql = "Select crt From fornecedor where idFornec = (Select idFornec From nota_fiscal Where idNf=" + idNf + ")";

            object obj = objPersistence.ExecuteScalar(session, sql);

            if (obj == null || String.IsNullOrEmpty(obj.ToString()))
                return 0;

            return Glass.Conversoes.StrParaInt(obj.ToString());
        }

        public uint? ObtemIdConta(GDASession session, uint idFornec)
        {
            return ObtemValorCampo<uint?>(session, "idConta", "idFornec=" + idFornec);
        }

        public int ObtemSituacao(uint idFornec)
        {
            return ObtemSituacao(null, idFornec);
        }

        public int ObtemSituacao(GDASession session, uint idFornec)
        {
            return ObtemValorCampo<int>(session, "situacao", "idFornec=" + idFornec);
        }

        public uint? ObtemTipoPagto(GDASession session, uint idFornec)
        {
            return ObtemValorCampo<uint?>(session, "tipoPagto", "idFornec=" + idFornec);
        }

        public uint ObtemIdCidade(uint idFornec)
        {
            return ObtemIdCidade(null, (int)idFornec);
        }

        public uint ObtemIdCidade(GDASession session, int idFornec)
        {
            return ObtemValorCampo<uint>(session, "idCidade", "idFornec=" + idFornec);
        }

        public string ObtemCpfCnpj(uint idFornec)
        {
            return ObtemCpfCnpj(null, idFornec);
        }

        public string ObtemCpfCnpj(GDASession session, uint idFornec)
        {
            return ObtemValorCampo<string>(session, "CpfCnpj", "IdFornec=" + idFornec);
        }

        public DateTime? ObtemDtVigenciaPreco(uint idFornec)
        {
            return ObtemValorCampo<DateTime?>("dataVigenciaPrecos", "IdFornec=" + idFornec);
        }

        /// <summary>
        /// Retorna a situação de um fornecedor.
        /// </summary>
        /// <param name="idFornecedor"></param>
        /// <returns></returns>
        public int GetSituacao(GDASession sessao, uint idFornecedor)
        {
            string sql = "select coalesce(situacao," + (int)SituacaoFornecedor.Inativo + ") from fornecedor where idFornec=" + idFornecedor;
            object retorno = objPersistence.ExecuteScalar(sessao, sql);
            return retorno != null && retorno != DBNull.Value && retorno.ToString() != "" ? Glass.Conversoes.StrParaInt(retorno.ToString()) : 2;
        }

        /// <summary>
        /// Obtém o valor da propriedade TELCONT do fornecedor.
        /// </summary>
        public string ObterTelCont(GDASession session, uint idFornec)
        {
            return ObtemValorCampo<string>(session, "TelCont", string.Format("IdFornec={0}", idFornec));
        }

        #endregion

        #region Busca fornecedores vinculados à um fornecedor

        /// <summary>
        /// Busca fornecedores vinculados ao fornecedor passado
        /// </summary>
        /// <param name="idFornec"></param>
        /// <returns></returns>
        public IList<Fornecedor> GetVinculados(int idFornec)
        {
            string sql = "SELECT * From FORNECEDOR WHERE IdFornec IN (SELECT IdFornecVinculo FROM fornecedor_vinculo WHERE IdFornec=" + idFornec + ")";
            return objPersistence.LoadData(sql).ToList();
        }

        #endregion

        public override int Delete(Fornecedor objDelete)
        {
            return DeleteByPrimaryKey(objDelete.IdFornec);
        }

        public override int DeleteByPrimaryKey(uint Key)
        {
            // Verifica se existem compras para este fornecedor
            if (CurrentPersistenceObject.ExecuteSqlQueryCount("Select Count(*) From compra Where idFornec=" + Key) > 0)
                throw new Exception("Este fornecedor não pode ser excluído pois existem compras relacionadas à ele.");

            // Verifica se existem contas a pagar/pagas para este fornecedor
            if (CurrentPersistenceObject.ExecuteSqlQueryCount("Select Count(*) From contas_pagar Where idFornec=" + Key) > 0)
                throw new Exception("Este fornecedor não pode ser excluído pois existem contas a pagar/pagas relacionadas à ele.");

            // Verifica se existem produtos associados à este fornecedor
            if (CurrentPersistenceObject.ExecuteSqlQueryCount("Select Count(*) From produto Where idFornec=" + Key) > 0)
                throw new Exception("Este fornecedor não pode ser excluído pois existem produtos relacionados à ele.");

            // Verifica se existem notas fiscais associadas à este fornecedor
            if (CurrentPersistenceObject.ExecuteSqlQueryCount("Select Count(*) From nota_fiscal Where idFornec=" + Key) > 0)
                throw new Exception("Este fornecedor não pode ser excluído pois existem notas fiscais relacionadas à ele.");

            LogAlteracaoDAO.Instance.ApagaLogFornecedor(Key);
            int retorno = GDAOperations.Delete(new Fornecedor { IdFornec = (int)Key });

            // Apaga os registros da tabela produto_fornecedor
            CurrentPersistenceObject.ExecuteCommand("delete from produto_fornecedor where idFornec=" + Key);

            CurrentPersistenceObject.ExecuteCommand("delete from parcelas_nao_usar where idfornec=" + Key);

            return retorno;
        }

        public override uint Insert(Fornecedor objInsert)
        {
            // Não permite que o nome do fornecedor possua ' ou "
            objInsert.Nomefantasia = objInsert.Nomefantasia != null ? objInsert.Nomefantasia.Replace("'", "").Replace("\"", "") : null;
            objInsert.Razaosocial = objInsert.Razaosocial != null ? objInsert.Razaosocial.Replace("'", "").Replace("\"", "") : null;

            return base.Insert(objInsert);
        }

        public override int Update(Fornecedor objUpdate)
        {
            // Não permite que o nome do fornecedor possua ' ou "
            objUpdate.Nomefantasia = objUpdate.Nomefantasia != null ? objUpdate.Nomefantasia.Replace("'", "").Replace("\"", "") : null;
            objUpdate.Razaosocial = objUpdate.Razaosocial != null ? objUpdate.Razaosocial.Replace("'", "").Replace("\"", "") : null;

            LogAlteracaoDAO.Instance.LogFornecedor(objUpdate);
            return base.Update(objUpdate);
        }

        public List<Fornecedor> ObterFornecedoresComUrlSistema()
        {
            string sql = "select * from fornecedor where UrlSistema is not null";
            return objPersistence.LoadData(sql);
        }

        public List<Fornecedor> ObterFornecedoresAtivos()
        {
            return objPersistence.LoadData("SELECT * FROM fornecedor WHERE Situacao=" + (int)SituacaoFornecedor.Ativo);
        }

        /// <summary>
        /// Retorna o campo que será usado no SQL para retornar o nome do fornecedor.
        /// </summary>
        /// <param name="aliasFornecedor"></param>
        /// <returns></returns>
        public string GetNomeFornecedor(string aliasFornecedor)
        {
            string retorno = Configuracoes.Liberacao.RelatorioLiberacaoPedido.TipoNomeExibirRelatorioPedido ==
                DataSources.TipoNomeExibirRelatorioPedido.NomeFantasia ?
                "coalesce({0}.nomeFantasia, {0}.razaoSocial)" : "coalesce({0}.razaoSocial, {0}.nomeFantasia)";

            return String.Format(retorno, aliasFornecedor);
        }
    }
}