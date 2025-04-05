﻿using System;
using System.Collections.Generic;

namespace Repository.Models;

public partial class Country
{
    public int Countryid { get; set; }

    public string? Countryname { get; set; }

    public virtual ICollection<State> States { get; set; } = new List<State>();
}
