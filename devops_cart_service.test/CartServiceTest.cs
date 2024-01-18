using System.Net;
using AutoMapper;
using devops_cart_service.Models;
using devops_cart_service.Models.Dto;
using devops_cart_service.Repository;
using devops_cart_service.Repository.IRepository;
using devops_cart_service.Services;
using Moq;

namespace devops_cart_service.test;

[TestClass]
public class CartServiceTest
{
    [TestClass]
    public class CartServiceTests
    {
        [TestMethod]
        public async Task GetCartAsync_ReturnsSuccessResponse()
        {
            // Arrange
            var userId = 1;
            var expectedCartOverview = new CartOverview { UserId = userId, CartId = 101 };
            var expectedCartProducts = new List<CartProduct>
            {
                new CartProduct { ProductId = 201, Quantity = 2 },
                new CartProduct { ProductId = 202, Quantity = 1 }
            };
            var expectedCart = new Cart { CartOverview = expectedCartOverview, CartProducts = expectedCartProducts };

            var cartOverviewRepoMock = new Mock<ICartOverviewRepository>();
            var cartProductRepoMock = new Mock<ICartProductRepository>();
            var mapperMock = new Mock<IMapper>();

            cartOverviewRepoMock.Setup(repo => repo.GetCartOverviewByUserIdAsync(userId))
                                .ReturnsAsync(expectedCartOverview);

            cartProductRepoMock.Setup(repo => repo.GetCartProductsByCartIdAsync(expectedCartOverview.CartId))
                                .ReturnsAsync(expectedCartProducts);

            mapperMock.Setup(mapper => mapper.Map<CartDto>(It.IsAny<Cart>()))
                      .Returns(new CartDto
                      {
                          CartOverview = new CartOverviewDto { UserId = userId, CartId = expectedCartOverview.CartId },
                          CartProducts = expectedCartProducts.Select(cp => new CartProductDto { ProductId = cp.ProductId, Quantity = cp.Quantity })
                      });

            var cartService = new CartService(cartOverviewRepoMock.Object, cartProductRepoMock.Object, mapperMock.Object);

            // Act
            var result = await cartService.GetCartAsync(userId);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
            Assert.IsNotNull(result.Result);
            Assert.IsInstanceOfType(result.Result, typeof(CartDto));

            var cartDto = (CartDto)result.Result;
            Assert.IsNotNull(cartDto.CartOverview);
            Assert.IsNotNull(cartDto.CartProducts);
            Assert.AreEqual(userId, cartDto.CartOverview.UserId);
            Assert.AreEqual(expectedCartOverview.CartId, cartDto.CartOverview.CartId);
            Assert.AreEqual(expectedCartProducts.Count(), cartDto.CartProducts.Count());

        }

        [TestMethod]
        public async Task GetCartAsync_WhenCartOverviewIsNull_ReturnsErrorResponse()
        {
            // Arrange
            var userId = 1;

            var cartOverviewRepoMock = new Mock<ICartOverviewRepository>();
            var cartProductRepoMock = new Mock<ICartProductRepository>();
            var mapperMock = new Mock<IMapper>();

            cartOverviewRepoMock.Setup(repo => repo.GetCartOverviewByUserIdAsync(userId))
                                .ReturnsAsync((CartOverview)null);

            var cartService = new CartService(cartOverviewRepoMock.Object, cartProductRepoMock.Object, mapperMock.Object);

            // Act
            var result = await cartService.GetCartAsync(userId);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
            Assert.IsNull(result.Result);
            Assert.IsNotNull(result.ErrorMessages);
            Assert.AreEqual(1, result.ErrorMessages.Count);
        }

        [TestMethod]
        public async Task DeleteCartAsync_ReturnsSuccessResponse()
        {
            // Arrange
            var cartId = 101;

            var cartOverviewRepoMock = new Mock<ICartOverviewRepository>();
            var mapperMock = new Mock<IMapper>();

            var expectedCartOverview = new CartOverview { CartId = cartId, IsDeleted = false };

            cartOverviewRepoMock.Setup(repo => repo.GetCartOverviewByIdAsync(cartId))
                                .ReturnsAsync(expectedCartOverview);

            var cartService = new CartService(cartOverviewRepoMock.Object, null, mapperMock.Object);

            // Act
            var result = await cartService.DeleteCartAsync(cartId);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(HttpStatusCode.NoContent, result.StatusCode);
            Assert.IsNull(result.Result);
            Assert.IsNotNull(result.ErrorMessages);

            cartOverviewRepoMock.Verify(repo => repo.UpdateCartOverviewAsync(It.Is<CartOverview>(c => c.CartId == cartId && c.IsDeleted == true)), Times.Once);
        }


        [TestMethod]
        public async Task DeleteCartAsync_WhenCartOverviewIsNull_ReturnsErrorResponse()
        {
            // Arrange
            var cartId = 101;

            var cartOverviewRepoMock = new Mock<ICartOverviewRepository>();
            var mapperMock = new Mock<IMapper>();

            cartOverviewRepoMock.Setup(repo => repo.GetCartOverviewByIdAsync(cartId))
                                .ReturnsAsync((CartOverview)null);

            var cartService = new CartService(cartOverviewRepoMock.Object, null, mapperMock.Object);

            // Act
            var result = await cartService.DeleteCartAsync(cartId);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
            Assert.IsNull(result.Result);
            Assert.IsNotNull(result.ErrorMessages);
            Assert.AreEqual(1, result.ErrorMessages.Count);

            cartOverviewRepoMock.Verify(repo => repo.UpdateCartOverviewAsync(It.IsAny<CartOverview>()), Times.Never);
        }

        [TestMethod]
        public async Task UpdateCartAsync_ShouldReturnSuccessResponse_WhenUpdateIsSuccessful()
        {
            // Arrange
            var cartUpdateDto = new CartUpdateDto
            {
                CartOverview = new CartOverviewUpdateDto
                {
                    CartId = 7,
                    UserId = 1,
                    Discount = 10.0M,
                    TotalPrice = 90.0M,
                    IsCheckedOut = false
                },
                CartProducts = new List<CartProductUpdateDto>
                {
                    new CartProductUpdateDto
                    {
                        ProductId = 1,
                        Quantity = 3,
                        IsDeleted = true
                    },
                    new CartProductUpdateDto
                    {
                        ProductId = 2,
                        Quantity = 2,
                        IsDeleted = false
                    },
                    new CartProductUpdateDto
                    {
                        ProductId = 3,
                        Quantity = 5,
                        IsDeleted = false
                    }
                }
            };

            var cartOverview = new CartOverview
            {
                CartId = 7,
                UserId = 1,
                Discount = 10.0M,
                TotalPrice = 90.0M,
                IsCheckedOut = false
            };
            var cartProducts = new List<CartProduct>
            {
                new CartProduct { ProductId = 1, Quantity = 3, IsDeleted = true },
                new CartProduct { ProductId = 2, Quantity = 2, IsDeleted = false },
                new CartProduct { ProductId = 3, Quantity = 5, IsDeleted = false }
            };
            var cart = new Cart { CartOverview = cartOverview, CartProducts = cartProducts };
            var cartDto = new CartDto
            {
                CartOverview = new CartOverviewDto
                {
                    CartId = 7,
                    UserId = 1,
                    Discount = 10.0M,
                    TotalPrice = 90.0M
                },
                CartProducts = new List<CartProductDto>
                {
                    new CartProductDto { ProductId = 1, Quantity = 3},
                    new CartProductDto { ProductId = 2, Quantity = 2},
                    new CartProductDto { ProductId = 3, Quantity = 5}
                }
            };

            var mapperMock = new Mock<IMapper>();
            var cartOverviewRepoMock = new Mock<ICartOverviewRepository>();
            var cartProductRepoMock = new Mock<ICartProductRepository>();

            mapperMock.Setup(m => m.Map<CartOverview>(cartUpdateDto.CartOverview)).Returns(cartOverview);
            mapperMock.Setup(m => m.Map<IEnumerable<CartProduct>>(cartUpdateDto.CartProducts)).Returns(cartProducts);
            mapperMock.Setup(m => m.Map<CartDto>(It.IsAny<Cart>())).Returns(cartDto);

            cartOverviewRepoMock.Setup(repo => repo.UpdateCartOverviewAsync(cartOverview)).Verifiable();
            cartProductRepoMock.Setup(repo => repo.UpdateCartProductAsync(It.IsAny<CartProduct>())).Verifiable();
            cartOverviewRepoMock.Setup(repo => repo.CheckoutCartOverviewAsync(cartOverview)).Verifiable();

            var cartService = new CartService(cartOverviewRepoMock.Object, cartProductRepoMock.Object, mapperMock.Object);

            // Act
            var result = await cartService.UpdateCartAsync(cartUpdateDto);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
            Assert.IsNotNull(result.Result);
            cartProductRepoMock.Verify();
            cartOverviewRepoMock.Verify(repo => repo.CheckoutCartOverviewAsync(cartOverview), Times.Exactly(cartUpdateDto.CartOverview.IsCheckedOut ? 1 : 0));
        }

        [TestMethod]
        public async Task CreateCartAsync_ReturnsSuccessResponse()
        {
            // Arrange
            var cartCreateDto = new CartCreateDto
            {
                CartOverview = new CartOverviewCreateDto
                {
                    UserId = 1,
                    Discount = 10.0M,
                    TotalPrice = 100.0M
                },
                CartProducts = new List<CartProductCreateDto>
                {
                    new CartProductCreateDto
                    {
                        ProductId = 1,
                        Quantity = 3
                    },
                    new CartProductCreateDto
                    {
                        ProductId = 3,
                        Quantity = 4
                    }
                }
            };

            var cartOverviewRepoMock = new Mock<ICartOverviewRepository>();
            var cartProductRepoMock = new Mock<ICartProductRepository>();
            var mapperMock = new Mock<IMapper>();

            var cartOverview = new CartOverview
            {
                UserId = cartCreateDto.CartOverview.UserId,
                Discount = cartCreateDto.CartOverview.Discount,
                TotalPrice = cartCreateDto.CartOverview.TotalPrice
            };
            var cartProducts = new List<CartProduct>
            {
                new CartProduct
                {
                    ProductId = 1,
                    Quantity = 3
                },
                new CartProduct
                {
                    ProductId = 3,
                    Quantity = 4
                }
            };

            var expectedCartDto = new CartDto
            {
                CartOverview = new CartOverviewDto
                {
                    UserId = cartCreateDto.CartOverview.UserId,
                    Discount = cartCreateDto.CartOverview.Discount,
                    TotalPrice = cartCreateDto.CartOverview.TotalPrice
                },
                CartProducts = new List<CartProductDto>
                {
                    new CartProductDto
                    {
                        ProductId = 1,
                        Quantity = 3
                    },
                    new CartProductDto
                    {
                        ProductId = 3,
                        Quantity = 4
                    }
                }
            };

            cartOverviewRepoMock.Setup(repo => repo.GetCartOverviewByUserIdAsync(cartCreateDto.CartOverview.UserId))
                               .ReturnsAsync((CartOverview)null); // Assume no existing cart overview

            cartOverviewRepoMock.Setup(repo => repo.CreateCartOverviewAsync(It.IsAny<CartOverview>()));

            cartProductRepoMock.Setup(repo => repo.CreateCartProductAsync(It.IsAny<CartProduct>()));

            mapperMock.Setup(mapper => mapper.Map<CartOverview>(cartCreateDto.CartOverview))
                      .Returns(cartOverview);

            mapperMock.Setup(mapper => mapper.Map<IEnumerable<CartProduct>>(cartCreateDto.CartProducts))
                      .Returns(cartProducts);

            mapperMock.Setup(m => m.Map<CartDto>(It.IsAny<Cart>())).Returns(expectedCartDto);
            var cartService = new CartService(cartOverviewRepoMock.Object, cartProductRepoMock.Object, mapperMock.Object);

            // Act
            var result = await cartService.CreateCartAsync(cartCreateDto);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(HttpStatusCode.Created, result.StatusCode);
            Assert.IsNotNull(result.Result);
            Assert.IsInstanceOfType(result.Result, typeof(CartDto));

            var cartDto = (CartDto)result.Result;
            Assert.IsNotNull(cartDto.CartOverview);
            Assert.IsNotNull(cartDto.CartProducts);
            Assert.AreEqual(cartCreateDto.CartOverview.UserId, cartDto.CartOverview.UserId);
            Assert.AreEqual(cartCreateDto.CartOverview.Discount, cartDto.CartOverview.Discount);
            Assert.AreEqual(cartCreateDto.CartOverview.TotalPrice, cartDto.CartOverview.TotalPrice);

            cartOverviewRepoMock.Verify(repo => repo.GetCartOverviewByUserIdAsync(cartCreateDto.CartOverview.UserId), Times.Once);
            cartOverviewRepoMock.Verify(repo => repo.CreateCartOverviewAsync(It.IsAny<CartOverview>()), Times.Once);
            cartProductRepoMock.Verify(repo => repo.CreateCartProductAsync(It.IsAny<CartProduct>()), Times.Exactly(cartProducts.Count));

            cartOverviewRepoMock.Verify(repo => repo.UpdateCartOverviewAsync(It.IsAny<CartOverview>()), Times.Never);
        }

    }

}
