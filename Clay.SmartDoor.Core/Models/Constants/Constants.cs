namespace Clay.SmartDoor.Core.Models.Constants
{
    public class Constants
    {
        public const string Generic_Begin_Operation_Message = "Beginning operation...";
        public const string Generic_Operation_Failed_Message = "Operation failed";
        public const string Generic_Failure_Message = "A problem occured while attempting operation. Please try again.";
        public const string Generic_Success_Message = "Operation Succeeded.";
        public const string Generic_Not_Found_Message = "Not Found.";
        public const string Generic_Save_Success_Message = "Changes persisted to database.";
        public const string Generic_Fail_Already_Exist_Message = "Operation failed. Item already exist.";
        public const string Generic_Fail_Does_Not_Exist_Message = "Operation failed. Item does not exist in group.";
        public const string Generic_Save_Fail_Message = "Failed to save.";
        public const string Generic_Activity_Logged_Message = "Activity logged.";
    }
    public class ActivityDescriptions
    {
        public const string Door_Opened = "Door Opened";
        public const string Door_Created = "Door Created";
        public const string Exit_Door = "Door Exited";
        public const string Activity_Logged = "Activity logged";
        public const string User_Created = "New user added.";
        public const string Group_Created = "New group added.";
        public const string Door_Added_To_Group = "Door added to group.";
        public const string Door_Removed_From_Group = "Door added to group.";
    }

    public class ApiResponseMesage
    {
        public const string Created_Successfully = "Created successfully";
        public const string Failed_To_Create = "Failed to create";
        public const string Ok_Result = "Completed Successful";
    }

    public class AuthenticationMessage
    {
        public const string Invalid_Credentials = "Invalid Credentials";
        public const string Not_Activated = "Account not activated";
        public const string Login_Success = "Login successful";
        public const string UnAuthorized = "You are not authorized to access this door.";
        public const string Forbidden = "You are not allowed to access this door.";
        public const string User_Already_Exist = "The user you are trying to add already exist.";
    }

    public class DoorMessage
    {
        public const string Not_Found = "Door does not exist in record.";
    }

}
