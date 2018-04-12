using System;
using System.Globalization;
using GDA;
using Glass.Data.Model;
using System.Linq;
using System.Collections.Generic;

namespace Glass.Data.DAL
{
    public sealed class ChequesAcertoDAO : BaseCadastroDAO<ChequesAcerto, ChequesAcertoDAO>
    {
        /// <summary>
        /// Cria um cheque, na tabela cheques_acerto, a partir de uma string.
        /// </summary>
        public void InserirPelaString(GDASession sessao, Acerto acerto, IEnumerable<string> dadosChequesRecebimento)
        {
            if ((dadosChequesRecebimento?.Where(f => !string.IsNullOrWhiteSpace(f)).Count()).GetValueOrDefault() == 0)
            {
                return;
            }

            foreach (var dadosChequeRecebimento in dadosChequesRecebimento.Where(f => !string.IsNullOrWhiteSpace(f)))
            {
                // Divide o cheque para pegar suas propriedades.
                var propriedadesCheque = dadosChequeRecebimento.Split('\t');
                var tipoCheque = ChequesDAO.Instance.GetTipo(propriedadesCheque[0]);

                // Insere cheque no BD.
                Insert(sessao, new ChequesAcerto
                {
                    IdAcerto = (int)acerto.IdAcerto,
                    IdLoja = propriedadesCheque[22].StrParaInt(),
                    // Recupera a conta banc�ria atrav�s da ag�ncia e conta do cheque, caso a conta banc�ria n�o tenha sido informada e o tipo do cheque seja Pr�prio.
                    IdContaBanco = tipoCheque == 1 && propriedadesCheque[1].StrParaIntNullable().GetValueOrDefault() == 0 ?
                        (int?)ContaBancoDAO.Instance.GetIdByAgenciaConta(propriedadesCheque[16], propriedadesCheque[17]) : propriedadesCheque[1].StrParaIntNullable(),
                    CpfCnpj = propriedadesCheque[21].Replace(".", string.Empty).Replace("-", string.Empty).Replace("/", string.Empty),
                    Num = propriedadesCheque[2].StrParaInt(),
                    DigitoNum = propriedadesCheque[3],
                    Banco = propriedadesCheque[15],
                    Agencia = propriedadesCheque[16],
                    Conta = propriedadesCheque[17],
                    Titular = propriedadesCheque[4],
                    Valor = decimal.Parse(propriedadesCheque[5], NumberStyles.AllowDecimalPoint),
                    DataVenc = DateTime.Parse(propriedadesCheque[6]),
                    Origem = propriedadesCheque[8].StrParaInt(),
                    Tipo = tipoCheque,
                    Obs = propriedadesCheque[19]
                });
            }
        }

        /// <summary>
        /// Recupera a string dos cheques, com base no acerto e com base nos dados inseridos na tabela cheques_acerto.
        /// </summary>
        public string ObterStringChequesPeloAcerto(GDASession sessao, int idAcerto)
        {
            var chequesAcerto = objPersistence.LoadData(sessao, string.Format("SELECT * FROM cheques_acerto WHERE IdAcerto={0}", idAcerto)).ToList();

            if (chequesAcerto == null || chequesAcerto.Count == 0)
            {
                return string.Empty;
            }

            return string.Join("|", chequesAcerto.Select(f =>
                string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\t{9}\t{10}\t{11}\t{12}\t{13}\t{14}\t{15}\t{16}\t{17}\t{18}\t{19}\t{20}\t{21}\t{22}",
                    // Posi��o 0.
                    f.Tipo == 1 ? "Pr�prio" : "Terceiro",
                    // Posi��o 1.
                    f.IdContaBanco,
                    // Posi��o 2.
                    f.Num,
                    // Posi��o 3.
                    f.DigitoNum,
                    // Posi��o 4.
                    f.Titular,
                    // Posi��o 5.
                    f.Valor,
                    // Posi��o 6.
                    f.DataVenc,
                    // Posi��o 7 (Situacao).
                    string.Empty,
                    // Posi��o 8.
                    f.Origem,
                    // Posi��o 9 (IdAcertoCheque).
                    string.Empty,
                    // Posi��o 10 (IdContaR).
                    string.Empty,
                    // Posi��o 11 (IdPedido).
                    string.Empty,
                    // Posi��o 12 (IdAcerto).
                    idAcerto,
                    // Posi��o 13 (IdLiberarPedido).
                    string.Empty,
                    // Posi��o 14 (IdTrocaDevolucao).
                    string.Empty,
                    // Posi��o 15.
                    f.Banco,
                    // Posi��o 16.
                    f.Agencia,
                    // Posi��o 17.
                    f.Conta,
                    // Posi��o 18 (IdCheque).
                    string.Empty,
                    // Posi��o 19.
                    f.Obs,
                    // Posi��o 20 (IdSinal).
                    string.Empty,
                    // Posi��o 21.
                    f.CpfCnpj,
                    // Posi��o 22.
                    f.IdLoja)).ToList());
        }
    }
}