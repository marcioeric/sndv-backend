namespace WebApplication.ViewModels.Inputs.Account
{
    public class CreateAccountViewModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }
}