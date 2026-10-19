# La Fortaleza

A modern, bilingual (Spanish / English) port of **La Fortaleza**, the text adventure written in Turbo Pascal
in the 1990s by [Miguel Cepero](https://twitter.com/miguelcepero) (Merchise Group). You explore a living fortress,
solve riddles, trade objects with strange creatures and try to defeat the Beast (game I) or escape the
Fortress (game II).

This repository contains:

* a faithful **.NET 10 port of the game engine** (console game, 50 + 55 rooms, ES/EN),
* a **web version**: a stateless ASP.NET Core API plus a mobile-friendly Vue 3 app,
* **Terraform** to run the web version on Google Cloud (Cloud Run + Firebase Hosting) for about $0,
* the **original Pascal sources**, a FreePascal-compatible copy and compiled executables used to verify that
  the port behaves like the original,
* **minimal winning tracks** (`saves/`) for the four game variants.

> The track files in `saves/` are complete walkthroughs. Do not open them if you want to play unspoiled.

## Architecture

```
                         ┌──────────────────── Google Cloud ─────────────────────┐
                         │                                                        │
 Browser (Vue 3 app) ───►│ Firebase Hosting ─┬─ /*       ─► static files          │
                         │                   └─ /api/**  ─► Cloud Run             │
                         │                                  └─ Fortaleza.Api      │
                         │                                     └─ Fortaleza       │
                         └────────────────────────────────────────────────────────┘
```

* **`Fortaleza`** is the game engine and the console front end. Rooms, items, creatures, links (doors,
  riddles, dangerous passages) and the command parser mirror the object model of the original Pascal code.
  All narrative text is a `LocalizedText` with a Spanish and an English version; commands are accepted in
  both languages whatever the display language is.
* **`Fortaleza.Api`** exposes the engine over HTTP and is **stateless**. The game state is fully determined by the
  player's *track* (the list of commands so far, the same data as a saved `.TRK` file), so every request
  carries the track, the API rebuilds the game by replaying it, runs the new command and returns the
  new output and the updated track. This makes every player isolated, needs no database or registration,
  and lets it run on Cloud Run where any instance can answer any request.
* **`Fortaleza.Web`** is the Vue 3 UI: terminal-like transcript, tappable verbs and objects, command history,
  save (downloads the track) and load (uploads a track), and resume after a reload. The track lives in the
  browser (`localStorage`).
* The engine keeps its per-request settings (language, output sink, visibility) in `AsyncLocal`s, which is what
  lets many web players with different languages run concurrently without interfering.
* **Security model:** the API holds no state and no secrets, and the public `/api` path is, by design, callable
  by anyone who can reach the site (Firebase Hosting calls Cloud Run anonymously, so its `run.app` URL is public
  too). A cap on Cloud Run instances bounds what abuse can cost. See [terraform/README.md](terraform/README.md).

## Repository layout

| Folder | What it is |
|---|---|
| `src/Fortaleza` | Game engine and console app (`Engine/`, `Games/` Fort1 + Fort2 data, `Parser/`, `Localization/`). |
| `src/Fortaleza.Api` | ASP.NET Core minimal API (`/api/games/start`, `play`, `load`), also hosts the built site locally. |
| `src/Fortaleza.Web` | Vue 3 + Vite single-page app. |
| `tests/Fortaleza.Tests` | Engine tests, including playing every minimal path from the start. |
| `tests/Fortaleza.Api.Tests` | API tests: stateless replay, isolation under concurrency, track upload/limits. |
| `saves/` | `FORT1_ES/EN.TRK`, `FORT2_ES/EN.TRK`: the shortest known winning track per game and language. |
| `terraform/` | Google Cloud infrastructure and deploy scripts. |
| `original/` | The original Turbo Pascal sources by Miguel Cepero (GPLv3+). |
| `modern/` | FreePascal-compatible copy of the Pascal sources. `build.bat` builds `FORT1.EXE` / `FORT2.EXE` from it. |
| `games/` | Compiled FreePascal builds of the original Spanish games (Windows). |
| `docs/` | Planning notes from the porting work. |

## Prerequisites

* [.NET 10 SDK](https://dotnet.microsoft.com/download)
* Node 20+ and npm (web version only)
* Terraform >= 1.6, Docker and the gcloud CLI (deployment only)
* FreePascal `fpc` (only to rebuild the original executables with `build.bat`)

## Play it

### Console

```bash
dotnet run --project src/Fortaleza
```

Choose a language and a game, then type commands. Both Spanish and English verbs work:

| Action | Spanish | English |
|---|---|---|
| Move through a door | `ir puerta verde` | `go green door` |
| Take / drop | `tomar hacha`, `dejar todo` | `take axe`, `drop all` |
| Open with a password or answer | `abrir puerta prohibida diciendo luz` | `open forbidden door saying light` |
| Kill / break with an object | `matar troll`, `romper cama con hacha` | `kill troll`, `break bed with axe` |
| Look / read | `mirar`, `ver inscripcion` | `look`, `read inscription` |
| Talk / give | `preguntar bruja`, `dar escoba a bruja` | `ask witch`, `give broom to witch` |
| Inventory / weigh (with the scale) | `inventario`, `pesar hacha` | `inventory`, `weigh axe` |
| Save / replay a track | `salvar`, `ejecutar` | `save`, `execute` |

A **track** is a plain text file with one command per line. `salvar` writes one, `ejecutar` replays it.
At the end of a game you are offered to save it as `TRAVEL.TRK`. To watch a winning game, replay one of the files
in `saves/` (for example `ejecutar` then `saves/FORT1_ES.TRK`).

### Web, one server

```bash
cd src/Fortaleza.Web && npm install && npm run build   # builds into src/Fortaleza.Api/wwwroot
cd ../.. && dotnet run --project src/Fortaleza.Api --urls http://localhost:5080
```

Open <http://localhost:5080>.

### Web, development

Run the API and the Vite dev server (which proxies `/api` to port 5080):

```bash
dotnet run --project src/Fortaleza.Api --urls http://localhost:5080
cd src/Fortaleza.Web && npm run dev
```

## Test

```bash
dotnet test
```

This runs the engine tests (including one that replays each of the four minimal winning tracks and asserts that
the game is won exactly on the last command) and the API tests (including playing each track one command per
request, as a browser would).

## Compatibility with the original Pascal games

The Spanish tracks in `saves/` are ASCII-only on purpose, so the original executables accept them. Running
`games/FORT1.exe saves/FORT1_ES.TRK` (and `FORT2.exe` with `FORT2_ES.TRK`) executes the track at startup and reaches
the ending. The English tracks only apply to this port; the original has no English version.

## Deploy to Google Cloud

After the one-time `terraform/bootstrap` stack ([terraform/README.md](terraform/README.md#one-time-setup)):

```bash
GOOGLE_CLOUD_PROJECT=my-project ./terraform/deploy.sh    # or deploy.ps1 in PowerShell 7
```

Pushes and merges to `master` also deploy automatically through GitHub Actions (Workload Identity Federation,
no stored keys); the setup is in [terraform/README.md](terraform/README.md#automatic-deployment-github-actions).

It builds the site and the API container, pushes the image to Artifact Registry, runs `terraform apply` and
publishes the site to Firebase Hosting. Inside the free quotas this costs about nothing. Custom domain, cost guards and state backend options are in
[terraform/README.md](terraform/README.md).

## Credits and license

La Fortaleza is © 1993–2013 Miguel Cepero, released under the GPLv3+ (see [original/COPYING](original/COPYING)).
The Pascal sources in `original/` and `modern/` are his; the .NET port, web version and infrastructure in this
repository are derived from them and are distributed under the same license.
