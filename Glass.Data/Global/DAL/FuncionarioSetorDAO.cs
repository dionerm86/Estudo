using System;
using System.Collections.Generic;
using Glass.Data.Model;
using GDA;
using Glass.Data.Helper;

namespace Glass.Data.DAL
{
    public sealed class FuncionarioSetorDAO : BaseDAO<FuncionarioSetor, FuncionarioSetorDAO>
    {
        //private FuncionarioSetorDAO() { }

        /// <summary>
        /// Retorna os setores que o funcionário informado possui acesso.
        /// </summary>
        /// <param name="idFunc"></param>
        /// <returns></returns>
        public IList<FuncionarioSetor> GetSetores(GDASession sessao, uint idFunc)
        {
            string sql = @"
                Select fs.*, s.Descricao as DescrSetor From funcionario_setor fs
                    Inner Join setor s On (fs.idSetor=s.idSetor)
                Where idFunc=" + idFunc + @"
                Order By s.numSeq asc";

            return objPersistence.LoadData(sessao, sql).ToList();
        }

        /// <summary>
        /// Retorna os setores que o funcionário informado possui acesso.
        /// </summary>
        /// <param name="idFunc"></param>
        /// <returns></returns>
        public IList<FuncionarioSetor> GetSetores(uint idFunc)
        {
            return GetSetores(null, idFunc);
        }

        public string GetDescricaoSetores(uint idFunc)
        {
            return GetDescricaoSetores(null, idFunc);
        }

        public string GetDescricaoSetores(GDASession sessao, uint idFunc)
        {
            return String.Join(", ", Array.ConvertAll(new List<FuncionarioSetor>(GetSetores(sessao, idFunc)).ToArray(), x => x.DescrSetor));
        }

        /// <summary>
        /// Associa os setores selecionados ao funcionário informado.
        /// </summary>
        /// <param name="idFunc"></param>
        /// <param name="setores"></param>
        public void InsereSetores(uint idFunc, string setores)
        {
            Funcionario func = FuncionarioDAO.Instance.GetElement(idFunc);

            if (func.IdTipoFunc != (uint)Utils.TipoFuncionario.MarcadorProducao)
                return;

            string[] vetIdSetor = setores.TrimEnd().TrimEnd(',').Split(',');

            objPersistence.ExecuteCommand("delete from funcionario_setor where idFunc=" + idFunc);

            foreach (string idSetor in vetIdSetor)
                objPersistence.ExecuteCommand("Insert Into funcionario_setor (IdFunc, IdSetor) Values (" + idFunc + ", " + idSetor + ")");

            LogAlteracaoDAO.Instance.LogFuncionario(func, LogAlteracaoDAO.SequenciaObjeto.Atual);
        }

        public bool PossuiSetorEntregue(uint idFunc)
        {
            string sql = @"
                Select Count(*) > 0 From funcionario_setor fs
                    Inner Join setor s On (fs.idSetor=s.idSetor)
                Where idFunc=" + idFunc + @"
                    And s.tipo=" + (int)TipoSetor.Entregue;

            return ExecuteScalar<bool>(sql);
        }

        /// <summary>
        /// Remove todos os setores de um funcionário.
        /// </summary>
        /// <param name="idFunc"></param>
        public void DeleteByIdFunc(uint idFunc)
        {
            objPersistence.ExecuteCommand("DELETE FROM funcionario_setor WHERE idFunc=" + idFunc);
        }

        public bool PossuiSetorCarregamento(uint idFunc)
        {
            string sql = @"
                Select Count(*) > 0 From funcionario_setor fs
                    Inner Join setor s On (fs.idSetor=s.idSetor)
                Where idFunc=" + idFunc + @"
                    And s.tipo=" + (int)TipoSetor.ExpCarregamento;

            return ExecuteScalar<bool>(sql);
        }

    }
}
