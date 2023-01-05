# MilasMatheTurnier

### Simple Konsolenapplikation mit Mathematikaufgaben für Grundschulkinder und darüber hinaus!
### Mit persistenter Bestenliste ohne jegliche Securitymaßnahmen ;-------)))
---
### Specs
#### Database: Two separate one type .json-Files (Userprogressions, Application-Configurations) -> Tested with huge dummydata collections
#### Backend: Async operations on repositories & datamodels in plain C# -> Now with decoupled structure for easier future updates
#### Middlelayer: Calculations & userinputvalidations in plain C# -> Also async where I think it is necessary
#### Frontend: Simple console application -> Classic procedural style in plain C#
---
### What's planned?
#### Persistent Userconfigurations like constraints for each eage, etc. to be configured inside the application only through secured "kind-of-supervisor"-useraccount
#### Options for security regarding usercredentials and the database via utilizing SecureString type and the .NET Cryptography Library
---
#### Applicationculture is currently german only!
