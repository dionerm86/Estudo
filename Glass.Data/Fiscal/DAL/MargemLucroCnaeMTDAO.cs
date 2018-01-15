using GDA;
using System;
using System.Collections.Generic;

namespace Glass.Data.DAL
{
    public sealed class MargemLucroCnaeMTDAO : BaseDAO<Model.MargemLucroCnaeMT, MargemLucroCnaeMTDAO>
    {
        private static object syncRoot;

        static MargemLucroCnaeMTDAO()
        {
            syncRoot = new object();
        }

        public float ObtemDadosMargemLucro(GDASession sessao, string cnae)
        {
            var dadosImposto = ObtemPorCnae(sessao, cnae);
            return dadosImposto.MargemLucro;
        }

        public Model.MargemLucroCnaeMT ObtemPorCnae(GDASession sessao, string cnae)
        {
            AtualizaPorCnae(sessao, cnae);

            lock (syncRoot)
            {
                var itens = objPersistence.LoadData(sessao, "select * from margem_lucro_cnae_mt where Replace(Replace(cnae,'-',''),?barra,'')=?cnae",
                    new GDAParameter("?barra", "/"), new GDAParameter("?cnae", cnae.Replace("-", "").Replace("/", ""))).ToList();

                if (itens.Count == 0)
                {
                    itens.Add(new Model.MargemLucroCnaeMT()
                    {
                        Cnae = cnae
                    });
                }

                return itens[0];
            }
        }

        public void AtualizaPorCnae(GDASession sessao, params string[] cnae)
        {
            if (cnae == null || cnae.Length == 0)
                return;

            lock (syncRoot)
            {
                List<string> naoBuscar = ExecuteMultipleScalar<string>(sessao, "select cnae from margem_lucro_cnae_mt where Replace(Replace(cnae,'-',''),?barra,'') in ('" +
                    string.Join("','", cnae).Replace("-", "").Replace("/", "") + "')", new GDAParameter("?barra", "/"));

                List<string> buscar = new List<string>(cnae);
                foreach (string nao in naoBuscar)
                    buscar.Remove(nao);

                if (buscar.Count == 0)
                    return;

                var retorno = new WebGlass.WebService.MargemLucro.MargemLucro().ObtemTabelaDados(buscar.ToArray());

                foreach (var item in retorno)
                {
                    Insert(new Model.MargemLucroCnaeMT()
                    {
                        Cnae = item.Cnae,
                        MargemLucro = item.MargemLucro
                    });
                }
            }
        }
    }
}
