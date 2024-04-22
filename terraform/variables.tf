variable "env" {
  default  = "dev"
  nullable = false
  type     = string
}

variable "sql_db_sku" {
  default = "S0"
  nullable = false
  type = string
}

variable "sql_max_storage" {
  default = "2"
  nullable = false
  type = string
}

variable "api_app_service_sku" {
  default = "B1"
  nullable = false
  type = string
}

variable "api_always_on" {
  default = true
  nullable = false
  type = bool
}