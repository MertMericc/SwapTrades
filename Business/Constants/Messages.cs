using Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Constants
{
    public static class Messages
    {
        
        public static string Success = "OK.";
        public static string Error = "Error.";
        public static string UnknownError = "Unknown Error.";
        public static string IncompletedEntry = "Eksik bilgi girildi.";


        public static string UserAlreadyExist = "Böyle bir kullanıcı zaten mevcut.";
        public static string UserListNotFound = "Kullanıcılar listelenemedi.";
        public static string UserAdded = "Kullanıcı başarıyla eklendi.";
        public static string UserDeleted = "Kullanıcı başarıyla silindi.";
        public static string UserNotFound = "Böyle bir kullanıcı bulunamadı.";
        public static string UserUpdated = "Kullanıcı başarıyla güncellendi.";
        public static string UserCouldNotBeAdded = "Kullanıcı eklerken bir hata oluştu!";
        public static string UserIsNotActive = "Kullanıcının statüsü aktif değil!";

        public static string GetClaimError = "Claimler listelenemedi.";
        public static string PasswordsNotMatched = "Parolalar eşleşmiyor.";


        public static string CryptoNotFound = "Böyle bir kripto para birimi bulunamadı.";
        public static string CryptoListNotFound = "Kripto para birimleri listelenemedi.";


        public static string ParityNotFound = "Parite bulunamadı.";
        public static string ParityListNotFound = "Pariteler listelenemedi.";
        public static string SellOrderNotFound = "Böyle bir satış emri bulunamadı.";

        public static string OrderListNotFound = "Emir listesi bulunamadı.";
        public static string OrderNotFound = "Böyle bir emir bulunamadı.";

        public static string WalletAlreadyExists = "Bu cüzdan zaten mevcut.";
        public static string WalletCouldNotBeDeleted = "Bu cüzdan silinemedi.";
        public static string WalletListNotFound = "Cüzdanlar listelenemedi.";
        public static string WalletNotFound = "Cüzdan bulunamadı.";

        public static string NotEnoughBalance = "Yeterli bakineyiz yoktur.";

        public static string StatusNotFound = "Böyle bir statü bulunamadı.";

        public static string WalletCouldNotBeAdded = "Wallet eklenemedi.";

        public static string PageNumberWasOutOfBounds = "Girilen sayfa numarası aralık dışındadır.";
    }
}
