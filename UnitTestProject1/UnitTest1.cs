using CandlesCompany;
using CandlesCompany.Cache;
using CandlesCompany.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void ConvertImage()
        {
            BitmapImage a = Utils.GetImageWindowsDialog(); // Вызов диалога
            if (a == null) return; // Если файл не выбран
            byte[] b = Utils.ImageToBinary(a); // Конвертация файла в массив байтов
            Assert.IsTrue(b is byte[]);
            BitmapImage c = Utils.BinaryToImage(b); // Конвертация массив байтов в объект
            Assert.IsTrue(c is BitmapImage);
        }

        [TestMethod]
        public void BasketTest()
        {
            Candles candle = new Candles
            {
                Count = 1,
                Description = "Свечка в баночке с ароматом лаванды",
                Id = 1,
                Id_Type_Candle = 1,
                Image = null,
                Name = "Арома свеча",
                Price = (decimal)100.0,
            }; // Создание объекта свечи

            UserCache.Basket.Add(candle, 1); // Добавляем свечу в кэш-корзину
            Assert.IsTrue(Utils.IsInBasket(candle)); // Проверяем, находится ли свеча в корзине?

            UserCache.Basket.Remove(candle); // Удалять свечу из корзины
            Assert.IsFalse(Utils.IsInBasket(candle)); // Проверяем, находится ли свеча в корзине?
        }
    }
}
