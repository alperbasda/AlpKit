AddMongoDbContext<TContext> extension metodunu içerir.
Bu metoda MongoSettings ile ayarlar geçilmelidir.

EF core ve AlpKit.Database kullanarak bir mongo db entegrasyonu gerçekler.
Soyut Repositorylerinizi IAsyncRepository, Somut Repositorylerinizi RepositoryBase den türetmeniz yeterli olacaktır.
