namespace Glass.Seguranca
{
    /// <summary>
    /// Possíveis tipos de funcionários.
    /// </summary>
    public enum TipoFuncionario : ushort
    {
        Gerente = 1,
        Vendedor,
        AuxAdministrativo,
        Ajudante,
        CaixaDiario,                    // 5
        Financeiro,
        FinanceiroPagto,
        AuxEscritorioMedicao,
        Medidor,
        SupervisorColocacaoComum,       // 10
        SupervisorColocacaoTemperado,
        SupervisorTemperado,
        Conferente,
        SupervisorProducao,
        MotoristaMedidor,               // 15
        MotoristaInstalador,
        InstaladorComum,
        InstaladorTemperado,
        Administrador,
        FinanceiroGeral,                // 20
        AuxAlmoxarifado = 30,
        AuxEscritorio = 38,
        MarcadorProducao = 196,
        AuxEtiqueta = 201,
        Fiscal = 202
    }
}
