# Clay.SmartDoor
This API allows users to open doors and show historical events beyond classical tags

## Project Summary

There are 5 core entities for this project
* AppUser
* Door
* AccessGroup
* DoorAssignment
* ActivityLog

The AccessGroup controls the overrall flow. It determines what set of doors a user can have access to. It is identified by an id and a unique name.
It contains a collection of Users and a collectiion of DoorAssignment.

A User belongs to only one AccessGroup and in extension, has access to the doors in the group. He is not authorized to access any other door outside his Access Group.

## Database Design

The databse used in this project is MySql. I chose SQL over NoSql because of the following reasons
* NoSql does not support relationships between data types.
* Sql are a better fit for heavy transactions beacuse it's more stable and ensures data integrity.

The databse was design to handle numerous relationships. 
* Each user is assigned to an Access group. 
* Access groups determine the set of doors that a user can open. 
* Each Access Group has a collection of Doors. 

## Authorization

The aplication is permission based to handle complex criterias for adding new users, assigning users to a group, adding doors to groups e.t.c.
There are 3 roles in this application
* Basic User
    * By default, this user has access to the main door only.
* Admin User
    * This user can perform certain administrative operations (e.g. Add User) based on the permission granted. 
    * This user may have access to a secure spaces if added to the Access Group where the secure space belongs.
* Super Admin
    * This user has all permissions and can grant and revoke permissions from other users.

## Architecture 

This project adopted the Clean Architectural pattern as it encapsulates the business logic but keeps it separate from the delivery mechanism. This makes the code easier to manage and with better performance. Some of the benefit of using clean architecture are
* Independent of Database and Frameworks. The database used can be changed at anytime without making any change to the UI
* The UI can change easily, without changing the rest of the system and business rules.
* Testability.It is highly testable. Business rules canbe tested without considering UI or database.

## Design Pattern
I have adopted the UnitOfWork design pattern. Since more than one database interaction can happen in a single request, the unit of work helps to ensure
that certain operations are handled as a single operation and would rollback if an operation fail. This way, the integrity of the system will be maintained and the activity logging can be trusted.


