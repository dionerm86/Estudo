using System.Collections.Generic;
using Glass.Data.Model;
using GDA;

namespace Glass.Data.DAL
{
    public sealed class PosicaoPecaIndividualDAO : BaseDAO<PosicaoPecaIndividual, PosicaoPecaIndividualDAO>
    {
        //private PosicaoPecaIndividualDAO() { }

        public string Sql(uint idPecaProjMod, int item, bool selecionar)
        {
            string campos = selecionar ? "ppi.*, (@row := @row + 1) as numInfo" : "Count(*)";

            string sql = @"
                set @row = 0;
                Select " + campos + @"
                From posicao_peca_individual ppi 
                Where ppi.idPecaProjMod=" + idPecaProjMod + " and item=" + item + @"
                order by ppi.idPosPecaInd";

            return sql;
        }

        public List<PosicaoPecaIndividual> GetPosicoes(uint idPecaProjMod, int item)
        {
            return GetPosicoes(null, idPecaProjMod, item);
        }

        public List<PosicaoPecaIndividual> GetPosicoes(GDASession session, uint idPecaProjMod, int item)
        {
            return objPersistence.LoadData(session, Sql(idPecaProjMod, item, true));
        }

        public PosicaoPecaIndividual GetElement(uint idPosPecaInd)
        {
            return GetElement(null, idPosPecaInd);
        }

        public PosicaoPecaIndividual GetElement(GDASession session, uint idPosPecaInd)
        {
            try
            {
                PosicaoPecaIndividual item = GetElementByPrimaryKey(session, idPosPecaInd);
                foreach (PosicaoPecaIndividual ppi in GetPosicoes(session, item.IdPecaProjMod, item.Item))
                    if (ppi.IdPosPecaInd == idPosPecaInd)
                        return ppi;

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
        public void AtualizaValores(uint idPecaProjMod, int item, int qtdPos, string[] vetCoord, string[] vetOrientacao, string[] vetCalc)
        {
            List<PosicaoPecaIndividual> lstPosicao = new List<PosicaoPecaIndividual>();
            PosicaoPecaIndividual posPecaMod;

            List<PosicaoPecaIndividual> atuais = GetPosicoes(idPecaProjMod, item);

            for (int i = 0; i < qtdPos; i++)
            {
                string[] coord = vetCoord[i].Split(';');

                posPecaMod = new PosicaoPecaIndividual();
                posPecaMod.IdPecaProjMod = idPecaProjMod;
                posPecaMod.Item = item;
                posPecaMod.CoordX = Glass.Conversoes.StrParaInt(coord[0]);
                posPecaMod.CoordY = Glass.Conversoes.StrParaInt(coord[1]);
                posPecaMod.Orientacao = Glass.Conversoes.StrParaInt(vetOrientacao[i]);
                posPecaMod.Calc = vetCalc[i];
                lstPosicao.Add(posPecaMod);
            }

            // Exclui todos os posicionamentos cadastrados para este modelo
            objPersistence.ExecuteCommand("Delete From posicao_peca_individual Where idPecaProjMod=" + idPecaProjMod +
                " And item=" + item);

            // Insere os novos dados coletados
            for (int i = 0; i < lstPosicao.Count; i++)
            {
                uint novoId = PosicaoPecaIndividualDAO.Instance.Insert(lstPosicao[i]);

                // Atualiza os ids dos logs
                if (atuais.Count > i)
                {
                    LogAlteracaoDAO.Instance.AtualizaID((int)LogAlteracao.TabelaAlteracao.PosicaoPecaIndividual, atuais[i].IdPosPecaInd, novoId);
                    lstPosicao[i].IdPosPecaInd = novoId;
                    LogAlteracaoDAO.Instance.LogPosicaoPecaIndividual(atuais[i], lstPosicao[i]);
                }
            }

            // Apaga os registros de log das peças atuais que não ficarão no banco de dados
            for (int i = lstPosicao.Count; i < atuais.Count; i++)
                LogAlteracaoDAO.Instance.ApagaLogPosicaoPecaIndividual(atuais[i].IdPosPecaInd);
        }

        /// <summary>
        /// Apaga as posições da peça pelo ID da mesma.
        /// </summary>
        public void ApagarPeloIdPecaProjMod(GDASession session, int idPecaProjMod)
        {
            objPersistence.ExecuteCommand(session, "DELETE FROM posicao_peca_individual WHERE IdPecaProjMod=" + idPecaProjMod);
        }
    }
}
