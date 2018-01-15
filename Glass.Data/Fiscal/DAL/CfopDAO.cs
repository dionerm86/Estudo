using System;
using System.Collections.Generic;
using Glass.Data.Model;
using GDA;
using System.Linq;

namespace Glass.Data.DAL
{
    public sealed class CfopDAO : BaseDAO<Cfop, CfopDAO>
    {
        //private CfopDAO() { }

        private string SqlList(string codInterno, string descricao, bool selecionar)
        {
            var campos = selecionar ? "c.*, tc.descricao as DescrTipo" : "Count(*)";
            var sql = @"
                Select " + campos + @" From cfop c 
                    Left Join tipo_cfop tc On (c.idTipoCfop=tc.idTipoCfop)
                Where 1 ";

            if (!String.IsNullOrEmpty(codInterno))
                sql += " And c.codInterno=?codInterno";

            if (!String.IsNullOrEmpty(descricao))
                sql += " And c.descricao like ?descricao";

            return sql;
        }

        public Cfop[] GetSortedByCodInterno()
        {
            var sql = SqlList(null, null, true);

            return objPersistence.LoadData(sql + " order by CodInterno asc").ToList().ToArray();
        }

        public Cfop[] GetList(string codInterno, string descricao, string sortExpression, int startRow, int pageSize)
        {
            var sort = String.IsNullOrEmpty(sortExpression) ? "CodInterno" : sortExpression;

            var sql = SqlList(codInterno, descricao, true);
            var retorno = LoadDataWithSortExpression(sql, sort, startRow, pageSize, GetParam(codInterno, descricao)).ToArray();

            return retorno;
        }

        public Cfop[] GetListForRpt()
        {
            return GetSortedByCodInterno();
        }

        public int GetCount(string codInterno, string descricao)
        {
            var sql = SqlList(codInterno, descricao, false);
            var retorno = objPersistence.ExecuteSqlQueryCount(sql, GetParam(codInterno, descricao));

            return retorno;
        }

        private GDAParameter[] GetParam(string codInterno, string descricao)
        {
            var lstParam = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(codInterno))
                lstParam.Add(new GDAParameter("?codInterno", codInterno));

            if (!String.IsNullOrEmpty(descricao))
                lstParam.Add(new GDAParameter("?descricao", "%" + descricao + "%"));

            return lstParam.Count > 0 ? lstParam.ToArray() : null;
        }

        /// <summary>
        /// Retorna a descrição do CFOP
        /// </summary>
        /// <param name="idCfop"></param>
        /// <param name="idLoja"></param>
        /// <returns></returns>
        public string GetDescricao(uint idCfop)
        {
            return ObtemValorCampo<string>("descricao", "idCfop=" + idCfop);
        }

        public string GetDescricoes(string idsCfop)
        {
            return string.Join(", ", ExecuteMultipleScalar<string>("SELECT Descricao from cfop WHERE IdCfop IN (" + idsCfop + ")"));
        }

        /// <summary>
        /// Retorna a observação do CFOP
        /// </summary>
        /// <param name="idCfop"></param>
        /// <param name="idLoja"></param>
        /// <returns></returns>
        public string GetObs(uint idCfop)
        {
            return GetObs(null, idCfop);
        }

        /// <summary>
        /// Retorna a observação do CFOP
        /// </summary>
        /// <param name="idCfop"></param>
        /// <param name="idLoja"></param>
        /// <returns></returns>
        public string GetObs(GDASession sessao, uint idCfop)
        {
            return ObtemValorCampo<string>(sessao, "obs", "idCfop=" + idCfop);
        }

        /// <summary>
        /// Obtém o tipo de mercadoria definido para o CFOP passado
        /// </summary>
        /// <param name="idCfop"></param>
        /// <returns></returns>
        public int? ObtemTipoMercadoria(uint idCfop)
        {
            return ObtemValorCampo<int?>("tipoMercadoria", "idCfop=" + idCfop);
        }

        /// <summary>
        /// Obtém o código interno definido para o CFOP passado
        /// </summary>
        /// <param name="idCfop"></param>
        public string ObtemCodInterno(uint idCfop)
        {
            return ObtemCodInterno(null, idCfop);
        }

        /// <summary>
        /// Obtém o código interno definido para o CFOP passado
        /// </summary>
        /// <param name="idCfop"></param>
        /// <param name="sessao"></param>
        public string ObtemCodInterno(GDASession sessao, uint idCfop)
        {
            return ObtemValorCampo<string>(sessao, "codInterno", "idCfop=" + idCfop);
        }

        /// <summary>
        /// Obtém o id do CFOP através do código interno informado por parâmetro.
        /// </summary>
        /// <param name="codInterno"></param>
        public uint ObtemIdCfop(string codInterno)
        {
            return ObtemIdCfop(null, codInterno);
        }

        /// <summary>
        /// Obtém o id do CFOP através do código interno informado por parâmetro.
        /// </summary>
        /// <param name="codInterno"></param>
        public uint ObtemIdCfop(GDASession sessao, string codInterno)
        {
            return ObtemValorCampo<uint>(sessao, "idCfop", "codInterno=?codInterno", new GDAParameter("?codInterno", codInterno));
        }

        /// <summary>
        /// Verifica se o CFOP passado é de entrada
        /// </summary>
        public bool IsCfopEntrada(uint idCfop)
        {
            return IsCfopEntrada(null, (int)idCfop);
        }

        /// <summary>
        /// Verifica se o CFOP passado é de entrada
        /// </summary>
        public bool IsCfopEntrada(GDASession session, int idCfop)
        {
            string codInterno = ObtemValorCampo<string>(session, "codInterno", "idCfop=" + idCfop);
            return Glass.Conversoes.StrParaInt(codInterno[0].ToString()) < 4;
        }

        /// <summary>
        /// Verifica se o CFOP passado deve alterar o estoque de terceiros
        /// </summary>
        public bool AlterarEstoqueTerceiros(uint idCfop)
        {
            return AlterarEstoqueTerceiros(null, idCfop);
        }

        /// <summary>
        /// Verifica se o CFOP passado deve alterar o estoque de terceiros
        /// </summary>
        public bool AlterarEstoqueTerceiros(GDASession session, uint idCfop)
        {
            return ObtemValorCampo<bool>(session, "alterarEstoqueTerceiros", "idCfop=" + idCfop);
        }

        public Cfop GetCfop(string codInterno)
        {
            string sql = "select * from cfop where codinterno='" + codInterno + "'";
            return objPersistence.LoadOneData(sql);
        }

        public Cfop GetCfop(uint idCfop)
        {
            return GetCfop(null, idCfop);
        }

        public Cfop GetCfop(GDASession session, uint idCfop)
        {
            string sql = "select * from cfop where idcfop='" + idCfop + "'";
            return objPersistence.LoadOneData(session, sql);
        }

        #region Pesquisar CFOP

        public string GetCFOPByCodInterno(string codInterno)
        {
            string sql = "select idcfop from cfop where codinterno='" + codInterno + "'";

            object obj = objPersistence.ExecuteScalar(sql);

            return obj != null && obj != DBNull.Value ? obj.ToString() : null;
        }

        #endregion

        #region Verifica se o CFOP é de devolução

        /// <summary>
        /// Verifica se o CFOP é de devolução.
        /// </summary>
        public bool IsCfopDevolucao(uint idCfop)
        {
            return IsCfopDevolucao(null, idCfop);
        }

        /// <summary>
        /// Verifica se o CFOP é de devolução.
        /// </summary>
        public bool IsCfopDevolucao(GDASession session, uint idCfop)
        {
            return IsCfopDevolucao(ObtemValorCampo<string>(session, "codInterno", "idCfop=" + idCfop));
        }

        /// <summary>
        /// Verifica se o CFOP é de devolução.
        /// </summary>
        /// <param name="codInterno"></param>
        /// <returns></returns>
        public bool IsCfopDevolucao(string codInterno)
        {
            List<string> cfopDevolucao = new List<string>() { "1201", "1202", "1203",
                "1204", "1208", "1209", "1410", "1411", "1503", "1504", "1505", "1506", 
                "1553", "1660", "1661", "1662", "1918", "1919", "2201", "2202", "2203", "2204", "2208", 
                "2209", "2410", "2411", "2503", "2504", "2505", "2506", "2553", "2660", "2661", "2662", "2918", "2919", 
                "3201", "3202", "3211", "3503", "3553", "5201", "5202", "5208", "5209", "5210", "5410", "5411", "5412", "5413", 
                "5503", "5553", "5555", "5556", "5660", "5661", "5662", "5918", "5919", "5921", "6201", "6202","6208", "6209", "6210", 
                "6410", "6411", "6412", "6413", "6503", "6553", "6555", "6556", "6660", "6661", "6662", "6918", "6919", "6921", "7201", 
                "7202", "7210", "7211", "7553", "7556",
                "7930" };

            return cfopDevolucao.Contains(codInterno);
        }

        /// <summary>
        /// Verifica se o CFOP é de devolução.
        /// </summary>
        /// <param name="idCfop"></param>
        /// <returns></returns>
        public bool IsCfopDevolucaoNFeRefereciada(uint idCfop)
        {
            var cfop = GetElementByPrimaryKey(idCfop);
            var tiposCfop = new GDA.Sql.Query("Devolucao").ToList<TipoCfop>();
            return tiposCfop.Any(f => f.IdTipoCfop == cfop.IdTipoCfop) && IsCfopDevolucaoNFeRefereciada(cfop.CodInterno);
        }

        /// <summary>
        /// Verifica se o CFOP é de devolução e necessita referenciar NFe na emissão do mesmo tipo.
        /// CFOPs que não precisam ter nota de referência quando emitir nota de devolução 
        /// 1.201, 1.202, 1.410, 1.411, 5,921 e 6,921  (NT 2013/005 v 1.20)
        /// </summary>
        /// <param name="codInterno"></param>
        /// <returns></returns>
        public bool IsCfopDevolucaoNFeRefereciada(string codInterno)
        {
            if (string.IsNullOrEmpty(codInterno))
                return true;
            List<string> cfopDevolucao = new List<string>() { "1203",
                "1204", "1208", "1209", "1503", "1504", "1505", "1506", 
                "1553", "1660", "1661", "1662", "1918", "1919", "2201", "2202", "2203", "2204", "2208", 
                "2209", "2410", "2411", "2503", "2504", "2505", "2506", "2553", "2660", "2661", "2662", "2918", "2919", 
                "3201", "3202", "3211", "3503", "3553", "5201", "5202", "5208", "5209", "5210", "5410", "5411", "5412", "5413", 
                "5503", "5553", "5555", "5556", "5660", "5661", "5662", "5916", "5918", "5919", "6201", "6202","6208", "6209", "6210", 
                "6410", "6411", "6412", "6413", "6503", "6553", "6555", "6556", "6660", "6661", "6662", "6918", "6919", "7201", 
                "7202", "7210", "7211", "7553", "7556",
                "7930" };

            /* Chamado 15376.
             * Caso a nota de devolução estivesse associada a um beneficiamento filho, como 6656A4, o sistema não considerava o CFOP
             * como um CFOP de devolução, pois, neste método estava sendo comparado o código interno do CFOP filho.
             * Sendo assim, devem ser considerados somente os 4 primeiros dígitos do CFOP para garantir que a verificação seja feita corretamente. */
            return cfopDevolucao.Contains(codInterno.Substring(0, 4));
            // return cfopDevolucao.Contains(codInterno);
        }

        #endregion

        #region Verifica se é um cfop valido para NFC-e

        /// <summary>
        /// Verifica se é um cfop valido para NFC-e
        /// </summary>
        /// <param name="codInterno"></param>
        /// <returns></returns>
        public bool IsCfopNFCe(string codInterno)
        {
            if (string.IsNullOrEmpty(codInterno))
                return false;

            List<string> cfop = new List<string>() { "5101", "5102", "5103", "5104", "5115", "5405", "5656", "5667", "5933" };

            return cfop.Contains(codInterno.Substring(0, 4));
        }

        #endregion

        #region Métodos sobrescritos

        public override uint Insert(Cfop objInsert)
        {
            // Se o código do CFOP já estiver cadastrado, não permite cadastrá-lo novamente
            /*if (objPersistence.ExecuteSqlQueryCount("Select count(*) from cfop Where codInterno=?codInterno",
                new GDAParameter("?codInterno", objInsert.CodInterno)) > 0)
                throw new Exception("Este CFOP já foi cadastrado.");

            uint idCfop = base.Insert(objInsert);

            #region Insere a natureza de operação padrão

            var natureza = new NaturezaOperacao()
            {
                IdCfop = (int)idCfop
            };

            NaturezaOperacaoDAO.Instance.Insert(natureza);

            #endregion

            return idCfop;*/

            throw new NotImplementedException();
        }

        public override int Update(Cfop objUpdate)
        {
            // Se o código do CFOP estiver sendo alterado, não permite realizar esta alteração 
            // se este CFOP já estiver sendo usado por alguma nota fiscal
            /*if (ObtemValorCampo<string>("codInterno", "idCfop=" + objUpdate.IdCfop) != objUpdate.CodInterno &&
                objPersistence.ExecuteSqlQueryCount("Select count(*) from nota_fiscal Where idNaturezaOperacao in (" +
                NaturezaOperacaoDAO.Instance.ObtemIdsNaturezaOperacaoPorCfop((uint)objUpdate.IdCfop) + ")") > 0)
                throw new Exception("O código deste CFOP não pode ser alterado por haver notas fiscais relacionadas ao mesmo.");

            LogAlteracaoDAO.Instance.LogCfop(objUpdate);
            return base.Update(objUpdate);*/
            throw new NotImplementedException();
        }

        public override int DeleteByPrimaryKey(uint Key)
        {
            // Verifica se esta CFOP está sendo utilizada em alguma nota
            /*if (objPersistence.ExecuteSqlQueryCount("Select count(*) from nota_fiscal Where idNaturezaOperacao in (" + 
                NaturezaOperacaoDAO.Instance.ObtemIdsNaturezaOperacaoPorCfop(Key) + ")") > 0)
                throw new Exception("Este CFOP não pode ser excluído por haver notas fiscais relacionadas ao mesmo.");

            LogAlteracaoDAO.Instance.ApagaLogCfop(Key);
            return base.DeleteByPrimaryKey(Key);*/
            throw new NotImplementedException();
        }

        public override int Delete(Cfop objDelete)
        {
            return DeleteByPrimaryKey(objDelete.IdCfop);
        }

        #endregion
    }
}