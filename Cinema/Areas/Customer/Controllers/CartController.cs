using Cinema.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;

namespace Cinema.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IRepositry<Cart> _cartRepository;
        private readonly IRepositry<Movie> _movieRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public CartController(IRepositry<Cart> cartRepository, IRepositry<Movie> movieRepository, UserManager<ApplicationUser> userManager)
        {
            _cartRepository = cartRepository;
            _movieRepository = movieRepository;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }
            var carts = await _cartRepository.GetListAsync(c => c.ApplicationUserId == user.Id, includes: [c => c.Movie]);
            return View(carts);
        }
        public async Task<IActionResult> AddToCart(int movieId, int tickets)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }
            var movie = await _movieRepository.GetOneAsync(p => p.Id == movieId);
            if (movie == null)
            {
                return NotFound();
            } 
            if (movie.Quantity < tickets)
            {
                return BadRequest();
            }
            var ExcitMovie = await _cartRepository.GetOneAsync(c =>
                c.ApplicationUserId == user.Id &&
                c.MovieId == movieId
            );
            if (ExcitMovie != null)
            {
                var x = ExcitMovie.Count + tickets;
                if (movie.Quantity < x)
                {
                    return BadRequest();
                }
                ExcitMovie.Count += tickets;
                await _cartRepository.CommitAsync();
                return RedirectToAction(nameof(Index));
            }

            var cart = new Cart()
            {
                ApplicationUserId = user.Id,
                MovieId = movieId,
                Count = tickets
            };
            await _cartRepository.AddAsync(cart);
            await _cartRepository.CommitAsync();

            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> IncrementMovie(int movieId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) 
            {
                return NotFound();
            }
            var movie = await _movieRepository.GetOneAsync(p => p.Id == movieId);
            if (movie == null) return NotFound();

            var cart = await _cartRepository.GetOneAsync(c =>
               c.ApplicationUserId == user.Id &&
               c.MovieId == movieId
            );
            if (cart == null)
            {
                return NotFound();
            }

            if (cart.Count == movie.Quantity)
            {
                TempData["Error_Notification"] = "Movie sold out";
                return RedirectToAction(nameof(Index));
            }
            cart.Count++;
            await _cartRepository.CommitAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> DecrementMovie(int movieId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }
            var movie = await _movieRepository.GetOneAsync(p => p.Id == movieId);
            if (movie == null)
            { 
                return NotFound();
            }

            var cart = await _cartRepository.GetOneAsync(c =>
               c.ApplicationUserId == user.Id &&
               c.MovieId == movieId
            );
            if (cart == null)
            {
                return NotFound();
            }

            if (cart.Count == 1)
            {
                TempData["Error_Notification"] = "The cart is equal one";
                return RedirectToAction(nameof(Index));
            }
            cart.Count--;
            await _cartRepository.CommitAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> DeleteMovie(int movieId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            { 
                return NotFound();
            }
            var movie = await _movieRepository.GetOneAsync(p => p.Id == movieId);
            if (movie == null) return NotFound();

            var cart = await _cartRepository.GetOneAsync(c =>
               c.ApplicationUserId == user.Id &&
               c.MovieId == movieId
            );
            if (cart == null)
            {
                return NotFound();
            }

            _cartRepository.Remove(cart);
            await _cartRepository.CommitAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Pay()
        {
            var options = new SessionCreateOptions
            {
                AllowedPaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                SuccessUrl = $"{Request.Scheme}://{Request.Host}/checkout/success",
                CancelUrl = $"{Request.Scheme}://{Request.Host}/checkout/cancel",
            };
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();
            var carts = await _cartRepository.GetListAsync(c => c.ApplicationUserId == user.Id, includes: [c => c.Movie]);
            foreach (var item in carts)
            {
                options.LineItems.Add(new SessionLineItemOptions()
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "egp",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Movie.Name,
                            Description = item.Movie.Description,
                        },
                        UnitAmount = (long)(item.Movie.Price * 100),
                    },
                    Quantity = item.Count,
                });
            }
            var service = new SessionService();
            var session = service.Create(options);
            return Redirect(session.Url);
        }

    }
}
