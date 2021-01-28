# Landing API

Some api for landing, such as sincact as form.

## Configure

/Landing.API/appsettings.Production.json:
```js
{
  "ConnectionStrings": {
    "POSTGRES": "Connection string to postgres database"
  },
  "GitHubOptinos": {
    "OAuthToken": "Personal OAuth token" // Place it in appsettings.Local.json
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
docker-compose -f ./docker-compose.yml -f ./docker-compose.production.yml config > stack.yml
```

3. Use stack file for `docker-compose` or `swarm`