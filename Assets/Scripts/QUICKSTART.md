# ⚡ Быстрый старт Backrooms: Echoes

## 📝 Чек-лист установки (15 минут)

### Phase 1: Player Setup (2 мин)
- [ ] Player объект имеет тег "Player"
- [ ] PlayerController привязан в PlayerLook
- [ ] PlayerLook привязан в PlayerController

### Phase 2: Prefabs Creation (8 мин)

**Комнаты:**
- [ ] Empty room (белый куб)
- [ ] Corridor (серый куб удлиненный)
- [ ] Office room (бежевый куб)
- [ ] Storage room (коричневый куб)
- [ ] Technical room (стальной куб)

**Интерактивные:**
- [ ] Fuse (желтый куб + Fuse.cs)
- [ ] Echo (кубик с Echo.cs + Animator)
- [ ] Lift (лифт с LiftManager.cs + Light + Audio)

### Phase 3: Managers Setup (3 мин)

1. **GameManager GameObject:**
   - [ ] GameManager.cs
   - [ ] UIManager.cs
   - [ ] AnomalySystem.cs
   - [ ] RandomEventTrigger.cs

2. **LevelGenerator GameObject:**
   - [ ] LevelGenerator.cs
   - [ ] RoomLayout.cs (добавится автоматически)

3. **Canvas UI:**
   - [ ] TextMeshPro Fuse Counter
   - [ ] TextMeshPro Message

### Phase 4: References (2 мин)

**LevelGenerator:**
- [ ] Room Prefabs: [Empty, Corridor, Office, Storage, Technical]
- [ ] Fuse Prefab: Fuse
- [ ] Echo Prefab: Echo
- [ ] Lift Prefab: Lift

**GameManager:**
- [ ] Level Generator: LevelGenerator

**UIManager:**
- [ ] Fuse Count Text: [Fuse Counter text]
- [ ] Message Text: [Message text]

## 🎮 Первый запуск

1. Нажми Play
2. LevelGenerator должен создать уровень в консоли:
   ```
   "Starting level generation..."
   "Level generation complete! Generated 50 rooms."
   "Spawned 3 fuses."
   "Spawned 2 echoes."
   ```

3. Проверь:
   - [x] Видишь UI с "FUSES: 0 / 3"
   - [x] Можешь двигаться WASD
   - [x] Камера вращается мышью
   - [x] Вижу комнаты вокруг
   - [x] Можешь спринтить Shift

## 🐛 Если что-то не работает

### Нет комнат
- Проверь: LevelGenerator → Room Prefabs заполнены
- Проверь: Prefabs имеют компонент Room

### Нет предохранителей
- Проверь: LevelGenerator → Fuse Prefab установлен
- Проверь: Fuse.cs имеет ссылку на Player

### Эхо не видно
- Проверь: LevelGenerator → Echo Prefab установлен
- Проверь: Echo имеет Animator (можно пустой)

### Лифт не открывается
- Проверь: LevelGenerator → Lift Prefab установлен
- Проверь: LiftManager имеет ссылку на Light и AudioSources

### UI не обновляется
- Проверь: Canvas имеет TextMeshPro компоненты
- Проверь: UIManager имеет ссылки на текст элементы

## 🎬 Тестирование механик

### Test 1: Предохранители
```
1. Найди желтый куб (Fuse)
2. Подойди ближе < 2 метра
3. Проверь: UI показывает FUSES: 1 / 3
4. Куб изменил цвет на зелёный
```

### Test 2: Эхо механика
```
1. Найди объект Echo
2. Смотри на него прямо
3. Сыграй: Эхо должно стоять неподвижно
4. Отвернись от Эхо
5. Сыграй: Эхо должно начать приближаться или исчезнуть
```

### Test 3: Финальный лифт
```
1. Собери все 3 предохранителя
2. Проверь: Мерцание света
3. Услышь: Звук генератора
4. Услышь: Звонок лифта
5. Сыграй: Лифт открывается (дверь анимируется)
6. Войди в лифт: "ESCAPED!" на экране
```

## 📊 Параметры по умолчанию

**Движение:**
- Walk Speed: 1.2
- Sprint Speed: 2.0
- Gravity: -20

**VHS Эффект:**
- Camera Shake: 0.015
- Movement Bob: 0.05

**Уровень:**
- Room Count: 50
- Spawn Chance: 0.7
- Fuses Required: 3

**Эхо:**
- Observation Distance: 15
- Disappear Distance: 5
- Move Speed: 0.5

## 🎨 Настройка визуала (опционально)

Для усиления PS1 стиля:
- [ ] Настроить камеру на 45° FOV (вместо 60°)
- [ ] Использовать низкополигональные модели для Эхо
- [ ] Добавить фильтр постпроцессинга для VHS
- [ ] Уменьшить разрешение текстур

## 🔊 Добавление звука

**Нужны аудиофайлы:**
- [ ] Generator sound (низкий гул)
- [ ] Lift bell (звонок лифта)
- [ ] Ambient music (атмосферная музыка)
- [ ] Step sounds (шаги)

**Где добавить:**
- LiftManager → AudioSources
- PlayerController → добавить FootstepAudio

## ✨ Улучшения (следующий этап)

- [ ] Создать 10+ уникальных комнат
- [ ] Добавить двери между комнатами
- [ ] PS1 визуал для Эхо
- [ ] Более сложные аномалии
- [ ] Сохранение прогресса
- [ ] Меню начального экрана

