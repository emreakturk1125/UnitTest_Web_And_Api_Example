using Microsoft.AspNetCore.Mvc;
using Moq;
using RealWorldUnitTest.Web.Controllers;
using RealWorldUnitTest.Web.Models;
using RealWorldUnitTest.Web.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace RealWorldUnitTest.Test
{
    // Web Uygulamasında bulunan controller daki action metodlar test ediliyor
    public class ProductControllerTest
    {
        private readonly Mock<IRepository<Product>> _mockRepo;      
        private readonly ProductsController _controller;
        private List<Product> products;
        public ProductControllerTest()
        { 
            _mockRepo = new Mock<IRepository<Product>>();                  // Controller içinde kullanılan IRepository den gelen temel crud metodlarına ulaşmak için         
            _controller = new ProductsController(_mockRepo.Object);        // Controller daki metodlara ulaşabiliyoruz.
            products = new List<Product>()
            {
               new Product
               {
                   Id = 1, Name = "Kalem", Price = 100, Stock = 20, Color = "Mavi"
               },
               new Product
               {
                   Id = 2, Name = "Defter", Price = 200, Stock = 20, Color = "Kırmızı"
               }
            };
        }


        [Fact]
        public async void Index_ActionExecutes_ReturnView()  // ViewResult dönüyor mu? Onu test ettik
        {
            var result = await _controller.Index();
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async void Index_ActionExecutes_ReturnProductList()    // ProductController daki Index  metodu GetAll() çağırdığı için biz de  burada GetAll() u çağırdık
        {
            _mockRepo.Setup(repo => repo.GetAll()).ReturnsAsync(products);
            var result = await _controller.Index();
            var viewResult = Assert.IsType<ViewResult>(result);                                  // Tipi ViewResult olan sonucu aldık
            var productList = Assert.IsAssignableFrom<IEnumerable<Product>>(viewResult.Model);   // Sonuc modelimizi Product a Atanabilir olması lazım
            Assert.Equal<int>(2, productList.Count());                                           // Gelen liste deki dataların sayısı 2 olmalıdır.
        }

        [Fact]
        public async void Details_IsIsNull_ReturnRedirectToAction()
        {
            var result = await _controller.Details(null);
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
        }


        [Fact]
        public async void Details_IdInValid_ReturnNotFound()
        {
            Product product = null;
            _mockRepo.Setup(x => x.GetById(0)).ReturnsAsync(product);
            var result = await _controller.Details(0);
            var redirect = Assert.IsType<NotFoundResult>(result);
            Assert.Equal<int>(404, redirect.StatusCode);
        }

        [Theory]
        [InlineData(1)]
        public async void Details_IdInValid_ReturnProduct(int productId)
        {
            Product product = products.FirstOrDefault(x => x.Id == productId); 
            _mockRepo.Setup(repo => repo.GetById(productId)).ReturnsAsync(product);
            var result = await _controller.Details(productId);

            var viewResult = Assert.IsType<ViewResult>(result);
            var resultProduct = Assert.IsAssignableFrom<Product>(viewResult.Model);

            Assert.Equal(product.Id, resultProduct.Id);
            Assert.Equal(product.Name, resultProduct.Name);

        }


        [Fact]
        public void Create_Get_ActionExecutes_ReturnView()
        {

            var result = _controller.Create();
            Assert.IsType<ViewResult>(result);

        }

        [Fact]
        public async void Create_Post_InValidModelState_ReturnView()
        {
            _controller.ModelState.AddModelError("Name", "Name alanı gereklidir.");
            var result = await _controller.Create(products.First());
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsType<Product>(viewResult.Model);
        }

        [Fact]
        public async void Create_Post_ValidModelState_ReturnRedirectToIndexAction()
        {

            var result = await _controller.Create(products.First());
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);

        }

        [Fact]
        public async void Create_Post_InValidModelState_CreateMethodExecute()
        {
            Product newProduct = null;
            _mockRepo.Setup(repo => repo.Create(It.IsAny<Product>())).Callback<Product>(x => newProduct = x);
            var result = await _controller.Create(products.First());
            _mockRepo.Verify(repo => repo.Create(It.IsAny<Product>()), Times.Once);
            Assert.Equal(products.First().Id, newProduct.Id);
        }

        [Fact]
        public async void Create_Post_InValidModelState_NeverCreateExecute()
        {
            _controller.ModelState.AddModelError("Name", "");
            var result = await _controller.Create(products.First());
            _mockRepo.Verify(repo => repo.Create(It.IsAny<Product>()), Times.Never);      // Create metodu asla çalışmamalı

        }


        [Fact]
        public async void Edit_IdIsNull_ReturnRedirectToIndexAction()
        {

            var result = await _controller.Edit(null);
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);

        }

        [Theory]
        [InlineData(3)]
        public async void Edit_IdInValid_ReturnNotFound(int productId)
        {
            Product product = null;
            _mockRepo.Setup(x => x.GetById(productId)).ReturnsAsync(product);
            var result = await _controller.Edit(productId);
            var redirect = Assert.IsType<NotFoundResult>(result);
            Assert.Equal<int>(404, redirect.StatusCode);

        }


        [Theory]
        [InlineData(2)]
        public async void Edit_ActionExecutes_ReturnProduct(int productId)
        {
            var product = products.First(x => x.Id == productId);
            _mockRepo.Setup(x => x.GetById(productId)).ReturnsAsync(product);
            var result = await _controller.Edit(productId);
            var viewResult = Assert.IsType<ViewResult>(result);
            var resultProduct = Assert.IsAssignableFrom<Product>(viewResult.Model);
            Assert.Equal(product.Name, resultProduct.Name); 
        }

        [Theory]
        [InlineData(1)]
        public void EditPOST_IdIsNotEqualProduct_ReturnNotFound(int productId)
        {
            var result = _controller.Edit(2, products.First(x => x.Id == productId));

            var redirect = Assert.IsType<NotFoundResult>(result);
        }

        [Theory]
        [InlineData(1)]
        public void EditPOST_InValidModelState_ReturnView(int productId)
        {
            _controller.ModelState.AddModelError("Name", "");  // Conttoller Name üzerinden hata ekledik

            var result = _controller.Edit(productId, products.First(x => x.Id == productId));

            var viewResult = Assert.IsType<ViewResult>(result);

            Assert.IsType<Product>(viewResult.Model);
        }

        [Theory]
        [InlineData(1)]
        public void EditPOST_ValidModelState_ReturnRedirectToIndexAction(int productId)
        {
            var result = _controller.Edit(productId, products.First(x => x.Id == productId));

            var redirect = Assert.IsType<RedirectToActionResult>(result);

            Assert.Equal("Index", redirect.ActionName);
        }

        /// <summary>
        /// 
        /// Update metodunun çalışması test ediliyor, 
        ///  It.IsAny<Product>()  => herhangbir product olabilir demektir.
        /// 
        /// </summary>
        /// <param name="productId"></param>
        [Theory]
        [InlineData(1)]
        public void EditPOST_ValidModelState_UpdateMethodExecute(int productId)
        {
            var product = products.First(x => x.Id == productId);

            _mockRepo.Setup(repo => repo.Update(product));                       

            _controller.Edit(productId, product);

            _mockRepo.Verify(repo => repo.Update(It.IsAny<Product>()), Times.Once);   
        }

        /// <summary>
        /// 
        /// Controller daki Delete metodundan, gelen id null ise NotFoundResult tipinde olan NotFound() geri döndürülüyor
        /// Null gönderdiğimiz zaman NotFoundResult tipinde bir sonuç mu dönüyor onu test ettik
        /// 
        /// </summary>
        [Fact]
        public async void Delete_IdIsNull_ReturnNotFound()
        {
            var result = await _controller.Delete(null);      
            Assert.IsType<NotFoundResult>(result);            
        }


        /// <summary>
        /// 
        /// Gönderdiğimiz id'ye ait ürün yoksa, null ise Controller'daki Delete metdoundu içindeki GetById() den NotFoundResult tipinde olan NotFound() geri döndürülüyor
        /// Ürün yoksa NotFoundResult tipinde bir sonuç mu dönüyor onu test ettik
        /// GetById() için Mock setup yaptık, yani o metod çalışıyormuş ve sonuç dönüyormuş gibi yaptık. _controller daki GetById() metoduna productId gönderdiğimizde,  bizim belirlediğimiz  " Product product = null"  null olan product dönsün dedik
        /// product null ise de NotFoundResult tipinde olan NotFound() geri dönüyor mu onu test ettik
        /// 
        /// </summary> 
        [Theory]
        [InlineData(0)]
        public async void Delete_IdIsNotEqualProduct_ReturnNotFound(int productId)
        {
            Product product = null;

            _mockRepo.Setup(x => x.GetById(productId)).ReturnsAsync(product);      

            var result = await _controller.Delete(productId);

            Assert.IsType<NotFoundResult>(result);
        }

        /// <summary>
        /// 
        /// Bu sefer gönderdiğimiz id'ye göre ürün dönüyorsa case'ini test ediyoruz
        /// Mock işlemiyle  GetById() çalıştığı vakit, bizim belirttiğimiz sonuç dönsün dedik
        /// 
        /// 
        /// </summary>
        /// <param name="productId"></param>
        [Theory]
        [InlineData(1)]
        public async void Delete_ActionExecutes_ReturnProduct(int productId)
        {
            var product = products.First(x => x.Id == productId);                       // Elimizde dummy data olan bir product var

            _mockRepo.Setup(repo => repo.GetById(productId)).ReturnsAsync(product);     // GetById() çalışınca üstteki dummy product dönsün dedik

            var result = await _controller.Delete(productId);                           //  Controller'daki Delete() metoduna parametereyi gönderdik

            var viewResult = Assert.IsType<ViewResult>(result);                         // Delete metdounda gelen ViewResult tipindeki sonucu aldık

            Assert.IsAssignableFrom<Product>(viewResult.Model);                         // Modeli test ettik, dönen sonuç ,Product nesnesine atanabiliyor mu 
        }


        /// <summary>
        /// Başarılı bir şekilde çalışınca RedirectToActionResult tipindeki Index sayfasına gidiyor, onu test ettik 
        /// </summary>
        /// <param name="productId"></param>
        [Theory]
        [InlineData(1)]
        public async void DeleteConfirmed_ActionExecutes_ReturnRedirectToIndexAction(int productId)
        {
            var result = await _controller.DeleteConfirmed(productId);

            Assert.IsType<RedirectToActionResult>(result);
        }

        /// <summary>
        /// 
        /// DeleteConfirmed() Action içinde  Delete() metodu çalışıp, çalışmadığını kontrol ediyoruz.
        /// 
        /// </summary>
        /// <param name="productId"></param>

        [Theory]
        [InlineData(1)]
        public async void DeleteConfirmed_ActionExecutes_DeleteMethodExecute(int productId)
        {
            var product = products.First(x => x.Id == productId);

            _mockRepo.Setup(repo => repo.Delete(product));

            await _controller.DeleteConfirmed(productId);

            _mockRepo.Verify(repo => repo.Delete(It.IsAny<Product>()), Times.Once);      
        }
    }
}
