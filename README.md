# Huskar

This application, named after the champion from Dota 2, Huskar, <br>has following functionalities, 
using the <a href="themoviedb.org">themoviedb.org</a> API:

* As a visitor I should be able to view latest movies / top movies<br>
* As a visitor I should be able to search movies by name and/or genre.<br>
* As a visitor I should be able to view movie details (Fetch image gallery, actors, description)<br>
* As a locally registered user I should be able to leave a comment on a movie in a local comments database.<br>

## Notes
> User authentication is done externally, based on social media accounts (Facebook & Google)<br>
> Movie data is not stored in the database, it is retrieved using the existing API.<br>
> No passwords or other sensitive data are stored in the database of users connected <br>through Facebook and Google accounts.

## Project Setup
<b>Package Manager Console</b>
```powershell
PM> Update-Database
PM> Remove-Migration
```
<br/>
<b>If you use CLI, open up terminal</b>:

```shell

> dotnet ef database update
> dotnet ef migrations remove

```
