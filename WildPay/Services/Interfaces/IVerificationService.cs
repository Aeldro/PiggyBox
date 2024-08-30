using WildPay.Models.Entities;

namespace WildPay.Services.Interfaces
{
    public interface IVerificationService
    {
        public bool IsUserBelongsToGroup(string userId, Group group);

    }
}
