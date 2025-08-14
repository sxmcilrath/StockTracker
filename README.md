# WIP! - StockTracker - Stock Volume Alert Web App 

## Overview
This project is a **Blazor Web App** focused on allowing users to setup alerts for when stocks cross a certain 
volume threshold. 


## Features
- **Stock Data Retrieval** - automated process routinely grabs stock data from the nasdaq via csv download http request
- **Data Transformation** - Stock checker service reads through the data, removes columns it doesn't need and standardizes volume for stock model.
- **Data Storage** - transformed data then initialized into stock DTO which is upsert into a Dockerized PostgreSQL table. 
- **Email Alert** - Configured SendGrid API to email user if retrieved data exceeds specified limit. Current functionality is hardcoded.
  ### WIP
  - Front end
  - Alert DTO
  ### Legacy Implemntation
  - Originally used flask API to go through saved stock tags and grab scrape stock data


## Technologies Used
- **C#**
- **ASP .NET Core**
- **PostgreSQL**
- **Docker**
- **SendGrid**

