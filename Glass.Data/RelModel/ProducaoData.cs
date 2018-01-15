using System;
using GDA;
using Glass.Data.RelDAL;
using Glass.Data.DAL;
using Glass.Configuracoes;

namespace Glass.Data.RelModel
{
    [PersistenceBaseDAO(typeof(ProducaoDataDAO))]
    [PersistenceClass("producao_nao_cortado")]
    public class ProducaoData
    {
        #region Propriedades

        [PersistenceProperty("Data")]
        public DateTime? DataHora { get; set; }

        [PersistenceProperty("IdCorVidro")]
        public uint IdCorVidro { get; set; }

        [PersistenceProperty("DescrCorVidro")]
        public string DescrCorVidro { get; set; }

        [PersistenceProperty("Espessura")]
        public float Espessura { get; set; }

        [PersistenceProperty("CodAplicacao")]
        public string CodAplicacao { get; set; }

        [PersistenceProperty("CodProcesso")]
        public string CodProcesso { get; set; }

        [PersistenceProperty("TotM2")]
        public double TotM2 { get; set; }

        [PersistenceProperty("Pronto")]
        public long Pronto { get; set; }

        [PersistenceProperty("Criterio")]
        public string Criterio { get; set; }

        #endregion

        #region Propriedades de Suporte

        public string DescrEspessura
        {
            get { return Espessura.ToString().PadLeft(2, '0') + "mm"; }
        }

        public string DescrAplProc
        {
            get 
            { 
                return (!String.IsNullOrEmpty(CodAplicacao) ? "Apl: " + CodAplicacao + "\n" : "") +
                    (!String.IsNullOrEmpty(CodProcesso) ? "Proc: " + CodProcesso : ""); 
            }
        }

        public string DescrPendente
        {
            get { return Pronto == -1 ? "Etiqueta não impressa" : Pronto == 0 ? "Pendente" : "Pronto"; }
        }

        public bool ExibirSituacao
        {
            get { return IdCorVidro > 0; }
        }

        public string Titulo
        {
            get { return ExibirSituacao ? "Cores" : "Situação"; }
        }

        public string Subtitulo
        {
            get { return ExibirSituacao ? DescrCorVidro + " / m²" : DescrPendente; }
        }

        public int ProducaoMaximaDia
        {
            get 
            { 
                return DataHora.HasValue ? 
                    CapacidadeProducaoDiariaDAO.Instance.MaximoVendasData(DataHora.Value) :
                    PedidoConfig.Pedido_MaximoVendas.MaximoVendasM2; 
            }
        }

        #endregion
    }
}