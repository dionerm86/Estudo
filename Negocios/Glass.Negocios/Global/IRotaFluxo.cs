using System.Collections.Generic;

namespace Glass.Global.Negocios
{
    /// <summary>
    /// Assinatura do fluxo de rota.
    /// </summary>
    public interface IRotaFluxo
    {
        #region Rota

        /// <summary>
        /// Pesquisa as rotas.
        /// </summary>
        /// <returns></returns>
        IList<Entidades.RotaPesquisa> PesquisarRotas();

        /// <summary>
        /// Recupera os descritores das rotas.
        /// </summary>
        /// <returns></returns>
        IList<Colosoft.IEntityDescriptor> ObtemRotas();

        /// <summary>
        /// Recupera os dados da rota.
        /// </summary>
        /// <param name="idRota"></param>
        /// <returns></returns>
        Entidades.Rota ObtemRota(int idRota);

        /// <summary>
        /// Salva os dados da rota.
        /// </summary>
        /// <param name="rota"></param>
        /// <returns></returns>
        Colosoft.Business.SaveResult SalvarRota(Entidades.Rota rota);

        /// <summary>
        /// Apaga os dados da rota.
        /// </summary>
        /// <param name="rota"></param>
        /// <returns></returns>
        Colosoft.Business.DeleteResult ApagarRota(Entidades.Rota rota);

        #endregion

        #region RotaCliente

        /// <summary>
        /// Pesquisa os clientes da rota.
        /// </summary>
        /// <param name="idRota">Identificador da rota.</param>
        /// <returns></returns>
        IList<Entidades.RotaClientePesquisa> PesquisarClientesRota(int idRota);

        /// <summary>
        /// Recupera os dados da associação entre a rota e o cliente.
        /// </summary>
        /// <param name="idRota">Identificador da rota.</param>
        /// <param name="idCliente">Identifador do cliente.</param>
        /// <returns></returns>
        Entidades.RotaCliente ObtemRotaCliente(int idRota, int idCliente);

        /// <summary>
        /// Adiciona salva a associação do cliente com a rota.
        /// </summary>
        /// <param name="rotaCliente"></param>
        /// <returns></returns>
        Colosoft.Business.SaveResult SalvarRotaCliente(Entidades.RotaCliente rotaCliente);

        /// <summary>
        /// Apaga a associação da rota com o cliente.
        /// </summary>
        /// <param name="rotaCliente"></param>
        /// <returns></returns>
        Colosoft.Business.DeleteResult ApagarRotaCliente(Entidades.RotaCliente rotaCliente);

        /// <summary>
        /// Altera a posição do cliente dentro da rota.
        /// </summary>
        /// <param name="idRota">Identificador da rota.</param>
        /// <param name="idCliente">Identificador do cliente.</param>
        /// <param name="paraCima">Identifica se é para mover para cima.</param>
        /// <returns></returns>
        Colosoft.Business.SaveResult AlterarPosicao(int idRota, int idCliente, bool paraCima);

        #endregion
    }
}
