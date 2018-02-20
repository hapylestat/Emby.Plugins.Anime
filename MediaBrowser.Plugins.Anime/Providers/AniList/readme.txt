Rate Limiting (https://anilist.gitbooks.io/anilist-apiv2-docs/rate-limiting.html):
- Controlled via response headers: X-RateLimit-Limit, X-RateLimit-Remaining, X-RateLimit-Reset, Retry-After

Exception on hitting the limit: HTTP 429

GraphQL resources doc: https://anilist.github.io/ApiV2-GraphQL-Docs/
Interactive shell: https://anilist.co/graphiql