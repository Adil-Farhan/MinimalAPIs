using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using MinimalCupons;
using MinimalCupons.Data;
using MinimalCupons.DTO;
using MinimalCupons.Models;
using MinimalCupons.Response;
using System.Net;

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

app.MapGet("/api/coupon", (ILogger<Program> _logger) =>
{
    _logger.Log(LogLevel.Information, "Getting All Coupons");
    APIResponse response = new APIResponse();

    response.Results = CouponStore.couponsList;
    response.IsSuccess = true;
    response.StatusCode = HttpStatusCode.Created;

    return Results.Ok(response);

}).WithName("GetAllCoupon").Produces<IEnumerable<APIResponse>>(201);


app.MapGet("/api/coupon/{id:int}", (int id) =>
{
    APIResponse response = new APIResponse();

    response.Results = CouponStore.couponsList.FirstOrDefault(u => u.Id == id);
    response.IsSuccess = true;
    response.StatusCode = HttpStatusCode.Created;

    return Results.Ok(response);
}).WithName("GetCoupon").Produces<APIResponse>(201);





app.MapPost("/api/coupon/", async (IValidator<CouponCreateDTO> _validator
    , IMapper _mapper
    , ILogger<Program> _logger
    , [FromBody] CouponCreateDTO obj) =>
{
    var validationResult = await _validator.ValidateAsync(obj);
    APIResponse response = new APIResponse();

    _logger.Log(LogLevel.Information, "Creating a Coupon: " + obj.Name);

    if (!validationResult.IsValid)
    {
        response.Results = new CouponCreateDTO();
        response.IsSuccess = false;
        response.StatusCode = HttpStatusCode.BadRequest;
        response.ErrorMessages.Add(validationResult.Errors.FirstOrDefault().ToString());
        return Results.BadRequest(response);
    
    }

    if (CouponStore.couponsList.FirstOrDefault(x => x.Name.ToLower().Equals(obj.Name.ToLower())) != null)
    {
        response.Results = new CouponCreateDTO();
        response.IsSuccess = false;
        response.StatusCode = HttpStatusCode.BadRequest;
        response.ErrorMessages.Add("Coupon Name already Exists");
        return Results.BadRequest(response);
    }

    Coupon coupon = _mapper.Map<Coupon>(obj);
    coupon.Id = CouponStore.couponsList.OrderByDescending(x => x.Id).FirstOrDefault().Id + 1;
    CouponStore.couponsList.Add(coupon);

    
    response.Results = coupon;
    response.IsSuccess = true;
    response.StatusCode = HttpStatusCode.Created;

    return Results.CreatedAtRoute("GetCoupon", new { id = coupon.Id }, obj);
}).WithName("CreateCoupon")
.Accepts<CouponCreateDTO>("application/json")
.Produces<APIResponse>(201).Produces(400);





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

