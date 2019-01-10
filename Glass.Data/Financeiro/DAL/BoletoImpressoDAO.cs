using GDA;
using Glass.Data.Helper;
using Glass.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Glass.Data.DAL
{
    public sealed class BoletoImpressoDAO : BaseCadastroDAO<BoletoImpresso, BoletoImpressoDAO>
    {
        private string Sql(int? idContaR, int? idNf, bool selecionar)
        {
            var campos = string.Empty;

            if (selecionar)
            {
                campos = "bi.*";
            }
            else
            {
                campos = "COUNT(*)";
            }

            var sql = $"SELECT {campos} FROM boleto_impresso bi WHERE 1";

            if (idContaR > 0)
            {
                sql += $" AND IdContaR = {idContaR}";
            }

            if (idNf > 0)
            {
                sql += $" AND IdNf = {idNf}";
            }

            return sql;
        }

        /// <summary>
        /// Obtém o boleto impresso da conta a receber ou da nota fiscal.
        /// </summary>
        /// <param name="session">session.</param>
        /// <param name="idContaR">idContaR.</param>
        /// <param name="idNf">idNf.</param>
        /// <returns>Retorna o boleto impresso da conta a receber ou da nota fiscal.</returns>
        public BoletoImpresso ObterBoletoImpresso(GDASession session, int? idContaR, int? idNf)
        {
            return this.objPersistence.LoadOneData(session, this.Sql(idContaR, idNf, true));
        }

        /// <summary>
        /// Verifica se a conta a receber possui boleto impresso.
        /// </summary>
        /// <param name="session">session.</param>
        /// <param name="idContaR">idContaR.</param>
        /// <returns>True: a conta a receber possui boleto impresso.</returns>
        public bool VerificarPossuiBoletoImpresso(GDASession session, int? idContaR)
        {
            return this.objPersistence.ExecuteSqlQueryCount(session, this.Sql(idContaR, null, false)) > 0;
        }

        /// <summary>
        /// Insere um novo registro de impressão de boleto.
        /// </summary>
        /// <param name="session">session.</param>
        /// <param name="objInsert">objInsert.</param>
        /// <returns>Retorna o ID do registro de impressão de boleto.</returns>
        public int InserirBoletoImpresso(GDASession session, BoletoImpresso objInsert)
        {
            if (objInsert.IdContaR == 0)
            {
                throw new Exception("Informe a conta a receber do boleto.");
            }

            if (objInsert.IdContaBanco == 0)
            {
                throw new Exception("Informe a conta bancária do boleto.");
            }

            var descricaoContaBancoAntiga = string.Empty;
            var descricaoContaBancoAtual = ContaBancoDAO.Instance.GetDescricao(session, (uint)objInsert.IdContaBanco);

            // Remove o registro de impressão de boleto da conta a receber,
            // pois a tabela permite somente um registro de impressão de boleto por conta a receber.
            if (this.Exists(session, objInsert.IdContaR))
            {
                var idContaBancoAntiga = this.ObtemValorCampo<int>("IdContaBanco", $"IdContaR = {objInsert.IdContaR}");
                descricaoContaBancoAntiga = ContaBancoDAO.Instance.GetDescricao(session, (uint)idContaBancoAntiga);

                // A descrição da conta bancária antiga, deve ser salva na variável somente se tiver sido trocada.
                // Para que o log da conta a receber seja gerado corretamente.
                if (!string.IsNullOrWhiteSpace(descricaoContaBancoAntiga) && descricaoContaBancoAntiga == descricaoContaBancoAtual)
                {
                    descricaoContaBancoAntiga = string.Empty;
                }

                this.Delete(objInsert);
            }

            // Toda impressão de boleto ficará salva no log de alterações da conta a receber.
            LogAlteracaoDAO.Instance.Insert(session,
                new LogAlteracao
                {
                    Campo = "Boleto Impresso",
                    DataAlt = DateTime.Now,
                    IdFuncAlt = UserInfo.GetUserInfo.CodUser,
                    IdRegistroAlt = (int)objInsert.IdContaR,
                    Tabela = (int)LogAlteracao.TabelaAlteracao.ContasReceber,
                    ValorAnterior = descricaoContaBancoAntiga,
                    ValorAtual = descricaoContaBancoAtual,
                });

            return (int)this.Insert(session, objInsert);
        }
    }
}
