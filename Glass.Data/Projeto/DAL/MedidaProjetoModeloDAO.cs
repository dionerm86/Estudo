using System;
using System.Collections.Generic;
using Glass.Data.Model;
using GDA;

namespace Glass.Data.DAL
{
    public sealed class MedidaProjetoModeloDAO : BaseDAO<MedidaProjetoModelo, MedidaProjetoModeloDAO>
    {
        //private MedidaProjetoModeloDAO() { }

        public List<MedidaProjetoModelo> GetByProjetoModelo(uint idProjetoModelo, bool calc)
        {
            return GetByProjetoModelo(null, idProjetoModelo, calc);
        }

        public List<MedidaProjetoModelo> GetByProjetoModelo(GDASession sessao, uint idProjetoModelo, bool calc)
        {
            string sql = @"
                Select mpm.*, mp.Descricao as DescrMedida 
                From medida_projeto_modelo mpm 
                    Inner Join medida_projeto mp On (mpm.idMedidaProjeto=mp.idMedidaProjeto)
                Where mpm.idProjetoModelo=" + idProjetoModelo + @"
                Group By mp.idMedidaProjeto";

            if (calc)
                sql += " Order By length(mp.Descricao) desc";
            else // Ordena pela ordem que a medida do projeto foi cadastrada, e não pela ordem que foi associada ao projeto.
                //sql += " Order By mpm.idMedidaProjetoModelo";
                sql += " Order By mpm.idMedidaProjeto asc";

            return objPersistence.LoadData(sessao, sql);
        }

        /// <summary>
        /// Verifica se as medidas retiradas do modelo de projeto (Após atualizar modelo de projeto) 
        /// estão sendo usadas em alguma expressão de cálculo
        /// </summary>
        public bool MedidasRetiradasEmUso(GDASession session, uint idProjetoModelo, string idsMedidaProjetoNovos)
        {
            if (string.IsNullOrEmpty(idsMedidaProjetoNovos))
                idsMedidaProjetoNovos = "0";

            var sql = @"
                Select mpm.*, mp.Descricao as DescrMedida 
                From medida_projeto_modelo mpm 
                    Inner Join medida_projeto mp On (mpm.idMedidaProjeto=mp.idMedidaProjeto)
                Where mpm.idMedidaProjeto Not In (" + idsMedidaProjetoNovos.Trim(',') + @")
                And mpm.idProjetoModelo=" + idProjetoModelo + @"
                Order By mp.idMedidaProjeto";

            var lstMedidasRetiradas = objPersistence.LoadData(session, sql).ToList();

            foreach (var mpm in lstMedidasRetiradas)
            {
                GDAParameter param = new GDAParameter("?expressao", mpm.CalcTipoMedida);

                if (objPersistence.ExecuteSqlQueryCount(session, "Select Count(*) From peca_projeto_modelo Where " + 
                    "idProjetoModelo=" + idProjetoModelo + " And InStr(calculoQtde, ?expressao) > 0", param) > 0)
                    return true;

                if (objPersistence.ExecuteSqlQueryCount(session, "Select Count(*) From peca_projeto_modelo Where " +
                    "idProjetoModelo=" + idProjetoModelo + " And InStr(calculoAltura, ?expressao) > 0", param) > 0)
                    return true;

                if (objPersistence.ExecuteSqlQueryCount(session, "Select Count(*) From peca_projeto_modelo Where " +
                    "idProjetoModelo=" + idProjetoModelo + " And InStr(calculoLargura, ?expressao) > 0", param) > 0)
                    return true;

                if (objPersistence.ExecuteSqlQueryCount(session, "Select Count(*) From material_projeto_modelo Where " +
                    "idProjetoModelo=" + idProjetoModelo + " And InStr(calculoQtde, ?expressao) > 0", param) > 0)
                    return true;

                if (objPersistence.ExecuteSqlQueryCount(session, "Select Count(*) From material_projeto_modelo Where " +
                    "idProjetoModelo=" + idProjetoModelo + " And InStr(calculoAltura, ?expressao) > 0", param) > 0)
                    return true;

                if (objPersistence.ExecuteSqlQueryCount(session, "Select Count(*) From posicao_peca_modelo Where " +
                    "idProjetoModelo=" + idProjetoModelo + " And InStr(calc, ?expressao) > 0", param) > 0)
                    return true;

                if (objPersistence.ExecuteSqlQueryCount(session, "Select Count(*) From posicao_peca_modelo Where " +
                    "idProjetoModelo=" + idProjetoModelo + " And InStr(calc, ?expressao) > 0", param) > 0)
                    return true;

                if (objPersistence.ExecuteSqlQueryCount(session, "Select Count(*) From posicao_peca_individual Where " +
                    "idPecaProjMod in (Select idPecaProjMod From peca_projeto_modelo Where idProjetoModelo=" + idProjetoModelo + 
                    ") And InStr(calc, ?expressao) > 0", param) > 0)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Salva os tipos de medidas a serem utilizadas no projeto
        /// </summary>
        public void SalvaMedidas(GDASession session, uint idProjetoModelo, string medidas)
        {
            if (string.IsNullOrEmpty(medidas))
                return;

            // Exclui as medidas deste modelo
            objPersistence.ExecuteCommand(session, "Delete from medida_projeto_modelo Where idProjetoModelo=" + idProjetoModelo);

            string sqlInsert = "Insert Into medida_projeto_modelo (idProjetoModelo, idMedidaProjeto) Values (" + idProjetoModelo + ", ?idMedidaProjeto)";

            // Salva as novas medidas
            string[] vetMedida = medidas.Split(',');

            foreach (string med in vetMedida)
                objPersistence.ExecuteCommand(session, sqlInsert.Replace("?idMedidaProjeto", med));
        }

        /// <summary>
        /// Verifica se uma medida existe no projeto passado pela sua descrição
        /// </summary>
        /// <param name="idProjetoModelo"></param>
        /// <param name="descricao"></param>
        /// <returns></returns>
        public bool ExisteMedidaProjeto(uint idProjetoModelo, string descricao)
        {
            string sql = @"
                Select Count(*) > 0
                From medida_projeto_modelo mpm
                    Inner Join medida_projeto mp On (mpm.idMedidaProjeto=mp.idMedidaProjeto)
                Where idProjetoModelo=" + idProjetoModelo + @" 
                    And mp.descricao=?descricao";

            return ExecuteScalar<bool>(sql, new GDAParameter("?descricao", descricao));
        }
    }
}
