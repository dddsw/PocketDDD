# PocketDDD

# Populating Sessionize data
Auto-populating the session data from Sessionize requires a basic set of seed data. An example of this can be found in [2024_SeedData.sql](PocketDDD.Server/PocketDDD.Server.DB/Migrations/2024_SeedData.sql)

Once this script has been run against the DB you can call the admin endpoint to refresh the data. This requires the Admin API key which can be retrieved from Azure key vault secret `pocketddd-<env>-admin-api-key`
```
POST /api/eventdata/RefreshFromSessionize HTTP/1.1
Host: pocketddd-dev-api-server.azurewebsites.net
Authorization: <insert-admin-key>
```

# Running terraform locally
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