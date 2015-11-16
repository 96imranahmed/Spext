Public Class Definition
    Private Sub Animater_Tick(sender As System.Object, e As System.EventArgs) Handles Animater.Tick
        Dim pointtoclient As Point = WebBrowser1.PointToClient(MousePosition)
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

    Private Sub Definition_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load





        Animater.Interval = 2 * My.Settings.FadeOutTimer
        Me.Visible = True
        Me.Opacity = 0.99
        Dim x As Integer
        Dim y As Integer
        x = Screen.PrimaryScreen.WorkingArea.Width
        y = Screen.PrimaryScreen.WorkingArea.Height - Me.Height
        Do Until x = Screen.PrimaryScreen.WorkingArea.Width - Me.Width
            x = x - 1
            Me.Location = New Point(x, y)
        Loop
        Me.TopMost = True
        Animater.Enabled = True
    End Sub
    Private Sub Popupbox_Click(sender As System.Object, e As System.EventArgs) Handles MyBase.Click
        Animater_Tick(Nothing, Nothing)
    End Sub
    Private Sub webControl_DocumentCompleted(ByVal sender As Object, _
            ByVal e As WebBrowserDocumentCompletedEventArgs) _
            Handles WebBrowser1.DocumentCompleted
        Dim link As HtmlElement
        Dim links As HtmlElementCollection = WebBrowser1.Document.Links
        For Each link In links
            link.AttachEventHandler("onclick", AddressOf LinkClicked)
        Next
    End Sub

    Private Sub LinkClicked(ByVal sender As Object, ByVal e As EventArgs)
        Dim link As HtmlElement = WebBrowser1.Document.ActiveElement
        Dim url As String = link.GetAttribute("href")
        Process.Start(url)
    End Sub
End Class