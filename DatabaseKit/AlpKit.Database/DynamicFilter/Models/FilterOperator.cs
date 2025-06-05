using System.ComponentModel;

namespace AlpKit.Database.DynamicFilter.Models
{
    public enum FilterOperator
    {
        [Description("Tanımsız")]
        Unknown,
        [Description("Eşit")]
        Equals,
        [Description("Eşit Değil")]
        DoesntEqual,
        [Description("Büyüktür")]
        GreaterThan,
        [Description("Büyük veya Eşittir")]
        GreaterThanOrEqual,
        [Description("Küçüktür")]
        LessThan,
        [Description("Küçük veya Eşittir")]
        LessThanOrEqual,
        [Description("İçerir")]
        Contains,
        [Description("İçerir. Harf Duyarsız")]
        ContainsIgnoreCase,
        [Description("İçermez")]
        NotContains,
        [Description("Başlar")]
        StartsWith,
        [Description("Biter")]
        EndsWith,
        [Description("Boş")]
        IsEmpty,
        [Description("Boş Değil")]
        IsNotEmpty,
        [Description("Özel İfade")]
        Raw
    }
    public enum OrderOperator
    {
        [Description("Tanımsız")]
        Unknown,
        [Description("asc")]
        Asc,
        [Description("desc")]
        Desc,

    }
    public enum Logic
    {
        [Description("and")]
        And,
        [Description("or")]
        Or,
    }
}
