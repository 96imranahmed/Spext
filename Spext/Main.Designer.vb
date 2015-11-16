<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Main
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Main))
        Me.TriggerCheck = New System.Windows.Forms.Timer(Me.components)
        Me.TriggerHasHeldDown = New System.Windows.Forms.Timer(Me.components)
        Me.Triggerholding = New System.Windows.Forms.Timer(Me.components)
        Me.NotifyIcon1 = New System.Windows.Forms.NotifyIcon(Me.components)
        Me.TextBox1 = New System.Windows.Forms.TextBox()
        Me.Animater = New System.Windows.Forms.Timer(Me.components)
        Me.Intermediary = New System.Windows.Forms.Timer(Me.components)
        Me.SuspendLayout()
        '
        'TriggerCheck
        '
        Me.TriggerCheck.Enabled = True
        Me.TriggerCheck.Interval = 1
        '
        'TriggerHasHeldDown
        '
        Me.TriggerHasHeldDown.Interval = 500
        '
        'Triggerholding
        '
        Me.Triggerholding.Interval = 1
        '
        'NotifyIcon1
        '
        Me.NotifyIcon1.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info
        Me.NotifyIcon1.BalloonTipText = "Hold CAPS & TAB to Activate"
        Me.NotifyIcon1.BalloonTipTitle = "Spext is Running!"
        Me.NotifyIcon1.Icon = CType(resources.GetObject("NotifyIcon1.Icon"), System.Drawing.Icon)
        Me.NotifyIcon1.Text = "Spext"
        Me.NotifyIcon1.Visible = True
        '
        'TextBox1
        '
        Me.TextBox1.Location = New System.Drawing.Point(-1, 0)
        Me.TextBox1.Name = "TextBox1"
        Me.TextBox1.Size = New System.Drawing.Size(109, 20)
        Me.TextBox1.TabIndex = 0
        '
        'Animater
        '
        Me.Animater.Interval = 3000
        '
        'Intermediary
        '
        Me.Intermediary.Interval = 200
        '
        'Main
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.AutoSize = True
        Me.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.ClientSize = New System.Drawing.Size(108, 20)
        Me.Controls.Add(Me.TextBox1)
        Me.ForeColor = System.Drawing.SystemColors.ControlText
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MaximumSize = New System.Drawing.Size(200, 110)
        Me.MinimizeBox = False
        Me.MinimumSize = New System.Drawing.Size(124, 56)
        Me.Name = "Main"
        Me.ShowInTaskbar = False
        Me.Tag = ""
        Me.Text = "Spext"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents TriggerCheck As System.Windows.Forms.Timer
    Friend WithEvents TriggerHasHeldDown As System.Windows.Forms.Timer
    Friend WithEvents Triggerholding As System.Windows.Forms.Timer
    Friend WithEvents NotifyIcon1 As System.Windows.Forms.NotifyIcon
    Friend WithEvents TextBox1 As System.Windows.Forms.TextBox
    Friend WithEvents Animater As System.Windows.Forms.Timer
    Friend WithEvents Intermediary As System.Windows.Forms.Timer

End Class
