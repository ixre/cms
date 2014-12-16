using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ops.Cms.Domain.Interface.Site.Extend
{
    public interface IExtendValue:IDomain<int>
    {
        IExtendField Field { get; }
        string Value { get; set; }
    }
}
