using System;
using GDA;
using Glass.Data.RelDAL;
using System.Linq;

namespace Glass.Data.RelModel
{
    [PersistenceBaseDAO(typeof(PlanoContasDAO))]
    [PersistenceClass("plano_contas")]
    public class PlanoContas
    {
        #region Propriedades

        [PersistenceProperty("IDCONTA")]
        public uint IdConta { get; set; }

        [PersistenceProperty("GRUPOCONTA")]
        public string GrupoConta { get; set; }

        [PersistenceProperty("PLANOCONTA")]
        public string PlanoConta { get; set; }

        [PersistenceProperty("DATA", DirectionParameter.InputOptional)]
        public DateTime Data { get; set; }

        [PersistenceProperty("TIPOMOV")]
        public long TipoMov { get; set; }

        [PersistenceProperty("VALOR")]
        public decimal Valor { get; set; }

        /// <summary>
        /// 0-Subtotal, 1-Crédito, 2-Débito, 3-Investimento
        /// </summary>
        [PersistenceProperty("NUMSEQCATEG")]
        public int NumSeqCateg { get; set; }

        [PersistenceProperty("GRUPOSUBTOTAL")]
        public string GrupoSubtotal { get; set; }

        [PersistenceProperty("GRUPOSUBTOTALAGREGADO")]
        public string GrupoSubtotalAgregado { get; set; }

        #endregion

        #region Propriedades Estendidas

        [PersistenceProperty("OBS", DirectionParameter.InputOptional)]
        public string Obs { get; set; }

        [PersistenceProperty("IDCOMPRA", DirectionParameter.InputOptional)]
        public int? IdCompra { get; set; }

        [PersistenceProperty("IDPAGTO", DirectionParameter.InputOptional)]
        public int? IdPagto { get; set; }

        [PersistenceProperty("IDDEPOSITO", DirectionParameter.InputOptional)]
        public int? IdDeposito { get; set; }

        [PersistenceProperty("IDPEDIDO", DirectionParameter.InputOptional)]
        public uint? IdPedido { get; set; }

        [PersistenceProperty("IDACERTO", DirectionParameter.InputOptional)]
        public uint? IdAcerto { get; set; }

        [PersistenceProperty("IDLIBERARPEDIDO", DirectionParameter.InputOptional)]
        public uint? IdLiberarPedido { get; set; }

        [PersistenceProperty("NOMEFORNEC", DirectionParameter.InputOptional)]
        public string NomeFornec { get; set; }

        [PersistenceProperty("NOMECLIENTE", DirectionParameter.InputOptional)]
        public string NomeCliente { get; set; }

        [PersistenceProperty("DESCRCATEGORIA", DirectionParameter.InputOptional)]
        public string DescrCategoria { get; set; }

        [PersistenceProperty("VencPeriodoNaoPagas", DirectionParameter.InputOptional)]
        public decimal VencPeriodoNaoPagas { get; set; }

        [PersistenceProperty("VencPassadoPagasPeriodo", DirectionParameter.InputOptional)]
        public decimal VencPassadoPagasPeriodo { get; set; }

        [PersistenceProperty("VALORCREDITOUTILIZADO", DirectionParameter.InputOptional)]
        public decimal ValorCreditoUtilizado { get; set; }

        [PersistenceProperty("MES", DirectionParameter.InputOptional)]
        public long Mes { get; set; }

        [PersistenceProperty("ANO", DirectionParameter.InputOptional)]
        public long Ano { get; set; }

        [PersistenceProperty("TipoCategoria")]
        public int TipoCategoria { get; set; }

        #endregion

        #region Propriedades de Suporte

        /// <summary>
        /// Recupera as observaçõs distintas.
        /// </summary>
        public string ObsDistintas
        {
            get
            {
                // Verifica se os observações são segmentadas
                if (!string.IsNullOrEmpty(Obs) &&
                    Obs[0] == '[' && Obs[Obs.Length - 1] == ']')
                    // Elimina as observações repetidas
                    return string.Join("; ",
                            Obs.Substring(1, Obs.Length - 2)
                               .Split(new string[] { "];[" }, StringSplitOptions.RemoveEmptyEntries)
                               .Select(f => f.Trim())
                               .Where(f => f.Length > 0)
                               .Distinct(StringComparer.InvariantCultureIgnoreCase)
                               .ToArray());


                return Obs;
            }
        }

        public string Criterio { get; set; }

        /// <summary>
        /// Retorna o cliente/fornecedor da operação junto com a referência da compra/pagto/deposito/pedido
        /// </summary>
        public string Referencial
        {
            get 
            {
                string referencial = String.Empty;

                if (!String.IsNullOrEmpty(NomeCliente))
                    referencial += "Cli.: " + NomeCliente + " ";
                else if (!String.IsNullOrEmpty(NomeFornec))
                    referencial += "Forn.: " + NomeFornec + " ";
                
                if (IdCompra > 0)
                    referencial += "(Compra: " + IdCompra + ")";

                if (IdPagto > 0)
                {
                    // Recupera a descrição do crédito utilizado no pagamento.
                    var descricaoCreditoUtilizado = ValorCreditoUtilizado > 0 ?
                        string.Format(" Crédito utilizado: {0}", ValorCreditoUtilizado.ToString("C")) : string.Empty;

                    referencial = IdCompra > 0 ?
                        referencial.TrimEnd(')') + " Pagto.: " + IdPagto + descricaoCreditoUtilizado + ")" : referencial + "(Pagto.: " + IdPagto + descricaoCreditoUtilizado + ")";
                }

                if (IdDeposito > 0)
                    referencial += "(Deposito: " + IdDeposito + ")";

                if (IdPedido > 0)
                    referencial += "(Pedido: " + IdPedido + ")";

                if (IdAcerto > 0)
                    referencial += "(Acerto: " + IdAcerto + ")";

                if (IdLiberarPedido > 0)
                    referencial = IdAcerto > 0 ? referencial.TrimEnd(')') + " Liber.: " + IdLiberarPedido + ")" : referencial + "(Liber.: " + IdLiberarPedido + ")";

                return referencial;
            }
        }

        public string DescrTipo
        {
            get 
            {  
                return TipoMov == 1 ? "Entrada" : "Saída";
            }
        }

        public decimal? ValorEntrada
        {
            get { return TipoMov == 1 ? (decimal?)Valor : null; }
        }

        public decimal? ValorSaida
        {
            get { return TipoMov != 1 ? (decimal?)Valor : null; }
        }

        public decimal ValorAjustado
        {
            get { return Valor + VencPeriodoNaoPagas - VencPassadoPagasPeriodo; }
        }

        public decimal? ValorAjustadoEntrada
        {
            get { return TipoMov == 1 ? (decimal?)ValorAjustado : null; }
        }

        public decimal? ValorAjustadoSaida
        {
            get { return TipoMov != 1 ? (decimal?)ValorAjustado : null; }
        }

        public string MesAno
        {
            get { return Mes.ToString("0#") + "/" + Ano.ToString(); }
        }

        private bool _descricaoCategoria = false;

        public bool DescricaoCategoria
        {
            get { return _descricaoCategoria; }
            set { _descricaoCategoria = value; }
        }

        private bool _descricaoGrupo = false;

        public bool DescricaoGrupo
        {
            get { return _descricaoGrupo; }
            set { _descricaoGrupo = value; }
        }

        public string PlanoContaRpt
        {
            get
            {
                if (_descricaoCategoria)
                    return DescrCategoria;
                else if (_descricaoGrupo)
                    return GrupoConta;
                else
                    return PlanoConta;
            }
        }

        #endregion
    }
}