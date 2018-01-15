using System;
using GDA;
using Glass.Data.DAL;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(SetorProducaoDAO))]
    [PersistenceClass("setor_producao")]
    public class SetorProducao
    {
        #region Propriedades

        [PersistenceProperty("IDPRODPEDPRODUCAO")]
        public uint IdProdPedProducao { get; set; }

        [PersistenceProperty("IDSSETORES")]
        public string IdsSetores { get; set; }

        [PersistenceProperty("SETOR0", DirectionParameter.InputOptional)]
        public DateTime? Setor0 { get; set; }

        [PersistenceProperty("SETOR1", DirectionParameter.InputOptional)]
        public DateTime? Setor1 { get; set; }

        [PersistenceProperty("SETOR2", DirectionParameter.InputOptional)]
        public DateTime? Setor2 { get; set; }

        [PersistenceProperty("SETOR3", DirectionParameter.InputOptional)]
        public DateTime? Setor3 { get; set; }

        [PersistenceProperty("SETOR4", DirectionParameter.InputOptional)]
        public DateTime? Setor4 { get; set; }

        [PersistenceProperty("SETOR5", DirectionParameter.InputOptional)]
        public DateTime? Setor5 { get; set; }

        [PersistenceProperty("SETOR6", DirectionParameter.InputOptional)]
        public DateTime? Setor6 { get; set; }

        [PersistenceProperty("SETOR7", DirectionParameter.InputOptional)]
        public DateTime? Setor7 { get; set; }

        [PersistenceProperty("SETOR8", DirectionParameter.InputOptional)]
        public DateTime? Setor8 { get; set; }

        [PersistenceProperty("SETOR9", DirectionParameter.InputOptional)]
        public DateTime? Setor9 { get; set; }

        [PersistenceProperty("SETOR10", DirectionParameter.InputOptional)]
        public DateTime? Setor10 { get; set; }

        [PersistenceProperty("SETOR11", DirectionParameter.InputOptional)]
        public DateTime? Setor11 { get; set; }

        [PersistenceProperty("SETOR12", DirectionParameter.InputOptional)]
        public DateTime? Setor12 { get; set; }

        [PersistenceProperty("SETOR13", DirectionParameter.InputOptional)]
        public DateTime? Setor13 { get; set; }

        [PersistenceProperty("SETOR14", DirectionParameter.InputOptional)]
        public DateTime? Setor14 { get; set; }

        [PersistenceProperty("SETOR15", DirectionParameter.InputOptional)]
        public DateTime? Setor15 { get; set; }

        [PersistenceProperty("SETOR16", DirectionParameter.InputOptional)]
        public DateTime? Setor16 { get; set; }

        [PersistenceProperty("SETOR17", DirectionParameter.InputOptional)]
        public DateTime? Setor17 { get; set; }

        [PersistenceProperty("SETOR18", DirectionParameter.InputOptional)]
        public DateTime? Setor18 { get; set; }

        [PersistenceProperty("SETOR19", DirectionParameter.InputOptional)]
        public DateTime? Setor19 { get; set; }

        [PersistenceProperty("SETOR20", DirectionParameter.InputOptional)]
        public DateTime? Setor20 { get; set; }

        [PersistenceProperty("SETOR21", DirectionParameter.InputOptional)]
        public DateTime? Setor21 { get; set; }

        [PersistenceProperty("SETOR22", DirectionParameter.InputOptional)]
        public DateTime? Setor22 { get; set; }

        [PersistenceProperty("SETOR23", DirectionParameter.InputOptional)]
        public DateTime? Setor23 { get; set; }

        [PersistenceProperty("SETOR24", DirectionParameter.InputOptional)]
        public DateTime? Setor24 { get; set; }

        [PersistenceProperty("SETOR25", DirectionParameter.InputOptional)]
        public DateTime? Setor25 { get; set; }

        [PersistenceProperty("SETOR26", DirectionParameter.InputOptional)]
        public DateTime? Setor26 { get; set; }

        [PersistenceProperty("SETOR27", DirectionParameter.InputOptional)]
        public DateTime? Setor27 { get; set; }

        [PersistenceProperty("SETOR28", DirectionParameter.InputOptional)]
        public DateTime? Setor28 { get; set; }

        [PersistenceProperty("SETOR29", DirectionParameter.InputOptional)]
        public DateTime? Setor29 { get; set; }

        private string _nomeFunc0;

        [PersistenceProperty("FUNC0", DirectionParameter.InputOptional)]
        public string NomeFunc0
        {
            get { return BibliotecaTexto.GetFirstName(_nomeFunc0); }
            set { _nomeFunc0 = value; }
        }

        private string _nomeFunc1;

        [PersistenceProperty("FUNC1", DirectionParameter.InputOptional)]
        public string NomeFunc1
        {
            get { return BibliotecaTexto.GetFirstName(_nomeFunc1); }
            set { _nomeFunc1 = value; }
        }

        private string _nomeFunc2;

        [PersistenceProperty("FUNC2", DirectionParameter.InputOptional)]
        public string NomeFunc2
        {
            get { return BibliotecaTexto.GetFirstName(_nomeFunc2); }
            set { _nomeFunc2 = value; }
        }

        private string _nomeFunc3;

        [PersistenceProperty("FUNC3", DirectionParameter.InputOptional)]
        public string NomeFunc3
        {
            get { return BibliotecaTexto.GetFirstName(_nomeFunc3); }
            set { _nomeFunc3 = value; }
        }

        private string _nomeFunc4;

        [PersistenceProperty("FUNC4", DirectionParameter.InputOptional)]
        public string NomeFunc4
        {
            get { return BibliotecaTexto.GetFirstName(_nomeFunc4); }
            set { _nomeFunc4 = value; }
        }

        private string _nomeFunc5;

        [PersistenceProperty("FUNC5", DirectionParameter.InputOptional)]
        public string NomeFunc5
        {
            get { return BibliotecaTexto.GetFirstName(_nomeFunc5); }
            set { _nomeFunc5 = value; }
        }

        private string _nomeFunc6;

        [PersistenceProperty("FUNC6", DirectionParameter.InputOptional)]
        public string NomeFunc6
        {
            get { return BibliotecaTexto.GetFirstName(_nomeFunc6); }
            set { _nomeFunc6 = value; }
        }

        private string _nomeFunc7;

        [PersistenceProperty("FUNC7", DirectionParameter.InputOptional)]
        public string NomeFunc7
        {
            get { return BibliotecaTexto.GetFirstName(_nomeFunc7); }
            set { _nomeFunc7= value; }
        }

        private string _nomeFunc8;

        [PersistenceProperty("FUNC8", DirectionParameter.InputOptional)]
        public string NomeFunc8
        {
            get { return BibliotecaTexto.GetFirstName(_nomeFunc8); }
            set { _nomeFunc8 = value; }
        }

        private string _nomeFunc9;

        [PersistenceProperty("FUNC9", DirectionParameter.InputOptional)]
        public string NomeFunc9
        {
            get { return BibliotecaTexto.GetFirstName(_nomeFunc9); }
            set { _nomeFunc9 = value; }
        }

        private string _nomeFunc10;

        [PersistenceProperty("FUNC10", DirectionParameter.InputOptional)]
        public string NomeFunc10
        {
            get { return BibliotecaTexto.GetFirstName(_nomeFunc10); }
            set { _nomeFunc10 = value; }
        }

        private string _nomeFunc11;

        [PersistenceProperty("FUNC11", DirectionParameter.InputOptional)]
        public string NomeFunc11
        {
            get { return BibliotecaTexto.GetFirstName(_nomeFunc11); }
            set { _nomeFunc11 = value; }
        }

        private string _nomeFunc12;

        [PersistenceProperty("FUNC12", DirectionParameter.InputOptional)]
        public string NomeFunc12
        {
            get { return BibliotecaTexto.GetFirstName(_nomeFunc12); }
            set { _nomeFunc12 = value; }
        }

        private string _nomeFunc13;

        [PersistenceProperty("FUNC13", DirectionParameter.InputOptional)]
        public string NomeFunc13
        {
            get { return BibliotecaTexto.GetFirstName(_nomeFunc13); }
            set { _nomeFunc13 = value; }
        }

        private string _nomeFunc14;

        [PersistenceProperty("FUNC14", DirectionParameter.InputOptional)]
        public string NomeFunc14
        {
            get { return BibliotecaTexto.GetFirstName(_nomeFunc14); }
            set { _nomeFunc14 = value; }
        }

        private string _nomeFunc15;

        [PersistenceProperty("FUNC15", DirectionParameter.InputOptional)]
        public string NomeFunc15
        {
            get { return BibliotecaTexto.GetFirstName(_nomeFunc15); }
            set { _nomeFunc15 = value; }
        }

        private string _nomeFunc16;

        [PersistenceProperty("FUNC16", DirectionParameter.InputOptional)]
        public string NomeFunc16
        {
            get { return BibliotecaTexto.GetFirstName(_nomeFunc16); }
            set { _nomeFunc16 = value; }
        }

        private string _nomeFunc17;

        [PersistenceProperty("FUNC17", DirectionParameter.InputOptional)]
        public string NomeFunc17
        {
            get { return BibliotecaTexto.GetFirstName(_nomeFunc17); }
            set { _nomeFunc17 = value; }
        }

        private string _nomeFunc18;

        [PersistenceProperty("FUNC18", DirectionParameter.InputOptional)]
        public string NomeFunc18
        {
            get { return BibliotecaTexto.GetFirstName(_nomeFunc18); }
            set { _nomeFunc18 = value; }
        }

        private string _nomeFunc19;

        [PersistenceProperty("FUNC19", DirectionParameter.InputOptional)]
        public string NomeFunc19
        {
            get { return BibliotecaTexto.GetFirstName(_nomeFunc19); }
            set { _nomeFunc19 = value; }
        }

        private string _nomeFunc20;

        [PersistenceProperty("FUNC20", DirectionParameter.InputOptional)]
        public string NomeFunc20
        {
            get { return BibliotecaTexto.GetFirstName(_nomeFunc20); }
            set { _nomeFunc20 = value; }
        }

        private string _nomeFunc21;

        [PersistenceProperty("FUNC21", DirectionParameter.InputOptional)]
        public string NomeFunc21
        {
            get { return BibliotecaTexto.GetFirstName(_nomeFunc21); }
            set { _nomeFunc21 = value; }
        }

        private string _nomeFunc22;

        [PersistenceProperty("FUNC22", DirectionParameter.InputOptional)]
        public string NomeFunc22
        {
            get { return BibliotecaTexto.GetFirstName(_nomeFunc22); }
            set { _nomeFunc22 = value; }
        }

        private string _nomeFunc23;

        [PersistenceProperty("FUNC23", DirectionParameter.InputOptional)]
        public string NomeFunc23
        {
            get { return BibliotecaTexto.GetFirstName(_nomeFunc23); }
            set { _nomeFunc23 = value; }
        }

        private string _nomeFunc24;

        [PersistenceProperty("FUNC24", DirectionParameter.InputOptional)]
        public string NomeFunc24
        {
            get { return BibliotecaTexto.GetFirstName(_nomeFunc24); }
            set { _nomeFunc24 = value; }
        }

        private string _nomeFunc25;

        [PersistenceProperty("FUNC25", DirectionParameter.InputOptional)]
        public string NomeFunc25
        {
            get { return BibliotecaTexto.GetFirstName(_nomeFunc25); }
            set { _nomeFunc25 = value; }
        }

        private string _nomeFunc26;

        [PersistenceProperty("FUNC26", DirectionParameter.InputOptional)]
        public string NomeFunc26
        {
            get { return BibliotecaTexto.GetFirstName(_nomeFunc26); }
            set { _nomeFunc26 = value; }
        }

        private string _nomeFunc27;

        [PersistenceProperty("FUNC27", DirectionParameter.InputOptional)]
        public string NomeFunc27
        {
            get { return BibliotecaTexto.GetFirstName(_nomeFunc27); }
            set { _nomeFunc27 = value; }
        }

        private string _nomeFunc28;

        [PersistenceProperty("FUNC28", DirectionParameter.InputOptional)]
        public string NomeFunc28
        {
            get { return BibliotecaTexto.GetFirstName(_nomeFunc28); }
            set { _nomeFunc28 = value; }
        }

        private string _nomeFunc29;

        [PersistenceProperty("FUNC29", DirectionParameter.InputOptional)]
        public string NomeFunc29
        {
            get { return BibliotecaTexto.GetFirstName(_nomeFunc29); }
            set { _nomeFunc29 = value; }
        }

        [PersistenceProperty("SETORCORTE0", DirectionParameter.InputOptional)]
        public bool SetorCorte0 { get; set; }

        [PersistenceProperty("SETORCORTE1", DirectionParameter.InputOptional)]
        public bool SetorCorte1 { get; set; }

        [PersistenceProperty("SETORCORTE2", DirectionParameter.InputOptional)]
        public bool SetorCorte2 { get; set; }

        [PersistenceProperty("SETORCORTE3", DirectionParameter.InputOptional)]
        public bool SetorCorte3 { get; set; }

        [PersistenceProperty("SETORCORTE4", DirectionParameter.InputOptional)]
        public bool SetorCorte4 { get; set; }

        [PersistenceProperty("SETORCORTE5", DirectionParameter.InputOptional)]
        public bool SetorCorte5 { get; set; }

        [PersistenceProperty("SETORCORTE6", DirectionParameter.InputOptional)]
        public bool SetorCorte6 { get; set; }

        [PersistenceProperty("SETORCORTE7", DirectionParameter.InputOptional)]
        public bool SetorCorte7 { get; set; }

        [PersistenceProperty("SETORCORTE8", DirectionParameter.InputOptional)]
        public bool SetorCorte8 { get; set; }

        [PersistenceProperty("SETORCORTE9", DirectionParameter.InputOptional)]
        public bool SetorCorte9 { get; set; }

        [PersistenceProperty("SETORCORTE10", DirectionParameter.InputOptional)]
        public bool SetorCorte10 { get; set; }

        [PersistenceProperty("SETORCORTE11", DirectionParameter.InputOptional)]
        public bool SetorCorte11 { get; set; }

        [PersistenceProperty("SETORCORTE12", DirectionParameter.InputOptional)]
        public bool SetorCorte12 { get; set; }

        [PersistenceProperty("SETORCORTE13", DirectionParameter.InputOptional)]
        public bool SetorCorte13 { get; set; }

        [PersistenceProperty("SETORCORTE14", DirectionParameter.InputOptional)]
        public bool SetorCorte14 { get; set; }

        [PersistenceProperty("SETORCORTE15", DirectionParameter.InputOptional)]
        public bool SetorCorte15 { get; set; }

        [PersistenceProperty("SETORCORTE16", DirectionParameter.InputOptional)]
        public bool SetorCorte16 { get; set; }

        [PersistenceProperty("SETORCORTE17", DirectionParameter.InputOptional)]
        public bool SetorCorte17 { get; set; }

        [PersistenceProperty("SETORCORTE18", DirectionParameter.InputOptional)]
        public bool SetorCorte18 { get; set; }

        [PersistenceProperty("SETORCORTE19", DirectionParameter.InputOptional)]
        public bool SetorCorte19 { get; set; }

        [PersistenceProperty("SETORCORTE20", DirectionParameter.InputOptional)]
        public bool SetorCorte20 { get; set; }

        [PersistenceProperty("SETORCORTE21", DirectionParameter.InputOptional)]
        public bool SetorCorte21 { get; set; }

        [PersistenceProperty("SETORCORTE22", DirectionParameter.InputOptional)]
        public bool SetorCorte22 { get; set; }

        [PersistenceProperty("SETORCORTE23", DirectionParameter.InputOptional)]
        public bool SetorCorte23 { get; set; }

        [PersistenceProperty("SETORCORTE24", DirectionParameter.InputOptional)]
        public bool SetorCorte24 { get; set; }

        [PersistenceProperty("SETORCORTE25", DirectionParameter.InputOptional)]
        public bool SetorCorte25 { get; set; }

        [PersistenceProperty("SETORCORTE26", DirectionParameter.InputOptional)]
        public bool SetorCorte26 { get; set; }

        [PersistenceProperty("SETORCORTE27", DirectionParameter.InputOptional)]
        public bool SetorCorte27 { get; set; }

        [PersistenceProperty("SETORCORTE28", DirectionParameter.InputOptional)]
        public bool SetorCorte28 { get; set; }

        [PersistenceProperty("SETORCORTE29", DirectionParameter.InputOptional)]
        public bool SetorCorte29 { get; set; }

        [PersistenceProperty("SETORROTEIRO0", DirectionParameter.InputOptional)]
        public bool SetorRoteiro0 { get; set; }

        [PersistenceProperty("SETORROTEIRO1", DirectionParameter.InputOptional)]
        public bool SetorRoteiro1 { get; set; }

        [PersistenceProperty("SETORROTEIRO2", DirectionParameter.InputOptional)]
        public bool SetorRoteiro2 { get; set; }

        [PersistenceProperty("SETORROTEIRO3", DirectionParameter.InputOptional)]
        public bool SetorRoteiro3 { get; set; }

        [PersistenceProperty("SETORROTEIRO4", DirectionParameter.InputOptional)]
        public bool SetorRoteiro4 { get; set; }

        [PersistenceProperty("SETORROTEIRO5", DirectionParameter.InputOptional)]
        public bool SetorRoteiro5 { get; set; }

        [PersistenceProperty("SETORROTEIRO6", DirectionParameter.InputOptional)]
        public bool SetorRoteiro6 { get; set; }

        [PersistenceProperty("SETORROTEIRO7", DirectionParameter.InputOptional)]
        public bool SetorRoteiro7 { get; set; }

        [PersistenceProperty("SETORROTEIRO8", DirectionParameter.InputOptional)]
        public bool SetorRoteiro8 { get; set; }

        [PersistenceProperty("SETORROTEIRO9", DirectionParameter.InputOptional)]
        public bool SetorRoteiro9 { get; set; }

        [PersistenceProperty("SETORROTEIRO10", DirectionParameter.InputOptional)]
        public bool SetorRoteiro10 { get; set; }

        [PersistenceProperty("SETORROTEIRO11", DirectionParameter.InputOptional)]
        public bool SetorRoteiro11 { get; set; }

        [PersistenceProperty("SETORROTEIRO12", DirectionParameter.InputOptional)]
        public bool SetorRoteiro12 { get; set; }

        [PersistenceProperty("SETORROTEIRO13", DirectionParameter.InputOptional)]
        public bool SetorRoteiro13 { get; set; }

        [PersistenceProperty("SETORROTEIRO14", DirectionParameter.InputOptional)]
        public bool SetorRoteiro14 { get; set; }

        [PersistenceProperty("SETORROTEIRO15", DirectionParameter.InputOptional)]
        public bool SetorRoteiro15 { get; set; }

        [PersistenceProperty("SETORROTEIRO16", DirectionParameter.InputOptional)]
        public bool SetorRoteiro16 { get; set; }

        [PersistenceProperty("SETORROTEIRO17", DirectionParameter.InputOptional)]
        public bool SetorRoteiro17 { get; set; }

        [PersistenceProperty("SETORROTEIRO18", DirectionParameter.InputOptional)]
        public bool SetorRoteiro18 { get; set; }

        [PersistenceProperty("SETORROTEIRO19", DirectionParameter.InputOptional)]
        public bool SetorRoteiro19 { get; set; }

        [PersistenceProperty("SETORROTEIRO20", DirectionParameter.InputOptional)]
        public bool SetorRoteiro20 { get; set; }

        [PersistenceProperty("SETORROTEIRO21", DirectionParameter.InputOptional)]
        public bool SetorRoteiro21 { get; set; }

        [PersistenceProperty("SETORROTEIRO22", DirectionParameter.InputOptional)]
        public bool SetorRoteiro22 { get; set; }

        [PersistenceProperty("SETORROTEIRO23", DirectionParameter.InputOptional)]
        public bool SetorRoteiro23 { get; set; }

        [PersistenceProperty("SETORROTEIRO24", DirectionParameter.InputOptional)]
        public bool SetorRoteiro24 { get; set; }

        [PersistenceProperty("SETORROTEIRO25", DirectionParameter.InputOptional)]
        public bool SetorRoteiro25 { get; set; }

        [PersistenceProperty("SETORROTEIRO26", DirectionParameter.InputOptional)]
        public bool SetorRoteiro26 { get; set; }

        [PersistenceProperty("SETORROTEIRO27", DirectionParameter.InputOptional)]
        public bool SetorRoteiro27 { get; set; }

        [PersistenceProperty("SETORROTEIRO28", DirectionParameter.InputOptional)]
        public bool SetorRoteiro28 { get; set; }

        [PersistenceProperty("SETORROTEIRO29", DirectionParameter.InputOptional)]
        public bool SetorRoteiro29 { get; set; }

        #endregion
    }
}