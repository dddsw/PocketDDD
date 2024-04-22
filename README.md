# PocketDDD

# Running terraform locally
Ensure the Azure, GitHub, and terraform CLIs are installed
```
brew install azure-cli
bre install gh
brew install terraform
```

Login to Azure and GitHub
```
az login
gh auth login
```

From the `terraform` directory run init, plan, then apply if happy with the changes.
```
cd ./terraform

terraform init
terraform plan -var-file ../tfvars/dev.tfvars
terraform apply -var-file ../tfvars/dev.tfvars
```