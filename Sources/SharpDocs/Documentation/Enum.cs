﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpDocs.Documentation
{
    public class Enum : Node
    {
        public string Namespace { get; set; }

        public IEnumerable<string> Values { get; set; }
    }
}
