using GDA;
using Glass.Data.DAL;
using Glass.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(MovMateriaPrimaDAO))]
    [PersistenceClass("mov_materia_prima")]
    public class MovMateriaPrima
    {
        #region Propiedades

        [PersistenceProperty("IdMovMateriaPrima", PersistenceParameterType.IdentityKey)]
        public int IdMovMateriaPrima { get; set; }

        [Log("Funcionário", "Nome", typeof(FuncionarioDAO))]
        [PersistenceProperty("IdFunc")]
        public uint IdFunc { get; set; }

        [PersistenceProperty("TipoMov")]
        public MovEstoque.TipoMovEnum TipoMov { get; set; }

        [Log("Data")]
        [PersistenceProperty("DataMov")]
        public DateTime DataMov { get; set; }

        [Log("CorVidro", "Descricao", typeof(CorVidro))]
        [PersistenceProperty("IdCorVidro")]
        public int IdCorVidro { get; set; }

        [Log("Espessura")]
        [PersistenceProperty("Espessura")]
        public decimal Espessura { get; set; }

        [PersistenceProperty("Qtde")]
        public decimal Qtde { get; set; }

        [PersistenceProperty("Saldo")]
        public decimal Saldo { get; set; }

        [PersistenceProperty("IdProdNf")]
        public int? IdProdNf { get; set; }

        [PersistenceProperty("IdProdPed")]
        public int? IdProdPed { get; set; }

        [PersistenceProperty("IdProdPedEsp")]
        public int? IdProdPedEsp { get; set; }

        [PersistenceProperty("IdPerdaChapaVidro")]
        public int? IdPerdaChapaVidro { get; set; }

        [PersistenceProperty("IdProdImpressao")]
        public int? IdProdImpressao { get; set; }

        #endregion
    }
}
