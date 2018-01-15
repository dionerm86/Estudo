using System.Collections.Generic;
using GDA;
using Glass.Data.Model;

namespace Glass.Data.DAL
{
    public sealed class TextoOrcamentoDAO : BaseDAO<TextoOrcamento, TextoOrcamentoDAO>
	{
        //private TextoOrcamentoDAO() { }

        /// <summary>
        /// Busca todos os textos selecionados para o orçamento passado
        /// </summary>
        /// <param name="idOrcamento"></param>
        /// <returns></returns>
        public IList<TextoOrcamento> GetByOrcamento(uint idOrcamento)
        {
            string sql = @"
                Select tor.*, tio.titulo, tio.descricao 
                From texto_orcamento tor 
                    Inner Join texto_impr_orca tio On (tor.idTextoImprOrca=tio.idTextoImprOrca) 
                Where idOrcamento=" + idOrcamento + @"
                Order By tor.idTextoOrcamento";

            return objPersistence.LoadData(sql).ToList();
        }

        /// <summary>
        /// Associa textos de orçamento padrão aos orçamentos
        /// </summary>
        public void AssociaTextoOrcamentoPadrao(uint idOrcamento)
        {
            AssociaTextoOrcamentoPadrao(null, idOrcamento);
        }

        /// <summary>
        /// Associa textos de orçamento padrão aos orçamentos
        /// </summary>
        public void AssociaTextoOrcamentoPadrao(GDASession session, uint idOrcamento)
        {
            // Se não houver textos de orçamento padrão, não associa textos.
            if (objPersistence.ExecuteSqlQueryCount(session, "Select Count(*) From texto_impr_orca Where buscarSempre=true") == 0)
                return;

            string sql = @"
                Insert Into texto_orcamento (IdOrcamento, IdTextoImprOrca) 
                    (
                        Select " + idOrcamento + @", idTextoImprOrca From texto_impr_orca 
                        Where buscarSempre=true
                    )
                ";

            objPersistence.ExecuteCommand(session, sql);
        }
	}
}