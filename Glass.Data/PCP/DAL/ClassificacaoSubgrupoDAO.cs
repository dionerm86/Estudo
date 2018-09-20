using GDA;
using Glass.Data.Model;
using System;
using System.Collections.Generic;

namespace Glass.Data.DAL
{
    public sealed class ClassificacaoSubgrupoDAO : BaseDAO<ClassificacaoSubgrupo, ClassificacaoSubgrupoDAO>
    {
        /// <summary>
        /// Retorna todos os subgrupos associados à classificação
        /// </summary>
        public List<ClassificacaoSubgrupo> ObterSubgruposAssociadosClassificacaoRoteiroProducao(int idClassificacaoRoteiroProducao)
        {
            var sql = @"Select cs.*, s.Descricao
                        FROM classificacao_subgrupo cs
                        LEFT JOIN subgrupo_prod s ON (cs.IdSubgrupoProd=s.IdSubgrupoProd)
                        WHERE cs.IdClassificacaoRoteiroProducao=" + idClassificacaoRoteiroProducao;

            var retorno = objPersistence.LoadData(sql);

            return retorno;
        }

        /// <summary>
        /// Retorna o subgrupo associado à classificação de roteiro
        /// </summary>
        public ClassificacaoSubgrupo ObterSubgrupoAssociadoClassificacao(int chave)
        {
            var idClassificacao = chave.ToString().Remove(chave.ToString().Length - 3).StrParaInt();
            var idSubgrupo = chave.ToString().Substring(chave.ToString().Length - 3).StrParaInt();

            var sql = @"Select cs.*, s.Descricao
                        FROM classificacao_subgrupo cs
                        LEFT JOIN subgrupo_prod s ON (cs.IdSubgrupoProd=s.IdSubgrupoProd)
                        WHERE cs.IdClassificacaoRoteiroProducao = " + idClassificacao + " AND cs.IdsubgrupoProd = " + idSubgrupo;

            var retorno = objPersistence.LoadOneData(sql);

            return retorno;
        }

        /// <summary>
        /// Cria uma nova associação de de classificação e subgrupo
        /// </summary>
        public void AssociarSubgrupo(int idSubgrupo, int idClassificacao)
        {
            var novaAssociacao = new ClassificacaoSubgrupo()
            {
                IdClassificacaoRoteiroProducao = idClassificacao,
                IdSubgrupoProd = idSubgrupo
            };

            if (ObterSubgrupoAssociadoClassificacao(novaAssociacao.ChaveComposta) != null)
                throw new Exception("Já foi feita a associação com este subgrupo.");

            var sql = @"SELECT IdClassificacaoRoteiroProducao
                        FROM classificacao_subgrupo
                        WHERE IdSubgrupoProd = " + idSubgrupo;

            var idClassificacaoExistente = ExecuteScalar<int>(sql);

            if (idClassificacaoExistente > 0)
                throw new Exception(string.Format("Este subgrupo está associado à classificação {0}.", idClassificacaoExistente));

            Instance.Insert(novaAssociacao);
        }



        /// <summary>
        /// Cria uma nova associação de de classificação e subgrupo
        /// </summary>
        public void DesassociarSubgrupo(int chaveComposta)
        {
            var associacao = ObterSubgrupoAssociadoClassificacao(chaveComposta);
            Instance.Delete(associacao);
        }

        /// <summary>
        /// Verifica se existe associação para o subgrupo passado e o processo
        /// </summary>
        public bool VerificarAssociacaoExistente(int idSubgrupo, int idProcesso)
        {
            return this.VerificarAssociacaoExistente(null, idProcesso, new[] { idSubgrupo });
        }

        /// <summary>
        /// Verifica se existe associação para os subgrupos passados e o processo.
        /// </summary>
        /// <param name="sessao">A transação atual com o banco de dados.</param>
        /// <param name="idProcesso">O identificador do processo.</param>
        /// <param name="idsSubgrupos">Os identificadores dos subgrupos que serão validados.</param>
        /// <returns>Verdadeiro, se o </returns>
        public bool VerificarAssociacaoExistente(GDASession sessao, int idProcesso, IEnumerable<int> idsSubgrupos)
        {
            if (this.GetAllForListCount() == 0)
            {
                return true;
            }

            var idsSubgruposString = idsSubgrupos != null
                ? string.Join(",", idsSubgrupos)
                : string.Empty;

            if (string.IsNullOrWhiteSpace(idsSubgruposString))
            {
                return false;
            }

            var sql = $@"
                SELECT COUNT(*)
                FROM classificacao_subgrupo
                WHERE IdSubgrupoProd in ({idsSubgruposString})
                  AND IdClassificacaoRoteiroProducao IN (
                    SELECT IdClassificacaoRoteiroProducao
                    FROM roteiro_producao
                    WHERE IdProcesso = {idProcesso}
                )";

            return this.ExecuteScalar<int>(sessao, sql) > 0;
        }
    }
}
