using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<BadgeActivityData>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.MapGet("api/badgeActivities", ([FromServices] BadgeActivityData data) =>
{
    return data.GetAll();
})
.WithName("GetBadgeActivities")
.WithOpenApi();

app.MapGet("api/badgeActivities/{activityId}", ([FromServices] BadgeActivityData data, Guid activityId) =>
{
    return data.GetById(activityId);
})
.WithName("GetBadgeActivitiesId")
.WithOpenApi();


app.MapPost("api/badgeActivities", ([FromServices] BadgeActivityData data, Guid employeeId) =>
{
    var activityId = data.Create(employeeId);
    return Results.Created($"api/badgeActivities/{activityId.ActivityId}", activityId);
})
.WithName("PostBadgeActivities")
.WithOpenApi();

app.Run();

internal record BadgeActivity(Guid ActivityId, Guid EmployeeId, DateTime dateTime);

class BadgeActivityData
{
    private readonly Dictionary<Guid, BadgeActivity> _badgeActivities = new();

    public BadgeActivity Create(Guid employeeId)
    {        

        var badgeActivity = new BadgeActivity(Guid.NewGuid(), employeeId, DateTime.UtcNow);
               
        _badgeActivities[badgeActivity.ActivityId] = badgeActivity;

        return badgeActivity;
    }

    public List<BadgeActivity> GetAll()
    {
        return _badgeActivities.Values.ToList();
    }

    public BadgeActivity? GetById(Guid ActivityId)
    {
        if (_badgeActivities.ContainsKey(ActivityId))
        {
            return _badgeActivities[ActivityId];
        }

        return null;
    }
}
