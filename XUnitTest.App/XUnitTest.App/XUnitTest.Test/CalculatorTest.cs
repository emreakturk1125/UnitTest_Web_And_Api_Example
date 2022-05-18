using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using XUnitTest.App;

namespace XUnitTest.Test
{
    // Önemli : Depedency Injection tasarım deseni UNİT TEST yapılabilmesi için gereklidir. En kolay şekilde yapılabilmesini sağlar

    //                    ****************  Yüklediğim kütüphaneler ****************
    // -----------------------------------------------------------------------------------------------------------------------------------------------------------
    // 1.) Moq kütüphanesi (Dummy datalar için)

    //                    ****************  Test 3 aşamadan oluşur. ****************
    // -----------------------------------------------------------------------------------------------------------------------------------------------------------
    // Arrange,
    // Act,
    // Assert  (Assert Metodları => Contain, DoesNotContain)

    // [Fact] Attribute
    // ---------------------------------
    //  1.) test edilebilir olduğunu
    //  2.) parametre almayan bir metod olduğunu belli etmiş oluruz

    // [Theory] Attribute
    // ---------------------------------
    //  1.)  Paramtere alabilir olduğunu belli etmiş oluruz.
    //  2.)  [InlineData](param1,param2)  ile bilikte kullanılır parametreyi vermek için

    //           ************  Method mock yaparken karşınıza çıkarak 3 tane davranışı bilmeniz gerekli. ************ 
    // -----------------------------------------------------------------------------------------------------------------------------------------------------------
    //   1.) MockBehavior.Strict   (Strict, mock’lu method çağrıldığında setup edilmemişse exception fırlatır.)
    //   2.) MockBehavior.Loose    (Loose, hiç bir zaman exception fırlatmaz. Çağrı sonrası exception yerine return tipi ne ise default halini döner, yeni object oluşturmaz. Dönüş türünüz int ise default value “0” olduğu için 0 döner. Eğer bir class ise null döner.)
    //   3.) MockBehavior.Default  (default hali loose’dur)

    public class CalculatorTest
    {
        // Tüm metodlarda ortak kullanılacak metod varsa Constructor da tanımlanmalıdır.
        public Calculator _calculator { get; set; }
        public Mock<ICalculatorService> _myMock;
        public CalculatorTest()
        {
            _myMock = new Mock<ICalculatorService>();
            _calculator = new Calculator(_myMock.Object);
        }

        [Fact]
        public void AddTest1()
        {
            // Arrange  => 
            int a = 5;
            int b = 15; 

            // Act      => 
            var total = _calculator.Add(a, b);

            // Assert   =>  
            Assert.Equal<int>(20, total);

            Assert.Contains("Emre", "Emre Aktürk");

            Assert.DoesNotContain("Emree", "Emre Aktürk");

            var names = new List<string>() { "Ali", "Mahmut", "Veli" };
            Assert.Contains(names, x => x == "Ali");

            Assert.True(5 > 6);

            Assert.False(5 > 6);

            Assert.True("".GetType() == typeof(string));

            var regex = "^dog";
            Assert.Matches(regex, "dogg eeee");

            Assert.StartsWith("Bir", "Bir hafta");
            Assert.EndsWith("hafta", "Bir hafta");

            Assert.Empty(new List<string>());
            Assert.NotEmpty(new List<string>());

            Assert.InRange(10, 2, 20);                                             // belirtilen değerler arasında olmalıdır.
            Assert.NotInRange(10, 2, 20);

            Assert.Single(new List<string>() { "Emre" });                          // tek kayıt olmalıdır
            Assert.Single(new List<string>() { "Emre", "Mahmut" });
            Assert.Single<int>(new List<int>() { 1, 2, 3 });

            Assert.IsType<string>("fatih");                                          // içerdeki değer string olmalıdır
            Assert.IsNotType<string>("fatih");                                       // içerdeki değer string olmamalıdır

            Assert.IsAssignableFrom<IEnumerable<string>>(new List<string>());        // beklediğin tipin istediğin gibi gelmesi olayıdır.

            string deger = null;
            Assert.Null(deger);
        }

        [Theory]
        [InlineData(2, 5, 7)]                
        [InlineData(10, 2, 12)]
        public void Add_SimpleValues_ReturnTotalValue(int a, int b, int expectedTotal)
        {
            
            _myMock.Setup(x => x.Add(a, b)).Returns(expectedTotal);   // servis metodu çalışıyormuş gibi 


            // Act      => 
            var actualTotal = _calculator.Add(a, b);
            Assert.Equal(expectedTotal, actualTotal);
            _myMock.Verify(x => x.Add(a, b), Times.Once);
             
        }

        [Theory]
        [InlineData(0, 5, 0)]                
        [InlineData(10, 0, 12)]
        public void Add_ZeroValues_ReturnZeroValue(int a, int b, int expectedTotal)
        {
            // Act      => 
            var actualTotal = _calculator.Add(a, b);
            Assert.Equal(expectedTotal, actualTotal);
        }


        [Theory]
        [InlineData(3, 5, 15)] 
        public void Multiply_SimpleValues_ReturnMultiplyValue(int a, int b, int expectedValue)
        {

            _myMock.Setup(x => x.Multiply(a, b)).Returns(expectedValue);  // servis metodu çalışıyormuş gibi 


            // Act      => 
            var actualTotal = _calculator.Add(a, b);
            Assert.Equal(15, _calculator.Multiply(a,b));

        }
    }
}
