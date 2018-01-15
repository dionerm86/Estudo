using System;
using System.Collections.Generic;
using Glass.Data.Model;
using GDA;
using System.Linq;
using Colosoft;

namespace Glass.Data.DAL
{
    public sealed class TipoCartaoCreditoDAO : BaseDAO<TipoCartaoCredito, TipoCartaoCreditoDAO>
    {
        private string Sql(bool selecionar)
        {
            string sql = "Select " + (selecionar ? "tc.*, bc.Descricao AS DescBandeira, oc.Descricao AS DescOperadora" : "Count(tc.IdTipoCartao)") +
                @" From tipo_cartao_credito tc
                LEFT JOIN bandeira_cartao bc ON (tc.Bandeira=bc.IdBandeiraCartao)
                LEFT JOIN operadora_cartao oc ON (tc.Operadora=oc.IdOperadoraCartao)
                WHERE 1";

            return sql;
        }

        public IList<TipoCartaoCredito> GetList(string sortExpression, int startRow, int pageSize)
        {
            if (GetCountReal() == 0)
            {
                List<TipoCartaoCredito> lst = new List<TipoCartaoCredito>();
                lst.Add(new TipoCartaoCredito());
                return lst.ToArray();
            }

            string filtro = String.IsNullOrEmpty(sortExpression) ? "idTipoCartao" : sortExpression;

            return LoadDataWithSortExpression(Sql(true), filtro, startRow, pageSize, null);
        }

        public int GetCountReal()
        {
            return objPersistence.ExecuteSqlQueryCount(Sql(false), null);
        }

        public int GetCount()
        {
            int count = objPersistence.ExecuteSqlQueryCount(Sql(false), null);

            return count == 0 ? 1 : count;
        }

        /// <summary>
        /// Obtém os tipos de cartão de acordo com o tipo do recebimento
        /// </summary>
        /// <param name="tipo">0-Todos, 1-Apenas Débito, 2-Apenas Crédito</param>
        /// <returns></returns>
        public TipoCartaoCredito[] ObtemListaPorTipo(int tipo)
        {
            string sql = Sql(true);

            if (tipo == 1)
                sql += " And idTipoCartao In (2,4,6)";
            else if (tipo == 2)
                sql += " And idTipoCartao In (1,3,5)";

            return objPersistence.LoadData(sql + " Order By idTipoCartao").ToList().ToArray();
        }

        public IList<TipoCartaoCredito> GetOrdered()
        {
            return objPersistence.LoadData("Select * From tipo_cartao_credito Order By idTipoCartao").ToList();
        }

        /// <summary>
        /// Obtém os cartões de tipo credito
        /// </summary>
        public IList<TipoCartaoCredito> GetCredito()
        {
            return objPersistence.LoadData(@"SELECT tc.*, bc.Descricao AS DescBandeira, oc.Descricao AS DescOperadora
                FROM tipo_cartao_credito tc
                LEFT JOIN bandeira_cartao bc ON (tc.Bandeira=bc.IdBandeiraCartao)
                LEFT JOIN operadora_cartao oc ON (tc.Operadora=oc.IdOperadoraCartao)
                WHERE tc.Tipo = ?tipo Order By tc.idTipoCartao",
                new GDAParameter("?tipo", (int)TipoCartaoEnum.Credito)).ToList();
        }

        public uint? GetContaBanco(uint idTipoCartao)
        {
            object retorno = objPersistence.ExecuteScalar("select idContaBanco from assoc_conta_banco where idTipoCartao=" + idTipoCartao);
            return retorno != null && retorno.ToString() != "" && retorno != DBNull.Value ? (uint?)Glass.Conversoes.StrParaUint(retorno.ToString()) : null;
        }

        /// <summary>
        /// Verifica se o cartão de crédito tem a parcela especificada.
        /// </summary>
        /// <param name="idTipoCartao"></param>
        /// <param name="numParc"></param>
        /// <returns></returns>
        public bool TemParcela(uint idTipoCartao, int numParc)
        {
            return objPersistence.ExecuteSqlQueryCount("select count(*) from tipo_cartao_credito where idTipoCartao=" + idTipoCartao +
                " and numParc=" + numParc) > 0;
        }

        /// <summary>
        /// Retorna o número de parcelas de um cartão de crédito.
        /// </summary>
        /// <param name="idTipoCartao"></param>
        /// <returns></returns>
        public int GetNumParcelas(uint idTipoCartao)
        {
            object retorno = objPersistence.ExecuteScalar("select coalesce(numParc,0) from tipo_cartao_credito where idTipoCartao=" + idTipoCartao);
            return retorno != null && retorno.ToString() != "" ? Glass.Conversoes.StrParaInt(retorno.ToString()) : 0;
        }

        /// <summary>
        /// Retorna o maior número de parcelas para os cartões de crédito (usado no controle de formas de pagamento).
        /// </summary>
        /// <returns></returns>
        public int GetMaxNumParcelas()
        {
            object retorno = objPersistence.ExecuteScalar("select max(coalesce(numParc,0)) from tipo_cartao_credito");
            return retorno != null && retorno.ToString() != "" ? Glass.Conversoes.StrParaInt(retorno.ToString()) : 0;
        }

        /// <summary>
        /// Retorna as parcelas para exibição no drp
        /// </summary>
        /// <returns></returns>
        public Dictionary<int, string> GetParcelasDrop()
        {
            var numParcelas = new Dictionary<int, string>();

            for (int j = 1; j <= Instance.GetMaxNumParcelas(); j++)
                numParcelas.Add(j, (j + "x"));

            return numParcelas;
        }

        /// <summary>
        /// Altera o número de parcelas de um cartão de crédito.
        /// </summary>
        /// <param name="idTipoCartao"></param>
        /// <param name="numParc"></param>
        public void AlteraNumParc(uint idTipoCartao, int numParc)
        {
            // Altera o número de parcelas
            objPersistence.ExecuteCommand("update tipo_cartao_credito set numParc=" + numParc + " where idTipoCartao=" + idTipoCartao);

            // Limpa as parcelas excedentes na tabela de juros
            objPersistence.ExecuteCommand("delete from juros_parcela_cartao where idTipoCartao=" + idTipoCartao + " and numParc>" + numParc);

            // Cria as parcelas adicionais na tabela de juros
            foreach (uint idLoja in LojaDAO.Instance.GetIdsLojas())
                for (int i = 1; i <= numParc; i++)
                    if (!JurosParcelaCartaoDAO.Instance.TemParcela(idTipoCartao, idLoja, i))
                    {
                        JurosParcelaCartao novo = new JurosParcelaCartao();
                        novo.IdTipoCartao = idTipoCartao;
                        novo.NumParc = i;
                        JurosParcelaCartaoDAO.Instance.Insert(novo);
                    }
        }

        public void AtualizaLog(GDASession session, int idTipoCartao, string descricaoAnterior, int idLoja)
        {
            var descricao = JurosParcelaCartaoDAO.Instance.ObtemDescricaoJurosParcelas(session, idTipoCartao, idLoja);
            LogAlteracaoDAO.Instance.LogTipoCartao(session, idTipoCartao, idLoja, descricaoAnterior, descricao);
        }

        public TipoCartaoEnum ObterTipoCartao(GDASession session, int idTipoCartao)
        {
            return ExecuteScalar<TipoCartaoEnum>(session, "SELECT Tipo FROM tipo_cartao_credito WHERE IdTipoCartao = " + idTipoCartao);
        }

        public string ObterDescricao(GDASession session, int idTipoCartao)
        {
            var tipoCartao = GetElementByPrimaryKey(session, idTipoCartao);

            if (tipoCartao == null)
                return null;

            return tipoCartao.Descricao;
        }

        /// <summary>
        /// Verifica se o Tipo Cartão já está sendo utilizado em algum lugar do sistema.
        /// </summary>
        /// <param name="idTipoCartao"></param>
        /// <returns></returns>
        public bool TipoCartaoCreditoEmUso(uint IdTipoCartao)
        {
            return objPersistence.ExecuteSqlQueryCount(string.Format(@"
                SELECT COUNT(*) FROM (
                    SELECT IdTipoCartao FROM assoc_conta_banco WHERE IdTipoCartao={0} UNION ALL 
                    SELECT IdTipoCartao FROM juros_parcela_cartao WHERE IdTipoCartao={0} UNION ALL
                    SELECT IdTipoCartao FROM pedido WHERE IdTipoCartao={0} UNION ALL
                    SELECT IdTipoCartao FROM pagto_acerto WHERE IdTipoCartao={0} UNION ALL
                    SELECT IdTipoCartao FROM pagto_acerto_cheque WHERE IdTipoCartao={0} UNION ALL
                    SELECT IdTipoCartao FROM pagto_antecipacao_fornecedor WHERE IdTipoCartao={0} UNION ALL
                    SELECT IdTipoCartao FROM Pagto_Contas_Receber WHERE IdTipoCartao={0} UNION ALL
                    SELECT IdTipoCartao FROM pagto_credito_fornecedor WHERE IdTipoCartao={0} UNION ALL
                    SELECT IdTipoCartao FROM pagto_liberar_pedido WHERE IdTipoCartao={0} UNION ALL
                    SELECT IdTipoCartao FROM pagto_obra WHERE IdTipoCartao={0} UNION ALL
                    SELECT IdTipoCartao FROM pagto_pagto WHERE IdTipoCartao={0} UNION ALL
                    SELECT IdTipoCartao FROM pagto_sinal WHERE IdTipoCartao={0} UNION ALL
                    SELECT IdTipoCartao FROM pagto_sinal_compra WHERE IdTipoCartao={0} UNION ALL
                    SELECT IdTipoCartao FROM pagto_troca_dev WHERE IdTipoCartao={0} UNION ALL
                    SELECT IdTipoCartao FROM desconto_forma_pagamento_dados_produto WHERE IdTipoCartao={0}
                ) AS temp", IdTipoCartao)) > 0;
        }

        /// <summary>
        /// Retorna o idPlanoConta utilizado por todos os tipos de cartão, exceto pelo idTipoCartao Atual
        /// </summary>
        /// <param name="idTipoCartao"></param>
        /// <returns></returns>
        public List<uint> PlanosContaEmUsoTipoCartao(uint idTipoCartao)
        {
            return ExecuteMultipleScalar<uint>(string.Format(@"
                SELECT idConta FROM(
                SELECT idTipoCartao, idContaFunc AS idConta FROM tipo_cartao_credito UNION ALL
                SELECT idTipoCartao, idContaEntrada AS idConta FROM tipo_cartao_credito UNION ALL
                SELECT idTipoCartao, idContaEstorno AS idConta FROM tipo_cartao_credito UNION ALL
                SELECT idTipoCartao, idContaEstornoRecPrazo AS idConta FROM tipo_cartao_credito UNION ALL
                SELECT idTipoCartao, idContaEstornoEntrada AS idConta FROM tipo_cartao_credito UNION ALL
                SELECT idTipoCartao, idContaEstornoChequeDev AS idConta FROM tipo_cartao_credito UNION ALL
                SELECT idTipoCartao, idContaDevolucaoPagto AS idConta FROM tipo_cartao_credito UNION ALL
                SELECT idTipoCartao, idContaEstornoDevolucaoPagto AS idConta FROM tipo_cartao_credito UNION ALL
                SELECT idTipoCartao, idContaVista AS idConta FROM tipo_cartao_credito UNION ALL
                SELECT idTipoCartao, idContaRecPrazo AS idConta FROM tipo_cartao_credito UNION ALL
                SELECT idTipoCartao, idContaRecChequeDev AS idConta FROM tipo_cartao_credito) AS planos_conta_em_uso_tipo_cartao
                    WHERE idTipoCartao <> {0} AND idConta <> 0 ORDER BY idConta;", idTipoCartao));
        }

        public TipoCartaoCredito ObterTipoCartao(uint operadora, string bandeira, string tipoVenda)
        {
            var tipoCartao = GetAll().Where(f => f.Operadora == operadora);

            if (bandeira.ToLower().Contains("visa"))
                tipoCartao = tipoCartao.Where(f => f.Bandeira == BandeiraCartaoDAO.Instance.ObterIdBandeiraPelaDescricao("Visa"));
            else if (bandeira.ToLower().Contains("mastercard") || bandeira.ToLower().Contains("maestro"))
                tipoCartao = tipoCartao.Where(f => f.Bandeira == BandeiraCartaoDAO.Instance.ObterIdBandeiraPelaDescricao("MasterCard"));
            else if (bandeira.ToLower().Contains("elo"))
                tipoCartao = tipoCartao.Where(f => f.Bandeira == BandeiraCartaoDAO.Instance.ObterIdBandeiraPelaDescricao("Elo"));
            else if (bandeira.ToLower().Contains("american"))
                tipoCartao = tipoCartao.Where(f => f.Bandeira == BandeiraCartaoDAO.Instance.ObterIdBandeiraPelaDescricao("AmericanExpress"));
            else if (bandeira.ToLower().Contains("hipercard"))
                tipoCartao = tipoCartao.Where(f => f.Bandeira == BandeiraCartaoDAO.Instance.ObterIdBandeiraPelaDescricao("Hipercard"));
            else if (bandeira.ToLower().Contains("sorocred"))
                tipoCartao = tipoCartao.Where(f => f.Bandeira == BandeiraCartaoDAO.Instance.ObterIdBandeiraPelaDescricao("Sorocred"));
            else
                throw new Exception("Não foi possivel identificar o tipo de cartão. Bandeira não encontrada: " + bandeira);

            if (!string.IsNullOrEmpty(tipoVenda))
                tipoCartao = tipoCartao.Where(f => f.Tipo == TipoCartaoEnum.Debito);
            else
                tipoCartao = tipoCartao.Where(f => f.Tipo == TipoCartaoEnum.Credito);

            if (tipoCartao.Count() == 0)
                throw new Exception("Nenhum tipo de cartão foi encontrado. " + OperadoraCartaoDAO.Instance.ObterDescricaoOperadora(operadora) + " " + bandeira + " " + tipoVenda);

            if (tipoCartao.Count() > 1)
                throw new Exception("Mais de um tipo de cartão foi encontrado. " + OperadoraCartaoDAO.Instance.ObterDescricaoOperadora(operadora) + " " + bandeira + " " + tipoVenda);

            return tipoCartao.FirstOrDefault();
        }

        #region Métodos Sobrescritos

        public override int Delete(TipoCartaoCredito objDelete)
        {
            if (TipoCartaoCreditoEmUso(objDelete.IdTipoCartao))
                throw new Exception("O tipo cartão não pode ser deletado pois ele já está sendo utilizado.");

            return base.Delete(objDelete);
        }

        #endregion
    }
}
