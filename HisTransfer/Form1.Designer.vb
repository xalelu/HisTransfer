<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form 覆寫 Dispose 以清除元件清單。
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

    '為 Windows Form 設計工具的必要項
    Private components As System.ComponentModel.IContainer

    '注意: 以下為 Windows Form 設計工具所需的程序
    '可以使用 Windows Form 設計工具進行修改。
    '請勿使用程式碼編輯器進行修改。
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.LogBox = New System.Windows.Forms.TextBox()
        Me.AlarmMsg = New System.Windows.Forms.Label()
        Me.checkCache = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'LogBox
        '
        Me.LogBox.BackColor = System.Drawing.Color.FromArgb(CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer))
        Me.LogBox.Font = New System.Drawing.Font("新細明體", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(136, Byte))
        Me.LogBox.ForeColor = System.Drawing.SystemColors.InactiveCaption
        Me.LogBox.Location = New System.Drawing.Point(-2, 29)
        Me.LogBox.Margin = New System.Windows.Forms.Padding(2)
        Me.LogBox.Multiline = True
        Me.LogBox.Name = "LogBox"
        Me.LogBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.LogBox.Size = New System.Drawing.Size(365, 439)
        Me.LogBox.TabIndex = 0
        '
        'AlarmMsg
        '
        Me.AlarmMsg.AutoSize = True
        Me.AlarmMsg.BackColor = System.Drawing.Color.Red
        Me.AlarmMsg.Font = New System.Drawing.Font("新細明體", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(136, Byte))
        Me.AlarmMsg.ForeColor = System.Drawing.SystemColors.Info
        Me.AlarmMsg.Location = New System.Drawing.Point(174, 8)
        Me.AlarmMsg.Margin = New System.Windows.Forms.Padding(2, 0, 2, 0)
        Me.AlarmMsg.Name = "AlarmMsg"
        Me.AlarmMsg.Padding = New System.Windows.Forms.Padding(0, 1, 0, 1)
        Me.AlarmMsg.Size = New System.Drawing.Size(37, 14)
        Me.AlarmMsg.TabIndex = 1
        Me.AlarmMsg.Text = "Label1"
        '
        'checkCache
        '
        Me.checkCache.Enabled = False
        Me.checkCache.Font = New System.Drawing.Font("新細明體", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(136, Byte))
        Me.checkCache.Location = New System.Drawing.Point(2, 3)
        Me.checkCache.Name = "checkCache"
        Me.checkCache.Size = New System.Drawing.Size(80, 23)
        Me.checkCache.TabIndex = 2
        Me.checkCache.Text = "查看處理紀錄"
        Me.checkCache.UseVisualStyleBackColor = True
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(359, 465)
        Me.Controls.Add(Me.checkCache)
        Me.Controls.Add(Me.AlarmMsg)
        Me.Controls.Add(Me.LogBox)
        Me.Margin = New System.Windows.Forms.Padding(2)
        Me.Name = "Form1"
        Me.Text = "HisTransfer"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents LogBox As TextBox
    Friend WithEvents AlarmMsg As Label
    Friend WithEvents checkCache As Button
End Class
