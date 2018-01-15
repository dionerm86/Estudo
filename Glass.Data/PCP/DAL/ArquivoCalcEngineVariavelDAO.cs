using System.Collections.Generic;
using Glass.Data.Model;

namespace Glass.Data.DAL
{
    public sealed class ArquivoCalcEngineVariavelDAO : BaseDAO<ArquivoCalcEngineVariavel, ArquivoCalcEngineVariavelDAO>
    {
        //private ArquivoCalcEngineVariavelDAO() { }

        #region Obtém dados

        /// <summary>
        /// Retorna as variáveis do arquivo do CalcEngine.
        /// </summary>
        /// <param name="idArquivoCalcEngine"></param>
        /// <param name="buscarAlturaLargura"></param>
        public List<ArquivoCalcEngineVariavel> ObtemPeloIdArquivoCalcEngine(uint idArquivoCalcEngine, bool buscarAlturaLargura)
        {
            return ObtemPeloIdArquivoCalcEngine(null, idArquivoCalcEngine, buscarAlturaLargura);
        }

        /// <summary>
        /// Retorna as variáveis do arquivo do CalcEngine.
        /// </summary>
        /// <param name="session"></param>
        /// <param name="idArquivoCalcEngine"></param>
        /// <param name="buscarAlturaLargura"></param>
        public List<ArquivoCalcEngineVariavel> ObtemPeloIdArquivoCalcEngine(GDA.GDASession session, uint idArquivoCalcEngine, bool buscarAlturaLargura)
        {
            return objPersistence.LoadData(session, @"SELECT acv.* FROM arquivo_calcengine_variavel acv
                WHERE acv.idArquivoCalcEngine=" + idArquivoCalcEngine +
                (!buscarAlturaLargura ? " AND acv.variavelCalcEngine NOT IN ('Altura', 'Largura') " : "") +
                " ORDER BY VariavelCalcEngine ASC");
        }

        #endregion

        #region Apaga variáveis

        /// <summary>
        /// Deleta as variáveis associadas ao arquivo do CalcEngine informado por parâmetro.
        /// </summary>
        /// <param name="idArquivoCalcEngine"></param>
        public List<ArquivoCalcEngineVariavel> DeletaPeloIdArquivoCalcEngine(uint idArquivoCalcEngine)
        {
            return objPersistence.LoadData(@"DELETE FROM arquivo_calcengine_variavel
                WHERE idArquivoCalcEngine=" + idArquivoCalcEngine);
        }

        #endregion

        #region Métodos sobescritos

        public override uint Insert(Glass.Data.Model.ArquivoCalcEngineVariavel objInsert)
        {
            if (objInsert.VariavelCalcEngine.ToLower() == "altura" || objInsert.VariavelCalcEngine.ToLower() == "largura")
                objInsert.VariavelSistema = objInsert.VariavelCalcEngine.ToLower();

            return base.Insert(objInsert);
        }

        #endregion
    }
}