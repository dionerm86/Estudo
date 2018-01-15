using System;
using System.Collections.Generic;
using Glass.Data.Helper;
using Glass.Data.Model;
using Glass.Data.DAL;
using Glass.Data.RelModel;
using GDA;
using Glass.Configuracoes;

namespace Glass.Data.RelDAL
{
    public sealed class EmailCobrancaDAO : BaseDAO<EmailCobranca, EmailCobrancaDAO>
    {
        //private EmailCobrancaDAO() { }

        #region SQL para recuperar contas a receber

        private string SqlVencidas(string data)
        {
            return @"
                SELECT COUNT(*) as NumContasVec, SUM(cr.valorVec) as ValorContasVec,
                    0 as NumContasVecHoje, 0 as ValorContasVecHoje,
                    0 as NumContasAVec, 0 as ValorContasAVec,
                    cli.Nome as NomeCli, cr.idCliente, cli.email
                FROM contas_receber cr
                    LEFT JOIN cliente cli ON (cr.idCliente = cli.id_Cli) 
                WHERE cr.recebida <> true 
                    AND coalesce(isParcelaCartao,false)=false
                    AND !coalesce(cli.NaoReceberEmailCobrancaVencida, false) 
                    AND DATE(cr.dataVec) <= '" + data + @"'
                GROUP BY cli.id_Cli";
        }

        private string SqlVecHoje(string data)
        {
            return @"
                SELECT 0 as NumContasVec, 0 as ValorContasVec,
                    COUNT(*) as NumContasVecHoje, SUM(cr.valorVec) as ValorContasVecHoje,
                    0 as NumContasAVec, 0 as ValorContasAVec,
                    cli.Nome as NomeCli, cr.idCliente, cli.email
                FROM contas_receber cr
                    LEFT JOIN cliente cli ON (cr.idCliente = cli.id_Cli) 
                WHERE cr.recebida <> true
                    AND coalesce(isParcelaCartao,false)=false
                    AND !coalesce(cli.NaoReceberEmailCobrancaVencer, false) 
                    AND DATE(cr.dataVec) = '" + data + @"'
                GROUP BY cli.id_Cli";
        }

        private string SqlAVec(string data)
        {
            return @"
                SELECT 0 as NumContasVec, 0 as ValorContasVec,
                    0 as NumContasVecHoje, 0 as ValorContasVecHoje,
                    COUNT(*) as NumContasAVec, SUM(cr.valorVec) as ValorContasAVec,
                    cli.Nome as NomeCli, cr.idCliente, cli.email
                FROM contas_receber cr
                    LEFT JOIN cliente cli ON (cr.idCliente = cli.id_Cli) 
                WHERE cr.recebida <> true 
                    AND coalesce(isParcelaCartao,false)=false
                    AND !coalesce(cli.NaoReceberEmailCobrancaVencer, false) 
                    AND DATE(cr.dataVec) = '" + data + @"'
                GROUP BY cli.id_Cli";
        }

        private string sqlContasReceber()
        {
            uint? numDiasAntVenc = FinanceiroConfig.NumDiasAnteriorVencContaRecEnviarEmailCli;
            uint? numDiasAposVenc = FinanceiroConfig.FormaPagamento.NumDiasAposVencContaRecEnviarEmailCli;

            DateTime dtHoje = DateTime.Now;

            string dtVencida = numDiasAntVenc.HasValue && numDiasAntVenc.Value > 0 ? dtHoje.AddDays(-numDiasAntVenc.Value).ToString("yyyy-MM-dd") : "";
            string dtAVec = numDiasAposVenc.HasValue && numDiasAposVenc > 0 ? dtHoje.AddDays(numDiasAntVenc.Value).ToString("yyyy-MM-dd") : "";

            return @"
                SELECT NomeCli, idCliente, email,
                    SUM(NumContasVec) as NumContasVec, SUM(ValorContasVec) as ValorContasVec,
                    SUM(NumContasVecHoje) as NumContasVecHoje, SUM(ValorContasVecHoje) as ValorContasVecHoje,
                    SUM(NumContasAVec) as NumContasAVec, SUM(ValorContasAVec) as ValorContasAVec
                FROM (
                    " + SqlVecHoje(dtHoje.ToString("yyyy-MM-dd"))
                      + (!string.IsNullOrEmpty(dtVencida) ? " UNION ALL " + SqlVencidas(dtVencida) : "")
                      + (!string.IsNullOrEmpty(dtAVec) ? " UNION ALL " + SqlAVec(dtAVec) : "") + @"
                ) as tmp
                GROUP BY idCliente";
        }

        #endregion

        #region Envia E-mail de cobrança de contas vencidas e a vencer

        public void EnviaEmailCobranca()
        {
            using (var trans = new GDATransaction())
            {
                try
                {
                    //Se a empresa nao usa o controle de cobraça sai da função
                    if (!FinanceiroConfig.UsarControleCobrancaEmail)
                        return;

                    trans.BeginTransaction();

                    List<EmailCobranca> contasRec = objPersistence.LoadData(trans, sqlContasReceber());

                    foreach (EmailCobranca email in contasRec)
                    {
                        if (string.IsNullOrEmpty(email.EmailCliente))
                            continue;

                        if (email.ValorContasVec == 0 && email.ValorContasVecHoje == 0 && email.ValorContasAVec == 0)
                            continue;

                        string mensagem = CorpoEmail(trans, email.IdCliente, email.NomeCliente, email.NumContasVec, email.NumContasVecHoje, email.NumContasAVec,
                            email.ValorContasVec, email.ValorContasVecHoje, email.ValorContasAVec);

                        if (string.IsNullOrEmpty(mensagem))
                            continue;

                        /* Chamado 38162. */
                        if (objPersistence.ExecuteSqlQueryCount(trans,
                            "SELECT COUNT(*) FROM fila_email WHERE Mensagem=?mensagem AND DataCad>?data",
                            new GDAParameter("?mensagem", mensagem), new GDAParameter("?data", DateTime.Now.AddMinutes(-5))) > 0)
                            continue;

                        List<AnexoEmail> anexos = new List<AnexoEmail>();

                        Email.EnviaEmailAsync(trans, UserInfo.GetUserInfo.IdLoja, email.EmailCliente, "Aviso de Cobrança", mensagem,
                            Email.EmailEnvio.Fiscal, anexos.ToArray());
                    }

                    trans.Commit();
                    trans.Close();

                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    trans.Close();

                    throw new Exception(Glass.MensagemAlerta.FormatErrorMsg("Falha ao gerar e-mails de cobrança", ex));
                }
            }
        }

        public string CorpoEmail(GDASession sessao, uint idCliente, string nomeCli, decimal numContasVec, decimal numContasVecHoje, decimal numContasAVec,
            decimal valorVec, decimal valorVecHoje, decimal valorAVec)
        {
            uint diasVec = FinanceiroConfig.NumDiasAnteriorVencContaRecEnviarEmailCli.GetValueOrDefault(0);
            uint diasApos = FinanceiroConfig.FormaPagamento.NumDiasAposVencContaRecEnviarEmailCli.GetValueOrDefault(0);

            bool enviarVencida = ClienteDAO.Instance.ObtemValorCampo<bool>(sessao, "naoReceberEmailCobrancaVencida", "id_Cli=" + idCliente);
            bool enviarVencer = ClienteDAO.Instance.ObtemValorCampo<bool>(sessao, "naoReceberEmailCobrancaVencer", "id_Cli=" + idCliente);

            string loja = LojaDAO.Instance.GetNome(sessao, UserInfo.GetUserInfo.IdLoja);

            string textoVec = numContasVec > 0 && valorVec > 0 && enviarVencida ? 
                numContasVec + " conta(s) vencidas a mais de " + diasVec + " dia(s) com total de " + valorVec.ToString("C") : 
                "";

            string textoHoje = numContasVecHoje > 0 && valorVecHoje > 0 ? 
                numContasVecHoje + " conta(s) vencendo hoje com total de " + valorVecHoje.ToString("C") : 
                "";

            string textoAVec = numContasAVec > 0 && valorAVec > 0 && enviarVencer ? 
                numContasAVec + " conta(s) a vencer em " + diasApos + " dia(s) com total de " + valorAVec.ToString("C") : 
                "";

            if (string.IsNullOrEmpty(textoVec) && string.IsNullOrEmpty(textoHoje) && string.IsNullOrEmpty(textoAVec))
                return String.Empty;

            return "Prezado " + nomeCli
                     + ",\nviemos atraves deste informar que consta em nosso sistema " + (!string.IsNullOrEmpty(textoVec) ? textoVec : "")
                     + (!string.IsNullOrEmpty(textoHoje) ? (!string.IsNullOrEmpty(textoVec) ? ", " : "") + textoHoje : "")
                     + (!string.IsNullOrEmpty(textoAVec) ? (!string.IsNullOrEmpty(textoVec) || !string.IsNullOrEmpty(textoHoje) ? ", " : "") + textoAVec : "")
                     + ".Caso o pagamento já tenha sido efetuado, favor desconsiderar este e-mail.\n\nAtenciosamente " + loja + ".";
        }

        #endregion
    }
}
