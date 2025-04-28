Bir logger tipini aktif edin. (AddElasticLogger, AddFileLogger, AddMsSqlLogger). Bknz : AlpKit.Logging
AddExceptionHandlerı DI a ekleyin.
Bu extension a geçeceğiniz isWebProject parametresi ExceptionHandler Tipinizi belirlemek içn kullanılır.
Hataları direkt json model olarak dönmek isterseniz (api çağrıları için) false geçebilirsiniz.
Bir web projesinde hata sayfasına yönlendirmek ve TempData["Error"] a hata mesajını yazdırmak için true geçin.

UseExceptionHandlerı middleware olarak ekleyin.
Exception ların data RedirectUrl bilgisine göre hatayı istenilen sayfaya redirect edebilirsiniz.

public enum LogType
{
    None,
    Info,
    Warn,
    Error
}

Exception ların data LogType bilgisine göre log atılmayabilir (none) veya istediğiniz tipte loglama yapabilirsiniz.Bknz : LogDetailWithException sınıfı.



