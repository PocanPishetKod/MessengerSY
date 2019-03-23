using System;
using System.Collections.Generic;
using System.Text;

namespace MessengerSY.Core
{
    public class Result<ReturnValue>
    {
        public bool IsSuccess { get; set; }
        public ReturnValue ResultValue { get; set; }
        public IEnumerable<Error> Errors { get; set; }
    }

    public class Error
    {
        public ErrorType ErrorType { get; set; }
    }

    public enum ErrorType
    {

    }
}
