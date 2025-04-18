using Microsoft.AspNetCore.Identity;

namespace WordSnapWeb.Models
{
    public class ApplicationUser : IdentityUser
    {
        public virtual ICollection<Cardset> Cardsets { get; set; } = new List<Cardset>();

        public virtual ICollection<Progress> Progresses { get; set; } = new List<Progress>();

        public virtual ICollection<Userscardset> Userscardsets { get; set; } = new List<Userscardset>();
    }
}
