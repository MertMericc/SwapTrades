﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Utilities.Results
{
    public interface IDataResult<T> : IResult
    {
        T Data { get; } // Data dışarıdan geleceği için setter'a ihtiyaç duymadık
    }
}
