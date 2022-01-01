Imports System.IO
Imports System.Threading
Imports System.Text

Public Class TestModule
    'Public udm As updateMsg
    Public Event NodeStatusChanged(msg As String)
    Public Sub threadone()
        Dim nThread_Agent_Init As New Threading.Thread(AddressOf one)
        nThread_Agent_Init.IsBackground = True
        nThread_Agent_Init.Start()
    End Sub

    Public Sub one()
        'AddHandler udm, AddressOf Form1.updatemsg1
        'AddHandler udm, AddressOf Form1.updatemsg2
        'AddHandler udm, AddressOf Form1.updatemsg3
        'udm.Invoke("jfisiw")
        For i = 0 To 1000
            If i = 10 Or i = 20 Or i = 30 Or i = 40 Then
                RaiseEvent NodeStatusChanged(i)
                System.Threading.Thread.Sleep(2000)
            End If
        Next
        Console.WriteLine("Can you get me?{0}", Form1.ToString()) '抓得到form1
    End Sub

End Class
