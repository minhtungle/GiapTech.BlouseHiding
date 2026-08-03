namespace GiapTech.BlouseHiding.Domain.Entities;

public class ProfileSpecialty : BaseEntity
{
    public Guid ProfileId { get; set; }

    public CandidateProfile? Profile { get; set; }

    public Guid SpecialtyId { get; set; }

    public Specialty? Specialty { get; set; }

    public SpecialtyLevel Level { get; set; }
}
