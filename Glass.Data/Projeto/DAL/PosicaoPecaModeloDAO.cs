using System.Collections.Generic;
using Glass.Data.Model;
using GDA;

namespace Glass.Data.DAL
{
    public sealed class PosicaoPecaModeloDAO : BaseDAO<PosicaoPecaModelo, PosicaoPecaModeloDAO>
    {
        //private PosicaoPecaModeloDAO() { }

        public string Sql(uint idProjetoModelo, bool selecionar)
        {
            string campos = selecionar ? "ppm.*, (@row := @row + 1) as numInfo" : "Count(*)";

            string sql = @"set @row = 0;
                Select " + campos + @"
                From posicao_peca_modelo ppm 
                Where ppm.idProjetoModelo=" + idProjetoModelo + @"
                order by ppm.idPosicaoPecaModelo asc";

            return sql;
        }

        public List<PosicaoPecaModelo> GetPosicoes(uint idProjetoModelo)
        {
            return GetPosicoes(null, idProjetoModelo);
        }

        public List<PosicaoPecaModelo> GetPosicoes(GDASession session, uint idProjetoModelo)
        {
            return objPersistence.LoadData(session, Sql(idProjetoModelo, true));
        }

        public PosicaoPecaModelo GetElement(uint idPosicaoPecaModelo)
        {
            return GetElement(null, idPosicaoPecaModelo);
        }

        public PosicaoPecaModelo GetElement(GDASession session, uint idPosicaoPecaModelo)
        {
            try
            {
                PosicaoPecaModelo item = GetElementByPrimaryKey(session, idPosicaoPecaModelo);
                foreach (PosicaoPecaModelo ppm in GetPosicoes(session, item.IdProjetoModelo))
                    if (ppm.IdPosicaoPecaModelo == idPosicaoPecaModelo)
                        return ppm;

                return item;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Insere/Atualiza posicionamentos coletados
        /// </summary>
        /// <param name="idProjetoModelo"></param>
        /// <param name="qtdPeca"></param>
        /// <param name="vetAltura"></param>
        /// <param name="vetLargura"></param>
        /// <param name="vetOrientacao"></param>
        public void AtualizaValores(uint idProjetoModelo, int qtdPos, string[] vetCoord, string[] vetOrientacao, string[] vetCalc)
        {
            List<PosicaoPecaModelo> lstPosicao = new List<PosicaoPecaModelo>();
            PosicaoPecaModelo posPecaMod;

            List<PosicaoPecaModelo> atuais = GetPosicoes(idProjetoModelo);

            for (int i = 0; i < qtdPos; i++)
            {
                string[] coord = vetCoord[i].Split(';');

                posPecaMod = new PosicaoPecaModelo();
                posPecaMod.IdProjetoModelo = idProjetoModelo;
                posPecaMod.CoordX = Glass.Conversoes.StrParaInt(coord[0]);
                posPecaMod.CoordY = Glass.Conversoes.StrParaInt(coord[1]);
                posPecaMod.Orientacao = Glass.Conversoes.StrParaInt(vetOrientacao[i]);
                posPecaMod.Calc = vetCalc[i];
                lstPosicao.Add(posPecaMod);
            }

            // Exclui todos os posicionamentos cadastrados para este modelo
            objPersistence.ExecuteCommand("Delete From posicao_peca_modelo Where idProjetoModelo=" + idProjetoModelo);

            // Insere os novos dados coletados
            for (int i = 0; i < lstPosicao.Count; i++)
            {
                uint novoId = PosicaoPecaModeloDAO.Instance.Insert(lstPosicao[i]);

                // Atualiza os ids dos logs
                if (atuais.Count > i)
                {
                    LogAlteracaoDAO.Instance.AtualizaID((int)LogAlteracao.TabelaAlteracao.PosicaoPecaModelo, atuais[i].IdPosicaoPecaModelo, novoId);
                    lstPosicao[i].IdPosicaoPecaModelo = novoId;
                    LogAlteracaoDAO.Instance.LogPosicaoPecaModelo(atuais[i], lstPosicao[i]);
                }
            }

            // Apaga os registros de log das peças atuais que não ficarão no banco de dados
            for (int i = lstPosicao.Count; i < atuais.Count; i++)
                LogAlteracaoDAO.Instance.ApagaLogPosicaoPecaModelo(atuais[i].IdPosicaoPecaModelo);
        }
    }
}
