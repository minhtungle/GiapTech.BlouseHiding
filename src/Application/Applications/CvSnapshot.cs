using System.Text.Json;
using GiapTech.BlouseHiding.Domain.Entities;

namespace GiapTech.BlouseHiding.Application.Applications;

// Chụp hồ sơ tại thời điểm ứng tuyển (ERD mục 4.8) — sửa hồ sơ gốc sau này KHÔNG ảnh hưởng bản chụp
// này. CV Builder (bảng cvs) chưa làm nên chụp trực tiếp từ CandidateProfile hiện có; khi CV Builder
// ra mắt, đổi nguồn dữ liệu đầu vào ở đây mà không đổi luồng nghiệp vụ SubmitApplicationCommand.
public record CvSnapshotData
{
    public string FullName { get; init; } = string.Empty;
    public string? Headline { get; init; }
    public string? Summary { get; init; }
    public List<CvSnapshotSpecialty> Specialties { get; init; } = [];
    public List<CvSnapshotLicense> Licenses { get; init; } = [];
}

public record CvSnapshotSpecialty(Guid SpecialtyId, string Level);

public record CvSnapshotLicense(string LicenseNo, string VerifyStatus);

public static class CvSnapshotBuilder
{
    public static string Build(CandidateProfile profile)
    {
        var data = new CvSnapshotData
        {
            FullName = profile.FullName,
            Headline = profile.Headline,
            Summary = profile.Summary,
            Specialties = profile.Specialties
                .Select(s => new CvSnapshotSpecialty(s.SpecialtyId, s.Level.ToString()))
                .ToList(),
            Licenses = profile.Licenses
                .Select(l => new CvSnapshotLicense(l.LicenseNo, l.VerifyStatus.ToString()))
                .ToList(),
        };

        return JsonSerializer.Serialize(data);
    }
}
