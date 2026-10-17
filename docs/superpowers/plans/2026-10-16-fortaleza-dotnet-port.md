# Fortaleza .NET 10 Port Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Port the 1990s Turbo Pascal text adventure game "La Fortaleza" to a modern .NET 10 C# console application with bilingual (Spanish/English) support.

**Architecture:** The game engine is a class hierarchy mirroring the original Pascal OOP design, translated to idiomatic C# with interfaces and records. All game content (room descriptions, item text, NPC dialogue) is represented as `LocalizedText` structs holding both Spanish and English strings. Commands are parsed bilingually — the same verb set works in both ES and EN regardless of the display language, which is chosen at startup.

**Tech Stack:** .NET 10, C# 13, xUnit 2.x, no external game libraries.

**Spec:** `C:\Projects\Fortaleza\original\` (original Pascal source) + `original\README.md`

## Global Constraints

- Target framework: `net10.0`
- C# language version: latest (13)
- All identifier names (classes, methods, properties, fields) must be in English
- Game narrative content (room names/descriptions, item text, NPC dialogue) must appear in both Spanish and English
- Commands accepted in both Spanish and English simultaneously, regardless of display language
- Preserve original game logic exactly: same rooms, items, NPCs, win conditions, puzzles
- Two independent games: Fort1 (33+ rooms, Beast goal) and Fort2 (55 rooms, Sorcerer goal)
- Player name: "Indy" (both games)
- Weight system: `LWeight = 40` bag-units capacity
- No external NuGet dependencies except xUnit for tests
- Track/save system: plain-text command log files (`.trk`)

## Review Focus

- **Riddle link case-insensitivity:** `RiddleLink` answers must match ignoring case and Spanish diacritics (`treinta y nueve` == `Treinta Y Nueve` == `treinta y nueve`); test this in Task 5.
- **DangerLink2 item-must-be-absent logic:** `DangerLink2` kills the player when they carry the named item, the inverse of `DangerLink`; a test must pass with item carried and pass without it in Task 5.
- **Weight-limit boundary:** taking an item whose weight exactly equals remaining capacity must succeed; one unit over must fail; test in Task 3.
- **Hidden-item replacement:** when a `Hidden` container is broken, it is removed from the room and its hidden item is inserted at the same index; test the index preservation in Task 4.
- **Command parser strip-words:** `EL`, `LA`, `POR`, `AL` (ES) and `THE`, `A`, `AN` (EN) must be stripped before verb lookup so `ir por la puerta` and `go through the door` both resolve identically; test in Task 7.

---

## File Map

```
C:\Projects\Fortaleza\
├── Fortaleza.sln
└── src\
    └── Fortaleza\
        ├── Fortaleza.csproj
        ├── Program.cs
        ├── Engine\
        │   ├── Thing.cs            - Abstract base: name, description, weight, children list
        │   ├── Item.cs             - Concrete leaf thing (non-container, non-NPC)
        │   ├── LivingThing.cs      - Abstract NPC base: Dead flag, Die/Speak/Accept
        │   ├── Room.cs             - Container room: Get/Take/Open/Break, visited flag
        │   ├── Inventory.cs        - Suitcase with weight limit; Describe lists weights
        │   ├── Player.cs           - All player verbs: Look/Take/Leave/Kill/Give/Open/Walk/Ask/Break/Weigh
        │   ├── Troll.cs            - Troll: wants a gift, reveals hint when happy
        │   ├── Guard.cs            - Guard: must be killed with exact weapon
        │   ├── Hidden.cs           - Container hiding a child thing; broken with exact item
        │   ├── Links\
        │   │   ├── Link.cs         - Base door: key string, open/closed, destination
        │   │   ├── OpenLink.cs     - Always open door
        │   │   ├── DangerLink.cs   - Requires player carries talisman item
        │   │   ├── DangerLink2.cs  - Requires player does NOT carry talisman item
        │   │   └── RiddleLink.cs   - Requires correct riddle answer (case+diacritic insensitive)
        │   └── GameEngine.cs       - Command dispatch, AskMan/MakeManWish loop, Save/Load track
        ├── Localization\
        │   ├── LocalizedText.cs    - Record struct { string Es; string En; } + Get() helper
        │   ├── Localizer.cs        - Static Language enum + Current; loads JSON messages
        │   └── res\
        │       ├── messages.es.json - All engine response strings in Spanish
        │       └── messages.en.json - All engine response strings in English
        ├── Parser\
        │   └── CommandParser.cs    - Tokenize input, strip articles, map verbs→CommandType enum
        └── Games\
            ├── GameKeys.cs         - Decoded puzzle keys as string constants (passwords, weapons, likenesses)
            ├── Fort1Builder.cs     - Creates all 50 rooms and their contents for Fort1
            └── Fort2Builder.cs     - Creates all 55 rooms and their contents for Fort2
└── tests\
    └── Fortaleza.Tests\
        ├── Fortaleza.Tests.csproj
        ├── Engine\
        │   ├── ThingTests.cs
        │   ├── RoomTests.cs
        │   ├── InventoryTests.cs
        │   ├── LinkTests.cs
        │   ├── HiddenTests.cs
        │   └── PlayerTests.cs
        └── Parser\
            └── CommandParserTests.cs
```

---

### Task 1: Solution Scaffold + Localization Foundation

**Files:**
- Create: `Fortaleza.sln`
- Create: `src\Fortaleza\Fortaleza.csproj`
- Create: `tests\Fortaleza.Tests\Fortaleza.Tests.csproj`
- Create: `src\Fortaleza\Localization\LocalizedText.cs`
- Create: `src\Fortaleza\Localization\Localizer.cs`
- Create: `src\Fortaleza\Localization\res\messages.es.json`
- Create: `src\Fortaleza\Localization\res\messages.en.json`
- Test: `tests\Fortaleza.Tests\LocalizationTests.cs`

**Interfaces:**
- Produces:
  - `LocalizedText` — `record struct LocalizedText(string Es, string En) { string Get(); }`
  - `Language` — `enum Language { Spanish, English }`
  - `Localizer.Current` — `static Language Current { get; set; }`
  - `Localizer.Msg(string key)` — `static string Msg(string key)` returns the message for the current language
  - All message keys listed in messages JSON (see step 3)

- [ ] **Step 1: Create solution and project files**

```xml
<!-- Fortaleza.csproj -->
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net10.0</TargetFramework>
    <LangVersion>latest</LangVersion>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>
</Project>
```

```xml
<!-- Fortaleza.Tests.csproj -->
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <LangVersion>latest</LangVersion>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <IsPackable>false</IsPackable>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="xunit" Version="2.*" />
    <PackageReference Include="xunit.runner.visualstudio" Version="2.*" />
    <ProjectReference Include="..\..\src\Fortaleza\Fortaleza.csproj" />
  </ItemGroup>
</Project>
```

Run: `dotnet build Fortaleza.sln`
Expected: Build succeeded.

- [ ] **Step 2: Write failing tests for LocalizedText**

```csharp
// LocalizationTests.cs
[Fact] void LocalizedText_Get_ReturnsSpanishWhenCurrentIsSpanish()
{
    Localizer.Current = Language.Spanish;
    var t = new LocalizedText("Hola", "Hello");
    Assert.Equal("Hola", t.Get());
}
[Fact] void LocalizedText_Get_ReturnsEnglishWhenCurrentIsEnglish()
{
    Localizer.Current = Language.English;
    var t = new LocalizedText("Hola", "Hello");
    Assert.Equal("Hello", t.Get());
}
[Fact] void Localizer_Msg_ReturnsCorrectLanguageString()
{
    Localizer.Current = Language.Spanish;
    Assert.Equal("Ok.", Localizer.Msg("Ok"));
    Localizer.Current = Language.English;
    Assert.Equal("Ok.", Localizer.Msg("Ok"));  // same in both languages
}
```

- [ ] **Step 3: Create `LocalizedText.cs` and `Localizer.cs`**

`LocalizedText`: `record struct` with `Es`, `En` string properties and `Get()` returning `Localizer.Current == Language.Spanish ? Es : En`.

`Localizer`:
- `public static Language Current { get; set; } = Language.Spanish;`
- Loads `messages.es.json` and `messages.en.json` from embedded resources on first access
- `public static string Msg(string key)` looks up key in current language dictionary

- [ ] **Step 4: Create `messages.es.json`**

```json
{
  "Ok": "Ok.",
  "Surprise": "¡Usted ha descubierto algo!",
  "RoomContains": "Usted ve:",
  "YouAreIn": "Usted se encuentra en ",
  "BagContains": "Sus pertenecias son:",
  "TryOpenFail": "Usted trata de abrir, pero no puede lograrlo.",
  "AlreadyOpen": "No es necesario.",
  "DoorWarning": "Tenga cuidado. Por aquí se puede entrar, pero no se puede salir.",
  "WrongPassword": " no es la contraseña.",
  "NeedPassword": "Necesita unas palabras mágicas.",
  "CantDo": "No puede hacer eso.",
  "CantKillInanimate": "No puede matar a algo que nunca tuvo vida.",
  "ThereIsNo": "No existe \"",
  "InTheRoom": "\" en este lugar.",
  "NotInBag": " no está dentro de sus pertenencias.",
  "AlsoNotInBag": " Entre sus pertenencias tampoco.",
  "WhosGift": "¿Y para quién es? ¿O es un obsequio suyo para sí mismo?",
  "WhatSee": "¿Ver qué?",
  "NotHere": " no está aquí.",
  "TooHeavy": "Sería demasiado peso.",
  "CantCarry": "Usted no puede cargar con eso.",
  "BagEmpty": "Usted no lleva nada consigo.",
  "Dying": "Lamento informarle que usted está muerto.",
  "MagicWords": "Usted se arrodilla, levanta los brazos y dice:",
  "Ouch": "OUCH! Usted se ha golpeado las narices.",
  "TryOpenFirst": "Trate de abrir antes.",
  "CantOpenThis": "Imposible de abrir.",
  "VeryThanks": "Muchas gracias. Hace mucho que esperaba por esto.",
  "CoolThanks": "¡Qué amable! Gracias...",
  "GiftRejected": "Su regalo no es aceptado.",
  "TalkingAlone": "¿Hablando solo?",
  "Silence": "No desea hablar con usted.",
  "NoProtection": "No debió cruzar desprotegido.",
  "UndesirableObject": "Usted llevaba un objeto que le impidió cruzar.",
  "CantBreak": "Usted trata, pero no lo consigue.",
  "NotBreakable": "Usted no puede romper eso.",
  "Ahhhh": "AAAAARRRGGGGG!!!",
  "Crash": "CRASSSHHH!!!",
  "Useless": "Todos sus esfuerzos son en vano. Probablemente no está usando el objeto indicado.",
  "SolveRiddle": "Debe resolver este acertijo:",
  "SolveRiddleHint": "Trate de esta forma: \"Abrir tal cosa con tal respuesta\".",
  "RiddleSolved": "Usted ha descifrado el acertijo correctamente.",
  "WrongAnswer": "Esa no es la respuesta.",
  "WhatToDo": "¿Qué desea hacer?",
  "DontUnderstand": "No le comprendo.",
  "EnterFileName": "¿Nombre del fichero rastro?",
  "WannaStop": "¿Desea interrumpir la ejecución (S/N)?",
  "SaveError": "Error salvando fichero rastro.",
  "FileNotFound": "Fichero rastro no encontrado.",
  "NothingToSave": "No hay nada que salvar.",
  "TakeOneAtATime": "Mejor tome las cosas una a una.",
  "DropAll": "Usted deja todas sus pertencias en el suelo.",
  "Complete": "Complete la oración. No sea ambiguo.",
  "CannotCarryMore": "Usted no puede ni con un átomo más.",
  "AlmostFull": "Usted está a punto de agotar sus capacidades.",
  "CouldCarryMore": "Usted podría cargar ",
  "MoreBags": " bolsas más.",
  "BagUnit": " bolsa.",
  "BagUnits": " bolsas.",
  "Initializing": "Inicializando la fortaleza...",
  "SaveTrackTo": "Salvando rastro hacia ",
  "SaveTrack": "¿Desea salvar su rastro (S/N)?",
  "GoodMorning": "Buenos días.",
  "GoodAfternoon": "Buenas tardes.",
  "GoodEvening": "Buenas noches.",
  "Victory1": "Usted ha vencido a la Bestia.",
  "Victory2": "Parece ser una persona persistente y eso es un mérito muy grande.",
  "Victory3": "La persistencia es indispensable para los que luchan por el bien, sobre todo porque los que luchan por el mal son muy persistentes también.",
  "Victory4": "Veremos si en la próxima versión de La Fortaleza tiene igual suerte.",
  "Author": "                                             Un servidor:\n                                             M. Cepero",
  "Defeat": "Su entierro se efectuará el próximo domingo a las 3:00 am. Está invitado.",
  "PercentVisited": "% de la Fortaleza visitado.",
  "YouWeigh": "Usted coloca el objeto en la balanza. Pesa "
}
```

- [ ] **Step 5: Create `messages.en.json`** with English equivalents for every key in step 4

```json
{
  "Ok": "Ok.",
  "Surprise": "You discovered something!",
  "RoomContains": "You see:",
  "YouAreIn": "You are in ",
  "BagContains": "Your belongings are:",
  "TryOpenFail": "You try to open it, but you cannot.",
  "AlreadyOpen": "Not necessary.",
  "DoorWarning": "Be careful. You can enter here but cannot exit.",
  "WrongPassword": " is not the password.",
  "NeedPassword": "You need magic words.",
  "CantDo": "You cannot do that.",
  "CantKillInanimate": "You cannot kill something that never had life.",
  "ThereIsNo": "There is no \"",
  "InTheRoom": "\" in this room.",
  "NotInBag": " is not in your belongings.",
  "AlsoNotInBag": " Not among your belongings either.",
  "WhosGift": "And for whom? Or is this a gift to yourself?",
  "WhatSee": "See what?",
  "NotHere": " is not here.",
  "TooHeavy": "That would be too heavy.",
  "CantCarry": "You cannot carry that.",
  "BagEmpty": "You are carrying nothing.",
  "Dying": "I regret to inform you that you are dead.",
  "MagicWords": "You kneel, raise your arms and say:",
  "Ouch": "OUCH! You have bumped your nose.",
  "TryOpenFirst": "Try opening it first.",
  "CantOpenThis": "Impossible to open.",
  "VeryThanks": "Thank you very much. I have been waiting for this.",
  "CoolThanks": "How kind! Thank you...",
  "GiftRejected": "Your gift is not accepted.",
  "TalkingAlone": "Talking to yourself?",
  "Silence": "It does not wish to speak with you.",
  "NoProtection": "You should not have crossed unprotected.",
  "UndesirableObject": "You were carrying an item that prevented you from crossing.",
  "CantBreak": "You try, but cannot manage it.",
  "NotBreakable": "You cannot break that.",
  "Ahhhh": "AAAAARRRGGGGG!!!",
  "Crash": "CRASSSHHH!!!",
  "Useless": "All your efforts are in vain. You are probably not using the right object.",
  "SolveRiddle": "You must solve this riddle:",
  "SolveRiddleHint": "Try this form: \"Open [thing] with [answer]\".",
  "RiddleSolved": "You have correctly solved the riddle.",
  "WrongAnswer": "That is not the answer.",
  "WhatToDo": "What do you want to do?",
  "DontUnderstand": "I do not understand you.",
  "EnterFileName": "Track file name?",
  "WannaStop": "Do you want to stop execution (Y/N)?",
  "SaveError": "Error saving track file.",
  "FileNotFound": "Track file not found.",
  "NothingToSave": "Nothing to save.",
  "TakeOneAtATime": "Better take things one at a time.",
  "DropAll": "You leave all your belongings on the floor.",
  "Complete": "Complete the sentence. Do not be ambiguous.",
  "CannotCarryMore": "You cannot carry one atom more.",
  "AlmostFull": "You are about to exhaust your carrying capacity.",
  "CouldCarryMore": "You could carry ",
  "MoreBags": " more bags.",
  "BagUnit": " bag.",
  "BagUnits": " bags.",
  "Initializing": "Initializing the fortress...",
  "SaveTrackTo": "Saving track to ",
  "SaveTrack": "Do you want to save your track (Y/N)?",
  "GoodMorning": "Good morning.",
  "GoodAfternoon": "Good afternoon.",
  "GoodEvening": "Good evening.",
  "Victory1": "You have defeated the Beast.",
  "Victory2": "You seem to be a persistent person and that is a great merit.",
  "Victory3": "Persistence is indispensable for those who fight for good, especially because those who fight for evil are also very persistent.",
  "Victory4": "We will see if in the next version of La Fortaleza you have equal luck.",
  "Author": "                                             Yours truly:\n                                             M. Cepero",
  "Defeat": "Your funeral will be held next Sunday at 3:00 am. You are invited.",
  "PercentVisited": "% of the Fortress visited.",
  "YouWeigh": "You place the object on the scale. It weighs "
}
```

- [ ] **Step 6: Run tests**

Run: `dotnet test tests\Fortaleza.Tests\`
Expected: 3 tests pass.

- [ ] **Step 7: Commit**

```
git add src\ tests\ Fortaleza.sln
git commit -m "feat: scaffold .NET 10 solution with localization foundation"
```

---

### Task 2: Core Thing Hierarchy

**Files:**
- Create: `src\Fortaleza\Engine\Thing.cs`
- Create: `src\Fortaleza\Engine\Item.cs`
- Create: `src\Fortaleza\Engine\LivingThing.cs`
- Test: `tests\Fortaleza.Tests\Engine\ThingTests.cs`

**Interfaces:**
- Consumes: `LocalizedText`, `Localizer.Msg`
- Produces:
  - `abstract class Thing` — `LocalizedText Name`, `LocalizedText Description`, `int Weight`, `List<Thing> Children`, `virtual string Image()`, `void Output(string s)`, `static bool Visible`
  - `class Item : Thing` — concrete leaf thing
  - `abstract class LivingThing : Thing` — `bool Dead`, `virtual bool Die(string weapon)`, `virtual void Speak()`, `virtual void Accept(Item gift)`

- [ ] **Step 1: Write failing tests**

```csharp
[Fact] void Thing_Name_ReturnsLocalizedName()
{
    Localizer.Current = Language.English;
    var item = new Item(new("Maza", "Mace"), new("Es muy pesada.", "It is very heavy."), 39);
    Assert.Equal("Mace", item.Name.Get());
}
[Fact] void Item_Weight_StoresCorrectly()
{
    var item = new Item(new("Maza","Mace"), new("desc","desc"), 39);
    Assert.Equal(39, item.Weight);
}
[Fact] void LivingThing_Die_SetsDead()
{
    var npc = new TestLivingThing();
    npc.Die("");
    Assert.True(npc.Dead);
}
```

- [ ] **Step 2: Implement `Thing.cs`**

Abstract class with:
- `LocalizedText Name`, `LocalizedText Description`, `int Weight`
- `List<Thing> Children { get; } = new()`
- `static bool Visible { get; set; } = true`
- `virtual string Image() => Description.Get()`
- `void Output(string s)` — word-wraps at 77 chars, splits on `\n`, writes to `Console`; respects `Visible`

- [ ] **Step 3: Implement `Item.cs`**

Concrete sealed class inheriting `Thing`. Constructor: `Item(LocalizedText name, LocalizedText description, int weight)`.

- [ ] **Step 4: Implement `LivingThing.cs`**

Abstract class inheriting `Thing`:
- `bool Dead { get; protected set; }`
- `virtual bool Die(string weapon) { Dead = true; return true; }`
- `virtual void Speak() => Output(Localizer.Msg("Silence"))`
- `virtual void Accept(Item gift) { }` — no-op base

- [ ] **Step 5: Run tests**

Run: `dotnet test`
Expected: all pass.

- [ ] **Step 6: Commit**

```
git commit -m "feat: add core Thing/Item/LivingThing hierarchy"
```

---

### Task 3: Room and Inventory

**Files:**
- Create: `src\Fortaleza\Engine\Room.cs`
- Create: `src\Fortaleza\Engine\Inventory.cs`
- Test: `tests\Fortaleza.Tests\Engine\RoomTests.cs`
- Test: `tests\Fortaleza.Tests\Engine\InventoryTests.cs`

**Interfaces:**
- Consumes: `Thing`, `Item`, `Localizer.Msg`
- Produces:
  - `class Room : Thing` — `bool Visited`, `Thing? Get(string name)`, `Thing? Take(string name)`, `void OpenDoor(string name, string key)`, `void BreakThing(string name, string weapon)`, `void Describe()`
  - `class Inventory : Room` — `bool IsEmpty`, `int Heaviness`, `bool CanAdd(int weight)`, `void Describe()` (with weights)
  - `const int LWeight = 40`

- [ ] **Step 1: Write failing tests**

```csharp
[Fact] void Room_Get_FindsItemByNormalizedName()
{
    var room = new Room(new("pasillo","corridor"), new("",""), LWeight+1);
    room.Children.Add(new Item(new("Antorcha","Torch"), new("",""), 1));
    Assert.NotNull(room.Get("antorcha"));
    Assert.NotNull(room.Get("ANTORCHA"));
}
[Fact] void Room_Get_MatchesWithoutDiacritics()
{
    var room = MakeRoom();
    room.Children.Add(new Item(new("Trébol","Clover"), new("",""), 1));
    Assert.NotNull(room.Get("Trebol"));  // no accent
}
[Fact] void Room_Take_RemovesItemFromRoom()
{
    var room = MakeRoom();
    var item = new Item(new("Maza","Mace"), new("",""), 1);
    room.Children.Add(item);
    var taken = room.Take("Maza");
    Assert.Same(item, taken);
    Assert.Empty(room.Children);
}
[Fact] void Inventory_Heaviness_SumsWeights()
{
    var inv = new Inventory();
    inv.Children.Add(new Item(new("A","A"), new("",""), 10));
    inv.Children.Add(new Item(new("B","B"), new("",""), 15));
    Assert.Equal(25, inv.Heaviness);
}
[Fact] void Inventory_CanAdd_FalseWhenOverLimit()
{
    var inv = new Inventory();
    inv.Children.Add(new Item(new("A","A"), new("",""), 35));
    Assert.False(inv.CanAdd(6));   // 35+6=41 > 40
}
[Fact] void Inventory_CanAdd_TrueAtExactLimit()
{
    var inv = new Inventory();
    inv.Children.Add(new Item(new("A","A"), new("",""), 35));
    Assert.True(inv.CanAdd(5));    // 35+5=40 == 40
}
```

- [ ] **Step 2: Implement `Room.cs`**

Class inheriting `Thing`:
- `bool Visited { get; set; }`
- `Thing? Get(string name)` — normalizes name (strip diacritics, uppercase), tries exact match then `Equals`-style partial match via `StringHelper.Normalize`
- `Thing? Take(string name)` — calls `Get`, removes from `Children`, returns it
- `void OpenDoor(string name, string key)` — finds a `Link` child by name, calls `link.TryOpen(key)`, outputs appropriate messages
- `void BreakThing(string name, string weapon)` — finds a `Hidden` child, calls `hidden.TryBreak(weapon)`, swaps in hidden item on success
- `void Describe()` — outputs each child's `Name.Get()`

Add static helper `StringHelper.Normalize(string s)` in `Engine\StringHelper.cs` — strips Spanish diacritics (`áéíóúüñÁÉÍÓÚÜÑ` → `aeiouunAEIOUUN`), uppercases.

- [ ] **Step 3: Implement `Inventory.cs`**

Class inheriting `Room` (following original Pascal design where Suitcase extends Room):
- `public const int LWeight = 40`
- `bool IsEmpty => Children.Count == 0`
- `int Heaviness => Children.Sum(t => t.Weight)`
- `bool CanAdd(int weight) => Heaviness + weight <= LWeight`
- `override void Describe()` — lists each item with weight, then prints capacity status message

- [ ] **Step 4: Run tests**

Run: `dotnet test`
Expected: all pass.

- [ ] **Step 5: Commit**

```
git commit -m "feat: add Room and Inventory with weight system"
```

---

### Task 4: NPC Types — Troll, Guard, Hidden

**Files:**
- Create: `src\Fortaleza\Engine\Troll.cs`
- Create: `src\Fortaleza\Engine\Guard.cs`
- Create: `src\Fortaleza\Engine\Hidden.cs`
- Test: `tests\Fortaleza.Tests\Engine\HiddenTests.cs`

**Interfaces:**
- Consumes: `LivingThing`, `Item`, `Localizer.Msg`
- Produces:
  - `class Troll : LivingThing` — `string Likeness`, `LocalizedText HiData`, `LocalizedText LoData`; `override void Speak()` outputs `LoData` when not happy, `HiData` when happy; `override void Accept(Item gift)` — sets happy if `gift.Name` matches `Likeness` (normalized)
  - `class Guard : LivingThing` — `LocalizedText Confession`, `string LethalWeapon`; `override bool Die(string weapon)` — returns true + outputs confession only if `weapon` matches `LethalWeapon` (normalized), else outputs `Msg("Useless")` + returns false
  - `class Hidden : Thing` — `string BreakerName`, `Thing HiddenChild`; `bool TryBreak(string weaponName)` returns true if normalized weapon matches `BreakerName`; `Thing Release()` returns `HiddenChild`
  - `class Daughter : Guard` (Fort2 special) — `override bool Die(string weapon)` always returns true (any weapon kills her)

- [ ] **Step 1: Write failing tests**

```csharp
[Fact] void Troll_Accept_SetsHappyOnCorrectGift()
{
    var troll = new Troll(new("Bruja","Witch"), new("",""), "Escoba",
        new("Gracias","Thank you"), new("Dame mi escoba","Give me my broom"));
    troll.Accept(new Item(new("Escoba","Broom"), new("",""), 1));
    Assert.True(troll.Happy);
}
[Fact] void Troll_Accept_IgnoresWrongGift()
{
    var troll = MakeTroll("Escoba");
    troll.Accept(new Item(new("Maza","Mace"), new("",""), 1));
    Assert.False(troll.Happy);
}
[Fact] void Guard_Die_ReturnsTrueWithCorrectWeapon()
{
    var guard = new Guard(new("Ciclope","Cyclops"), new("",""),
        new("Dying words","Dying words"), "Maza");
    Assert.True(guard.Die("Maza"));
}
[Fact] void Guard_Die_ReturnsFalseWithWrongWeapon()
{
    var guard = new Guard(new("Ciclope","Cyclops"), new("",""),
        new("Dying","Dying"), "Maza");
    Assert.False(guard.Die("Espada"));
}
[Fact] void Hidden_TryBreak_ReturnsTrueOnMatch()
{
    var child = new Item(new("Puerta oculta","Hidden door"), new("",""), 100);
    var hidden = new Hidden(new("Espejo opaco","Opaque mirror"), new("",""), "Hueso de gato", child);
    Assert.True(hidden.TryBreak("Hueso de gato"));
}
[Fact] void Hidden_TryBreak_ReturnsFalseOnMismatch()
{
    var child = new Item(new("Tesoro","Treasure"), new("",""), 1);
    var hidden = new Hidden(new("Cofre","Chest"), new("",""), "Llave", child);
    Assert.False(hidden.TryBreak("Espada"));
}
// Review Focus: Hidden-item index preservation
[Fact] void Room_Break_InsertHiddenChildAtSameIndex()
{
    var room = new Room(new("",""), new("",""), 100);
    var before = new Item(new("A","A"), new("",""), 1);
    var hiddenChild = new Item(new("C","C"), new("",""), 1);
    var hidden = new Hidden(new("B","B"), new("",""), "Hacha", hiddenChild);
    var after = new Item(new("D","D"), new("",""), 1);
    room.Children.AddRange([before, hidden, after]);
    room.BreakThing("B", "Hacha");
    Assert.Equal(1, room.Children.IndexOf(hiddenChild));
}
```

- [ ] **Step 2: Implement `Troll.cs`**

- `string Likeness` — normalized item name that makes the troll happy
- `LocalizedText HiData`, `LocalizedText LoData`
- `bool Happy { get; private set; }`
- `override void Speak()` — outputs `Happy ? HiData.Get() : LoData.Get()`
- `override void Accept(Item gift)` — if `StringHelper.Normalize(gift.Name.Es)` matches `StringHelper.Normalize(Likeness)`: set `Happy = true`, output `Msg("VeryThanks")`; else output `Msg("CoolThanks")`

- [ ] **Step 3: Implement `Guard.cs`**

- `LocalizedText Confession`, `string LethalWeapon`
- `override bool Die(string weapon)` — if empty weapon or normalized weapon != normalized `LethalWeapon`: output `Msg("Useless")` + return false. Else: output `Confession.Get()`, set `Dead = true`, return true.

- [ ] **Step 4: Implement `Hidden.cs`**

- `string BreakerName` (item name needed, normalized)
- `Thing HiddenChild`
- `bool TryBreak(string weaponName)` — returns `StringHelper.Normalize(weaponName) == StringHelper.Normalize(BreakerName)`
- Note: `Room.BreakThing` is responsible for the swap (see Task 3 step 2)

- [ ] **Step 5: Add `Daughter` class at bottom of `Guard.cs`**

```csharp
class Daughter(LocalizedText name, LocalizedText desc, LocalizedText confession, string weapon)
    : Guard(name, desc, confession, weapon)
{
    public override bool Die(string weapon) { Dead = true; Output(Confession.Get()); return true; }
}
```

- [ ] **Step 6: Run tests**

Run: `dotnet test`
Expected: all pass.

- [ ] **Step 7: Commit**

```
git commit -m "feat: add Troll, Guard, Hidden NPC types"
```

---

### Task 5: Link Types

**Files:**
- Create: `src\Fortaleza\Engine\Links\Link.cs`
- Create: `src\Fortaleza\Engine\Links\OpenLink.cs`
- Create: `src\Fortaleza\Engine\Links\DangerLink.cs`
- Create: `src\Fortaleza\Engine\Links\DangerLink2.cs`
- Create: `src\Fortaleza\Engine\Links\RiddleLink.cs`
- Test: `tests\Fortaleza.Tests\Engine\LinkTests.cs`

**Interfaces:**
- Consumes: `Thing`, `Room`, `Inventory`, `Localizer.Msg`, `StringHelper.Normalize`
- Produces:
  - `class Link : Thing` — `Room? Destination`, `string Key`, `LocalizedText PassComment`, `bool IsOpen`, `virtual bool TryOpen(string key)`, `virtual Room? GetDest()`, `virtual void Pass(Inventory bag)`
  - `class OpenLink : Link` — always open; `TryOpen` always returns true
  - `class DangerLink : Link` — `string TalismanName`; `Pass` kills player if bag lacks talisman
  - `class DangerLink2 : Link` — kills player if bag HAS the named item
  - `class RiddleLink : Link` — `LocalizedText Riddle`, `string Answer`; `TryOpen` requires normalized answer match; `GetDest` returns null until opened

- [ ] **Step 1: Write failing tests**

```csharp
[Fact] void Link_TryOpen_SucceedsWithEmptyKey()
{
    var link = new Link(new("Puerta","Door"), new("",""), new("you pass","you pass"), "", MakeRoom());
    Assert.True(link.TryOpen(""));
}
[Fact] void Link_TryOpen_SucceedsWithCorrectKey()
{
    var link = new Link(new("Puerta","Door"), new("",""), new("",""), "Fortaleza", MakeRoom());
    Assert.True(link.TryOpen("Fortaleza"));
    Assert.True(link.TryOpen("FORTALEZA"));  // case insensitive
}
[Fact] void Link_TryOpen_FailsWithWrongKey()
{
    var link = new Link(new("Puerta","Door"), new("",""), new("",""), "Fortaleza", MakeRoom());
    Assert.False(link.TryOpen("wrong"));
}
// Review Focus: RiddleLink diacritic-insensitive match
[Fact] void RiddleLink_TryOpen_IgnoresDiacriticsAndCase()
{
    var link = new RiddleLink(new("Escalera","Staircase"), new("",""),
        new("¿Cuántos peldaños?","How many steps?"), "Treinta y nueve",
        new("you climb","you climb"), MakeRoom());
    Assert.True(link.TryOpen("treinta y nueve"));
    Assert.True(link.TryOpen("TREINTA Y NUEVE"));
    Assert.True(link.TryOpen("Treinta Y Nueve"));
}
// Review Focus: DangerLink kills without talisman
[Fact] void DangerLink_Pass_KillsPlayerWithoutTalisman()
{
    var player = MakePlayer();
    var link = new DangerLink(new("Traquea","Trachea"), new("",""),
        new("",""), "you cross", "Talismán de aire", MakeRoom());
    link.Pass(player.Bag);
    Assert.True(player.Dead);
}
[Fact] void DangerLink_Pass_SucceedsWithTalisman()
{
    var player = MakePlayer();
    player.Bag.Children.Add(new Item(new("Talismán de aire","Air talisman"), new("",""), 1));
    var link = new DangerLink(new("Traquea","Trachea"), new("",""),
        new("",""), "you cross", "Talismán de aire", MakeRoom());
    link.Pass(player.Bag);
    Assert.False(player.Dead);
}
// Review Focus: DangerLink2 item-must-be-absent
[Fact] void DangerLink2_Pass_KillsPlayerWithForbiddenItem()
{
    var player = MakePlayer();
    player.Bag.Children.Add(new Item(new("Talismán de Nieve","Snow talisman"), new("",""), 1));
    var link = new DangerLink2(new("Camino","Path"), new("",""),
        new("",""), "you walk", "Talismán de Nieve", MakeRoom());
    link.Pass(player.Bag);
    Assert.True(player.Dead);
}
[Fact] void DangerLink2_Pass_SucceedsWithoutForbiddenItem()
{
    var player = MakePlayer();
    var link = new DangerLink2(new("Camino","Path"), new("",""),
        new("",""), "you walk", "Talismán de Nieve", MakeRoom());
    link.Pass(player.Bag);
    Assert.False(player.Dead);
}
```

- [ ] **Step 2: Implement `Link.cs`**

Base class for all passages:
- `Room? Destination { get; }`
- `string Key { get; }` — empty string means always open
- `LocalizedText PassComment`
- `bool IsOpen { get; protected set; }`
- Constructor sets `IsOpen = string.IsNullOrEmpty(Key)`
- `virtual bool TryOpen(string key)` — `IsOpen = (Key == "" || StringHelper.Normalize(key) == StringHelper.Normalize(Key))`; returns `IsOpen`
- `virtual Room? GetDest() => IsOpen ? Destination : null`
- `virtual void Pass(Inventory bag)` — outputs `PassComment.Get()`
- `void Close() => IsOpen = false`

- [ ] **Step 3: Implement `OpenLink.cs`**

Inherits `Link`. Constructor calls base with empty key. `TryOpen` always returns true.

- [ ] **Step 4: Implement `DangerLink.cs`**

- `string TalismanName`
- `override void Pass(Inventory bag)` — if bag contains item matching `TalismanName` (normalized): output `PassComment.Get()`; else: output `Msg("NoProtection")`, call player's `Die("")` method.

Note: `Pass` needs access to the `Player` to call `Die`. Inject via constructor as `Action killPlayer` callback, or pass `Player` directly. Use `Action killPlayer` to avoid circular dependency.

- [ ] **Step 5: Implement `DangerLink2.cs`**

Same as `DangerLink` but inverted: kills player if bag DOES contain the named item; passes if item absent.

- [ ] **Step 6: Implement `RiddleLink.cs`**

- `LocalizedText Riddle`, `string Answer`
- `override bool TryOpen(string key)` — normalize both, compare; if match: `IsOpen = true`, output `Msg("RiddleSolved")`; else if key was empty: output riddle + hint; else output `Msg("WrongAnswer")`; return `IsOpen`

- [ ] **Step 7: Run tests**

Run: `dotnet test`
Expected: all pass.

- [ ] **Step 8: Commit**

```
git commit -m "feat: add all five Link types with danger/riddle logic"
```

---

### Task 6: Player

**Files:**
- Create: `src\Fortaleza\Engine\Player.cs`
- Test: `tests\Fortaleza.Tests\Engine\PlayerTests.cs`

**Interfaces:**
- Consumes: `Room`, `Inventory`, `LivingThing`, `Link`, `OpenLink`, `DangerLink`, `RiddleLink`, `Troll`, `Guard`, `Hidden`, `Localizer.Msg`
- Produces:
  - `class Player : LivingThing` — `Room CurrentRoom`, `Inventory Bag`, `int VisitCount`
  - Methods: `Look()`, `See(string what)`, `Take(string what)`, `LeaveItem(string what)`, `LeaveAll()`, `Walk(string door)`, `OpenDoor(string door, string key)`, `Kill(string who, string weapon)`, `Give(string what, string who)`, `Ask(string who)`, `Break(string what, string with)`, `Weigh(string what)`, `Inventory()`
  - `void Go(Room next)` — moves player, increments `VisitCount` if first visit
  - `override bool Die(string weapon)` — sets `Dead`, outputs `Msg("Dying")`

- [ ] **Step 1: Write failing tests**

```csharp
[Fact] void Player_Take_AddsItemToBag()
{
    var (player, room) = SetupPlayerInRoom();
    room.Children.Add(new Item(new("Maza","Mace"), new("",""), 5));
    player.Take("Maza");
    Assert.Single(player.Bag.Children);
    Assert.Empty(room.Children);
}
[Fact] void Player_Take_FailsWhenTooHeavy()
{
    var (player, room) = SetupPlayerInRoom();
    room.Children.Add(new Item(new("Cedro","Cedar"), new("",""), Inventory.LWeight + 1));
    player.Take("Cedro");
    Assert.Empty(player.Bag.Children);
}
[Fact] void Player_Leave_MovesItemToRoom()
{
    var (player, room) = SetupPlayerInRoom();
    var item = new Item(new("Maza","Mace"), new("",""), 5);
    player.Bag.Children.Add(item);
    player.LeaveItem("Maza");
    Assert.Single(room.Children);
    Assert.Empty(player.Bag.Children);
}
[Fact] void Player_Kill_WithCorrectWeapon_RemovesGuardFromRoom()
{
    var (player, room) = SetupPlayerInRoom();
    var guard = new Guard(new("Lobo","Wolf"), new("",""), new("I die","I die"), "Látigo");
    room.Children.Add(guard);
    player.Bag.Children.Add(new Item(new("Látigo","Whip"), new("",""), 2));
    player.Kill("Lobo", "Látigo");
    Assert.DoesNotContain(guard, room.Children);
}
[Fact] void Player_Give_ToTroll_RemovesItemFromBag()
{
    var (player, room) = SetupPlayerInRoom();
    var troll = new Troll(new("Bruja","Witch"), new("",""), "Escoba",
        new("Gracias","Thanks"), new("Dame la escoba","Give broom"));
    room.Children.Add(troll);
    var broom = new Item(new("Escoba","Broom"), new("",""), 1);
    player.Bag.Children.Add(broom);
    player.Give("Escoba", "Bruja");
    Assert.Empty(player.Bag.Children);
    Assert.True(troll.Happy);
}
[Fact] void Player_Go_IncrementsVisitCountOnFirstVisit()
{
    var player = MakePlayer();
    var room = new Room(new("sala","room"), new("",""), 100) { Visited = false };
    player.Go(room);
    Assert.Equal(1, player.VisitCount);
    player.Go(room);
    Assert.Equal(1, player.VisitCount);  // no increment on revisit
}
```

- [ ] **Step 2: Implement `Player.cs`**

Methods delegate to `CurrentRoom` and `Bag` operations; follow original Pascal logic exactly:
- `Take`: check room has item, check weight, move to bag
- `LeaveItem`: check bag has item, move to room
- `Walk`: find link in current room, check `GetDest()`, call `Go()` + `link.Pass(Bag)`; if `GetDest()` null: Ouch + try-open hint
- `OpenDoor`: optional magic-words output, delegates to `CurrentRoom.OpenDoor(door, key)`
- `Kill`: find living thing in room, check weapon in bag (or empty weapon), call `thing.Die(weapon)`, remove from room on true
- `Give`: find troll in room, take item from bag, call `troll.Accept(item)`, dispose item
- `Ask`: find living thing in room, call `Speak()`
- `Break`: check weapon in bag, delegate to `CurrentRoom.BreakThing(what, with)`
- `Weigh`: requires "Balanza" in bag; finds item in bag or room; outputs weight

- [ ] **Step 3: Run tests**

Run: `dotnet test`
Expected: all pass.

- [ ] **Step 4: Commit**

```
git commit -m "feat: add Player with all game action methods"
```

---

### Task 7: Bilingual Command Parser

**Files:**
- Create: `src\Fortaleza\Parser\CommandParser.cs`
- Test: `tests\Fortaleza.Tests\Parser\CommandParserTests.cs`

**Interfaces:**
- Consumes: `StringHelper.Normalize`
- Produces:
  - `enum CommandType { Go, Take, Leave, LeaveAll, Open, Kill, Look, See, Break, Ask, Inventory, Give, Quit, Exec, Save, Percent, Weigh, Clear, Unknown }`
  - `record ParsedCommand(CommandType Type, string Arg1, string Arg2)` — Arg1 is direct object, Arg2 is indirect object (after "con"/"with" or "a"/"to")
  - `static ParsedCommand CommandParser.Parse(string input)` — full bilingual parse

- [ ] **Step 1: Write failing tests**

```csharp
// Review Focus: article strip
[Fact] void Parse_StripsSpanishArticles()
{
    var cmd = CommandParser.Parse("ir por la puerta principal");
    Assert.Equal(CommandType.Go, cmd.Type);
    Assert.Equal("PUERTA PRINCIPAL", cmd.Arg1);
}
[Fact] void Parse_StripsEnglishArticles()
{
    var cmd = CommandParser.Parse("go through the main door");
    Assert.Equal(CommandType.Go, cmd.Type);
    Assert.Equal("MAIN DOOR", cmd.Arg1);
}
[Fact] void Parse_SpanishTake() => Assert.Equal(CommandType.Take, CommandParser.Parse("coger maza").Type);
[Fact] void Parse_EnglishTake() => Assert.Equal(CommandType.Take, CommandParser.Parse("take mace").Type);
[Fact] void Parse_SpanishGiveWithPreposition()
{
    var cmd = CommandParser.Parse("dar escoba a la bruja");
    Assert.Equal(CommandType.Give, cmd.Type);
    Assert.Equal("ESCOBA", cmd.Arg1);
    Assert.Equal("BRUJA", cmd.Arg2);
}
[Fact] void Parse_EnglishGiveWithPreposition()
{
    var cmd = CommandParser.Parse("give broom to the witch");
    Assert.Equal(CommandType.Give, cmd.Type);
    Assert.Equal("BROOM", cmd.Arg1);
    Assert.Equal("WITCH", cmd.Arg2);
}
[Fact] void Parse_SpanishBreakWithCon()
{
    var cmd = CommandParser.Parse("romper espejo con hueso de gato");
    Assert.Equal(CommandType.Break, cmd.Type);
    Assert.Equal("ESPEJO", cmd.Arg1);
    Assert.Equal("HUESO DE GATO", cmd.Arg2);
}
[Fact] void Parse_SpanishOpenWithRespondiendo()
{
    var cmd = CommandParser.Parse("abrir escalera respondiendo treinta y nueve");
    Assert.Equal(CommandType.Open, cmd.Type);
    Assert.Equal("ESCALERA", cmd.Arg1);
    Assert.Equal("TREINTA Y NUEVE", cmd.Arg2);
}
[Fact] void Parse_OpenWithDiciendo()
{
    var cmd = CommandParser.Parse("abrir puerta diciendo fortaleza");
    Assert.Equal(CommandType.Open, cmd.Type);
    Assert.Equal("PUERTA", cmd.Arg1);
    Assert.Equal("FORTALEZA", cmd.Arg2);
}
[Fact] void Parse_LeaveAll()
{
    Assert.Equal(CommandType.LeaveAll, CommandParser.Parse("dejar todo").Type);
    Assert.Equal(CommandType.LeaveAll, CommandParser.Parse("drop all").Type);
}
[Fact] void Parse_Unknown() => Assert.Equal(CommandType.Unknown, CommandParser.Parse("xyzzy").Type);
```

- [ ] **Step 2: Implement `CommandParser.cs`**

Verb lookup table covers both Spanish and English synonyms:

```csharp
static readonly Dictionary<string, CommandType> Verbs = new()
{
    ["ATRAVESAR"] = CommandType.Go, ["IR"] = CommandType.Go,
    ["CRUZAR"] = CommandType.Go, ["PASAR"] = CommandType.Go,
    ["GO"] = CommandType.Go, ["WALK"] = CommandType.Go,
    ["TRAVERSE"] = CommandType.Go, ["CROSS"] = CommandType.Go,
    ["ENTER"] = CommandType.Go, ["THROUGH"] = CommandType.Go,
    ["TOMAR"] = CommandType.Take, ["COGER"] = CommandType.Take,
    ["TAKE"] = CommandType.Take, ["GET"] = CommandType.Take, ["PICK"] = CommandType.Take,
    ["SOLTAR"] = CommandType.Leave, ["DEJAR"] = CommandType.Leave,
    ["DROP"] = CommandType.Leave, ["LEAVE"] = CommandType.Leave, ["PUT"] = CommandType.Leave,
    ["ABRIR"] = CommandType.Open, ["OPEN"] = CommandType.Open,
    ["MATAR"] = CommandType.Kill, ["ASESINAR"] = CommandType.Kill,
    ["KILL"] = CommandType.Kill, ["ATTACK"] = CommandType.Kill, ["SLAY"] = CommandType.Kill,
    ["OBSERVAR"] = CommandType.Look, ["MIRAR"] = CommandType.Look,
    ["LOOK"] = CommandType.Look, ["EXAMINE"] = CommandType.Look,
    ["LEER"] = CommandType.See, ["VER"] = CommandType.See,
    ["READ"] = CommandType.See, ["SEE"] = CommandType.See,
    ["ROMPER"] = CommandType.Break, ["FORZAR"] = CommandType.Break,
    ["DESTROZAR"] = CommandType.Break,
    ["BREAK"] = CommandType.Break, ["FORCE"] = CommandType.Break, ["SMASH"] = CommandType.Break,
    ["PREGUNTAR"] = CommandType.Ask, ["INTERROGAR"] = CommandType.Ask,
    ["ASK"] = CommandType.Ask, ["TALK"] = CommandType.Ask, ["QUESTION"] = CommandType.Ask,
    ["INVENTARIO"] = CommandType.Inventory, ["INVENTORY"] = CommandType.Inventory,
    ["ITEMS"] = CommandType.Inventory, ["BAG"] = CommandType.Inventory, ["I"] = CommandType.Inventory,
    ["REGALAR"] = CommandType.Give, ["DAR"] = CommandType.Give,
    ["GIVE"] = CommandType.Give,
    ["ABANDONAR"] = CommandType.Quit, ["TERMINAR"] = CommandType.Quit,
    ["QUIT"] = CommandType.Quit, ["EXIT"] = CommandType.Quit,
    ["EJECUTAR"] = CommandType.Exec, ["EXECUTE"] = CommandType.Exec, ["RUN"] = CommandType.Exec,
    ["SALVAR"] = CommandType.Save, ["SAVE"] = CommandType.Save,
    ["PORCIENTO"] = CommandType.Percent, ["PERCENT"] = CommandType.Percent,
    ["PESAR"] = CommandType.Weigh, ["WEIGH"] = CommandType.Weigh,
    ["CLS"] = CommandType.Clear, ["CLEAR"] = CommandType.Clear,
};
```

Strip words (articles/prepositions to remove before verb lookup):
```csharp
static readonly HashSet<string> StripWords = new()
{ "EL","LA","LOS","LAS","POR","AL","UN","UNA","THE","A","AN","BY","INTO" };
```

Split words that separate Arg1 from Arg2 (kept in token stream to detect boundary):
`"CON"`, `"WITH"`, `"DICIENDO"`, `"RESPONDIENDO"`, `"SAYING"`, `"ANSWERING"`, `"A"`, `"TO"` (only when following an object)

Algorithm:
1. Normalize input to uppercase, no diacritics
2. Split on whitespace
3. Remove strip words
4. First remaining token = verb → `CommandType`
5. Scan remaining tokens for separator (`CON/WITH/DICIENDO/RESPONDIENDO/SAYING/ANSWERING`) or (`A/TO` after non-verb token); tokens before = `Arg1`, tokens after = `Arg2`
6. Check if `Arg1 == "TODO"` or `Arg1 == "ALL"` → `CommandType.LeaveAll`

- [ ] **Step 3: Run tests**

Run: `dotnet test`
Expected: all pass.

- [ ] **Step 4: Commit**

```
git commit -m "feat: add bilingual command parser"
```

---

### Task 8: Game Engine

**Files:**
- Create: `src\Fortaleza\Engine\GameEngine.cs`
- Test: Integration tests added to `tests\Fortaleza.Tests\Engine\PlayerTests.cs`

**Interfaces:**
- Consumes: all `Engine\` and `Parser\` types
- Produces:
  - `abstract class GameEngine` — `Player Player`, `string Wish`, `bool IsOver`, `abstract bool CheckGoal()`
  - `void Start()` — sets player to first room
  - `void AskPlayer()` — reads input, stores in `Wish`
  - `void ProcessWish()` — dispatches `ParsedCommand` to `Player` methods
  - `void SaveTrack(string path)` — writes command log
  - `bool LoadTrack(string path)` — replays commands silently
  - `void ShowPercent()` — outputs `VisitCount / TotalRooms * 100`

- [ ] **Step 1: Implement `GameEngine.cs`**

- Holds list of all `Room` objects
- `void AddRoom(Room r)` — appends to room list
- `string Wish { get; private set; }` — current raw input
- `bool IsOver { get; private set; }` — set when player dies
- `List<string> Track` — command log (excludes look/see/inventory/ask/percent)
- `void AskPlayer()` — prints `Msg("WhatToDo")`, reads `Console.ReadLine()`, stores in `Wish`
- `void ClearWish() => Wish = ""`
- `void ProcessWish()` — parse `Wish`, dispatch via `switch(cmd.Type)`:
  - `Go` → `Player.Walk(arg1)`
  - `Take` → `Player.Take(arg1)`
  - `Leave` → `Player.LeaveItem(arg1)`; if `LeaveAll`: `Player.LeaveAll()`
  - `Open` → `Player.OpenDoor(arg1, arg2)`
  - `Kill` → `Player.Kill(arg1, arg2)`
  - `Look` → `Player.Look()`
  - `See` → `Player.See(arg1)`
  - `Break` → `Player.Break(arg1, arg2)`
  - `Ask` → `Player.Ask(arg1)`
  - `Inventory` → `Player.Inventory()`
  - `Give` → `Player.Give(arg1, arg2)`
  - `Quit` → `IsOver = true`
  - `Save` → ask filename, `SaveTrack(...)`
  - `Exec` → ask filename, `LoadTrack(...)`
  - `Percent` → `ShowPercent()`
  - `Weigh` → `Player.Weigh(arg1)`
  - `Clear` → `Console.Clear()`
  - `Unknown` → `Output(Msg("DontUnderstand"))`
- Log non-read commands to `Track`

- [ ] **Step 2: Integration test: full simple command sequence**

```csharp
[Fact] void Engine_TakeAndLeave_RoundTrip()
{
    var engine = BuildMinimalEngine();  // one room, one item
    engine.ProcessCommand("coger maza");
    Assert.Single(engine.Player.Bag.Children);
    engine.ProcessCommand("dejar maza");
    Assert.Empty(engine.Player.Bag.Children);
}
```

- [ ] **Step 3: Run tests**

Run: `dotnet test`
Expected: all pass.

- [ ] **Step 4: Commit**

```
git commit -m "feat: add GameEngine command dispatch and track system"
```

---

### Task 9: Decoded Game Keys

**Files:**
- Create: `src\Fortaleza\Games\GameKeys.cs`

**Interfaces:**
- Produces: `static class GameKeys` with `const string` fields for all 47 Fort1 keys and Fort2 puzzle values

- [ ] **Step 1: Implement `GameKeys.cs`**

Each key is the decoded value (subtract 20 from each byte in CP437). Keys confirmed from source analysis are marked; others are best-guess from context and should be verified against original game play.

```csharp
static class GameKeys
{
    // Fort1 keys ─ decoded from FORT1.PAS key[] array
    // Confirmed: key[6] from room 5 return staircase hardcoded as "treinta y nueve"
    // Confirmed: key[7] from statue description "Le falta la lanza"
    // Confirmed: key[40] from rat dialogue "buscando la quinta escencia"

    public const string F1_K01_BronzeKnockerHappy = "Las palabras mágicas para abrir la puerta son el título del juego.";  // TODO: verify
    public const string F1_K02_MainDoorPassword   = "Fortaleza";          // TODO: verify (inscription says "obvious")
    public const string F1_K03_WallBreaker         = "Maza";               // TODO: verify
    public const string F1_K04_MonolithBreaker     = "Roble";              // TODO: verify
    public const string F1_K05_CloverHappy         = "El libro de la biblioteca conduce al jardín.";  // TODO: verify
    public const string F1_K06_StaircaseAnswer     = "Treinta y nueve";   // CONFIRMED
    public const string F1_K07_AthenaLikeness      = "Lanza";              // CONFIRMED (missing lance)
    public const string F1_K08_AthenaHappy         = "El libro te llevará a otro lugar.";  // TODO: verify
    public const string F1_K09_HermesLikeness      = "Paraguas";           // TODO: verify (moisture protection)
    public const string F1_K10_HermesHappy         = "En la biblioteca hay un libro que conduce al jardín.";  // TODO: verify
    public const string F1_K11_OpaquesMirrorBreaker = "Hueso de gato";     // TODO: verify
    public const string F1_K12_CyclopsConfession   = " Pasa a través de la puerta con Ojo de cíclope.";  // TODO: verify
    public const string F1_K13_CyclopsWeapon       = "Maza";               // TODO: verify
    public const string F1_K14_LibraryDoorPassword = "Ojo de cíclope";     // TODO: verify
    public const string F1_K15_ForbiddenDoorPass   = "Fuerza";             // TODO: verify
    public const string F1_K16_WardrobeInjury      = "Las sombras te engullen. El candelabro te hubiera protegido.";  // TODO: verify
    public const string F1_K17_WitchHappy          = "La escoba es el objeto más poderoso. Úsala bien.";  // TODO: verify
    public const string F1_K18_BedBreaker          = "Hacha";              // TODO: verify
    public const string F1_K20_HomunculusHappy     = "¡Por fin puedo crecer! Gracias.";  // TODO: verify
    public const string F1_K22_TrollHappy          = "Nadie podrá ver mi fealdad. Gracias. El troll desaparece.";  // TODO: verify
    public const string F1_K23_DwarfName           = "Rumpelstiltskin";    // TODO: verify
    public const string F1_K24_MaidenLikeness      = "Rosa";               // INFERRED (rose from room 12)
    public const string F1_K25_MaidenHappy         = "El corazón de unicornio abrirá la arteria principal.";  // TODO: verify
    public const string F1_K26_HarpyWeapon         = "Cigarro";            // TODO: verify
    public const string F1_K27_HarpyConfession     = "¡Arpía derrotada! El acceso al pasillo queda libre.";  // TODO: verify
    public const string F1_K29_SkeletonHappy       = "El talismán de aire te permitirá cruzar la tráquea.";  // TODO: verify
    public const string F1_K30_TracheaTalisman     = "Talismán de aire";   // TODO: verify (air protects lungs)
    public const string F1_K31_EsophagusTalisman   = "Paraguas";           // TODO: verify
    public const string F1_K32_LungCenterConfession = "¡Me has matado! Los pulmones de la Bestia se detienen.";  // TODO: verify
    public const string F1_K33_StomachCenterWeapon = "Daga";               // TODO: verify
    public const string F1_K34_HeartCenterWeapon   = "Corazón de unicornio";  // TODO: verify
    public const string F1_K35_LungCenterWeapon    = "Lanza";              // TODO: verify
    public const string F1_K37_HeartCenterConfession = "¡Me has matado! El corazón de la Bestia se detiene.";  // TODO: verify
    public const string F1_K38_BrainCenterWeapon   = "Antorcha";           // INFERRED (brain says "¡Usa la antorcha!")
    public const string F1_K39_BrainInscription    = "El fin de la Bestia está cerca. El cerebro es su último bastión.";  // TODO: verify
    public const string F1_K40_RatLikeness         = "Piedra filosofal";   // CONFIRMED (rat seeks quinta escencia)
    public const string F1_K41_RatHappy            = "¡La piedra filosofal! ¡Por fin! ¡Ahora puedo completar la fórmula!";  // TODO: verify
    public const string F1_K42_CrystalDoorPass     = "Espejo";             // TODO: verify
    public const string F1_K43_MoorTunnelTalisman  = "Nada";               // TODO: verify (trap - kills with any item?)
    public const string F1_K44_WolfConfession      = "¡Aúllooo! ¡Me has vencido!";  // TODO: verify
    public const string F1_K47_DancerHappy         = "Seré joven para siempre. Gracias. Ve hacia el norte para encontrar la entrada de la Bestia.";  // TODO: verify

    // Fort2 puzzle passwords (from FORT2.PAS context)
    public const string F2_DaughterName_Short = "Aura";          // from PRiddleLink in room 2 (hardcoded)
    public const string F2_DaughterName_Full  = "Aura Srka";     // from PRiddleLink in room 44 (hardcoded)
    public const string F2_ChestPassword      = "MIAU";          // from room 5 (hardcoded)
    public const string F2_TreasurePassword   = "Omicuos Ihanti"; // from room 13 (hardcoded)
    public const string F2_GriffonWeapon      = "Daga";          // from room 2 (hardcoded)
    // All other Fort2 Guard weapons and Troll likenesses are hardcoded in FORT2.PAS
}
```

- [ ] **Step 2: Commit**

```
git commit -m "feat: add decoded game keys with TODO markers for unverified values"
```

---

### Task 10: Fort1 Game Builder

**Files:**
- Create: `src\Fortaleza\Games\Fort1Builder.cs`
- (No separate test file — integration tested via Program.cs game loop)

**Interfaces:**
- Consumes: all `Engine\` types, `GameKeys`, `Localizer`
- Produces:
  - `class Fort1 : GameEngine` — overrides `CheckGoal()` with original Fort1 win condition
  - `static Fort1Builder.Build(Action killPlayer)` → `Fort1` fully populated game

- [ ] **Step 1: Implement `Fort1.cs` goal check**

Win condition (from `FORT1.PAS Fort.Goal`):
```csharp
public override bool CheckGoal() =>
    GetRoom(20).Get("Centro del cerebro") is null &&
    GetRoom(19).Get("Centro del corazon") is null &&
    GetRoom(18).Get("Centro del estomago") is null &&
    GetRoom(17).Get("Centro de los pulmones") is null &&
    GetRoom(10).Get("Troll") is null;
```
Note: room indices are 0-based here (0 = room 1 in Pascal).

- [ ] **Step 2: Implement all 50 rooms in `Fort1Builder.cs`**

Faithfully translates the Pascal `SetData` procedure. Each room gets:
- A bilingual `LocalizedText` name and description (Spanish from source, English translated)
- Its items, trolls, guards, hidden containers, and links populated exactly as in the original

Begin with rooms 1–10, each in their own `Build_Room_N` private method called from `Build()`.

English translations for room descriptions are provided inline as the second argument to `LocalizedText`. Example:
```csharp
new Room(
    new("el exterior de la fortaleza", "the exterior of the fortress"),
    new("Las paredes son muy negras...",
        "The walls are very black and apparently have no windows. You try to see the top of the towers, but the clouds will not allow it."),
    Inventory.LWeight + 1)
```

- [ ] **Step 3: Implement rooms 11–33 and labyrinth rooms 34–50**

Labyrinth rooms 31–48 all use `LocalizedText("un pasillo del Laberinto","a Labyrinth corridor")` name and same description. Room 49 uses `LocalizedText("un pasillo del Laberinto","a Labyrinth corridor")` (exit). Room 50 uses `LocalizedText("el Jardín","the Garden")` with "Cuidado" inscription.

- [ ] **Step 4: Wire DangerLink kill callbacks**

`DangerLink` and `DangerLink2` receive `() => player.Die("")` as the kill callback in their constructor. This is injected at build time.

- [ ] **Step 5: Commit**

```
git commit -m "feat: implement Fort1 game data — all 50 rooms"
```

---

### Task 11: Fort2 Game Builder

**Files:**
- Create: `src\Fortaleza\Games\Fort2Builder.cs`

**Interfaces:**
- Consumes: all `Engine\` types, `GameKeys`
- Produces: `class Fort2 : GameEngine` — `CheckGoal()` with Fort2 win condition + `Fort2Builder.Build()`

- [ ] **Step 1: Implement `Fort2.cs` goal check**

Win condition (from `FORT2.PAS Fort.Goal`):
```csharp
public override bool CheckGoal() =>
    GetRoom(29).Get("Rosa diamante") is not null &&
    GetRoom(10).Get("Bote") is not null &&
    GetRoom(0).Get("Antorcha") is not null &&
    GetRoom(41).Get("Cinta de Moebius") is not null &&
    GetRoom(38).Get("Escudo de Aquiles") is not null &&
    GetRoom(3).Get("Espejo") is not null &&
    GetRoom(2).Get("Péndulo") is not null &&
    GetRoom(44).Get("Hija del Hechicero") is null &&
    GetRoom(8).Get("Monstruo") is null;
```

- [ ] **Step 2: Implement all 55 rooms in `Fort2Builder.cs`**

Same pattern as Fort1. Fort2 has a sorcerer-defeated condition and epilogue text (see FORT2.PAS lines 1099–1134).

- [ ] **Step 3: Commit**

```
git commit -m "feat: implement Fort2 game data — all 55 rooms"
```

---

### Task 12: Program Entry Point

**Files:**
- Create: `src\Fortaleza\Program.cs`

**Interfaces:**
- Consumes: `Localizer`, `Fort1Builder`, `Fort2Builder`, `GameEngine`

- [ ] **Step 1: Implement `Program.cs`**

```csharp
// Language selection
Console.WriteLine("Select language / Seleccione idioma:");
Console.WriteLine("  1. Español");
Console.WriteLine("  2. English");
var langKey = Console.ReadKey(true).KeyChar;
Localizer.Current = langKey == '2' ? Language.English : Language.Spanish;

// Game selection
Console.Clear();
Console.WriteLine(Localizer.Current == Language.Spanish
    ? "Seleccione juego:"
    : "Select game:");
Console.WriteLine("  1. La Fortaleza I — En las entrañas de la bestia");
Console.WriteLine("  2. La Fortaleza II");
var gameKey = Console.ReadKey(true).KeyChar;

// Build and run
var player = new Player(new("Indy","Indy"), new("",""), 0);
GameEngine engine = gameKey == '2'
    ? Fort2Builder.Build(player)
    : Fort1Builder.Build(player);

engine.Start();
engine.Player.Output("");
// Time-of-day greeting
var hour = DateTime.Now.Hour;
engine.Player.Output(hour < 12 ? Localizer.Msg("GoodMorning")
    : hour < 19 ? Localizer.Msg("GoodAfternoon") : Localizer.Msg("GoodEvening"));

// Main game loop
while (!engine.CheckGoal() && !engine.IsOver)
{
    engine.AskPlayer();
    engine.ClearWish();
    engine.ProcessWish();
}

// End state
if (engine.CheckGoal())
{
    // Fort1 victory text; Fort2 has extended epilogue — delegated to engine subclass
    engine.ShowVictory();
}
else
{
    Console.Beep();
    engine.Player.Output(Localizer.Msg("Defeat"));
}

// Save track prompt
engine.Player.Output(Localizer.Msg("SaveTrack"));
if (char.ToUpper(Console.ReadKey(true).KeyChar) is 'S' or 'Y')
{
    engine.Player.Output(Localizer.Msg("EnterFileName"));
    var path = Console.ReadLine() ?? "TRAVEL.TRK";
    engine.Player.Output(Localizer.Msg("SaveTrackTo") + path);
    engine.SaveTrack(path);
}
```

- [ ] **Step 2: Run the game manually**

Start the game with `dotnet run --project src\Fortaleza`, select language and game, verify:
- Language selection works
- Game title screen appears
- First room description renders in chosen language
- `mirar` / `look` both work
- `inventario` / `inventory` both work

- [ ] **Step 3: Commit**

```
git commit -m "feat: add Program.cs with language/game selection and main loop"
```

---

## Execution Recommendation

**Plan complete and saved to `docs/superpowers/plans/2026-10-16-fortaleza-dotnet-port.md`. Please review the plan. Which execution approach would you prefer?**

- **Subagent-driven** — A fresh subagent implements each task, independent review per task, full-branch review at end. Most thorough; higher cost.
- **Native** — I implement every task in this session from the plan, then one reviewer on the whole branch. Fastest; no per-task independent gate.

**I recommend Native execution**, because the twelve tasks have tightly coupled interfaces (every later task consumes types from earlier ones) and the plan carries all the signatures needed — a single-session implementer reading the plan can stay consistent without per-task context resets.

Does the plan capture what you want, and which approach should we use?
