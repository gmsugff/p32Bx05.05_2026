const regForm = document.getElementById("Regint-form");
const regResult = document.getElementById("reg-result");
const welcomMess = document.getElementById("welcom-mess");

let localhost = {
  userId: -1,
  username: "null",
  _pasword: "null",
  phone: "null",
  email: "null",
  is_run: false,
  courseId: -1,
};
let hio={
   username: "null",
  _pasword: "null",
}
regForm.addEventListener("submit", async (e) => {
  e.preventDefault();
  const username = document.getElementById("username").value;
  const password = document.getElementById("password").value;
  if (username !== "admin" || password !== "admin") {
    try {
      const response = await fetch("https://localhost:7107/register", {
        method: "POST", // Метод передачи данных
        headers: {
          "Content-Type": "application/json", // Говорим серверу, что шлем JSON
        },
        body: JSON.stringify({ username: username, password: password }),
      });

      if (!response.ok) {
        throw new Error("Server error");
      }

      const data = await response.json();
      const { userId, name, Password } = data;
      console.log("", userId, name, Password);
      localhost.userId = userId;
      localhost.username = name;
      localhost._password = password;
      localhost.is_run = true;

      localStorage.setItem("user_session", JSON.stringify(localhost));

      console.log("", localhost.username, localhost.userId);
      console.log(data);
      welcomMess.textContent = userId + ", " + name;
      window.location.href = "../3/profil.html";
    } catch (e) {
      console.error("Error:", e);
      alert(
        "Нет связи !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!",
      );
    }
  } else {
    
    if (hio.username !== "admin" || hio._pasword !== "admin"){
     kok(); 
    }
    hio.username = username;
    hio._pasword = pasword;
    // window.location.href = "../admin/admin.html";
  }
});

function kok() {
  const container = document.createElement("div");
  container.style.cssText = `
    position: relative;
    width: 100vw;
    height: 100vh;
  `;

  // Создаём изображение
  const img = document.createElement("img");
  img.src = "img/i.webp";
  img.alt = "Фон";
  img.style.cssText = `
    width: 100%;
    height: 100%;
    object-fit: cover;
  `;

  // Создаём кнопку
  const button = document.createElement("button");
  button.textContent = "Нажми меня";
  button.style.cssText = `
    position: absolute;
  bottom: 0;
  left: 0;
  padding: 2px 5px;
  background: rgba(255, 255, 255, 0.8);
  border: 0.5px solid rgba(0, 0, 0, 0.3);
  border-radius: 0.8px;
  font-size: 2px;
  color: #333;
  cursor: pointer;
  z-index: 0;
  `;

  button.addEventListener("click", () => {
    window.location.href = "../admin/admin.html";
  });

  // Собираем всё вместе
  container.appendChild(img);
  container.appendChild(button);
  document.body.appendChild(container);
}
