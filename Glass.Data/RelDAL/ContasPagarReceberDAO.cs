using System;
using System.Collections.Generic;
using System.Linq;
using Glass.Data.RelModel;
using Glass.Data.DAL;
using Glass.Data.Model;

namespace Glass.Data.RelDAL
{
    public sealed class ContasPagarReceberDAO : BaseDAO<ContasPagarReceber, ContasPagarReceberDAO>
    {
        //private ContasPagarReceberDAO() { }

        #region Busca contas a pagar / receber

        public ContasPagarReceber[] GetContasPagarReceber(uint idCli, string nomeCli, uint idFornec, string nomeFornec,
            string dtVecIni, string dtVecFim, float valorIni, float valorFim, string sortExpression, int startRow, int pageSize)
        {
            pageSize = pageSize / 2;
            startRow = startRow > 0 ? startRow / 2 : startRow;

            var contasReceber = ContasReceberDAO.Instance.GetNaoRecebidas(0, 0, 0, 0, 0, 0, 0, false, idCli, 0, 0, nomeCli, dtVecIni, dtVecFim, null,
                null, null, null, valorIni, valorFim, 0, 0, true, 0, false, 0, 0, 2, null, null, null, null, 0, true, 0, 0, false, 0,
                sortExpression, startRow, pageSize).ToArray();

            var contasPagar = ContasPagarDAO.Instance.GetPagtos(0, 0, null, 0, 0, 0, idFornec, nomeFornec, dtVecIni, dtVecFim, null, null, new uint[] { },
                valorIni, valorFim, false, 0, false, false, null, 0, false, true, null, null, null, null, 0, 0, null, 0, 0,
                sortExpression, startRow, pageSize).ToArray();

            if (!(idCli > 0) && idFornec > 0)
            {
                contasReceber = new ContasReceber[0];
            }

            else if (!(idFornec > 0) && idCli > 0)
            {
                contasPagar = new ContasPagar[0];
            }

            else if (idCli > 0 && idFornec > 0)
                return new ContasPagarReceber[0];

            var contasPagarReceber = new List<ContasPagarReceber>();

            foreach (ContasPagar cp in contasPagar)
            {
                contasPagarReceber.Add(new ContasPagarReceber()
                {
                    IdContaPagRec = cp.IdContaPg,
                    IdCliFornec = cp.IdFornec.GetValueOrDefault(),
                    NomeCliFornec = cp.NomeFornec,
                    DataVenc = cp.DataVenc,
                    ValorVencPag = cp.ValorVenc,
                    Referencia = cp.Referencia,
                    TipoConta = TipoContaPagarReceber.Pagar
                });
            }

            foreach (ContasReceber cr in contasReceber)
            {
                contasPagarReceber.Add(new ContasPagarReceber()
                {
                    IdContaPagRec = cr.IdContaR,
                    IdCliFornec = cr.IdCliente,
                    NomeCliFornec = cr.NomeCli,
                    DataVenc = cr.DataVec,
                    ValorVencRec = cr.ValorVec,
                    Referencia = cr.Referencia,
                    TipoConta = TipoContaPagarReceber.Receber
                });
            }

            contasPagarReceber = contasPagarReceber
                .OrderByDescending(c => c.DataVenc)
                .ThenBy(c => c.NomeCliFornec).ToList();

            return contasPagarReceber.ToArray();
        }

        public int GetContasPagarReceberCount(uint idCli, string nomeCli, uint idFornec, string nomeFornec,
            string dtVecIni, string dtVecFim, float valorIni, float valorFim)
        {
            int countReceber = ContasReceberDAO.Instance.GetNaoRecebidasCount(0, 0, 0, 0, 0, 0, 0, false, idCli, 0, 0, nomeCli, dtVecIni, dtVecFim, null,
                null, null, null, valorIni, valorFim, 0, 0, true, 0, false, 0, 0, 2, null, null, null, null, 0, true, false, 0, 0, 0);

            int countPagar = ContasPagarDAO.Instance.GetPagtosCount(0, 0, null, 0, 0, 0, idFornec, nomeFornec, dtVecIni, dtVecFim, null, null, new uint[] { },
                valorIni, valorFim, false, 0, false, false, null, 0, false, true, null, null, null, null, 0, 0, null, 0, 0);

            return countReceber + countPagar;
        }

        public ContasPagarReceber[] GetContasPagarReceberRpt(uint idCli, string nomeCli, uint idFornec, string nomeFornec,
           string dtVecIni, string dtVecFim, float valorIni, float valorFim)
        {
            string criterio = "";

            if (idCli > 0)
                criterio += "Cliente: " + ClienteDAO.Instance.GetNome(idCli) + "    ";
            else if (!String.IsNullOrEmpty(nomeCli))
                criterio += "Cliente: " + nomeCli + "    ";

            if (idFornec > 0)
                criterio += "Fornecedor: " + FornecedorDAO.Instance.GetNome(idCli) + "    ";
            else if (!String.IsNullOrEmpty(nomeCli))
                criterio += "Fornecedor: " + nomeFornec + "    ";

            if (!String.IsNullOrEmpty(dtVecIni))
                criterio += "Data venc. início: " + dtVecIni + "    ";

            if (!String.IsNullOrEmpty(dtVecFim))
                criterio += "Data venc. fim: " + dtVecFim + "    ";

            if (valorIni > 0)
                criterio += valorFim > 0 ? "Valor Boleto: " + valorIni + " até " + valorFim + "    " : "Valor Boleto: a partir de " + valorIni + "    ";

            if (valorFim > 0)
                criterio += valorIni > 0 ? "" : "Valor Boleto: até " + valorFim + "    ";

            var contasReceber = ContasReceberDAO.Instance.GetNaoRecebidasRpt(0, 0, 0, 0, 0, 0, 0, false, idCli, 0, 0, nomeCli, dtVecIni, dtVecFim, null,
                null, null, null, valorIni, valorFim, 0, 0, true, 0, false, 0, null, 0, 2, null, null, null, null, 0, true, false, 0, 0, 0).ToArray();

            var lstPrevisaoPg = new decimal[4];
            var contasPagar = ContasPagarDAO.Instance.GetPagtosForRpt(0, 0, null, 0, 0, 0, idFornec, nomeFornec, dtVecIni, dtVecFim, null, null, "",
                valorIni, valorFim, 0, true, true, false, false, null, 0, false, null, ref lstPrevisaoPg, true, null, null, null, null,
                0, 0, null, 0, 0).ToArray();

            var contasPagarReceber = new List<ContasPagarReceber>();

            foreach (ContasPagar cp in contasPagar)
            {
                contasPagarReceber.Add(new ContasPagarReceber()
                {
                    IdContaPagRec = cp.IdContaPg,
                    IdCliFornec = cp.IdFornec.GetValueOrDefault(),
                    NomeCliFornec = cp.NomeFornec,
                    DataVenc = cp.DataVenc,
                    ValorVencPag = cp.ValorVenc,
                    Referencia = cp.Referencia,
                    TipoConta = TipoContaPagarReceber.Pagar,
                    Criterio = criterio
                });
            }

            foreach (ContasReceber cr in contasReceber)
            {
                contasPagarReceber.Add(new ContasPagarReceber()
                {
                    IdContaPagRec = cr.IdContaR,
                    IdCliFornec = cr.IdCliente,
                    NomeCliFornec = cr.NomeCli,
                    DataVenc = cr.DataVec,
                    ValorVencRec = cr.ValorVec,
                    Referencia = cr.Referencia,
                    TipoConta = TipoContaPagarReceber.Receber,
                    Criterio = criterio
                });
            }

            contasPagarReceber = contasPagarReceber
                .OrderByDescending(c => c.DataVenc)
                .ThenBy(c => c.NomeCliFornec).ToList();

            return contasPagarReceber.ToArray();
        }

        #endregion
    }
}
