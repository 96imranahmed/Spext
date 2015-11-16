Public Class Time
    Private Sub Animater_Tick(sender As System.Object, e As System.EventArgs) Handles Animater.Tick
        Animater.Enabled = False
        Dim iCount As Integer
        For iCount = 90 To 10 Step -10
            Me.Opacity = iCount / 100
            Me.Refresh()
            Threading.Thread.Sleep(50)
        Next
        Me.Close()
    End Sub
    Private Sub Time_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        Animater.Interval = My.Settings.FadeOutTimer
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

End Class