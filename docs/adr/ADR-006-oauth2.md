# ADR-006 — Keycloak OAuth2/OIDC with JWT bearer API validation
Status: Accepted

## Decision
- Keycloak is the external authorization server/identity provider; ProductInventory does not implement user registration, password storage or login endpoints.
- API validates signed JWT access tokens using ASP.NET Core JWT bearer middleware: signature, issuer, intended audience and lifetime; enforce HTTPS outside local development.
- Every business endpoint requires authenticated access; missing/invalid/expired token returns 401; valid token without required authorization (if a policy is later introduced) returns 403.
- Swagger UI uses Authorization Code with PKCE and a public client configured for the correct redirect URI. Use a separate API resource/audience configuration so the access token is intended for ProductInventory.
- Compose may import a **development-only** Keycloak realm/client configuration with no production credentials. No client secret in Swagger's browser-based public client.
- Prefer discovery/JWKS over hardcoded signing keys; ensure issuer URL resolves consistently from API containers and browser clients. Verify token issuer/audience against the actual imported realm.

## Consequences
The environment has a third container. No RBAC matrix or permissions administration is in MVP scope.
