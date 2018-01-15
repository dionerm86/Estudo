using System;
using System.Collections.Generic;
using Glass.Data.Model;

namespace Glass.Data.DAL
{
    public sealed class PontoFotoMedicaoDAO : BaseDAO<PontoFotoMedicao, PontoFotoMedicaoDAO>
    {
        //private PontoFotoMedicaoDAO() { }

        /// <summary>
        /// Retorna todos os pontos cadastrados para a foto passada
        /// </summary>
        /// <param name="idFoto"></param>
        public IList<PontoFotoMedicao> GetByFoto(uint idFoto)
        {
            var sql = "Select * From ponto_foto_medicao Where idFoto=" + idFoto;

            return objPersistence.LoadData(sql).ToList();
        }

        /// <summary>
        /// Salva os pontos utilizados para formar a área de uma figura.
        /// </summary>
        /// <param name="idFoto">Foto a qual os pontos passados se relacionam</param>
        /// <param name="pontos">Pontos separados da seguinte maneira: p1x;p1y|p2x;p2y|...</param>
        public void SalvarPontos(uint idFoto, string pontos)
        {
            var sql = "Insert Into ponto_foto_medicao (idFoto, coordX, coordY) Values ";

            var vetPontos = pontos.TrimEnd('|').Split('|');

            foreach (string ponto in vetPontos)
            {
                var p = ponto.Split(';');

                sql += "(" + idFoto + ", " + p[0] + ", " + p[1] + "),";
            }

            if (objPersistence.ExecuteCommand(sql.TrimEnd(',') + ";") <= 0)
                throw new Exception("Inserção de pontos retornou 0.");
        }

        /// <summary>
        /// Exclui todos os pontos cadastrados para a foto passada
        /// </summary>
        /// <param name="idFoto"></param>
        public void DeleteByFoto(uint idFoto)
        {
            var sql = "Delete From ponto_foto_medicao Where idFoto=" + idFoto;

            objPersistence.ExecuteCommand(sql);
        }
    }
}
