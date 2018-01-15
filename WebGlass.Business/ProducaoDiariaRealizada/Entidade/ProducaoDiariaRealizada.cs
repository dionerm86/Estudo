using System;
using System.Text;
using Glass.Data.Helper;
using Glass.Data.DAL;
using Microsoft.Practices.ServiceLocation;

namespace WebGlass.Business.ProducaoDiariaRealizada.Entidade
{
    public class ProducaoDiariaRealizada
    {
        #region Variaveis Locais

        private Glass.Data.RelModel.ProducaoDiariaRealizada _producaoDiaria;
        private Glass.Data.RelModel.PrevisaoProducao _previsaoProducao;
        private DateTime _dataProducao;
        private string _nomeSetor;
        private string _nomeClassificacao;
        private decimal _capacidadeProducao;

        #endregion

        #region Construtores

        internal ProducaoDiariaRealizada(Glass.Data.RelModel.ProducaoDiariaRealizada model, DateTime dataProducao)
        {
            _producaoDiaria = model;
            _dataProducao = dataProducao;
            _capacidadeProducao = CapacidadeProducaoDiariaSetorDAO.Instance.CapacidadeSetorPelaData(dataProducao, model.IdSetor);
        }

        internal ProducaoDiariaRealizada(Glass.Data.RelModel.PrevisaoProducao model)
        {
            _previsaoProducao = model;
            _dataProducao = model.DataFabrica;
            _capacidadeProducao = model.IdClassificacaoRoteiroProducao > 0 ? 
                ServiceLocator.Current.GetInstance<Glass.PCP.Negocios.ICapacidadeProducaoDiariaClassificacaoFluxo>().ObtemPelaData(model.IdClassificacaoRoteiroProducao, model.DataFabrica) :
                CapacidadeProducaoDiariaSetorDAO.Instance.CapacidadeSetorPelaData(model.DataFabrica, model.IdSetor);
        }

        #endregion

        #region Propiedades

        public uint CodigoSetor
        {
            get { return _producaoDiaria != null ? _producaoDiaria.IdSetor : _previsaoProducao.IdSetor; }
        }

        public string NomeSetor
        {
            get
            {
                if (string.IsNullOrEmpty(_nomeSetor))
                    _nomeSetor = Utils.ObtemSetor(CodigoSetor).Descricao;

                return _nomeSetor;
            }
        }

        public int CodClassificacao
        {
            get { return _previsaoProducao != null ? _previsaoProducao.IdClassificacaoRoteiroProducao : 0; }
        }

        public string NomeClassificacao
        {
            get
            {
                if (string.IsNullOrEmpty(_nomeClassificacao))
                    _nomeClassificacao = ServiceLocator.Current.GetInstance<Glass.PCP.Negocios.IClassificacaoRoteiroProducaoFluxo>().ObtemDescricao(CodClassificacao);

                return _nomeClassificacao;
            }
        }

        public decimal Capacidade
        {
            get { return _capacidadeProducao; }
        }

        public decimal M2Previsto
        {
            get { return _producaoDiaria != null ? _producaoDiaria.TotMPrevisto : (decimal)_previsaoProducao.TotM; }
        }

        public decimal M2Realizado
        {
            get { return _producaoDiaria.TotMRealizado; }
        }

        public byte[] ImagemGrafico { get; private set; }

        public void AlteraImagemGrafico(string base64)
        {
            var b = new StringBuilder(base64.Split(',')[1]);
            b.Replace("\r\n", String.Empty);
            b.Replace(" ", String.Empty);

            ImagemGrafico = Convert.FromBase64String(b.ToString());
        }

        public DateTime DataProducao { get { return _dataProducao; } }

        public string DataProducaoStr { get { return DataProducao.ToShortDateString(); } }

        #endregion
    }
}
