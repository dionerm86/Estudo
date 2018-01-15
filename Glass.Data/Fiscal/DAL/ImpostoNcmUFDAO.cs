using System;
using System.Collections.Generic;
using Glass.Data.Model;
using GDA;
using System.Linq;

namespace Glass.Data.DAL
{
    public sealed class ImpostoNcmUFDAO : BaseDAO<ImpostoNcmUF, ImpostoNcmUFDAO>
    {
        //private ImpostoNcmDAO() { }

        private static object syncRoot;

        static ImpostoNcmUFDAO()
        {
            syncRoot = new object();
        }

        public class DadosImposto
        {
            public float AliquotaNacional { get; set; }
            public float AliquotaEstadual { get; set; }
            public string Fonte { get; set; }
            public decimal ValorImpostoNacional { get; set; }
            public decimal ValorImpostoEstadual { get; set; }
        }

        public DadosImposto ObtemDadosImpostos(string ncm, string cst, decimal valorFiscal)
        {
            var dadosImposto = ObtemPorNCMUF(ncm, Glass.Data.Helper.UserInfo.GetUserInfo.UfLoja);

            var aliquotaNacional = "0345".Contains(cst[0].ToString()) ? dadosImposto.AliquotaNacional : dadosImposto.AliquotaImportacao;
            var valorNacional = valorFiscal * (decimal)(aliquotaNacional / 100);
            var valorEstadual = valorFiscal * (decimal)(dadosImposto.AliquotaEstadual / 100);

            return new DadosImposto()
            {
                AliquotaNacional = aliquotaNacional,
                AliquotaEstadual = dadosImposto.AliquotaEstadual,
                Fonte = dadosImposto.Fonte,
                ValorImpostoNacional = valorNacional,
                ValorImpostoEstadual = valorEstadual
            };
        }

        public DadosImposto ObtemDadosImpostos(IEnumerable<ProdutosNf> prodNf)
        {
            decimal impostoNacionalTotal = 0, valorTotal = 0, impostoEstadualTotal = 0;

            List<string> ncm = new List<string>();
            foreach (var d in prodNf)
                if (!ncm.Contains(d.Ncm))
                    ncm.Add(d.Ncm);

            AtualizaPorNcm(ncm.ToArray());

            var fonte = string.Empty;

            foreach (var d in prodNf)
            {
                var item = ObtemDadosImpostos(d.Ncm, d.CstOrig + d.Cst, d.Total);
                impostoNacionalTotal += item.ValorImpostoNacional;
                impostoEstadualTotal += item.ValorImpostoEstadual;
                valorTotal += d.Total;

                fonte = item.Fonte;
            }

            return new DadosImposto()
            {
                AliquotaNacional = valorTotal == 0 ? 0 : (float)(impostoNacionalTotal / valorTotal),
                ValorImpostoNacional = impostoNacionalTotal,
                ValorImpostoEstadual = impostoEstadualTotal,
                Fonte = fonte
            };
        }

        public ImpostoNcmUF ObtemPorNCMUF(string ncm, string uf)
        {
            AtualizaPorNcm(ncm);

            lock (syncRoot)
            {
                var itens = objPersistence.LoadData("select * from imposto_ncm_uf where ncm=?ncm AND uf=?uf",
                    new GDAParameter("?ncm", ncm),
                    new GDAParameter("?uf", uf)).ToList();

                if (itens.Count == 0)
                {
                    itens.Add(new ImpostoNcmUF()
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
                var naoBuscar = new GDA.Sql.Query(string.Format("ncm in ('{0}')", String.Join("','", ncm))).ToList<ImpostoNcmUF>();

                var buscar = new List<string>(ncm);
                foreach (string nao in naoBuscar.Select(f=> f.Ncm))
                    buscar.Remove(nao);

                if (buscar.Count == 0)
                    return;

                try
                {
                    var instancia = new WebGlass.WebService.ImpostoTotal.ImpostoTotal();

                    //var naoBuscar = ExecuteMultipleScalar<string>("select ncm from imposto_ncm where ncm in ('" + 
                    //    String.Join("','", ncm) + "')");

                    var ncmAtualizar = naoBuscar.Where(f => f.VigenciaFim.GetValueOrDefault() < DateTime.Now);

                    var versao = instancia.ObtemVersaoNCM(Glass.Data.Helper.UserInfo.GetUserInfo.UfLoja).Versao;

                    var retorno = new WebGlass.WebService.ImpostoTotal.ImpostoTotal()
                        .ObtemTabelaDadosNCMUf(buscar.ToArray(), Glass.Data.Helper.UserInfo.GetUserInfo.UfLoja);

                    foreach (var item in retorno)
                    {
                        Insert
                            (new ImpostoNcmUF()
                            {
                                Ncm = item.Ncm,
                                AliquotaNacional = item.AliquotaNacional,
                                AliquotaImportacao = item.AliquotaImportacao,
                                AliquotaEstadual = item.AliquotaEstadual,
                                AliquotaMunicipal = item.AliquotaMunicipal,
                                Versao = versao,
                                VigenciaInicio = item.VigenciaInicio,
                                VigenciaFim = item.VigenciaFim,
                                UF = item.UF,
                                Fonte = item.Fonte
                            });
                    }

                    foreach (var item in ncmAtualizar)
                    {
                        item.Versao = versao;
                        ImpostoNcmUFDAO.Instance.Update(item);
                    }
                }
                catch
                {
                    // ignored
                }
            }
        }
    }
}
