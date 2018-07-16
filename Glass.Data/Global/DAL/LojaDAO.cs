using System;
using System.Collections.Generic;
using Glass.Data.Model;
using GDA;
using System.Linq;

namespace Glass.Data.DAL
{
    public sealed class LojaDAO : BaseCadastroDAO<Loja, LojaDAO>
    {
        //private LojaDAO() { }

        #region Busca padrão

        private string Sql(uint idLoja, int situacao, bool selecionar)
        {
            string campos = selecionar ? "l.*, if(l.idCidade is null, l.cidade, cid.NomeCidade) as nomeCidade, cid.NomeUf as uf, " + 
                "cid.codIbgeUf, cid.codIbgeCidade" : "Count(*)";

            string sql = "Select " + campos + @" From loja l 
                Left Join cidade cid On (cid.idCidade=l.idCidade) Where 1";

            if (idLoja > 0)
                sql += " And idLoja=" + idLoja;

            if (situacao > 0)
                sql += " And situacao=" + situacao;

            return sql;
        }

        public Loja GetElement(uint idLoja)
        {
            return GetElement(null, idLoja);
        }

        public Loja GetElement(GDASession sessao, uint idLoja)
        {
            return objPersistence.LoadOneData(sessao, Sql(idLoja, 0, true));
        }

        public IList<Loja> GetList(string sortExpression, int startRow, int pageSize)
        {
            return LoadDataWithSortExpression(Sql(0, 0, true), sortExpression, startRow, pageSize, null);
        }

        public int GetCount()
        {
            return objPersistence.ExecuteSqlQueryCount(Sql(0, 0, false), null);
        }

        public Loja GetElementSituacao(uint idLoja, int situacao)
        {
            return objPersistence.LoadOneData(Sql(idLoja, situacao, true));
        }

        public IList<Loja> GetListSituacao(int situacao)
        {
            return GetListSituacao(null, situacao);
        }

        public IList<Loja> GetListSituacao(GDASession session, int situacao)
        {
            return objPersistence.LoadData(session, Sql(0, situacao, true)).ToList();
        }

        public IList<Loja> GetListSituacao(int situacao, string sortExpression, int startRow, int pageSize)
        {
            return LoadDataWithSortExpression(Sql(0, situacao, true), sortExpression, startRow, pageSize, null);
        }

        public int GetCountSituacao(int situacao)
        {
            return objPersistence.ExecuteSqlQueryCount(Sql(0, situacao, false), null);
        }

        #endregion

        #region Busca lojas para EFD

        /// <summary>
        /// Busca lojas para montar arquivo EFD
        /// </summary>
        /// <param name="idLoja"></param>
        /// <param name="dataInicio"></param>
        /// <param name="dataFim"></param>
        /// <returns></returns>
        public Loja GetForEFD(uint idLoja)
        {
            return objPersistence.LoadOneData(@"
                Select l.*, cid.codIbgeUf, cid.codIbgeCidade, cid.nomeUf as UF From loja l
                    Left Join cidade cid On (cid.idCidade=l.idCidade) Where idLoja=" + idLoja);
        }

        /// <summary>
        /// Busca lojas para montar arquivo EFD
        /// </summary>
        /// <param name="idLoja"></param>
        /// <param name="dataInicio"></param>
        /// <param name="dataFim"></param>
        /// <returns></returns>
        public IList<Loja> GetForEFD(IEnumerable<Sync.Fiscal.EFD.Entidade.INFe> nf, IEnumerable<Sync.Fiscal.EFD.Entidade.IParticipanteCTe> partCte)
        {
            var lojas = new List<int>();

            lojas.AddRange(nf.Where(x => x.CodigoLoja > 0).Select(x => x.CodigoLoja.Value));
            lojas.AddRange(partCte.Where(x => x.CodigoLoja > 0).Select(x => x.CodigoLoja.Value));
            lojas = lojas.Distinct().ToList();

            string idsLoja = String.Join(",", lojas.Select(x => x.ToString()).ToArray());

            return String.IsNullOrEmpty(idsLoja) ? new List<Loja>() : objPersistence.LoadData(@"
                Select l.*, cid.codIbgeUf, cid.codIbgeCidade, cid.nomeUf as UF From loja l
                    Left Join cidade cid On (cid.idCidade=l.idCidade) 
                Where idLoja in (" + idsLoja.Trim(',') + ")").ToList();
        }

        #endregion

        /// <summary>
        /// Busca as lojas de vários ids.
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public IList<Loja> GetByString(string ids)
        {
            if (string.IsNullOrEmpty(ids))
                ids = "0";

            string sql = "select * from loja where idLoja in (" + ids.Trim(',') + ")";
            return objPersistence.LoadData(sql).ToList();
        }

        /// <summary>
        /// Busca o tipo da loja.
        /// </summary>
        /// <param name="idLoja"></param>
        /// <returns></returns>
        public int? BuscaTipoLoja(uint idLoja)
        {
            string sql = "select tipo from loja where idLoja=" + idLoja;
            object retorno = objPersistence.ExecuteScalar(sql);
            return retorno != null && retorno != DBNull.Value && retorno.ToString() != "" ? (int?)Glass.Conversoes.StrParaInt(retorno.ToString()) : null;
        }

        /// <summary>
        /// Recupera o nome da loja.
        /// </summary>
        /// <param name="idLoja"></param>
        /// <returns></returns>
        public string GetNome(uint idLoja)
        {
            return GetNome(null, idLoja);
        }

        /// <summary>
        /// Recupera o nome da loja.
        /// </summary>
        /// <param name="idLoja"></param>
        /// <returns></returns>
        public string GetNome(GDASession sessao, uint idLoja)
        {
            string sql = "select coalesce(nomeFantasia, razaoSocial) from loja where idLoja=" + idLoja;
            object retorno = objPersistence.ExecuteScalar(sessao, sql);
            return retorno != null && retorno != DBNull.Value ? retorno.ToString() : null;
        }

        public bool GetIgnorarBloquearItensCorEspessura(GDASession sessao, uint idLoja)
        {
            string sql = "select IgnorarBloquearItensCorEspessura from loja where idLoja=" + idLoja;
            object retorno = objPersistence.ExecuteScalar(sessao, sql);
            return retorno != null && retorno != DBNull.Value ? (retorno.ToString().StrParaInt() > 0) : false;
        }

        public bool GetIgnorarLiberarProdutosProntos(GDASession sessao, uint idLoja)
        {
            string sql = "select IgnorarLiberarApenasProdutosProntos from loja where idLoja=" + idLoja;
            object retorno = objPersistence.ExecuteScalar(sessao, sql);
            return retorno != null && retorno != DBNull.Value ? (retorno.ToString().StrParaInt() > 0) : false;
        }

        /// <summary>
        /// Recupera a razão social da loja.
        /// </summary>
        public string ObterRazaoSocial(GDASession sessao, int idLoja)
        {
            var sql = string.Format("SELECT COALESCE(RazaoSocial, NomeFantasia) FROM loja WHERE IdLoja={0}", idLoja);
            object retorno = objPersistence.ExecuteScalar(sessao, sql);
            return retorno != null && retorno != DBNull.Value ? retorno.ToString() : null;
        }

        /// <summary>
        /// Recupera o telefone da loja.
        /// </summary>
        public string ObtemTelefone(GDASession session, uint idLoja)
        {
            var sql = "SELECT l.Telefone FROM loja l WHERE l.IdLoja=" + idLoja;
            object retorno = objPersistence.ExecuteScalar(session, sql);
            return retorno != null && retorno != DBNull.Value ? retorno.ToString() : null;
        }

        /// <summary>
        /// Recupera o endereço completo da loja.
        /// </summary>
        /// <param name="idLoja"></param>
        /// <returns></returns>
        public string ObtemEnderecoCompleto(uint idLoja)
        {
            var where = "idLoja=" + idLoja;
            var idCidade = ObtemValorCampo<uint>("idCidade", where);
            var endereco = ObtemValorCampo<string>("endereco", where);
            var numero = ObtemValorCampo<string>("numero", where);
            var compl = ObtemValorCampo<string>("compl", where);
            var bairro = ObtemValorCampo<string>("bairro", where);
            var cidade = CidadeDAO.Instance.GetNome(idCidade);
            var uf = CidadeDAO.Instance.ObtemValorCampo<string>("nomeUf", "idCidade=" + idCidade);
            
            compl = String.IsNullOrEmpty(compl) ? " " : " (" + compl + ") ";

            return endereco + (!String.IsNullOrEmpty(numero) ? ", " + numero : String.Empty) + compl + bairro + " - " + cidade + "/" + uf;
        }

        public string ObtemInscEst(uint idLoja)
        {
            return ObtemValorCampo<string>("InscEst", "IdLoja=" + idLoja);
        }

        public string ObtemCnpj(uint idLoja)
        {
            return ObtemCnpj(null, idLoja);
        }

        public string ObtemCnpj(GDASession session, uint idLoja)
        {
            return ObtemValorCampo<string>(session, "cnpj", "idLoja=" + idLoja);
        }

        public List<string> ObtemCnpj()
        {
            return ExecuteMultipleScalar<string>("SELECT cnpj FROM loja");
        }

        public uint ObtemIdCidade(uint idLoja)
        {
            return ObtemIdCidade(null, idLoja);
        }

        public uint ObtemIdCidade(GDASession sessao, uint idLoja)
        {
            return ObtemValorCampo<uint>(sessao, "idCidade", "idLoja=" + idLoja);
        }

        public bool ObtemCalculaIcmsStPedido(GDASession sessao, uint idLoja)
        {
            return ObtemValorCampo<bool>(sessao, "CalcularIcmsPedido", $"IdLoja={ idLoja }");
        }

        public bool ObtemCalculaIpiPedido(GDASession sessao, uint idLoja)
        {
            return ObtemValorCampo<bool>(sessao, "CalcularIpiPedido", $"IdLoja={ idLoja }");
        }
        
        public bool ObtemCalculaIcmsStLiberacao(GDASession sessao, uint idLoja)
        {
            return ObtemValorCampo<bool>(sessao, "CalcularIcmsLiberacao", $"IdLoja={ idLoja }");
        }
        
        public bool ObtemCalculaIpiLiberacao(GDASession sessao, uint idLoja)
        {
            return ObtemValorCampo<bool>(sessao, "CalcularIpiLiberacao", $"IdLoja={ idLoja }");
        }
        
        #region Obtém identificação da loja

        /// <summary>
        /// Obtém a identificação da loja pelo CNPJ.
        /// </summary>
        /// <param name="cnpj"></param>
        public uint ObtemIdLojaPeloCnpj(string cnpj)
        {
            return ObtemIdLojaPeloCnpj(null, cnpj);
        }

        /// <summary>
        /// Obtém a identificação da loja pelo CNPJ e pela sessão, informados por parâmetro.
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="cnpj"></param>
        public uint ObtemIdLojaPeloCnpj(GDASession sessao, string cnpj)
        {
            return ObtemValorCampo<uint>(sessao, "idLoja", "cnpj=?cnpj", new GDAParameter("?cnpj", cnpj));
        }

        #endregion

        /// <summary>
        /// Recupera a UF da loja.
        /// </summary>
        /// <param name="idLoja"></param>
        /// <returns></returns>
        public string GetUf(uint idLoja)
        {
            string sql = @"select cid.nomeUf from cidade cid inner join loja l on (cid.idCidade=l.idCidade)
                where l.idLoja=" + idLoja;

            return ExecuteScalar<string>(sql);
        }

        /// <summary>
        /// Recupera a cidade da loja.
        /// </summary>
        /// <param name="idLoja">Identificador da loja</param>
        /// <param name="incluiUF">Se for true, concatena o estado</param>
        /// <returns>string</returns>
        public string GetCidade(uint idLoja, bool incluiUF)
        {
            string campo = !incluiUF ? "cid.nomeCidade" : "concat(cid.nomeCidade, '/', cid.nomeUf)";
            string sql = "select " + campo + @" from cidade cid inner join loja l on (cid.idCidade=l.idCidade)
                where l.idLoja=" + idLoja;

            return ExecuteScalar<string>(sql);
        }

        /// <summary>
        /// Retorna o regime (CRT) da loja emitente na NFe passada
        /// 0-N/D
        /// 1-Simples Nacional
        /// 2-Simples Nacional - excesso de sublimite de receita bruta
        /// 3-Lucro Real
        /// 4-Lucro Presumido
        /// </summary>
        /// <param name="idNf"></param>
        /// <returns></returns>
        public int BuscaCrtLoja(GDASession sessao, uint idLoja)
        {
            string sql = "Select crt From loja where idLoja=" + idLoja;

            object obj = objPersistence.ExecuteScalar(sessao, sql);

            if (obj == null || String.IsNullOrEmpty(obj.ToString()))
                return 0;

            return Glass.Conversoes.StrParaInt(obj.ToString());
        }

        /// <summary>
        /// Atualiza a senha do certificado digital da loja
        /// </summary>
        /// <param name="senhaCert"></param>
        public void AtualizaSenhaCert(uint idLoja, string senhaCert)
        {
            string sql = "Update loja set senhaCert=?senhaCert Where idLoja=" + idLoja;

            objPersistence.ExecuteCommand(sql, new GDAParameter[] { new GDAParameter("?senhaCert", senhaCert) });
        }

        /// <summary>
        /// Retorna a senha cadastrada para uso do certificado digital
        /// </summary>
        /// <param name="idLoja"></param>
        /// <returns></returns>
        public string RecuperaSenhaCert(uint idLoja)
        {
            return ObtemValorCampo<string>("senhaCert", "idLoja=" + idLoja);
        }

        public string GetLojaByCNPJIE(GDASession sessao, string cnpj, string inscEst, bool ativa)
        {
            var param = new GDAParameter("?cnpj", cnpj.Replace("-", "").Replace("/", "").Replace(".", ""));
            var sql = "Select idloja from loja where Replace(Replace(Replace(cnpj, '-',''), '.',''), '/','')=?cnpj";

            if (ativa)
                sql += string.Format(" AND Situacao = {0} ", (int)Situacao.Ativo);

            // Se houver mais de uma loja com o mesmo cnpj, filtra também pela inscrição estadual
            if (ExecuteScalar<bool>(sessao, "Select count(*) > 1 from loja where Replace(Replace(Replace(cnpj, '-',''), '.',''), '/','')=?cnpj", param))
                return ExecuteScalar<string>(sessao, sql + " And Replace(Replace(Replace(inscEst, '-',''), '.',''), '/','')=?inscEst", param, new GDAParameter("?inscEst", inscEst));

            return ExecuteScalar<string>(sessao, sql, param);
        }

        public List<uint> GetIdsLojas()
        {
            return objPersistence.LoadResult("select idLoja from loja").Select(f => f.GetUInt32(0))
                .ToList(); ;
        }

        public List<uint> GetIdsLojasAtivas()
        {
            return GetIdsLojasAtivas(null);
        }
 
        public List<uint> GetIdsLojasAtivas(GDASession session)
        {
            return objPersistence.LoadResult(session, "select idLoja from loja where situacao = " + (int)Glass.Situacao.Ativo).Select(f => f.GetUInt32(0)).ToList();
        }

        public int SalvaDataVencimento(uint idLoja, DateTime data)
        {
            string sql = "Update loja set DATAVENCIMENTOCERTIFICADO=?data Where idLoja=" + idLoja;
            return Convert.ToInt32((objPersistence.ExecuteScalar(sql, new GDAParameter("?data", data))));
        }

        #region Ativa/Inativa

        /// <summary>
        /// Inativa a loja se a mesma estiver ativada e vice-versa.
        /// </summary>
        /// <param name="idLoja"></param>
        public void AtivarInativarLoja(uint idLoja)
        {
            string sql = "Update loja Set situacao=If(situacao=" + (int)Situacao.Inativo +
                ", " + (int)Situacao.Ativo + ", " + (int)Situacao.Inativo + ") Where idLoja=" + idLoja;

            objPersistence.ExecuteCommand(sql);
        }

        #endregion

        #region Métodos sobrescritos

        public override uint Insert(Loja objInsert)
        {
            uint idLoja = base.Insert(objInsert);

            foreach (Produto p in ProdutoDAO.Instance.GetAll())
                ProdutoLojaDAO.Instance.NewProd(p.IdProd, (int)idLoja);

            return idLoja;
        }

        public override int Update(Loja objUpdate)
        {
            Loja lojaOld = GetElementByPrimaryKey((uint)objUpdate.IdLoja);
            objUpdate.EmailComercial = lojaOld.EmailComercial;
            objUpdate.EmailContato = lojaOld.EmailContato;
            objUpdate.EmailFiscal = lojaOld.EmailFiscal;
            objUpdate.LoginEmailComercial = lojaOld.LoginEmailComercial;
            objUpdate.LoginEmailContato = lojaOld.LoginEmailContato;
            objUpdate.LoginEmailFiscal = lojaOld.LoginEmailFiscal;
            objUpdate.SenhaEmailComercial = lojaOld.SenhaEmailComercial;
            objUpdate.SenhaEmailContato = lojaOld.SenhaEmailContato;
            objUpdate.SenhaEmailFiscal = lojaOld.SenhaEmailFiscal;
            objUpdate.ServidorEmailComercial = lojaOld.ServidorEmailComercial;
            objUpdate.ServidorEmailContato = lojaOld.ServidorEmailContato;
            objUpdate.ServidorEmailFiscal = lojaOld.ServidorEmailFiscal;
            objUpdate.SenhaCert = lojaOld.SenhaCert;

            LogAlteracaoDAO.Instance.LogLoja(objUpdate);
            return base.Update(objUpdate);
        }

        public int UpdateDadosEmail(Loja objUpdate)
        {
            LogAlteracaoDAO.Instance.LogLoja(objUpdate);
            return base.Update(objUpdate);
        }

        public override int Delete(Loja objDelete)
        {
            return DeleteByPrimaryKey(objDelete.IdLoja);
        }

        public override int DeleteByPrimaryKey(uint Key)
        {
            // Verifica se existem funcionários associados à esta loja
            if (CurrentPersistenceObject.ExecuteSqlQueryCount("Select Count(*) From funcionario Where idLoja=" + Key) > 0)
                throw new Exception("Esta loja não pode ser excluída, pois existem funcionários associados à mesma.");

            // Verifica se existem pedidos associados à esta loja
            if (CurrentPersistenceObject.ExecuteSqlQueryCount("Select Count(*) From pedido Where idLoja=" + Key) > 0)
                throw new Exception("Esta loja não pode ser excluída, pois existem pedidos associados à mesma.");

            // Verifica se existem orçamentos associados à esta loja
            if (CurrentPersistenceObject.ExecuteSqlQueryCount("Select Count(*) From orcamento Where idLoja=" + Key) > 0)
                throw new Exception("Esta loja não pode ser excluída, pois existem orçamentos associados à mesma.");

            // Verifica se existem projetos associados à esta loja
            if (CurrentPersistenceObject.ExecuteSqlQueryCount("Select Count(*) From projeto Where idLoja=" + Key) > 0)
                throw new Exception("Esta loja não pode ser excluída, pois existem projetos associados à mesma.");

            return GDAOperations.Delete(new Loja { IdLoja = (int)Key });
        }

        #endregion
    }
}
