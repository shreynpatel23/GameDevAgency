# GameDevAgency Website

Welcome to the GameDevAgency website! This platform is designed for game developers and enthusiasts to explore, manage, and interact with various game projects, genres, users, and activities.

## Features

### Admin User
- **Game Projects Management**: Create, read, update, and delete game projects.
- **Genres Management**: Create, read, update, and delete genres.
- **Link Games with Genres**: Associate games with corresponding genres.
- **User Management**: Add users to the application.
- **Activity Management**: Create, read, update, and delete activities.
- **Link Activities with Users and Game Projects**: Associate activities with specific users and game projects.

### Anonymous User/Developer
By default when a new user is created they will be assigned a developer role. You can change the role by modifying the ASP.NET_USerRoles Table by mapping the role id and user id
- **View Game Details**: Browse and view details of various game projects.
- **View Genre Details**: Explore genres and see which games fall under each genre.
- **View User Details**: Access the list of users and their information.
- **View Activity Details**: See the list of activities and details for each activity.

## Access Information

### Temporary Login Credentials
- **Admin Access**:
  - Username: Christine_HTTP
  - Password: Test@12345

- **Developer Access**:
  - Username: yash_13
  - Password: Test@12345


## Getting Started

To run TaskSync locally, follow these steps:

1. Clone the repository to your local machine.
2. Navigate to the project directory.
3. Go To Project > GameDevAgency Properties > Change target framework to 4.7.1 -> Change back to 4.7.2
4. Please make sure there is an App_Data folder in the project you can check that by View -> Solution Explorer
5. Navigate to Tools > Nuget Package Manager > Package Manage Console
6. Run Update-Database
7. Check if the database is created by clicking on View -> SQL Server Object Explorer
8. Run API commands through CURL commands in CMD, Git Bash

Thank you for using GameDevAgency! 🚀
