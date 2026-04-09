namespace IczTask.Options;

public class JwtOptions
{
    /// <summary>
    /// Větišnou bývá ještě dobré volit tento přístup, kde mám název sekce uložení přímo v třídě.
    /// 
    /// Poté by se místo "Jwt" v Progmra.cs použilo builder.Configuration.GetSection(JwtOptions.SectionName);
    /// </summary>
    public const string SectionName = "Jwt";
    public string Key { get; init; } = string.Empty;
    public string Issuer { get; init; } = string.Empty;
    public string Audience { get; init; } = string.Empty;
    public int ExpireMinutes { get; init; } = 60;
}