# StockMarket
### About This Project
This is a Web API service of a platform where registered users, with the admin role, can manage stock portfolios and perform buying and selling transactions.

You can access the database design of this project from the following link: 
https://dbdiagram.io/d/StockMarket-6565fa053be1495787e5feb7

All database configurations for the project are within the Migration, and the database is automatically created when the project runs.

### Things you need to know before using the database:
- There are three different roles available in the 'Roles' table. These roles are 'admin,' 'user,' and 'vault.'
- In the 'Transactions' table, it is recommended that the values entered in the 'Type' column be "Buy" and "Sell."

Stock market data is automatically retrieved and added to the database from the following link:
https://bigpara.hurriyet.com.tr/borsa/canli-borsa/