using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Common.Consts;

public static class FiltersOptions
{
    public const string PriceAsc = "price_asc";
    public const string PriceDesc = "price_desc";

    public const string RateAsc = "rate_asc";
    public const string RateDesc = "rate_desc";

    public const string Clinic = "clinic";
    public const string Online = "online";
    public const string Home = "home";



    public static readonly IList<string> SortFilters = [PriceAsc, PriceDesc, RateAsc, RateDesc];
    public static readonly IList<string> AppointmentTypes = [Clinic, Online, Home];

}
