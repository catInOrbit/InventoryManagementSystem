# InventoryManagementSystem

### Overview

Maintain by: https://github.com/catInOrbit (Mao Nguyen Minh Tam)
A backend project built in .NET core with Entity Framework for FPT Final Capstone project 2021: Inventory Management System

### Architecture:
Project is organized following CLEAN architecture from Microsoft 

https://docs.microsoft.com/en-us/dotnet/architecture/modern-web-apps-azure/common-web-application-architectures

Architecture Diagram:
![](https://drive.google.com/file/d/1-hQ6cJdW4CuDxMiGPY3eHeBG82x0PKx3/view?usp=sharing)

Visualization: 

### Tech stack:
- .NET Core 5 with Entity Framework
- Authorization with Identity Framework
- Elasticsearch service for searching and indexing
- SignalR for real time notification
- BigQuery for data warehouse solution, visulized wih Google Data Studio
- Redis cache

Designed for deployement on Heroku Cloud

### Feature:
- Role based management for specified roles: Manager, Accountant, SalePerson, StockKeeper
- Optimize searching with Elasticsearch
- Management of Inbound and Outbound order: Requisition Order, PriceQuote Order, Purchase Order, GoodsReceipt Order, StockTake Order, GoodsIssue Order
- Management of Products for creating, updating properties
- Management of Location (assume single shelf location with no constraint on maximum storage threshold) for stock taking purpose
- Management of accounts for authorized role
- Real time notification with SignalR
