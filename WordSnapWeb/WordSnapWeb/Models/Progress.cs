using System;
using System.Collections.Generic;

namespace WordSnapWeb.Models;

public partial class Progress
{
    public required string UserRef { get; set; }

    public int CardsetRef { get; set; }

    public DateTime? LastAccessed { get; set; }

    public double? SuccessRate { get; set; }

    public virtual Cardset CardsetRefNavigation { get; set; } = null!;

    public virtual ApplicationUser UserRefNavigation { get; set; } = null!;
}
