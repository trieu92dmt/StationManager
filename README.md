## StationManager - API:

## References URLs
- Docs: https://aspnetboilerplate.com/Pages/Documents/NLayer-Architecture
- Architecture: https://learn.microsoft.com/en-us/azure/architecture/guide/architecture-styles/microservices
- Templates: https://github.com/yanpitangui/dotnet-api-boilerplate

## Prepare environment, Tools
- .NET 6.0
- Visual Studio 2022
- SQL server 2017

## Pakages References
- CQRS Pattern 
- SwaggerUI
- EntityFramework
- AutoMapper
- Generic repository pattern
- Serilog with request logging and easily configurable sinks
- .Net Dependency Injection (Auto)
- Identity server
- MediatR

## Project main structure
1. Services
	-Thư mục chức các project chạy chính, đây là tầng cao nhất để giao tiếp trực tiếp với FE
		+ Controllers: Chứa các controller, các HTTP request
		+ Configure: Bắt exception hệ thống
		+ Infrastructure : Chứa các cấu hình cho hệ thống: Automapper, authentication, database, serilog
		+ appsetting.json: File chứa các chỉ số cấu hình hệ thống
2. Application
	- Folder chứa tất cả các logic chính của hệ thống
		+ Commands: Business chính của từng api
		+ Query: Get data to database
		+ Services: Chứa các logic có thể dùng chung giữa các command.

