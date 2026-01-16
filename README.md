**CartonCaps.Referrals API**

**HatchWorks AI .NET Developer Code Challenge**

*“I have no special talents. I am only passionately curious.”*\
**Albert Einstein**

Throughout my professional career, I have discovered and learned different ways to solve a problem and find multiple solutions. Over time, I have learned that there is no single correct answer; what is truly valuable is the experience gained, as it gives you the judgment to know which option is the best, or the humility to seek and learn new approaches that adapt to current needs and allow evolution when circumstances change.

**Technical Analysis**

Since my interest is to demonstrate my technical knowledge in this challenge, I attempted to implement various techniques, design patterns, and best practices that I have adopted across the different projects I have worked on throughout my professional career, as well as the use of new syntax that enables cleaner and more readable code in .NET.

- **SOLID principles**

For this solution, I decided to use a **traditional N-Layer architecture**, meaning the decoupling of the application into presentation, business, and data layers, in addition to an extra domain layer for handling entities, DTOs, and cross-cutting models within the solution.

**Project Structure**

The projects that make up the solution are the following (please define **CartonCaps.Referrals.Api** as the startup project):

All API projects and libraries are developed in **.NET 8.0**.

The solution contains two **RESTful API** projects:

- **CartonCaps.Referrals.Api**
- **CartonCaps.Referrals.ApiMock**

Both projects are practically identical; the difference is that one requires parameters to consume the service and the other does not.

<img width="320" height="293" alt="Projects" src="https://github.com/user-attachments/assets/7205f4e4-d5a0-4b47-acf5-62e63455a5b4" />



**Database Management (SQLite)**

For practicality reasons in this technical challenge, I configured the **DbContext** using **SQLite**, so that when the project is executed, the **referrals.db** file is automatically created.\
Additionally, **AutoMapper** is used for mapping between entities and DTOs.

-----
**Versioning and Documentation (OpenAPI and Swagger)**

I have experience documenting APIs using **OpenAPI**, through **YAML** or **JSON** files; however, for this challenge I configured **OpenAPI and Swagger** to load the documentation from the **.xml** file generated from the comments registered within the solution.\
Likewise, API service versioning has been implemented.

-----
**Unit Testing (NUnit)**

The implemented tests are written using **NUnit**, although I also have experience working with **MSTest**.

-----
<img width="1441" height="583" alt="Api" src="https://github.com/user-attachments/assets/67e7e659-33d2-4c6b-926b-0d7b82e5df64" />

**Notes**

- I used **GenAI** exclusively for text generation and API documentation; it was not used for code generation.
- I leveraged solutions that I have already implemented and refined throughout my professional career.
- Based on my experience, I implemented **clean code** best practices, supported by the intelligence tools in **Visual Studio 2022** and the capabilities of the **.editorconfig** file, which I consider key strategies for maintaining high code quality and up-to-date best practices. These rules are usually configured at the team level to maintain consistency and quality during code review processes.
-----
I hope this README has been clear and that you enjoy this small sample of my work and way of thinking.

I will gladly be available for any questions that may arise and will do my best to address them.

**Thank you very much!**
