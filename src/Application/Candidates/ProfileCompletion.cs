namespace GiapTech.BlouseHiding.Application.Candidates;

// Tính % hoàn thiện hồ sơ hiển thị ở dashboard ứng viên — trọng số đơn giản cho MVP, không cần chính
// xác tuyệt đối. 5 tiêu chí x 20% mỗi tiêu chí.
public static class ProfileCompletion
{
    public static int Calculate(Domain.Entities.CandidateProfile profile)
    {
        var score = 0;

        if (!string.IsNullOrWhiteSpace(profile.FullName)) score += 20;
        if (!string.IsNullOrWhiteSpace(profile.Headline)) score += 20;
        if (!string.IsNullOrWhiteSpace(profile.Summary)) score += 20;
        if (profile.Specialties.Count > 0) score += 20;
        if (profile.Licenses.Count > 0) score += 20;

        return score;
    }
}
