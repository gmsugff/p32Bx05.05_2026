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
