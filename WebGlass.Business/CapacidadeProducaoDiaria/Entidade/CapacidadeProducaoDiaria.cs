using System;
using System.Collections.Generic;
using Glass.Data.DAL;
using Glass.Data.Helper;
using Microsoft.Practices.ServiceLocation;

namespace WebGlass.Business.CapacidadeProducaoDiaria.Entidade
{
    [Serializable]
    public class CapacidadeProducaoDiaria
    {
        internal Glass.Data.Model.CapacidadeProducaoDiaria _capacidade;
        internal IEnumerable<Glass.Data.Model.CapacidadeProducaoDiariaSetor> _setores;
        internal IEnumerable<Glass.PCP.Negocios.Entidades.CapacidadeDiariaProducaoClassificacao> _classificacoes;
        
        #region Construtores

        public CapacidadeProducaoDiaria()
            : this(new Glass.Data.Model.CapacidadeProducaoDiaria())
        {
        }

        internal CapacidadeProducaoDiaria(Glass.Data.Model.CapacidadeProducaoDiaria model)
        {
            _capacidade = model;
            _setores = model.Data.Ticks != 0 ?
                CapacidadeProducaoDiariaSetorDAO.Instance.ObtemPelaData(model.Data) :
                new Glass.Data.Model.CapacidadeProducaoDiariaSetor[0];
            _classificacoes = model.Data.Ticks != 0 ?
                ServiceLocator.Current.GetInstance<Glass.PCP.Negocios.ICapacidadeProducaoDiariaClassificacaoFluxo>().ObtemPelaData(model.Data) :
                new Glass.PCP.Negocios.Entidades.CapacidadeDiariaProducaoClassificacao[0];
        }

        #endregion

        public uint CodigoParaLog
        {
            get { return _capacidade.IdParaLog; }
        }

        public DateTime Data
        {
            get { return _capacidade.Data; }
            set { _capacidade.Data = value; } 
        }

        public int? MaximoVendasM2
        {
            get { return _capacidade.MaximoVendasM2; }
            set { _capacidade.MaximoVendasM2 = value; }
        }

        public IDictionary<uint, int> CapacidadeSetores
        {
            get
            {
                var dicionario = new Dictionary<uint, int>();

                foreach (var dados in _setores)
                    dicionario.Add(dados.IdSetor, dados.Capacidade);

                return dicionario;
            }
            set
            {
                var lista = new List<Glass.Data.Model.CapacidadeProducaoDiariaSetor>();

                foreach (var dados in value)
                {
                    if (dados.Value != Utils.ObtemSetor(dados.Key).CapacidadeDiaria)
                        lista.Add(new Glass.Data.Model.CapacidadeProducaoDiariaSetor()
                        {
                            Capacidade = dados.Value,
                            Data = this.Data,
                            IdSetor = dados.Key
                        });
                }

                _setores = lista;
            }
        }

        public IDictionary<int, int> CapacidadeClassificacao
        {
            get
            {
                var dicionario = new Dictionary<int, int>();

                foreach (var dados in _classificacoes)
                    dicionario.Add(dados.IdClassificacaoRoteiroProducao, dados.Capacidade);

                return dicionario;
            }
            set
            {
                var lista = new List<Glass.PCP.Negocios.Entidades.CapacidadeDiariaProducaoClassificacao>();
                var instance = ServiceLocator.Current.GetInstance<Glass.PCP.Negocios.IClassificacaoRoteiroProducaoFluxo>();

                foreach (var dados in value)
                {
                    if (dados.Value != instance.ObtemCapacidadeDiaria(dados.Key))
                        lista.Add(new Glass.PCP.Negocios.Entidades.CapacidadeDiariaProducaoClassificacao()
                        {
                            Capacidade = dados.Value,
                            Data = this.Data,
                            IdClassificacaoRoteiroProducao = dados.Key
                        });
                }

                _classificacoes = lista;
            }
        }
    }
}
