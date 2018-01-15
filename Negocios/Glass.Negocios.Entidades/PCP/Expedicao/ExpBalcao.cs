using System.Collections.Generic;
using System.Linq;

namespace Glass.PCP.Negocios.Entidades
{
    public interface IProvedorExpBalcao
    {
        /// <summary>
        /// Busca o setores pendentes de uma peça
        /// </summary>
        /// <param name="idProdPedProducao"></param>
        /// <returns></returns>
        string ObtemDescricaoSetoresRestantes(int idProdPedProducao);
    }

    /// <summary>
    /// Armazena os dados para a expedição balcão
    /// </summary>
    public class ExpBalcao
    {
        #region Variaveis Locais

        private List<Entidades.ItemExpBalcao> _itensExp;

        #endregion

        #region Construtor

        public ExpBalcao() { }

        public ExpBalcao(int idLiberacao)
        {
            IdLiberacao = IdLiberacao;
        }

        #endregion

        #region Propiedades

        /// <summary>
        /// Identificador da liberação
        /// </summary>
        public int IdLiberacao { get; set; }

        /// <summary>
        /// Identificador do cliente da liberação
        /// </summary>
        public int IdCliente { get; set; }

        /// <summary>
        /// Nome do cliente da liberação
        /// </summary>
        public string NomeCliente { get; set; }

        /// <summary>
        /// Itens da exp. (Peças e volumes).
        /// </summary>
        public List<Entidades.ItemExpBalcao> ItensExp
        {
            get 
            {
                if (_itensExp == null)
                    _itensExp = new List<ItemExpBalcao>();

                return _itensExp;
            }
            set { _itensExp = value; }
        }

        /// <summary>
        /// Peças de vidro da expedição
        /// </summary>
        public IList<Entidades.ItemExpBalcao> Pecas
        {
            get
            {
                return ItensExp
                    .Where(f => f.IdVolume == null).ToList();
            }
        }

        /// <summary>
        /// Volumes da expedição
        /// </summary>
        public IList<Entidades.ItemExpBalcao> Volumes
        {
            get
            {
                return ItensExp
                    .Where(f => f.IdVolume.GetValueOrDefault(0) > 0).ToList();
            }
        }

        /// <summary>
        /// Verifica se o botão de estornar vai estar visivel
        /// </summary>
        public bool EstornarVisivel
        {
            get
            {
                return ItensExp.Where(f => f.Expedido).Count() > 0;
            }
        }

        #region Total

        /// <summary>
        /// Quantidade total de peças de vidro
        /// </summary>
        public int TotalPecas
        {
            get
            {
                return ItensExp
                    .Where(f => f.IdVolume == null)
                    .Count();
            }
        }

        /// <summary>
        /// Quantidade total de volumes
        /// </summary>
        public int TotalVolumes
        {
            get
            {
                return ItensExp
                    .Where(f => f.IdVolume.GetValueOrDefault(0) > 0)
                    .Count();
            }
        }

        /// <summary>
        /// Peso total da expedição
        /// </summary>
        public double PesoTotal
        {
            get
            {
                return ItensExp.Sum(f => f.Peso);
            }
        }

        #endregion

        #region Expedido

        /// <summary>
        /// Quantidade de peças de vidro expedidas
        /// </summary>
        public int TotalPecasExpedidas
        {
            get
            {
                return ItensExp
                    .Where(f => (f.IdVolume == null) &&
                        f.IdFuncLeitura.GetValueOrDefault(0) > 0 && f.DataLeitura != null)
                    .Count();
            }
        }

        /// <summary>
        /// Quantidade de volumes expedidos
        /// </summary>
        public int TotalVolumesExpedidos
        {
            get
            {
                return ItensExp
                    .Where(f => (f.IdVolume.GetValueOrDefault(0) > 0) &&
                        f.IdFuncLeitura.GetValueOrDefault(0) > 0 && f.DataLeitura != null)
                    .Count();
            }
        }

        /// <summary>
        /// Peso total expedido
        /// </summary>
        public double PesoTotalExpedido
        {
            get
            {
                return ItensExp
                    .Where(f => f.IdFuncLeitura.GetValueOrDefault(0) > 0 && f.DataLeitura != null)
                    .Sum(f => f.Peso);
            }
        }

        #endregion

        #region Pendente

        /// <summary>
        /// Quantidade de peças de vidro pendentes
        /// </summary>
        public int TotalPecasPendentes
        {
            get
            {
                return ItensExp
                    .Where(f => (f.IdVolume == null) &&
                        f.IdFuncLeitura == null && f.DataLeitura == null)
                    .Count();
            }
        }

        /// <summary>
        /// Quantidade de volumes pendentes
        /// </summary>
        public int TotalVolumesPendentes
        {
            get
            {
                return ItensExp
                    .Where(f => (f.IdVolume.GetValueOrDefault(0) > 0) &&
                        f.IdFuncLeitura == null && f.DataLeitura == null)
                    .Count();
            }
        }

        /// <summary>
        /// Peso total Pendente
        /// </summary>
        public double PesoTotalPendente
        {
            get
            {
                return ItensExp
                    .Where(f => f.IdFuncLeitura == null && f.DataLeitura == null)
                    .Sum(f => f.Peso);
            }
        }

        #endregion

        #endregion
    }
}
