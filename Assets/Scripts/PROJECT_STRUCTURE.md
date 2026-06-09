# Backrooms: Echoes - Полная структура проекта

## 📋 Готовые компоненты

### Core Systems (Systems/)

#### GameManager.cs
```
Назначение: Управление состоянием игры
Синглтон: Да
Ключевые методы:
  - GenerateLevel() - инициализирует уровень
  - ActivateFuse() - активирует предохранитель
  - AreAllFusesActivated() - проверка состояния
  - CompleteLevel() - завершение уровня
```

#### UIManager.cs
```
Назначение: Управление UI элементами
Синглтон: Да
Отображает: FUSES: X / 3
```

#### AnomalySystem.cs
```
Назначение: Случайные события
Частота: проверка каждые 10 сек
События:
  - Дубликат комнаты
  - Неправильная дверь
  - Манекен
```

#### RandomEventTrigger.cs
```
Назначение: Более сложные события с эффектами
Готовые события:
  - Мерцание света
  - Искаженные звуки
  - Странные движения
```

### World Generation (World/)

#### LevelGenerator.cs
```
Назначение: Процедурная генерация уровней
Функции:
  - Генерирует N комнат из префабов
  - Расположение через RoomLayout
  - Спавн лифта, предохранителей, Эхо
Параметры:
  - roomCount: 50 (кол-во комнат)
  - spawnChance: 0.7 (вероятность появления)
```

#### RoomLayout.cs
```
Назначение: Правильное расположение комнат в сетку
Функции:
  - BuildLayout(rooms) - построить сетку
  - GetStartRoom() - стартовая комната
  - GetEndRoom() - комната с лифтом
  - Визуализация через гизмо
```

#### Room.cs
```
Назначение: Базовый класс комнаты
Типы:
  - Empty (пустая)
  - Corridor (коридор)
  - Office (офис)
  - Storage (склад)
  - Technical (техническое помещение)
Функции:
  - TrySpawnFuse() - попытка спавна предохранителя
  - PlaceObject() - размещение объектов
```

#### Echo.cs
```
Назначение: Сущность с механикой наблюдения
Состояния:
  - Idle: спокойно стоит
  - Observed: смотрят на Эхо (неподвижна)
  - Moving: движется к игроку
  - Disappearing: исчезает
Механика:
  - IsPlayerLooking() - проверка взгляда
  - MoveTowardPlayer() - приближение
  - SetRandomIdlePose() - случайные позы
```

### Interaction (Interaction/)

#### Fuse.cs
```
Назначение: Предохранитель, активируется при контакте
Активирует при расстоянии < activationDistance
Эффекты: зелёный цвет + частицы
Результат: GameManager.ActivateFuse()
```

#### LiftManager.cs
```
Назначение: Управление лифтом и финальной последовательностью
Синглтон: Да
При активации всех предохранителей:
  1. Гасит свет на 2-3 сек
  2. Звук генератора
  3. Звонок лифта
  4. Открытие дверей
Условие выхода: игрок в триггере лифта
```

## 🎮 Как собрать в Unity

### Структура сцены

```
Scene
├── GameManager (GameObject)
│   ├── GameManager.cs
│   ├── UIManager.cs
│   ├── AnomalySystem.cs
│   └── RandomEventTrigger.cs
├── LevelGenerator (GameObject)
│   ├── LevelGenerator.cs
│   └── RoomLayout.cs
├── Player (GameObject)
│   ├── PlayerController.cs
│   ├── PlayerLook.cs
│   ├── CharacterController
│   └── [Camera]
├── Canvas (UI)
│   ├── Text - Fuse Counter
│   └── Text - Message Display
└── [Сгенерированные комнаты]
```

### Шаг 1: Подготовка Player

1. Создай объект Player с:
   - CharacterController
   - PlayerController.cs
   - PlayerLook.cs на камере
   - Тег "Player"

2. В PlayerController установи:
   - PlayerLook reference = Camera

3. В PlayerLook установи:
   - PlayerBody reference = Player body

### Шаг 2: Создание комнат

Создай 5 типов комнат (кубы 10x3x10):

**Empty** (белый)
```
Mesh: Cube
Color: White
Room.Type: Empty
```

**Corridor** (серый)
```
Mesh: Cube (elongated 20x3x10)
Color: Gray
Room.Type: Corridor
```

**Office** (бежевый)
```
Mesh: Cube
Color: Beige
Room.Type: Office
```

**Storage** (коричневый)
```
Mesh: Cube
Color: Brown
Room.Type: Storage
```

**Technical** (стальной)
```
Mesh: Cube
Color: Steel Blue
Room.Type: Technical
```

Сохрани как префабы в `Assets/Prefabs/Rooms/`

### Шаг 3: Создание Fuse

```
GameObject: Cube (0.5x0.5x0.5)
Color: Yellow
Material: Emissive
Fuse.cs (component)
  - Activation Distance: 2
Сохрани как: Assets/Prefabs/Fuse
```

### Шаг 4: Создание Echo

```
GameObject: Cube (1x2x0.5) или модель
Animator: (можно пустой для начала)
Echo.cs (component)
  - Observation Distance: 15
  - Disappear Distance: 5
  - Move Speed: 0.5
Сохрани как: Assets/Prefabs/Echo
```

### Шаг 5: Создание Lift

```
GameObject: Empty
├── Mesh: Cube (дверь)
│   └── Animator для анимации дверей
├── Light (для эффекта)
├── AudioSource (генератор)
├── AudioSource (звонок)
├── Collider (IsTrigger)
└── LiftManager.cs (component)
    - Blackout Duration: 3
    - Door Open Distance: 2

Сохрани как: Assets/Prefabs/Lift
```

### Шаг 6: Настройка GameManager

1. Создай пустой объект "GameManager"
2. Добавь компоненты:
   - GameManager.cs
   - UIManager.cs
   - AnomalySystem.cs
   - RandomEventTrigger.cs

3. В GameManager:
   - Fuses Required: 3

### Шаг 7: Настройка LevelGenerator

1. Создай пустой объект "LevelGenerator" на сцене
2. Добавь компоненты:
   - LevelGenerator.cs
   - RoomLayout.cs (добавится автоматически)

3. В LevelGenerator:
   - Room Prefabs: выбери 5 комнат
   - Room Count: 50
   - Spawn Chance: 0.7
   - Fuse Prefab: Fuse
   - Echo Prefab: Echo
   - Lift Prefab: Lift

4. В GameManager:
   - Level Generator: перетащи LevelGenerator

### Шаг 8: Создание UI

1. Создай Canvas
2. Добавь TextMeshPro текст:
   - "Fuse Counter" (видимый)
   - "Message" (неактивный)

3. В UIManager:
   - Fuse Count Text: Fuse Counter
   - Message Text: Message

## 🎯 Игровой цикл

```
1. Запуск сцены
   ↓
2. GameManager инициализирует LevelGenerator
   ↓
3. LevelGenerator генерирует уровень:
   - 50 комнат в сетку
   - Лифт в конце
   - 3 предохранителя в случайных местах
   - 1-3 Эхо в случайных комнатах
   ↓
4. Игрок исследует уровень
   - Может видеть Эхо (они наблюдают за ним)
   - Ищет предохранители
   ↓
5. При подходе к предохранителю:
   - Автоматическая активация
   - UI обновляется: FUSES: X / 3
   ↓
6. После 3-го предохранителя:
   - Мерцание света
   - Звук генератора + звонок
   - Лифт открывается
   ↓
7. Игрок входит в лифт
   - Уровень завершен!
```

## 🔧 Параметры для балансировки

**Movement:**
- walkSpeed: 1.2 → медленнее для атмосферы
- sprintSpeed: 2.0 → спринт немного быстрее

**Echo:**
- observationDistance: 15 → дальность видения игроком
- stareThreshold: 0.8 → как точно нужно смотреть

**Level:**
- roomCount: 50 → больше = дольше игра
- spawnChance: 0.7 → меньше = разреженнее уровень

**VHS Effect:**
- cameraShakeAmount: 0.015 → меньше = менее заметно
- movementBob: 0.05 → амплитуда покачивания

## ✅ Что работает прямо сейчас

- [x] FPS контроллер с WASD + Left Shift
- [x] VHS эффекты на камере (tremor, bob)
- [x] Процедурная генерация комнат
- [x] Система предохранителей
- [x] Механика наблюдения за Эхо
- [x] Лифт с финальной последовательностью
- [x] UI счетчик предохранителей
- [x] Случайные события

## 🚀 Следующие шаги

- [ ] Создать 10+ уникальных типов комнат
- [ ] Улучшить подключение комнат (открываемые двери)
- [ ] PS1 визуал для Эхо (низкий полигон)
- [ ] Более сложная механика аномалий
- [ ] Аудиодизайн (атмосферная музыка, звуки)
- [ ] Улучшенная анимация для Эхо
- [ ] Система сохранений и перезапуска

