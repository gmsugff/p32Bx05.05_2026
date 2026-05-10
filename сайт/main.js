const form = document.querySelector("form");
let localhost = {
  userId: -1,
  username: "null",
  _pasword: "null",
  phone: "null",
  email: "null",
  is_run: false,
  courseId: -1,
};
let tmpCours={
  id: -1,
}
const savedSession = localStorage.getItem("user_session");
let isForm = false;
if (savedSession) {
  localhost = JSON.parse(savedSession);
}

async function Start_form() {
  // Собираем данные
  const name = document.getElementById("name").value;
  const phone = document.getElementById("phone").value;
  const email = document.getElementById("email").value;
  const comment = document.getElementById("comment").value;
  localhost.phone = phone;
  localhost.email = email;
  // Формируем объект для отправки
  let formData = {
    userId: localhost.userId,
    name: name,
    phone: phone,
    email: email,
    comment: comment,
  };
  
  try {
    // Отправляем POST-запрос
    const response = await fetch("https://localhost:7107/application", {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify(formData),
    });

    // Проверяем, успешен ли ответ
    if (!response.ok) {
      throw new Error(`Ошибка HTTP: ${response.status}`);
    }

    // Парсим ответ (если сервер что-то вернул)
    const result = await response.json();
    console.log("Успешно отправлено:", result);

    // Оповещаем пользователя
    alert("Спасибо! Ваша заявка отправлена.");
    form.reset(); // очищаем форму
    localStorage.setItem("user_session", JSON.stringify(localhost));
    
  } catch (error) {
    console.error("Ошибка при отправке:", error);
    alert("Произошла ошибка. Попробуйте позже.");
  }
  
  await End_json(name, phone, email, comment);
}
async function End_json(name, phone, email, comment) {
  isForm = false;
  // Проверка для отладки: выведите в консоль, что именно вы отправляете
  console.log(
    "Отправляем ID:",
    localhost.userId,
    "Тип:",
    typeof localhost.userId,
  );

  try {
    const response = await fetch("https://localhost:7107/formNew", {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify({
        // Приводим userId к числу на всякий случай
        UserId: localhost.userId,
        UserName: name,
        phone: phone,
        email: email,
        comment: comment,
        
      }),
    });

    if (!response.ok) throw new Error("Ошибка сервера");

    const result = await response.json();
    console.log("Успех!", result);
    isForm = true;
  } catch (error) {
    console.error("Ошибка десериализации на сервере:", error);
  }
}

form.addEventListener("submit", async (e) => {
  e.preventDefault();
  if (localhost.userId > 0) {
    await Start_form();
    
  }
});
form.addEventListener("submit", async (e) => {
  e.preventDefault();
  if (localhost.userId > 0) {
    await Start_form();
  }
});


  const course1 = document.getElementById("course-1");
  const course2 = document.getElementById("course-2");
  const course3 = document.getElementById("course-3");

  course1.addEventListener("click", async (e) => {
    e.preventDefault();
    localhost.courseId = 1;

    console.log("", localhost.courseId);
    
    localStorage.setItem("user_session", JSON.stringify(localhost));
  });
  course2.addEventListener("click", async (e) => {
    e.preventDefault();
    localhost.courseId = 2;
    console.log("", localhost.courseId);

  
    localStorage.setItem("user_session", JSON.stringify(localhost));
  });

  course3.addEventListener("click", async (e) => {
    e.preventDefault();
    localhost.courseId = 3;
    console.log("", localhost.courseId);
    localStorage.setItem("user_session", JSON.stringify(localhost));
  });

  async function PostAdd() {
    try {
      const response = await fetch("https://localhost:7107/addCourseUser", {
        method: "POST", // Метод передачи данных
        headers: {
          "Content-Type": "application/json", // Говорим серверу, что шлем JSON
        },
        body: JSON.stringify({
          UserId: localhost.userId,
          CoursId: localhost.courseId,
        }),
      });

      console.log(response);
    } catch {
      console.error();
    }
  }

