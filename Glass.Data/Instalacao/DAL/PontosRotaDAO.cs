using System;
using System.Collections.Generic;
using Glass.Data.Model;
using GDA;

namespace Glass.Data.DAL
{
    public sealed class PontosRotaDAO : BaseDAO<PontosRota, PontosRotaDAO>
    {
        //private PontosRotaDAO() { }

        /// <summary>
        /// Retorna o último ponto capturado pela equipe
        /// </summary>
        /// <param name="idEquipe"></param>
        /// <returns></returns>
        public PontosRota GetLastByEquipe(uint idEquipe)
        {
            string sql = "Select * From pontos_rota Where idEquipe=" + idEquipe + " order by dataPonto desc limit 1";

            List<PontosRota> lstPontos = objPersistence.LoadData(sql);

            return lstPontos.Count > 0 ? lstPontos[0] : null;
        }

        /// <summary>
        /// Retorna os pontos capturados pela equipe no período passado
        /// </summary>
        /// <param name="idEquipe"></param>
        /// <param name="dtIni"></param>
        /// <param name="dtFim"></param>
        /// <returns></returns>
        public IList<PontosRota> GetByEquipe(uint idEquipe, string dtIni, string dtFim)
        {
            string sql = "Select * From pontos_rota p Where p.DataPonto>=?dataIni " +
                "And p.DataPonto<=?dataFim And p.idEquipe=" + idEquipe + " Order By p.DataPonto";

            return objPersistence.LoadData(sql, GetParams(dtIni, dtFim)).ToList();
        }

        /// <summary>
        /// Busca os últimos 40 pontos capturados pela equipe
        /// </summary>
        /// <param name="idEquipe"></param>
        /// <param name="qtdPontos">Qtd de pontos a serem retornados</param>
        /// <returns></returns>
        public IList<PontosRota> GetLastPoints(uint idEquipe, int qtdPontos)
        {
            string sql = "Select * From pontos_rota Where idEquipe=" + idEquipe + " order by dataPonto desc limit " + qtdPontos;

            return objPersistence.LoadData(sql).ToList();
        }

        /// <summary>
        /// Verifica se existe algum ponto dados os filtros passados
        /// </summary>
        /// <param name="idEquipe"></param>
        /// <param name="dtIni"></param>
        /// <param name="dtFim"></param>
        /// <returns></returns>
        public bool CheckPontosByEquipe(uint idEquipe, string dtIni, string dtFim)
        {
            string sql = "Select Count(*) From pontos_rota p Where p.DataPonto>=?dataIni " +
                         "And p.DataPonto<=?dataFim And p.idEquipe=" + idEquipe;

            return objPersistence.ExecuteSqlQueryCount(sql, GetParams(dtIni, dtFim)) > 0;
        }

        private GDAParameter[] GetParams(string dataIni, string dataFim)
        {
            List<GDAParameter> lstParam = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(dataIni))
                lstParam.Add(new GDAParameter("?dataIni", DateTime.Parse(dataIni)));

            if (!String.IsNullOrEmpty(dataFim))
                lstParam.Add(new GDAParameter("?dataFim", DateTime.Parse(dataFim)));

            return lstParam.Count > 0 ? lstParam.ToArray() : null;
        }
    }
}
