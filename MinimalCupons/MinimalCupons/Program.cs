using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using MinimalCupons;
using MinimalCupons.Data;
using MinimalCupons.DTO;
using MinimalCupons.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(MappingConfig));
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
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

app.MapGet("/api/coupon", (ILogger <Program> _logger) =>
{
    _logger.Log(LogLevel.Information, "Getting All Coupons");
    //var result = _mapper.Map<List<CouponCreateDTO>>(CouponStore.couponsList);
    return Results.Ok(CouponStore.couponsList);

}).WithName("GetAllCoupon").Produces<IEnumerable<CouponCreateDTO>>(201);


app.MapGet("/api/coupon/{id:int}", (int id) =>
{
    return Results.Ok(CouponStore.couponsList.FirstOrDefault(u => u.Id == id));
}).WithName("GetCoupon").Produces<Coupon>(201);





app.MapPost("/api/coupon/", async (IValidator<CouponCreateDTO> _validator, IMapper _mapper, ILogger<Program> _logger, [FromBody] CouponCreateDTO obj) =>
{
    var validationResult = await _validator.ValidateAsync(obj);
    

    _logger.Log(LogLevel.Information, "Creating a Coupon: " + obj.Name);

    if(!validationResult.IsValid)
    {
        return Results.BadRequest(validationResult.Errors.FirstOrDefault().ToString());
    }

    if (CouponStore.couponsList.FirstOrDefault(x => x.Name.ToLower().Equals(obj.Name.ToLower())) != null)
    {
        return Results.BadRequest("Coupon Name already Exists");
    }

    Coupon coupon = _mapper.Map<Coupon>(obj);
    coupon.Id = CouponStore.couponsList.OrderByDescending(x => x.Id).FirstOrDefault().Id + 1;
    CouponStore.couponsList.Add(coupon);


    return Results.CreatedAtRoute("GetCoupon", new { id = coupon.Id }, obj);
}).WithName("CreateCoupon")
.Accepts<CouponCreateDTO>("application/json")
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

