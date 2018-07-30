using System;
using System.Collections.Generic;
using System.Text;
using Glass.Data.Model;
using GDA;

namespace Glass.Data.DAL
{
    public sealed class NaturezaOperacaoDAO : BaseDAO<NaturezaOperacao, NaturezaOperacaoDAO>
    {
        //private NaturezaOperacaoDAO() { }

        #region Busca das naturezas de operação

        private string Sql(uint idNaturezaOperacao, string codNaturezaOperacao, uint idCfop, string codigoCfop,
            string descricaoCfop, bool selecionar, out string filtroAdicional)
        {
            StringBuilder sql = new StringBuilder("select ");

            sql.Append(selecionar ? "no.*, c.codInterno as codCfop, c.descricao As descricaoCfop" : "count(*)");

            sql.AppendFormat(@"
                from natureza_operacao no
                    inner join cfop c on (no.idCfop=c.idCfop)
                where 1 {0}", FILTRO_ADICIONAL);

            StringBuilder fa = new StringBuilder();

            if (idNaturezaOperacao > 0)
                fa.AppendFormat(" and no.idNaturezaOperacao={0}", idNaturezaOperacao);

            if (!String.IsNullOrEmpty(codNaturezaOperacao))
                fa.AppendFormat(" And no.codInterno={0}", "?codNaturezaOperacao");

            if (idCfop > 0)
                fa.AppendFormat(" and no.idCfop={0}", idCfop);

            if (!String.IsNullOrEmpty(codigoCfop))
                fa.AppendFormat(" And no.idCfop In (Select idCfop From cfop Where codInterno={0})", "?codigoCfop");

            if (!String.IsNullOrEmpty(descricaoCfop))
                fa.AppendFormat(" And c.descricao like {0}", "?descricaoCfop");

            filtroAdicional = fa.ToString();
            return sql.ToString();
        }

        public IList<NaturezaOperacao> ObtemLista(uint idCfop, string sortExpression, int startRow, int pageSize)
        {
            return ObtemLista(null, idCfop, null, null, sortExpression, startRow, pageSize);
        }

        public int ObtemNumeroItens(uint idCfop)
        {
            string filtro;
            return GetCountWithInfoPaging(Sql(0, null, idCfop, null, null, true, out filtro), false, filtro);
        }

        public IList<NaturezaOperacao> ObtemLista(string codNaturezaOperacao, uint idCfop, string codigoCfop,
            string descricaoCfop, string sortExpression, int startRow, int pageSize)
        {
            sortExpression = !String.IsNullOrEmpty(sortExpression) ? sortExpression : "c.codInterno Asc, no.codInterno Asc";

            var filtro = "";
            var sql = Sql(0, codNaturezaOperacao, idCfop, codigoCfop, descricaoCfop, true, out filtro);

            return LoadDataWithSortExpression(sql, sortExpression, startRow, pageSize, false, filtro,
                ObtemParametros(codNaturezaOperacao, codigoCfop, descricaoCfop));
        }

        public int ObtemNumeroItens(string codNaturezaOperacao, uint idCfop, string codigoCfop, string descricaoCfop)
        {
            var filtro = "";
            var sql = Sql(0, codNaturezaOperacao, idCfop, codigoCfop, descricaoCfop, true, out filtro);

            return GetCountWithInfoPaging(sql, false, filtro, ObtemParametros(codNaturezaOperacao, codigoCfop, descricaoCfop));
        }

        /// <summary>
        /// Método usado no log de alteração da model RegraNaturezaOperacao
        /// </summary>
        /// <param name="idNaturezaOperacao"></param>
        /// <returns></returns>
        public NaturezaOperacao ObtemElemento(int idNaturezaOperacao)
        {
            return ObtemElemento(null, idNaturezaOperacao);
        }

        /// <summary>
        /// Método usado no log de alteração da model RegraNaturezaOperacao
        /// </summary>
        /// <param name="idNaturezaOperacao"></param>
        /// <returns></returns>
        public NaturezaOperacao ObtemElemento(GDASession sessao, int idNaturezaOperacao)
        {
            string filtro;
            var item = objPersistence.LoadData(sessao, Sql((uint)idNaturezaOperacao, null, 0, null, null, true, out filtro).Replace(FILTRO_ADICIONAL, filtro)).ToList();
            return item.Count > 0 ? item[0] : null;
        }

        public IList<NaturezaOperacao> ObtemTodosOrdenados()
        {
            string filtro;
            return objPersistence.LoadData(Sql(0, null, 0, null, null, true, out filtro).Replace(FILTRO_ADICIONAL, filtro) + 
                " order by c.codInterno, no.codInterno").ToList();
        }

        public IList<NaturezaOperacao> ObtemListaCfop(uint idCfop)
        {
            string filtro;
            return objPersistence.LoadData(Sql(0, null, idCfop, null, null, true, out filtro).Replace(FILTRO_ADICIONAL, filtro) +
                " order by c.codInterno, no.codInterno").ToList();
        }

        private GDAParameter[] ObtemParametros(string codNaturezaOperacao, string codigoCfop, string descricaoCfop)
        {
            var lstParam = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(codNaturezaOperacao))
                lstParam.Add(new GDAParameter("?codNaturezaOperacao", codNaturezaOperacao));

            if (!String.IsNullOrEmpty(codigoCfop))
                lstParam.Add(new GDAParameter("?codigoCfop", codigoCfop));

            if (!String.IsNullOrEmpty(descricaoCfop))
                lstParam.Add(new GDAParameter("?descricaoCfop", "%" + descricaoCfop + "%"));

            return lstParam.Count > 0 ? lstParam.ToArray() : null;
        }

        #endregion

        #region Obtém dados da natureza de operação

        public uint ObtemIdCfop(uint idNaturezaOperacao)
        {
            return ObtemIdCfop(null, idNaturezaOperacao);
        }

        public uint ObtemIdCfop(GDASession sessao, uint idNaturezaOperacao)
        {
            return ObtemValorCampo<uint>(sessao, "idCfop", "idNaturezaOperacao=" + idNaturezaOperacao);
        }

        public string ObtemCodigoInterno(uint idNaturezaOperacao)
        {
            return ObtemCodigoInterno(null, idNaturezaOperacao);
        }

        public string ObtemCodigoInterno(GDASession sessao, uint idNaturezaOperacao)
        {
            return ObtemValorCampo<string>(sessao, "codInterno", "idNaturezaOperacao=" + idNaturezaOperacao);
        }

        public string ObtemCodigoCompleto(uint idNaturezaOperacao)
        {
            return ObtemCodigoCompleto(null, idNaturezaOperacao);
        }

        public string ObtemCodigoCompleto(GDASession session, uint idNaturezaOperacao)
        {
            uint idCfop = ObtemIdCfop(session, idNaturezaOperacao);
            string codInterno = ObtemCodigoInterno(session, idNaturezaOperacao);

            return !String.IsNullOrEmpty(codInterno) ? codInterno :
                CfopDAO.Instance.ObtemCodInterno(session, idCfop);
        }

        /// <summary>
        /// Verifica se a natureza de operação calcula ICMS
        /// </summary>
        /// <param name="idNaturezaOperacao"></param>
        /// <returns></returns>
        public bool CalculaIcms(GDASession sessao, uint idNaturezaOperacao)
        {
            return ObtemValorCampo<bool>(sessao, "calcIcms", "idNaturezaOperacao=" + idNaturezaOperacao);
        }

        /// <summary>
        /// Verifica se a natureza de operação calcula ICMS ST
        /// </summary>
        /// <param name="idNaturezaOperacao"></param>
        /// <returns></returns>
        public bool CalculaIcmsSt(GDASession sessao, uint idNaturezaOperacao)
        {
            return ObtemValorCampo<bool>(sessao, "calcIcmsSt", "idNaturezaOperacao=" + idNaturezaOperacao);
        }

        /// <summary>
        /// Verifica se a natureza de operação calcula IPI
        /// </summary>
        /// <param name="idNaturezaOperacao"></param>
        /// <returns></returns>
        public bool CalculaIpi(GDASession sessao, uint idNaturezaOperacao)
        {
            return ObtemValorCampo<bool>(sessao, "calcIpi", "idNaturezaOperacao=" + idNaturezaOperacao);
        }

        /// <summary>
        /// Verifica se a natureza de operação calcula PIS
        /// </summary>
        /// <param name="idNaturezaOperacao"></param>
        /// <returns></returns>
        public bool CalculaPis(uint idNaturezaOperacao)
        {
            return CalculaPis(null, idNaturezaOperacao);
        }

        /// <summary>
        /// Verifica se a natureza de operação calcula PIS
        /// </summary>
        /// <param name="idNaturezaOperacao"></param>
        /// <returns></returns>
        public bool CalculaPis(GDASession sessao, uint idNaturezaOperacao)
        {
            return ObtemValorCampo<bool>(sessao, "calcPis", "idNaturezaOperacao=" + idNaturezaOperacao);
        }

        /// <summary>
        /// Verifica se a natureza de opereação calcula Cofins
        /// </summary>
        /// <param name="idNaturezaOperacao"></param>
        /// <returns></returns>
        public bool CalculaCofins(uint idNaturezaOperacao)
        {
            return CalculaCofins(null, idNaturezaOperacao);
        }

        /// <summary>
        /// Verifica se a natureza de opereação calcula Cofins
        /// </summary>
        /// <param name="idNaturezaOperacao"></param>
        /// <returns></returns>
        public bool CalculaCofins(GDASession sessao, uint idNaturezaOperacao)
        {
            return ObtemValorCampo<bool>(sessao, "calcCofins", "idNaturezaOperacao=" + idNaturezaOperacao);
        }

        /// <summary>
        /// Verifica se a natureza de operação é pra calcular icms de compra de energia elétrica
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idNaturezaOperacao"></param>
        /// <returns></returns>
        public bool CalculaEnergiaEletrica(GDASession sessao, uint idNaturezaOperacao)
        {
            return ObtemValorCampo<bool>("CalcEnergiaEletrica", "idNaturezaOperacao=" + idNaturezaOperacao);
        }

        /// <summary>
        /// Verifica se a natureza de operação integra o IPI na BC ICMS
        /// </summary>
        /// <param name="idNaturezaOperacao"></param>
        /// <returns></returns>
        public bool IpiIntegraBcIcms(GDASession sessao, uint idNaturezaOperacao)
        {
            return ObtemValorCampo<bool>(sessao, "ipiIntegraBcIcms", "idNaturezaOperacao=" + idNaturezaOperacao);
        }

        /// <summary>
        /// Verifica se a natureza de operação integra o Frete na BC IPI
        /// </summary>
        /// <param name="idNaturezaOperacao"></param>
        /// <returns></returns>
        public bool FreteIntegraBcIpi(GDASession sessao, uint idNaturezaOperacao)
        {
            return ObtemValorCampo<bool>(sessao, "freteIntegraBcIpi", "idNaturezaOperacao=" + idNaturezaOperacao);
        }

        /// <summary>
        /// Verifica se a natureza de operação passada deve alterar o estoque fiscal
        /// </summary>
        public bool AlterarEstoqueFiscal(uint idNaturezaOperacao)
        {
            return AlterarEstoqueFiscal(null, idNaturezaOperacao);
        }

        /// <summary>
        /// Verifica se a natureza de operação passada deve alterar o estoque fiscal
        /// </summary>
        public bool AlterarEstoqueFiscal(GDASession session, uint idNaturezaOperacao)
        {
            return ObtemValorCampo<bool>(session, "alterarEstoqueFiscal", "idNaturezaOperacao=" + idNaturezaOperacao);
        }

        /// <summary>
        /// Retorna o CST do ICMS configurado para a natureza de operação.
        /// </summary>
        /// <param name="idNaturezaOperacao"></param>
        /// <returns></returns>
        public string ObtemCstIcms(GDASession sessao, uint idNaturezaOperacao)
        {
            return ObtemValorCampo<string>(sessao, "cstIcms", "idNaturezaOperacao=" + idNaturezaOperacao);
        }

        /// <summary>
        /// Retorna o NCM configurado para a natureza de operação.
        /// </summary>
        /// <param name="idNaturezaOperacao"></param>
        /// <returns></returns>
        public string ObtemNcm(GDASession sessao, uint idNaturezaOperacao)
        {
            return ObtemValorCampo<string>(sessao, "Ncm", "idNaturezaOperacao=" + idNaturezaOperacao);
        }

        /// <summary>
        /// Retorna o CST do IPI configurado para a natureza de operação.
        /// </summary>
        /// <param name="idNaturezaOperacao"></param>
        /// <returns></returns>
        public ProdutoCstIpi? ObtemCstIpi(GDASession sessao, uint idNaturezaOperacao)
        {
            return ObtemValorCampo<ProdutoCstIpi?>(sessao, "cstIpi", "idNaturezaOperacao=" + idNaturezaOperacao);
        }

        /// <summary>
        /// Retorna o percentual de redução da BC ICMS para a natureza de operação.
        /// </summary>
        public float ObtemPercReducaoBcIcms(GDASession session, uint idNaturezaOperacao)
        {
            return ObtemValorCampo<float>(session, "percReducaoBcIcms", "idNaturezaOperacao=" + idNaturezaOperacao);
        }

        /// <summary>
        /// Retorna o CSOSN configurado para a natureza de operação.
        /// </summary>
        public string ObterCsosn(GDASession sessao, int idNaturezaOperacao)
        {
            return ObtemValorCampo<string>(sessao, "Csosn", $"IdNaturezaOperacao={ idNaturezaOperacao }");
        }

        #endregion

        #region Busca os ids de naturezas de operação a partir de um CFOP

        /// <summary>
        /// Busca os ids de naturezas de operação a partir de um CFOP
        /// </summary>
        /// <param name="idCfop"></param>
        /// <returns></returns>
        public string ObtemIdsNaturezaOperacaoPorCfop(uint idCfop)
        {
            string filtro;
            return GetValoresCampo(Sql(0, null, idCfop, null, null, true, out filtro).Replace(FILTRO_ADICIONAL, filtro), "idNaturezaOperacao");
        }

        #endregion

        #region Obtem id natureza operação

        /// <summary>
        /// Busca o id da natureza de operação a partir do id do CFOP e do codInterno da natureza de operação.
        /// </summary>
        /// <param name="idCfop"></param>
        /// <param name="codInternoNatOp"></param>
        public uint ObtemIdNatOpPorCfopCodInterno(uint idCfop, string codInternoNatOp)
        {
            return ObtemIdNatOpPorCfopCodInterno(null, idCfop, codInternoNatOp);
        }

        /// <summary>
        /// Busca o id da natureza de operação a partir do id do CFOP e do codInterno da natureza de operação.
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idCfop"></param>
        /// <param name="codInternoNatOp"></param>
        public uint ObtemIdNatOpPorCfopCodInterno(GDASession sessao, uint idCfop, string codInternoNatOp)
        {
            if (!String.IsNullOrEmpty(codInternoNatOp))
                return ObtemValorCampo<uint>(sessao, "idNaturezaOperacao", "idCfop=" + idCfop + " And codInterno=?codInterno",
                    new GDAParameter("?codInterno", codInternoNatOp));
            else
                return ObtemValorCampo<uint>(sessao, "idNaturezaOperacao", "idCfop=" + idCfop);
        }

        #endregion

        #region Verificações
        private void ValidaCodigoInterno(NaturezaOperacao obj)
        {
            var pCfop = new GDAParameter("?cfop", obj.IdCfop);
            var codCfop = CfopDAO.Instance.ObtemCodInterno((uint)obj.IdCfop);

            if (obj.CodInterno != null)
                obj.CodInterno = obj.CodInterno.Trim();

            if (String.IsNullOrEmpty(obj.CodInterno))
            {
                if (objPersistence.ExecuteSqlQueryCount(@"select count(*) from natureza_operacao 
                    where idCfop=?cfop and coalesce(codInterno, '')='' and idNaturezaOperacao<>" + obj.IdNaturezaOperacao, pCfop) > 0)
                    throw new Exception("Já foi cadastrada a natureza de operação padrão para o CFOP " + codCfop + ".");
            }
            else
            {
                if (objPersistence.ExecuteSqlQueryCount(@"select count(*) from natureza_operacao 
                    where idCfop=?cfop and codInterno=?cod and idNaturezaOperacao<>" + obj.IdNaturezaOperacao,
                    pCfop, new GDAParameter("?cod", obj.CodInterno)) > 0)
                    throw new Exception("O código '" + obj.CodInterno + "' já está cadastrado para o CFOP " + codCfop + ".");
            }
        }

        private void ValidaExistencia(uint idNaturezaOperacao, string inicioTextoErro)
        {
            string textoErro = inicioTextoErro + " Ela é utilizada em pelo menos {0}.";
            GDAParameter id = new GDAParameter("?id", idNaturezaOperacao);

            if (objPersistence.ExecuteSqlQueryCount("select count(*) from nota_fiscal where idNaturezaOperacao=?id", id) > 0)
                throw new Exception(String.Format(textoErro, "uma nota fiscal"));

            if (objPersistence.ExecuteSqlQueryCount("select count(*) from produtos_nf where idNaturezaOperacao=?id", id) > 0)
                throw new Exception(String.Format(textoErro, "um produto de nota fiscal"));

            if (objPersistence.ExecuteSqlQueryCount("select count(*) from conhecimento_transporte where idNaturezaOperacao=?id", id) > 0)
                throw new Exception(String.Format(textoErro, "um conhecimento de transporte"));

            if (objPersistence.ExecuteSqlQueryCount(@"select count(*) from regra_natureza_operacao where
                IDNATUREZAOPERACAOPRODINTRA=?id Or IDNATUREZAOPERACAOREVINTRA=?id Or IDNATUREZAOPERACAOPRODINTER=?id Or
                IDNATUREZAOPERACAOREVINTER=?id Or IDNATUREZAOPERACAOPRODSTINTRA=?id Or IDNATUREZAOPERACAOREVSTINTRA=?id Or
                IDNATUREZAOPERACAOPRODSTINTER=?id Or IDNATUREZAOPERACAOREVSTINTER=?id", id) > 0)
                throw new Exception(String.Format(textoErro, "uma regra de natureza de operação"));
        }

        /// <summary>
        /// Valida se o cfop pode ser utilizado na nota fiscal
        /// </summary>
        public bool ValidarCfop(int idNaturezaOperacao, int tipoDocumento)        {
            return ValidarCfop(null, idNaturezaOperacao, tipoDocumento);
        }

        /// <summary>
        /// Valida se o cfop pode ser utilizado na nota fiscal
        /// </summary>
        public bool ValidarCfop(GDASession session, int idNaturezaOperacao, int tipoDocumento)        {            var natOp = ObtemElemento(session, idNaturezaOperacao);

            //Se a nota fiscal for de entrada valida se o cfop é um cfop para notas de entrada
            if (tipoDocumento == 1 || tipoDocumento == 3)            {                if (natOp.CodCfop.StartsWith("1") || natOp.CodCfop.StartsWith("2") || natOp.CodCfop.StartsWith("3"))                    return true;                return false;            }

            //Se a nota fiscal for de saida valida se o cfop é um cfop para notas de saida
            else if (tipoDocumento == 2)            {                if (natOp.CodCfop.StartsWith("5") || natOp.CodCfop.StartsWith("6") || natOp.CodCfop.StartsWith("7"))                    return true;                return false;            }            return true;        }

        #endregion

        #region Métodos sobrescritos
        public override uint Insert(NaturezaOperacao objInsert)
        {
            /*ValidaCodigoInterno(objInsert);
            return base.Insert(objInsert);*/
            throw new NotImplementedException();
        }

        public override int Update(NaturezaOperacao objUpdate)
        {
            /*string codInternoAtual = ObtemCodigoInterno((uint)objUpdate.IdNaturezaOperacao);
            
            if (codInternoAtual != objUpdate.CodInterno)
            {
                if (!String.IsNullOrEmpty(codInternoAtual))
                    ValidaExistencia((uint)objUpdate.IdNaturezaOperacao, "Não é possível alterar o código dessa natureza de operação.");
                else
                    throw new Exception("Não é possível alterar o código da natureza de operação padrão.");
            }

            ValidaCodigoInterno(objUpdate);

            LogAlteracaoDAO.Instance.LogNaturezaOperacao(objUpdate);
            return base.Update(objUpdate);*/
            throw new NotImplementedException();
        }

        public override int Delete(NaturezaOperacao objDelete)
        {
            /*return DeleteByPrimaryKey(objDelete.IdNaturezaOperacao);*/
            throw new NotImplementedException();
        }

        public override int DeleteByPrimaryKey(uint Key)
        {
            /*if (String.IsNullOrEmpty(ObtemCodigoInterno(Key)))
                throw new Exception("Não é possível excluir a natureza de operação padrão.");

            ValidaExistencia(Key, "Não é possível excluir essa natureza de operação.");
            return base.DeleteByPrimaryKey(Key);*/
            throw new NotImplementedException();
        }

        #endregion
    }
}