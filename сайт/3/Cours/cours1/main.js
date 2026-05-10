document.addEventListener("DOMContentLoaded", function () {
  const tabButtons = document.querySelectorAll(".tab-btn");
  const tabPanes = document.querySelectorAll(".tab-pane");

  // Функция переключения вкладок
  function switchTab(tabId) {
    // Убираем активный класс у всех кнопок и вкладок
    tabButtons.forEach((btn) => btn.classList.remove("active"));
    tabPanes.forEach((pane) => pane.classList.remove("active"));

    // Добавляем активный класс нужной кнопке и вкладке
    document.querySelector(`[data-tab="${tabId}"]`).classList.add("active");
    document.getElementById(tabId).classList.add("active");

    // Прокрутка к началу контента при смене вкладки
    window.scrollTo(0, 0);
  }

  // Обработчики для кнопок вкладок
  tabButtons.forEach((button) => {
    button.addEventListener("click", function () {
      const tabId = this.getAttribute("data-tab");
      switchTab(tabId);
    });
  });

  // По умолчанию открываем первую вкладку
  switchTab("tab1");

  // Поддержка навигации с клавиатуры
  document.addEventListener("keydown", function (e) {
    if (e.key === "ArrowRight" || e.key === "ArrowDown") {
      e.preventDefault();
      const currentIndex = Array.from(tabButtons).findIndex((btn) =>
        btn.classList.contains("active"),
      );
      const nextIndex = (currentIndex + 1) % tabButtons.length;
      switchTab(tabButtons[nextIndex].getAttribute("data-tab"));
    } else if (e.key === "ArrowLeft" || e.key === "ArrowUp") {
      e.preventDefault();
      const currentIndex = Array.from(tabButtons).findIndex((btn) =>
        btn.classList.contains("active"),
      );
      const prevIndex =
        (currentIndex - 1 + tabButtons.length) % tabButtons.length;
      switchTab(tabButtons[prevIndex].getAttribute("data-tab"));
    }
  });
});

const test1 = document.getElementById("test1");

const btnTest1 = document.getElementById("chek");

chek.addEventListener("click", function () {
  if (test1.value === "Привет Евгений") {
    console.log("Ok");
    alert("Ок");
    return;
  }
  console.log("Привет Евгений");
  alert("error");
  console.log(test1.value);
});

const rbBtnTest1 = document.getElementById("chek_endTest1");

const rbBtnTest2 = document.getElementById("chek_endTest2");

const rbBtnTest3 = document.getElementById("chek_endTest3");

const rbBtnTest4 = document.getElementById("chek_endTest4");

const rbBtnTest5 = document.getElementById("chek_endTest5");

const rbBtnTest6 = document.getElementById("chek_endTest6");

rbBtnTest1.addEventListener("click", function () {
  const rbTest1_1 = document.querySelector('input[name="test1"]:checked').value;

  if (rbTest1_1 == 1) {
    alert("Good!");
  } else {
    alert("error");
  }
});

rbBtnTest2.addEventListener("click", function () {
  const rbTest2_1 = document.querySelector('input[name="test2"]:checked').value;

  if (rbTest2_1 == 2) {
    alert("Good!");
  } else {
    alert("error");
  }
});

rbBtnTest3.addEventListener("click", function () {
  const rbTest3_1 = document.querySelector('input[name="test3"]:checked').value;

  if (rbTest3_1 == 2) {
    alert("Good!");
  } else {
    alert("error");
  }
});

rbBtnTest4.addEventListener("click", function () {
  const rbTest4_1 = document.querySelector('input[name="test4"]:checked').value;

  if (rbTest4_1 == 3) {
    alert("Good!");
  } else {
    alert("error");
  }
});

rbBtnTest5.addEventListener("click", function () {
  const rbTest5_1 = document.querySelector('input[name="test5"]:checked').value;

  if (rbTest5_1 == 3) {
    alert("Good!");
  } else {
    alert("error");
  }
});

rbBtnTest6.addEventListener("click", function () {
  const rbTest6_1 = document.querySelector('input[name="test6"]:checked').value;

  if (rbTest6_1 == 3) {
    alert("Good!");
  } else {
    alert("error");
  }
});
