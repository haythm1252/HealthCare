using System;
using System.Collections.Generic;
using System.Text;

namespace HealthCare.Application.Common.Consts;

public static class EgyptGovernorates
{
    public static readonly List<string> All =
    [
        "Cairo",
        "Giza",
        "Alexandria",
        "Port Said",
        "Suez",
        "Luxor",
        "Aswan",
        "Assiut",
        "Beheira",
        "Beni Suef",
        "Dakahlia",
        "Damietta",
        "Faiyum",
        "Gharbia",
        "Ismailia",
        "Kafr El Sheikh",
        "Matrouh",
        "Minya",
        "Monufia",
        "New Valley",
        "North Sinai",
        "Qalyubia",
        "Qena",
        "Red Sea",
        "Sharqia",
        "Sohag",
        "South Sinai"
    ];


    public static bool IsValid(string governorate)
        => !string.IsNullOrWhiteSpace(governorate)
           && All.Any(g => g.Equals(governorate, StringComparison.OrdinalIgnoreCase));
}