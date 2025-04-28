Redis Cache
AddRedisCache servis extension metodu çagırılmalı ve CacheSettings sınıfı dolu birşekilde iletilmelidir.

Memory Cache
AddMemCache servis extension metodu çagırılmalıdır.

Her iki implementasyonda da DI dan ICacheHelper geçildiğinde içerdiği metodlar seçilen yönteme göre çalışacaktır.
