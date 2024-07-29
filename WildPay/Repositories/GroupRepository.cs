using Microsoft.EntityFrameworkCore;
using WildPay.Data;
using WildPay.Interfaces;
using WildPay.Models.Entities;

namespace WildPay.Repositories
{
    public class GroupRepository : IGroupRepository
    {
        private readonly WildPayDbContext _context;

        public GroupRepository(WildPayDbContext context)
        {
            _context = context;
        }

        private WildPayDbContext Get_context()
        {
            return _context;
        }

        public async Task<List<Group>> GetGroupsAsync()
        {
            List<Group> groups = await _context.Groups.ToListAsync();
            return groups;
        }

        public async Task<Group?> GetGroupByIdAsync(int groupId)
        {
            Group? group = await _context.Groups.FindAsync(groupId);
            return group;
        }

        public async Task EditGroupAsync(Group group)
        {
            if (await _context.Groups.FindAsync(group.Id) is Group found && found != null)
            {
                found.Name = group.Name;
                found.Image = group.Image;

                await _context.SaveChangesAsync();
            }
        }

        public async Task AddGroupAsync(string name, string image)
        {
            Group newGroup = new Group()
            {
                Name = name,
                Image = image
            };

            await _context.Groups.AddAsync(newGroup);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteGroupAsync(int groupId)
        {
            if (await _context.Groups.FindAsync(groupId) is Group found)
            {
                _context.Groups.Remove(found);

                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }
    }
}
