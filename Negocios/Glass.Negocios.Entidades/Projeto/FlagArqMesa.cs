﻿using Colosoft;
using System;
using System.Collections.Generic;
using Colosoft.Business;
using Colosoft.Data;

namespace Glass.Projeto.Negocios.Entidades
{
    /// <summary>
    /// Assinatura do validador de cliente.
    /// </summary>
    public interface IValidadorFlagArqMesa
    {
        /// <summary>
        /// Valida a exclusão do flag.
        /// </summary>
        /// <param name="flagArqMesa"></param>
        /// <returns></returns>
        IMessageFormattable[] ValidaExclusao(FlagArqMesa flagArqMesa);
    }

    [Colosoft.Business.EntityLoader(typeof(FlagArqMesaLoader))]
    public class FlagArqMesa : Colosoft.Business.Entity<Glass.Data.Model.FlagArqMesa>
    {
        #region Tipos Aninhados

        class FlagArqMesaLoader : Colosoft.Business.EntityLoader<FlagArqMesa, Glass.Data.Model.FlagArqMesa>
        {
            public FlagArqMesaLoader()
            {
                Configure()
                    .Uid(f => f.IdFlagArqMesa)
                    .Description(f => f.Descricao)
                    .FindName(f => f.Descricao)
                    .Creator(f => new FlagArqMesa(f));
            }
        }

        #endregion

        #region Propriedades

        /// <summary>
        /// Identificador do flag.
        /// </summary>
        public int IdFlagArqMesa
        {
            get { return DataModel.IdFlagArqMesa; }
            set
            {
                if (DataModel.IdFlagArqMesa != value &&
                    RaisePropertyChanging("IdFlagArqMesa", value))
                {
                    DataModel.IdFlagArqMesa = value;
                    RaisePropertyChanged("IdFlagArqMesa");
                }
            }
        }

        /// <summary>
        /// Descrição.
        /// </summary>
        public string Descricao
        {
            get { return DataModel.Descricao; }
            set
            {
                if (DataModel.Descricao != value &&
                    RaisePropertyChanging("Descricao", value))
                {
                    DataModel.Descricao = value;
                    RaisePropertyChanged("Descricao");
                }
            }
        }

        /// <summary>
        /// Indica se o flag é padrão para todas as peças do sistema.
        /// </summary>
        public bool Padrao
        {
            get { return DataModel.Padrao; }
            set
            {
                if (DataModel.Padrao != value &&
                    RaisePropertyChanging("Padrao", value))
                {
                    DataModel.Padrao = value;
                    RaisePropertyChanged("Padrao");
                }
            }
        }

        /// <summary>
        /// Indica em qual tipo de arquivo o flag sera informado.
        /// </summary>
        public Glass.Data.Model.TipoArquivoMesaCorte? TipoArquivo
        {
            get { return DataModel.TipoArquivo; }
            set
            {
                if (DataModel.TipoArquivo != value &&
                    RaisePropertyChanging("TipoArquivo", value))
                {
                    DataModel.TipoArquivo = value;
                    RaisePropertyChanged("TipoArquivo");
                }
            }
        }

        /// <summary>
        /// Situação Flag
        /// </summary>
        public Glass.Situacao Situacao
        {
            get { return DataModel.Situacao; }
            set
            {
                if (DataModel.Situacao != value &&
                    RaisePropertyChanging("Situacao", value))
                {
                    DataModel.Situacao = value;
                    RaisePropertyChanged("Situacao");
                }
            }
        }

        /// <summary>
        /// Indica em qual tipo de arquivo o flag sera informado.
        /// </summary>
        public int[] TipoArquivoArr 
        {
            get
            {
                if (!TipoArquivo.HasValue)
                    return new int[0];

                var lst = new List<int>();

                foreach (Glass.Data.Model.TipoArquivoMesaCorte item in Enum.GetValues(typeof(Glass.Data.Model.TipoArquivoMesaCorte)))
                    if (TipoArquivo.HasValue && TipoArquivo.Value.HasFlag(item))
                        lst.Add((int)item);

                return lst.ToArray();
            }

            set 
            {
                TipoArquivo = null;

                foreach (var item in value)
                {
                    if (!TipoArquivo.HasValue)
                        TipoArquivo = (Glass.Data.Model.TipoArquivoMesaCorte)item;
                    else
                        TipoArquivo |= (Glass.Data.Model.TipoArquivoMesaCorte)item;
                }
            }
        }

        public string TipoArquivoDescr
        {
            get
            {
                var lst = new List<string>();

                foreach (Glass.Data.Model.TipoArquivoMesaCorte item in Enum.GetValues(typeof(Glass.Data.Model.TipoArquivoMesaCorte)))
                    if (TipoArquivo.HasValue && TipoArquivo.Value.HasFlag(item))
                        lst.Add(item.ToString());

                return string.Join(",", lst.ToArray());
            }
        }

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        public FlagArqMesa()
            : this(null, null, null)
        {

        }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="args"></param>
        protected FlagArqMesa(Colosoft.Business.EntityLoaderCreatorArgs<Glass.Data.Model.FlagArqMesa> args)
            : base(args.DataModel, args.UIContext, args.TypeManager)
        {

        }

        /// <summary>
        /// Cria a instancia com os dados do modelo de dados.
        /// </summary>
        /// <param name="dataModel"></param>
        /// <param name="uiContext"></param>
        /// <param name="entityTypeManager"></param>
        public FlagArqMesa(Glass.Data.Model.FlagArqMesa dataModel, string uiContext, Colosoft.Business.IEntityTypeManager entityTypeManager)
            : base(dataModel, uiContext, entityTypeManager)
        {

        }

        #endregion

        #region Métodos sobrescritos

        /// <summary>
        /// Sobrescreve o método de exclusão da entidade de flags.
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public override DeleteResult Delete(IPersistenceSession session)
        {
            var validador = Microsoft.Practices.ServiceLocation.ServiceLocator
                .Current.GetInstance<IValidadorFlagArqMesa>();

            // Executa a validação
            var resultadoValidacao = validador.ValidaExclusao(this);
            if (resultadoValidacao.Length > 0)
                return new Colosoft.Business.DeleteResult(false, resultadoValidacao.Join(" "));

            return base.Delete(session);
        }

        #endregion
    }
}
