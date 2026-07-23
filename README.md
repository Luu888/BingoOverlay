# 🎯 BingoOverlay

Widget Bingo 5x5 dla streamerów Twitch, przeznaczony do wyświetlania w OBS.

Aplikacja pozwala prowadzić interaktywne bingo podczas transmisji:
- plansza 5x5 widoczna jako overlay w OBS,
- możliwość oznaczania wykonanych pól,
- aktualizacja planszy w czasie rzeczywistym,
- obsługa SignalR,
- lokalna baza SQLite.

## ✨ Funkcje

✅ Plansza Bingo 5x5  
✅ Overlay kompatybilny z OBS Studio  
✅ Aktualizacja pól bez odświeżania strony  
✅ Zapisywanie stanu planszy  
✅ Możliwość personalizacji wyglądu  
✅ Aplikacja webowa ASP.NET Core .NET 10 

## 🛠 Technologie

- ASP.NET Core .NET 10
- Razor Pages
- Entity Framework Core
- SQLite
- SignalR
- HTML / CSS / JavaScript

## 🚀 Uruchomienie projektu

### Wymagania

- .NET 10 SDK
- Visual Studio 2026 lub nowsze

### Start aplikacji

1. Sklonuj repozytorium:

```bash
git clone https://github.com/twoj-login/BingoOverlay.git
```

2. Przejdź do katalogu projektu:
   
```bash
cd BingoOverlay
```

3. Uruchom aplikację:
   
```bash
dotnet run
```
Aplikacja będzie dostępna pod adresem:

```bash
http://localhost:8888
```

## 🔐 Konfiguracja Twitch

Aby korzystać z integracji z Twitch, należy utworzyć własną aplikację developerską i uzupełnić dane uwierzytelniające.

### 1. Utworzenie aplikacji Twitch Developer

1. Wejdź na stronę:

[https://dev.twitch.tv/console/apps](https://dev.twitch.tv/console/apps)

2. Zaloguj się kontem Twitch.

3. Kliknij:
```bash
Register Your Application
```
4. Uzupełnij formularz:
   
```bash
Name: BingoOverlay
OAuth Redirect URLs: http://localhost:8888/twitch/callback
Category: Application Integration
```
5. Kliknij Create
6. Po utworzeniu aplikacji pojawi się:
```bash
- Client ID
- Client Secret
```
Client Secret należy wygenerować przyciskiem **New Secret**
⚠️ Nie udostępniaj Client Secret publicznie.

### 2. Konfiguracja appsettings.json

1. Otwórz plik **appsettings.json** z folderu gdzie jest aplikacja
2. Uzupełnij sekcję Twitch
   
```bash
"Twitch": {
    "ClientId": "TWOJ_CLIENT_ID",
    "ClientSecret": "TWOJ_CLIENT_SECRET",
    "RedirectUri": "http://localhost:8888/twitch/callback"
  }
```
Tu przykład:

```bash
"Twitch": {
    "ClientId": "abc123xyz",
    "ClientSecret": "secret-value",
    "RedirectUri": "http://localhost:8888/twitch/callback"
  }
```
### 3. Połączenie z Twitch
Po uruchomieniu aplikacji (BingoOverlay.exe):

1. Otwórz stronę aplikacji.
2. Kliknij przycisk: **Połącz z Twitch**
3. Nastąpi przekierowanie do strony logowania Twitch
4. Zaakceptuj wymagane uprawnienia
5. Po poprawnej autoryzacji aplikacja zostanie połączona z kontem Twitch
   
Połączenie jest wykorzystywane do obsługi funkcji Twitch, takich jak:

   odbieranie zdarzeń z kanału,
   integracja z czatem,
   automatyczne aktualizowanie bingo.

# 📺 Dodanie do OBS

1. Uruchom aplikację.
2. W OBS dodaj źródło:
   
```bash
Źródło → Przeglądarka (Browser Source)
```
3. Ustaw URL:

```bash
http://localhost:8888
```

4. Ustaw rozdzielczość (potem w OBS można zmieniać rozmiar widżetu, a plansza automatycznie się skaluje):
```bash
700 x 700
```

5. Gotowe — bingo będzie widoczne na streamie.


## 💬 Dostępne komendy Twitch

Po połączeniu aplikacji z kanałem Twitch dostępne są komendy sterujące planszą Bingo.

### 🎯 Komenda Bingo

| Komenda | Opis | Uprawnienia |
|---|---|---|
| `!bingo <numer>` | Oznacza kafelek jako wykonany | Streamer, Moderator |
| `!bingoreset` | Resetuje całą planszę Bingo | Streamer |

---

### Przykłady użycia

Oznaczenie kafelka numer 1 lub kafelka numer 18:
```bash
!bingo 1
!bingo 18
```
- Próba użycia numeru spoza zakresu nie zmienia planszy.

Resetowanie całej planszy:
```bash
!bingoreset
```
- Reset planszy usuwa wszystkie oznaczenia wykonanych kafelków.

  ### 🔒 Uprawnienia Twitch

| Rola na kanale | `!bingo` | `!bingoreset` |
|---|---|---|
| Widz | ❌ | ❌ |
| Moderator | ✅ | ❌ |
| Streamer | ✅ | ✅ |

Uprawnienia są sprawdzane na podstawie roli użytkownika Twitch wysyłającego wiadomość.

## 📄 License

This project is licensed under the Apache License 2.0.

Copyright © 2026 Luu

All rights reserved.
