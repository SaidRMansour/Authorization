# Secure Software Development - Authorization API

This project is a proof of concept for implementing fine-grained authorization controls in a news site backend system. The system manages articles, comments, and user roles, providing different levels of access and manipulation capabilities based on the user's role.

## Technology Stack

- .NET Core 8.0.203
- SQLite 3.41.2
- Entity Framework Core

## Setup

To get started with this project, follow these steps:

### Prerequisites

Ensure you have the following installed on your system:
- .NET SDK 8.0.203
- SQLite 3.41.2

### Database Setup

Run the following commands to create and seed the database:

```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

## Running the Application

To run the application, use the following command:

```bash
dotnet run
```

The API will be available at http://localhost:7120. (Refer to the launchSettings.json file to verify the correct port)

## API Endpoints

The application supports the following endpoints:

* **GET /articles** - Retrieves all articles. Available to any role.
* **POST /articles** - Creates a new article. Available to Journalists.
* **PUT /articles/{id}** - Edits an existing article. Available to Journalists for their own articles and to Editors for any articles.
* **DELETE /articles/{id}** - Deletes an article. Available only to Editors.
* **GET /articles/comments** - Retrieves all articles. Available to any role.
* **POST /articles/comments** - Adds a comment to an article. Available to Subscribers and higher roles.
* **PUT /articles/comments/{id}** - Edits an existing article. Available to Journalists for their own articles and to Editors for any articles.
* **DELETE /articles/comments/{id}** - Deletes a comment. Available only to Editors.

## Authorization Policy

* Editors can **edit** and **delete** any articles and comments.
* Journalists can **create** and **edit** their own articles but cannot delete them.
* Subscribers can **comment** on articles.
* Guests can only **read** articles.

## Using the API

A Postman collection with relevant requests is available to demonstrate API usage.


## License

This project is licensed under the MIT License - see the LICENSE file for details.