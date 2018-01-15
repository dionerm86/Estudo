using System;
using System.Collections.Generic;
using Glass.Data.Model;
using GDA;
using Glass.Data.Helper;

namespace Glass.Data.DAL
{
    public sealed class LogProdutoDAO : BaseDAO<LogProduto, LogProdutoDAO>
	{
        //private LogProdutoDAO() { }

        private string Sql(uint grupo, uint? subgrupo, bool selecionar)
        {
            string filtroGrupo = "IdGrupoProd=" + grupo;
            string filtroSubgrupo = "IdSubgrupoProd" + (subgrupo > 0 ? "=" + subgrupo.Value : " is null");
            string campos = selecionar ? "lp.*, f.Nome as NomeFunc" : "Count(*)";

            string sql = "select " + campos + " " +
                "from log_produto lp left join funcionario f on (lp.IdFunc=f.IdFunc) " +
                "where " + filtroGrupo + " and " + filtroSubgrupo;

            return sql;
        }

        public LogProduto GetElementByProdGrupoSubgrupo(uint grupo, uint? subgrupo)
        {
            try
            {
                return objPersistence.LoadOneData(Sql(grupo, subgrupo, true));
            }
            catch
            {
                return new LogProduto();
            }
        }

        #region M�todos de busca de altera��es por grupo/subgrupo

        public IList<LogProduto> GetByProdGrupoSubgrupo(uint grupo, uint? subgrupo, string sortExpression, int startRow, int pageSize)
        {
            try
            {
                string filtro = String.IsNullOrEmpty(sortExpression) ? "DataAjuste desc" : sortExpression;
                return LoadDataWithSortExpression(Sql(grupo, subgrupo, true), filtro, startRow, pageSize, null);
            }
            catch
            {
                return new LogProduto[0];
            }
        }

        public int GetByProdGrupoSubgrupoCount(uint grupo, uint? subgrupo)
        {
            return objPersistence.ExecuteSqlQueryCount(Sql(grupo, subgrupo, false));
        }

        #endregion

        /// <summary>
        /// Retorna o SQL com o ajuste de pre�o.
        /// </summary>
        private string SqlAjustePreco(int tipoPrecoBase, int idGrupoProd, int idSubgrupoProd, Dictionary<int, string> valoresAjustar)
        {
            #region Vari�veis

            // Lista para salvar o comando de ajuste de cada pre�o a ser alterado.
            var sqlAtualizarCampo = new List<string>();
            // SQL com o formato padr�o do comando de atualiza��o de pre�o.
            var sqlBaseAtualizarCampo = "{0}={1} * {2}";
            // Lista para salvar o filtro dos pre�os que ser�o alterados.
            var sqlFiltroPreco = new List<string>();
            // SQL com o formato padr�o do filtro dos pre�os.
            var sqlBaseFiltroPreco = "({0} IS NOT NULL AND {0} <> 0)";
            // Vari�vel para salvar o filtro do produto/beneficiamento.
            var filtroProdutoBeneficiamento = string.Empty;
            // Dicion�rio com a identifica��o do pre�o e o nome do campo.
            var nomesCampoTipoPreco =
                new Dictionary<int, string> {
                    { 0, "CustoFabBase" },
                    { 1, string.Format("Custo{0}", idGrupoProd > 0 ? "Compra" : string.Empty) },
                    { 2, "ValorAtacado" },
                    { 3, "ValorBalcao" },
                    { 4, "ValorObra" } };

            #endregion

            #region Filtro produto/beneficiamento

            // Caso o ID do grupo seja maior que zero, a altera��o est� sendo feita em produto(s).
            if (idGrupoProd > 0)
            {
                // Filtro grupo.
                filtroProdutoBeneficiamento = string.Format("IdGrupoProd={0}", idGrupoProd);

                // Filtro subgrupo.
                if (idSubgrupoProd > 0)
                    filtroProdutoBeneficiamento += string.Format(" AND IdSubgrupoProd={0}", idSubgrupoProd);
            }
            // Altera��o de pre�o em beneficiamentos.
            else
                filtroProdutoBeneficiamento = string.Format("IdBenefConfig IN (SELECT IdBenefConfig FROM benef_config WHERE IdParent={0})", idSubgrupoProd);

            #endregion

            #region Comandos de atualiza��o campos e filtro do pre�o

            // Percorre o dicion�rio que cont�m todos os pre�os que ser�o atualizados.
            foreach (var chave in valoresAjustar.Keys)
            {
                /* Chamado 49672. */
                // O pre�o base deve ser alterado ap�s todos os pre�os.
                if (chave == tipoPrecoBase)
                    continue;

                // Salva na lista o comando de atualiza��o do campo, o pre�o ser� atualizado com o pre�o base multiplicado pelo percentual definido no ajuste.
                sqlAtualizarCampo.Add(string.Format(sqlBaseAtualizarCampo, nomesCampoTipoPreco[chave], nomesCampoTipoPreco[tipoPrecoBase], valoresAjustar[chave]));
                // Salva na lista um filtro espec�fico para o pre�o alterado, ele deve ser diferente de 0 e n�o nulo.
                sqlFiltroPreco.Add(string.Format(sqlBaseFiltroPreco, nomesCampoTipoPreco[chave]));
            }

            // Caso o pre�o base esteja sendo alterado, inclui o comando de atualiza��o dele por �ltimo.
            if (valoresAjustar.ContainsKey(tipoPrecoBase))
                sqlAtualizarCampo.Add(string.Format(sqlBaseAtualizarCampo, nomesCampoTipoPreco[tipoPrecoBase], nomesCampoTipoPreco[tipoPrecoBase], valoresAjustar[tipoPrecoBase]));

            #endregion

            return
                // Update base.
                string.Format("UPDATE {0} SET {1} WHERE {2} AND {3} IS NOT NULL AND {3} <> 0 AND ({4})",
                    // Define qual tabela ser� alterada.
                    idGrupoProd > 0 ? "produto" : "benef_config_preco",
                    // Recupera todos os comandos de altera��o de pre�o e concatena com v�rgula
                    sqlAtualizarCampo.Count > 0 ? string.Join(", ", sqlAtualizarCampo) : string.Empty,
                    // Filtro do produto/beneficiamento.
                    filtroProdutoBeneficiamento,
                    // Pre�o base do ajuste.
                    nomesCampoTipoPreco[tipoPrecoBase],
                    // Filtro dos pre�os alterados.
                    sqlFiltroPreco.Count > 0 ? string.Join(" OR ", sqlFiltroPreco) : string.Empty);
        }

        public override uint Insert(LogProduto objInsert)
        {
            FilaOperacoes.AjustePrecoProdutoBeneficiamento.AguardarVez();

            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    #region Valida ajuste de pre�o

                    // Aplica o ajuste de pre�o com base na porcentagem escolhida pelo usu�rio.
                    var valorAtacado = (100f + objInsert.AjusteAtacado) / 100f;
                    var valorBalcao = (100f + objInsert.AjusteBalcao) / 100f;
                    var valorObra = (100f + objInsert.AjusteObra) / 100f;
                    var valorCustoCompra = (100 + objInsert.AjusteCustoCompra) / 100;
                    var valorCustoFabBase = (100 + objInsert.AjusteCustoFabBase) / 100;

                    // Verifica se pre�o base do produto foi informado.
                    if (objInsert.IdGrupoProd > 0 && !(objInsert.TipoPrecoBase >= 0 && objInsert.TipoPrecoBase <= 4))
                        throw new Exception("Informe o pre�o base do ajuste.");

                    // Verifica se pre�o base do beneficiamento foi informado.
                    if (objInsert.IdGrupoProd.GetValueOrDefault() == 0 && !(objInsert.TipoPrecoBase >= 0 && objInsert.TipoPrecoBase <= 2))
                        throw new Exception("Informe o pre�o base do ajuste.");

                    #endregion

                    #region Recupera os valores que ser�o ajustados

                    // Vari�vel criada para salvar o identificador do pre�o e o valor a ser atualizado.
                    var valoresAjustar = new Dictionary<int, string>();

                    // Custo Fornecedor.
                    if (valorCustoFabBase >= 1)
                        valoresAjustar.Add(0, valorCustoFabBase.ToString().Replace(',', '.'));

                    // Custo Compra.
                    if (valorCustoCompra >= 1)
                        valoresAjustar.Add(1, valorCustoCompra.ToString().Replace(',', '.'));

                    // Valor Atacado.
                    if (valorAtacado >= 1)
                        valoresAjustar.Add(2, valorAtacado.ToString().Replace(',', '.'));

                    // Valor Balc�o.
                    if (valorBalcao >= 1)
                        valoresAjustar.Add(3, valorBalcao.ToString().Replace(',', '.'));

                    // Valor Obra.
                    if (valorObra >= 1)
                        valoresAjustar.Add(4, valorObra.ToString().Replace(',', '.'));

                    #endregion

                    // Recupera o SQL de atualiza��o de pre�o.
                    var sqlAtualizar = SqlAjustePreco(objInsert.TipoPrecoBase, (int)objInsert.IdGrupoProd.GetValueOrDefault(),
                        (int)objInsert.IdSubgrupoProd.GetValueOrDefault(), valoresAjustar);

                    // Executa o SQL retornado pelo m�todo SqlAjustePreco.
                    objPersistence.ExecuteCommand(transaction, sqlAtualizar);

                    // Insere o log do produto.
                    var inserir = InsertBase(transaction, objInsert);

                    if (inserir > 0)
                        InserirLog(transaction, objInsert.IdGrupoProd.Value, objInsert.IdSubgrupoProd);

                    transaction.Commit();
                    transaction.Close();

                    return inserir;
                }
                catch
                {
                    transaction.Rollback();
                    transaction.Close();
                    throw;
                }
                finally
                {
                    FilaOperacoes.AjustePrecoProdutoBeneficiamento.ProximoFila();
                }
            }
        }

        public uint InsertBase(LogProduto objInsert)
        {
            return InsertBase(null, objInsert);
        }

        public uint InsertBase(GDASession session, LogProduto objInsert)
        {
            // Cadastra o item
            return base.Insert(session, objInsert);
        }

        private void InserirLog(uint idGrupo, uint? idSubGrupo)
        {
            InserirLog(null, idGrupo, idSubGrupo);
        }

        private void InserirLog(GDASession session, uint idGrupo, uint? idSubGrupo)
        {
            var idsProduto = GrupoProdDAO.Instance.ObterIdProdGrupoProd(session, idGrupo, idSubGrupo);

            foreach (var idProd in idsProduto)
            {
                LogAlteracaoDAO.Instance.Insert(session, new LogAlteracao()
                {
                    Tabela = (int)LogAlteracao.TabelaAlteracao.Produto,
                    IdRegistroAlt = idProd,
                    NumEvento = LogAlteracaoDAO.Instance.GetNumEvento(session, LogAlteracao.TabelaAlteracao.Produto, idProd),
                    Campo = "Ajuste de Pre�o em Configura��es ",
                    DataAlt = DateTime.Now,
                    IdFuncAlt = Helper.UserInfo.GetUserInfo.CodUser
                });
            }
        }
    }
}