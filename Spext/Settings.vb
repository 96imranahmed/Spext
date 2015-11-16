Imports System.Net
Imports System.IO
Imports System.Text
Public Class Settings
    Dim coded As Boolean
    Dim headers As String
    Private Sub Settings_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        Animater.Interval = My.Settings.FadeOutTimer
        Dim x As Integer
        Dim y As Integer
        x = Screen.PrimaryScreen.WorkingArea.Width
        y = Screen.PrimaryScreen.WorkingArea.Height - Me.Height
        Do Until x = Screen.PrimaryScreen.WorkingArea.Width - Me.Width
            x = x - 1
            Me.Location = New Point(x, y)
        Loop
        Me.TopMost = True
        Label2.Text = My.Settings.ListeningTimer
        TextBox1.Text = My.Settings.Location
        WebBrowser1.ScriptErrorsSuppressed = True
        headers = "User-Agent: Mozilla/5.0 (compatible; MSIE 10.0; Windows Phone 8.0; Trident/6.0; IEMobile/10.0; ARM; Touch; NOKIA; Lumia 920)"
        HScrollBar1.Value = My.Settings.ListeningTimer
        Label2.Text = My.Settings.ListeningTimer.ToString
        HScrollBar2.Value = My.Settings.FadeOutTimer
        Label6.Text = My.Settings.FadeOutTimer.ToString
        Animater.Enabled = True

    End Sub
    Private Sub Settings_FormClosed(sender As System.Object, e As System.EventArgs) Handles MyBase.FormClosed
        My.Settings.Save()
    End Sub
    Private Sub HScrollBar1_Scroll(sender As System.Object, e As System.Windows.Forms.ScrollEventArgs) Handles HScrollBar1.Scroll
        Dim x As Integer = HScrollBar1.Value
        x = CustomCieling(x)
        Label2.Text = x
        My.Settings.ListeningTimer = x
        My.Forms.Main.TriggerHasHeldDown.Interval = HScrollBar1.Value
        My.Settings.Save()
    End Sub

    Private Sub TextBox1_TextChanged(sender As System.Object, e As System.EventArgs) Handles TextBox1.TextChanged
        My.Settings.Location = TextBox1.Text
    End Sub
    Private Sub TextBox1_KeyDown(sender As System.Object, e As System.Windows.Forms.KeyEventArgs) Handles TextBox1.KeyDown
        My.Settings.Location = TextBox1.Text
    End Sub

    Private Sub Button1_Click(sender As System.Object, e As System.EventArgs) Handles Button1.Click
        Dim cookies As String() = System.IO.Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.Cookies))
        For Each currentfile As String In cookies
            Try
                System.IO.File.Delete(currentfile)
            Catch
            End Try
        Next
        If Button1.Text.Contains("in to") Then
            Button1.Enabled = False
            WebBrowser1.Navigate("https://www.facebook.com/dialog/oauth?client_id=576593989017732&redirect_uri=http://spext.webs.com/&scope=publish_stream", Nothing, Nothing, headers)
        ElseIf Button1.Text.Contains("out from") Then
            Button1.Enabled = False
            Dim result = MessageBox.Show("Would you really like to logout from your Facebook account? All saved tokens and data will be erased.", "Confirm Logout", MessageBoxButtons.OKCancel, MessageBoxIcon.Information)
            If result = Windows.Forms.DialogResult.OK Then
                My.Settings.FacebookPublishToken = ""
                My.Settings.FacebookIsConnected = False
                My.Settings.FacebookTokenChange = Nothing
                My.Settings.Save()
                MessageBox.Show("You have been logged out from your Facebook Account.", "Successful Logout", MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If
            Button1.Enabled = True
        End If
    End Sub
    Public Shared Function Extendtoken(existingToken As String) As String
        Dim interpretationstring As String
        Dim request As HttpWebRequest
        Dim response As HttpWebResponse = Nothing
        Dim reader As StreamReader
        Try
            request = DirectCast(WebRequest.Create("https://graph.facebook.com/oauth/access_token?grant_type=fb_exchange_token&client_id=576593989017732&client_secret=7d178cef990f4f23621135a81b1eac04&fb_exchange_token=" + existingToken), HttpWebRequest)
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.0; WOW64) AppleWebKit/537.17 (KHTML, like Gecko) Chrome/24.0.1312.56 Safari/537.17"
            response = DirectCast(request.GetResponse(), HttpWebResponse)
            reader = New StreamReader(response.GetResponseStream())
            interpretationstring = reader.ReadToEnd()
        Finally
            If Not response Is Nothing Then response.Close()
        End Try
        Try
            Dim crazylongcode As String
            crazylongcode = interpretationstring
            crazylongcode = crazylongcode.Remove(0, crazylongcode.IndexOf("access_token="))
            crazylongcode = crazylongcode.Replace("access_token=", "")
            crazylongcode = crazylongcode.Substring(0, crazylongcode.IndexOf("&expires"))
            Return crazylongcode
        Catch
            Return ("Error - log back out and in again")
        End Try

    End Function


    Private Sub WebBrowser1_Navigated(sender As System.Object, e As System.Windows.Forms.WebBrowserNavigatedEventArgs) Handles WebBrowser1.Navigated
        If WebBrowser1.DocumentText.ToString.Contains("access_token=") And My.Settings.FacebookIsConnected = False Then
            Dim x As String = WebBrowser1.Document.Body.InnerText.ToString()
            x = x.Replace("access_token=", "")
            x = x.Substring(0, x.IndexOf("&"))
            x = Extendtoken(x)
            If x = "Error - log back out and in again" Then
                MessageBox.Show("There was an error with your token retrieval", "Token Extension Retrieval Failure", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Button1.Enabled = True
            Else
                My.Settings.FacebookPublishToken = x
                My.Settings.Save()
                Dim permission As String = RetrivePermissions(x)
                If permission.Contains("""publish_stream"": 1") = False Then
                    Dim result = MessageBox.Show("Bugger. It looks like you didn't allow the ""access on your behalf"" permission. This will stop any of the Facebook related features from functioning. Press OK, to open a tutorial to re-enable permissions. After this, restart the login process to complete the set-up. Cancelling will abort the login process", "Permission Access Denied", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning)
                    If result = Windows.Forms.DialogResult.OK Then
                        Process.Start("http://spext.webs.com/facebook-setup")
                        My.Settings.FacebookIsConnected = False
                        My.Settings.FacebookPublishToken = ""
                        My.Settings.FacebookTokenChange = Nothing
                        My.Settings.Save()
                        Button1.Enabled = True
                    End If
                    If result = Windows.Forms.DialogResult.Cancel Then
                        My.Settings.FacebookIsConnected = False
                        My.Settings.FacebookPublishToken = ""
                        My.Settings.FacebookTokenChange = Nothing
                        My.Settings.Save()
                        Button1.Enabled = True
                    End If
                Else
                    Dim count As Integer = WebBrowser1.Document.Cookie.Length
                    Dim Currentdate As Date = DateAndTime.Now
                    My.Settings.FacebookTokenChange = Currentdate
                    My.Settings.FacebookIsConnected = True
                    My.Settings.Save()
                    WebBrowser1.Document.Cookie.Remove(0, count)
                    WebBrowser1.Navigate("http://spext.webs.com", Nothing, Nothing, headers)
                    MessageBox.Show("Token Retrieved! If you ever want to change your Facebook details, go to Settings, logout and login to Facebook with your new account and follow through with the same process. Tokens will expire in 60 days!", "Token Retrieved", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    Button1.Enabled = True
                End If
            End If
        End If
        Try
            If WebBrowser1.Url.ToString.Contains("/?code=") Or WebBrowser1.Url.ToString.Contains("?dm_path=") Then
                If My.Settings.FacebookIsConnected = False Then
                    If WebBrowser1.Url.ToString.Contains("?dm_path=") Then
                        Dim crazylongcode As String
                        crazylongcode = WebBrowser1.Url.ToString
                        crazylongcode = URLDecode(crazylongcode)
                        If crazylongcode.Contains("/?code=") Then
                            crazylongcode = crazylongcode.Remove(0, crazylongcode.IndexOf("/?code="))
                            crazylongcode = crazylongcode.Replace("/?code=", "")
                            crazylongcode = crazylongcode.Substring(0, crazylongcode.IndexOf("&"))
                            WebBrowser1.Navigate("https://graph.facebook.com/oauth/access_token?client_id=576593989017732&redirect_uri=http://spext.webs.com/&client_secret=7d178cef990f4f23621135a81b1eac04&code=" + crazylongcode, Nothing, Nothing, headers)
                        End If
                    Else
                        Dim crazylongcode As String
                        crazylongcode = WebBrowser1.Url.ToString
                        crazylongcode = crazylongcode.Remove(0, crazylongcode.IndexOf("/?code="))
                        crazylongcode = crazylongcode.Replace("/?code=", "")
                        WebBrowser1.Navigate("https://graph.facebook.com/oauth/access_token?client_id=576593989017732&redirect_uri=http://spext.webs.com/&client_secret=7d178cef990f4f23621135a81b1eac04&code=" + crazylongcode, Nothing, Nothing, headers)
                    End If
                End If
            End If
        Catch
        End Try

    End Sub
    Public Function URLDecode(StringToDecode As String) As String
        Dim TempAns As String
        Dim CurChr As Integer
        CurChr = 1
        Do Until CurChr - 1 = Len(StringToDecode)
            Select Case Mid(StringToDecode, CurChr, 1)
                Case "+"
                    TempAns = TempAns & " "
                Case "%"
                    TempAns = TempAns & Chr(Val("&h" & _
                       Mid(StringToDecode, CurChr + 1, 2)))
                    CurChr = CurChr + 2
                Case Else
                    TempAns = TempAns & Mid(StringToDecode, CurChr, 1)
            End Select

            CurChr = CurChr + 1
        Loop
        URLDecode = TempAns
    End Function
    Public Function RetrivePermissions(ByVal accesstoken As String) As String
        Dim permissions As String
        Dim interpretationstring As String
        Dim request As HttpWebRequest
        Dim response As HttpWebResponse = Nothing
        Dim reader As StreamReader
        Try
            request = DirectCast(WebRequest.Create("https://graph.facebook.com/me/permissions?access_token=" + accesstoken), HttpWebRequest)
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.0; WOW64) AppleWebKit/537.17 (KHTML, like Gecko) Chrome/24.0.1312.56 Safari/537.17"
            response = DirectCast(request.GetResponse(), HttpWebResponse)
            reader = New StreamReader(response.GetResponseStream())
            interpretationstring = reader.ReadToEnd()
        Finally
            If Not response Is Nothing Then response.Close()
        End Try
        permissions = interpretationstring
        Return permissions
    End Function

    Private Sub Timer1_Tick(sender As System.Object, e As System.EventArgs) Handles Timer1.Tick
        If WebBrowser1.DocumentText.Contains("access_token=") Then
            WebBrowser1_Navigated(Nothing, Nothing)
        End If
        If My.Settings.FacebookIsConnected = True Then
            Button1.Text = "Logout from Facebook"
            Dim cookies As String() = System.IO.Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.Cookies))
            For Each currentfile As String In cookies
                Try
                    System.IO.File.Delete(currentfile)
                Catch
                End Try
            Next
        End If
        If My.Settings.FacebookIsConnected = False Then
            Button1.Text = "Login to Facebook"
            Dim cookies As String() = System.IO.Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.Cookies))
            For Each currentfile As String In cookies
                Try
                    System.IO.File.Delete(currentfile)
                Catch
                End Try
            Next
        End If
    End Sub


    Private Sub HScrollBar2_Scroll(sender As System.Object, e As System.Windows.Forms.ScrollEventArgs) Handles HScrollBar2.Scroll
        Dim x As Integer = HScrollBar2.Value
        x = CustomCieling(x)
        Label6.Text = x
        My.Settings.FadeOutTimer = x
        My.Settings.Save()
        My.Forms.Main.Animater.Interval = x
        Me.Animater.Interval = x
    End Sub
    Public Function CustomCieling(number As Integer) As Integer
        Dim nearesthundred As Integer
        NearestHundred = Math.Round(number / 100, 0) * 100
        Return nearesthundred
    End Function

    Private Sub Animater_Tick(sender As System.Object, e As System.EventArgs) Handles Animater.Tick
        Dim pointtoclient As Point = Me.PointToClient(MousePosition)
        If pointtoclient.X < 0 Or pointtoclient.Y < 0 Then
            Animater.Enabled = False
            Dim iCount As Integer
            For iCount = 90 To 10 Step -10
                Me.Opacity = iCount / 100
                Me.Refresh()
                Threading.Thread.Sleep(50)
            Next
            Me.Close()
        End If
    End Sub
End Class