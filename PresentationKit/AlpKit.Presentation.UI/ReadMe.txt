Actionlara extension metod sağlar.
Örneğin return View().Success(your_message) veya return RedirecToAction("Index","Home").Error(your_message) extensionları ile TempData["Error"] ve TempData["Success"] tempdatalarını doldurur.;
TempData bilgileri cookie ve tempdatan faydalanacagı için
AddCookieAndTempData() servicecollection extension metodu çağırılmalıdır.


İçerisinde 3 adet Attribute bulunmaktadır ; 

CustomAuthorizationAttribute
cookie de tutulan Jwt Authentication mekanizmasını yönetir.
Kullanabilmek için AuthSettings singleton olarak DI a eklenmelidir.
WebConstants.Authorization constant değerini cookie da arar ve AuthSettings modelindeki bilgilere göre doğrulama yapar.

FillTokenParameterAttribute
Herhangi bir token dogrulaması yapmaz tek ilgilendiği şey token okunabiliyor mu okunuyorsa içindeki verilerdir.
WebConstants sınıfı içerisindeki cookieleri yakalayıp TokenParameters içerisine doldurur. ve AdminScope constants değerine sahip ise IsSuperUser bayrağını true ya set eder.
Kullanabilmek için AuthSettings singleton ve TokenParameters scoped olarak DI a eklenmelidir.

FluentValidationFilter<T>
Request ile alınan model için bir FluentValidation Validatorü tanımlanmış ise kontrol sağlar. Hata var ise Response<T> türünde bir hata modeli döner.

