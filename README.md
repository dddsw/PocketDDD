# PocketDDD

A Blazor web app and .NET API to make DDD South West even more awesome for attendees.

## Folders

- `PocketDDD.BlazorClient` - This is the UI
- `PocketDDD.Server` - This is the API
- `PocketDDD.Shared` - This is shared between the UI and the API
- `PocketDDDClient` - This is deprecated, we'll delete it at some point!

## Getting started

You will need: 

- .NET 8
- SQL Server

### Configuring the backend for the first time

- Set up the database
  - Create a new database
  - Run `PocketDDD.Server/PocketDDD.Server.DB/Migrations/FullDBScript.sql`
  - Add a row to the `EventDetail` table with `Id=1, Version=1, SessionizeId=<SessionizeId>`
- Set up the API
  - Set two user secrets for API (see [Microsoft docs](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-8.0&tabs=windows#secret-manager) if unfamiliar with user secrets)
    - `AdminKey` - this is used in an `Authorization` header to access the API
    - `ConnectionStrings:PocketDDDContext` - this is the connection string for the database
  - Start the API
- Set up the data
  - Call the `api/EventData/RefreshFromSessionize` endpoint with the `AdminKey` in an `Authorization` header
  - In the database, tidy up the `Tracks` data and add `TimeSlots` for the breaks

### Running the frontend

The Blazor web app is designed to be run as a static web app. This means that the `appsettings` files are located inside `PocketDDD.BlazorClient/PocketDDD.BlazorClient/wwwroot`. 
There are two settings which can be altered:

- `apiUrl` - the api the app is pointing to
- `fakeBackend` - whether or not to use a local fake api

Beware when running the frontend that it caches on the client. The app is designed to be downloaded once and then largely work offline. 

If testing a new UI change then use a new incognito window or delete local storage for the site


## Populating Sessionize data
Auto-populating the session data from Sessionize requires a basic set of seed data. An example of this can be found in [2024_SeedData.sql](PocketDDD.Server/PocketDDD.Server.DB/Migrations/2024_SeedData.sql)

Once this script has been run against the DB you can call the admin endpoint to refresh the data. This requires the Admin API key which can be retrieved from Azure key vault secret `pocketddd-<env>-admin-api-key`
```
POST /api/eventdata/RefreshFromSessionize HTTP/1.1
Host: pocketddd-dev-api-server.azurewebsites.net
Authorization: <insert-admin-key>
```

## Running terraform locally
Ensure the Azure, GitHub, and terraform CLIs are installed
```
brew install azure-cli
brew install gh
brew install terraform
```

Login to Azure and GitHub
```
az login
gh auth login
```

Retrieve the access key for the terraform state storage account
```
export ARM_ACCESS_KEY=$(az storage account keys list -g pocketddd-terraform-state -n pocketdddterraformstate --query [0].value -o tsv)
```

From the `terraform` directory run init, plan, then apply if happy with the changes.
```
cd ./terraform

terraform init -backend-config="dev.terraform.tfstate"  
terraform plan -var-file ../tfvars/dev.tfvars
terraform apply -var-file ../tfvars/dev.tfvars
```