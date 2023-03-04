using Microsoft.EntityFrameworkCore;
using MinimalShoppingListApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ApiDbContext>(opt => opt.UseInMemoryDatabase("ShoppingListApi"));


var app = builder.Build();

app.MapGet("/shoppinglist" , async (ApiDbContext db ) => 
            await db.Groceries.ToListAsync());

app.MapPost("/shoppingList", async (Grocery grocery, ApiDbContext db) =>

            {
                db.Groceries.Add(grocery);
                await db.SaveChangesAsync();

                return Results.Created($"/shoppinglist/{grocery.Id}", grocery);


            });


app.MapGet("/shoppingList/{id}", async (int id, ApiDbContext db) =>
            {
                var grocery = await db.Groceries.FindAsync(id);

                return grocery != null ? Results.Ok(grocery) : Results.NotFound();
            });


app.MapPut("/shoppingList/{id}", async(int id, Grocery grocery, ApiDbContext db) => 
            {
                var groceryItem = await db.Groceries.FindAsync(id);
                
                
                if(groceryItem != null )
                {
                    groceryItem.Name = grocery.Name;
                    groceryItem.Purchased = grocery.Purchased;

                    await db.SaveChangesAsync();
                    return Results.Ok(groceryItem);
                }else
                {
                    return Results.NotFound();
                }
              
            });

app.MapDelete("/shoppingList/{id}", async (int id, ApiDbContext db) =>
{
    var grocery = await db.Groceries.FindAsync(id);

    if ( grocery != null )
    {
        db.Groceries.Remove(grocery);
        await db.SaveChangesAsync();
        return Results.NoContent();

    } else
    {
        return Results.NotFound();
    }

});



if ( app.Environment.IsDevelopment() )
{
    app.UseSwagger();
    app.UseSwaggerUI();

}

app.UseHttpsRedirection();

app.Run();  