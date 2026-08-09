using Domain.Errors;
using Domain.Exceptions;

namespace Domain.Entities;

public partial class Season
{
    public void UpdateDates(DateOnly? startDate, DateOnly? endDate)
    {
        if (startDate.HasValue && endDate.HasValue && endDate < startDate)
            throw new ValidationException(SeasonErrors.EndDateBeforeStartDate());

        StartDate = startDate;
        EndDate = endDate;
        UpdatedOnUtc = UtcNow;
    }
}
