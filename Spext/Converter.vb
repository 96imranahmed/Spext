Public Class Converter

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
    Private Sub Popupbox_Click(sender As System.Object, e As System.EventArgs) Handles MyBase.Click
        Animater_Tick(Nothing, Nothing)
    End Sub

    Private Sub Converter_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
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
        If Label1.Text = "Invalid Conversion" Then
            Animater.Enabled = False
            Dim result = MessageBox.Show("It appears your conversion returned an error. This could be due to a mismatch (i.e. converting weight to a length). Would you like me to Google this conversion online?", "Conversion Error", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning)
            If result = Windows.Forms.DialogResult.OK Then
                Dim URL As String = LinkLabel1.Links(0).LinkData.ToString
                Process.Start(URL)
            End If
            Animater.Enabled = True
        End If
    End Sub
    Private Sub Converter_Click(sender As System.Object, e As System.EventArgs) Handles MyBase.Click
        Animater.Enabled = False
        Dim iCount As Integer
        For iCount = 90 To 10 Step -10
            Me.Opacity = iCount / 100
            Me.Refresh()
            Threading.Thread.Sleep(50)
        Next
        Me.Close()
    End Sub

    Private Sub LinkLabel1_LinkClicked(sender As System.Object, e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles LinkLabel1.LinkClicked
        Dim target As String = CType(e.Link.LinkData, String)
        Process.Start(target)
    End Sub
End Class