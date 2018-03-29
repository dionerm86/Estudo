using System;
using System.Collections.Generic;
using GDA;
using Glass.Data.Model;

namespace Glass.Data.DAL
{
    public sealed class EtiquetaAplicacaoDAO : BaseDAO<EtiquetaAplicacao, EtiquetaAplicacaoDAO>
	{
        //private EtiquetaAplicacaoDAO() { }

        private string SqlList(int situacao, bool selecionar)
        {
            string campos = selecionar ? "*" : "Count(*)";

            string sql = "Select " + campos + " From etiqueta_aplicacao Where 1";

            if (situacao > 0)
                sql += " and situacao=" + situacao;

            return sql;
        }

        public IList<EtiquetaAplicacao> GetList(string sortExpression, int startRow, int pageSize)
        {
            if (GetCountReal() == 0)
            {
                List<EtiquetaAplicacao> lst = new List<EtiquetaAplicacao>();
                lst.Add(new EtiquetaAplicacao());
                return lst.ToArray();
            }

            string filtro = String.IsNullOrEmpty(sortExpression) ? "Descricao" : sortExpression;

            return LoadDataWithSortExpression(SqlList(0, true), filtro, startRow, pageSize, null);
        }

        public IList<EtiquetaAplicacao> GetForFilter()
        {
            return objPersistence.LoadData(SqlList(0, true) + " order by codInterno").ToList();
        }

        public int GetCountReal()
        {
            return objPersistence.ExecuteSqlQueryCount(SqlList(0, false), null);
        }

        public int GetCount()
        {
            int count = objPersistence.ExecuteSqlQueryCount(SqlList(0, false), null);

            return count == 0 ? 1 : count;
        }

        public IList<EtiquetaAplicacao> GetForSel(string sortExpression, int startRow, int pageSize)
        {
            string orderBy = String.IsNullOrEmpty(sortExpression) ? "Descricao" : sortExpression;

            return LoadDataWithSortExpression(SqlList((int)Glass.Situacao.Ativo, true), orderBy, startRow, pageSize, null);
        }

        public int GetForSelCount()
        {
            return objPersistence.ExecuteSqlQueryCount(SqlList((int)Glass.Situacao.Ativo, false), null);
        }

        public string GetCodInternoByIds(string idsAplicacao)
        {
            return objPersistence.ExecuteScalar("select cast(group_concat(codInterno separator ', ') as char) from " +
                "etiqueta_aplicacao where idAplicacao in (" + idsAplicacao + ")").ToString();
        }

        public IList<EtiquetaAplicacao> GetForSel()
        {
            return objPersistence.LoadData(SqlList((int)Glass.Situacao.Ativo, true) + " order by codInterno").ToList();
        }

        #region Obtem valores dos campos

        public uint? ObtemIdAplicacaoAtivo(string codInterno)
        {
            return ObtemIdAplicacaoAtivo(null, codInterno);
        }

        public uint? ObtemIdAplicacaoAtivo(GDASession session, string codInterno)
        {
            return ObtemValorCampo<uint?>(session, "idAplicacao", "codInterno=?codInterno and situacao=" + (int)Glass.Situacao.Ativo,
                new GDAParameter("?codInterno", codInterno));
        }

        public uint? ObtemIdAplicacao(string codInterno)
        {
            return ObtemIdAplicacao(null, codInterno);
        }

        public uint? ObtemIdAplicacao(GDASession session, string codInterno)
        {
            return ObtemValorCampo<uint?>(session, "idAplicacao", "codInterno=?codInterno", new GDAParameter("?codInterno", codInterno));
        }

        public string ObtemCodInterno(uint idAplicacao)
        {
            return ObtemCodInterno(null, idAplicacao);
        }

        public string ObtemCodInterno(GDASession session, uint idAplicacao)
        {
            return ObtemValorCampo<string>(session, "CodInterno", string.Format("IdAplicacao={0}", idAplicacao));
        }

        public string ObtemTipoPedido(uint idAplicacao)
        {
            return ObtemValorCampo<string>("TipoPedido", "idAplicacao=" + idAplicacao);
        }

        public bool ObtemGerarFormaInexistente(uint idAplicacao)
        {
            return ObtemGerarFormaInexistente(null, idAplicacao);
        }

        public bool ObtemGerarFormaInexistente(GDASession session, uint idAplicacao)
        {
            return ObtemValorCampo<bool>(session, "coalesce(gerarFormaInexistente, false)", "idAplicacao=" + idAplicacao);
        }

        public int ObtemDiasMinimos(GDASession session, uint idAplicacao)
        {
            return ObtemValorCampo<int>(session, "DiasMinimos", "idAplicacao=" + idAplicacao);
        }

        public bool NaoPermitirFastDelivery(uint idAplicacao)
        {
            return NaoPermitirFastDelivery(null, idAplicacao);
        }

        public bool NaoPermitirFastDelivery(GDASession session, uint idAplicacao)
        {
            return ObtemValorCampo<bool>(session, "NaoPermitirFastDelivery", string.Format("IdAplicacao={0}", idAplicacao));
        }

        #endregion

        #region Busca aplicação pelo seu código interno

        /// <summary>
        /// Busca aplicação pelo seu código interno
        /// </summary>
        /// <param name="codInterno"></param>
        /// <returns></returns>
        public EtiquetaAplicacao GetByCodInterno(string codInterno)
        {
            List<EtiquetaAplicacao> lst = objPersistence.LoadData(
                "Select * From etiqueta_aplicacao Where CodInterno=?codInterno And situacao=" + (int)Glass.Situacao.Ativo,
                new GDAParameter[] { new GDAParameter("?codInterno", codInterno) });

            return lst.Count > 0 ? lst[0] : null;
        }

        #endregion

        #region Métodos Sobrescritos

        public override int Update(EtiquetaAplicacao objUpdate)
        {
            string codInterno = ObtemCodInterno((uint)objUpdate.IdAplicacao);

            // Atualiza a configuração para ficar igual à esta alteração
            if (objUpdate.CodInterno != codInterno)
            {
                objPersistence.ExecuteCommand("Update config_loja set valorTexto=?codAplNovo Where idConfig In (20,22) And valorTexto=?codAplAntigo", 
                    new GDAParameter("?codAplNovo", objUpdate.CodInterno), new GDAParameter("?codAplAntigo", codInterno));
            }

            return base.Update(objUpdate);
        }

        public override int Delete(EtiquetaAplicacao objDelete)
        {
            return DeleteByPrimaryKey(objDelete.IdAplicacao);
        }

        public override int DeleteByPrimaryKey(uint idAplicacao)
        {
            // Verifica se esta aplicação está sendo usada em alguma parte do sistema
            if (objPersistence.ExecuteSqlQueryCount("Select Count(*) From produtos_pedido Where idAplicacao=" + idAplicacao) > 0)
                throw new Exception("Esta Aplicação não pode ser excluída por haver pedidos relacionados à mesma.");

            if (objPersistence.ExecuteSqlQueryCount("Select Count(*) From produtos_pedido_espelho Where idAplicacao=" + idAplicacao) > 0)
                throw new Exception("Esta Aplicação não pode ser excluída por haver conferências relacionadas à mesma.");

            if (objPersistence.ExecuteSqlQueryCount("Select Count(*) From material_item_projeto Where idAplicacao=" + idAplicacao) > 0)
                throw new Exception("Esta Aplicação não pode ser excluída por haver projetos relacionados à mesma.");

            if (objPersistence.ExecuteSqlQueryCount("Select Count(*) From ambiente_pedido Where idAplicacao=" + idAplicacao) > 0)
                throw new Exception("Esta Aplicação não pode ser excluída por haver pedidos de mão de obra relacionados à mesma.");

            if (objPersistence.ExecuteSqlQueryCount("Select Count(*) From ambiente_pedido_espelho Where idAplicacao=" + idAplicacao) > 0)
                throw new Exception("Esta Aplicação não pode ser excluída por haver pedidos de mão de obra relacionados à mesma.");

            if (objPersistence.ExecuteSqlQueryCount("Select Count(*) From benef_config Where idAplicacao=" + idAplicacao) > 0)
                throw new Exception("Esta Aplicação não pode ser excluída por haver beneficiamentos relacionados à mesma.");

            if (objPersistence.ExecuteSqlQueryCount("Select Count(*) From produto_troca_dev Where idAplicacao=" + idAplicacao) > 0)
                throw new Exception("Esta Aplicação não pode ser excluída por haver trocas relacionadas à mesma.");

            if (objPersistence.ExecuteSqlQueryCount("Select Count(*) From produto_trocado Where idAplicacao=" + idAplicacao) > 0)
                throw new Exception("Esta Aplicação não pode ser excluída por haver trocas relacionadas à mesma.");

            return base.DeleteByPrimaryKey(idAplicacao);
        }

        #endregion

        #region Verifica o numero de dias minimos para entrega do pedido com produtos da aplicação informada.

        /// <summary>
        /// Verifica o numero de dias minimos para entrega do pedido com produtos da aplicação informada.
        /// </summary>
        /// <returns></returns>
        public bool VerificaPrazoEntregaAplicacao(GDASession session, List<KeyValuePair<int, uint>> prods, DateTime dataEntrega, out string msg)
        {
            var dataAtual = DateTime.Now;

            foreach (var p in prods)
            {
                var diasMinimos = ObtemDiasMinimos(session, p.Value);

                if (diasMinimos == 0)
                    continue;

                var prazoMinimo = dataAtual.AddDays(diasMinimos);

                if (prazoMinimo.Date > dataEntrega.Date)
                {
                    var codInternoApl = ObtemCodInterno(p.Value);
                    var codInternoProduto = ProdutoDAO.Instance.GetCodInterno(session, p.Key);
                    msg = string.Format("A data de entrega do pedido é menor que o pazo mínimo da aplicação do produto.\n\nProduto: {0}\nAplicação: {1}\nData de Entrega: {2}\nPrazo mínimo: {3} dias ({4})",
                        codInternoProduto, codInternoApl, dataEntrega.ToShortDateString(), diasMinimos, prazoMinimo.ToShortDateString());
                    return false;
                }
            }

            msg = "";
            return true;
        }

        #endregion
    }
}