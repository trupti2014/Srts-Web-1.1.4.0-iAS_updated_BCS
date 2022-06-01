using System.Web;
using System.Web.UI;

namespace SrtsWeb.CustomErrors
{
    public class ValidationError : IValidator
    {
        private ValidationError(string message)
        {
            ErrorMessage = message;
            IsValid = false;
        }

        public string ErrorMessage { get; set; }

        public bool IsValid { get; set; }

        public static void Display(string message)
        {
            Page currentPage = HttpContext.Current.Handler as Page;
            currentPage.Validators.Add(new ValidationError(message));
        }

        public void Validate()
        {
        }
    }
}