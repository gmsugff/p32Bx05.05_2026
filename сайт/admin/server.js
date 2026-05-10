const express = require('express');
const bodyParser = require('body-parser');
const cors = require('cors');
const fs = require('fs');
const path = require('path'); // 1. Добавили встроенный модуль для работы с путями

const app = express();
const PORT = 3000;

// Middleware
app.use(cors()); 
app.use(bodyParser.json()); 
app.use(bodyParser.urlencoded({ extended: true })); 

// 2. САМОЕ ГЛАВНОЕ: Указываем папку, из которой сервер будет раздавать файлы.
// __dirname означает "папка, в которой лежит этот файл server.js"
// Теперь Express сам найдет index.html, styles.css и main.js и отдаст их браузеру.
app.use(express.static(__dirname)); 

// Обработка POST-запросов на /application (оставляем как было)
app.post('/application', (req, res) => {
    const data = req.body;
    console.log('Получена заявка:', data);
    
    const logLine = JSON.stringify({
        ...data,
        timestamp: new Date().toISOString()
    }) + '\n';
    
    fs.appendFile('applications.log', logLine, (err) => {
        if (err) console.error('Ошибка записи:', err);
    });
    
    res.status(201).json({
        success: true,
        message: 'Заявка успешно отправлена',
        data: data
    });
});


app.listen(PORT, () => {
    console.log(`Сайт доступен по адресу: https://localhost:${PORT}`);
});
