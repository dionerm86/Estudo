﻿using GDA;
using Glass.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Data.DAL
{
    public class CTeCidadeDescargaMDFeDAO : BaseDAO<CTeCidadeDescargaMDFe, CTeCidadeDescargaMDFeDAO>
    {
        #region Busca Padrão

        private string Sql(int idCidadeDescarga, bool selecionar)
        {
            var campos = selecionar ? @" ccd.*, ct.NumeroCTe, ct.Modelo, ct.TipoDocumentoCte,
                    IF(ct.TipoDocumentoCte = 3, (COALESCE(c.Nome, c.NomeFantasia)), l.RazaoSocial) AS NomeEmitente, ct.DataEmissao" : " COUNT(*)";

            var sql = @"SELECT" + campos +
                @" FROM cte_cidade_descarga_mdfe ccd
                LEFT JOIN conhecimento_transporte ct ON (ccd.IdCTe=ct.IdCTe)
                LEFT JOIN participante_cte pct ON (ct.IdCTe = pct.IdCTe AND pct.IdLoja IS NOT NULL AND pct.IdLoja > 0)
                LEFT JOIN loja l ON (l.IdLoja=pct.IdLoja)
                LEFT JOIN cliente c ON (c.Id_cli=pct.IdCliente)
                WHERE ccd.IdCidadeDescarga={0}
                GROUP BY ct.CHAVEACESSO";

            return string.Format(sql, idCidadeDescarga);
        }

        public IList<CTeCidadeDescargaMDFe> GetList(int idCidadeDescarga, string sortExpression, int startRow, int pageSize)
        {
            if (GetCountReal(idCidadeDescarga) == 0)
                return new CTeCidadeDescargaMDFe[] { new CTeCidadeDescargaMDFe() };

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

        public List<CTeCidadeDescargaMDFe> ObterCTesCidadeDescargaMDFe(int idCidadeDescarga)
        {
            var sql = Sql(idCidadeDescarga, true);

            return objPersistence.LoadData(sql).ToList();
        }

        /// <summary>
        /// Verifica se o CTe já foi inserido no Mdfe.
        /// </summary>
        /// <param name="chaveAcesso">Chave de acesso do CTe a ser verificado.</param>
        /// <returns>Um boolean referente à situação do CTe (se o mesmo já está associado ou não a outro CTe).</returns>
        public bool VerificarCteJaIncluso(string chaveAcesso)
        {
            return this.objPersistence.ExecuteSqlQueryCount(
                $@"SELECT ccdm.IdCidadeDescarga FROM cidade_descarga_mdfe cdm
	                INNER JOIN cte_cidade_descarga_mdfe ccdm ON (ccdm.IdCidadeDescarga = cdm.IdCidadeDescarga)
	                INNER JOIN manifesto_eletronico me ON (me.IdManifestoEletronico = cdm.IdManifestoEletronico)
                   WHERE ccdm.ChaveAcesso={ chaveAcesso } AND me.Situacao <> { (int)SituacaoEnum.Cancelado }") > 0;
        }

        /// <summary>
        /// Obtém o número do MDFe associado ao CTe informado, através da chave de acesso.
        /// </summary>
        /// <param name="session">Sessão do GDA.</param>
        /// <param name="chaveAcesso">chaveAcesso.</param>
        /// <returns>Retorna o número do MDFe associado ao CTe informado, através da chave de acesso.</returns>
        public int? ObterNumeroMdfeAssociadoCte(GDASession session, string chaveAcesso)
        {
            var sqlObterNumeroMdfe = $@"SELECT me.NumeroManifestoEletronico FROM cidade_descarga_mdfe cdm
	                INNER JOIN cte_cidade_descarga_mdfe ccdm ON (ccdm.IdCidadeDescarga = cdm.IdCidadeDescarga)
	                INNER JOIN manifesto_eletronico me ON (me.IdManifestoEletronico = cdm.IdManifestoEletronico)
                WHERE ccdm.ChaveAcesso = ?chaveAcesso AND
                    me.Situacao <> {(int)SituacaoEnum.Cancelado}";

            return this.ExecuteScalar<int?>(session, sqlObterNumeroMdfe, new GDAParameter("?chaveAcesso", chaveAcesso));
        }

        #region Metodos Sobrescritos

        public override uint Insert(GDASession session, CTeCidadeDescargaMDFe objInsert)
        {
            // Verifica se o CTe tem chave de acesso válida
            if (string.IsNullOrWhiteSpace(objInsert.ChaveAcesso) || objInsert.ChaveAcesso.Length != 44)
                throw new Exception("O CTe deve ter chave de acesso válida para ser adicionada ao MDFe");

            return base.Insert(session, objInsert);
        }

        /// <summary>
        /// Deleta todos os CTes associadas a Cidade Descarga
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idCidadeDescarga"></param>
        public void DeletarPorIdCidadeDescarga(GDASession sessao, int idCidadeDescarga)
        {
            objPersistence.ExecuteCommand(sessao, "DELETE FROM cte_cidade_descarga_mdfe WHERE IdCidadeDescarga=" + idCidadeDescarga, null);
        }

        #endregion
    }
}
