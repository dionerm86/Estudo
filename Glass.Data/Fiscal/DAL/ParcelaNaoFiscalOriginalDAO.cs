using System;
using System.Linq;
using Glass.Data.Model;
using GDA;

namespace Glass.Data.DAL
{
    public sealed class ParcelaNaoFiscalOriginalDAO : BaseDAO<ParcelaNaoFiscalOriginal, ParcelaNaoFiscalOriginalDAO>
    {
        //private ParcelaNaoFiscalOriginalDAO() { }

        public void DeleteByIdsContasR(uint[] idsContasR)
        {
            DeleteByIdsContasR(null, idsContasR);
        }

        public void DeleteByIdsContasR(GDASession sessao, uint[] idsContasR)
        {
            if (idsContasR == null || idsContasR.Length == 0)
                return;

            string ids = String.Join(",", Array.ConvertAll(idsContasR, x => x.ToString()));
            objPersistence.ExecuteCommand(sessao, "delete from parcela_naofiscal_original where idContaR in (" + ids + ")");
        }

        public void DeleteByIdsContasPg(uint[] idsContasPg)
        {
            DeleteByIdsContasPg(null, idsContasPg);
        }

        public void DeleteByIdsContasPg(GDASession sessao, uint[] idsContasPg)
        {
            if (idsContasPg == null || idsContasPg.Length == 0)
                return;

            string ids = String.Join(",", Array.ConvertAll(idsContasPg, x => x.ToString()));
            objPersistence.ExecuteCommand(sessao, "delete from parcela_naofiscal_original where idContaPg in (" + ids + ")");
        }

        public void InsertContasReceber(uint idNf, ContasReceber[] contasReceber)
        {
            InsertContasReceber(null, idNf, contasReceber);
        }

        public void InsertContasReceber(GDASession sessao, uint idNf, ContasReceber[] contasReceber)
        {
            foreach (var c in contasReceber)
            {
                /* Chamado 25258. */
                if (c.IdLiberarPedido.GetValueOrDefault() > 0 &&
                    PedidosNotaFiscalDAO.Instance.GetByLiberacaoPedido(sessao, c.IdLiberarPedido.Value)
                        .Count(f => f.IdNf == idNf) == 0)
                    throw new Exception(
                        string.Format(@"A liberação {0} não está associada à nota fiscal de ID {1},
                            refaça a separação de valores, caso o erro persista entre em contato com o suporte.",
                            c.IdLiberarPedido.Value, idNf));

                var p = new ParcelaNaoFiscalOriginal()
                {
                    IdNf = idNf,
                    IdLoja = c.IdLoja,
                    IdContaR = c.IdContaR,
                    IdPedido = c.IdPedido,
                    IdLiberarPedido = c.IdLiberarPedido,
                    IdConta = c.IdConta.GetValueOrDefault(),
                    DataVec = c.DataVec,
                    ValorVec = c.ValorVec,
                    NumParc = c.NumParc,
                    NumParcMax = c.NumParcMax,
                    DataCad = c.DataCad,
                    UsuCad = c.Usucad,
                    TipoConta = c.TipoConta
                };

                Insert(sessao, p);
            }
        }

        public void InsertContasPagar(uint idNf, ContasPagar[] contasPagar)
        {
            InsertContasPagar(null, idNf, contasPagar);
        }

        public void InsertContasPagar(GDASession sessao, uint idNf, ContasPagar[] contasPagar)
        {
            foreach (ContasPagar c in contasPagar)
            {
                ParcelaNaoFiscalOriginal p = new ParcelaNaoFiscalOriginal()
                {
                    IdNf = idNf,
                    IdLoja = c.IdLoja.GetValueOrDefault(),
                    IdContaPg = c.IdContaPg,
                    IdCompra = c.IdCompra,
                    IdConta = c.IdConta.GetValueOrDefault(),
                    DataVec = c.DataVenc,
                    ValorVec = c.ValorVenc,
                    NumParc = c.NumParc,
                    NumParcMax = c.NumParcMax,
                    DataCad = c.DataCad,
                    UsuCad = (uint)c.Usucad,
                    TipoConta = (byte)(c.Contabil ? 1 : 0)
                };

                Insert(sessao, p);
            }
        }

        public void DeleteByIdNf(GDASession session, uint idNf)
        {
            objPersistence.ExecuteCommand(session, "delete from parcela_naofiscal_original where idNf=" + idNf);
        }

        public ParcelaNaoFiscalOriginal[] GetByNf(GDASession session, uint idNf)
        {
            return objPersistence.LoadData(session, "select * from parcela_naofiscal_original where idNf=" + idNf).ToArray();
        }

        public bool NfTemParcela(GDASession sessao, uint idNf)
        {
            return objPersistence.ExecuteSqlQueryCount(sessao, "select count(*) from parcela_naofiscal_original where idNf=" + idNf) > 0;
        }
    }
}
