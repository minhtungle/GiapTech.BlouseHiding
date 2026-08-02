using GiapTech.BlouseHiding.Domain.Constants;
using GiapTech.BlouseHiding.Domain.Entities;
using GiapTech.BlouseHiding.Domain.Enums;
using GiapTech.BlouseHiding.Infrastructure.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace GiapTech.BlouseHiding.Infrastructure.Data;

public static class InitialiserExtensions
{
    public static async Task InitialiseDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var initialiser = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitialiser>();

        await initialiser.InitialiseAsync();
        await initialiser.SeedAsync();
    }
}

public class ApplicationDbContextInitialiser
{
    private readonly ILogger<ApplicationDbContextInitialiser> _logger;
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public ApplicationDbContextInitialiser(ILogger<ApplicationDbContextInitialiser> logger, ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _logger = logger;
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task InitialiseAsync()
    {
        try
        {
            // Dùng migration thật (docs/database/QUY-UOC-MIGRATION.md), KHÔNG dùng
            // EnsureDeleted+EnsureCreated của template mặc định — cách đó xóa sạch DB mỗi lần khởi
            // động dev và bỏ qua __EFMigrationsHistory, không phù hợp khi đã có migration thật.
            await _context.Database.MigrateAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while initialising the database.");
            throw;
        }
    }

    public async Task SeedAsync()
    {
        try
        {
            await TrySeedAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }

    public async Task TrySeedAsync()
    {
        // Default roles — xem docs/kien-truc/THUAT-NGU.md (role backend != tên site hiển thị)
        foreach (var roleName in new[] { Roles.Candidate, Roles.Employer, Roles.Admin, Roles.Moderator })
        {
            if (_roleManager.Roles.All(r => r.Name != roleName))
            {
                await _roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        // Default user — role "admin" (đội Vận hành nội bộ), dùng cho dev/test cục bộ
        var administrator = new ApplicationUser { UserName = "admin@localhost", Email = "admin@localhost" };

        if (_userManager.Users.All(u => u.UserName != administrator.UserName))
        {
            await _userManager.CreateAsync(administrator, "Administrator1!");
            await _userManager.AddToRolesAsync(administrator, new[] { Roles.Admin });
        }

        await SeedCatalogAsync();
    }

    // Danh mục chuẩn — bản dịch đủ 5 ngôn ngữ (en/ja/zh/ko/es) theo CLAUDE.md mục 4 quy tắc 10
    // (không để trống rồi tính sau). vi lấy từ cột Name của bảng gốc, không lưu ở translations.
    private async Task SeedCatalogAsync()
    {
        if (!_context.Specialties.Any())
        {
            var specialties = new (string Code, string Vi, string En, string Ja, string Zh, string Ko, string Es)[]
            {
                ("hscc", "Hồi sức cấp cứu", "Emergency & Critical Care", "救急・集中治療科", "急诊重症监护科", "응급 중환자 치료과", "Cuidados Intensivos y Emergencias"),
                ("duoc", "Dược", "Pharmacy", "薬剤科", "药剂科", "약제과", "Farmacia"),
                ("xet-nghiem", "Xét nghiệm", "Laboratory Testing", "検査科", "检验科", "검사과", "Laboratorio Clínico"),
                ("noi-tong-quat", "Nội tổng quát", "General Internal Medicine", "総合内科", "普通内科", "일반내과", "Medicina Interna General"),
                ("ngoai-khoa", "Ngoại khoa", "Surgery", "外科", "外科", "외과", "Cirugía"),
                ("san-phu-khoa", "Sản phụ khoa", "Obstetrics & Gynecology", "産婦人科", "妇产科", "산부인과", "Obstetricia y Ginecología"),
                ("nhi-khoa", "Nhi khoa", "Pediatrics", "小児科", "儿科", "소아과", "Pediatría"),
                ("rhm", "Răng Hàm Mặt", "Dentistry", "歯科", "口腔科", "치과", "Odontología"),
            };

            foreach (var s in specialties)
            {
                _context.Specialties.Add(new Specialty
                {
                    Code = s.Code,
                    Name = s.Vi,
                    Translations =
                    [
                        new SpecialtyTranslation { Locale = "en", Name = s.En },
                        new SpecialtyTranslation { Locale = "ja", Name = s.Ja },
                        new SpecialtyTranslation { Locale = "zh", Name = s.Zh },
                        new SpecialtyTranslation { Locale = "ko", Name = s.Ko },
                        new SpecialtyTranslation { Locale = "es", Name = s.Es },
                    ],
                });
            }
        }

        if (!_context.Locations.Any())
        {
            var locations = new (string Vi, string En, string Ja, string Zh, string Ko, string Es)[]
            {
                ("TP. Hồ Chí Minh", "Ho Chi Minh City", "ホーチミン市", "胡志明市", "호치민시", "Ciudad Ho Chi Minh"),
                ("Hà Nội", "Hanoi", "ハノイ", "河内", "하노이", "Hanói"),
                ("Đà Nẵng", "Da Nang", "ダナン", "岘港", "다낭", "Da Nang"),
                ("Cần Thơ", "Can Tho", "カントー", "芹苴", "껀터", "Can Tho"),
                ("Hải Phòng", "Hai Phong", "ハイフォン", "海防", "하이퐁", "Hai Phong"),
            };

            foreach (var l in locations)
            {
                _context.Locations.Add(new Location
                {
                    Name = l.Vi,
                    Translations =
                    [
                        new LocationTranslation { Locale = "en", Name = l.En },
                        new LocationTranslation { Locale = "ja", Name = l.Ja },
                        new LocationTranslation { Locale = "zh", Name = l.Zh },
                        new LocationTranslation { Locale = "ko", Name = l.Ko },
                        new LocationTranslation { Locale = "es", Name = l.Es },
                    ],
                });
            }
        }

        if (!_context.JobPackages.Any())
        {
            var packages = new (JobPackageTier Tier, string Vi, string En, string Ja, string Zh, string Ko, string Es, int Days, decimal Price, int? MaxJobs, Dictionary<string, bool> Perks)[]
            {
                (JobPackageTier.Free, "Free", "Free", "無料", "免费", "무료", "Gratis", 7, 0m, 1, new Dictionary<string, bool>()),
                (JobPackageTier.Eco, "Eco", "Eco", "Eco", "Eco", "Eco", "Eco", 30, 490000m, null, new Dictionary<string, bool> { ["pin_top"] = false, ["highlight"] = false }),
                (JobPackageTier.Pro, "Pro", "Pro", "Pro", "Pro", "Pro", "Pro", 30, 990000m, null, new Dictionary<string, bool> { ["pin_top"] = false, ["highlight"] = true }),
                (JobPackageTier.Max, "Max", "Max", "Max", "Max", "Max", "Max", 45, 1990000m, null, new Dictionary<string, bool> { ["pin_top"] = true, ["highlight"] = true }),
            };

            foreach (var p in packages)
            {
                _context.JobPackages.Add(new JobPackage
                {
                    Tier = p.Tier,
                    Name = p.Vi,
                    DurationDays = p.Days,
                    Price = p.Price,
                    MaxActiveJobs = p.MaxJobs,
                    Perks = p.Perks,
                    Translations =
                    [
                        new JobPackageTranslation { Locale = "en", Name = p.En },
                        new JobPackageTranslation { Locale = "ja", Name = p.Ja },
                        new JobPackageTranslation { Locale = "zh", Name = p.Zh },
                        new JobPackageTranslation { Locale = "ko", Name = p.Ko },
                        new JobPackageTranslation { Locale = "es", Name = p.Es },
                    ],
                });
            }
        }

        await _context.SaveChangesAsync(CancellationToken.None);
    }
}
