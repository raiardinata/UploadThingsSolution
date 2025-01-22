A gRpc and RESTful project using .NET 9.

This project is a playground for me to test gRpc while also have the capability as a RESTful using jsonTranscoding.

Project structure :
Application
|_Images -- contain all image for the project
|
|_Services -- contain all the services

Domain
|
|_Entities -- database model of your table
|
|_Interfaces -- interfaces and abstraction of your system

Infrastructure
|
|_Data -- connection to your databases
|
|_Repositories -- Strategy Design Pattern being used in here. Define your system behaviour in here.
|
|_UnitofWork.cs -- Unit of Work Design Pattern syntax.

Migration -- Migration folder. All of your Entity Framework migration is here.

Presentation
|
|_Protos -- your proto behaviour in here
|
|_google -- gRpc library in here

Properties -- launch settings
bin -- build folder
obj -- system setting and nuget package in here
