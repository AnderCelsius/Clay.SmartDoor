using Clay.SmartDoor.Core.Entities;
using Clay.SmartDoor.Core.Interfaces.InfrastructureServices;
using Clay.SmartDoor.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Clay.SmartDoor.Infrastructure.Repositories
{
    internal class DoorRepository : GenericRepository<Door>, IDoorRepository
    {
        private readonly SmartDoorContext _context;

        public DoorRepository(SmartDoorContext context) : base(context)
        {
            _context = context;
        }

        /// <summary>
        /// Asynchronously returns the door that matches the given <paramref name="doorId"/>
        /// <para>This is a read-only operation and any changes done will not be 
        /// persisted to the database.</para>
        /// </summary>
        /// <param name="doorId"></param>
        /// <returns>A Task that represents the asynchronous operation. 
        /// The task result contains the <see cref="Door"/> that matches 
        /// the given <paramref name="doorId"/>, or null if no such door exist.</returns>
        public async Task<Door?> GetDoorAsync(string doorId)
        {
            return await _context.Doors.AsNoTracking().SingleOrDefaultAsync(d => d.Id == doorId);
        }

        public IQueryable<Door> GetAllDoorsAsync()
        {
            return _context.Doors.AsNoTracking().OrderByDescending(d => d.CreatedAt);
        }
    }
}
