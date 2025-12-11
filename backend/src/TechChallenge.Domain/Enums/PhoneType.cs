using System.Text.Json.Serialization;
using TechChallenge.Domain.JsonConverters;

namespace TechChallenge.Domain.Enums;

[JsonConverter(typeof(PhoneTypeJsonConverter))]
public enum PhoneType
{
    Mobile = 1,
    Landline = 2,
    Work = 3,
    Home = 4,
    Fax = 5,
    Emergency = 6,
    Other = 7
}
