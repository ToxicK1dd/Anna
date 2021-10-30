# [Anna](https://www.youtube.com/watch?v=1XK5-n4rR7Q)
C# Discord bot made with [DSharpPlus](https://github.com/DSharpPlus/DSharpPlus).

## Requirements
* .NET 6
* MySQL Database
* A working PC

## Setup
The bot is designed to be easily setup, and run.</br></br>Configure the following in appsettings.json:
1. **EF Core:**
    * Add a connection string for connecting to a MySQL database
2. **Config:**
    * Add Discord bot token
    * Set bot prefix
3. **Channels:**
    * Watchdog: Support Server Watchdog Log Channel Id
    * ErrorLog: Support Server Error Log Channel Id
    * Log: Support Server Log Channel Id
    * Feedback: Support Server Feedback Channel Id
    
**Remember to also run the Entity Framework migrations to create your database.**

## Used libraries
* [DSharpPlus](https://github.com/DSharpPlus/DSharpPlus)
* [Autofac](https://github.com/autofac/Autofac)
* [ASP.NET Core](https://github.com/dotnet/aspnetcore)
* [Entity Framework Core 5](https://github.com/dotnet/efcore)
* [Pomelo Entity Framework Core MySql](https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql)
* [Newtonsoft Json](https://github.com/JamesNK/Newtonsoft.Json)
* [RestSharp](https://github.com/restsharp/RestSharp)
* [QRCoder](https://github.com/codebude/QRCoder)

## Disclaimer
This bot is a personal hobby project, no help will be provided, and no feature requests will be accepted.</br></br>
The project also requires an intermediate to higher understanding of C#,</br>
and OOP practices, due to the use of DSharpPlus, EF Core, and Autofac.</br></br>
For help regarding any of the libraries used, check out their own documentation.

## License
This project is [MIT licensed](https://github.com/ToxicK1dd/Anna/blob/master/LICENSE)
