using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Pedido.Negocios.Componentes
{
    /// <summary>
    /// Representa o verificador do pedido pela rentabilidade para liberação
    /// </summary>
    public class VerificadorRentabilidadeLiberacaoPedido : Data.IVerificadorRentabilidadeLiberacao<Data.Model.Pedido>
    {
        #region Properties

        /// <summary>
        /// Obtém o verificador.
        /// </summary>
        private Rentabilidade.Negocios.IVerificadorRentabilidadeLiberacao Verificador { get; }

        /// <summary>
        /// Obtém o provedor de item de rentabilidade para o pedido.
        /// </summary>
        private Rentabilidade.Negocios.IProvedorItemRentabilidade<Data.Model.Pedido> ProvedorItemRentabilidade { get; }

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="verificador"></param>
        /// <param name="provedorItemRentabilidade"></param>
        public VerificadorRentabilidadeLiberacaoPedido(
            Rentabilidade.Negocios.IVerificadorRentabilidadeLiberacao verificador,
            Rentabilidade.Negocios.IProvedorItemRentabilidade<Data.Model.Pedido> provedorItemRentabilidade)
        {
            Verificador = verificador;
            ProvedorItemRentabilidade = provedorItemRentabilidade;
        }

        #endregion

        #region Métodos Públicos

        /// <summary>
        /// Executa a verificação da rentabilidade para a liberação.
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool VerificarRequerLiberacao(GDA.GDASession sessao, Data.Model.Pedido instancia)
        {
            if (!Configuracoes.RentabilidadeConfig.CalcularRentabilidade) return true;
            var item = ProvedorItemRentabilidade.ObterItem(sessao, instancia);
            return Verificador.VerificarRequerLiberacao(item);
        }

        /// <summary>
        /// Verifica se pode liberar.
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="instancia"></param>
        /// <returns></returns>
        public bool PodeLiberar(GDA.GDASession sessao, Data.Model.Pedido instancia)
        {
            if (!Configuracoes.RentabilidadeConfig.CalcularRentabilidade) return true;
            var item = ProvedorItemRentabilidade.ObterItem(sessao, instancia);
            return Verificador.PodeLiberar(item);
        }

        #endregion
    }
}
