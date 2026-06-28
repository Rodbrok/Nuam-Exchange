namespace NuamExchange.Application.Authentication;

public static class RoleNames
{
    public const string Administrador = "Administrador";
    public const string AnalistaTributario = "Analista Tributario";
    public const string Supervisor = "Supervisor";

    public static readonly IReadOnlyList<string> All = [Administrador, AnalistaTributario, Supervisor];
}
