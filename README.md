# Clay.SmartDoor
This API allows users to open doors and show historical events beyond classical tags

## Database Design
The databse was design to handle numerous relationships. Each user is assigned to an Access group. The Access group will determine the set of doors that the user can open. Each Access Group has a collection of Doors. 
The aplication is permission based to handle complex criterias for adding new users, assigning ussers to a group, adding doors to groups and so forth.
The application has 3 Roles
### 1. 

## Architecture
This project adopted the Clean Architectural pattern as it encapsulates the business logic but keeps it separate from the delivery mechanism. This makes the code easier to manage and with better performance.

## Design Pattern
I have adopted the UnitOfWork design pattern. 

