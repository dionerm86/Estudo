using System;
using System.Collections.Generic;
using System.Linq;

namespace Glass.PCP.Negocios.Entidades
{
    public class DetalhesReposicaoPeca
    {
        #region Variáveis locais

        private string[] _dadosReposicaoPeca;
        private Glass.Negocios.Entidades.IObtemDescritor _descritor;

        #endregion

        #region Métodos privados

        private string[] ObtemDadosReposicaoPeca()
        {
            if (_dadosReposicaoPeca == null && !String.IsNullOrEmpty(DadosReposicaoPeca))
                _dadosReposicaoPeca = DadosReposicaoPeca.Split('~');

            return _dadosReposicaoPeca ?? new string[6];
        }

        private Glass.Negocios.Entidades.IObtemDescritor ObtemDescritor()
        {
            if (_descritor == null)
                _descritor = Microsoft.Practices.ServiceLocation.ServiceLocator.Current
                    .GetInstance<Glass.Negocios.Entidades.IObtemDescritor>();

            return _descritor;
        }

        #endregion

        public int IdProdPedProducao { get; set; }

        public string NumEtiqueta { get; set; }

        public string DadosReposicaoPeca { get; set; }

        public string FuncRepos { get; set; }

        public string SetorRepos { get; set; }

        public DateTime DataRepos { get; set; }

        public int? IdPedidoExpedicao
        {
            get { return ObtemDadosReposicaoPeca()[0].StrParaIntNullable(); }
        }

        public string Setor
        {
            get { return Glass.Data.Helper.Utils.ObtemSetor(ObtemDadosReposicaoPeca()[1].StrParaUint()).Descricao; }
        }

        public string Obs
        {
            get { return ObtemDadosReposicaoPeca()[2]; }
        }

        public Glass.Data.Model.SituacaoProdutoProducao SituacaoProducao
        {
            get { return (Glass.Data.Model.SituacaoProdutoProducao)ObtemDadosReposicaoPeca()[3].StrParaInt(); }
        }

        public int? IdImpressao
        {
            get { return ObtemDadosReposicaoPeca()[4].StrParaIntNullable(); }
        }

        public string PlanoCorte
        {
            get { return ObtemDadosReposicaoPeca()[5]; }
        }

        public int? PosicaoArqOtimiz
        {
            get { return ObtemDadosReposicaoPeca()[6].StrParaIntNullable(); }
        }

        public string DadosLeituraProducao
        {
            get
            {
                var itens = ObtemDadosReposicaoPeca()
                    .Skip(7)
                    .Where(x => !String.IsNullOrEmpty(x))
                    .Select(x =>
                    {
                        var dados = x.Split('!');
                        var idFunc = dados[0].StrParaInt();
                        var idSetor = dados[1].StrParaInt();
                        var dataLeitura = dados[2].StrParaDate();

                        return new
                        {
                            Func = ObtemDescritor().ObtemDescritor<Global.Negocios.Entidades.Funcionario>(idFunc).Name,
                            Setor = Glass.Data.Helper.Utils.ObtemSetor((uint)idSetor).Descricao,
                            DataLeitura = dataLeitura,
                            planoCorte = idSetor == 1 ? PlanoCorte : string.Empty,
                        };
                    })
                    .OrderBy(x => x.DataLeitura);

                var retorno = new List<string>();

                foreach (var item in itens)
                    retorno.Add(String.Format(
                        "• Marcador: {0}, Setor: {1}{2}, Data leitura: {3:dd/MM/yyyy HH:mm:ss}",
                        item.Func, item.Setor,!string.IsNullOrWhiteSpace(item.planoCorte) ? $",Plano Corte:{ item.planoCorte }" : string.Empty ,item.DataLeitura));

                return String.Join("<br />", retorno);
            }
        }
    }
}
