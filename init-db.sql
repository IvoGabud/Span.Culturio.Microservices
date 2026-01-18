IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'Culturio.Users')
    CREATE DATABASE [Culturio.Users];
GO

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'Culturio.CultureObjects')
    CREATE DATABASE [Culturio.CultureObjects];
GO

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'Culturio.Packages')
    CREATE DATABASE [Culturio.Packages];
GO

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'Culturio.Subscriptions')
    CREATE DATABASE [Culturio.Subscriptions];
GO
