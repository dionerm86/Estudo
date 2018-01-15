using System;
using System.Collections.Generic;
using Glass.Data.Model;
using Glass.Data.Helper;
using GDA;

namespace Glass.Data.DAL
{
    public sealed class BemAtivoImobilizadoDAO : BaseDAO<BemAtivoImobilizado, BemAtivoImobilizadoDAO>
    {
        //private BemAtivoImobilizadoDAO() { }

        #region Busca padrão

        private string Sql(int tipo, bool forEfd, bool selecionar)
        {
            bool isForEfd = forEfd && selecionar;
            string campos = selecionar ? @"bai.*, p.codInterno as codInternoProd, p.descricao as descrProd, 
                pcc.descricao as descrPlanoContaContabil, coalesce(l.nomeFantasia, l.razaoSocial) as nomeLoja,
                pcc.codInterno as codInternoContaContabil" : "count(*)";

            string sql = "select " + campos + @"
                from bem_ativo_imobilizado bai
                    left join produto p on (bai.idProd=p.idProd)
                    left join plano_conta_contabil pcc on (bai.idContaContabil=pcc.idContaContabil)
                    left join loja l on (bai.idLoja=l.idLoja)
                    " + (isForEfd ? @"
                        left join movimentacao_bem_ativo_imob mbai on (bai.idBemAtivoImobilizado=mbai.idBemAtivoImobilizado)
                        left join produtos_nf pnf on (mbai.idProdNf=pnf.idProdNf)
                        left join nota_fiscal nf on (pnf.idNf=nf.idNf)" : String.Empty) + @"
                where 1";

            if (tipo > 0)
                sql += " and bai.tipo=" + tipo;

            if (isForEfd)
                sql += " group by bai.idBemAtivoImobilizado";

            return sql;
        }

        public IList<BemAtivoImobilizado> GetList(int tipo, string sortExpression, int startRow, int pageSize)
        {
            if (GetCountReal(tipo) == 0)
                return new BemAtivoImobilizado[] { new BemAtivoImobilizado() };

            return GetListReal(tipo, sortExpression, startRow, pageSize);
        }

        public int GetCount(int tipo)
        {
            int retorno = GetCountReal(tipo);
            return retorno > 0 ? retorno : 1;
        }

        public IList<BemAtivoImobilizado> GetListReal(int tipo, string sortExpression, int startRow, int pageSize)
        {
            return LoadDataWithSortExpression(Sql(tipo, false, true), sortExpression, startRow, pageSize);
        }

        public int GetCountReal(int tipo)
        {
            return objPersistence.ExecuteSqlQueryCount(Sql(tipo, false, false));
        }

        #endregion

        #region Busca pelo tipo

        /// <summary>
        /// Busca os bens/componentes de acordo com o tipo.
        /// </summary>
        /// <param name="tipo"></param>
        /// <returns></returns>
        public IList<BemAtivoImobilizado> GetByTipo(int tipo)
        {
            return objPersistence.LoadData(Sql(tipo, false, true)).ToList();
        }

        #endregion

        #region Validação do cadastro

        /// <summary>
        /// Validação do cadastro.
        /// </summary>
        /// <param name="idProd"></param>
        /// <param name="idBemAtivoImobilizadoPrinc"></param>
        /// <param name="inserindo"></param>
        /// <param name="mensagemErro"></param>
        /// <returns></returns>
        public bool Validar(uint idLoja, uint idProd, uint idContaContabil, 
            uint? idBemAtivoImobilizadoPrinc, uint? idBemAtivoImobilizadoIgnorar, out string mensagemErro)
        {
            bool retorno = true;
            mensagemErro = String.Empty;

            // Valida os parâmetros
            if (idLoja == 0)
            {
                retorno = false;
                mensagemErro += "Loja inválida. ";
            }
            
            if (idProd == 0)
            {
                retorno = false;
                mensagemErro += "Produto inválido. ";
            }

            if (idContaContabil == 0)
            {
                retorno = false;
                mensagemErro += "Plano de Conta Contábil inválido. ";
            }

            // Valida o produto
            if (objPersistence.ExecuteSqlQueryCount("select count(*) from bem_ativo_imobilizado where idLoja=" + idLoja + " and idProd=" + idProd +
                (idBemAtivoImobilizadoIgnorar > 0 ? " and idBemAtivoImobilizado<>" + idBemAtivoImobilizadoIgnorar : String.Empty)) > 0)
            {
                retorno = false;
                mensagemErro += "Esse produto já foi atribuído a um bem. ";
            }

            // Valida o bem principal
            if (idBemAtivoImobilizadoPrinc > 0 && objPersistence.ExecuteSqlQueryCount(@"select count(*) from bem_ativo_imobilizado 
                where tipo=1 and idLoja=" + idLoja + " and idBemAtivoImobilizado=" + idBemAtivoImobilizadoPrinc.Value) == 0)
            {
                retorno = false;
                mensagemErro += "O bem principal não existe (ou não é um bem). ";
            }

            mensagemErro = mensagemErro.Trim();
            return retorno;
        }

        #endregion

        #region Busca para EFD

        public IList<BemAtivoImobilizado> GetForEFD(string idsLojas, DateTime inicio, DateTime fim)
        {
            string sql = Sql(0, true, true);

            if (!String.IsNullOrEmpty(idsLojas) && idsLojas != "0")
                sql += " and bai.idLoja in (" + idsLojas + ")";

            string campoData = "coalesce(if(nf.tipoDocumento<>" + (int)NotaFiscal.TipoDoc.Saída + ", nf.dataSaidaEnt, null), nf.dataEmissao)";
            sql += " and " + campoData + ">=?dataIni and " + campoData + "<=?dataFim";

            return objPersistence.LoadData(sql, new GDAParameter("?dataIni", inicio), new GDAParameter("?dataFim", fim)).ToList();
        }

        #endregion

        #region Métodos sobrescritos

        public override uint Insert(BemAtivoImobilizado objInsert)
        {
            objInsert.DataCad = DateTime.Now;
            objInsert.UsuCad = UserInfo.GetUserInfo.CodUser;

            BemAtivoImobilizado vazio = new BemAtivoImobilizado();
            vazio.IdBemAtivoImobilizado = base.Insert(objInsert);
            LogAlteracaoDAO.Instance.LogBemAtivoImobilizado(vazio, LogAlteracaoDAO.SequenciaObjeto.Atual);

            return vazio.IdBemAtivoImobilizado;
        }

        public override int Update(BemAtivoImobilizado objUpdate)
        {
            LogAlteracaoDAO.Instance.LogBemAtivoImobilizado(objUpdate, LogAlteracaoDAO.SequenciaObjeto.Novo);
            return base.Update(objUpdate);
        }

        public override int Delete(BemAtivoImobilizado objDelete)
        {
            return DeleteByPrimaryKey(objDelete.IdBemAtivoImobilizado);
        }

        public override int DeleteByPrimaryKey(uint Key)
        {
            if (CurrentPersistenceObject.ExecuteSqlQueryCount(@"select count(*) from bem_ativo_imobilizado") > 0)
                throw new Exception("Não é possível apagar esse bem ativo imobilizado porque ele está em uso.");

            return GDAOperations.Delete(new BemAtivoImobilizado { IdBemAtivoImobilizado = Key });
        }

        #endregion
    }
}
