using GDA;
using Glass.Data.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(LogMovimentacaoNotaFiscalDAO))]
    [PersistenceClass("log_movimentacao_nota_fiscal")]
    public class LogMovimentacaoNotaFiscal : ModelBaseCadastro
    {
        [PersistenceProperty("IDNF")]
        public uint IdNf { get; set; }

        [PersistenceProperty("IDPRODNF")]
        public uint? IdProdNf { get; set; }

        [PersistenceProperty("MENSAGEMLOG")]
        public string MensagemLog { get; set; }


        [PersistenceProperty("NUMERONFE", DirectionParameter.InputOptional)]
        public uint? NumeroNfe { get; set; }

        [PersistenceProperty("DESCRICAOPROD", DirectionParameter.InputOptional)]
        public string DescricaoProd { get; set; }

    }
}
