using Microsoft.AspNetCore.Mvc;
using MinimalCupons.Data;
using MinimalCupons.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.MapGet("/helloworld/{id:int}", (int id) =>
{
    //return Results.BadRequest("HEllo");
    return Results.Ok("HEllo" + id);
});

app.MapGet("/api/coupon", (ILogger<Program> _logger) =>
{
    _logger.Log(LogLevel.Information, "Getting All Coupons");
    return Results.Ok(CouponStore.couponsList);
}).WithName("GetCoupon").Produces<IEnumerable< Coupon>>(201);


app.MapGet("/api/coupon/{id:int}", (int id) =>
{
    return Results.Ok(CouponStore.couponsList.FirstOrDefault(u => u.Id == id));
}).WithName("GetCoupon").Produces<Coupon>(201);





app.MapPost("/api/coupon/", (ILogger<Program> _logger,  [FromBody] Coupon obj) =>
{
    _logger.Log(LogLevel.Information, "Creating a Coupon: " +obj.Name);
    if (obj.Id != 0 || string.IsNullOrEmpty(obj.Name))
    {
        return Results.BadRequest("Invalid Id or Coupon Name");
    }

    if (CouponStore.couponsList.FirstOrDefault(x => x.Name.ToLower().Equals(obj.Name.ToLower())) != null)
    {
        return Results.BadRequest("Coupon Name already Exists");
    }

    obj.Id = CouponStore.couponsList.OrderByDescending(x => x.Id).FirstOrDefault().Id + 1;
    CouponStore.couponsList.Add(obj);

    return Results.CreatedAtRoute("GetCoupon", new { id = obj.Id}, obj);
}).WithName("CreateCoupon")
.Accepts<Coupon>("application/json")
.Produces<Coupon>(201).Produces(400);





app.MapPut("/api/coupon/{id:int}", (int id) =>
{
    return Results.Ok(CouponStore.couponsList.FirstOrDefault(u => u.Id == id));
}).WithName("UpdateCoupon"); ;

app.MapDelete("/api/coupon/{id:int}", (int id) =>
{
    return Results.Ok(CouponStore.couponsList.FirstOrDefault(u => u.Id == id));
}).WithName("DeleteCoupon"); ;


app.UseHttpsRedirection();
app.Run();

