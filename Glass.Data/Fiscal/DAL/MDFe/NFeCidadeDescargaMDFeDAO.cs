using Glass.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GDA;

namespace Glass.Data.DAL
{
    public class NFeCidadeDescargaMDFeDAO : BaseDAO<NFeCidadeDescargaMDFe, NFeCidadeDescargaMDFeDAO>
    {
        #region Busca Padrão

        private string Sql(int idCidadeDescarga, bool selecionar)
        {
            var campos = selecionar ? @" ncd.*, nf.NumeroNFe, nf.Modelo, nf.TipoDocumento,
                    IF(nf.TipoDocumento = 3, (COALESCE(f.RazaoSocial, f.NomeFantasia)), l.RazaoSocial) AS NomeEmitente, nf.DataEmissao" : " COUNT(*)";

            var sql = @"SELECT" + campos +
                @" FROM nfe_cidade_descarga_mdfe ncd
                LEFT JOIN nota_fiscal nf ON (ncd.IdNFe=nf.IdNF)
                LEFT JOIN loja l ON (l.IdLoja=nf.IdLoja)
                LEFT JOIN fornecedor f ON (f.IdFornec=nf.IdFornec)
                WHERE ncd.IdCidadeDescarga={0}";

            return string.Format(sql, idCidadeDescarga);
        }

        public IList<NFeCidadeDescargaMDFe> GetList(int idCidadeDescarga, string sortExpression, int startRow, int pageSize)
        {
            if (GetCountReal(idCidadeDescarga) == 0)
                return new NFeCidadeDescargaMDFe[] { new NFeCidadeDescargaMDFe() };

            var sql = Sql(idCidadeDescarga, true);

            return LoadDataWithSortExpression(sql, sortExpression, startRow, pageSize, null);
        }

        public int GetCountReal(int idCidadeDescarga)
        {
            return objPersistence.ExecuteSqlQueryCount(Sql(idCidadeDescarga, false));
        }

        public int GetCount(int idCidadeDescarga)
        {
            int retorno = GetCountReal(idCidadeDescarga);
            return retorno > 0 ? retorno : 1;
        }

        #endregion

        public List<NFeCidadeDescargaMDFe> ObterNFesCidadeDescargaMDFe(int idCidadeDescarga)
        {
            var sql = Sql(idCidadeDescarga, true);

            return objPersistence.LoadData(sql).ToList();
        }

        /// <summary>
        /// Obtém o número do MDFe associado à NFe informada, através da chave de acesso.
        /// </summary>
        /// <param name="chaveAcesso">chaveAcesso.</param>
        /// <returns>Retorna o número do MDFe associado à NFe informada, através da chave de acesso.</returns>
        public string ObterNumeroMdfeAssociadoNfe(GDASession session, string chaveAcesso)
        {
            var sqlObterNumeroMdfe = $@"SELECT GROUP_CONCAT(me.NumeroManifestoEletronico) FROM cidade_descarga_mdfe cdm
	                INNER JOIN nfe_cidade_descarga_mdfe ncdm ON (cdm.IdCidadeDescarga = ncdm.IdCidadeDescarga)
	                INNER JOIN manifesto_eletronico me ON (cdm.IdManifestoEletronico = me.IdManifestoEletronico)
                WHERE ncdm.ChaveAcesso = ?chaveAcesso AND
                    me.Situacao NOT IN ({(int)SituacaoEnum.Cancelado},{(int)SituacaoEnum.Encerrado})";

            return this.ExecuteScalar<string>(session, sqlObterNumeroMdfe, new GDAParameter("?chaveAcesso", chaveAcesso));
        }

        #region Metodos Sobrescritos

        public override uint Insert(GDASession session, NFeCidadeDescargaMDFe objInsert)
        {
            // Verifica se tem chave de acesso válida
            if (string.IsNullOrWhiteSpace(objInsert.ChaveAcesso) || objInsert.ChaveAcesso.Length != 44)
                throw new Exception("A NFe deve ter chave de acesso válida para ser adicionada ao MDFe");

            return base.Insert(session, objInsert);
        }

        /// <summary>
        /// Deleta todas as NFes associadas a Cidade Descarga
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idCidadeDescarga"></param>
        public void DeletarPorIdCidadeDescarga(GDASession sessao, int idCidadeDescarga)
        {
            objPersistence.ExecuteCommand(sessao, "DELETE FROM nfe_cidade_descarga_mdfe WHERE IdCidadeDescarga=" + idCidadeDescarga, null);
        }

        #endregion
    }
}
