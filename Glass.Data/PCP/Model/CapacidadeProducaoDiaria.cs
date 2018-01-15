using System;
using System.Text;
using GDA;
using Glass.Data.DAL;
using Glass.Data.Helper;
using Glass.Log;

namespace Glass.Data.Model
{
    [Serializable,
    PersistenceClass("capacidade_producao_diaria"),
    PersistenceBaseDAO(typeof(CapacidadeProducaoDiariaDAO))]
    public class CapacidadeProducaoDiaria
    {
        internal static readonly DateTime DATA_INICIO = DateTime.Parse("01/01/2013");

        [PersistenceProperty("DATA", PersistenceParameterType.Key)]
        public DateTime Data { get; set; }

        [Log("Máximo Vendas (m²)"),
        PersistenceProperty("MAXIMOVENDASM2")]
        public int? MaximoVendasM2 { get; set; }

        #region Propriedades de suporte

        public uint IdParaLog
        {
            get
            {
                return (uint)(Data.Date - DATA_INICIO).TotalDays;
            }
            internal set
            {
                Data = DATA_INICIO.AddDays(value);
            }
        }

        internal string _dadosSetor;

        [Log("Capacidade Produção Setor")]
        internal string DadosSetor
        {
            get
            {
                if (_dadosSetor == null)
                {
                    var retorno = new StringBuilder();

                    var setores = CapacidadeProducaoDiariaSetorDAO.Instance.ObtemPelaData(Data);
                    foreach (var s in setores)
                        retorno.AppendFormat("{0}: {1}m², ", Utils.ObtemSetor(s.IdSetor).Descricao, s.Capacidade);

                    _dadosSetor = retorno.ToString().TrimEnd(' ', ',');
                }

                return _dadosSetor;
            }
        }

        [Log("Capacidade Produção Classificação")]
        public string DadosClassificacao { get; set; }

        #endregion
    }
}
