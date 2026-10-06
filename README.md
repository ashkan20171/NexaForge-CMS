# NexaForge CMS

**AI-assisted, extensible enterprise content management platform built with ASP.NET Core MVC.**

NexaForge CMS is a portfolio-grade CMS focused on clean architecture, editorial productivity, safe extensibility and bilingual user experience. It combines a familiar WordPress/Joomla-inspired content workflow with a modern ASP.NET Core MVC codebase that is intentionally readable and practical to extend.

> Portfolio release candidate · .NET 8 · ASP.NET Core MVC · EF Core · SQLite · RTL/LTR · AI-assisted editorial tools

## Why this project matters

This repository demonstrates more than CRUD screens. It brings content authoring, governance, extension packaging, operational visibility and security-oriented engineering into one coherent MVC application. The project is especially intended to demonstrate backend/full-stack .NET engineering skills to European product teams.

## Engineering highlights

- **ASP.NET Core MVC on .NET 8** with clear Models, Views, Controllers and service boundaries.
- **EF Core data layer** with SQLite for frictionless local onboarding and a provider-friendly design for production databases.
- **Bilingual administration** with persistent English/Persian switching and LTR/RTL-aware UI behavior.
- **AI Content Studio** for rewriting, summaries, SEO assistance, analysis and translation workflows, with optional OpenAI-compatible provider configuration.
- **Professional editor** with autosave foundation, draft recovery, live responsive preview, SEO meter, AI toolbar and revision restore/compare workflows.
- **Open Extension Platform** for manifest-based Plugin, Theme and Module ZIP packages, including traversal checks and executable/server-binary blocking.
- **Editorial governance** with roles, permission grants, review workflow, revisions, trash/restore, audit trail and scheduled publishing foundation.
- **Platform operations** with analytics, login history, error center, system health, API tokens, webhooks, diagnostics export and reliability dashboard.
- **Creator tooling** with Page Builder, Widgets, Theme Customizer, Child Themes, Developer Kit, Hook registry and extension compatibility/update foundation.
- **SEO & delivery** with redirects, sitemap, robots.txt, public content API and PWA/offline-shell foundation.
- **Business modules** including Forms, Submission Inbox, CRM Lite and Newsletter subscribers.

## Feature map

| Area | Capabilities |
|---|---|
| Content | Posts, Pages, Categories, Tags, Comments, Media, Menus, Widgets |
| Authoring | Autosave, Live Preview, Revisions, Trash/Restore, AI toolbar, SEO feedback |
| AI | Rewrite, Summarize, Translate, SEO suggestions, Content analysis, History |
| Extensions | Plugins, Themes, Modules, ZIP installer, Manifest validation, Hooks, Child Themes |
| Governance | Users/Roles, Permission Matrix, Workflow, Audit Log, Scheduled Jobs |
| Growth | Forms, Leads/Submissions, CRM Lite, Newsletter, Analytics |
| Platform | API Tokens, Webhooks, Redirects, Backup, Health, Diagnostics, Error/Login history |
| UX | Responsive admin, Dark Mode, collapsible/scrollable sidebar, FA/EN, RTL/LTR |

## Architecture

```text
AshkanCMS/
├── Areas/Admin/          # Administrative modules and views
├── Controllers/          # Public/auth/localization/API controllers
├── Data/                 # EF Core context + seed data
├── Models/               # Content, administration and platform models
├── Services/             # AI, audit, slug, hooks and package installation services
├── Views/                # Public site and authentication UI
└── wwwroot/              # CSS, uploads, extension assets and PWA shell
```

The extension boundary deliberately favors **manifest/assets and registered hooks** over blindly loading uploaded assemblies. That keeps the demo understandable and reduces the risk of arbitrary server-side code execution from uploaded packages.

## Local setup

### Requirements
- Visual Studio 2022 (17.8+) or .NET 8 SDK
- No external database required for the default local configuration

### Run
```bash
dotnet restore
dotnet run
```

Or open `AshkanCMS.sln` in Visual Studio and run the web project.

Initial local administrator:

```text
Username: admin
Password: Ashkan@123
```

**Change the seeded password before any non-local deployment.** Never commit production API keys or credentials. Prefer environment variables, .NET User Secrets or your deployment platform's secret store.

## Useful routes

- `/Admin` — administration dashboard
- `/Admin/Studio` — AI Content Studio
- `/Admin/Creator` — Creator & Developer Hub
- `/Admin/Extensions` — Extension Marketplace
- `/Admin/Reliability` — Reliability & Operations Center
- `/health` — lightweight health endpoint
- `/api/content/posts` — public read-only content API (when enabled)

## Extension package example

```json
{
  "id": "nexaforge-advanced-seo",
  "name": "Advanced SEO",
  "type": "Plugin",
  "version": "1.0.0",
  "author": "Your Name",
  "description": "SEO extension for NexaForge CMS"
}
```

Package the manifest and approved assets in a ZIP and install it from the Extension Marketplace. The installer validates package size/path safety, duplicate IDs and blocked executable/server-binary types.

## Security notes

The portfolio release includes cookie authentication, authorization boundaries, anti-forgery validation on state-changing forms, password hashing, API token hashing, login/security event history, extension package validation and baseline browser security headers. See `SECURITY.md` for deployment guidance and the project's security scope.

## Reliability & operations

Stage 15 introduces an administrator-only Reliability & Operations Center that summarizes application errors, authentication signals, scheduled work and extension status. A sanitized JSON diagnostics report can be exported without secrets for support and troubleshooting.

## Roadmap

- Integration and end-to-end test suite
- Production database migration templates (SQL Server/PostgreSQL)
- Background worker implementation for scheduled jobs
- Object-storage media provider and image transformation pipeline
- Fine-grained policy authorization replacing remaining role-only checks
- Signed extension packages and remote update feed
- Distributed cache and observability exporters
- Container/deployment examples and CI pipeline

## Recruiter / reviewer notes

Interesting areas to review first:
1. `Areas/Admin/Controllers/EditorController.cs` — authoring and revision workflows.
2. `Services/PackageInstallerService.cs` — safe extension package boundary.
3. `Services/AiAssistantService.cs` — local/remote AI assistance abstraction.
4. `Areas/Admin/Controllers/CreatorController.cs` — migration and creator tooling.
5. `Areas/Admin/Controllers/ReliabilityController.cs` — operational diagnostics.
6. `Areas/Admin/Views/Shared/_AdminLayout.cshtml` — responsive bilingual admin shell.

## License

Portfolio/demo project. Add the license that matches your intended public distribution before publishing the repository.

## SQLite schema upgrades (Stage 15.2)

Older NexaForge/Ashkan CMS stages used `EnsureCreated()`. EF Core does not add newly introduced tables to an already existing database when `EnsureCreated()` is called again. Stage 15.2 adds a startup compatibility bootstrapper for SQLite: it preserves the existing database and replays EF Core's generated `CREATE TABLE/INDEX` statements as `IF NOT EXISTS` statements before seed data runs. This fixes upgrades where newer modules such as `SystemPreferences`, permissions, login history, error logs, child themes, autosaves or import jobs were missing from an older database.

For a long-lived production product, use versioned EF Core migrations and tested deployment migrations. The compatibility bootstrapper is intentionally scoped to this portfolio project's SQLite upgrade path.

## Stage 16 — Production Engineering Edition

Stage 16 adds an administrator-only **Production Engineering Center** that turns several operational foundations into visible, reviewable workflows: a maintenance/background-work queue, portable content snapshots, API Explorer, media-footprint visibility, webhook-delivery readiness, and a production-readiness checklist. The SQLite compatibility bootstrapper from Stage 15.2 remains in the startup path so newly introduced Stage 16 tables can be added to older local databases without deleting content.

### Engineering decisions worth discussing in an interview

- **Safe extension boundary:** uploaded packages are treated as manifests/assets rather than blindly loaded server assemblies.
- **Upgrade compatibility:** local SQLite installations are upgraded conservatively while the roadmap explicitly recommends versioned EF Core migrations for production.
- **Secret-aware backups:** portable content snapshots intentionally exclude password hashes, API tokens and AI credentials.
- **Operational transparency:** health, diagnostics, login/error history and engineering dashboards expose support signals without claiming a full observability stack.
- **Progressive platform design:** background tasks and webhook deliveries are modeled explicitly so a durable worker/retry implementation can replace the portfolio queue without rewriting the admin UX.

### Suggested GitHub repository name

`NexaForge-CMS`

### Suggested GitHub description

> AI-powered, extensible enterprise CMS built with ASP.NET Core MVC — bilingual RTL/LTR authoring, modular plugins & themes, editorial workflows, SEO, analytics, security and production-engineering tooling.
"# NexaForge-CMS" 
