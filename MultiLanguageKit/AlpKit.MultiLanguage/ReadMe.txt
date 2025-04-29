Proje seviyesinde bir adet localization.json dosyası bulunmalı ve içeriği şu şekilde devam etmeli.
[
 {
     "Key": "loading",
     "LocalizedValue": {
       "tr-TR": "Lütfen Bekleyin. Yükleniyor...",
       "en-US": "Please Wait. Loading..."
     }
 },
 {
     "Key": "loading",
     "LocalizedValue": {
       "tr-TR": "Lütfen Bekleyin. Yükleniyor...",
       "en-US": "Please Wait. Loading..."
     }
 }
]
proje seviyesinde route_localization.json bulunmalı. içeriği şu şekilde devam etmeli

[
  {
    "Key": "home",
    "LocalizedValue": {
      "tr-TR": "biorezonans-eskisehir",
      "en-US": "bioresonance-eskisehir"
    }
  },
  {
    "Key": "blog",
    "LocalizedValue": {
      "tr-TR": "blog",
      "en-US": "blog"
    }
  },
]

kullanım şu şekilde olmalıdır. 

koleksiyona eklenmeli;
builder.Services.AddControllersAndMultiLanguage(cultures);

middleware eklenmeli;
app.UseLocalization(cultures);

route mappingler ayarlanmalı
app.MapDynamicControllerRoute<LocalizedTransformer>("{culture=tr}/{controller=Home}/{action=Index}/{name?}");

_viewImporta şu satılar eklenmeli
@using Microsoft.AspNetCore.Mvc.Localization
@using AlpKit.MultiLanguage.Localizer
@using System.Globalization
@addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers

@inject IViewLocalizer Localizer



eger route_localization.json dosyasında eksik veri var ise sayfa bulunamayabilir.