# Landing API

Some api for landing, such as sincact as form.

## Configure

```js
{
  "ConnectionStrings": {
    "POSTGRES": "Connection string to postgres database"
  },
  "GitHubOptinos": {
    "OAuthToken": "Personal OAuth token" // Place it in appsettings.Local.json while develop
  },
  "ScrubOptions": {
    "Delay": "01:00:00" // TimeSpan delay between scrubs
  }
}
```

## Build for production

1. Create env variable with [OAuth token](https://github.com/settings/tokens)

```bash
# powershell
$Env:GITHUB_OAUTH_TOKEN="abcdefg..."
# bash
export GITHUB_OAUTH_TOKEN="abcdefg..."
```

2. Generate stack file

```bash
docker-compose -f ./docker-compose.yml -f ./docker-compose.prod.yml config > stack.yml
```

3. Use stack file for `docker-compose` or `swarm`

## Develop

Restore tools
```bash
dotnet tool restore
```

Add new migration
```bash
cd Database
dotnet ef migrations add MIGRATION_NAME --startup-project ../Landing.API
```