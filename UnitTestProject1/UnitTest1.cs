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
            BitmapImage a = Utils.GetImageWindowsDialog();
            if (a == null) return;
            byte[] b = Utils.ImageToBinary(a);
            BitmapImage c = Utils.BinaryToImage(b);
            Assert.IsTrue(b is byte[]);
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
            };

            UserCache.Basket.Add(candle, 1);
            MessageBox.Show($"Товар \"{candle.Name}\" добавлен!\nОписание товара: \"{candle.Description}\".");
            Assert.IsTrue(Utils.IsInBasket(candle));

            UserCache.Basket.Remove(candle);
            MessageBox.Show($"Товар \"{candle.Name}\" удален!");
            Assert.IsFalse(Utils.IsInBasket(candle));
        }
    }
}
