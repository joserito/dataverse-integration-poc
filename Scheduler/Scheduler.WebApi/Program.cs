using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.Resource;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));
builder.Services.AddAuthorization();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "SchedulerAPI", Version = "v1" });
    c.AddSecurityDefinition("OAuth2", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.OAuth2,
        Flows = new OpenApiOAuthFlows()
        {
            Implicit = new OpenApiOAuthFlow()
            {
                AuthorizationUrl = new Uri("https://login.microsoftonline.com/5806aa46-07f3-4e5f-9376-f1be2b899539/oauth2/v2.0/authorize"),
                TokenUrl = new Uri("https://login.microsoftonline.com/5806aa46-07f3-4e5f-9376-f1be2b899539/oauth2/v2.0/token"),
                Scopes = new Dictionary<string, string>
                {
                    { "api://f68eb041-0d67-4d31-80d7-2829cc34b515/Scheduler.Read", "Read Scheduler Information" },
                    { "api://f68eb041-0d67-4d31-80d7-2829cc34b515/Scheduler.Write", "Write Scheduler Information" }
                }
            }
        }
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "oauth2"
                },
                Scheme = "oauth2",
                Name = "oauth2",
                In = ParameterLocation.Header
            },
            new List < string > ()
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger(options => { options.SerializeAsV2 = true; });
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

var persons = new List<Person>
{
    new Person
    {
        PersonId = 1,
        FirstName = "Ross",
        LastName = "Geller",
        Email = "ross@friends.com"
    },
    new Person
    {
        PersonId = 2,
        FirstName = "Rachel",
        LastName = "Green",
        Email = "rachel@friends.com"
    },
    new Person
    {
        PersonId = 3,
        FirstName = "Monica",
        LastName = "Geller",
        Email = "monica@friends.com"
    },
    new Person
    {
        PersonId = 4,
        FirstName = "Chandler",
        LastName = "Bing",
        Email = "chandler@friends.com"
    },
    new Person
    {
        PersonId = 5,
        FirstName = "Joey",
        LastName = "Tribbiani",
        Email = "joey@friends.com"
    },
    new Person
    {
        PersonId = 6,
        FirstName = "Phoebe",
        LastName = "Buffay",
        Email = "phoebe@friends.com"
    }
};

var availableTimes = new List<AvailableTime>
{
    new AvailableTime
    {
        PersonId = 1,
        DateTime = DateTime.Now.Date.AddHours(9),
        Duration = 60
    },
    new AvailableTime
    {
        PersonId = 1,
        DateTime = DateTime.Now.Date.AddHours(9),
        Duration = 60
    },
    new AvailableTime
    {
        PersonId = 2,
        DateTime = DateTime.Now.Date.AddHours(9),
        Duration = 60
    }
};

var appointments = new List<Appointment>
{
    new Appointment
    {
        AppointmentId = 1,
        DateTime = DateTime.Now.Date.AddDays(1).AddHours(9),
        PersonId = 1
    }
};

app.MapGet("/", () =>{ })
.WithName("Root")
.WithDisplayName("Root");

app.MapGet("/person", [RequiredScope("Scheduler.Read")] (HttpContext httpContext) =>
{
    return persons;
})
.WithName("GetPersons")
.WithDisplayName("Retrieve Persons")
.RequireAuthorization();
app.MapGet("/person/{personId}", [RequiredScope("Scheduler.Read")] (int personId) =>
{
    return persons.FirstOrDefault(p => p.PersonId.Equals(personId));
})
.WithName("GetPersonById")
.WithDisplayName("Retrieve Persons by person identifier")
.RequireAuthorization();

app.MapGet("/availability", [RequiredScope("Scheduler.Read")] () =>
{
    return availableTimes;
})
.WithName("GetAvailabilities")
.WithDisplayName("Retrieve Availability")
.RequireAuthorization();
app.MapGet("/availability/{personId}", [RequiredScope("Scheduler.Read")] (int personId) =>
{
    return availableTimes
        .Where(at => at.PersonId.Equals(personId));
})
.WithName("GetAvailabilityByPersonId")
.WithDisplayName("Retrieve Availability by Person identifier")
.RequireAuthorization();

app.MapPost("/appointment", [RequiredScope("Scheduler.Write")] (Appointment appointment) =>
{
    appointments.Add(appointment);
})
.WithName("CreateAppointment")
.WithDisplayName("Create Appointment")
.RequireAuthorization();
app.MapPut("/appointment/{appointmentId}", [RequiredScope("Scheduler.Write")] (int appointmentId, Appointment appointment) =>
{
    var existingAppointment = appointments
        .FirstOrDefault(a => a.AppointmentId.Equals(appointmentId));
    if (existingAppointment != null)
        appointments.Remove(existingAppointment);
    appointments.Add(appointment);
})
.WithName("UpdateAppointment")
.WithDisplayName("Update Appointment")
.RequireAuthorization();
app.MapGet("/appointment", [RequiredScope("Scheduler.Read")] () =>
{
    return appointments;
})
.WithName("GetAppointments")
.WithDisplayName("Retrieve Appointments")
.RequireAuthorization();
app.MapGet("/appointment/{appointmentId}", [RequiredScope("Scheduler.Read")] (int appointmentId) =>
{
    return appointments
        .FirstOrDefault(a => a.AppointmentId.Equals(appointmentId));
})
.WithName("GetAppointmentByAppointmentId")
.WithDisplayName("Retrieve Appointment by Appointment Identifier")
.RequireAuthorization();

app.Run();


public class Person
{
    public int PersonId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
}

public class AvailableTime
{
    public int PersonId { get; set; }
    public DateTime DateTime { get; set; }
    public int Duration { get; set; }
}

public class Appointment
{
    public int AppointmentId { get; set; }
    public DateTime DateTime { get; set; }
    public int PersonId { get; set; }
}