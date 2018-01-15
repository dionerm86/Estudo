using System;
using System.Collections.Generic;
using System.Linq;
using Glass.Data.Model;
using GDA;

namespace Glass.Data.DAL
{
    public sealed class CompraNotaFiscalDAO : BaseDAO<CompraNotaFiscal, CompraNotaFiscalDAO>
    {
        //private CompraNotaFiscalDAO() { }

        public bool PossuiCompra(uint idNf)
        {
            return PossuiCompra(null, (int)idNf);
        }

        public bool PossuiCompra(GDASession session, int idNf)
        {
            return objPersistence.ExecuteSqlQueryCount(session, "select count(*) from compra_nota_fiscal where idNf=" + idNf) > 0;
        }

        public bool PossuiNFe(uint idCompra)
        {
            return objPersistence.ExecuteSqlQueryCount("select count(*) from compra_nota_fiscal where idCompra=" + idCompra) > 0;
        }

        /// <summary>
        /// Retorna o número de parcelas das compras da nota fiscal.
        /// Se houver mais de um valor (ou nenhum), retorna NULL.
        /// </summary>
        /// <param name="idNf"></param>
        /// <returns></returns>
        public int? NumeroParcelasCompras(GDASession session, int idNf)
        {
            var itens = ExecuteMultipleScalar<int>(session, @"select distinct c.numParc from compra c
                inner join compra_nota_fiscal cnf on (c.idCompra=cnf.idCompra)
                where cnf.idNf=" + idNf);

            return itens.Count == 1 && itens[0] > 0 ? itens[0] : (int?)null;
        }

        public string ObtemIdCompras(uint idNf)
        {
            return GetValoresCampo("select idCompra from compra_nota_fiscal where idNf=" + idNf +
                " order by idCompra asc", "idCompra", ", ");
        }

        public List<uint> ObtemLstIdCompras(GDASession session, uint idNf)
        {
            return ExecuteMultipleScalar<uint>(session, "select idCompra from compra_nota_fiscal where idNf=" + idNf);
        }

        public string ObtemIdNfs(uint idCompra)
        {
            return GetValoresCampo("select idNf from compra_nota_fiscal where idCompra=" + idCompra, "idNf");
        }

        public string ObtemNumerosNFe(uint idCompra)
        {
            return GetValoresCampo(@"select nf.numeroNFe from compra_nota_fiscal cnf
                inner join nota_fiscal nf on (cnf.idNf=nf.idNf)
                where cnf.idCompra=" + idCompra + " order by nf.numeroNFe asc", "numeroNFe", ", ");
        }

        public void ApagarPelaNFe(GDASession sessao, uint idNf)
        {
            objPersistence.ExecuteCommand(sessao, "delete from compra_nota_fiscal where idNf=" + idNf);
        }

        /// <summary>
        /// Verifica se as contas a pagar podem ser separadas na finalização da NF-e.
        /// </summary>
        public bool PodeSepararContasPagarFiscaisEReais(uint idNf)
        {
            return PodeSepararContasPagarFiscaisEReais(null, (int)idNf);
        }

        /// <summary>
        /// Verifica se as contas a pagar podem ser separadas na finalização da NF-e.
        /// </summary>
        public bool PodeSepararContasPagarFiscaisEReais(GDASession session, int idNf)
        {
            try
            {
                uint[] idC;
                PodeSepararContasPagarFiscaisEReais(session, (uint)idNf, out idC);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Verifica se as contas a pagar podem ser separadas na finalização da NF-e.
        /// </summary>
        internal void PodeSepararContasPagarFiscaisEReais(uint idNf, out uint[] idsCompra)
        {
            PodeSepararContasPagarFiscaisEReais(null, idNf, out idsCompra);
        }

        /// <summary>
        /// Verifica se as contas a pagar podem ser separadas na finalização da NF-e.
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idNf"></param>
        /// <param name="idsCompra"></param>
        /// <returns></returns>
        internal void PodeSepararContasPagarFiscaisEReais(GDASession sessao, uint idNf, out uint[] idsCompra)
        {
            idsCompra = null;

            // Garante que a NF-e tenha sido gerada a partir de compra
            var comprasNf = objPersistence.LoadData(sessao, "select * from compra_nota_fiscal where idNf=" + idNf).ToArray();
            if (comprasNf.Length == 0)
                throw new Exception("Nota fiscal não foi gerada de compra.");

            string idsString, nomeCampo;

            idsCompra = (from item in comprasNf
                         where item.IdCompra > 0
                         select item.IdCompra).ToArray();

            idsString = String.Join(",", Array.ConvertAll(idsCompra.ToArray(), x => x.ToString()));
            nomeCampo = "idCompra";

            if (String.IsNullOrEmpty(idsString))
                throw new Exception("Há outra nota fiscal finalizada para pelo menos uma compra desta nota fiscal.");

            // Só separa as contas se ainda não houver parcela paga para essas compras
            if (objPersistence.ExecuteSqlQueryCount(sessao, "select count(*) from contas_pagar where " +
                nomeCampo + " in (" + idsString + ") and paga") > 0)
                throw new Exception("Existe pelo menos uma conta paga que seria utilizada na separação.");

            // Verifica se alguma conta a pagar já possui a coluna IDNF preenchida para
            // essas compras
            idsString = GetValoresCampo(sessao, "select idNf from compra_nota_fiscal where " + nomeCampo +
                " in (" + idsString + ")", "idNf");

            if (objPersistence.ExecuteSqlQueryCount(sessao, @"select count(*) from contas_pagar
                where idNf in (" + idsString + ")") > 0)
                throw new Exception("Já houve uma separação de valores para pelo menos uma compra desta nota fiscal.");
        }
    }
}
