namespace GiapTech.BlouseHiding.Application.Catalog.Queries.GetEmploymentTypes;

// Enum cố định, không lưu bản dịch trong DB — nhãn hiển thị lấy từ file dịch phía frontend
// (xem docs/backend/API-DESIGN.md mục 10).
public record GetEmploymentTypesQuery : IQuery<List<string>>;

public class GetEmploymentTypesQueryHandler : IQueryHandler<GetEmploymentTypesQuery, List<string>>
{
    public ValueTask<List<string>> Handle(GetEmploymentTypesQuery query, CancellationToken cancellationToken)
    {
        var values = Enum.GetNames<EmploymentType>().ToList();
        return ValueTask.FromResult(values);
    }
}
