using GiapTech.BlouseHiding.Application.Employers.Commands.CreateOrganization;
using GiapTech.BlouseHiding.Application.Employers.Queries.GetMyOrganizations;
using GiapTech.BlouseHiding.Domain.Constants;
using GiapTech.BlouseHiding.Domain.Entities;
using GiapTech.BlouseHiding.Domain.Enums;

namespace GiapTech.BlouseHiding.Application.FunctionalTests.Employers;

public class CreateOrganizationTests : TestBase
{
    [Test]
    public async Task Should_Create_Organization_With_Owner_Member()
    {
        var ownerId = await TestApp.RunAsUserAsync($"employer-{Guid.NewGuid():N}@test.local", "Testing1234!", [Roles.Employer]);

        var orgId = await TestApp.SendAsync<Guid>(new CreateOrganizationCommand
        {
            OwnerUserId = ownerId,
            Name = "Bệnh viện Test",
            OrgType = OrganizationType.BenhVienTu,
        });

        orgId.ShouldNotBe(Guid.Empty);

        var members = await TestApp.CountAsync<EmployerMember>();
        members.ShouldBe(1);
    }

    [Test]
    public async Task GetMyOrganizations_Should_Return_Orgs_Where_User_Is_Member()
    {
        var ownerId = await TestApp.RunAsUserAsync($"employer2-{Guid.NewGuid():N}@test.local", "Testing1234!", [Roles.Employer]);

        var orgId = await TestApp.SendAsync<Guid>(new CreateOrganizationCommand
        {
            OwnerUserId = ownerId,
            Name = "Bệnh viện Test 2",
            OrgType = OrganizationType.PhongKham,
        });

        var myOrgs = await TestApp.SendAsync<List<MyOrganizationDto>>(new GetMyOrganizationsQuery { UserId = ownerId });

        myOrgs.ShouldContain(o => o.Id == orgId && o.MemberRole == "Owner");
    }
}
