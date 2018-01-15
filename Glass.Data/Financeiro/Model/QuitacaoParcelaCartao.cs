using GDA;
using System;

namespace Glass.Data.Model
{
    [PersistenceClass("quitacao_parcela_cartao")]
    public class QuitacaoParcelaCartao : ModelBaseCadastro
    {
        [PersistenceProperty("IDQUITACAOPARCELACARTAO", PersistenceParameterType.IdentityKey)]
        public int IdQuitacaoParcelaCartao { get; set; }

        [PersistenceProperty("IDARQUIVOQUITACAOPARCELACARTAO")]
        public int IdArquivoQuitacaoParcelaCartao { get; set; }

        [PersistenceProperty("NUMAUTCARTAO")]
        public string NumAutCartao { get; set; }

        [PersistenceProperty("ULTIMOSDIGITOSCARTAO")]
        public string UltimosDigitosCartao { get; set; }

        [PersistenceProperty("VALOR")]
        public decimal Valor { get; set; }

        [PersistenceProperty("TIPO")]
        public TipoCartaoEnum Tipo { get; set; }

        [PersistenceProperty("BANDEIRA")]
        public int Bandeira { get; set; }

        [PersistenceProperty("NUMPARCELA")]
        public int NumParcela { get; set; }

        [PersistenceProperty("NUMPARCELAMAX")]
        public int NumParcelaMax { get; set; }
        
        [PersistenceProperty("TARIFA")]
        public decimal Tarifa { get; set; }

        [PersistenceProperty("QUITADA")]
        public bool Quitada { get; set; }

        [PersistenceProperty("DATAVENC")]
        public DateTime DataVenc { get; set; }
    }
}
