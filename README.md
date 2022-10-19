# Clay.SmartDoor
This API allows users to open doors and show historical events beyond classical tags

## Database Design
The databse was design to handle numerous relationships. Each user is assigned to an Access group. The Access group will determine the set of doors that the user can open. Each Access Group has a collection of Doors. 
The aplication is permission based to handle complex criterias for adding new users, assigning users to a group, adding doors to groups and so forth.
The application has 3 Roles
### 1. Super Admin
### 2. Admin
### 3. Basic

### Super Admin
Handles creation of permissions and assigning permissions to users. 
### Admin
Can perform certain administrative operations (e.g. Add User) based on the permissions granted. Can have access to Secure doors if he/she belongs to the
Secure group.
### Basic
Can access the main door and any other door that falls in the Genral group.


## Architecture
This project adopted the Clean Architectural pattern as it encapsulates the business logic but keeps it separate from the delivery mechanism. This makes the code easier to manage and with better performance.

## Design Pattern
I have adopted the UnitOfWork design pattern. Since more than one database interaction can happen in a single request, the unit of work helps to ensure
that certain operations are handled as a single operation and would rollback if an operation fail. This way, the integrity of the system will be maintained and the activity logging can be trusted.

## Database
Azure database for MySql was used as most of the entities are relational. It's a fully managed database as a service offering that can handle mission-critical workloads with predictable performance and dynamic scalability.

