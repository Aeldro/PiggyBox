using System.Drawing;
using WildPay.Models.Entities;

namespace WildPay.Models.ViewModels
{
    public class UpdateGroupModel
    {
        public Group? GroupToUpdate { get; set; }
        public GroupInfos? InfosToUpdate { get; set; }
        public MemberAdded? NewMember { get; set; }
    }
}
