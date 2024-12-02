namespace Cachey.Persistence.SQLite;

public class CacheItemEntity
{
    public string Key { get; set; } // Кључ кеша (извршава идентификацију кеша)

    public string
        Value
    {
        get;
        set;
    } // Вредност кеша, која ће бити серијализована у стринг (мора се десеријализовати приликом читања)

    public DateTime CreatedAt { get; set; } // Време када је ставка додата
    public DateTime? Expiration { get; set; } // Време истека кеша
}