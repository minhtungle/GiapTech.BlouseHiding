namespace GiapTech.BlouseHiding.Shared;

public static class Services
{
    /// <summary>
    /// The name of the Web Frontend service.
    /// This service is responsible for hosting the frontend application.
    /// </summary>
    public const string WebFrontend = "webfrontend";

    /// <summary>
    /// The name of the Web API service.
    /// This service is responsible for hosting the Web API application.
    /// </summary>
    public const string WebApi = "webapi";

    /// <summary>
    /// The name of the Database Server service.
    /// This service is responsible for hosting the database server (e.g., PostgreSQL, SQL Server, or SQLite).
    /// </summary>
    public const string DatabaseServer = "dbserver";

    /// <summary>
    /// The name of the Database — khớp key <c>ConnectionStrings:Postgres</c> theo quy ước ở
    /// docs/ha-tang/BIEN-MOI-TRUONG.md (biến môi trường tương ứng: <c>ConnectionStrings__Postgres</c>).
    /// </summary>
    public const string Database = "Postgres";
}
