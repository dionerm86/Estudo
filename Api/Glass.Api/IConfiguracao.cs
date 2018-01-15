using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glass.Api
{
    /// <summary>
    /// Assinatura das configurações da api.
    /// </summary>
    public interface IConfiguracao
    {
        /// <summary>
        /// Endereço do Serviço de imagem.
        /// </summary>
        string EnderecoServicoImagem { get; }

        /// <summary>
        /// Caminho para foto pedido.
        /// </summary>
        string CaminhoFotoPedido { get; }

        /// <summary>
        /// Define o endereço do serviço de imagem.
        /// </summary>
        /// <param name="enderecoServicoImagem"></param>
        void DefinirEnderecoServicoImagem(string enderecoServicoImagem);

        /// <summary>
        /// Define o caminho da foto do pedido.
        /// </summary>
        /// <param name="caminhoFotoPedido"></param>
        void DefinirCaminhoFotoPedido(string caminhoFotoPedido);
    }
}
