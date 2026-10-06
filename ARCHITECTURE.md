# Architecture

NexaForge CMS uses a pragmatic modular-monolith architecture on ASP.NET Core MVC. Administrative capabilities are grouped under the Admin Area while public delivery and authentication remain at the application root.

## Design principles
- Keep domain concepts explicit and easy to navigate.
- Prefer framework-native ASP.NET Core capabilities over unnecessary dependencies.
- Treat uploaded extensions as untrusted input.
- Keep AI integration optional and isolated behind a service.
- Make local onboarding simple while leaving production infrastructure replaceable.

## Main boundaries
**Presentation:** Razor Views + responsive CSS.  
**Application:** MVC controllers orchestrate use cases.  
**Services:** AI, audit, slugging, hooks and extension installation.  
**Persistence:** EF Core `AppDbContext`.  
**Extensions:** manifest-driven package metadata/assets plus registered hook definitions.

## Production evolution
For a production deployment, use migrations rather than `EnsureCreated`, externalize secrets, choose a production database provider, add a background worker for jobs, use object storage for media, add centralized telemetry, and enforce fine-grained policy authorization.
