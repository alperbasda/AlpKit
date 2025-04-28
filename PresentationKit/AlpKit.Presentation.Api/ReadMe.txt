ToResult<T> extension metodunu bulundurur.
Response tipindeki dönüş verimizi uygun http minimalApi tipine dönüştürerek döner.

İçerisinde 3 adet EndpointFilter bulunmaktadır ; 

AuthorizationFilter
Jwt Authentication mekanizmasını yönetir.
Kullanabilmek için AuthSettings singleton olarak DI a eklenmelidir.
HeaderConstants.Authorization constant değerini header da arar ve AuthSettings modelindeki bilgilere göre doğrulama yapar.

FillTokenParameterFilter
Herhangi bir token dogrulaması yapmaz tek ilgilendiği şey token okunabiliyor mu okunuyorsa içindeki verilerdir.
HeaderConstants sınıfı içerisindeki headersları yakalayıp TokenParameters içerisine doldurur. ve AdminScope constants değerine sahip ise IsSuperUser bayrağını true ya set eder.
Kullanabilmek için AuthSettings singleton ve TokenParameters scoped olarak DI a eklenmelidir.

FluentValidationFilter<T>
Request ile alınan model için bir FluentValidation Validatorü tanımlanmış ise kontrol sağlar. Hata var ise hatayı geri döner.

