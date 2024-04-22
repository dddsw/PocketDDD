# PocketDDD

# Running terraform locally
Ensure both terraform and the Azure CLI are installed
```
brew install terraform
brew install azure-cli
```

Login to Azure
```
az login
```

From the `terraform` directory run init, plan, then apply if happy with the changes.
```
cd ./terraform

terraform init
terraform plan -var-file ../tfvars/dev.tfvars
terraform apply -var-file ../tfvars/dev.tfvars
```