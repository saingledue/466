# ABABI_BACKEND
Deployment Instructions


Download Visual Studio Community version if you don’t already have a version of visual studio installed. 

Additionally Download .NET Version 6.0

Copy the repository and unzip it to a folder in your repository

Open the solution file in visual studio. To test out the API, you can press the 466Backend run button in the top center of the screen. This will launch a local version of the web API. Currently, the web API is set up to contact our database for production. This will likely cause an error as the IP address you are navigating from will not have access to the database. To fix this, navigate to appsettings.json file in the main solution file. This will likely appear below the Utilities Folder in the file structure in visual studio. In this file edit the string in the line that says 466Database under the connection strings. Change this to the connection string of your database. For us this was given by azure in the main settings page of your database. If you decide to use a local database you can use the following line as a template for setting up your connection string. Our testing used visual studio’s local SQL server and we created a new database by connecting to it using SQL Server Management Studio. You should only have to change the server name which is what Data Source equals and the database name which you modify with the Database attribute.

"466Database": "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=master;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False;Database=466Database"

Now that we have our connection string set up we can set up the tables in our database. Navigate to the terminal in visual studio which can be found under in through View->Terminal. Make sure you are in the block-backend folder. This should be set up by default. Run the following command.

dotnet ef database update

This should create all of the tables you need in your database using the migrations feature in entity framework. 

Next, you can adjust any cors settings you would like to program.cs file on line 20, but we have decided to set it to allow all origins for testing purposes.

Assuming you have all of these settings changed, you are now ready to deploy. You can test the api by now running the command in the top middle as we talked about earlier. If you want to test the web api locally, you can simply run this project in visual studio. If you wish to deploy this project, you can go to azure and create an app service for this web api. Upload your custom folder with the changed appsettings file to your own github and use azure to automatically build the application. Once you get this azure resource set up, it should be accessible anywhere.
