using Clay.SmartDoor.Core.Entities;

namespace Clay.SmartDoor.Core.Interfaces.InfrastructureServices
{
    public interface IDoorAssignmentRepository : IGenericRepository<DoorAssignment>
    {
        /// <summary>
        /// Asynchronously returns the <see cref="DoorAssignment"/> that matches the given <paramref name="doorId"/>
        /// and <paramref name="accessGroupId"/> and status is Active.
        /// <para>This is a read-only operation and any changes done will not be 
        /// persisted to the database.</para>
        /// </summary>
        /// <param name="doorId"></param>
        /// <param name="accessGroupId"></param>
        /// <returns>A Task that represents the asynchronous operation. 
        /// The task result contains the <see cref="DoorAssignment"/> that matches 
        /// the given condition, or null if no such door exist.</returns>
        Task<DoorAssignment?> GetDoorAssignmentAsync(string doorId, string accessGroupId);

    }
}
