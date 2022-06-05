# MFA-proof-of-concept

This is from a forum post I posted to WebmasterWorld in 2010, it was featured as 'Article of the day' on the www.asp.net website a few days later.

Post: https://www.webmasterworld.com/microsoft_asp_net/4189650.htm

----------------------------------------------

Recently a project I am working on decided to adapt the Intranet version of a Web Application to an Internet version. Amongst other things we changed the standard login system from Windows to Forms authentication and supplied the website with a Security certificate and all was fine, until a few customers decided that standard password authentication wouldn't cut it.

After looking at a few existing solutions (SecurId, Hardware SMS systems etc.) and being unpleasantly surprised by their pricing, we decided to build a simple system ourselves.

Basically we wanted a system that works as follows:
* User goes to website and is confronted with a login screen.
* User enters their standard login/password combination and submits.
* A Code (Token) is generated and sent to the User by SMS.
* User enters the token, if it matches they are logged in.

To create this example we need the following:
* .NET Membership [msdn.microsoft.com]
* A Database, .NET User Profiles [msdn.microsoft.com] or other method for storing user mobile phone numbers. For this example we will use a fixed number.
* An SMS gateway provider (for this example I will use VoipBuster)

First we will create a simple form with two PlaceHolders; One containing the Login area, and the other hidden placeholder containing a 'Token' entry form:

Page source
<asp:PlaceHolder ID="plcLoginArea" runat="server">
User name: <asp:TextBox ID="txtUserName" runat="server" />
<br />
Password: <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" />
<br />
<asp:Button ID="btnSubmitLogin" runat="server" Text="Log in" OnClick="btnSubmitLogin_Click" />
</asp:PlaceHolder>

<asp:PlaceHolder ID="plcTokenEntry" runat="server" Visible="false">
Enter Token: <asp:TextBox ID="txtToken" runat="server" />
<br />
<asp:Button ID="btnTokenEntry" runat="server" Text="Submit" OnClick="btnTokenEntry_Click" />
</asp:PlaceHolder>


Nothing special, no pretty markup or any attempt to validate input, I want to keep this simple.

Helper Classes
We need two simple Helper Classes, one to generate a token, and the second one to send the Text Message:

Generate Token (VB)
Private Function GenerateToken(ByVal length As Integer) As String
Dim sb As New StringBuilder()
' Wait to force new Seed
System.Threading.Thread.Sleep(20)
Dim rnd As New Random()
Dim ch As Char
Dim i As Integer
For i = 1 To length
ch = Convert.ToChar(Convert.ToInt32(25 * rnd.NextDouble() + 65))
sb.Append(ch)
Next i
Return sb.ToString()
End Function


Generate Token (C#)
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


Send SMS (VB)
Private Function SendSMS(ByVal token As String, ByVal number As String) As Boolean
Dim URL As String = "https://www.voipbuster.com/myaccount/sendsms.php?username={0}&password={1}&from={2}&to={3}&text={4}"
URL = String.Format(URL, "userName", "password", "0123456789", number, token)
Try
Dim wRequest As Net.WebRequest = Net.WebRequest.Create(URL)
wRequest.GetResponse()
Return True
Catch ex As Exception
Return False
End Try
End Function


Send SMS (C#)
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


Login Process
Now we are ready for the actual login process. The first step is when the User has entered the Username and Password fields and has clicked on btnSubmitLogin. The following will occur:

- Credentials are checked (user is not logged in).
- A random code (Token) is generated and sent.
- UserName and Token are saved to Session variables.
- plcLoginArea.visible is set to false, plcTokenEntry.visible is set to true.

btnSubmitLogin_Click (VB)
Protected Sub btnSubmitLogin_Click(ByVal sender As Object, ByVal e As System.EventArgs)
' Validate the User (this does not log the user in)
If Membership.ValidateUser(txtUserName.Text, txtPassword.Text) Then
' Generate 4 character Token
Dim token As String = GenerateToken(4)
' Get the Mobile phone numer for this user (in this example hard coded)
Dim number As String = "0123456789"

' Try sending the SMS
If SendSMS(token, number) Then
' Hide and Show the PlaceHolders
plcTokenEntry.Visible = True
plcLoginArea.Visible = False
' Save values to Session Variables
HttpContext.Current.Session.Add("Username", txtUserName.Text)
HttpContext.Current.Session.Add("Token", token)
Else
' TODO Error sending SMS, give feedback, Log or whatever (or do this in the SendSMS method)
End If
Else
' TODO Verification failed, let the User know
End If
End Sub


btnSubmitLogin_Click (C#)
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


The user should have received and SMS with the Token, and they can enter it into the text box, when they click on btnSubmitToken the following happens:

- The entered Token is verified with the token saved in the Session Variable.
- If the Tokens match the user is logged in by setting a Auth Cookie (using the UserName stored in the session).

btnTokenEntry_Click (VB)
Protected Sub btnTokenEntry_Click(ByVal sender As Object, ByVal e As System.EventArgs)
Dim token As String = HttpContext.Current.Session("Token").ToString()
If Not txtToken.Text = token Then
' TODO Token does not match, inform the User
Else
Dim userName As String = HttpContext.Current.Session("Username").ToString()
' Log the user in by setting the cookie
FormsAuthentication.SetAuthCookie(userName, False)
End If
End Sub


btnTokenEntry_Click (C#)
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


This is all very simple and needs a bit of work to make it user friendly, but the basics are there for a working two-factor authentication process which is very simple to implement in any website.

Shortcomings:
If the user closes their browser during the login process, the session has been lost and the whole process will need to be repeated as the sent code is no longer valid. (One option is to save the details to a database instead of to the session.)


Comments from Ocean10000
The seeding mechanism for generating the token can produce duplicate codes.*
In server farms you will need a server farm friendly form of Session-State [msdn.microsoft.com].
You can also choose to send the token to an email address instead of a Text Message (SMS).
* to force a new seed every time I have set the system to wait 20 ticks as the New Random() uses the current time as the seed. (In a loop with shorter times I was getting dupes).

Possible improvements:
Use Validators to check input (you should do this)
Use Ajax to show progress spinner while sending an SMS (this can sometimes take a few seconds)
