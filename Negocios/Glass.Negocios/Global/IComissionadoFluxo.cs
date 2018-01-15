using System.Collections.Generic;

namespace Glass.Global.Negocios
{
    /// <summary>
    /// Assinatura do fluxo de negócio dos comissionados.
    /// </summary>
    public interface IComissionadoFluxo
    {
        /// <summary>
        /// Pesquisa os comissionados.
        /// </summary>
        /// <param name="nome">Nome do comissionado.</param>
        /// <param name="situacao"></param>
        /// <returns></returns>
        IList<Entidades.Comissionado> PesquisarComissionados(string nome, Glass.Situacao? situacao);
        
        /// <summary>
        /// Recupera os descritores dos comissionados.
        /// </summary>
        /// <returns></returns>
        IList<Colosoft.IEntityDescriptor> ObtemComissionados();

        /// <summary>
        /// Recupera os dados do comissionado.
        /// </summary>
        /// <param name="idComissionado"></param>
        /// <returns></returns>
        Entidades.Comissionado ObtemComissionado(int idComissionado);

        /// <summary>
        /// Salva os dados do comissionado.
        /// </summary>
        /// <param name="comissionado"></param>
        /// <returns></returns>
        Colosoft.Business.SaveResult SalvarComissionado(Entidades.Comissionado comissionado);

        /// <summary>
        /// Apaga os dados do comissionado.
        /// </summary>
        /// <param name="comissionado"></param>
        /// <returns></returns>
        Colosoft.Business.DeleteResult ApagarComissionado(Entidades.Comissionado comissionado);

        /// <summary>
        /// Verifica se existencia de algum CPF/CNPJ já cadastrado para algum comissionado.
        /// </summary>
        /// <param name="cpfCnpj">Valor que será verificador.</param>
        /// <returns></returns>
        bool VerificarCpfCnpj(string cpfCnpj);
    }
}
