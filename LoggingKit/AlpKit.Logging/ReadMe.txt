3 tip logger kullanılabilir ; 

ElasticLogger
Bu loggerı kullanabilmek için DI a ElasticLoggerConfiguration kayıt edilmelidir.
Sonrasında AddElasticLogger extension metodu çagırılarak aktif hale getirilir.

FileLogger
Bu loggerı kullanabilmek için DI a FileLoggerConfiguration kayıt edilmelidir.
Sonrasında AddFileLogger extension metodu çagırılarak aktif hale getirilir.

MsSqlLogger
Bu loggerı kullanabilmek için DI a MsSqlLoggerConfiguration kayıt edilmelidir.
Sonrasında AddMsSqlLogger extension metodu çagırılarak aktif hale getirilir.

Gerekli implementasyon tamamlandıktan sonra LoggerServiceBase DI a geçildiğinde istenilen tipte loglama işlemi gerçekleştirilebilir.


RequestLogger kullanılacaksa RequestLoggingOptions ı doldurarak DI a kayıt edin.
Sonrasında UseRequestLogging middleware eklenmelidir.
aşagıdaki formatta loglarınız akmaya başlayacaktır.

Request URL: {url},Time: {DateTime.Now} Method: {method}, QueryString: {queryString}, IP Address: {ipAddress}, Body: {body}
