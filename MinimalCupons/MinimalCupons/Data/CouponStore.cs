using MinimalCupons.Models;

namespace MinimalCupons.Data
{
    public static class CouponStore
    {
        public static List<Coupon> couponsList = new List<Coupon> {
        new Coupon {Id = 1, Name = "test1" , Percent = 10 , isActive = true},
        new Coupon {Id = 2, Name = "test2" , Percent = 20 , isActive = false},
        };
    }
}
