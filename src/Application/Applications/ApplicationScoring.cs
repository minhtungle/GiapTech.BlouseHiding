using GiapTech.BlouseHiding.Domain.Entities;

namespace GiapTech.BlouseHiding.Application.Applications;

// Giai đoạn 1: match-score dựa trên trường có cấu trúc sẵn có (không NLP/AI) — xem ERD mục 4.8.
// HR vẫn là người quyết định qua Kanban, score chỉ là gợi ý sắp xếp phụ (có thể ghi đè thủ công).
public static class ApplicationScoring
{
    public static int Calculate(CandidateProfile profile, Job job)
    {
        var score = 0;

        if (profile.Specialties.Any(s => s.SpecialtyId == job.SpecialtyId))
        {
            score += 50;
        }

        if (profile.Licenses.Any(l => l.VerifyStatus == Domain.Enums.LicenseVerifyStatus.Verified))
        {
            score += 30;
        }

        if (profile.LocationId is not null && profile.LocationId == job.LocationId)
        {
            score += 20;
        }

        return score;
    }
}
