<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class CacheForm
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
        Me.cachelist = New System.Windows.Forms.ListBox()
        Me.SuspendLayout()
        '
        'cachelist
        '
        Me.cachelist.BackColor = System.Drawing.Color.FromArgb(CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer), CType(CType(64, Byte), Integer))
        Me.cachelist.Font = New System.Drawing.Font("新細明體", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(136, Byte))
        Me.cachelist.ForeColor = System.Drawing.SystemColors.InactiveCaption
        Me.cachelist.FormattingEnabled = True
        Me.cachelist.ItemHeight = 12
        Me.cachelist.Location = New System.Drawing.Point(1, 0)
        Me.cachelist.Name = "cachelist"
        Me.cachelist.Size = New System.Drawing.Size(462, 124)
        Me.cachelist.TabIndex = 0
        '
        'CacheForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(462, 123)
        Me.Controls.Add(Me.cachelist)
        Me.Name = "CacheForm"
        Me.Text = "CacheForm"
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents cachelist As ListBox
End Class
