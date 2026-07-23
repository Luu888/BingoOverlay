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
✅ Backend ASP.NET Core .NET 10  

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

4. Ustaw rozdzielczość (potem w OBS można zmieniać rozmiar widżetu i on sie skaluje):
```bash
700 x 700
```

5. Gotowe — bingo będzie widoczne na streamie.
