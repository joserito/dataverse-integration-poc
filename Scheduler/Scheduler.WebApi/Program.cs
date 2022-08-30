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

app.UseHttpsRedirection();

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

app.MapGet("/person", () =>
{
    return persons;
})
.WithName("GetPersons");
app.MapGet("/person/{personId}", (int personId) =>
{
    return persons.FirstOrDefault(p => p.PersonId.Equals(personId));
})
.WithName("GetPersonById");

app.MapGet("/availability", () =>
{
    return availableTimes;
})
.WithName("GetAvailabilities");
app.MapGet("/availability/{personId}", (int personId) =>
{
    return availableTimes
        .Where(at => at.PersonId.Equals(personId));
})
.WithName("GetAvailabilityByPersonId");

app.MapPost("/appointment", (Appointment appointment) =>
{
    appointments.Add(appointment);
})
.WithName("CreateAppointment");
app.MapPut("/appointment/{appointmentId}", (int appointmentId, Appointment appointment) =>
{
    var existingAppointment = appointments
        .FirstOrDefault(a => a.AppointmentId.Equals(appointmentId));
    if (existingAppointment != null)
        appointments.Remove(existingAppointment);
    appointments.Add(appointment);
})
.WithName("UpdateAppointment");
app.MapGet("/appointment", () =>
{
    return appointments;
})
.WithName("GetAppointments");
app.MapGet("/appointment/{appointmentId}", (int appointmentId) =>
{
    return appointments
        .FirstOrDefault(a => a.AppointmentId.Equals(appointmentId));
})
.WithName("GetAppointmentByAppointmentId");

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