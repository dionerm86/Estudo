using GDA;
using Glass.Data.Model;
using System.Linq;

namespace Glass.Data.DAL
{
    public sealed class DadosCnabDAO : BaseDAO<DadosCnab, DadosCnabDAO>
    {
        /// <summary>
        /// Obtem o valor padrão
        /// </summary>
        public int ObterTipoCnabPadrao(GDASession sessao, int codBanco)
        {
            var tipoCnabPadrao = objPersistence.ExecuteScalar(sessao,
                string.Format(@"SELECT TipoCnab FROM dados_cnab WHERE COALESCE(ValorPadrao, 0) = 1 AND CodBanco = {0}", codBanco));

            return tipoCnabPadrao != null && tipoCnabPadrao.ToString().StrParaInt() > 0 ?
                tipoCnabPadrao.ToString().StrParaInt() : 0;
        }

        /// <summary>
        /// Obtem o valor padrão
        /// </summary>
        public DadosCnab ObtemValorPadrao(GDASession sessao, int codBanco, int tipoArquivo)
        {
            var sql = @"SELECT * FROM dados_cnab WHERE COALESCE(ValorPadrao, 0) = 1 AND CodBanco = " + codBanco;

            if (tipoArquivo > 0)
                sql += string.Format(" AND TipoCnab={0}", tipoArquivo);

            return objPersistence.LoadOneData(sessao, sql);
        }

        public DadosCnab ObtemValorPadrao(int codBanco, int idNf, int idContaR)
        {
            int idRemessa = 0;

            if (idContaR > 0)
                idRemessa = ContasReceberDAO.Instance.ObtemIdArquivoRemessa(idContaR);
            else if (idNf > 0)
            {
                var idsContasReceber = ContasReceberDAO.Instance.ObtemPelaNfe((uint)idNf);

                if (idsContasReceber.Any())
                    idRemessa = ContasReceberDAO.Instance.ObtemIdArquivoRemessa((int)idsContasReceber[0]);
            }

            DadosCnab dados = null;

            if (idRemessa > 0)
                dados = objPersistence.LoadOneData("SELECT * FROM dados_cnab WHERE IdArquivoRemessa = " + idRemessa + " AND CodBanco = " + codBanco);

            if (dados == null)
                dados = objPersistence.LoadOneData("SELECT * FROM dados_cnab WHERE COALESCE(ValorPadrao, 0) = 1 AND CodBanco = " + codBanco);

            return dados;
        }

        /// <summary>
        /// Salva o valor padrão de um banco
        /// </summary>
        /// <param name="dados"></param>
        public void SalvarValorPadrao(DadosCnab dados)
        {
            using (var trans = new GDATransaction())
            {
                try
                {
                    trans.BeginTransaction();

                    var dadosOriginais = ObtemValorPadrao(trans, dados.CodBanco, 0);

                    dados.IdArquivoRemessa = null;
                    dados.ValorPadrao = true;

                    if (dadosOriginais != null)
                    {
                        dados.IdDadosCnab = dadosOriginais.IdDadosCnab;
                        dados.ExistsInStorage = true;
                        Update(trans, dados);
                    }
                    else
                        Insert(trans, dados);
                    
                    trans.Commit();
                    trans.Close();
                }
                catch (System.Exception ex)
                {
                    trans.Rollback();
                    trans.Close();
                    throw ex;
                }
            }
        }
    }
}
