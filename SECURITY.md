# Security Policy

## Portfolio security baseline
NexaForge CMS demonstrates anti-forgery validation for state-changing MVC forms, cookie authentication, password hashing, role/permission foundations, API token hashing, login/security history, package path validation, blocked executable uploads and baseline browser security headers.

## Before production
- Replace the seeded administrator password immediately.
- Store AI/API credentials in environment variables, User Secrets or a managed secret store.
- Use HTTPS and configure secure cookie policy for the deployment environment.
- Replace `EnsureCreated` with reviewed EF Core migrations.
- Configure rate limiting, reverse-proxy rules, centralized logs and backups.
- Review extension packages before installation; do not weaken the executable/binary restrictions.
- Configure a strict Content Security Policy after auditing all required asset sources.

## Reporting
For a public repository, add a private security contact before accepting external vulnerability reports.
