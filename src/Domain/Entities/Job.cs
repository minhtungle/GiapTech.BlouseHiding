namespace GiapTech.BlouseHiding.Domain.Entities;

public class Job : BaseAuditableEntity
{
    public Guid OrganizationId { get; set; }

    public Organization? Organization { get; set; }

    public string Title { get; set; } = string.Empty;

    public Guid SpecialtyId { get; set; }

    public Specialty? Specialty { get; set; }

    public EmploymentType EmploymentType { get; set; }

    public int? SalaryMin { get; set; }

    public int? SalaryMax { get; set; }

    public bool SalaryNegotiable { get; set; }

    public Guid LocationId { get; set; }

    public Location? Location { get; set; }

    public string? AddressDetail { get; set; }

    public bool RequiredLicense { get; set; } = true;

    public int MinExperienceYears { get; set; }

    public string Description { get; set; } = string.Empty;

    public string? Requirements { get; set; }

    public string? Benefits { get; set; }

    public JobStatus Status { get; set; } = JobStatus.Draft;

    public string? RejectReason { get; set; }

    public DateTimeOffset? PublishedAt { get; set; }

    public DateTimeOffset? ExpiresAt { get; set; }

    public Guid CreatedByUserId { get; set; }

    public bool CanEdit => Status is JobStatus.Draft or JobStatus.Rejected;

    public void Submit(JobPackageTier tier, int durationDays)
    {
        if (!CanEdit)
        {
            throw new InvalidOperationException("Chỉ nộp duyệt được tin ở trạng thái draft hoặc rejected.");
        }

        Status = tier == JobPackageTier.Free ? JobStatus.Pending : JobStatus.PendingPayment;

        if (tier == JobPackageTier.Free)
        {
            ExpiresAt = DateTimeOffset.UtcNow.AddDays(durationDays);
        }
    }

    public void ConfirmPayment(int durationDays)
    {
        if (Status != JobStatus.PendingPayment)
        {
            throw new InvalidOperationException("Chỉ xác nhận thanh toán khi tin đang chờ thanh toán.");
        }

        Status = JobStatus.Pending;
        ExpiresAt = DateTimeOffset.UtcNow.AddDays(durationDays);
    }

    public void RejectPayment()
    {
        if (Status != JobStatus.PendingPayment)
        {
            throw new InvalidOperationException("Chỉ từ chối thanh toán khi tin đang chờ thanh toán.");
        }

        Status = JobStatus.Draft;
    }

    public void Moderate(bool approved, string? rejectReason, bool organizationVerified)
    {
        if (Status != JobStatus.Pending)
        {
            throw new InvalidOperationException("Chỉ duyệt nội dung khi tin đang chờ duyệt.");
        }

        if (approved)
        {
            if (!organizationVerified)
            {
                throw new InvalidOperationException("Chỉ publish tin khi tổ chức đã được xác thực.");
            }

            Status = JobStatus.Published;
            PublishedAt = DateTimeOffset.UtcNow;
            RejectReason = null;
        }
        else
        {
            Status = JobStatus.Rejected;
            RejectReason = rejectReason;
        }
    }

    public void Close()
    {
        if (Status is JobStatus.Closed or JobStatus.Expired)
        {
            throw new InvalidOperationException("Tin đã đóng hoặc hết hạn.");
        }

        Status = JobStatus.Closed;
    }

    public void Suspend()
    {
        Status = JobStatus.Suspended;
    }

    public Job CreateRenewalCopy()
    {
        var renewed = new Job
        {
            OrganizationId = OrganizationId,
            Title = Title,
            SpecialtyId = SpecialtyId,
            EmploymentType = EmploymentType,
            SalaryMin = SalaryMin,
            SalaryMax = SalaryMax,
            SalaryNegotiable = SalaryNegotiable,
            LocationId = LocationId,
            AddressDetail = AddressDetail,
            RequiredLicense = RequiredLicense,
            MinExperienceYears = MinExperienceYears,
            Description = Description,
            Requirements = Requirements,
            Benefits = Benefits,
            Status = JobStatus.Draft,
            CreatedByUserId = CreatedByUserId,
        };

        Close();

        return renewed;
    }
}
