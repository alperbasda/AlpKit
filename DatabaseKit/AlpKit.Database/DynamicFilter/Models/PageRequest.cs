﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlpKit.Database.DynamicFilter.Models;

public class PageRequest : ICloneable
{
    public int PageIndex { get; set; }
    public int PageSize { get; set; }

    public object Clone()
    {
        return MemberwiseClone();
    }
}

