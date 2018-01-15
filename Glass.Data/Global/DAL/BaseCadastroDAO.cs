using System;
using Glass.Data.Model;
using Glass.Data.Helper;

namespace Glass.Data.DAL
{
    public abstract class BaseCadastroDAO<Model, DAO> : BaseDAO<Model, DAO>
        where Model : ModelBaseCadastro, new()
        where DAO : BaseCadastroDAO<Model, DAO>, new()
    {
        /// <summary>
        /// APAGAR:
        /// Insere os dados no BD.
        /// </summary>
        /// <param name="objInsert">Objeto contendo os dados a serem inseridos.</param>
        /// <returns>Identidade gerada.</returns>
        public override uint Insert(Model objInsert)
        {
            uint retorno = 0;
            int cont = 0;

            // Tratamento feito para tentar resolver o problema misterioso que acontece no MySql ao inserir registros.
            while (true)
            {
                try
                {
                    retorno = Insert(null, objInsert);
                    break;
                }
                catch
                {
                    if (cont++ >= 2)
                        throw;
                    else
                        Thread.Sleep(300);
                }
            }

            return retorno;
        }

        /// <summary>
        /// Insere os dados no BD.
        /// </summary>
        /// <param name="objInsert">Objeto contendo os dados a serem inseridos.</param>
        /// <returns>Identidade gerada.</returns>
        public override uint Insert(GDA.GDASession sessao, Model objInsert)
        {
            if (UserInfo.GetUserInfo != null)
                objInsert.Usucad = UserInfo.GetUserInfo.CodUser;

            objInsert.DataCad = DateTime.Now;

            return base.Insert(sessao, objInsert);
        }

        /// <summary>
        /// Insere os dados no BD.
        /// </summary>
        /// <param name="objInsert">Objeto contendo os dados a serem inseridos.</param>
        /// <returns>Identidade gerada.</returns>
        public uint InsertBase(GDA.GDASession session, Model objInsert)
        {
            return base.Insert(session, objInsert);
        }
    }
}
