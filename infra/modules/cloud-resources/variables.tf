# Required variables
variable "env" {}

# Optional variables
variable "namespace" {
  default = "platereader"
}
variable "service" {
  default = "platereader"
}
variable "service_short" {
  default = "ptr"
}
variable "region" {
  default = "WestEurope"
}