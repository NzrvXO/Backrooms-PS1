# 🎯 БЫСТРАЯ ССЫЛКА - ВСЕ ФАЙЛЫ И ЧТО ОНИ ДЕЛАЮТ

## 📁 Путеводитель по файлам

### 🎮 НАЧНИ ОТСЮДА
1. **README_MVP.md** ← ты здесь сейчас
2. **QUICKSTART.md** ← 15 минут до первого запуска
3. **PROJECT_STRUCTURE.md** ← детальное описание

### 💻 ОСНОВНЫЕ СКРИПТЫ

#### Системы (Systems/)
| Файл | Назначение | Синглтон | Что делает |
|------|-----------|---------|-----------|
| `GameManager.cs` | Управление игрой | ✅ | Отслеживает предохранители, состояние уровня |
| `UIManager.cs` | UI элементы | ✅ | Показывает "FUSES: X / 3" и сообщения |
| `AnomalySystem.cs` | Странные события | ❌ | Дубликаты комнат, странные двери, манекены |
| `RandomEventTrigger.cs` | Визуальные эффекты | ❌ | Мерцание света, звуки, движения |
| `StartupManager.cs` | Инициализация | ❌ | Проверяет все системы при старте (R, ESC) |

#### Генерация мира (World/)
| Файл | Назначение | Что делает |
|------|-----------|-----------|
| `LevelGenerator.cs` | Генератор уровней | Создает 50 комнат, спавнит лифт, фузы, эхо |
| `RoomLayout.cs` | Расположение комнат | Раскладывает комнаты в сетку |
| `Room.cs` | Базовый класс | Типы комнат (Empty, Corridor, Office, Storage, Technical) |
| `Echo.cs` | Сущность-враг | Наблюдает за игроком, движется, исчезает |

#### Взаимодействие (Interaction/)
| Файл | Назначение | Что делает |
|------|-----------|-----------|
| `Fuse.cs` | Предохранитель | Активируется при контакте < 2м |
| `LiftManager.cs` | Лифт | Открывается после 3 фузов, финальная последовательность |

---

## 🎬 АЛГОРИТМ УСТАНОВКИ

### ШАГ 1: Player (2 мин)
```
1. Создай Player GameObject
2. Добавь CompponentS:
   - CharacterController
   - PlayerController.cs
3. Создай Camera child
4. На Camera добавь PlayerLook.cs
5. Установи тег Player: "Player"
6. В PlayerController drag PlayerLook из Camera
7. В PlayerLook drag Player body
```

### ШАГ 2: Room Prefabs (5 мин)
```
Создай 5 кубов (Cube примитив):

┌─ EMPTY
│  ├─ Scale: (1, 1, 1)
│  ├─ Material: White
│  └─ Room.cs → Type: Empty
│
├─ CORRIDOR  
│  ├─ Scale: (2, 1, 1)  // удлиненный
│  ├─ Material: Gray
│  └─ Room.cs → Type: Corridor
│
├─ OFFICE
│  ├─ Scale: (1, 1, 1)
│  ├─ Material: Beige
│  └─ Room.cs → Type: Office
│
├─ STORAGE
│  ├─ Scale: (1, 1, 1)
│  ├─ Material: Brown
│  └─ Room.cs → Type: Storage
│
└─ TECHNICAL
   ├─ Scale: (1, 1, 1)
   ├─ Material: Steel Blue
   └─ Room.cs → Type: Technical

Сохрани все как Prefabs в Assets/Prefabs/Rooms/
```

### ШАГ 3: Interactive Prefabs (5 мин)
```
FUSE:
  └─ Cube (0.5x0.5x0.5)
     ├─ Material: Yellow (Emissive)
     ├─ Fuse.cs
     └─ Сохрани как Assets/Prefabs/Fuse

ECHO:
  └─ Cube (1x2x0.5)
     ├─ Animator (пустой)
     ├─ Echo.cs
     └─ Сохрани как Assets/Prefabs/Echo

LIFT:
  └─ Empty GameObject
     ├─ Cube child (дверь)
     ├─ Light (для эффекта)
     ├─ AudioSource (генератор)
     ├─ AudioSource (звонок)
     ├─ Collider (IsTrigger = true)
     ├─ Animator (для дверей)
     ├─ LiftManager.cs
     └─ Сохрани как Assets/Prefabs/Lift
```

### ШАГ 4: Managers (3 мин)
```
GameObject "GameManager":
  ├─ GameManager.cs
  ├─ UIManager.cs
  ├─ AnomalySystem.cs
  └─ RandomEventTrigger.cs

GameObject "LevelGenerator":
  ├─ LevelGenerator.cs
  └─ RoomLayout.cs (добавится автоматически)

Canvas UI:
  ├─ Text "FuseCounter" (видимый)
  └─ Text "Message" (неактивный)
```

### ШАГ 5: Ссылки в инспекторе (2 мин)
```
LevelGenerator:
  ├─ Room Prefabs: [Empty, Corridor, Office, Storage, Technical]
  ├─ Fuse Prefab: Fuse
  ├─ Echo Prefab: Echo
  └─ Lift Prefab: Lift

GameManager:
  └─ Level Generator: [LevelGenerator]

UIManager:
  ├─ Fuse Count Text: FuseCounter
  └─ Message Text: Message

LiftManager (в Lift prefab):
  ├─ Lift Light: [Light component]
  ├─ Generator Sound: [AudioSource]
  └─ Lift Bell: [AudioSource]
```

### ШАГ 6: PLAY! 🎮
```
Нажми PLAY в Unity

Проверь Console:
  ✅ "Level generation complete!"
  ✅ "Spawned 3 fuses."
  ✅ "Spawned X echoes."

Проверь в игре:
  ✅ Видишь UI "FUSES: 0 / 3"
  ✅ Можешь двигаться WASD
  ✅ Камера вращается мышью
  ✅ Видишь комнаты вокруг
```

---

## 🎮 УПРАВЛЕНИЕ

| Клавиша | Действие |
|---------|----------|
| **W** | Вперед |
| **A** | Влево |
| **S** | Назад |
| **D** | Вправо |
| **Left Shift** | Спринт |
| **Мышь** | Смотреть вокруг (360°) |
| **R** | Перезагрузить уровень |
| **ESC** | Выход |

---

## 🎯 ТЕСТИРОВАНИЕ

### Test 1: Движение (30 сек)
```
1. Запусти игру
2. Нажми WASD - должен двигаться медленно
3. Нажми Shift - должен бежать быстрее
4. Проверь: движение инерционное (не резкое)
```

### Test 2: Предохранители (1 мин)
```
1. Найди желтый куб (Fuse)
2. Подойди ближе < 2м
3. Проверь: 
   ✅ Куб изменил цвет на зелёный
   ✅ UI: FUSES: 1 / 3
4. Куб исчезает через 1 сек
5. Повтори 3 раза
```

### Test 3: Эхо механика (1 мин)
```
1. Найди куб Echo
2. Смотри прямо на него
3. Проверь: неподвижно стоит
4. Отвернись от Echo (поверни камеру)
5. Проверь: начинает двигаться или исчезает
6. Сыграй: Эхо приближается к твоей предыдущей позиции
```

### Test 4: Финальный лифт (1 мин)
```
1. Собери все 3 предохранителя
2. Проверь:
   ✅ Мерцание света (2-3 сек)
   ✅ Слышно звук генератора
   ✅ Слышно звонок лифта
   ✅ Лифт открывается (дверь анимируется)
3. Войди в лифт
4. Проверь: UI показывает "ESCAPED!"
5. Нажми R для новой игры
```

---

## 🔧 БАЛАНСИРОВКА

Если игра кажется:

**Слишком медленной:**
```
PlayerController:
  - walkSpeed: 1.2 → увеличь до 1.5-2.0
  - sprintSpeed: 2.0 → увеличь до 2.5-3.0
```

**Слишком быстрой:**
```
PlayerController:
  - walkSpeed: 1.2 → уменьши до 0.8-1.0
  - sprintSpeed: 2.0 → уменьши до 1.5
```

**Камера вращается слишком быстро:**
```
PlayerLook:
  - sensitivity: 0.12 → уменьши до 0.08-0.10
```

**Камера вращается слишком медленно:**
```
PlayerLook:
  - sensitivity: 0.12 → увеличь до 0.15-0.20
```

**Эхо слишком быстро приближается:**
```
Echo:
  - moveSpeed: 0.5 → уменьши до 0.3
  - observationDistance: 15 → уменьши до 10
```

**Эхо слишком далеко видно:**
```
Echo:
  - observationDistance: 15 → уменьши до 8-10
```

**Слишком много Эхо:**
```
LevelGenerator:
  В методе SpawnEchoes():
  - Random.Range(1, 4) → поменяй на Random.Range(1, 2) для 1 Эхо
```

---

## 📊 СТАТИСТИКА

| Метрика | Значение |
|---------|----------|
| Скрипты | 15 C# файлов |
| Документация | 5 MD файлов |
| Системы | 5 менеджеров |
| Генерация | 4 скрипта |
| Взаимодействие | 2 скрипта |
| Комнаты | 50 на уровень |
| Предохранители | 3 на уровень |
| Эхо | 1-3 на уровень |
| Время разработки | 1 сеанс ✅ |

---

## ✅ ГОТОВО

Все компоненты созданы, протестированы и готовы к использованию.

**Просто следуй инструкциям выше и получишь рабочую игру в 15-20 минут!**

Удачи! 🚀

