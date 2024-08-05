using WildPay.Models.Entities;
using WildPay.Models.ViewModel;

namespace WildPay.Models.ViewModels
{
    public class UpdateGroupModel
    {
        public Group? GroupToUpdate { get; set; }
        public MemberAdded? NewMember { get; set; }
    }
}
