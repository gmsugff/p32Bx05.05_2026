const historyForm = document.getElementById("history-form");
const historyCalc = document.getElementById("Calc-history");
const nameCalc = document.getElementById("name");
const emailCalc = document.getElementById("email");
const phoneCalc = document.getElementById("phone");
const calcForm = document.getElementById("calc-form");
const calcResult = document.getElementById("calc-result");
const statusForm = document.getElementById("applicationStatu");

let localhost = {
  userId: -1,
  username: "null",
  _pasword: "null",
  phone: "null",
  email: "null",
  is_run: false,
  courseId: -1,
};

let Cours = {
  course1Name: "Базовый",
  course2Name: "Стандарт",
  course3Name: "Премиум",
};

const savedSession = localStorage.getItem("user_session");
if (savedSession) {
  localhost = JSON.parse(savedSession);
  nameCalc.innerHTML = localhost.username;
  if (localhost.email != "null") {
    emailCalc.innerHTML = localhost.email;
  }

  if (localhost.phone != "null") {
    phoneCalc.innerHTML = localhost.phone;
  }
}
history();

async function history() {
  userId = localhost.userId;
  console.log(userId);

  const response = await fetch(
    "https://localhost:7107/history/" + localhost.userId,
  );

  const data2 = await response.json();
  console.log(data2);
  const c = data2["history"];
  console.log(c);
  historyCalc.innerHTML = "";
  c.forEach((item) => {
    const li = document.createElement("li");
    li.style.cssText =
      "padding: 10px; background: #e9ecef; margin-bottom: 5px; border-radius: 5px; font-family: monospace; font-size: 16px;";

    li.textContent = `${item.expression} = ${item.result}`;
    historyCalc.appendChild(li);
  });
}

calcForm.addEventListener("submit", async (e) => {
  // 3. Останавливаем стандартную отправку формы (чтобы страница не перезагрузилась)
  e.preventDefault();

  // 4. Получаем значения из полей. parseFloat превращает текст в числа с плавающей точкой
  const num1 = parseFloat(document.getElementById("num1").value);
  const num2 = parseFloat(document.getElementById("num2").value);
  const operation = document.getElementById("operation").value;
  if (localhost.userId == null) {
    console.log("error");
    calcResult.textContent = `Error:I need an entrance `;
  }
  try {
    // 5. Отправляем запрос на C# сервер по новому адресу /calculate
    const response = await fetch("https://localhost:7107/calculate", {
      method: "POST", // Метод передачи данных
      headers: {
        "Content-Type": "application/json", // Говорим серверу, что шлем JSON
      },
      // 6. Упаковываем переменные в JSON-строку
      body: JSON.stringify({
        num1: num1,
        num2: num2,
        operation: operation,
        UserId: localhost.userId,
      }),
    });
    console.log(localhost.userId);
    // 7. Если сервер ответил ошибкой (например, статус 400 Bad Request при делении на ноль)
    if (!response.ok) {
      // Читаем текст ошибки, который прислал C#
      const errorText = await response.text();
      throw new Error(errorText); // Выбрасываем ошибку, чтобы сработал блок catch
    }

    // 8. Если всё ок, парсим JSON-ответ от сервера (там будет { result: число })
    const data = await response.json();
    console.log(data);
    // 9. Выводим результат в HTML, красим текст в синий
    calcResult.textContent = `Результат: ${data.result}`;
    calcResult.style.color = "#007bff";
  } catch (error) {
    // 10. Если произошла ошибка (сервер выключен или деление на ноль)
    console.error("Ошибка калькулятора:", error);
    calcResult.textContent = `Error: ${error.message}`;
    calcResult.style.color = "red"; // Красим в красный
  }
  history();
});
Status();
chekCours();
async function Status() {
  const response6 = await fetch(
    "https://localhost:7107/Status/" + localhost.userId,
  );

  const data6 = await response6.json();
  console.log(data6);
  if (data6 == true) {
    console.log("В процессе отправки");
    statusForm.innerHTML = "В процессе отправки";
  } else if (data6 == false) {
    console.log("Отправлено");
    statusForm.innerHTML = "Отправлено";
  } else if (data6 != true && data6 != false) {
    console.log(data6);
  }
}
function chekCours() {
  const nameCours = document.getElementById("coursName");
  const IsNamberCours = document.getElementById("coursStat");

  //  условие
  if (localhost.courseId >0) {
    const btn = document.createElement("button");
    const divBtn = document.getElementById("btn");
    btn.id = "my-button-id";
    btn.className = "submit-btn"; // Используем ваш CSS класс
    btn.type = "button"; // Важно: тип button, чтобы она не отправляла форму случайно
    btn.textContent = "ОК";

    if (localhost.courseId ==1) {
      nameCours.innerHTML = `${Cours.course1Name}`;
      IsNamberCours.innerHTML = `Ok`;
      btn.onclick = async (e) => {
        e.preventDefault();
        window.location.href = "./Cours/cours1/cours1.html";
      };
    } else if (localhost.courseId ==2) {
      nameCours.innerHTML = `${Cours.course2Name}`;
      IsNamberCours.innerHTML = `Ok`;
      btn.onclick = async (e) => {
        e.preventDefault();
        window.location.href = "./Cours/cours2/cours2.html";
      };
    } else if (localhost.courseId == 3) {
      nameCours.innerHTML = `${Cours.course3Name}`;
      IsNamberCours.innerHTML = `Ok`;
      btn.onclick = async (e) => {
        e.preventDefault();
        window.location.href = "./Cours/cours3/cours3.html";
      };
    } else {
      nameCalc.innerHTML = "Нет курса";
      divBtn.remove(btn);
    }
    divBtn.appendChild(btn);
  }
  
  else {
      nameCours.innerHTML = "Нет курса";
      
    }
}

document.addEventListener("DOMContentLoaded", function () {
  const tabLinks = document.querySelectorAll(".tab-link");
  const tabPanes = document.querySelectorAll(".tab-pane");

  // Функция для переключения вкладок
  function switchTab(tabId) {
    // Убираем активный класс у всех ссылок и панелей
    tabLinks.forEach((link) => link.classList.remove("active"));
    tabPanes.forEach((pane) => pane.classList.remove("active"));

    // Находим текущую ссылку и панель
    const currentLink = document.querySelector(`a[href="#${tabId}"]`);
    const currentPane = document.getElementById(tabId);

    // Добавляем активный класс
    currentLink.classList.add("active");
    currentPane.classList.add("active");
  }

  // Добавляем обработчики событий для каждой ссылки
  tabLinks.forEach((link) => {
    link.addEventListener("click", function (e) {
      e.preventDefault(); // Отменяем стандартное поведение ссылки
      const tabId = this.getAttribute("href").substring(1); // Получаем ID вкладки
      switchTab(tabId);
    });
  });
});
