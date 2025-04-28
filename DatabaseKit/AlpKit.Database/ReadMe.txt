Mongo ve Mssql adapterlarının temelini oluşturur.
Bu 2 adapter dışında 2 adet NameValueCollection extension metodu barındırır ;

ToPageRequest
Bu extension metod NameValueCollectionda bulunan Page(default 1) ve PageSize (default 10) değerlerini querystringten alarak filtrelemelerde kullanılacak PageRequest modelini döner.

ToDynamicFilter<T>
T tipinde geçilen modelin propertylerini içeren bir anahtar değeri querystringte görürse filtrelenecek alanlar arasına bu değeri ekleyerek DynamicQuery modeli döndürür.
Ayrıca Sort.Field (property adı) ve Sort.OrderOperator (asc veya desc) değerlerini de querystringten alır ve sıralama işlemi gerçekleştirir.

Repository temel sınıfları ,metotları ve modelleri burada bulunur.