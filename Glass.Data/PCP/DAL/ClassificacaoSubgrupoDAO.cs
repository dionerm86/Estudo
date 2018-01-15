using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Glass.Data.Model;
using GDA;

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
        public bool VerificarAssociacaoExistente(int idSubgrupo, int IdProcesso)
        {
            if (ExecuteScalar<int>("SELECT COUNT(*) FROM classificacao_subgrupo") == 0)
                return true;

            var sql = @"SELECT COUNT(*) 
                        FROM classificacao_subgrupo 
                        WHERE IdSubgrupoProd = " + idSubgrupo + 
                        @" AND IdClassificacaoRoteiroProducao IN 
                            (Select IdClassificacaoRoteiroProducao FROM roteiro_producao WHERE IdProcesso = " + IdProcesso + ")";

            return ExecuteScalar<int>(sql) > 0;
        }
    }
}
