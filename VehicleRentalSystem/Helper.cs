namespace ConsoleHelpers;

internal static class Helper
{
    public static string BilgiAl(string soru)
    {
        Console.Write($"{soru}: ");
        return Console.ReadLine();
    }

    public static string SifreliAl(string soru)
    {
        Console.Write($"{soru}: ");
        string sifre = string.Empty;

        while (true)
        {
            var input = Console.ReadKey(true);

            if (input.Key == ConsoleKey.Enter)
            {
                Console.WriteLine("");
                break;
            }

            if (input.Key == ConsoleKey.Backspace)
            {
                if (sifre.Length > 0)
                {
                    sifre = sifre.Remove(sifre.Length - 1);
                    Console.Write("\b \b");
                }
                continue;
            }

            Console.Write("*");
            sifre += input.KeyChar.ToString();
        }

        return sifre;
    }

    // fonksiyon class içindeyse metot olarak isimlendirilir.
    // metot içinde fonksiyon tanımı da yapabiliyoruz. çok sık kullanılan bir yöntem değil.

    // method overloading
    // aynı isimde fakat farklı parametreler ile method tanımlamaya method overloading denir.
    // bu sadece isim arama derdimiz olmaz ve bir çok fonksiyonelliği
    // //aynı isim altında, parametreler ile sağlamış oluruz

    public static int SayiAl(string soru)
    {
        string inputSayi = BilgiAl(soru);
        if (int.TryParse(inputSayi, out int sayi))
        {
            return sayi;
        }
        Hata("Sadece sayı girişi yapın!");
        return SayiAl(soru);
    }

    public static int SecimYaptir(string[] secenekler, string msj = "Seçim yapın")
    {
        Console.WriteLine(msj);
        for (int i = 0; i < secenekler.Length; i++)
        {
            Console.WriteLine($"{i + 1}. {secenekler[i]}");
        }

        Console.Write("Seçiminiz: ");
        string inputSecim = Console.ReadKey(true).KeyChar.ToString();
        if (int.TryParse(inputSecim, out int secim))
        {
            if (secim > 0 && secim <= secenekler.Length)
            {
                return secim;
            }
        }

        Hata("Geçerli bir seçim yapmalısın!\nSeçenekleri kontrol ederek tekrar dene.\n");
        return SecimYaptir(secenekler, msj);
    }

    public static void RenkliMesajGoster(string msj, ConsoleColor renk)
    {
        Console.ForegroundColor = renk;
        Console.WriteLine(msj);
        Console.ResetColor();
    }

    public static void Hata(string msj)
    {
        RenkliMesajGoster(msj, ConsoleColor.Red);
    }

    public static void Basarili(string msj)
    {
        RenkliMesajGoster(msj, ConsoleColor.Green);
    }
}




