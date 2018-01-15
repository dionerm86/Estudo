using System;
using System.Collections.Generic;
using System.Text;
using Glass.Data.Model;
using GDA;

namespace Glass.Data.DAL
{
    public sealed class ImpostoNcmDAO : BaseDAO<ImpostoNcm, ImpostoNcmDAO>
    {
        //private ImpostoNcmDAO() { }

        private static object syncRoot;

        static ImpostoNcmDAO()
        {
            syncRoot = new object();
        }

        public class DadosImposto
        {
            public float AliquotaImposto { get; set; }
            public decimal ValorImposto { get; set; }
        }

        public DadosImposto ObtemDadosImpostos(string ncm, string cst, decimal valorFiscal)
        {
            var dadosImposto = ObtemPorNcm(ncm);

            float aliquota = "0345".Contains(cst[0].ToString()) ? dadosImposto.AliquotaNacional : dadosImposto.AliquotaImportacao;
            decimal valor = valorFiscal * (decimal)(aliquota / 100);

            return new DadosImposto()
            {
                AliquotaImposto = aliquota,
                ValorImposto = valor
            };
        }

        public DadosImposto ObtemDadosImpostos(IEnumerable<ProdutosNf> prodNf)
        {
            decimal impostoTotal = 0, valorTotal = 0;

            List<string> ncm = new List<string>();
            foreach (var d in prodNf)
                if (!ncm.Contains(d.Ncm))
                    ncm.Add(d.Ncm);

            AtualizaPorNcm(ncm.ToArray());

            foreach (var d in prodNf)
            {
                var item = ObtemDadosImpostos(d.Ncm, d.CstOrig + d.Cst, d.Total);
                impostoTotal += item.ValorImposto;
                valorTotal += d.Total;
            }

            return new DadosImposto()
            {
                AliquotaImposto = valorTotal == 0 ? 0 : (float)(impostoTotal / valorTotal),
                ValorImposto = impostoTotal
            };
        }

        public ImpostoNcm ObtemPorNcm(string ncm)
        {
            AtualizaPorNcm(ncm);

            lock (syncRoot)
            {
                var itens = objPersistence.LoadData("select * from imposto_ncm where ncm=?ncm",
                    new GDAParameter("?ncm", ncm)).ToList();

                if (itens.Count == 0)
                {
                    itens.Add(new ImpostoNcm()
                    {
                        Ncm = ncm
                    });
                }

                return itens[0];
            }
        }

        public void AtualizaPorNcm(params string[] ncm)
        {
            if (ncm == null || ncm.Length == 0)
                return;

            lock (syncRoot)
            {
                List<string> naoBuscar = ExecuteMultipleScalar<string>("select ncm from imposto_ncm where ncm in ('" + 
                    String.Join("','", ncm) + "')");

                List<string> buscar = new List<string>(ncm);
                foreach (string nao in naoBuscar)
                    buscar.Remove(nao);

                if (buscar.Count == 0)
                    return;
            
                var retorno = new WebGlass.WebService.ImpostoTotal.ImpostoTotal().ObtemTabelaDados(buscar.ToArray());

                foreach (var item in retorno)
                {
                    Insert(new ImpostoNcm()
                    {
                        Ncm = item.Ncm,
                        AliquotaNacional = item.AliquotaNacional,
                        AliquotaImportacao = item.AliquotaImportacao
                    });
                }
            }
        }
    }
}
