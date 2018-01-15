using System;
using GDA;
using Glass.Data.Model;
using Glass.Data.RelDAL;

namespace Glass.Data.RelModel
{
    [PersistenceBaseDAO(typeof(AlteracoesSistemaDAO))]
    [PersistenceClass("alteracoes_sistema")]
    public class AlteracoesSistema
    {
        #region Propriedades

        [PersistenceProperty("TIPO")]
        public string Tipo { get; set; }

        [PersistenceProperty("TABELA")]
        public int Tabela { get; set; }

        [PersistenceProperty("IDREGISTRO")]
        public uint IdRegistro { get; set; }

        [PersistenceProperty("CAMPO")]
        public string Campo { get; set; }

        [PersistenceProperty("VALOR")]
        public string Valor { get; set; }

        [PersistenceProperty("INFOADICIONAL")]
        public string InfoAdicional { get; set; }

        [PersistenceProperty("DATA")]
        public DateTime Data { get; set; }

        [PersistenceProperty("IDFUNC")]
        public uint IdFunc { get; set; }

        [PersistenceProperty("NOMEFUNC")]
        public string NomeFunc { get; set; }

        [PersistenceProperty("REFERENCIA")]
        public string Referencia { get; set; }

        [PersistenceProperty("NUMEVENTO")]
        public uint NumEvento { get; set; }

        [PersistenceProperty("CRITERIO")]
        public string Criterio { get; set; }

        #endregion

        #region Propriedades de Suporte

        public string DescrTipo
        {
            get { return Tipo == "Alt" ? "Alteração" : "Cancelamento"; }
        }

        public string NomeTabela
        {
            get
            {
                if (Tipo == "Alt")
                    return LogAlteracao.GetDescrTabela(Tabela);
                else
                    return LogCancelamento.GetDescrTabela(Tabela);
            }
        }

        #endregion
    }
}