using System;
using System.Collections.Generic;
using GDA;
using Glass.Data.Model;
using Glass.Data.RelDAL;
using Glass.Data.Helper;

namespace Glass.Data.RelModel
{
    [PersistenceBaseDAO(typeof(ProducaoSituacaoDataDAO))]
    [PersistenceClass("producao_situacao_data")]
    public class ProducaoSituacaoData
    {
        #region Propriedades

        [PersistenceProperty("Data")]
        public DateTime Data { get; set; }

        [PersistenceProperty("IdPedido")]
        public uint IdPedido { get; set; }

        [PersistenceProperty("DataConf")]
        public DateTime? DataConf { get; set; }

        [PersistenceProperty("DataLiberacao")]
        public DateTime? DataLiberacao { get; set; }

        [PersistenceProperty("Valor")]
        public decimal Valor { get; set; }

        [PersistenceProperty("TotM2")]
        public double TotM2 { get; set; }

        [PersistenceProperty("Criterio")]
        public string Criterio { get; set; }

        [PersistenceProperty("NomesSetores")]
        public string NomesSetores { get; set; }

        [PersistenceProperty("DatasSetores")]
        public string DatasSetores { get; set; }

        [PersistenceProperty("IdsSetores")]
        public string IdsSetores { get; set; }

        #endregion

        #region Propriedades de Suporte

        #region Métodos Privados

        private static string FormatTimeSpan(TimeSpan tempo)
        {
            string retorno = Math.Abs((int)tempo.TotalHours).ToString("0#") + ":";
            retorno += Math.Abs((int)Math.Round((tempo.TotalHours - (int)tempo.TotalHours) * 60, 0)).ToString("0#");

            return (tempo.TotalMilliseconds < 0 ? "-" : "") + retorno;
        }

        #endregion

        public uint?[] IdSetor
        {
            get
            {
                if (string.IsNullOrEmpty(IdsSetores))
                    return null;

                string[] ids = IdsSetores.Split(',');
                uint?[] retorno = new uint?[ids.Length];

                for (int i = 0; i < retorno.Length; i++)
                {
                    uint id;
                    retorno[i] = uint.TryParse(ids[i], out id) ? (uint?)id : null;
                }

                return retorno;
            }
        }

        private string[] _nomeSetor = null;

        public string[] NomeSetor
        {
            get 
            {
                if (_nomeSetor == null)
                {
                    string[] nomes = NomesSetores.Split(',');
                    List<KeyValuePair<int, string>> lista = new List<KeyValuePair<int, string>>();

                    for (int i = 0; i < nomes.Length; i++)
                    {
                        uint? idSetor = IdSetor == null ? null : IdSetor[i];
                        int numSeq = 999;
                        if (idSetor != null)
                        {
                            Setor setor = Utils.ObtemSetor(idSetor.Value);
                            if (setor != null) numSeq = setor.NumeroSequencia;
                        }
                        lista.Add(new KeyValuePair<int, string>(numSeq, nomes[i]));
                    }

                    lista.Sort(new Comparison<KeyValuePair<int, string>>(delegate(KeyValuePair<int, string> x, KeyValuePair<int, string> y)
                    {
                        return Comparer<int>.Default.Compare(x.Key, y.Key);

                    }));

                    _nomeSetor = new string[nomes.Length];
                    for (int i = 0; i < nomes.Length; i++)
                        _nomeSetor[i] = lista[i].Value;
                }

                return _nomeSetor;
            }
        }

        private DateTime?[] _dataSetor = null;

        public DateTime?[] DataSetor
        {
            get
            {
                if (_dataSetor == null && !string.IsNullOrEmpty(DatasSetores))
                {
                    string[] datas = DatasSetores.Split(',');
                    List<KeyValuePair<int, DateTime?>> lista = new List<KeyValuePair<int, DateTime?>>();

                    for (int i = 0; i < datas.Length; i++)
                    {
                        DateTime data;
                        Setor setor = IdSetor[i] != null ? Utils.ObtemSetor(IdSetor[i].Value) : null;
                        int numSeq = setor != null ? setor.NumeroSequencia : 999;
                        lista.Add(new KeyValuePair<int, DateTime?>(numSeq, DateTime.TryParse(datas[i], out data) ? (DateTime?)data : null));
                    }

                    lista.Sort(new Comparison<KeyValuePair<int, DateTime?>>(delegate(KeyValuePair<int, DateTime?> x, KeyValuePair<int, DateTime?> y)
                    {
                        return Comparer<int>.Default.Compare(x.Key, y.Key);

                    }));

                    _dataSetor = new DateTime?[datas.Length];
                    for (int i = 0; i < datas.Length; i++)
                        _dataSetor[i] = lista[i].Value;
                }

                return _dataSetor;
            }
        }

        private TimeSpan[] _tempoSetor = null;

        public TimeSpan[] TempoSetor
        {
            get
            {
                if (_tempoSetor == null && DataSetor != null)
                {
                    _tempoSetor = new TimeSpan[DataSetor.Length];

                    if (DataSetor[0] != null)
                    {
                        DateTime tempoComparacao = DataSetor[0].Value;

                        for (int i = 1; i < _tempoSetor.Length; i++)
                            if (DataSetor[i] != null)
                            {
                                _tempoSetor[i] = DataSetor[i].Value - tempoComparacao;
                                tempoComparacao = DataSetor[i].Value;
                            }
                    }
                }

                return _tempoSetor;
            }
        }

        private bool _relatorio = false;

        public bool Relatorio
        {
            get { return _relatorio; }
            set { _relatorio = value; }
        }

        private string[] _textoSetor = null;

        public string[] TextoSetor
        {
            get
            {
                if (_textoSetor == null && DataSetor != null)
                {
                    _textoSetor = new string[DataSetor.Length];

                    for (int i = 0; i < _textoSetor.Length; i++)
                    {
                        _textoSetor[i] = DataSetor[i] != null ? Conversoes.ConverteData(DataSetor[i].Value, true) + (TempoSetor[i].TotalMilliseconds != 0 ?
                            "<br />(Tempo: " + FormatTimeSpan(TempoSetor[i]) + ")" : "") : null;
                    }
                }

                return _textoSetor;
            }
        }

        private string _tempoTotal = null;

        public string TempoTotal
        {
            get
            {
                if (_tempoTotal == null)
                {
                    TimeSpan tempoTotal = new TimeSpan();
                    for (int i = 0; i < TempoSetor.Length; i++)
                        tempoTotal += TempoSetor[i];

                    _tempoTotal = FormatTimeSpan(tempoTotal);
                }
                else
                    _tempoTotal = _tempoTotal.Replace("'", "");

                return _tempoTotal;
            }
            internal set { _tempoTotal = value; }
        }

        private double? _tempoTotalHoras = null;

        public double TempoTotalHoras
        {
            get
            {
                if (_tempoTotalHoras == null)
                {
                    TimeSpan tempoTotal = new TimeSpan();
                    for (int i = 0; i < TempoSetor.Length; i++)
                        tempoTotal += TempoSetor[i];

                    _tempoTotalHoras = Math.Round(tempoTotal.TotalHours, 2);
                }

                return _tempoTotalHoras.GetValueOrDefault();
            }
            internal set { _tempoTotalHoras = value; }
        }

        public int DiferencaDataLiberacao
        {
            get
            {
                if (!DataLiberacao.HasValue)
                    return 0;

                return DataLiberacao.Value.Subtract(Data).Days;
            }
        }

        #endregion
    }
}