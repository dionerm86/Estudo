using Glass.Data.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Glass.Api.Projeto
{
    /// <summary>
    /// Representa um item de projeto.
    /// </summary>
    public class ItemProjeto : IItemProjeto
    {
        #region Variáveis Locais

        private List<MedidaItemProjeto> _medidas = new List<MedidaItemProjeto>();
        private List<PecaItemProjeto> _pecas = new List<PecaItemProjeto>();
        private List<MaterialItemProjeto> _materiais = new List<MaterialItemProjeto>();
        private List<PosicaoPeca> _posicoesPeca = new List<PosicaoPeca>();

        #endregion

        #region Propriedades

        /// <summary>
        /// Medidas.
        /// </summary>
        public List<MedidaItemProjeto> Medidas
        {
            get { return _medidas; }
        }

        /// <summary>
        /// Peças.
        /// </summary>
        public List<PecaItemProjeto> Pecas
        {
            get { return _pecas; }
        }

        /// <summary>
        /// Materiais.
        /// </summary>
        public List<MaterialItemProjeto> Materiais
        {
            get { return _materiais; }
        }

        /// <summary>
        /// Posições da peça do item projeto
        /// </summary>
        public List<PosicaoPeca> PosicoesPeca
        {
            get { return _posicoesPeca; }
        }

        /// <summary>
        /// Identificador do item.
        /// </summary>
        public Guid IdItemProjeto { get; set; }

        /// <summary>
        /// Identificador do modelo de projeto.
        /// </summary>
        public int IdProjetoModelo { get; set; }

        /// <summary>
        /// Identificador da cor do vidro.
        /// </summary>
        public int IdCorVidro { get; set; }

        /// <summary>
        /// Espessura do vidro.
        /// </summary>
        public int EspessuraVidro { get; set; }

        /// <summary>
        /// Medida exata.
        /// </summary>
        public bool MedidaExata { get; set; }

        /// <summary>
        /// Ambiente.
        /// </summary>
        public string Ambiente { get; set; }
        
        /// <summary>
        /// Quantidade.
        /// </summary>
        public int Qtde { get; set; }

        /// <summary>
        /// m² do vão.
        /// </summary>
        public float M2Vao { get; set; }

        /// <summary>
        /// Total.
        /// </summary>
        public decimal Total { get; set; }

        /// <summary>
        /// Custo total.
        /// </summary>
        public decimal CustoTotal { get; set; }

        /// <summary>
        /// Observações.
        /// </summary>
        public string Obs { get; set; }

        #endregion

        #region IItemProjeto Members

        uint IItemProjeto.IdItemProjeto { get; set; }

        uint IItemProjeto.IdProjetoModelo
        {
            get
            {
                return (uint)IdProjetoModelo;
            }
        }

        public uint? IdProjeto
        {
            get
            {
                return null;
            }
        }

        public uint? IdOrcamento
        {
            get
            {
                return null;
            }
        }

        public uint? IdPedido
        {
            get
            {
                return null;
            }
        }

        public uint? IdPedidoEspelho
        {
            get
            {
                return null;
            }
        }

        public uint? IdCliente
        {
            get
            {
                return null;
            }
        }

        uint IItemProjeto.IdCorVidro
        {
            get
            {
                return (uint)IdCorVidro;
            }

            set
            {
                IdCorVidro = (int)value;
            }
        }

        IEnumerable<IMedidaItemProjeto> IItemProjeto.Medidas
        {
            get
            {
                return Medidas;
            }
        }

        IEnumerable<IPecaItemProjeto> IItemProjeto.Pecas
        {
            get
            {
                return Pecas;
            }
        }

        IEnumerable<IMaterialItemProjeto> IItemProjeto.Materiais
        {
            get
            {
                return Materiais;
            }
        }

        IEnumerable<IPosicaoPeca> IItemProjeto.PosicoesPeca
        {
            get
            {
                return PosicoesPeca;
            }
        }

        #endregion
    }
}