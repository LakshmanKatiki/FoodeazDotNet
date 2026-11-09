using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Foodeaz.Web.Pages;

public class IndexModel : PageModel
{
    public record SliderItem(string Image, string Title, string Highlight, string Description);
    public record Retailer(string Name, string Image, string Url);
    public record Testimonial(string Image, string Name, string City, string Message);

    // Replace these sample entries with the exact values from your original PHP arrays.
    public List<SliderItem> SliderItems { get; } = new()
    {
        new("FoodeazBigBanner.jpg", "Authentic South Indian", "Breakfast",
            "Freshly prepared batter for your family.")
    };

    public List<Retailer> Retailers { get; } = new()
        {
            new("BigBasket", "bigbasket.png",
                "https://www.bigbasket.com/pd/40247666/foodeaz-dosa-batter-instant-mix-healthy-carbs-perfect-breakfast-800-g-pouch/"),

            new("Vijetha", "vijetha.png", "#"),

            new("Ratnadeep", "ratnadeep.jpeg",
                "https://www.ratnadeep.com"),

            new("Instamart", "instamart.png",
                "https://www.swiggy.com/instamart"),

            new("Blinkit", "blinkit.png",
                "https://blinkit.com"),

            new("QMart", "qmart.png",
                "https://www.qmart.in"),

            new("CountryOven", "countryoven.png",
                "https://www.countryoven.com"),

            new("LULU", "lulu.png",
                "https://www.luluindia.in")
        };

    public List<Testimonial> Testimonials { get; } = new()
        {
            new(
                "usericon.png",
                "Shanmu Keshav",
                "Hyderabad",
                "The batter was fantastic! Soft idlis and crispy dosas just like homemade. The experience was delightful and I'll definitely recommend Foodeaz to friends and family!"),

            new(
                "usericon.png",
                "Subham",
                "Hyderabad",
                "Foodeaz is simply amazing! The batter is fresh, authentic, and so easy to cook with. Thank you for bringing the real taste of South Indian breakfast to our homes!"),

            new(
                "usericon.png",
                "Tarun Kumar",
                "Hyderabad",
                "The idlis turned out soft and fluffy, and the dosas were perfectly crisp! Foodeaz batter makes breakfast so simple and delicious. Great job!"),

            new(
                "usericon.png",
                "Srinivasarao",
                "Hyderabad",
                "Absolutely loved it! The batter was fresh and the idlis came out soft and fluffy while the dosas turned out perfectly crisp. I'll definitely order again!")
        };

    public void OnGet()
    {
    }
}
