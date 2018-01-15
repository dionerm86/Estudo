﻿using System;
using System.Collections.Generic;
using Glass.Data.Model;
using GDA;

namespace Glass.Data.DAL
{
    public sealed class MedidaProjetoDAO : BaseDAO<MedidaProjeto, MedidaProjetoDAO>
    {
        //private MedidaProjetoDAO() { }

        private string SqlList(string descricao, bool selecionar)
        {
            string campos = selecionar ? "mp.*, gmp.descricao as descrGrupo" : "Count(*)";

            string where = "";

            string sort = " order by mp.descricao asc";

            if (!string.IsNullOrEmpty(descricao))
                where += " and mp.descricao like '%" + descricao + "%'";

            string sql = @"
                Select " + campos + @" 
                From medida_projeto mp Left Join grupo_medida_projeto gmp On (mp.idGrupoMedProj=gmp.idGrupoMedProj) where 1 " + where + sort;

            return sql;
        }

        public IList<MedidaProjeto> GetMedidas()
        {
            return objPersistence.LoadData(SqlList(null, true)).ToList();
        }

        public IList<MedidaProjeto> GetList(string descricao, string sortExpression, int startRow, int pageSize)
        {
            if (GetCountReal(descricao) == 0)
            {
                var lst = new List<MedidaProjeto>();
                lst.Add(new MedidaProjeto());
                return lst.ToArray();
            }

            return LoadDataWithSortExpression(SqlList(descricao, true), sortExpression, startRow, pageSize, null);
        }

        public int GetCountReal(string descricao)
        {
            return objPersistence.ExecuteSqlQueryCount(SqlList(descricao, false), null);
        }

        public int GetCount(string descricao)
        {
            int count = objPersistence.ExecuteSqlQueryCount(SqlList(descricao, false), null);

            return count == 0 ? 1 : count;
        }

        /// <summary>
        /// Verifica se a descrição da medida está sendo usada em alguma expressão
        /// </summary>
        /// <param name="descricao"></param>
        /// <returns></returns>
        private bool EstaEmUso(string descricao)
        {
            if (objPersistence.ExecuteSqlQueryCount("Select Count(*) From peca_projeto_modelo Where InStr(calculoQtde, ?calcQtd)>0", 
                new GDAParameter("?calcQtd", MedidaProjetoModelo.TrataDescricao(descricao).ToUpper())) > 0)
                return true;

            if (objPersistence.ExecuteSqlQueryCount("Select Count(*) From peca_projeto_modelo Where InStr(calculoAltura, ?calcAlt)>0", 
                new GDAParameter("?calcAlt", MedidaProjetoModelo.TrataDescricao(descricao).ToUpper())) > 0)
                return true;
            
            if (objPersistence.ExecuteSqlQueryCount("Select Count(*) From peca_projeto_modelo Where InStr(calculoLargura, ?calcLarg)>0", 
                new GDAParameter("?calcLarg", MedidaProjetoModelo.TrataDescricao(descricao).ToUpper())) > 0)
                return true;

            if (objPersistence.ExecuteSqlQueryCount("Select Count(*) From material_projeto_modelo Where InStr(calculoQtde, ?calcQtd)>0", 
                new GDAParameter("?calcQtd", MedidaProjetoModelo.TrataDescricao(descricao).ToUpper())) > 0)
                return true;
            
            if (objPersistence.ExecuteSqlQueryCount("Select Count(*) From material_projeto_modelo Where InStr(calculoAltura, ?calcAlt)>0", 
                new GDAParameter("?calcAlt", MedidaProjetoModelo.TrataDescricao(descricao).ToUpper())) > 0)
                return true;
            
            if (objPersistence.ExecuteSqlQueryCount("Select Count(*) From posicao_peca_modelo Where InStr(calc, ?calc)>0", 
                new GDAParameter("?calc", MedidaProjetoModelo.TrataDescricao(descricao).ToUpper())) > 0)
                return true;

            if (objPersistence.ExecuteSqlQueryCount("Select Count(*) From posicao_peca_individual Where InStr(calc, ?calc)>0",
                new GDAParameter("?calc", MedidaProjetoModelo.TrataDescricao(descricao).ToUpper())) > 0)
                return true;

            return false;
        }

        public uint? FindByDescricao(uint idMedidaProjeto, string descricao)
        {
            string trataDescr = @"
                Replace(Replace(Replace(Replace(Replace(Replace(Replace(Replace(Replace(descricao, ' ', ''), 
                '.', ''), 'ã', 'a'), 'á', 'a'), 'â', 'a'), 'é', 'e'), 'ê', 'e'), 'í', 'i'), 'ç', 'c')";

            GDAParameter p = new GDAParameter("?descricao", MedidaProjetoModelo.TrataDescricao(descricao));
            string sql = "select count(*) from medida_projeto where idMedidaProjeto=" + idMedidaProjeto + " and " + trataDescr + "=?descricao";
            if (objPersistence.ExecuteSqlQueryCount(sql, p) > 0)
                return idMedidaProjeto;

            sql = "select {0} from medida_projeto where " + trataDescr + "=?descricao";
            if (objPersistence.ExecuteSqlQueryCount(string.Format(sql, "count(*)"), p) > 0)
                return ExecuteScalar<uint?>(string.Format(sql, "idMedidaProjeto"), p);

            return null;
        }

        /// <summary>
        /// Obtém o valor padrão de uma medida de projeto
        /// </summary>
        /// <param name="idMedidaProjeto"></param>
        /// <returns></returns>
        public int ObtemValorPadrao(uint idMedidaProjeto)
        {
            string sql = "Select valorPadrao From medida_projeto where idMedidaProjeto=" + idMedidaProjeto;

            object obj = objPersistence.ExecuteScalar(sql);

            return obj != null && obj.ToString() != String.Empty ? Glass.Conversoes.StrParaInt(obj.ToString()) : 0;
        }

        public string ObtemDescricao(uint idMedidaProjeto)
        {
            return ObtemDescricao(null, idMedidaProjeto);
        }

        public string ObtemDescricao(GDASession sessao, uint idMedidaProjeto)
        {
            return ObtemValorCampo<string>(sessao, "descricao", "idMedidaProjeto=" + idMedidaProjeto);
        }

        /// <summary>
        /// Define se a medida deverá ser preenchida mesmo se for cálculo de medidas exatas.
        /// </summary>
        /// <param name="idMedidaProjeto"></param>
        /// <returns></returns>
        public bool ExibirCalcMedidaExata(uint idMedidaProjeto)
        {
            string sql = "Select Count(*) From medida_projeto Where exibirMedidaExata=true and idMedidaProjeto=" + idMedidaProjeto;

            return objPersistence.ExecuteSqlQueryCount(sql) > 0;
        }

        #region Métodos sobrescritos

        public override int Update(MedidaProjeto objUpdate)
        {
            string descrAntiga = ObtemValorCampo<string>("descricao", "idMedidaProjeto=" + objUpdate.IdMedidaProjeto);

            // Verifica se a descrição antiga e a nova desta medida já está sendo usada
            if (descrAntiga != objUpdate.Descricao && (EstaEmUso(descrAntiga) || EstaEmUso(objUpdate.Descricao)))
                throw new Exception("A descrição desta medida não pode ser alterada por haver expressões de cálculo relacionadas à mesma.");

            LogAlteracaoDAO.Instance.LogMedidaProjeto(objUpdate);
            return base.Update(objUpdate);
        }

        public override uint Insert(MedidaProjeto objInsert)
        {
            // Verifica se medida já existe
            if (FindByDescricao(0, objInsert.Descricao) > 0)
                throw new Exception("Já existe uma medida cadastrada com esta descrição.");

            return base.Insert(objInsert);
        }

        public override int DeleteByPrimaryKey(uint Key)
        {
            if (EstaEmUso(ObtemValorCampo<string>("descricao", "idMedidaProjeto=" + Key)))
                throw new Exception("Esta medida não pode ser excluída por haver expressões de cálculo relacionadas à mesma.");

            LogAlteracaoDAO.Instance.ApagaLogMedidaProjeto(Key);
            return base.DeleteByPrimaryKey(Key);
        }

        public override int Delete(MedidaProjeto objDelete)
        {
            return DeleteByPrimaryKey(objDelete.IdMedidaProjeto);
        }

        #endregion
    }
}
