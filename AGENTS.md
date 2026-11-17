# AGENTS.md

Rules for AI coding agents working in this repository. Humans: see [README.md](README.md) and
[terraform/README.md](terraform/README.md).

## 1. What this is

La Fortaleza: a bilingual (Spanish/English) .NET 11 port of Miguel Cepero's Turbo Pascal text adventure
(GPLv3+), plus a stateless ASP.NET Core API, a Vue 3 web app, and Terraform for Google Cloud
(Cloud Run + Firebase Hosting).

| Path | Role |
|---|---|
| `src/Fortaleza` | Engine + console app (`Engine/`, `Games/` Fort1/Fort2 data, `Parser/`, `Localization/`) |
| `src/Fortaleza.Api` | Minimal API: `POST /api/games/start`, `POST /api/games/play`, `PUT /api/games/load` |
| `src/Fortaleza.Web` | Vue 3 + Vite SPA (plain JavaScript, no TypeScript) |
| `tests/Fortaleza.Tests`, `tests/Fortaleza.Api.Tests` | xUnit v3 tests |
| `saves/` | Minimal winning tracks (`FORT{1,2}_{ES,EN}.TRK`), used as regression oracles |
| `terraform/` | Cloud infrastructure, `deploy.sh` / `deploy.ps1`, one-time `bootstrap/` stack |
| `original/`, `modern/`, `games/`, `build.bat` | Original Pascal sources, FreePascal copy, Windows executables. **Read-only reference: never edit.** |

Dependency direction: `Fortaleza.Api` -> `Fortaleza`; tests -> `Fortaleza` (and `Fortaleza.Api`). Never the reverse.

## 2. Commands

```bash
dotnet build Fortaleza.slnx -c Release                 # warnings are errors; must be 0 errors / 0 warnings
dotnet test Fortaleza.slnx -c Release                  # same command CI runs; no Docker needed
dotnet format Fortaleza.slnx --verify-no-changes       # must exit 0
dotnet run --project src/Fortaleza                     # console game
dotnet run --project src/Fortaleza.Api --urls http://localhost:5080   # API; serves src/Fortaleza.Api/wwwroot

npm ci --prefix src/Fortaleza.Web                      # the lockfile is committed; use ci, not install
npm run build --prefix src/Fortaleza.Web               # builds into src/Fortaleza.Api/wwwroot (git-ignored)
npm run dev --prefix src/Fortaleza.Web                 # port 5173, proxies /api to localhost:5080

terraform fmt -check -recursive terraform              # CI checks this
```

A task is done only when the build, the tests and `dotnet format --verify-no-changes` pass (and `terraform fmt`
if Terraform changed, and `npm run build` if the web app changed).

## 3. Architecture invariants (do not break)

- **Stateless API.** Game state is fully determined by the *track* (the list of commands so far, same as a
  `.TRK` file). Every request carries the track; the API rebuilds the game by replaying it. Do not add server
  state, sessions, caches keyed by player, or a database.
- **Per-request engine settings live in `AsyncLocal`s** (`Localizer` language; `Thing` output sink, visibility,
  line width). That is what makes concurrent players with different languages safe. Do not replace them with
  plain statics. `Thing.Output` is `static` for this reason. Tests disable parallelization
  (`AssemblyInfo.cs`) because they touch this shared state; keep it that way.
- **Bilingual everywhere.** All narrative text is a `LocalizedText` (Spanish + English). Commands are accepted in
  both languages regardless of display language. Every key in `Localization/res/messages.es.json` must exist in
  `messages.en.json` and vice versa (currently 88 each).
- **Engine <-> web coupling.** `src/Fortaleza.Web/src/i18n.js` `markers` (`Usted ve:`, `Sus pertenecias son:` /
  `You see:`, `Your belongings are:`) must equal the engine's `RoomContains` / `BagContains` messages; the web
  `transcript.js` parses the API's plain-text output with them. Change both together.
- **Fidelity to the original.** The engine mirrors the Pascal object model. Do not "improve" game behavior, texts
  or puzzles. The minimal-path tests replay each `saves/*.TRK` and assert the game is won exactly on the last
  command. Spanish tracks in `saves/` must stay ASCII-only so the original `.exe` files accept them.
- **Track limits** (`TrackFile`): 64 KiB, 1000 lines, 200 chars per line, no control characters, and the `salvar`
  / `ejecutar` (save/execute) commands are rejected over the API.
- **Security model.** The API holds no state and no secrets and is public by design; `max_instances` on Cloud Run
  bounds cost. Never put secrets, tokens, project ids or `terraform.tfvars` content in the repo.

## 4. .NET conventions

The root [.editorconfig](.editorconfig) sets every analyzer diagnostic to **error**, and `Directory.Build.props`
adds StyleCop, Roslynator, Sonar, Meziantou and SecurityCodeScan, `TreatWarningsAsErrors`,
`GenerateDocumentationFile` and `EnforceCodeStyleInBuild`. Write code that builds clean the first time.

- Folder-level `.editorconfig` files (`src/Fortaleza`, `src/Fortaleza.Api`, `src/Fortaleza.Web`, `tests`) hold the
  **scoped** relaxations with a comment each. If a rule is genuinely wrong for a case, change the narrowest
  `.editorconfig` section and say why. **Never** use `#pragma warning disable`, `[SuppressMessage]` or
  `GlobalSuppressions.cs`.
- `var` only when the type is apparent from the right-hand side; otherwise write the type (also for built-ins).
  Braces on every `if`/`for`/`foreach`/`while`/`case` section with several statements; opening brace on its own
  line; trailing comma in multi-line initializers.
- File-scoped namespaces, `using`s outside the namespace, **one type per file** named after the type.
- `internal` by default. `Fortaleza` exposes internals only to `Fortaleza.Api` and `Fortaleza.Tests`
  (`InternalsVisibleTo` in the csproj). It is an Exe, so CA1515 forbids public types there.
- Private fields are `_camelCase` (including `private const`); `static readonly` fields are `PascalCase`.
- `string.Empty`, never `""`; `string.IsNullOrEmpty(x)` instead of `x == ""` / `x != ""`.
- Culture/ordinal: `StringComparison.Ordinal` for every `string` method that has an overload, `ToString(CultureInfo.InvariantCulture)`
  for numbers in text, `Convert.*` with `CultureInfo.InvariantCulture` instead of explicit casts (MA0181).
- Time via `TimeProvider`, never `DateTime.Now/UtcNow`.
- Methods returning `Task` end in `Async`; thread `CancellationToken` through API handlers.
- Switch expressions/statements need not list every enum member (IDE0072/IDE0010 are off). Keep the `default`/`_`
  arm: removing or adding arms changes behavior (see pitfalls).
- Comments: none by default; only a non-obvious *why*. No commented-out code, no unowned TODOs.
- XML docs are required on public API only where `CS1591` is not disabled; the exe, web and test projects disable it.

## 5. Testing conventions

- xUnit v3 (Microsoft.Testing.Platform runner, enabled in `global.json`), NSubstitute is not used; assertions are plain `Assert.*`. Tests are `public sealed class`.
- Test method names are PascalCase **without underscores** (CA1707): `GuardDieAcceptsWeaponNameInEitherLanguage`.
  Async tests end in `Async` and use `.ConfigureAwait(true)`.
- `[Theory]` + `[InlineData]` over duplicated `[Fact]`s. Theory parameters cannot be internal enums: pass the name
  as a string and `Enum.Parse` it.
- Every test asserts something (S2699). Tests that print resources still assert (`ResourceDebugTests`).
- API tests use `WebApplicationFactory<Program>`; `Program` is the generated public class, do not declare it.
- Any change to engine behavior, texts, parser or `Fort*Builder` data must keep all four minimal-path tests green.

## 6. Web (Vue 3) conventions

- Single-file components with `<script setup>`, plain JS modules, one global `style.css` using CSS variables.
  Dependencies are `vue`, `vite`, `@vitejs/plugin-vue` only; do not add a UI framework or TypeScript without asking.
- UI text lives in `i18n.js` for **both** `es` and `en`. Default language is **Spanish** (a saved game/language wins).
- Browser storage keys: `fortaleza.session`, `fortaleza.theme`, `fortaleza.interaction` (wrap in try/catch).
- Five themes in `themes.js` (`dungeon`, `parchment`, `terminal`, `contrast`, `daylight`) x two interaction modes
  (`scroll`, `typewrite`). Any color change must be checked in all of them.
- **WCAG 2.2 AA is a requirement.** Text 4.5:1, UI and focus indicators 3:1, touch targets at least 36px on phones
  (44px elsewhere), visible focus, no horizontal scrolling at 320px, honor `prefers-reduced-motion`, announce
  changes via the persistent `role="status"` region, return focus to the trigger when closing menus/dialogs.
  After UI changes run axe-core and Lighthouse (expect Accessibility 100 on desktop and mobile).
- Verb bar: all 11 verbs show on one row at >= 920px (game column `max-width: 920px`); below that, the first
  `PRIMARY_VERBS` (4) show plus a "More… / Less…" toggle. Keep that behavior and the Spanish/English labels.
- The API serves the built site from `wwwroot` (git-ignored, regenerated by `npm run build`).

## 7. Git, PRs and releases

- Default branch `main` is protected: one approving review is required, and a self-authored PR cannot be approved
  by its author (admin bypass exists). Work on `RS/<topic>` branches and open PRs; merges use merge commits.
- Commit messages use Conventional Commits (`feat(web): ...`, `fix(ci): ...`, `build: ...`, `chore: ...`, `style(web): ...`).
- Commits are authored by the repository owner. **Do not add `Co-Authored-By` trailers or "Generated with ..."
  lines** to commits or PR bodies.
- Do not commit, push, or open PRs unless asked. When a task leaves changes, ask whether to commit, commit and
  push, or open a PR.
- **Pushing or merging to `main` deploys to production** (see below). Treat it as an outward-facing action.
- `main` history has been rewritten twice (to drop AI and bot attribution); never force-push it without explicit approval.

## 8. CI/CD and infrastructure

- `ci.yml` (PRs and non-main pushes): `dotnet test` in Release, `npm ci && npm run build`, `terraform fmt -check`.
- `deploy.yml` (push to `main`, ignoring `*.md`, `docs/`, `original/`, `modern/`, `games/`, `build.bat`): jobs
  **test -> build -> deploy**. The build job has no Google credentials; only the deploy job impersonates the
  deploy service account through Workload Identity Federation (token must match the repository id and
  `refs/heads/main`). Terraform state is in a GCS bucket via a generated, git-ignored `backend_override.tf`.
- Workflow actions are pinned to **full commit SHAs** with a version comment; keep that when updating.
  Dependabot was removed, so action updates are manual.
- The Firebase CLI is pinned and installed with `--ignore-scripts`; Node is 22 in CI. Do not weaken the
  credential-isolation between jobs.
- The container is built with `dotnet publish -t:PublishContainer` (no Dockerfile), chiseled Ubuntu, port 8080,
  `-p:ExcludeWwwroot=true`.
- Run `terraform fmt` on any Terraform change. `terraform.tfvars`, `*.tfstate` and `build/` are git-ignored.

## 9. Dependencies

- NuGet: latest **stable** that supports `net11.0` (check with `dotnet list Fortaleza.slnx package --outdated`).
  Test async calls pass `TestContext.Current.CancellationToken` (xUnit1051). Analyzer packages
  are versioned in `Directory.Build.props`; there is no central package management.
- npm: keep `package-lock.json` in sync; install with `npm ci`.
- After any package change run the full build and tests; analyzer upgrades can introduce new errors.

## 10. Pitfalls learned the hard way

- `dotnet format analyzers/style` code fixes are **not safe to accept blindly**: they once inserted
  `throw new NotImplementedException()` switch arms and a `case X: break;` that changed behavior, converted test
  classes to `internal` (xUnit needs `public`). Review the diffs and run the tests.
- Some enabled analyzers contradict each other (RCS1031/RCS1111, MA0174/MA0175, RCS1231/MA0209); the chosen
  side is encoded in `.editorconfig`. Do not flip it back.
- `Program` exists in both `Fortaleza` and `Fortaleza.Api`; do not give `Fortaleza.Api.Tests` an
  `InternalsVisibleTo` into `Fortaleza` (CS0433 ambiguity).
- README still mentions `.github/dependabot.yml`, which no longer exists.
- No Chrome binary at `/opt/google/chrome`: Lighthouse via the DevTools MCP fails. Chrome is a Flatpak
  (`flatpak run com.google.Chrome --headless=new --remote-debugging-port=9222 ...`) and `npx lighthouse --port=9222` works.
- `games/*.exe` and `build.bat` are Windows/FreePascal only; they are not part of the .NET build.
- `.claude/`, `.remember/` and `docs/superpowers/` are git-ignored agent workspaces: never commit them.
