using System.Text.Json.Serialization;
using Azure.Identity;
using GiapTech.BlouseHiding.Application.Common.Interfaces;
using GiapTech.BlouseHiding.Infrastructure.Data;
using GiapTech.BlouseHiding.Web.Services;
using Microsoft.AspNetCore.Mvc;
using HttpJsonOptions = Microsoft.AspNetCore.Http.Json.JsonOptions;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static void AddWebServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddDatabaseDeveloperPageExceptionFilter();

        builder.Services.AddScoped<IUser, CurrentUser>();
        builder.Services.AddScoped<ICurrentLocale, CurrentLocale>();

        builder.Services.AddHttpContextAccessor();

        builder.Services.AddExceptionHandler<ProblemDetailsExceptionHandler>();

        // Customise default API behaviour
        builder.Services.Configure<ApiBehaviorOptions>(options =>
            options.SuppressModelStateInvalidFilter = true);

        // Enum dạng string trong JSON (vd "BenhVienTu" thay vì số thứ tự) — dễ đọc/ổn định hơn khi
        // thêm giá trị enum mới ở giữa, khớp ví dụ response trong docs/backend/API-DESIGN.md.
        builder.Services.Configure<HttpJsonOptions>(options =>
            options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));

        builder.Services.AddEndpointsApiExplorer();

        builder.Services.AddOpenApi(options =>
        {
            options.AddOperationTransformer<ApiExceptionOperationTransformer>();
            options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
        });

        builder.Services.AddCors();
    }

    public static void AddKeyVaultIfConfigured(this IHostApplicationBuilder builder)
    {
        var keyVaultUri = builder.Configuration["AZURE_KEY_VAULT_ENDPOINT"];
        if (!string.IsNullOrWhiteSpace(keyVaultUri))
        {
            builder.Configuration.AddAzureKeyVault(
                new Uri(keyVaultUri),
                new DefaultAzureCredential());
        }
    }
}
