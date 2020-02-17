using System;
using System.Threading.Tasks;

namespace component.logger
{
    public interface ILogger
    {
        Task Error(string error);
        Task Information(string error);

        Task Information(string actiontext, string error);

        Task Error(string actiontext, string error);

        Task Error(string actiontext, Exception ex);
        Task Trace(string error);

    }

    public class Constants
    {
        public static readonly string ACTION_ENTRY = "ActionEntry:: {0}. ";
        public static readonly string ACTION = "Action:: {0}. ";
        public static readonly string ACTION_EXIT = "ActionExit:: {0}. ";
        public static readonly string ACTION_INIT_CLASS = "ActionInitClass:: Initiating Class - {0}. ";
        public static readonly string ACTION_EXEC_METHOD = "ActionMethod:: Executing Method - {0}. ";

        public static readonly string ACTION_EXEC_METHOD_FAILED =
            "ActionMethodFailed:: Executing Method- {0} Failed. Message - {1}. ";

        public static readonly string ACTION_EXCEPTION = "ActionException:: {0}. ";

        public static readonly string SIGN_OUT_URL = "SignoutUrl";
    }
}
