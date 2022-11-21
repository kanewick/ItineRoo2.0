namespace ItineRoo.WebApp.Models
{
    public class AccountModel
    {
        public UserRegisterModel UserRegisterModel { get; set; }
        public UserLoginModel UserLoginModel { get; set; }
        public UserVerificationModel UserVerificationModel { get; set; }

        public AccountModel()
        {
            UserRegisterModel = new UserRegisterModel();
            UserLoginModel = new UserLoginModel();
            UserVerificationModel = new UserVerificationModel();
        }




    }
}
