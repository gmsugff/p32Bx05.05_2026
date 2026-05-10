using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using MimeKit;
using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Ocsp;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Net;
using System.Net.Mail;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;


SQLitePCL.Batteries.Init();
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors();
builder.Services.AddCors(); // Добавляем поддержку CORS (чтобы JS с другого порта мог делать запросы)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=app.db"));


var app = builder.Build();

// Настраиваем CORS: разрешаем любые адреса, методы и заголовки
app.UseCors(policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

// Стандартный маршрут для проверки
app.MapGet("/", () => "Hello World!");





app.MapPost("/register", async (User req, AppDbContext db) =>
{
    var user = await db.Users.FirstOrDefaultAsync(u => u.UserName == req.UserName && u.Password == req.Password);
    if (user == null)
    {
        user = new User { UserName = req.UserName, Password = req.Password };
        db.Users.Add(user);
        await db.SaveChangesAsync();
    }


    return Results.Ok(new { userId = user.Id, name = user.UserName, password = user.Password });

});

app.MapPost("/addCourseUser", async (CoursUser req, AppDbContext db) =>
{
    var user = await db.addCoursUsers.FirstOrDefaultAsync(u => u.UserId == req.UserId && u.CoursId == req.CoursId );
    if (user == null)
    {
        user = new CoursUser { UserId = req.UserId, CoursId = req.CoursId };
        db.addCoursUsers.Add(user);
        await db.SaveChangesAsync();
    }


    return Results.Ok(new { userId = user.Id, coursId = user.CoursId });

});







app.MapGet("/history/{userId:int}", async (int userId, AppDbContext db) =>
{

    var history = await db.Histories
                        .Where(h => h.UserId == userId)
                        .OrderByDescending(h => h.Id)
                        .ToListAsync();


    return Results.Ok(new { history });
});
app.MapGet("/requestUnderConsideration", async (AppDbContext db) =>
{

    var history = await db.applicationStatus
                        .Where(h => h.Id >= 0)
                        .OrderByDescending(h => h.Id)
                        .ToListAsync();


    return Results.Ok(new { history });
});


app.MapGet("/Status/{UserId:int}", async (int UserId, AppDbContext db) =>
{

    var history = await db.Forms
    .Where(h => h.UserId == UserId)
    .OrderByDescending(h => h.Id)
    .FirstOrDefaultAsync();


    if (history != null)
    {
        var history2 = await db.applicationStatus
        .Where(h => h.Id == history.applicationStatuId)
        .OrderByDescending(h => h.Id)
        .FirstOrDefaultAsync();
        if (history2 != null)
        {
            return Results.Ok(true); ;
        }
        return Results.Ok(false);
    }
    return Results.Ok();
});



app.MapGet("/requestUnderConsiderationDelite/{formeId:int}", async (int formeId, AppDbContext db) =>
{

    var history = await db.applicationStatus
    .Where(h => h.Id == formeId)
    .OrderByDescending(h => h.Id)
    .FirstOrDefaultAsync();
    db.applicationStatus.Remove(history);
    await db.SaveChangesAsync();

    return Results.Ok(new { history });
});



app.MapPost("/application", async (HttpRequest request, ILogger<Program> logger) =>
{
    var data = await request.ReadFromJsonAsync<object>();
    logger.LogInformation("Получена заявка: {Data}", data);
    var logLine = System.Text.Json.JsonSerializer.Serialize(new
    {
        Data = data,
        Timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss:fffZ") + Environment.NewLine
    });

    await System.IO.File.AppendAllTextAsync("application.log", logLine);

    return Results.Created("/application", new { success = true, message = "Заявка принята", data = data });
}
);

app.MapPost("/formNew", async (Form req, AppDbContext db) =>
{

    var user = await db.Users.FirstOrDefaultAsync(u => u.Id == req.UserId);
    if (user == null)
    {
        user = new User { UserName = req.UserName, Password = "hh/sdk?sdozgjjd!", Email = req.email };
        db.Users.Add(user);
        // 2. Отправка уведомления на почту
        try
        {
            var emailMessage = new MimeMessage();
            // От кого (ваш Gmail)
            emailMessage.From.Add(new MailboxAddress("Система отправки", "testmaillget@gmail.com"));
            // Кому (например, администратору или самому пользователю)
            emailMessage.To.Add(new MailboxAddress($"{req.UserName}", $"{req.email}"));

            emailMessage.Subject = "Данные для входа в профиль отправлены";

            // Формируем текст письма
            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = $@"
                <div style='font-family: Arial, sans-serif; border: 1px solid #eee; padding: 20px;'>
                    <h2 style='color: #2c3e50;'>Здравствуйте, {req.UserName}!</h2>
                    <p>Мы получили вашу форму и уже приступили к её обработке.</p>
                    <hr>
                    <p><b>Ваши данные для входа в профиль:</b></p>
                    <ul>
                        <li><b>Имя(Логин):</b> {user.UserName}</li>
                        <li><b>Password:</b> {user.Password}</li>
                        <li><b>Email:</b> {user.Email}</li>          
                    </ul>
                    <p style='color: #7f8c8d; font-size: 12px;'>Это автоматическое уведомление, на него не нужно отвечать.</p>
                </div>";

            emailMessage.Body = bodyBuilder.ToMessageBody();

            using (var client = new MailKit.Net.Smtp.SmtpClient())
            {
                // Используем порт 587, который у вас прошел тест
                await client.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);

                // ВАЖНО: Используйте 16-значный ПАРОЛЬ ПРИЛОЖЕНИЯ (переменная pas)
                // Убедитесь, что pas содержит правильный код без пробелов
                await client.AuthenticateAsync("testmaillget@gmail.com", "lmrb nwdf gfur sjzx");

                await client.SendAsync(emailMessage);
                await client.DisconnectAsync(true);
            }
        }
        catch (Exception ex)
        {
            // Логируем ошибку, если почта не ушла, но форму в базе сохраняем
            Console.WriteLine($"Ошибка отправки почты: {ex.Message}");
        }
    }
    var entry = await db.Forms.FirstOrDefaultAsync(u => u.UserName == user.UserName && u.UserId == user.Id);
    if (entry == null)
    {
        applicationStatu appStatu = new applicationStatu { IsShipped = "Ok" };
        db.applicationStatus.Add(appStatu);
        await db.SaveChangesAsync();

        if (req.CoursId != 0)
        {
            entry = new Form
            {
                UserId = req.UserId,
                applicationStatuId = appStatu.Id,
                UserName = req.UserName,
                phone = req.phone,
                email = req.email,
                Comment = req.Comment,

            };
        }
        else
        {
            entry = new Form
            {
                UserId = req.UserId,
                applicationStatuId = appStatu.Id,
                UserName = req.UserName,
                phone = req.phone,
                email = req.email,
                Comment = req.Comment

            };
        }

        db.Forms.Add(entry);
        await db.SaveChangesAsync();
    }
    else
    {
        return Results.Ok(new
        {
            success = true,
            message = $"Форма не сохранена и уведомление не отправлено  {req.Id}",
            data = entry

        });
    }
    return Results.Ok(new
    {
        success = true,
        message = $"Форма сохранена и уведомление отправлено  {req.Id}",
        data = entry

    });
});
app.MapGet("/form/{formId:int}", async (int formId, AppDbContext db) =>
{


    var req = await db.Forms
                        .Where(h => h.applicationStatuId == formId)
                        .OrderByDescending(h => h.Id)
                        .FirstOrDefaultAsync();
    if (req != null)
    {
        // 2. Отправка уведомления на почту
        try
        {
            var emailMessage = new MimeMessage();
            // От кого (ваш Gmail)
            emailMessage.From.Add(new MailboxAddress("Система заявок", "testmaillget@gmail.com"));
            // Кому (например, администратору или самому пользователю)
            emailMessage.To.Add(new MailboxAddress($"{req.UserName}", $"{req.email}"));

            emailMessage.Subject = "Новая форма отправлена";

            // Формируем текст письма
            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = $@"
                <div style='font-family: Arial, sans-serif; border: 1px solid #eee; padding: 20px;'>
                    <h2 style='color: #2c3e50;'>Здравствуйте, {req.UserName}!</h2>
                    <p>Мы получили вашу форму и уже приступили к её обработке.</p>
                    <hr>
                    <p><b>Ваши данные:</b></p>
                    <ul>
                        <li><b>Телефон:</b> {req.phone}</li>
                        <li><b>Ваш вопрос:</b> {req.Comment}</li>
                        <li><b>Ваш Курс:</b> {req.CoursId}</li>          
                    </ul>
                    <p style='color: #7f8c8d; font-size: 12px;'>Это автоматическое уведомление, на него не нужно отвечать.</p>
                </div>";

            emailMessage.Body = bodyBuilder.ToMessageBody();

            using (var client = new MailKit.Net.Smtp.SmtpClient())
            {
                // Используем порт 587, который у вас прошел тест
                await client.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);

                // ВАЖНО: Используйте 16-значный ПАРОЛЬ ПРИЛОЖЕНИЯ (переменная pas)
                // Убедитесь, что pas содержит правильный код без пробелов
                await client.AuthenticateAsync("testmaillget@gmail.com", "lmrb nwdf gfur sjzx");

                await client.SendAsync(emailMessage);
                await client.DisconnectAsync(true);
            }
        }
        catch (Exception ex)
        {
            // Логируем ошибку, если почта не ушла, но форму в базе сохраняем
            Console.WriteLine($"Ошибка отправки почты: {ex.Message}");
        }

        return Results.Ok(new
        {
            message = $"Форма сохранена и уведомление отправлено {req.email}"
        });
    }
    else
    {
        return Results.Ok(new { message = $"Форма сохранена и не уведомление отправлено {formId}", data = req });

    }

});


app.MapPost("/reg", async (HttpRequest request, ILogger<Program> logger) =>
{
    var data = await request.ReadFromJsonAsync<object>();
    logger.LogInformation("Получена заявка: {Data}", data);




    return Results.Created("/application", new { success = true, message = "Заявка принята", data = data });
}
);

// ======== НОВЫЙ МАРШРУТ: КАЛЬКУЛЯТОР ========
// Создаем endpoint /calculate, который ждет POST-запрос с объектом CalcRequest

app.MapPost("/calculate", async (CalcRequest req, AppDbContext db) =>
{


    // Переменная для хранения итогового результата
    double result = 0;
    // Проверяем, какой знак операции нам прислал фронтенд
    switch (req.Operation)
    {


        case "+":
            result = req.Num1 + req.Num2;
            break;
        case "-":
            result = req.Num1 - req.Num2;
            break;
        case "*":
            result = req.Num1 * req.Num2;
            break;
        case "/":
            // Защита от деления на ноль
            if (req.Num2 == 0)
            {
                // Возвращаем статус 400 (Bad Request) и текст ошибки
                return Results.BadRequest("Ошибка: Деление на ноль невозможно!");
            }
            result = req.Num1 / req.Num2;
            break;
        case "?=":
            result = Convert.ToDouble(req.Num1 == req.Num2);
            break;
        case "%":
            result = req.Num1 % req.Num2;
            break;
        default:
            return Results.BadRequest("Ошибка: Неизвестная операция");

    }

    // Возвращаем статус 200 (OK) и JSON с результатом: { "result": 15.5 }




    if (req.UserId > -1)
    {
        Results.Ok(new { result = 3 });
        if (req == null)
        {
            req = new CalcRequest { Num1 = req.Num1, Num2 = req.Num2, Operation = req.Operation, UserId = req.UserId };
            await db.SaveChangesAsync();
            Console.WriteLine(req.UserId);
            if (req.UserId == null)
            {
                return Results.Ok(req.UserId);
            }
        }
        Results.Ok(new { result = 4 });


    }
    try
    {
        var history = new CalcHistory
        {
            UserId = req.UserId,
            Expression = $"{req.Num1} {req.Operation} {req.Num2}",
            Result = result



        };

        db.Histories.Add(history);
        await db.SaveChangesAsync();
    }
    catch
    {
        return Results.Ok("404");
    }


    return Results.Ok(new { result = result, userId = req.UserId });

});


app.MapPost("/addNew_Cours", async (CourseRequest req, AppDbContext db) =>
{
    // 1. Ищем или создаем курс
    var course = await db.courses.FirstOrDefaultAsync(u => u.Title == req.Name);
    if (course == null)
    {
        course = new Courses
        {
            Title = req.Name,
            Description = "", 
            CoverUrl = ""     
        };
        db.courses.Add(course);
        await db.SaveChangesAsync();
    }

    // 2. Ищем, нет ли уже секции с таким порядковым номером в этом курсе
    var section = await db.courseSections
        .FirstOrDefaultAsync(s => s.CourseId == course.Id && s.Order == req.Number);

    if (section == null)
    {
        section = new CourseSections
        {
            CourseId = course.Id,
            Order = req.Number,
            TheoryText = req.T,
            HasPractice = true,
            PracticeTask = req.P,
            PracticeExpectedAnswer = req.Otv
        };
        db.courseSections.Add(section);
    }
    else
    {
        section.TheoryText = req.T;
        section.PracticeTask = req.P;
        section.PracticeExpectedAnswer = req.Otv;
    }

    await db.SaveChangesAsync();

    return Results.Ok(new { Message = "Данные успешно сохранены", CourseId = course.Id });
});

// Запуск сервера
app.Run();
public record CourseRequest(string Name, string T, string P, int Number, string Otv);

// ==========================================
// Модель данных (Класс) для калькулятора
// ==========================================
// C# автоматически сопоставит JSON от JS с этими свойствами.
// Свойства обязательно должны быть с большой буквы (C# сам поймет, если в JS были с маленькой).
public class CalcRequest
{
    public double Num1 { get; set; }      // Первое число
    public double Num2 { get; set; }      // Второе число
    public string Operation { get; set; } // Операция (+, -, *, /)
    public int UserId { get; set; }
}

class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<User> Users { get; set; }
    public DbSet<CalcHistory> Histories { get; set; }
    public DbSet<Form> Forms { get; set; }
    public DbSet<applicationStatu> applicationStatus { get; set; }
    public DbSet<CoursUser> addCoursUsers { get; set; }
    public DbSet<Courses> courses { get; set; }
    public DbSet<CourseSections> courseSections { get; set; }
    public DbSet<UserProgress>userProgresses { get; set; }


}
class User

{
    public int Id { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public string? Email { get; set; }
    public bool? Is_applicationStatu { get; set; }
    
    public CoursUser coursUser { get; set; }

}
class CalcHistory
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Expression { get; set; }
    public double Result { get; set; }
}



class Form
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int applicationStatuId { get; set; }
    public string? UserName { get; set; }
    public string? phone { get; set; }
    public string? email { get; set; }
    public string? Comment { get; set; }
    public int? CoursId { get; set; }
}


class applicationStatu
{
    public int Id { get; set; }
    public string? IsShipped { get; set; }
}

class CoursUser
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int? CoursId { get; set; }
    
}

class Courses
{
    public int Id { get; set; }
    //Название курса
    public string Title { get; set; }
    //Описание
    public string? Description { get; set; }
    //Ссылка на обложку (/courses/cover_1.jpg).
    public string? CoverUrl { get; set; }
}
class CourseSections
{
    public int Id { get; set; }
    //К какому курсу относится раздел.
    public int CourseId { get; set; }
    //Порядковый номер раздела (чтобы выводить по порядку 1, 2, 3...).
    public int Order { get; set; }
    //Текст урока.
    public string TheoryText { get; set; }
    //Картинка к уроку(необязательно).
    public string? TheoryImageUrl { get; set; }
    //Есть ли в конце упражнение (true/false).
    public bool HasPractice { get; set; }
    //Задание (например: "Напишите тег для абзаца").
    public string? PracticeTask { get; set; }
    //Правильный ответ для автопроверки (например: <p>).
    public string? PracticeExpectedAnswer { get; set; }

}

class UserProgress
{
    public int Id { get; set; }
    //Студент.
    public int UserId { get; set; }
    //Изучаемый курс.
    public int CourseId { get; set; }
    //Сколько разделов (уроков) успешно пройдено и проверено. (По этому числу высчитывается процент % прогресса).
    public int CompletedSectionsCount { get; set; }
}