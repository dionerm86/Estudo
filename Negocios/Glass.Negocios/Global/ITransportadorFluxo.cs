using System.Collections.Generic;

namespace Glass.Global.Negocios
{
    /// <summary>
    /// Assinatura do fluxo de negócio de transportadores.
    /// </summary>
    public interface ITransportadorFluxo
    {
        #region Transportador

        /// <summary>
        /// Recupera os descritores dos transportadores.
        /// </summary>
        /// <returns></returns>
        IList<Colosoft.IEntityDescriptor> ObtemDescritoresTransportadores();

         /// <summary>
        /// Pesquisa os transportadores.
        /// </summary>
        /// <param name="idTransportador">Identificador do transportador.</param>
        /// <param name="nome">Nome que será usado na pesquisa.</param> 
        /// <param name="cpfCnpj">CPF/CNPJ</param>
        /// <returns></returns>
        IList<Entidades.Transportador> PesquisarTransportadores(int? idTransportador, string nome, string cpfCnpj);

        /// <summary>
        /// Recupera os dados do transportador.
        /// </summary>
        /// <param name="idTransportador"></param>
        /// <returns></returns>
        Entidades.Transportador ObtemTransportador(int idTransportador);

        /// <summary>
        /// Salva os dados do transportador.
        /// </summary>
        /// <param name="transportador"></param>
        /// <returns></returns>
        Colosoft.Business.SaveResult SalvarTransportador(Entidades.Transportador transportador);

        /// <summary>
        /// Apaga os dados do transportador.
        /// </summary>
        /// <param name="transportador"></param>
        /// <returns></returns>
        Colosoft.Business.DeleteResult ApagarTransportador(Entidades.Transportador transportador);

        /// <summary>
        /// Verifica se existencia de algum CPF/CNPJ já cadastrado para algum transportador.
        /// </summary>
        /// <param name="cpfCnpj">Valor que será verificador.</param>
        /// <returns></returns>
        bool VerificarCpfCnpj(string cpfCnpj);

        #endregion
    }
}
