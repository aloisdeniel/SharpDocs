﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpDocs.Documentation
{
    public class TypeParameter : Node
    {
        public TypeParameter()
        {
            this.Id = $"type_{this.Id }";
        }
    }
}
