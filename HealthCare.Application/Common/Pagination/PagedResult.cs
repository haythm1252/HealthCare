using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Common.Pagination;

public record PagedResult<T>
(
    int PageNumber,
    int PageSize,
    int TotalCount,
    IEnumerable<T> Items
);
