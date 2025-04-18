using System;
using System.Collections.Generic;

namespace WordSnapWeb.Models;

public partial class Userscardset
{
    public int Id { get; set; }

    public required string UserRef { get; set; }

    public int CardsetRef { get; set; }

    public virtual Cardset CardsetRefNavigation { get; set; } = null!;

    public virtual ApplicationUser UserRefNavigation { get; set; } = null!;
}
