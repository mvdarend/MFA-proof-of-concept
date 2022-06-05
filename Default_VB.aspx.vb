
Partial Class Default_VB
	Inherits System.Web.UI.Page

	Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load

	End Sub

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

End Class
