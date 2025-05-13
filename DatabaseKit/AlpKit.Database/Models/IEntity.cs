namespace AlpKit.Database.Models;

public interface IEntity
{
    public Guid Id { get; set; }
    public DateTime CreatedTime { get; set; }
    public DateTime? UpdatedTime { get; set; }
    public DateTime? DeletedTime { get; set; }
}

public interface ITranslationParent<T>
    where T : IEntity
{
    public ICollection<T> Translations { get; set; }
}

public interface ITranslation
{
    public string LanguageCode { get; set; }
}
