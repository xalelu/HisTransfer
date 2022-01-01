Imports System.Threading
Imports System.Windows
Imports System.IO

Public Class Form1
    Private Delegate Sub Ctrl_StatusChanged_CallBack(ByVal Args As String)
    Public WithEvents _time As System.Windows.Forms.Timer
    Public DefaultStr As String = "請勿關閉程式, 資料正在移轉中"
    Public processStr As String = "."
    Public _main As HisTransplant

    'Public handler As New updateMsg(AddressOf updatemsg1) '關聯updateMsg的委派事件處理.
    Public Sub StartWork() Handles Me.Load
        '移轉基本流程
        '每個Access檔案, 一旦移轉完後, 即會將該檔案, 轉移到PAST_HIS 或者 PAST_ALARM目錄下

        If File.Exists(HisTransplant._tempRecordTransFile) Then File.Delete(HisTransplant._tempRecordTransFile)
        If File.Exists(HisTransplant._tempAlarmTransFile) Then File.Delete(HisTransplant._tempAlarmTransFile)
        HisTransplant.CheckHisFile = checkDirectoryMDBfile(DataType.His, Environment.CurrentDirectory & "\His")
        HisTransplant.CheckAlarmFile = checkDirectoryMDBfile(DataType.Alarm, Environment.CurrentDirectory & "\Alr")

        'Dim checkresult As DataType = CheckExecNeed() '查詢執行必要性.[DataType.His:移置歷史即時值, DataType.Alarm:移置警報, Nothing:無法移置]
        '以上必須修改..
        If HisTransplant.CheckHisFile.Count > 0 Then '不以快取檔為基準, 以實際檔案為主
            'If checkresult = DataType.His Then '預設是DataType.His(0)
            AlarmMsg.Text = "準備進行舊資料移轉" & processStr

            '主要處理部分
            _main = New HisTransplant()
            AddHandler _main.NodeStatusChanged, AddressOf Me.updatemsg '設定Log紀錄委派
            updatemsg(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") & ": 開始進行His即時值移轉!")
            AlarmMsg.Text = "準備進行(His)舊資料移轉" & processStr
            _main.startMove(DataType.His) '若警報快取檔不存在, 

            'If Not File.Exists(_main._tempAlarmTransFile) Then
            '    AlarmMsg.Text = "準備進行(His)舊資料移轉" & processStr
            '    _main.startMove(DataType.His) '若警報快取檔不存在, 
            'Else
            '    AlarmMsg.Text = "準備進行(Alarm)舊資料移轉" & processStr
            '    _main.startMove(DataType.Alarm)
            'End If
        ElseIf HisTransplant.CheckAlarmFile.Count > 0 Then
            'ElseIf checkresult = DataType.Alarm Then
            _main = New HisTransplant()
            AddHandler _main.NodeStatusChanged, AddressOf Me.updatemsg
            updatemsg(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") & ": 開始進行Alert即時值移轉!")
            AlarmMsg.Text = "準備進行(Alarm)舊資料移轉" & processStr
            _main.startMove(DataType.Alarm)

        Else
            AlarmMsg.Text = "無須移轉資料"
            System.Threading.Thread.Sleep(5000) '暫停5秒, 即關閉程式.
            Environment.Exit(Environment.ExitCode)
            Exit Sub
        End If

        '用於更新"..."而已.
        _time = New System.Windows.Forms.Timer()
        _time.Interval = 1000
        _time.Start()
    End Sub

    '查詢各個要移轉的資料是否存在.
    Public Function checkDirectoryMDBfile(_Datatype As DataType, path As String)
        Dim result As Object
        If _Datatype = DataType.His Then
            result = New Dictionary(Of String, List(Of String)) '紀錄未移轉的Access檔 [His]
            Dim theNextDirtory As DirectoryInfo() = New DirectoryInfo(path).GetDirectories()
            For Each i As DirectoryInfo In theNextDirtory
                If i.FullName.IndexOf("PAST_HIS") < 0 Then '濾掉包含PAST字樣的目錄 [因為該目錄是存放已移轉的access檔]
                    Dim theFiles As FileInfo() = i.GetFiles() 'His下的各設備目錄層下的即時歷史檔
                    For Each x As FileInfo In theFiles
                        If x.Extension = ".mdb" Then '若包含Access檔案, 代表有資料未處理完
                            If Not result.ContainsKey(i.Name) Then
                                result.Add(i.Name, New List(Of String) From {x.Name})
                            Else
                                result(i.Name).Add(x.Name)
                            End If
                        End If
                    Next
                End If
            Next

        ElseIf _Datatype = DataType.Alarm Then
            result = New List(Of String) '紀錄未移轉的Access檔 [Alarm]
            Dim theFiles As FileInfo() = New DirectoryInfo(path).GetFiles() '即當下該目錄
            For Each i As FileInfo In theFiles
                If i.Extension = ".mdb" Then
                    result.Add(i.Name)
                End If
            Next
        End If
        Return result
    End Function

    '查詢 進行設備移轉 的重要性 20201112 sam
    Public Function CheckExecNeed()
        'If File.Exists(ReadContent.HandlerLog) Then '(1)若結束紀錄存在, 則代表已完成移轉
        '    Return Nothing
        'Else
        If File.Exists(ReadContent.TransFlag) Then '(2)若歷史即時值 快取檔存在, 則回傳執行移轉His
            Return DataType.His
        ElseIf File.Exists(ReadContent.AlarmFlag) Then '(3)若警報 快取檔存在, 則回傳執行移轉Alarm
            Return DataType.Alarm
        Else
            Return DataType.None
            'Console.WriteLine("CheckExecNeed Exception")
        End If
    End Function

    Public Sub updatemsg(msg As String)
        Try
            If Me.InvokeRequired Then
                Dim callback As New Ctrl_StatusChanged_CallBack(AddressOf updatemsg)
                Me.Invoke(callback, msg)
            Else
                LogBox.AppendText(msg & Environment.NewLine)
            End If
        Catch ex As Exception
            Console.WriteLine(ex.ToString())
        End Try
    End Sub

    Private Sub showworkingstr() Handles _time.Tick
        Select Case processStr
            Case ""
                processStr = "."
            Case "."
                processStr = ".."
            Case ".."
                processStr = "..."
            Case "..."
                processStr = ""
        End Select
        AlarmMsg.Text = DefaultStr & processStr
    End Sub

    '查看快取檔案
    Private Sub checkCache_Click(sender As Object, e As EventArgs) Handles checkCache.Click
        CacheForm.Visible = True
        ReadCacheDta()
    End Sub

End Class

'讀取快取紀錄檔 20201103 sam
Public Module ReadContent
    Public TransFlag As String = Application.StartupPath & "\_tempRecordTransFile.tmp"
    Public AlarmFlag As String = Application.StartupPath & "\_tempAlarmTransFile.tmp"
    'Public HandlerLog As String = Application.StartupPath & "\_HandlerLog.tmp"

    Public Sub ReadCacheDta()
        CacheForm.cachelist.Items.Clear() '清空上次讀取的資料

        If Not FileIO.FileSystem.FileExists(TransFlag) And Not FileIO.FileSystem.FileExists(AlarmFlag) Then
            MsgBox("找不到相關檔案")
            Exit Sub
        End If

        Try
            CacheForm.cachelist.Items.Clear() '清空上次讀取的資料
            Dim ObjReader As StreamReader
            If File.Exists(TransFlag) And HisTransplant._context = DataType.His Then
                ObjReader = New StreamReader(TransFlag)
            ElseIf File.Exists(AlarmFlag) And HisTransplant._context = DataType.Alarm Then
                ObjReader = New StreamReader(AlarmFlag)
            End If
            Dim sLine As String = ""
            Dim arrText As New ArrayList()

            Do
                sLine = ObjReader.ReadLine()
                If Not sLine Is Nothing Then
                    arrText.Add(sLine)
                    CacheForm.cachelist.Items.Add(sLine)
                End If
            Loop Until sLine Is Nothing
            ObjReader.Close()
        Catch ex As Exception
            Console.WriteLine(ex.ToString())
        End Try
    End Sub
End Module
