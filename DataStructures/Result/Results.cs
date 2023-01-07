using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatternToolbox.DataStructures.Result
{
    public struct ResultNone
    {
        public static readonly ResultNone None = new ();
    }

    public struct Result<SuccessType, ErrorType> //where SuccessType: struct where ErrorType: struct
    {
        public SuccessType? Res { get; set; }// = null;
        public ErrorType? Err { get; set; }// = null;

        private bool _success;

        public bool IsFail { get { return !_success; } }
        public bool IsSuccess { get { return _success; } }

        public Result(SuccessType? defaultSuccess, ErrorType? defaultErrorType)
        {
            _success = false;
            Res = defaultSuccess;
            Err = defaultErrorType;
        }

        public Result<SuccessType, ErrorType> SetSuccess(SuccessType? val)
        {
            _success = true;
            Res = val;
            return this;
        }

        public static Result<SuccessType, ErrorType> Success(SuccessType? val)
        {
            return new Result<SuccessType, ErrorType>().SetSuccess(val);
        }

        public Result<SuccessType, ErrorType> SetFailure(ErrorType? val)
        {
            _success = false;
            Err = val;
            return this;
        }

        public static Result<SuccessType, ErrorType> Failure(ErrorType? val)
        {
            return new Result<SuccessType, ErrorType>().SetFailure(val);
        }
    }
}
