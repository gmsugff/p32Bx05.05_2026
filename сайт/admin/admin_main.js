document.addEventListener("DOMContentLoaded", function () {
    const historyCalc = document.getElementById("Calc-history");
    const courseForm = document.getElementById("courseForm"); // Обращаемся к форме, а не к секции

    // ========== 1. ЛОГИКА ВКЛАДОК ==========
    const tabLinks = document.querySelectorAll(".tab-link");
    const tabPanes = document.querySelectorAll(".tab-pane");

    function switchTab(tabId) {
        tabLinks.forEach((link) => link.classList.remove("active"));
        tabPanes.forEach((pane) => pane.classList.remove("active"));

        const currentLink = document.querySelector(`a[href="#${tabId}"]`);
        const currentPane = document.getElementById(tabId);

        if(currentLink && currentPane) {
            currentLink.classList.add("active");
            currentPane.classList.add("active");
        }
    }

    tabLinks.forEach((link) => {
        link.addEventListener("click", function (e) {
            e.preventDefault();
            const tabId = this.getAttribute("href").substring(1);
            switchTab(tabId);
        });
    });

    // ========== 2. ЗАГРУЗКА ИСТОРИИ ==========
    loadHistory();

    async function loadHistory() {
        try {
            const response = await fetch("https://localhost:7107/requestUnderConsideration");
            const data2 = await response.json();
            const historyItems = data2["history"] || [];
            
            historyCalc.innerHTML = "";

            if (historyItems.length === 0) {
                historyCalc.innerHTML = "Нет записей";
                return;
            }

            historyItems.forEach((item) => {
                const li = document.createElement("li");
                li.style.cssText = "padding: 10px; background: rgb(233, 236, 239); margin-bottom: 5px; border-radius: 5px; font-family: monospace; font-size: 16px; display: flex; justify-content: space-between; align-items: center;";
                li.textContent = `FormId = ${item.id} | isShipped = ${item.isShipped}`;

                const btn = document.createElement("button");
                // Убрано btn.id = "my-button-id", так как ID должны быть уникальными!
                btn.className = "submit-btn";
                btn.type = "button"; 
                btn.textContent = "ОК";
                
                btn.onclick = async (e) => {
                    e.preventDefault();
                    try {
                        await fetch("https://localhost:7107/form/" + item.id);
                        await fetch("https://localhost:7107/requestUnderConsiderationDelite/" + item.id);
                        
                        li.remove(); // Современный метод удаления элемента
                        
                        // Если удалили последний элемент, показываем заглушку
                        if (historyCalc.children.length === 0) {
                            historyCalc.innerHTML = "Нет записей";
                        }
                    } catch (error) {
                        console.error("Ошибка при обработке/удалении:", error);
                    }
                };

                li.appendChild(btn);
                historyCalc.appendChild(li);
            });
        } catch (error) {
            console.error("Ошибка загрузки данных:", error);
            historyCalc.innerHTML = "Ошибка загрузки сервера";
        }
    }

    // ========== 3. ОТПРАВКА ФОРМЫ КУРСА ==========
    courseForm.addEventListener("submit", async (e) => {
        e.preventDefault();

        // ОШИБКА 1 ИСПРАВЛЕНА: нужно брать .value, а не сам HTML-элемент
        const courseName = document.getElementById("name").value.trim();

        if (courseName === "") {
            alert("Ошибка: Введите название курса");
            return;
        }

        // Используем цикл для обработки 5 блоков — это заменяет ваши 15 переменных
        for (let i = 1; i <= 5; i++) {
            const t = document.getElementById(`t${i}`).value.trim();
            const p = document.getElementById(`p${i}`).value.trim();
            const otv = document.getElementById(`otv${i}`).value.trim();

            await checkAndSend(courseName, t, p, i, otv);
        }
        
        alert("Процесс сохранения завершен!");
        courseForm.reset(); // Очищаем форму после отправки
    });

    // ОШИБКА 2 ИСПРАВЛЕНА: передаем courseName внутрь функции параметром
    async function checkAndSend(courseName, t, p, numb, otv) {
        // Если заполнены все три поля блока — отправляем
        if (t !== "" && p !== "" && otv !== "") {
            try {
                await fetch("https://localhost:7107/addNew_Cours", {
                    method: "POST",
                    headers: {
                        "Content-Type": "application/json",
                    },
                    body: JSON.stringify({
                        name: courseName,
                        t: t,
                        p: p,
                        number: numb,
                        otv: otv
                    }),
                });
                console.log(`Блок ${numb} отправлен успешно.`);
            } catch (error) {
                console.error(`Ошибка при отправке блока ${numb}:`, error);
            }
        } 
        // Если все поля пустые — ничего не делаем (возможно, пользователь заполнил только 2 блока из 5)
        else if (t === "" && p === "" && otv === "") {
            return; 
        } 
        // Если блок заполнен частично
        else {
            console.warn(`Блок ${numb} пропущен: заполнены не все поля (Теория, Практика, Ответ).`);
        }
    }
});