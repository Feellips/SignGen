using System;
using System.Collections.Generic;
using System.Text;

namespace SignGen
{
     static class ControlValidationExtension
        {
            public static Validator Rules(this string[] args) => new Validator(args);
        }
}
