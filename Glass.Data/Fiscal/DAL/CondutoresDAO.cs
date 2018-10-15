using GDA;
using Glass.Data.DAL;
using Glass.Data.Model;
using System.Collections.Generic;

namespace Glass.Data.DAL
{
    public sealed class CondutoresDAO : BaseDAO<Condutores, CondutoresDAO>
    {
        /// <summary>
        /// Recupera a listagem de condutores.
        /// </summary>
        /// <returns>Lista de condutores.</returns>
        public IList<Condutores> GetList()
        {
            return objPersistence.LoadData("select * from condutores").ToList();
        }

        /// <summary>
        /// Verificar se o condutor pode ser inserido.
        /// </summary>
        /// <param name="session">Sessão.</param>
        /// <param name="cpf">Cpf do condutor.</param>
        private void VerificarPodeInserirCondutor(GDASession session, string cpf)
        {
            if (objPersistence.ExecuteSqlQueryCount(session, $"SELECT * FROM condutores WHERE CPF=?cpf", new GDAParameter("?cpf", cpf)) > 0)
            {
                throw new System.Exception("Já existe um condutor cadastrado com o mesmo Cpf");
            }
        }

        /// <summary>
        /// Insere um novo condutor.
        /// </summary>
        /// <param name="session">Sessão.</param>
        /// <param name="objInsert">Novo condutor.</param>
        /// <returns>Identificador do condutor inserido.</returns>
        public override uint Insert(GDASession session, Condutores objInsert)
        {
            VerificarPodeInserirCondutor(session, objInsert.Cpf);

            return base.Insert(session, objInsert);
        }

        /// <summary>
        /// Verifica se o condutor poderá ser alterados com os dados selecionados.
        /// </summary>
        /// <param name="session">Sessão.</param>
        /// <param name="condutor">Condutor cujos dados serão verificados.</param>
        private void VerificarPodeAtualizarCondutor(GDASession session, Condutores condutor)
        {
            if (objPersistence.ExecuteSqlQueryCount(
                session,
                $"SELECT * FROM condutores WHERE IDCONDUTOR!=?idCondutor AND CPF=?cpf",
                new GDAParameter("?idCondutor", condutor.IdCondutor),
                new GDAParameter("?cpf", condutor.Cpf)) > 0)
            {
                throw new System.Exception("Já existe um condutor cadastrado com o mesmo Cpf");
            }
        }

        /// <summary>
        /// Método para alteração de um condutor.
        /// </summary>
        /// <param name="session">Sessão.</param>
        /// <param name="objUpdate">Condutor que está sendo alterado.</param>
        /// <returns>>Retorna o identificador do condutor inserido.</returns>
        public override int Update(GDASession session, Condutores objUpdate)
        {
            VerificarPodeAtualizarCondutor(session, objUpdate);

            return base.Update(session, objUpdate);
        }

        /// <summary>
        /// Verifica se o condutor pode ser excluído.
        /// </summary>
        /// <param name="sessao">Sessão.</param>
        /// <param name="idCondutor">identificador do condutor validado.</param>
        private void VerificarPodeExcluirCondutor(GDASession sessao, int idCondutor)
        {
            if (objPersistence.ExecuteSqlQueryCount(
                sessao,
                $"SELECT * FROM condutor_veiculo_mdfe WHERE IDCONDUTOR=?idCondutor",
                new GDAParameter("?idCondutor", idCondutor)) > 0)
            {
                throw new System.Exception("O Condutor está associado a um MDFe.");
            }
        }

        /// <summary>
        /// Deleta os dados do condutor.
        /// </summary>
        /// <param name="sessao">Sessão.</param>
        /// <param name="key">Identificador do condutor.</param>
        /// <returns>Retorna o identificador do condutor excluído.</returns>
        public override int DeleteByPrimaryKey(GDASession sessao, int key)
        {
            VerificarPodeExcluirCondutor(sessao, key);

            return base.DeleteByPrimaryKey(sessao, key);
        }
    }
}
