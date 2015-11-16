Imports System.Diagnostics
Imports System.Runtime.InteropServices
Imports NAudio.Wave
Imports System.Media
Imports System.IO
Imports System.Text
Imports System.Net
Imports Microsoft.Win32
Imports System.Text.RegularExpressions
Public Class Main
    Public softwareinstalled As List(Of String)
    Public softwarepath As List(Of String)
    Public held As Boolean
    Public WithEvents waveInStream As WaveIn
    Dim writer As WaveFileWriter
    Public filepath As String = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)
    Dim proc As New Process
    Public finalreceived As String
    Private checktext As String
    <DllImport("user32.dll")> _
    Public Shared Function GetAsyncKeyState(ByVal vKey As System.Windows.Forms.Keys) As Short
    End Function
    <DllImport("user32.dll", SetLastError:=True)> _
    Private Shared Function CloseWindow(ByVal hWnd As IntPtr) As Integer
    End Function
    Private Declare Function GetForegroundWindow Lib "user32.dll" () As Long
    <DllImport("user32.dll", SetLastError:=True, CharSet:=CharSet.Auto)> _
    Private Shared Function SetForegroundWindow(ByVal hWnd As IntPtr) As Long
    End Function
    Private Declare Function SetFocus Lib "user32.dll" (ByVal hwnd As Int32) As Int32
    Dim hwnd As Long
    Private Sub Form1_Close(sender As System.Object, e As System.EventArgs) Handles MyBase.FormClosed
        My.Settings.Save()
    End Sub
    Private Sub Form1_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        Animater.Interval = My.Settings.FadeOutTimer
        If (Not System.IO.Directory.Exists(filepath + "/Spext")) Then
            System.IO.Directory.CreateDirectory(filepath + "/Spext")
            If (Not File.Exists(filepath + "/Spext/ffmpeg.exe")) Then
                System.IO.File.WriteAllBytes(filepath + "/Spext/ffmpeg.exe", My.Resources.ffmpeg)
            End If
        End If
        Dim source As New AutoCompleteStringCollection()
        Dim obj As [Object]
        For Each obj In My.Settings.Autocomplete
            source.Add(obj)
        Next
        TextBox1.AutoCompleteMode = AutoCompleteMode.Append
        TextBox1.AutoCompleteSource = AutoCompleteSource.CustomSource
        TextBox1.AutoCompleteCustomSource = source
        Me.Visible = True
        Dim x As Integer
        Dim y As Integer
        x = Screen.PrimaryScreen.WorkingArea.Width
        y = Screen.PrimaryScreen.WorkingArea.Height - Me.Height
        Do Until x = Screen.PrimaryScreen.WorkingArea.Width - Me.Width
            x = x - 1
            Me.Location = New Point(x, y)
        Loop
        Me.Opacity = 0.99
        Animater.Enabled = True
        Me.TopMost = True
        Dim datenow As Date = DateAndTime.Now
        Dim previousdate As Date = My.Settings.FacebookTokenChange
        Dim elapsed As TimeSpan
        elapsed = datenow.Subtract(previousdate)
        If elapsed.TotalSeconds.ToString >= 5000000 And My.Settings.FacebookIsConnected = True Then
            MessageBox.Show("Fiddlesticks! Your Facebook Access Token has Expired! Please let me renew your token by logging into Facebook from the settings page", "Facebook Token Expiry Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            My.Settings.FacebookIsConnected = False
            My.Settings.FacebookPublishToken = ""
            My.Settings.FacebookTokenChange = Nothing
            My.Forms.Settings.Show()
            My.Forms.Settings.TabPage2.Focus()
            My.Forms.Settings.Button1.PerformClick()
            My.Forms.Settings.TopMost = True
        End If
        If My.Settings.FacebookIsConnected = True Then
    
            Dim permissions As String
            Dim interpretationstring As String
            Dim request As HttpWebRequest
            Dim response As HttpWebResponse = Nothing
            Dim reader As StreamReader
            Try
                request = DirectCast(WebRequest.Create("https://graph.facebook.com/me?access_token=" + My.Settings.FacebookPublishToken), HttpWebRequest)
                request.UserAgent = "Mozilla/5.0 (Windows NT 6.0; WOW64) AppleWebKit/537.17 (KHTML, like Gecko) Chrome/24.0.1312.56 Safari/537.17"
                response = DirectCast(request.GetResponse(), HttpWebResponse)
                reader = New StreamReader(response.GetResponseStream())
                interpretationstring = reader.ReadToEnd()
            Finally
                If Not response Is Nothing Then response.Close()
            End Try
            permissions = interpretationstring
            If permissions = Nothing Then
                MessageBox.Show("Facebook connection lost. Please login to Facebook again in the Settings Page.", "Facebook Connection Lost", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                My.Settings.FacebookIsConnected = False
                My.Settings.FacebookPublishToken = ""
                My.Settings.FacebookTokenChange = Nothing
                My.Settings.Save()
                My.Forms.Settings.Show()
                My.Forms.Settings.TabPage2.Focus()
                My.Forms.Settings.Button1.PerformClick()
                My.Forms.Settings.TopMost = True
            Else
                If permissions.Contains("Exception") Then
                    My.Settings.FacebookIsConnected = False
                    My.Settings.FacebookPublishToken = ""
                    My.Settings.FacebookTokenChange = Nothing
                    My.Settings.Save()
                    MessageBox.Show("Facebook connection lost. Please login to Facebook again in the Settings Page.", "Facebook Connection Lost", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    My.Forms.Settings.Show()
                    My.Forms.Settings.TabPage2.Focus()
                    My.Forms.Settings.Button1.PerformClick()
                    My.Forms.Settings.TopMost = True
                End If
            End If
        End If

    End Sub
    Private Sub Animater_Tick(sender As System.Object, e As System.EventArgs) Handles Animater.Tick
        If Me.Focused = False And Me.TextBox1.Focused = False Then         
                Animater.Enabled = False
                Dim iCount As Integer
                For iCount = 90 To 10 Step -10
                    Me.Opacity = iCount / 100
                    Me.Refresh()
                    Threading.Thread.Sleep(50)
                Next
                Me.Opacity = 0.01
                Me.SendToBack()
        Else
            If Me.TextBox1.Text = checktext Then
                Animater.Enabled = False
                Dim iCount As Integer
                For iCount = 90 To 10 Step -10
                    Me.Opacity = iCount / 100
                    Me.Refresh()
                    Threading.Thread.Sleep(50)
                Next
                Me.Opacity = 0.01
                Me.SendToBack()
            End If
        End If
        checktext = TextBox1.Text
    End Sub
    Private Sub TriggerCheck_Tick(sender As System.Object, e As System.EventArgs) Handles TriggerCheck.Tick
        Dim capslock As Boolean
        Dim tab As Boolean
        Dim caps As Boolean
        Dim f1 As Boolean
        capslock = GetAsyncKeyState(Keys.ShiftKey)
        tab = GetAsyncKeyState(Keys.LControlKey)
        caps = GetAsyncKeyState(Keys.LWin)
        f1 = GetAsyncKeyState(Keys.F1)
        If f1 = True And Me.Opacity > 0.01 Then
            Settings.Show()
        End If
        If caps = True And capslock = True Then
            NotifyIcon1_MouseDoubleClick(Nothing, Nothing)
        End If
        If capslock = True And tab = True Then

            held = True
            If TriggerHasHeldDown.Enabled = False Then
                TriggerHasHeldDown.Enabled = True
            End If
        Else
            held = False
            If TriggerHasHeldDown.Enabled = True Then
                TriggerHasHeldDown.Enabled = False
            End If
        End If
    End Sub
    Sub Wavein_dataavailable(ByVal sender As System.Object, ByVal e As WaveInEventArgs) Handles waveInStream.DataAvailable
        writer.Write(e.Buffer, 0, e.BytesRecorded)
    End Sub
    Private Sub TriggerHasHeldDown_Tick(sender As System.Object, e As System.EventArgs) Handles TriggerHasHeldDown.Tick
        If Triggerholding.Enabled = True Then

        Else
            If held = True Then
                Triggerholding.Enabled = True
                Dim sound As New SoundPlayer(My.Resources.Sound)
                sound.Play()
                Dim FileToDelete As String
                FileToDelete = filepath + "\record.flac"
                If System.IO.File.Exists(FileToDelete) = True Then
                    System.IO.File.Delete(FileToDelete)
                End If
                waveInStream = New WaveIn
                waveInStream.WaveFormat = New WaveFormat(16000, 1)
                writer = New WaveFileWriter(filepath + "\record.flac", waveInStream.WaveFormat)
                waveInStream.StartRecording()

            End If
        End If
    End Sub
    Public Shared Function UploadFile(ByVal uploadfilename As String, ByVal url As String, ByVal fileFormName As String, ByVal contenttype As String, ByVal querystring As System.Collections.Specialized.NameValueCollection, ByVal cookies As CookieContainer) As String
        If (fileFormName Is Nothing) OrElse (fileFormName.Length = 0) Then
            fileFormName = "file"
        End If
        If (contenttype Is Nothing) OrElse (contenttype.Length = 0) Then
            contenttype = "application/octet-stream"
        End If
        Dim postdata As String
        postdata = "?"
        If Not (querystring Is Nothing) Then
            For Each key As String In querystring.Keys
                postdata += key + "=" + querystring.Get(key) + "&"
            Next
        End If
        Dim uri As Uri = New Uri(url + postdata)
        Dim boundary As String = "----------" + DateTime.Now.Ticks.ToString("x")
        Dim webrequest As HttpWebRequest = CType(Net.WebRequest.Create(uri), HttpWebRequest)
        webrequest.CookieContainer = cookies
        webrequest.ContentType = "audio/x-flac; rate=16000"
        webrequest.Method = "POST"
        Dim sb As StringBuilder = New StringBuilder
        sb.Append("--")
        sb.Append(boundary)
        sb.Append("" & Microsoft.VisualBasic.Chr(13) & "" & Microsoft.VisualBasic.Chr(10) & "")
        sb.Append("Content-Disposition: form-data; name=""")
        sb.Append(fileFormName)
        sb.Append("""; filename=""")
        sb.Append(IO.Path.GetFileName(uploadfilename))
        sb.Append("""")
        sb.Append("" & Microsoft.VisualBasic.Chr(13) & "" & Microsoft.VisualBasic.Chr(10) & "")
        sb.Append("Content-Type: ")
        sb.Append(contenttype)
        sb.Append("" & Microsoft.VisualBasic.Chr(13) & "" & Microsoft.VisualBasic.Chr(10) & "")
        sb.Append("" & Microsoft.VisualBasic.Chr(13) & "" & Microsoft.VisualBasic.Chr(10) & "")
        Dim postHeader As String = sb.ToString
        Dim postHeaderBytes As Byte() = Encoding.UTF8.GetBytes(postHeader)
        Dim boundaryBytes As Byte() = Encoding.ASCII.GetBytes("" & Microsoft.VisualBasic.Chr(13) & "" & Microsoft.VisualBasic.Chr(10) & "--" + boundary + "" & Microsoft.VisualBasic.Chr(13) & "" & Microsoft.VisualBasic.Chr(10) & "")
        Dim fileStreama As FileStream = New FileStream(uploadfilename, FileMode.Open, FileAccess.Read)
        Dim length As Long = postHeaderBytes.Length + fileStreama.Length + boundaryBytes.Length
        webrequest.ContentLength = length
        Dim requestStream As Stream = webrequest.GetRequestStream
        requestStream.Write(postHeaderBytes, 0, postHeaderBytes.Length)
        Dim sendBuffer(Math.Min(4096, fileStreama.Length)) As Byte
        Dim bytesRead As Integer = 0
        Do
            bytesRead = fileStreama.Read(sendBuffer, 0, sendBuffer.Length)
            If bytesRead = 0 Then Exit Do
            requestStream.Write(sendBuffer, 0, bytesRead)
        Loop
        requestStream.Write(boundaryBytes, 0, boundaryBytes.Length)
        Dim responce As WebResponse = webrequest.GetResponse
        Dim s As Stream = responce.GetResponseStream
        Dim sr As StreamReader = New StreamReader(s)
        fileStreama.Dispose()
        Return sr.ReadToEnd
    End Function

    Private Sub Triggerholding_Tick(sender As System.Object, e As System.EventArgs) Handles Triggerholding.Tick
        Dim capslock As Boolean
        Dim tab As Boolean
        capslock = GetAsyncKeyState(Keys.ShiftKey)
        tab = GetAsyncKeyState(Keys.LControlKey)
        If tab = True And capslock = True Then
            Triggerholding.Enabled = True
            Me.Focus()
        Else
            Triggerholding.Enabled = False
            Try
                waveInStream.StopRecording()
                waveInStream.Dispose()
                writer.Dispose()
                Dim sound As New SoundPlayer(My.Resources.Sounddouble)
                sound.Play()
                Try
                    Dim input As String = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\record.flac"
                    Dim output As String = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\record.flac"
                    Dim exepath As String = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\Spext\ffmpeg.exe"
                    Dim startinfo As New System.Diagnostics.ProcessStartInfo
                    Dim sr As StreamReader
                    Dim cmd As String = " -i """ + input + """ -ar 16000 -y """ + output + """"
                    Dim ffmpegOutput As String
                    startinfo.FileName = exepath
                    startinfo.Arguments = cmd
                    startinfo.UseShellExecute = False
                    startinfo.WindowStyle = ProcessWindowStyle.Hidden
                    startinfo.RedirectStandardError = True
                    startinfo.RedirectStandardOutput = True
                    startinfo.CreateNoWindow = True
                    proc.StartInfo = startinfo
                    proc.Start()
                    sr = proc.StandardError
                    Do
                        ffmpegOutput = sr.ReadLine
                    Loop Until proc.HasExited And ffmpegOutput = Nothing Or ffmpegOutput = ""
                    If proc.HasExited = False Then
                        proc.Kill()
                    End If

                    Dim outdata As String = UploadFile(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\record.flac", "http://www.google.com/speech-api/v1/recognize?xjerr=1&client=chromium&lang=en-US", "", "audio/x-flac; rate=16000", Nothing, Nothing)
                    Dim txt As String = outdata
                    txt = txt.Substring(txt.IndexOf("utterance"":"""), txt.IndexOf(""",""confidence") - txt.IndexOf("utterance"":"""))
                    txt = txt.Replace("utterance"":""", "")
                    finalreceived = txt
                    Dim final As String = finalreceived
                    final = final.Trim()
                    TextBox1.Text = final
                    Analysis(final)
                Catch ex As Exception

                End Try

            Catch ex As Exception
            End Try

        End If

    End Sub

    Public Function Analysis(ByVal Input As String)
        If Input.StartsWith("scrape(") Then
            Input = Input.Replace("scrape(", "")
            Input = Input.Replace(")", "")
            Input = scrape(Input)
        End If
        Input = Input.Trim
        If Input.StartsWith("Wiki") Or Input.StartsWith("wiki") Then
            Input = Input.Remove(0, 4)
            Input = Input.Trim
            MessageBox.Show(Wiki(Input))
        End If
        If Input = "facebook" Or Input = "Facebook" Then
            Process.Start("http://www.facebook.com")
            Return Nothing
        End If
        If Input.StartsWith("weather in") Or Input.StartsWith("Weather in") Or Input = "Weather" Or Input = "weather" Then
            If Input = "Weather" Or Input = "weather" Then
                Weatherform("https://www.google.com/search?q=" + Input, True)
            Else
                Input = StrConv(Input, VbStrConv.ProperCase)
                Weatherform("https://www.google.com/search?q=" + Input, False)
            End If
            Return Nothing
        End If
        If Input.StartsWith("Google") Or Input.StartsWith("google") Then
            Input = Input.Remove(0, 6)
            Input = Input.Trim()
            Process.Start("https://www.google.com/search?q=" + Input)
            Return Nothing
        End If
        If Input.StartsWith("youtube") Or Input.StartsWith("Youtube") Then
            Input = Input.Remove(0, 7)
            Input = Input.Trim()
            Process.Start("https://www.youtube.com/results?search_query=" + Input)
            Return Nothing
        End If
        If Input.StartsWith("definition of") Or Input.StartsWith("Definition of") Or Input.StartsWith("Define") Or Input.StartsWith("define") Then
            If Input.StartsWith("definition of") Then
                Input = Input.Replace("definition of", "")
                Definition(Input)
            ElseIf Input.StartsWith("Definition of") Then
                Input = Input.Replace("Defintion of", "")
                Definition(Input)
            ElseIf Input.StartsWith("Define") Then
                Input = Input.Replace("Define ", "")
                Definition(Input)
            ElseIf Input.StartsWith("define") Then
                Input = Input.Replace("define ", "")
                Definition(Input)
            End If
            Return Nothing
        End If
        If Input.StartsWith("Time in") Or Input.StartsWith("time in") Then
            Input = Input.Remove(0, 7)
            Input.Trim()
            Input = Input.Replace(" ", "+")
            Time(Input)
        End If
        If Input.StartsWith("Type") Or Input.StartsWith("type") Then
            Input = Input.Remove(0, 4)
            Typetext(Input)
        End If
        If Input.StartsWith("Convert") Or Input.StartsWith("convert") Then
            Input = Input.Remove(0, 7)
            Input = Input.Replace(" ", "+")
            Conversion(Input)
        End If
        If (Input.StartsWith("Post", StringComparison.OrdinalIgnoreCase)) Then
            Dim x As String = Input
            If Input.Contains(" link ") Then
                If Input.Contains(" to ") Then
                    If (Input.IndexOf(" wall", 0, StringComparison.CurrentCultureIgnoreCase)) > -1 Then
                        If Input.Contains(" my wall") Then
                            If Input.Contains(" copy") = False Or Input.Contains(" copied") = False Then
                                Dim link As String = Clipboard.GetText
                                If (link.IndexOf("http", 0, StringComparison.CurrentCultureIgnoreCase) > -1) And link.Contains("://") And link.IndexOf("://") > (link.IndexOf("http", 0, StringComparison.CurrentCultureIgnoreCase) > -1) Then
                                    MessageBox.Show(link)
                                End If
                            End If
                            If Input.Contains(" copied") Or Input.Contains("a copy of") And Input.IndexOf(" cop", 0, StringComparison.CurrentCultureIgnoreCase) < Input.IndexOf(" link ", 0, StringComparison.CurrentCultureIgnoreCase) Then
                                Dim link As String = Clipboard.GetText
                                If (link.IndexOf("http", 0, StringComparison.OrdinalIgnoreCase) > -1) And link.Contains("://") And link.IndexOf("://") > (link.IndexOf("http", 0, StringComparison.CurrentCultureIgnoreCase) > -1) Then
                                    MessageBox.Show(link)
                                End If
                            End If
                        End If
                    End If
                End If
            End If
        End If
        Return Nothing
    End Function
    Public Function Wiki(ByVal link As String) As String
        Try
            Dim interpretationstring As String
            Dim request As HttpWebRequest
            Dim response As HttpWebResponse = Nothing
            Dim reader As StreamReader
            Try
                request = DirectCast(WebRequest.Create("http://en.wikipedia.org/w/index.php?title=" + link + "&printable=yes"), HttpWebRequest)
                request.UserAgent = "Mozilla/5.0 (Windows NT 6.0; WOW64) AppleWebKit/537.17 (KHTML, like Gecko) Chrome/24.0.1312.56 Safari/537.17"
                response = DirectCast(request.GetResponse(), HttpWebResponse)
                reader = New StreamReader(response.GetResponseStream())
                interpretationstring = reader.ReadToEnd()
            Finally
                If Not response Is Nothing Then response.Close()
            End Try
            Dim processtext As String = interpretationstring
            Clipboard.SetText(processtext)
            If processtext.Contains("Wikipedia does not have an article with this exact name.") = False Then
                If processtext.Contains("/wiki/Category:Disambiguation_pages") = False Then
                    Dim finaltext As String
                    Return finaltext
                Else
                    Dim finaltext As String = "Disambiguation Error"
                    Return finaltext
                End If
            Else
                Dim finaltext As String = "No Articles Available on that Topic"
                Return finaltext
            End If
        Catch
            Dim finaltext As String = "No Articles Available on that Topic"
            Return finaltext
        End Try
   
    End Function
    Public Function scrape(ByVal Input As String) As String
        Dim inputscrape As String = Input
        Dim ender As String = ">"
        For Each ender In Input
            Dim starter As Integer = inputscrape.IndexOf(ender)
            Dim subbed As String = inputscrape.Substring(starter, inputscrape.IndexOf("<"))
            If subbed.Length <= 1 Then
                inputscrape = inputscrape.Remove(0, inputscrape.IndexOf("<") + 1)
                inputscrape = inputscrape.Remove(0, inputscrape.IndexOf(">"))
                MessageBox.Show("Illegal" + " " + inputscrape)
            Else
                MessageBox.Show(subbed)
            End If
        Next
        Return inputscrape
    End Function
    Public Function determinelink(ByVal link As String) As Boolean
        Dim x As String = link
        If (x.IndexOf("http", 0, StringComparison.OrdinalIgnoreCase) > -1) And link.Contains("://") And link.IndexOf("://") > (link.IndexOf("http", 0, StringComparison.CurrentCultureIgnoreCase) > -1) Then
            If x.Contains("/") Then
                If x.IndexOf("/") > x.IndexOf(".") Then
                    Return True
                Else
                    Return False
                End If
            Else
                Return True
            End If
        Else
        End If

    End Function

    Public Function Conversion(ByVal input As String)
        Try
            input = input.Trim()
            Dim interpretationstring As String
            Dim request As HttpWebRequest
            Dim response As HttpWebResponse = Nothing
            Dim reader As StreamReader
            Try
                request = DirectCast(WebRequest.Create("https://www.google.com/search?q=" + input), HttpWebRequest)
                request.UserAgent = "Mozilla/5.0 (Windows NT 6.0; WOW64) AppleWebKit/537.17 (KHTML, like Gecko) Chrome/24.0.1312.56 Safari/537.17"
                response = DirectCast(request.GetResponse(), HttpWebResponse)
                reader = New StreamReader(response.GetResponseStream())
                interpretationstring = reader.ReadToEnd()
            Finally
                If Not response Is Nothing Then response.Close()
            End Try
            If interpretationstring.Contains("<div class=""vk_sh vk_gy cursrc"">") Then
                Dim original As String = interpretationstring
                original = original.Remove(0, original.IndexOf("<div class=""vk_sh vk_gy cursrc"">"))
                original = original.Replace("<div class=""vk_sh vk_gy cursrc"">", "")
                original = original.Substring(0, original.IndexOf("</div>"))
                original = original.Replace("<span style=""word-break:break-all"">", "")
                original = original.Replace("</span>", "")
                original = original.Replace("<span>", "")
                original = original.Replace("equals", "")
                original = original.Replace("&nbsp;", "")
                original.Trim()
                Dim converted As String = interpretationstring
                converted = converted.Remove(0, converted.IndexOf("<div class=""vk_ans vk_bk curtgt"" style"))
                converted = converted.Substring(0, converted.IndexOf("</div>"))
                converted = converted.Replace("<div class=""vk_ans vk_bk curtgt"" style", "")
                converted = converted.Replace("<span style=""word-break:break-all"">", "")
                converted = converted.Replace("</span>", "")
                converted = converted.Replace("<span>", "")
                Try
                    converted = converted.Remove(0, converted.IndexOf(">") + 1)
                    converted = converted.Trim()
                    My.Forms.Converter.Label1.Text = original
                    My.Forms.Converter.Label3.Text = converted
                    Dim currencydisclaimer As String = "https://www.google.com/intl/en/help/currency_disclaimer.html"
                    My.Forms.Converter.LinkLabel1.Text = "Source: Google (Disclaimer)"
                    My.Forms.Converter.LinkLabel1.Links.Add(0, 14, "https://www.google.com/search?q=" + input)
                    My.Forms.Converter.LinkLabel1.Links.Add(16, 12, currencydisclaimer)
                    My.Forms.Converter.Show()
                Catch ex As Exception
                    My.Forms.Converter.Label1.Text = original
                    My.Forms.Converter.Label3.Text = converted
                    Dim currencydisclaimer As String = "https://www.google.com/intl/en/help/currency_disclaimer.html"
                    My.Forms.Converter.LinkLabel1.Text = "Source: Google (Disclaimer)"
                    My.Forms.Converter.LinkLabel1.Links.Add(0, 14, "https://www.google.com/search?q=" + input)
                    My.Forms.Converter.LinkLabel1.Links.Add(16, 12, currencydisclaimer)
                    My.Forms.Converter.Show()
                End Try
            ElseIf interpretationstring.Contains("<input class=""ucw_data"" value=""") Then
                Dim originalphrase As String = input
                Dim originalmeasure As String = originalphrase
                Dim newmeasure As String = originalphrase
                If input.Contains("+to+") Then
                    originalmeasure = originalmeasure.Substring(0, originalmeasure.IndexOf("+to+"))
                    originalmeasure = Regex.Replace(originalmeasure, "[0-9]", "")
                    originalmeasure = originalmeasure.Replace("+", "")
                    newmeasure = newmeasure.Remove(0, newmeasure.IndexOf("+to+") + 4)
                    newmeasure = newmeasure.Replace("+", "")
                ElseIf input.Contains("+in+") Then
                    originalmeasure = originalmeasure.Substring(0, originalmeasure.IndexOf("+in+"))
                    originalmeasure = Regex.Replace(originalmeasure, "[0-9]", "")
                    originalmeasure = originalmeasure.Replace("+", "")
                    newmeasure = newmeasure.Remove(0, newmeasure.IndexOf("+in+") + 4)
                    newmeasure = newmeasure.Replace("+", "")
                ElseIf input.Contains("+into+") Then
                    originalmeasure = originalmeasure.Substring(0, originalmeasure.IndexOf("+into+"))
                    originalmeasure = Regex.Replace(originalmeasure, "[0-9]", "")
                    originalmeasure = originalmeasure.Replace("+", "")
                    newmeasure = newmeasure.Remove(0, newmeasure.IndexOf("+into+") + 6)
                    newmeasure = newmeasure.Replace("+", "")
                End If
                Dim originalvalue As String = interpretationstring
                originalvalue = originalvalue.Remove(0, originalvalue.IndexOf("<input class=""ucw_data"" value="""))
                originalvalue = originalvalue.Replace("<input class=""ucw_data"" value=""", "")
                originalvalue = originalvalue.Substring(0, originalvalue.IndexOf(""""))
                Dim newvalue As String = interpretationstring
                newvalue = newvalue.Remove(0, newvalue.IndexOf("<div class=""side_div"" id=""rhs_div"">"))
                newvalue = newvalue.Remove(0, newvalue.IndexOf("<input class=""ucw_data"" value="""))
                newvalue = newvalue.Replace("<input class=""ucw_data"" value=""", "")
                newvalue = newvalue.Substring(0, newvalue.IndexOf(""""))
                originalvalue = originalvalue.Trim()
                originalmeasure = originalmeasure.Trim()
                newvalue = newvalue.Trim()
                newmeasure = newmeasure.Trim()
                Dim finaloriginal As String = originalvalue + " " + originalmeasure
                Dim finalnew As String = newvalue + " " + newmeasure
                My.Forms.Converter.Label1.Text = finaloriginal
                My.Forms.Converter.Label3.Text = finalnew
                My.Forms.Converter.LinkLabel1.Links.Add(0, 14, "https://www.google.com/search?q=" + input)
                My.Forms.Converter.Show()
            ElseIf interpretationstring.Contains("images/icons/onebox/calculator-40.gif") Then
                Dim conversionstring As String = interpretationstring
                conversionstring = conversionstring.Remove(0, conversionstring.IndexOf("images/icons/onebox/calculator-40.gif"))
                conversionstring = conversionstring.Remove(0, conversionstring.IndexOf("<h2 class=r style=""font-size:138%"" >"))
                conversionstring = conversionstring.Replace("<h2 class=r style=""font-size:138%"" >", "")
                conversionstring = conversionstring.Substring(0, conversionstring.IndexOf("</h2>"))
                conversionstring = conversionstring.Replace("<b>", "")
                conversionstring = conversionstring.Replace("</b>", "")
                Dim originalmeasure As String = conversionstring
                Dim newmeasure As String = conversionstring
                originalmeasure = originalmeasure.Substring(0, originalmeasure.IndexOf("="))
                originalmeasure = originalmeasure.Trim()
                newmeasure = newmeasure.Substring(newmeasure.IndexOf("="), newmeasure.Length - newmeasure.IndexOf("="))
                originalmeasure = originalmeasure.Trim()
                My.Forms.Converter.Label1.Text = originalmeasure
                My.Forms.Converter.Label3.Text = newmeasure
                My.Forms.Converter.LinkLabel1.Links.Add(0, 14, "https://www.google.com/search?q=" + input)
                My.Forms.Converter.Show()
            Else
                My.Forms.Converter.Label1.Text = "Invalid Conversion"
                My.Forms.Converter.Label3.Text = "Invalid Conversion"
                My.Forms.Converter.LinkLabel1.Links.Add(0, 14, "https://www.google.com/search?q=" + input)
                My.Forms.Converter.Show()
            End If
            Return (Nothing)
        Catch ex As Exception
            Return Nothing
        End Try
    End Function
    Public Function Typetext(ByVal input As String)
        SendKeys.SendWait(input)
        Return Nothing
    End Function
    'Public Function CheckInstalled(ByVal Name As String) As String   
    '    If Name = "spotify" Then
    '        If softwarepath.Contains("Spotify") Then
    '            Dim i As Integer = softwarepath.IndexOf("Spotify")
    '            Dim finalpath As String = softwareinstalled(i)
    '            Return finalpath
    '        Else
    '            Return ("http://www.spotify.com/start/")
    '        End If
    '    End If
    '    Return Nothing
    'End Function

    Public Function Weatherform(ByVal input As String, ByVal specific As Boolean)
        Dim city As String
        If specific = True Then
            city = My.Settings.Location
            input = input + "+in+" + city
        End If
        Try
            Dim interpretationstring As String
            Dim request As HttpWebRequest
            Dim response As HttpWebResponse = Nothing
            Dim reader As StreamReader
            Try
                request = DirectCast(WebRequest.Create(input), HttpWebRequest)
                response = DirectCast(request.GetResponse(), HttpWebResponse)
                reader = New StreamReader(response.GetResponseStream())
                interpretationstring = reader.ReadToEnd()
            Finally
                If Not response Is Nothing Then response.Close()
            End Try
            Dim degrees As String = interpretationstring
            degrees = degrees.Remove(0, degrees.IndexOf("td rowspan=""2"" style=""font-size:140%;white-space:nowrap;vertical-align:top;padding-right:15px;font-weight:bold"">"))
            degrees = degrees.Replace("td rowspan=""2"" style=""font-size:140%;white-space:nowrap;vertical-align:top;padding-right:15px;font-weight:bold"">", "")
            degrees = degrees.Replace("&#8206;", "")
            degrees = System.Text.RegularExpressions.Regex.Replace(degrees, "</?span[^>]*>", "")
            If degrees.Substring(0, 7).Contains("F") Then
                Dim matcher As Match
                matcher = System.Text.RegularExpressions.Regex.Match(degrees, "(\d.)\D")
                Dim temporary As String = matcher.Value
                temporary = temporary.Remove(temporary.Length - 1, 1)
                Dim far As Double
                far = temporary
                far = (far - 32) * (5 / 9)
                far = Math.Round(far, MidpointRounding.AwayFromZero)
                degrees = far
            Else
                Dim match As Match
                match = System.Text.RegularExpressions.Regex.Match(degrees, "(\d.)\D")
                degrees = match.Value
            End If
            degrees = degrees + "°C"
            Dim type As String = interpretationstring
            type = type.Remove(0, type.IndexOf("><tr><td style=""white-space:nowrap;padding-right:15px;color:#666"">"))
            type = type.Replace("><tr><td style=""white-space:nowrap;padding-right:15px;color:#666"">", "")
            type = type.Replace("&#8206;", "")
            type = type.Substring(0, type.IndexOf("</td"))
            type = type.Trim
            Dim humidity As String = interpretationstring
            Dim humidmatch As Match
            humidmatch = System.Text.RegularExpressions.Regex.Match(humidity, "<td style=""(.*)>Humidity:")
            humidity = humidity.Remove(0, humidmatch.Index)
            humidity = System.Text.RegularExpressions.Regex.Replace(humidity, "<td style=""(.*)>Humidity:", "")
            humidity = "Humidity: " + humidity
            humidity = humidity.Substring(0, humidity.IndexOf("%") + 1)
            humidity = humidity.Trim
            Dim wind As String = interpretationstring
            Dim windmatch As Match
            windmatch = System.Text.RegularExpressions.Regex.Match(wind, "<td style=""(.*)>Wind:")
            wind = wind.Remove(0, windmatch.Index)
            wind = System.Text.RegularExpressions.Regex.Replace(wind, "<td style=""(.*)>Wind:", "")
            wind = wind.Remove(wind.IndexOf("<"), (wind.IndexOf(">") + 1) - wind.IndexOf("<"))
            wind = wind.Trim
            wind = "Wind: " + wind
            wind = wind.Substring(0, wind.IndexOf("</"))
            Dim image As String = interpretationstring
            image = image.Remove(0, image.IndexOf("//ssl.gstatic.com/onebox/weather"))
            image = image.Substring(0, image.IndexOf("png"))
            image = image.Insert(0, "http:")
            image = image + "png"
            Dim title As String
            title = interpretationstring
            Dim titlematch As Match
            titlematch = System.Text.RegularExpressions.Regex.Match(title, "<div class=(.*)<b>Weather</b> for <b>")
            title = title.Remove(0, titlematch.Index)
            title = System.Text.RegularExpressions.Regex.Replace(title, "<div class=(.*)<b>Weather</b> for <b>", "")
            title = title.Substring(0, title.IndexOf("</b>"))
            title = title.Trim
            title = title.Replace("+", "")
            My.Forms.Weatherbox.Label1.Text = StrConv(title, VbStrConv.ProperCase)
            My.Forms.Weatherbox.Label2.Text = degrees
            My.Forms.Weatherbox.Label2.Text = My.Forms.Weatherbox.Label2.Text.Replace("�", Chr(176))
            My.Forms.Weatherbox.Label3.Text = type
            My.Forms.Weatherbox.Label4.Text = wind
            My.Forms.Weatherbox.Label5.Text = humidity
            My.Forms.Weatherbox.LinkLabel1.Links.Add(0, 7, input)
            My.Forms.Weatherbox.PictureBox1.ImageLocation = image
            My.Forms.Weatherbox.Show()
            Return (Nothing)
        Catch ex As Exception
            Process.Start(input)
            Return (Nothing)
        End Try

    End Function
    Public Function Time(ByVal word As String)
        Try
            Dim interpretationstring As String
            Dim request As HttpWebRequest
            Dim response As HttpWebResponse = Nothing
            Dim reader As StreamReader
            Try
                word = word.Replace(" ", "+")
                request = DirectCast(WebRequest.Create("https://www.google.com/search?q=" + "time+in+" + word), HttpWebRequest)
                request.UserAgent = "Mozilla/5.0 (Windows NT 6.0; WOW64) AppleWebKit/537.17 (KHTML, like Gecko) Chrome/24.0.1312.56 Safari/537.17"
                response = DirectCast(request.GetResponse(), HttpWebResponse)
                reader = New StreamReader(response.GetResponseStream())
                interpretationstring = reader.ReadToEnd()
            Finally
                If Not response Is Nothing Then response.Close()
            End Try
            interpretationstring = interpretationstring.Remove(0, interpretationstring.IndexOf("<div class=""vk_bk vk_ans"">"))
            Dim Timecity As String = interpretationstring
            Timecity = Timecity.Replace("<div class=""vk_bk vk_ans"">", "")
            Timecity = Timecity.Substring(0, Timecity.IndexOf("</div>"))
            interpretationstring = interpretationstring.Remove(0, interpretationstring.IndexOf("<div class=""vk_gy vk_sh"">"))
            Dim datecity As String = interpretationstring
            datecity = datecity.Replace("<div class=""vk_gy vk_sh"">", "")
            datecity = datecity.Substring(0, datecity.IndexOf("</div>"))
            datecity = datecity.Replace("<span style=""white-space:nowrap"">", "")
            datecity = datecity.Replace("</span>", "")
            datecity = datecity.Replace("""", "")
            datecity = datecity.Trim()
            interpretationstring = interpretationstring.Remove(0, interpretationstring.IndexOf("<span class=""vk_gy vk_sh"">"))
            Dim city As String = interpretationstring
            city = city.Replace("<span class=""vk_gy vk_sh"">", "")
            city = city.Substring(0, city.IndexOf("</span>"))
            city = city.Replace("""", "")
            city = city.Replace("Time in", "")
            city = city.Trim()
            My.Forms.Time.Label4.Text = Timecity
            My.Forms.Time.Label5.Text = datecity
            My.Forms.Time.Label6.Text = city
            My.Forms.Time.Show()

            Return Nothing
        Catch
            Process.Start("https://www.google.com/search?q=" + "time+in+" + word)
            Return Nothing
        End Try
    End Function
    Public Function Definition(ByVal word As String)
        Try
            Dim interpretationstring As String
            Dim request As HttpWebRequest
            Dim response As HttpWebResponse = Nothing
            Dim reader As StreamReader
            Try
                word = word.Replace(" ", "+")
                request = DirectCast(WebRequest.Create("https://www.google.com/search?q=" + "define+" + word), HttpWebRequest)
                request.UserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 6.2; Trident/4.0; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; Media Center PC 6.0)"
                response = DirectCast(request.GetResponse(), HttpWebResponse)
                reader = New StreamReader(response.GetResponseStream())
                interpretationstring = reader.ReadToEnd()
            Finally
                If Not response Is Nothing Then response.Close()
            End Try
            interpretationstring = interpretationstring.Remove(0, interpretationstring.IndexOf("<div class=""lr_dct_ent"""))
            interpretationstring = interpretationstring.Substring(0, interpretationstring.IndexOf("<div class=""xpdxpnd vk_tblspacer vkc_np"""))
            If interpretationstring.Contains("<div style=""word-break:break-all""><a href=""") Then
                Dim x As Integer = interpretationstring.IndexOf("<div style=""word-break:break-all""><a href=""")
                Dim y As Integer = interpretationstring.IndexOf("</div>", x)
                y = y + 6
                interpretationstring = interpretationstring.Remove(x, y - x)

            End If
            Try
                If interpretationstring.Contains("<audio") Then
                    interpretationstring.Remove(interpretationstring.IndexOf("<span class=""lr_dct_spkr"), (interpretationstring.IndexOf("</span>", interpretationstring.IndexOf("<span class=""lr_dct_spkr")) + 7) - interpretationstring.IndexOf("<span class=""lr_dct_spkr"))
                End If
            Catch ex As Exception

            End Try
            interpretationstring = System.Text.RegularExpressions.Regex.Replace(interpretationstring, "</?a[^>]*>", "")
            interpretationstring = interpretationstring.Remove(0, interpretationstring.IndexOf("div class=""xpdxpnd"))
            interpretationstring = interpretationstring.Remove(0, interpretationstring.IndexOf(">") + 1)
            interpretationstring = System.Text.RegularExpressions.Regex.Replace(interpretationstring, "<strong>(\d*?)</strong>", "")
            interpretationstring = System.Text.RegularExpressions.Regex.Replace(interpretationstring, "<span class=""lr_dct_more_btn(.*?)</span>", "")
            Try
                interpretationstring = interpretationstring.Remove(interpretationstring.IndexOf("Origin"), interpretationstring.Length - interpretationstring.IndexOf("Origin"))
            Catch ex As Exception

            End Try
             interpretationstring = interpretationstring + "<CENTER><P>" + "<a href=" + "https://www.google.com/search?q=" + "define+" + word + ">Source: Google</a></centre></p>"
            My.Forms.Definition.Show()
            My.Forms.Definition.WebBrowser1.DocumentText = interpretationstring
            Return Nothing
        Catch
            Process.Start("https://www.google.com/search?q=" + "define+" + word)
            Return Nothing
        End Try
    End Function
    Private Sub TextBox1_KeyDown(sender As System.Object, e As System.Windows.Forms.KeyEventArgs) Handles TextBox1.KeyDown
        If e.KeyCode = Keys.Enter Then
            e.SuppressKeyPress = True
            Analysis(TextBox1.Text)
        End If
        If e.KeyCode = Keys.Space Then
            If TextBox1.Text.Length > TextBox1.SelectionStart Then
                e.SuppressKeyPress = True
                TextBox1.Text = TextBox1.Text + " "
                TextBox1.SelectionStart = TextBox1.TextLength
            End If
        End If

    End Sub

    Private Sub TextBox1_Click(sender As System.Object, e As System.EventArgs) Handles TextBox1.Click
        TextBox1.Text = ""
    End Sub

    Private Sub NotifyIcon1_MouseDoubleClick(sender As System.Object, e As System.Windows.Forms.MouseEventArgs) Handles NotifyIcon1.MouseClick
        If Me.Opacity = 0.01 Then
            Intermediary.Enabled = True
            Me.Activate()
            Me.BringToFront()
            Animater.Enabled = True
            Me.Opacity = 0.99
            Me.TopMost = True
            Me.TextBox1.Focus()
            Me.ActiveControl = TextBox1
            Me.TextBox1.Text = ""

        Else
            If Intermediary.Enabled = True Then
            Else
                Animater.Enabled = False
                Dim iCount As Integer
                For iCount = 90 To 10 Step -10
                    Me.Opacity = iCount / 100
                    Me.Refresh()
                    Threading.Thread.Sleep(50)
                Next
                Me.Opacity = 0.01
                Me.SendToBack()
            End If
        End If

    End Sub

    Private Sub Intermediary_Tick(sender As System.Object, e As System.EventArgs) Handles Intermediary.Tick
        Intermediary.Enabled = False
    End Sub


End Class
