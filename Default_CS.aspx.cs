
public partial class Default_CS : System.Web.UI.Page
{

    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected void btnTokenEntry_Click(object sender, EventArgs e)
    {
        string token = HttpContext.Current.Session["Token"].ToString();
        if (!(txtToken.Text == token))
        {
            // TODO Token does not match, inform the User
        }
        else
        {
            string userName = HttpContext.Current.Session["Username"].ToString();
            // Log the user in by setting the cookie
            System.Web.Security.FormsAuthentication.SetAuthCookie(userName, false);
        }
    }

    protected void btnSubmitLogin_Click(object sender, EventArgs e)
    {
        // Validate the User (this does not log the user in)
        if (System.Web.Security.Membership.ValidateUser(txtUserName.Text, txtPassword.Text))
        {
            // Generate 4 character Token
            string token = GenerateToken(4);
            // Get the Mobile phone numer for this user (in this example hard coded)
            string number = "0123456789";

            // Try sending the SMS
            if (SendSMS(token, number))
            {
                // Hide and Show the PlaceHolders
                plcTokenEntry.Visible = true;
                plcLoginArea.Visible = false;
                // Save values to Session Variables
                HttpContext.Current.Session.Add("Username", txtUserName.Text);
                HttpContext.Current.Session.Add("Token", token);
            }
            else
            {
                // TODO Error sending SMS, give feedback, Log or whatever (or do this in the SendSMS method)
            }
        }
        else
        {
            // TODO Verification failed, let the User know
        }
    }

    private string GenerateToken(int length)
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        // Wait to force new Seed
        System.Threading.Thread.Sleep(20);
        Random rnd = new Random();
        char ch;
        int i = 0;
        for (i = 1; i <= length; i++)
        {
            ch = Convert.ToChar(Convert.ToInt32(25 * rnd.NextDouble() + 65));
            sb.Append(ch);
        }
        return sb.ToString();
    }

    private bool SendSMS(string token, string number)
    {
        string URL = "https://www.voipbuster.com/myaccount/sendsms.php?username={0}&password={1}&from={2}&to={3}&text={4}";
        URL = string.Format(URL, "userName", "password", "0123456789", number, token);
        try
        {
            System.Net.WebRequest wRequest = System.Net.WebRequest.Create(URL);
            wRequest.GetResponse();
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }

}
