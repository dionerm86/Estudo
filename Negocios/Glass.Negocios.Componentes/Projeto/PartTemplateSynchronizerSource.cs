using CADProject.Remote.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glass.Projeto.Negocios.Componentes
{
    /// <summary>
    /// Implementação da fonte de sincronização dos modelos de ferragem.
    /// </summary>
    class PartTemplateSynchronizerSource : IPartTemplateSynchronizerSource
    {
        #region Local Variables

        private List<Item> _pendings = new List<Item>();
        private Entidades.IFerragemRepositorioCalcPackage _repositorioCalcPackage;

        #endregion

        #region Constructors

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="repositorioCalcPackage"></param>
        public PartTemplateSynchronizerSource(Entidades.IFerragemRepositorioCalcPackage repositorioCalcPackage)
        {
            _repositorioCalcPackage = repositorioCalcPackage;
        }

        #endregion

        #region Métodos Privados

        /// <summary>
        /// Recupera o estilo de ancoragem.
        /// </summary>
        /// <param name="ancoragem"></param>
        /// <returns></returns>
        private static AnchorStyles GetAnchorStyle(Data.Model.EstiloAncoragem ancoragem)
        {
            return (AnchorStyles)(int)ancoragem;
        }

        /// <summary>
        /// Converte a ferragem no modelo.
        /// </summary>
        /// <param name="ferragem"></param>
        /// <returns></returns>
        private PartTemplate Converter(Entidades.Ferragem ferragem)
        {
            var template = new PartTemplate
            {
                Uid = ferragem.UUID,
                Name = ferragem.Nome,
                Description = ferragem.Nome,
                Width = ferragem.Largura,
                Height = ferragem.Altura,
                IsCalcEngine = true,
                IsStaticParameters = ferragem.MedidasEstaticas,
                AnchorStyle = GetAnchorStyle(ferragem.EstiloAncoragem),
                CanRotate = ferragem.PodeRotacionar,
                CanMirror = ferragem.PodeEspelhar,
            };

            template.Codes.AddRange(ferragem.Codigos.Select(f => f.Codigo));
            template.Parameters.AddRange(ferragem.Constantes
                .Select(f => new PartTemplateParameter
                {
                    Name = f.Nome,
                    DefaultValue = f.Valor
                }));

            var arquivo = _repositorioCalcPackage.ObterCaminho(ferragem.IdFerragem);

            if (System.IO.File.Exists(arquivo))
            {
                var stream = System.IO.File.OpenRead(arquivo);
                template.Content = new StreamPartTemplateContent(stream);
            }

            return template;
        }

        #endregion

        #region Métodos Públicos

        /// <summary>
        /// Atualia os dados da ferragem.
        /// </summary>
        /// <param name="ferragem"></param>
        /// <param name="confirm">Método acionado quando a ferragem for atualizada.</param>
        /// <param name="fail">Método acionado quando ocorrer uma falha na atualização.</param>
        public void Atualizar(Entidades.Ferragem ferragem, 
            Action<Entidades.Ferragem> confirm, Action<Entidades.Ferragem, string> fail)
        {
            _pendings.Add(new Item(
                PartTemplateSynchronizerOperation.Update,
                Converter(ferragem), ferragem, confirm, fail));
        }

        /// <summary>
        /// Apaga os dados da ferragem.
        /// </summary>
        /// <param name="ferragem"></param>
        /// <param name="confirm">Método acionado quando a ferragem for apagada..</param>
        /// <param name="fail">Método acionado quando ocorrer uma falha na exclusão.</param>
        public void Apagar(Entidades.Ferragem ferragem,
            Action<Entidades.Ferragem> confirm, Action<Entidades.Ferragem, string> fail)
        {
            _pendings.Add(new Item(
                PartTemplateSynchronizerOperation.Delete,
                Converter(ferragem), ferragem, confirm, fail));
        }

        #endregion

        #region IPartTemplateSynchronizerSource Members

        /// <summary>
        /// Recupera os modelos pendentes para a sincronização.
        /// </summary>
        /// <returns></returns>
        IEnumerable<IPartTemplateSynchronizerItem> IPartTemplateSynchronizerSource.GetPendings()
        {
            return _pendings;
        }

        /// <summary>
        /// Confirma a sincronização.
        /// </summary>
        /// <param name="item">Item que foi sincronizado.</param>
        void IPartTemplateSynchronizerSource.Confirm(IPartTemplateSynchronizerItem item)
        {
            var item2 = item as Item;

            if (item2 != null)
                item2.Confirmer(item2.Ferragem);
        }

        /// <summary>
        /// Notifica falha na sincronização do modelo.
        /// </summary>
        /// <param name="item">Item que foi sincronizado.</param>
        /// <param name="message">Mensagem da falha.</param>
        void IPartTemplateSynchronizerSource.NotifyFail(IPartTemplateSynchronizerItem item, string message)
        {
            var item2 = item as Item;

            if (item2 != null)
                item2.ErrorNotifier(item2.Ferragem, message);
        }

        #endregion

        #region Nested Types

        /// <summary>
        /// Representa um item do sincronizador.
        /// </summary>
        class Item : IPartTemplateSynchronizerItem
        {
            #region Propriedades

            /// <summary>
            /// Operação.
            /// </summary>
            public PartTemplateSynchronizerOperation Operation { get; private set; }

            /// <summary>
            /// Modelo de peça associado.
            /// </summary>
            public PartTemplate PartTemplate { get; private set; }

            /// <summary>
            /// Ferragem associada.
            /// </summary>
            public Entidades.Ferragem Ferragem { get; private set; }

            /// <summary>
            /// Referência do método de confirmação.
            /// </summary>
            public Action<Entidades.Ferragem> Confirmer { get; private set; }

            /// <summary>
            /// Referência do método de notificação de erro.
            /// </summary>
            public Action<Entidades.Ferragem, string> ErrorNotifier { get; set; }

            #endregion

            #region Construtores

            /// <summary>
            /// Construtor padrão.
            /// </summary>
            /// <param name="operation"></param>
            /// <param name="partTemplate"></param>
            /// <param name="ferragem"></param>
            /// <param name="confirmer"></param>
            /// <param name="errorNotifier"></param>
            public Item(
                PartTemplateSynchronizerOperation operation, PartTemplate partTemplate, 
                Entidades.Ferragem ferragem,
                Action<Entidades.Ferragem> confirmer,
                Action<Entidades.Ferragem, string> errorNotifier)
            {
                Operation = operation;
                PartTemplate = partTemplate;
                Ferragem = ferragem;
                Confirmer = confirmer;
                ErrorNotifier = errorNotifier;
            }

            #endregion
        }

        #endregion
    }
}
