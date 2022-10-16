﻿using Clay.SmartDoor.Core.Dtos;
using Clay.SmartDoor.Core.Entities;
using Clay.SmartDoor.Core.Models;

namespace Clay.SmartDoor.Core.Interfaces.CoreServices
{
    public interface IDoorService
    {
        /// <summary>
        /// Creates a new door, persists the activity to the database as a unit of work
        /// operation.
        /// <para>Both the <seealso cref="Door"/> and <see cref="ActivityLog"/>
        /// must be succesfully added before changes are persisted to the databse.
        /// <br></br>If either fails, the entire operation is rolled back.
        /// </para>
        /// </summary>
        /// <param name="door"></param>
        /// <returns>
        /// A response containing the state of the operation.
        /// </returns>
        Task<ApiResponse<string>> CreateNewDoorAsync(CreateDoorRecord door);
    }
}