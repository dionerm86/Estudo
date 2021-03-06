﻿using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(FotosDevolucaoPagtoDAO))]
    [PersistenceClass("fotos_devolucao_pagto")]
    public class FotosDevolucaoPagto : IFoto
    {
        #region Propriedades

        [PersistenceProperty("IDFOTO", PersistenceParameterType.IdentityKey)]
        public override uint IdFoto { get; set; }

        [PersistenceProperty("IDDEVOLUCAOPAGTO")]
        public uint IdDevolucaoPagto { get; set; }

        [PersistenceProperty("DESCRICAO")]
        public override string Descricao { get; set; }

        [PersistenceProperty("EXTENSAO")]
        public override string Extensao { get; set; }

        #endregion

        #region IFoto Members

        public override uint IdParent
        {
            get { return IdDevolucaoPagto; }
            set { IdDevolucaoPagto = value; }
        }

        public override IFoto.TipoFoto Tipo
        {
            get { return TipoFoto.DevolucaoPagto; }
        }

        public override string CodInterno
        {
            get { return null; }
            set { }
        }

        public override float AreaQuadrada
        {
            get { return 0; }
            set { }
        }

        public override float MetroLinear
        {
            get { return 0; }
            set { }
        }

        public override bool PermiteCalcularArea
        {
            get { return false; }
        }

        public override bool ApenasImagens
        {
            get { return false; }
        }

        #endregion
    }
}
