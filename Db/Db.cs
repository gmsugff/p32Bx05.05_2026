/*

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
using System.Linq.Expressions;
using System.Net;
using System.Net.Mail;
using System.Text;


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





app.MapPost("/register", async (Users req, AppDbContext db) =>
{
    var user = await db.users.FirstOrDefaultAsync(u => u.UserName == req.UserName && u.Password == req.Password);
    if (user == null)
    {
        user = new Users { UserName = req.UserName, Password = req.Password, AvatarUrl = @"C:/Users/34qw3/source/repos/Новый курс 25.02_2026/сайт/3/avat/cfd234fb73c2913b458c5d811b81640d.jpg", IsPasswordTemporary = false };
        db.users.Add(user);
        await db.SaveChangesAsync();
    }


    return Results.Ok(new { userId = user.Id, name = user.UserName, password = user.Password });

});

app.MapPost("/addCourseUser", async (Courses req, AppDbContext db) =>
{
    var user = await db.courses.FirstOrDefaultAsync(u => u.U == req.UserId && u.is_course1 == req.is_course1 && u.is_course2 == req.is_course2 && u.is_course3 == req.is_course3);
    if (user == null)
    {
        user = new CoursUser { UserId = req.UserId, is_course1 = req.is_course1, is_course2 = req.is_course2, is_course3 = req.is_course3 };
        db.addCoursUsers.Add(user);
        await db.SaveChangesAsync();
    }


    return Results.Ok(new { userId = user.Id, is_course1 = req.is_course1, is_course2 = req.is_course2, is_course3 = req.is_course3 });

});







app.MapGet("/history/{userId:int}", async (int userId, AppDbContext db) =>
{

    var history = await db.histories
                        .Where(h => h.UserId == userId)
                        .OrderByDescending(h => h.Id)
                        .ToListAsync();


    return Results.Ok(new { history });
});
*//*app.MapGet("/requestUnderConsideration", async ( AppDbContext db) =>
{

    var history = await db.applications
                        .Where(h => h.Id >= 0)
                        .OrderByDescending(h => h.Id)
                        .ToListAsync();


    return Results.Ok(new { history });
});*//*


app.MapGet("/Status/{UserId:int}", async (int UserId, AppDbContext db) =>
{

    var history = await db.applications
    .Where(h => h.UserId == UserId)
    .OrderByDescending(h => h.Id)
    .FirstOrDefaultAsync();

    return Results.Ok(history.Status);
});



*//*app.MapGet("/requestUnderConsiderationDelite/{formeId:int}", async (int formeId,AppDbContext db) =>
{

    var history = await db.applications
    .Where(h => h.Id == formeId)
    .OrderByDescending(h => h.Id)
    .FirstOrDefaultAsync();
        db.applications.Remove(history);
        await db.SaveChangesAsync(); 
    
    return Results.Ok(new { history });
});
*//*


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

app.MapPost("/formNew", async (Applications req, AppDbContext db) =>
{

    var user = await db.users.FirstOrDefaultAsync(u => u.Id == req.UserId);
    if (user == null)
    {
        user = new Users { UserName = req.Name, Password = "hh/sdk?sdozgjjd!", Email = req.Email, AvatarUrl = @"C:/Users/34qw3/source/repos/Новый курс 25.02_2026/сайт/3/avat/cfd234fb73c2913b458c5d811b81640d.jpg", IsPasswordTemporary = true };
        db.users.Add(user);
        // 2. Отправка уведомления на почту
        try
        {
            var emailMessage = new MimeMessage();
            // От кого (ваш Gmail)
            emailMessage.From.Add(new MailboxAddress("Система отправки", "testmaillget@gmail.com"));
            // Кому (например, администратору или самому пользователю)
            emailMessage.To.Add(new MailboxAddress($"{req.Name}", $"{req.Email}"));

            emailMessage.Subject = "Данные для входа в профиль отправлены";

            // Формируем текст письма
            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = $@"
                <div style='font-family: Arial, sans-serif; border: 1px solid #eee; padding: 20px;'>
                    <h2 style='color: #2c3e50;'>Здравствуйте, {req.Name}!</h2>
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
    var entry = await db.applications.FirstOrDefaultAsync(u => u.Name == user.UserName && u.UserId == user.Id);
    if (entry == null)
    {

        entry = new Applications
        {
            UserId = user.Id,
            Name = user.UserName,
            Email = user.Email,
            Phone = req.Phone,
            Comment = req.Comment,
            Status = "В обработке",
            CreatedAt = DateTime.UtcNow,

        };



        db.applications.Add(entry);
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


    var req = await db.applications
                        .Where(h => h.Id == formId)
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
            emailMessage.To.Add(new MailboxAddress($"{req.Name}", $"{req.Email}"));

            emailMessage.Subject = "Новая форма отправлена";

            // Формируем текст письма
            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = $@"
                <div style='font-family: Arial, sans-serif; border: 1px solid #eee; padding: 20px;'>
                    <h2 style='color: #2c3e50;'>Здравствуйте, {req.Name}!</h2>
                    <p>Мы получили вашу форму и уже приступили к её обработке.</p>
                    <hr>
                    <p><b>Ваши данные:</b></p>
                    <ul>
                        <li><b>Телефон:</b> {req.Phone}</li>
                        <li><b>Ваш вопрос:</b> {req.Comment}</li>
                        <li><b>Дата создания:</b> {req.CreatedAt}</li>          
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
            message = $"Форма сохранена и уведомление отправлено {req.Email}"
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
        var history = new Histories
        {
            UserId = req.UserId,
            Expression = $"{req.Num1} {req.Operation} {req.Num2}",
            Result = result



        };

        db.histories.Add(history);
        await db.SaveChangesAsync();
    }
    catch
    {
        return Results.Ok("404");
    }


    return Results.Ok(new { result = result, userId = req.UserId });

});





// Запуск сервера
app.Run();

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

    public DbSet<Users> users { get; set; }
    public DbSet<Applications> applications { get; set; }
    public DbSet<Histories> histories { get; set; }
    public DbSet<Courses> courses { get; set; }
    public DbSet<CourseSections> courseSections { get; set; }
    public DbSet<UserProgress> userProgress { get; set; }


}
class Users

{
    public int Id { get; set; }
    //Имя / Логин.
    public string UserName { get; set; }
    //Пароль.
    public string Password { get; set; }
    //Почта клиента.
    public string? Email { get; set; }
    //Ссылка на фото (например: /avatars/1.jpg).
    public string AvatarUrl { get; set; }
    /// <summary>
    /// Флаг true/false. 
    /// Если true, система заставит сменить пароль при первом входе (используется при авторегистрации).
    /// </summary>
    public bool IsPasswordTemporary { get; set; }



}
class Applications
{
    //Номер заявки.
    public int Id { get; set; }
    //ID пользователя (если был авторизован) или null (если гость).
    public int? UserId { get; set; }
    //Контактные данные.
    public string Name { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
    //Пожелания клиента.
    public string? Comment { get; set; }
    //Текущий статус (В обработке,Одобрена,Отклонена).
    public string Status { get; set; }
    //Дата создания.
    public DateTime CreatedAt { get; set; }
}
class Histories
{
    public int Id { get; set; }
    //Кто считал.
    public int UserId { get; set; }
    //Пример (например, "5 + 10").
    public string Expression { get; set; }
    //Ответ (15).
    public double Result { get; set; }
}

class Courses
{
    public int Id { get; set; }
    //Название курса
    public string Title { get; set; }
    //Описание
    public string Description { get; set; }
    //Ссылка на обложку (/courses/cover_1.jpg).
    public string CoverUrl { get; set; }
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


*/