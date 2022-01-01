'Imports System
'Imports System.Data.SQLite
'Imports System.IO
'Imports System.Threading
'Imports System.Text
'Imports System.Xml

'Module HisTransplant
'    Public ConnectionPath As String = My.Application.Info.DirectoryPath & "\Config\Connection.xml"
'    Public Mydirectory As String '來源目錄.
'    Public Mydestination As String '目標目錄.
'    Public Core As Integer '執行緒數.

'    Public _target As Form = Form1

'    Public endCut As Integer = 0
'    Public ThreadWork As New List(Of Thread) '封裝執行緒的容器
'    Public jobschedual As New List(Of List(Of String)) '相對應於上述執行緒要做的工作量.
'    Public working As New List(Of String) '紀錄運行中的執行緒.

'    '啟動點
'    Public Sub startMove(DT As DataType)
'        Select Case DT
'            Case DataType.His
'                Dim nThread_Agent_Init As New Threading.Thread(AddressOf Transplant_start)
'                nThread_Agent_Init.IsBackground = True
'                nThread_Agent_Init.Start()

'            Case DataType.Alarm
'                Dim nThread_Agent_Init As New Threading.Thread(AddressOf AlarmTransplant_start)
'                nThread_Agent_Init.IsBackground = True
'                nThread_Agent_Init.Start()
'        End Select
'    End Sub

'    Public Function check()
'        Dim _directory As String = Environment.CurrentDirectory & "\His" '"C:\Users\JNC\Desktop\測試" 'Console.ReadLine()
'        Dim _destination As String = Environment.CurrentDirectory & "\His" '"C:\Users\JNC\Desktop\目標" 'Console.ReadLine()
'        Core = Environment.ProcessorCount 'CType(Console.ReadLine(), Integer)

'        If Not Directory.Exists(_directory) Then
'            Exit Function
'        End If

'        Dim tbnames As List(Of DirectoryInfo) 'New List(Of DirectoryInfo) From {"a", "b"}

'        '*********** 測試下方函式用 20200124 ************
'        Dim test As List(Of DirectoryInfo) = GetAccessDirectory(_directory) '取得His下的目錄資訊.(各裝置的目錄)
'        '*********** 上述三行, 可能不會再用到. ************
'        'Core = IIf(test.Count >= Core, Core, test.Count) '若要處理的設備數量 小於 cpu的核心數量時, 則以設備數量作為建立執行緒數量的依據.
'        Return SaveNecessity(test, "double")
'    End Function

'    'His
'    Public Sub Transplant_start()
'        Try
'            'Console.WriteLine("請輸入要備份的目錄:")
'            Mydirectory = Environment.CurrentDirectory & "\His" '"C:\Users\JNC\Desktop\測試" 'Console.ReadLine()
'            'Console.WriteLine("請輸入要備份的位置:")
'            Mydestination = Environment.CurrentDirectory & "\His" '"C:\Users\JNC\Desktop\目標" 'Console.ReadLine()
'            'Console.WriteLine("輸入預備執行緒的數量:")
'            Core = Environment.ProcessorCount '取得CPU核心數

'            If Not Directory.Exists(Mydirectory) Then
'                'Console.WriteLine("抱歉, 你輸入的目錄不存在..")
'                'Form1.handler.Invoke("抱歉, 你輸入的目錄不存在..")
'                'RaiseEvent udm("抱歉, 你輸入的目錄不存在..")
'                'Form1.LogBox.AppendText("抱歉, 你輸入的目錄不存在.." & Environment.NewLine)
'                'DebugList.Add(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") & "Access to Sqlite移植作業: 抱歉, Input目錄不存在..")
'                'Debug.Add(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") & "Access to Sqlite移植作業: 抱歉, Input目錄不存在..")
'                Form1.LogBox.AppendText(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") & "Access to Sqlite移植作業: 抱歉, Input目錄不存在.." & Environment.NewLine)
'                Exit Sub
'            Else
'                'DebugList.Add("開始進行 歷史即時資料 備份 " & Mydirectory & " 至 " & Mydestination & " ...")
'                Form1.LogBox.AppendText("開始進行 歷史即時資料 備份 " & Mydirectory & " 至 " & Mydestination & " ..." & Environment.NewLine)
'            End If

'            '查詢來源資料表(His)
'            'sqliteTB.Add("CREATE TABLE test (ID Integer PRIMARY KEY, Time DateTime, Millisecond VARCHAR(10), Label0 VARCHAR(10), Label1 VARCHAR(10), Label2 VARCHAR(10), Label3 VARCHAR(10), Label4 VARCHAR(10), Label5 VARCHAR(10), Label6 VARCHAR(10), Label7 VARCHAR(10), Label8 VARCHAR(10), Label9 VARCHAR(10), Label10 VARCHAR(10), Label11 VARCHAR(10), Label12 VARCHAR(10))")
'            'SqliteHandler.initTable("C:\Users\JNC\Desktop\SQLITE", sqliteTB)

'            '*********** 測試下方函式用 20200124 ************
'            Dim DeviceMac As List(Of DirectoryInfo) = GetAccessDirectory(Mydirectory) '取得His下的目錄資訊.(各裝置的目錄)
'            '*********** 上述三行, 可能不會再用到. ************
'            'Core = IIf(DeviceMac.Count >= Core, Core, DeviceMac.Count) '若要處理的設備數量 小於 cpu的核心數量時, 則以設備數量作為建立執行緒數量的依據.

'            'Dim Pre_Work As Boolean = SaveNecessity(DeviceMac)
'            'If Pre_Work Then
'            '    DebugList.Add("現有資料覆蓋率已達99.5%, 無須進行移植作業")
'            '    System.Threading.Thread.Sleep(10000) '等侯10秒
'            '    'Environment.Exit(Environment.ExitCode)
'            '    Exit Sub
'            'End If

'            HisTransplant.ReadTemp(DataType.His) '讀取上次備份一半的紀錄檔(紀錄在_tempTransSet變數裡)
'            Core = IIf(_tempTransSet.Count >= Core, Core, _tempTransSet.Count)
'            '先行預設二個執行緒..
'            'If DeviceMac IsNot Nothing And DeviceMac.Count > 0 Then
'            If _tempTransSet IsNot Nothing And _tempTransSet.Count > 0 Then
'                Dim Index As Integer = 0

'                For i As Integer = 0 To Core - 1
'                    jobschedual.Add(New List(Of String)) '視建立的執行緒數量, 加入對應的資料庫處理列表.
'                Next
'                '分配各個裝置 到 各處理核心
'                For Each a As DirectoryInfo In DeviceMac
'                    If _tempTransSet.ContainsKey(a.Name) Then
'                        Dim sk As Integer = Index Mod Core
'                        jobschedual(sk).Add(a.Name) '目的地的 裝置..
'                        Index += 1
'                    End If
'                Next

'                For i As Integer = 0 To Core - 1
'                    Dim sthread As Thread = New Thread(AddressOf TransSqlite)

'                    sthread.Name = "CORE_" & i
'                    Console.WriteLine("已啟動" & i & "支執行緒" & sthread.Name)
'                    ThreadWork.Add(sthread)
'                    sthread.IsBackground = True

'                    Thread.Sleep(50)
'                    sthread.Start()
'                Next
'                'MyTimer.startcheck(3) '開始偵測 執行緒的處理
'            End If

'        Catch ex As Exception
'            Console.WriteLine("Exception_Main: " & ex.ToString())
'        Finally
'            'Console.ReadKey()
'        End Try
'    End Sub

'    'Alarm
'    Public Sub AlarmTransplant_start()
'        Try
'            'Console.WriteLine("請輸入要備份的目錄:")
'            Mydirectory = Environment.CurrentDirectory & "\Alr" '"C:\Users\JNC\Desktop\測試" 'Console.ReadLine()
'            'Console.WriteLine("請輸入要備份的位置:")
'            Mydestination = Environment.CurrentDirectory & "\Alr" '"C:\Users\JNC\Desktop\目標" 'Console.ReadLine()
'            'Console.WriteLine("輸入預備執行緒的數量:")
'            Core = Environment.ProcessorCount '取得CPU核心數

'            If Not Directory.Exists(Mydirectory) Then
'                Form1.LogBox.AppendText("抱歉, 你輸入的目錄不存在.." & Environment.NewLine)
'                'DebugList.Add(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") & "Alarm Access to Sqlite移植作業: 抱歉, Input目錄不存在..")
'                Form1.LogBox.AppendText(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") & "Alarm Access to Sqlite移植作業: 抱歉, Input目錄不存在.." & Environment.NewLine)
'                Exit Sub
'            Else
'                'DebugList.Add("開始進行 警報資料 備份 " & Mydirectory & " 至 " & Mydestination & " ...")
'                Form1.LogBox.AppendText("開始進行 警報資料 備份 " & Mydirectory & " 至 " & Mydestination & " ..." & Environment.NewLine)
'            End If

'            '查詢來源資料表(His)
'            'sqliteTB.Add("CREATE TABLE test (ID Integer PRIMARY KEY, Time DateTime, Millisecond VARCHAR(10), Label0 VARCHAR(10), Label1 VARCHAR(10), Label2 VARCHAR(10), Label3 VARCHAR(10), Label4 VARCHAR(10), Label5 VARCHAR(10), Label6 VARCHAR(10), Label7 VARCHAR(10), Label8 VARCHAR(10), Label9 VARCHAR(10), Label10 VARCHAR(10), Label11 VARCHAR(10), Label12 VARCHAR(10))")
'            'SqliteHandler.initTable("C:\Users\JNC\Desktop\SQLITE", sqliteTB)

'            '**** 這裡讀取Alarm對應的移植狀態, 若到99.9%, 則無須往下進行移植作業 ****

'            '*********** 測試下方函式用 20201005 ************
'            Dim AlarmFn As List(Of FileInfo) = GetAccessTB(New DirectoryInfo(Mydirectory), "*.mdb") '取得Alarm下的目錄資訊.(各裝置的目錄)
'            '*********** 上述三行, 可能不會再用到. ************
'            HisTransplant.ReadTemp(DataType.Alarm) '讀取上次備份一半的紀錄檔(紀錄在_tempTransSet 或 _tempAlarmSet變數裡)
'            Core = IIf(_tempAlarmSet.Count >= Core, Core, _tempAlarmSet.Count)

'            Console.WriteLine("check that: " & _tempAlarmSet.Count)
'            '-----------------------------------------以下要重構--------------------------------------------------
'            '先行預設二個執行緒..
'            If _tempAlarmSet IsNot Nothing And _tempAlarmSet.Count > 0 Then
'                Dim Index As Integer = 0

'                For i As Integer = 0 To Core - 1 '視建立的執行緒數量, 加入對應的資料庫處理列表.
'                    jobschedual.Add(New List(Of String))
'                Next

'                Console.WriteLine("check alarm1:" & _tempAlarmSet.Count)
'                '將所屬的年月的檔案 分配到各個裝置處理核心
'                For Each a As FileInfo In AlarmFn
'                    Dim thetargetdate As String = a.Name.Substring(0, 6)
'                    If _tempAlarmSet.ContainsKey(thetargetdate) Then
'                        Dim sk As Integer = Index Mod Core
'                        jobschedual(sk).Add(a.Name) '目的地的 裝置..
'                        Index += 1
'                    End If
'                Next

'                Console.WriteLine("check alarm2: " & jobschedual.Count)
'                '逐一啟動執行
'                For i As Integer = 0 To Core - 1
'                    Dim sthread As Thread = New Thread(AddressOf TransAlarmSqlite)

'                    sthread.Name = "CORE_" & i
'                    ThreadWork.Add(sthread)
'                    sthread.IsBackground = True

'                    Thread.Sleep(100)
'                    ThreadWork(i).Start()
'                Next
'                'MyTimer.startcheck(3) '開始偵測 執行緒的處理
'            End If

'        Catch ex As Exception
'            Console.WriteLine("Exception_Main: " & ex.ToString())
'        Finally
'            'Console.ReadKey()
'        End Try
'    End Sub

'    '將所有警報檔案依年月過濾出來 20201007[無用]
'    'Private Function distributionCore(FI As List(Of FileInfo))
'    '    Dim YM As New List(Of String)
'    '    For Each i As FileInfo In FI
'    '        If Not YM.Contains(i.Name.Substring(0, 6)) Then
'    '            YM.Add(i.Name.Substring(0, 6))
'    '        End If
'    '    Next
'    '    Return YM
'    'End Function

'    Public LockJobschedual As Object = "sam" 'Lock the target to handler it.

'    '取得對應Mac底下各時間的資料庫檔案.
'    Public Sub TransSqlite() '如何取得運行它的執行緒Name.會做到重覆的動作.
'        Dim target As New List(Of String) 'His: 目錄
'        SyncLock LockJobschedual
'            target = jobschedual(0)
'            jobschedual.RemoveAt(0)
'        End SyncLock

'        If target.Count > 0 Then
'            For i As Integer = 0 To target.Count - 1 'His目錄下的裝置名稱
'                Dim origin As String = Mydirectory & "\" & target(i) '來源
'                Dim destin As String = Mydestination & "\" & target(i) '目標

'                Directory.CreateDirectory(destin) '在目的地建立裝置名稱的目錄夾
'                'Dim new_tablename As New List(Of String) '建立新資料庫檔案名稱
'                Dim vos As DirectoryInfo = New DirectoryInfo(origin)
'                ProcessCopy_transcation2(vos, destin) '這裡由多執行進行處理.
'            Next
'        Else
'            Console.WriteLine("the value is nothing..")
'        End If
'    End Sub

'    '抓取Access的資料
'    Public Sub TransAlarmSqlite()
'        Dim target As New List(Of String) 'Alarm: 檔案
'        SyncLock LockJobschedual
'            target = jobschedual(0)
'            jobschedual.RemoveAt(0)
'        End SyncLock

'        Dim macs As List(Of String) = GetConnectionMac(ConnectionPath) '取得設備列表索引.

'        Try
'            If target.Count > 0 Then
'                Dim ACCESSER As AccessHandler
'                For i As Integer = 0 To target.Count - 1 'Alr目錄下的裝置名稱
'                    Dim origin As String = Mydirectory & "\" & target(i) '來源
'                    Dim destin As String = origin.Replace(".mdb", ".db3") 'Mydestination & "\" & target(i) '目標

'                    '查詢有無該DB3檔案, 無則建立之("yyyymmdd.db3")
'                    If Not File.Exists(destin) Then
'                        If GenernalCreateTableAlarmSql(destin) Then '若建立SQLITE資料庫檔成功, 若沒有該年月Sqlite檔案, 即建立
'                            Console.WriteLine("成功建立資料表: " & destin)
'                        Else
'                            'DebugList.Add("建立資料表失敗: " & destin)
'                            Form1.LogBox.AppendText("建立資料表失敗: " & destin & Environment.NewLine)
'                            Continue For
'                        End If
'                    End If

'                    '****-------------------------------------------------------------------20201005 sam
'                    For Each j As KeyValuePair(Of String, String) In _tempAlarmSet
'                        If Compare(target(i), j.Value) >= 0 Then
'                            Dim td As New Dictionary(Of String, DataTable)
'                            '這裡來進行
'                            ACCESSER = New AccessHandler(origin) '進入到access的處理.
'                            Dim Acked_tb As DataTable = ACCESSER.GetData("SELECT * FROM Acked;")
'                            td.Add("Acked", Acked_tb)

'                            ACCESSER = New AccessHandler(origin)
'                            Dim Alarm_tb As DataTable = ACCESSER.GetData("SELECT * FROM Alarm;")
'                            td.Add("Alarm", Alarm_tb)

'                            ACCESSER = New AccessHandler(origin)
'                            Dim History_tb As DataTable = ACCESSER.GetData("SELECT * FROM History;")
'                            td.Add("History", History_tb)

'                            Console.WriteLine("Debug: " & Acked_tb.Rows(0).ItemArray.Length) '查詢資料的抓取.

'                            Dim PPT As String = ""
'                            For Each a As KeyValuePair(Of String, DataTable) In td
'                                Select Case a.Key
'                                    Case "Acked"
'                                        For Each b As DataRowCollection In a.Value.Rows
'                                            PPT &= "INSERT INTO Acked(""StartTime"", ""EndTime"", ""AckTime"", ""Value"", ""Label"", ""TagName"", ""Message"", ""Type"", ""Note"", ""Personnel"") VALUES(""" & b.Item(1).ToString() & """, """ & b.Item(2).ToString() & """, """ & b.Item(3).ToString() & """, """ & b.Item(4).ToString() & """, """ & recombineLabelValue(macs, b.Item(5).ToString()) & """, """ & b.Item(6).ToString() & """, """ & b.Item(7).ToString() & """, """ & b.Item(8).ToString() & """, """ & b.Item(9).ToString() & """, """ & b.Item(2).ToString() & """);"
'                                        Next
'                                    Case "Alarm"
'                                        For Each b As DataRowCollection In a.Value.Rows
'                                            PPT &= "INSERT INTO Alarm(""StartTime"", ""EndTime"", ""AckTime"", ""Value"", ""Label"", ""TagName"", ""Message"", ""Type"", ""Note"", ""Personnel"") VALUES(""" & b.Item(1).ToString() & """, """ & b.Item(2).ToString() & """, """ & b.Item(3).ToString() & """, """ & b.Item(4).ToString() & """, """ & recombineLabelValue(macs, b.Item(5).ToString()) & """, """ & b.Item(6).ToString() & """, """ & b.Item(7).ToString() & """, """ & b.Item(8).ToString() & """, """ & b.Item(9).ToString() & """, """ & b.Item(2).ToString() & """);"
'                                        Next
'                                    Case "History"
'                                        For Each b As DataRowCollection In a.Value.Rows
'                                            PPT &= "INSERT INTO History(""StartTime"", ""EndTime"", ""AckTime"", ""Value"", ""Label"", ""TagName"", ""Message"", ""Type"", ""Note"", ""Personnel"") VALUES(""" & b.Item(1).ToString() & """, """ & b.Item(2).ToString() & """, """ & b.Item(3).ToString() & """, """ & b.Item(4).ToString() & """, """ & recombineLabelValue(macs, b.Item(5).ToString()) & """, """ & b.Item(6).ToString() & """, """ & b.Item(7).ToString() & """, """ & b.Item(8).ToString() & """, """ & b.Item(9).ToString() & """, """ & b.Item(2).ToString() & """);"
'                                        Next
'                                End Select
'                            Next

'                            Console.WriteLine("check the ppt: " & PPT)

'                            '以下執行警報資料的主要移植動作.
'                            Dim conn As SQLiteConnection = New SQLiteConnection("Data Source=" & destin & ";Pooling=true;FailIfMissing=false")
'                            Dim cmd As SQLiteCommand = conn.CreateCommand()

'                            RecordHandler(j.Key, j.Value, TransAction.Add, DataType.Alarm) '紀錄即將要移植的access檔
'                            conn.Open()
'                            Using tran As SQLiteTransaction = conn.BeginTransaction()
'                                Try
'                                    '(1)寫法 1 
'                                    'For Each v As String In i.Value
'                                    '    cmd.CommandText = v '"INSERT INTO AllData(Time, Millisecond, " & getLabelStr() & ") VALUES(" & v & ");" 'SqlQuery(a)
'                                    '    Dim result As Integer = cmd.ExecuteNonQuery() '若是寫入或更新的話, 即會回傳寫入更新的筆數.
'                                    'Next

'                                    '(2)寫法2 [有rollback的情況發生]
'                                    cmd.CommandText = PPT '"INSERT INTO AllData(Time, Millisecond, " & getLabelStr() & ") VALUES(" & v & ");" 'SqlQuery(a)
'                                    Dim result As Integer = cmd.ExecuteNonQuery() '若是寫入或更新的話, 即會回傳寫入更新的筆數.

'                                    tran.Commit()
'                                Catch ex As Exception
'                                    tran.Rollback() '執行失敗, 則RollBack.
'                                End Try
'                            End Using
'                            RecordHandler(j.Key, j.Value, TransAction.Remove, DataType.Alarm) '移除已完成的檔案紀錄
'                            conn.Close() '關閉
'                        End If
'                    Next

'                    '****-------------------------------------------------------------------20201005 sam
'                Next
'            Else
'                Console.WriteLine("the value is nothing..")
'            End If
'        Catch ex As Exception
'            Console.WriteLine("The all: " & ex.ToString())
'        End Try
'    End Sub

'    '重組Label的value.
'    Private Function recombineLabelValue(ByRef macorigin As List(Of String), ByRef Label As String)
'        If Label.IndexOf("Label") = 0 Then
'            '格式
'            Return macorigin(getMacFormLabel(Label, MathType.quotient)) & "Chr(1)" & getMacFormLabel(Label, MathType.remainder)
'        Else
'            Return Label
'        End If
'    End Function

'    '抓取connect.xml檔 20201007 sam 'List(mac) [重併SQLITE的QUERY用]
'    Private Function GetConnectionMac(filename As String)
'        If File.Exists(filename) Then
'            Dim macs As New List(Of String)

'            Dim doc As XmlDocument = New XmlDocument()
'            doc.Load(filename)
'            Dim reader As XmlNodeReader = New XmlNodeReader(doc)
'            While reader.Read()
'                Select Case reader.NodeType
'                    Case XmlNodeType.Element
'                        If reader.Name = "Mac" Then
'                            'Console.WriteLine("標籤名稱 {0}, 標籤內容 {1}", reader.Name, reader.ReadInnerXml())
'                            macs.Add(reader.ReadInnerXml())
'                        End If
'                End Select
'            End While
'            Return macs
'        Else
'            Return Nothing
'        End If
'    End Function

'    '依據Label抓取對應的Mac值 20201007 sam [重併SQLITE的QUERY用]
'    Private Function getMacFormLabel(label As String, type As MathType)
'        Select Case type
'            Case MathType.quotient '取商數 [抓其對應的設備索引]
'                Return (CType(label.Substring(6), Integer)) Mod 120
'            Case MathType.remainder '取餘數 [重新指定頻道索引]
'                Return (CType(label.Substring(6), Integer)) \ 120
'        End Select
'    End Function

'    Public Function CheckEnd() As Boolean '查詢是否處理結束.
'        Dim result As Boolean = True
'        For i As Integer = 0 To ThreadWork.Count - 1
'            If ThreadWork(i).ThreadState = ThreadState.Aborted Then
'                result = False
'                Exit For
'            End If
'        Next
'        Return result
'    End Function

'    '改用交易處理 20200831 sam (一次整理的月處理量)
'    Public Sub ProcessCopy_transcation(a As DirectoryInfo, cretNewDirect As String)
'        Dim test2 As List(Of DirectoryInfo) = GetAccessDirectory(a.FullName) '(這裡已是下一層目錄(裝置名稱)的處理.)
'        Dim querylist As New Dictionary(Of String, List(Of String)) '紀錄要處理的sqlite語法. 年月為Key
'        Try
'            If test2 IsNot Nothing And test2.Count > 0 Then
'                Console.WriteLine("right here.")
'            Else
'                Dim tmptbname As String = ""
'                Dim tableFile As List(Of FileInfo) = GetAccessTB(a, "*.mdb") '抓取副檔名為.mdb的檔案
'                Dim _tableFile As New List(Of FileInfo)
'                'DebugList.Add("閞始讀取Access資料")
'                Form1.LogBox.AppendText("閞始讀取Access資料" & Environment.NewLine)
'                'DebugList.Add(DateTime.Now.ToString("yyyy/MM/dd HH:m:ss") & " (" & Thread.CurrentThread.ManagedThreadId.ToString() & "): 開始讀取Access資料")
'                Form1.LogBox.AppendText(DateTime.Now.ToString("yyyy/MM/dd HH:m:ss") & " (" & Thread.CurrentThread.ManagedThreadId.ToString() & "): 開始讀取Access資料" & Environment.NewLine)
'                '****-------------------------------------------------------------------20201005 sam
'                If _tempTransSet.ContainsKey(a.Name) Then '查詢 紀錄快取表裡是否有該Mac.
'                    '比對b.name 及 紀錄快取表裡的檔案.
'                    For Each b As FileInfo In tableFile
'                        If Compare(b.Name, _tempTransSet(a.Name)) >= 0 Then '比對上次紀錄的 快取紀錄檔內的日期, 
'                            _tableFile.Add(b)
'                        End If
'                    Next
'                Else
'                    _tableFile = tableFile
'                End If
'                '****-------------------------------------------------------------------20201005 sam

'                If _tableFile.Count > 0 Then
'                    For Each b As FileInfo In _tableFile
'                        '整理其資料庫檔案的檔名.
'                        tmptbname = Mid(b.Name, 1, 6) '取得到月份的檔案名稱.
'                        Dim db3file As String = cretNewDirect & "\" & tmptbname & ".db3"
'                        If Not querylist.ContainsKey(db3file) Then
'                            querylist.Add(db3file, New List(Of String))
'                        End If

'                        If Not File.Exists(db3file) Then
'                            If GenernalCreateTableSql(db3file) Then '若建立SQLITE資料庫檔成功, 若沒有該年月Sqlite檔案, 即建立
'                                Console.WriteLine("成功建立資料表: " & db3file)
'                            Else
'                                'DebugList.Add("建立資料表失敗: " & db3file)
'                                Form1.LogBox.AppendText("建立資料表失敗: " & db3file & Environment.NewLine)
'                                Continue For
'                            End If
'                        End If

'                        '(1) 以下為Access的操作.
'                        Dim ACCESSER As AccessHandler = New AccessHandler(b.FullName) '進入到access的處理.
'                        Dim tb As DataTable = ACCESSER.GetData("SELECT * FROM AllData;")
'                        Dim PPT As String = ""
'                        For v As Integer = 0 To tb.Rows.Count - 1
'                            'querylist(db3file).Add("INSERT INTO AllData(Time, Millisecond, " & getLabelStr() & ") VALUES(" & combineRowValue(tb.Rows(v)) & ");")
'                            PPT &= "INSERT INTO AllData(Time, Millisecond, " & getLabelStr() & ") VALUES(" & combineRowValue(tb.Rows(v)) & ");"
'                        Next
'                        querylist(db3file).Add(PPT)
'                    Next
'                    'DebugList.Add(" *********************** His移植前置作業完畢, 移轉中 *********************** ")
'                    Form1.LogBox.AppendText(" *********************** His移植前置作業完畢, 移轉中 *********************** " & Environment.NewLine)
'                Else
'                    'DebugList.Add(a.FullName & " 找不到相關.db3檔.")
'                    Form1.LogBox.AppendText(a.FullName & " 找不到相關.db3檔." & Environment.NewLine)
'                    Exit Sub
'                End If

'                'DebugList.Add("結束讀取Access資料")
'                Form1.LogBox.AppendText("結束讀取Access資料" & Environment.NewLine)
'                'DebugList.Add(DateTime.Now.ToString("yyyy/MM/dd HH:m:ss") & " (" & Thread.CurrentThread.ManagedThreadId.ToString() & "): 結束讀取Access資料")
'                Form1.LogBox.AppendText(DateTime.Now.ToString("yyyy/MM/dd HH:m:ss") & " (" & Thread.CurrentThread.ManagedThreadId.ToString() & "): 結束讀取Access資料" & Environment.NewLine)
'                '測試時間
'                'DebugList.Add("閞始進行Sqlite批次寫入處理")
'                Form1.LogBox.AppendText("閞始進行Sqlite批次寫入處理" & Environment.NewLine)
'                'DebugList.Add(DateTime.Now.ToString("yyyy/MM/dd HH:m:ss") & " (" & Thread.CurrentThread.ManagedThreadId.ToString() & "): 進行Sqlite寫入處理")
'                Form1.LogBox.AppendText(DateTime.Now.ToString("yyyy/MM/dd HH:m:ss") & " (" & Thread.CurrentThread.ManagedThreadId.ToString() & "): 進行Sqlite寫入處理" & Environment.NewLine)
'                '以下開始處理
'                coreHandler(querylist)

'                'DebugList.Add("結束進行Sqlite批次寫入處理")
'                Form1.LogBox.AppendText("結束進行Sqlite批次寫入處理" & Environment.NewLine)
'                'DebugList.Add(DateTime.Now.ToString("yyyy/MM/dd HH:m:ss") & " (" & Thread.CurrentThread.ManagedThreadId.ToString() & "): 完成Sqlite寫入處理")
'                Form1.LogBox.AppendText(DateTime.Now.ToString("yyyy/MM/dd HH:m:ss") & " (" & Thread.CurrentThread.ManagedThreadId.ToString() & "): 完成Sqlite寫入處理" & Environment.NewLine)
'            End If

'        Catch ex As Exception
'            Console.WriteLine("ProcessCopy_transcation: " & ex.Message)
'            Console.WriteLine("ProcessCopy_transcation: " & ex.StackTrace)
'        Finally
'            'DebugList.Add("現在執行緒: " & Thread.CurrentThread.Name)
'            Form1.LogBox.AppendText("現在執行緒: " & Thread.CurrentThread.Name & Environment.NewLine)
'            delThread(Thread.CurrentThread.Name) '刪除紀錄中的執行緒

'            '以下這段改由MyTimer去判斷
'            If CheckEnd() Then '若為True, 代表所有執行isAlive = false
'                '刪除紀錄移植檔
'                remove_tempRecordTransFile(DataType.His)
'                'DebugList.Add("移植作業結束.")
'                Form1.LogBox.AppendText("移植作業結束." & Environment.NewLine)
'                Environment.Exit(Environment.ExitCode) '強制終止程序
'            End If
'        End Try
'    End Sub

'    '改用交易處理 20201006 sam (改一天處理) ------------------------------------------------ 
'    Public Sub ProcessCopy_transcation2(a As DirectoryInfo, cretNewDirect As String)
'        'Dim sqlite_query As List(Of String) = New List(Of String)()
'        Dim test2 As List(Of DirectoryInfo) = GetAccessDirectory(a.FullName) '(這裡已是下一層目錄(裝置名稱)的處理.)
'        Dim querylist As New Dictionary(Of String, List(Of String)) '紀錄要處理的sqlite語法. 年月為Key
'        Try
'            If test2 IsNot Nothing And test2.Count > 0 Then
'                Console.WriteLine("right here.")
'            Else
'                Dim tmptbname As String = ""
'                Dim tableFile As List(Of FileInfo) = GetAccessTB(a, "*.mdb") '抓取副檔名為.mdb的檔案
'                Dim _tableFile As New List(Of FileInfo)
'                'DebugList.Add("閞始讀取Access資料")
'                Form1.LogBox.AppendText("閞始讀取Access資料" & Environment.NewLine)
'                'DebugList.Add(DateTime.Now.ToString("yyyy/MM/dd HH:m:ss") & " (" & Thread.CurrentThread.ManagedThreadId.ToString() & "): 開始讀取Access資料")
'                Form1.LogBox.AppendText(DateTime.Now.ToString("yyyy/MM/dd HH:m:ss") & " (" & Thread.CurrentThread.ManagedThreadId.ToString() & "): 開始讀取Access資料" & Environment.NewLine)
'                '比對上次移植中斷的儲存點.
'                '****-------------------------------------------------------------------20201005 sam
'                If _tempTransSet.ContainsKey(a.Name) Then '查詢 紀錄快取表裡是否有該Mac.
'                    '比對b.name 及 紀錄快取表裡的檔案.
'                    For Each b As FileInfo In tableFile
'                        If Compare(b.Name, _tempTransSet(a.Name)) >= 0 Then '比對上次紀錄的 快取紀錄檔內的日期, 
'                            _tableFile.Add(b)
'                        End If
'                    Next
'                Else
'                    _tableFile = tableFile
'                End If
'                '****-------------------------------------------------------------------20201005 sam

'                If _tableFile.Count > 0 Then
'                    For Each b As FileInfo In _tableFile
'                        '整理其資料庫檔案的檔名.
'                        tmptbname = Mid(b.Name, 1, 6) '取得到月份的檔案名稱.
'                        Dim db3file As String = cretNewDirect & "\" & tmptbname & ".db3" '移植到目標sqlite檔案
'                        If Not querylist.ContainsKey(db3file) Then
'                            querylist.Add(db3file, New List(Of String))
'                        End If

'                        If Not File.Exists(db3file) Then
'                            If GenernalCreateTableSql(db3file) Then '若建立SQLITE資料庫檔成功, 若沒有該年月Sqlite檔案, 即建立
'                                Console.WriteLine("成功建立資料表: " & db3file)
'                            Else
'                                'DebugList.Add("建立資料表失敗: " & db3file)
'                                Form1.LogBox.AppendText("建立資料表失敗: " & db3file & Environment.NewLine)
'                                Continue For
'                            End If
'                        End If

'                        'Console.WriteLine("(1)" & b.Name.Replace("/", "-"))
'                        'Console.WriteLine("(2)" & chkAccessDayCounts(b.FullName))
'                        'Console.WriteLine("(3)" & chkSqliteDayCounts(db3file).Count)

'                        '*** 以下逐一移植. 20201006 sam ***
'                        'Dim checkresult As Boolean = compareDayCount(b.Name.Replace("/", "-"), chkAccessDayCounts(b.FullName), chkSqliteDayCounts(db3file))  '取得sqlite的該月各日期的總筆數.
'                        'If Not checkresult Then
'                        RecordHandler(a.Name, b.FullName, TransAction.Add, DataType.His) '紀錄即將要移植的access檔 **************************
'                        '(1) 以下為Access的操作.
'                        Dim ACCESSER As AccessHandler = New AccessHandler(b.FullName) '進入到access的處理.
'                        Dim tb As DataTable = ACCESSER.GetData("SELECT * FROM AllData;")
'                        Dim PPT As String = ""
'                        For v As Integer = 0 To tb.Rows.Count - 1
'                            'querylist(db3file).Add("INSERT INTO AllData(Time, Millisecond, " & getLabelStr() & ") VALUES(" & combineRowValue(tb.Rows(v)) & ");")
'                            PPT &= "INSERT INTO AllData(Time, Millisecond, " & getLabelStr() & ") VALUES(" & combineRowValue(tb.Rows(v)) & ");"
'                        Next
'                        querylist(db3file).Add(PPT)

'                        '******** 以下開始處理 ********
'                        'DebugList.Add("結束讀取Access資料")
'                        Form1.LogBox.AppendText("結束讀取Access資料" & Environment.NewLine)
'                        'DebugList.Add(DateTime.Now.ToString("yyyy/MM/dd HH:m:ss") & " (" & Thread.CurrentThread.ManagedThreadId.ToString() & "): 結束讀取Access資料")
'                        Form1.LogBox.AppendText(DateTime.Now.ToString("yyyy/MM/dd HH:m:ss") & " (" & Thread.CurrentThread.ManagedThreadId.ToString() & "): 結束讀取Access資料" & Environment.NewLine)
'                        '測試時間
'                        'DebugList.Add("閞始進行Sqlite批次寫入處理")
'                        Form1.LogBox.AppendText("閞始進行Sqlite批次寫入處理" & Environment.NewLine)
'                        'DebugList.Add(DateTime.Now.ToString("yyyy/MM/dd HH:m:ss") & " (" & Thread.CurrentThread.ManagedThreadId.ToString() & "): 進行Sqlite寫入處理")
'                        Form1.LogBox.AppendText(DateTime.Now.ToString("yyyy/MM/dd HH:m:ss") & " (" & Thread.CurrentThread.ManagedThreadId.ToString() & "): 進行Sqlite寫入處理" & Environment.NewLine)
'                        coreHandler(querylist)

'                        querylist.Clear()
'                        RecordHandler(a.Name, b.FullName, TransAction.Remove, DataType.His) '移除已完成的檔案紀錄 ***************
'                        'DebugList.Add("結束進行Sqlite批次寫入處理")
'                        Form1.LogBox.AppendText("結束進行Sqlite批次寫入處理" & Environment.NewLine)
'                        'DebugList.Add(DateTime.Now.ToString("yyyy/MM/dd HH:m:ss") & " (" & Thread.CurrentThread.ManagedThreadId.ToString() & "): 完成Sqlite寫入處理")
'                        Form1.LogBox.AppendText(DateTime.Now.ToString("yyyy/MM/dd HH:m:ss") & " (" & Thread.CurrentThread.ManagedThreadId.ToString() & "): 完成Sqlite寫入處理" & Environment.NewLine)
'                        'End If
'                    Next
'                    'DebugList.Add(" *********************** His移植前置作業完畢, 移轉中 *********************** ")
'                    Form1.LogBox.AppendText(" *********************** His移植前置作業完畢, 移轉中 *********************** " & Environment.NewLine)
'                Else
'                    'DebugList.Add(a.FullName & " 找不到相關.db3檔.")
'                    Form1.LogBox.AppendText(a.FullName & " 找不到相關.db3檔." & Environment.NewLine)
'                    Exit Sub
'                End If

'            End If

'        Catch ex As Exception
'            Console.WriteLine("ProcessCopy_transcation: " & ex.Message)
'            Console.WriteLine("ProcessCopy_transcation: " & ex.StackTrace)
'        Finally
'            'DebugList.Add("現在執行緒: " & Thread.CurrentThread.Name)
'            Form1.LogBox.AppendText("現在執行緒: " & Thread.CurrentThread.Name & Environment.NewLine)
'            delThread(Thread.CurrentThread.Name) '刪除紀錄中的執行緒

'            '以下這段改由MyTimer去判斷
'            If CheckEnd() Then '若為True, 代表所有執行isAlive = false
'                '刪除紀錄移植檔
'                remove_tempRecordTransFile(DataType.His)
'                'DebugList.Add("移植作業結束.")
'                Form1.LogBox.AppendText("移植作業結束." & Environment.NewLine)
'                Environment.Exit(Environment.ExitCode) '強制終止程序
'            End If
'        End Try
'    End Sub

'    '比對Access與Sqlite特定天數的資料筆數. 20201006 sam
'    'SQLITEDAYDTA的Key為: 2020-09
'    Private Function compareDayCount(dayname As String, daycount As Integer, ByRef SQLITEDAYDTA As Dictionary(Of String, Integer))
'        Try
'            If SQLITEDAYDTA Is Nothing OrElse daycount = 0 Then Return False 'SQLITEDAYDTA若是空值 或 daycount等於0, 代表完成沒有移植過.
'            If daycount > 0 And SQLITEDAYDTA.ContainsKey(dayname) Then
'                Return IIf(SQLITEDAYDTA(dayname) = daycount, True, False) '若數量一致, 則無須移置(true)
'            Else
'                Return False '須進行移植.
'            End If
'        Catch ex As Exception
'            Console.WriteLine("compareDayCount: " & ex.Message)
'        End Try
'    End Function
'    '取得Access當天的資料筆數(備用) 20201006 sam
'    Private Function chkAccessDayCounts(dbfile As String)
'        Try
'            Dim ACCESSER As AccessHandler = New AccessHandler(dbfile) '進入到access的處理.
'            Dim tb As DataTable = ACCESSER.GetData("SELECT Count(*) FROM AllData;")
'            If tb IsNot Nothing And tb.Rows.Count > 0 Then
'                Return CType(tb.Rows(0).ItemArray(0), Integer)
'            Else
'                Return 0
'            End If
'        Catch ex As Exception
'            Console.WriteLine(ex.ToString())
'        End Try
'    End Function
'    '取得sqlite各當月 日期的筆數(備用) 20201006 sam
'    Private Function chkSqliteDayCounts(dbfile As String) '2020-08-22, 221
'        Try
'            Dim TB As DataTable = SqliteHandler._ExecSQLs(dbfile, "SELECT substr(Time, 0, 11), COUNT(*) FROM AllData Group by substr(Time,0,11);", 1)
'            If TB IsNot Nothing And TB.Rows.Count > 0 Then
'                Dim m_result As New Dictionary(Of String, String)
'                For i = 0 To TB.Rows.Count - 1
'                    m_result.Add(TB.Rows(i).Item(0).ToString(), CType(TB.Rows(i).Item(1), Integer))
'                Next
'                Return m_result
'            Else
'                Return Nothing
'            End If
'        Catch ex As Exception
'            Console.WriteLine("Error: " & ex.ToString())
'            Return Nothing
'        End Try
'    End Function

'    '自絶對資料庫名稱取得Mac
'    Private Function GetMacFormDBfile(dbfile As String)
'        Dim fn As String() = dbfile.Split("\")
'        Return fn(fn.Length - 2)
'    End Function

'    'OriginName: Mac裡的檔案名稱, CacheName: 紀錄快取表裡的檔案名稱(比對時間) 20201005 sam
'    Private Function Compare(OriginName As String, CacheName As String)
'        Dim fntype As Integer = "yyyyMMdd".Length
'        Dim a As Integer = CType(OriginName.TrimEnd().Substring(OriginName.IndexOf(".") - fntype, fntype), Integer)
'        Dim b As Integer = CType(CacheName.TrimEnd().Substring(CacheName.IndexOf(".") - fntype, fntype), Integer)
'        Return IIf(a > b, 1, IIf(a < b, -1, 0))
'    End Function

'    '主要處理
'    Private Sub coreHandler(alldta As Dictionary(Of String, List(Of String)))
'        '(2) 交易處理(速度快很多) 20200831 sam
'        '參考: http://zetcode.com/db/sqlitevb/trans/
'        Try
'            If alldta.Count > 0 Then
'                For Each i As KeyValuePair(Of String, List(Of String)) In alldta
'                    Dim conn As SQLiteConnection = New SQLiteConnection("Data Source=" & i.Key & ";Pooling=true;FailIfMissing=false")
'                    Dim cmd As SQLiteCommand = conn.CreateCommand()

'                    conn.Open()
'                    Using tran As SQLiteTransaction = conn.BeginTransaction()
'                        Try
'                            '(1)寫法 1 
'                            'For Each v As String In i.Value
'                            '    cmd.CommandText = v '"INSERT INTO AllData(Time, Millisecond, " & getLabelStr() & ") VALUES(" & v & ");" 'SqlQuery(a)
'                            '    Dim result As Integer = cmd.ExecuteNonQuery() '若是寫入或更新的話, 即會回傳寫入更新的筆數.
'                            'Next

'                            '(2)寫法2 [有rollback的情況發生]
'                            Dim test As StringBuilder = New StringBuilder()
'                            For Each v As String In i.Value
'                                test.Append(v)
'                            Next
'                            cmd.CommandText = test.ToString() '"INSERT INTO AllData(Time, Millisecond, " & getLabelStr() & ") VALUES(" & v & ");" 'SqlQuery(a)
'                            Dim result As Integer = cmd.ExecuteNonQuery() '若是寫入或更新的話, 即會回傳寫入更新的筆數.

'                            tran.Commit()
'                        Catch ex As Exception
'                            tran.Rollback() '執行失敗, 則RollBack.
'                        End Try
'                    End Using

'                    conn.Close() '關閉
'                    '交易處理(速度快很多) 20200831 sam
'                Next
'            End If
'        Catch ex As Exception
'            Console.WriteLine("coreHandler: " & ex.Message)
'            Console.WriteLine("coreHandler: " & ex.StackTrace)
'        End Try
'    End Sub

'    '原先的備份方法(速度較慢) '暫棄用
'    Public Sub ProcessCopy(a As DirectoryInfo, cretNewDirect As String)
'        Dim sqlite_query As List(Of String) = New List(Of String)()
'        Dim test2 As List(Of DirectoryInfo) = GetAccessDirectory(a.FullName) '(這裡已是下一層目錄(裝置名稱)的處理.)

'        If test2 IsNot Nothing And test2.Count > 0 Then
'            Console.WriteLine("")
'        Else
'            Dim tableFile As List(Of FileInfo) = GetAccessTB(a, "*.mdb") '抓取副檔名為.mdb的檔案
'            If tableFile.Count > 0 Then
'                For Each b As FileInfo In tableFile
'                    '整理其資料庫檔案的檔名.
'                    Dim tmptbname As String = ""
'                    tmptbname = Mid(b.Name, 1, 6) '取得到月份的檔案名稱.
'                    Dim db3file As String = cretNewDirect & "\" & tmptbname
'                    Console.WriteLine("*************** " & db3file & ".db3" & " **************")
'                    If (Not File.Exists(db3file & ".db3")) Then
'                        If (GenernalCreateTableSql(db3file)) Then '若建立SQLITE資料庫檔成功
'                            Console.WriteLine("成功建立資料表: " & db3file)
'                            '以下為Access的操作.
'                            Dim ACCESSER As AccessHandler = New AccessHandler(b.FullName) '進入到access的處理.
'                            Dim tb As DataTable = ACCESSER.GetData("SELECT * FROM AllData ORDER BY Time DESC;")

'                            '測試
'                            Console.WriteLine("(1) access_count: " & tb.Rows.Count)
'                            For v As Integer = 0 To tb.Rows.Count - 1
'                                sqlite_query.Add("INSERT INTO AllData(Time, Millisecond, " & getLabelStr() & ") VALUES(" & combineRowValue(tb.Rows(v)) & ");")
'                            Next
'                            SqliteHandler.insertData(db3file & ".db3", sqlite_query)
'                        Else
'                            'DebugList.Add("建立資料表失敗: " & db3file)
'                            Form1.LogBox.AppendText("建立資料表失敗: " & db3file)
'                        End If
'                    Else
'                        Console.WriteLine("處理資料表: " & db3file)
'                        '以下為Access的操作.
'                        Dim ACCESSER As AccessHandler = New AccessHandler(b.FullName) '進入到access的處理.
'                        Dim tb As DataTable = ACCESSER.GetData("SELECT * FROM AllData ORDER BY Time DESC;")

'                        '測試
'                        For v As Integer = 0 To tb.Rows.Count - 1
'                            'sqlite_query.Add("INSERT INTO AllData(Time, Millisecond, Label0, Label1, Label2, Label3, Label4, Label5, Label6, Label7, Label8, Label9, Label10, Label11, Label2) VALUES(""" & tb.Rows(v).Item(0) & """,""" & CType(tb.Rows(v).Item(1), Integer) & """,""" & tb.Rows(v).Item(2) & """,""" & tb.Rows(v).Item(3) & """,""" & tb.Rows(v).Item(4) & """,""" & tb.Rows(v).Item(5) & """,""" & tb.Rows(v).Item(6) & """,""" & tb.Rows(v).Item(7) & """,""" & tb.Rows(v).Item(8) & """,""" & tb.Rows(v).Item(9) & """,""" & tb.Rows(v).Item(10) & """,""" & tb.Rows(v).Item(11) & """,""" & tb.Rows(v).Item(12) & """,""" & tb.Rows(v).Item(13) & """,""" & tb.Rows(v).Item(14) & """);")
'                            sqlite_query.Add("INSERT INTO AllData(Time, Millisecond, " & getLabelStr() & ") VALUES(" & combineRowValue(tb.Rows(v)) & ");")
'                        Next
'                        SqliteHandler.insertData(db3file & ".db3", sqlite_query)
'                    End If
'                    sqlite_query.Clear()
'                Next
'                'DebugList.Add(" *********************** END *********************** ")
'                Form1.LogBox.AppendText(" *********************** END *********************** " & Environment.NewLine)
'            Else
'                'DebugList.Add(a.FullName & " 找不到相關.db3檔.")
'                Form1.LogBox.AppendText(a.FullName & " 找不到相關.db3檔." & Environment.NewLine)
'            End If
'        End If

'        'DebugList.Add("現在執行緒: " & Thread.CurrentThread.Name)
'        Form1.LogBox.AppendText("現在執行緒: " & Thread.CurrentThread.Name & Environment.NewLine)
'        delThread(Thread.CurrentThread.Name) '刪除紀錄中的執行緒

'        '以下這段改由MyTimer去判斷
'        If CheckEnd() Then '若為True, 代表所有執行isAlive = false
'            '刪除_tempRecordTrans檔案
'            remove_tempRecordTransFile(DataType.His)
'            'DebugList.Add("移植作業完畢.")
'            Form1.LogBox.AppendText("移植作業完畢." & Environment.NewLine)
'            Thread.Sleep(10000) '等侯10秒
'            Environment.Exit(Environment.ExitCode) '強制終止程序
'        End If
'    End Sub

'    '****--------------------------------------------------------------------------------------------****
'    '(1) 紀錄處理中的Access檔案(暫存檔),檔案如下: 20201005 sam
'    Public _tempRecordTransFile As String = Application.StartupPath & "\_tempRecordTransFile.tmp" '歷史即時值
'    Public _tempAlarmTransFile As String = Application.StartupPath & "\_tempAlarmTransFile.tmp" '警報
'    '(2) 紀錄運行中的紀錄集合處理
'    Public _tempTransSet As New Dictionary(Of String, String) '歷史即時值
'    Public _tempAlarmSet As New Dictionary(Of String, String) '警報

'    Public Sub testWrite(mac As String, type As TransAction)
'        RecordHandler(mac, "C:\Users\JNC\Desktop\皆展\I6_Web_Central_integration\I6 Web中央集成\I6_Web中央集成\bin\Debug\His\" & mac & "\20201007.mdb", type, DataType.His)
'    End Sub

'    '(3) 塞入(His)紀錄
'    Public Sub RecordHandler(mac As String, dbfile As String, actiontype As TransAction, DbType As DataType)
'        remove_tempRecordTransFile(DataType.His) '刪除檔案, 再新建一個
'        Select Case actiontype
'            Case TransAction.Add
'                If _tempTransSet.ContainsKey(mac) Then
'                    Select Case DbType
'                        Case DataType.His
'                            _tempTransSet(mac) = dbfile
'                        Case DataType.Alarm
'                            _tempAlarmSet(mac) = dbfile
'                    End Select
'                Else
'                    Select Case DbType
'                        Case DataType.His
'                            _tempTransSet.Add(mac, dbfile)
'                        Case DataType.Alarm
'                            _tempAlarmSet.Add(mac, dbfile)
'                    End Select
'                End If
'            Case TransAction.Remove
'                If _tempTransSet.ContainsKey(mac) Then
'                    _tempTransSet.Remove(mac)
'                End If
'        End Select
'        recordNowAccessFile(DbType)
'    End Sub

'    '(3) 塞入(Alarm)紀錄
'    'Public Sub RecordHandler(dbfile As String, actiontype As TransAction)
'    '    remove_tempRecordTransFile(DataType.Alarm) '刪除檔案, 再新建一個
'    '    Select Case actiontype
'    '        Case TransAction.Add
'    '            If Not _tempAlarmSet.Contains(dbfile) Then
'    '                _tempAlarmSet.Add(dbfile)
'    '            End If
'    '        Case TransAction.Remove
'    '            If _tempAlarmSet.Contains(dbfile) Then
'    '                _tempAlarmSet.Remove(dbfile)
'    '            End If
'    '    End Select
'    '    recordNowAccessFile(DataType.Alarm)
'    'End Sub

'    '(4) 取得_tempTransSet集合文字
'    Private Function get_tempTransSet_str(type As DataType)
'        Dim thetempstr As String = ""
'        Select Case type
'            Case DataType.His
'                If _tempTransSet.Count > 0 Then
'                    For Each i As KeyValuePair(Of String, String) In _tempTransSet
'                        thetempstr &= i.Key & "-" & i.Value & vbCrLf
'                    Next
'                End If
'            Case DataType.Alarm
'                If _tempAlarmSet.Count > 0 Then
'                    For Each i As KeyValuePair(Of String, String) In _tempAlarmSet
'                        thetempstr &= i.Key & "-" & i.Value & vbCrLf
'                    Next
'                    'For Each i As String In _tempAlarmSet
'                    '    thetempstr &= i & vbCrLf
'                    'Next
'                End If
'        End Select
'        Return thetempstr
'    End Function
'    '(5) 紀錄處理中的Access檔案(暫存檔),處理函數如下: 20201005 sam
'    Private _SyncLock As Object = {}
'    Public Sub recordNowAccessFile(type As DataType) '參數1 為access資料庫的絶對路徑.
'        '(1) 若該檔案不存在, 則建立
'        Dim fs As FileStream
'        Try
'            SyncLock _SyncLock
'                If Not File.Exists(IIf(type = DataType.His, _tempRecordTransFile, _tempAlarmTransFile)) Then
'                    fs = File.Create(IIf(type = DataType.His, _tempRecordTransFile, _tempAlarmTransFile))
'                Else
'                    fs = New FileStream(IIf(type = DataType.His, _tempRecordTransFile, _tempAlarmTransFile), FileMode.Open)
'                End If
'                Dim info As Byte() = New UTF8Encoding(True).GetBytes(get_tempTransSet_str(type))
'                fs.Write(info, 0, info.Length)
'                fs.Close()
'            End SyncLock
'        Catch ex As Exception
'            'DebugList.Add("移植備份 紀錄功能發生例外錯誤: " & ex.Message)
'            Form1.LogBox.AppendText("移植備份 紀錄功能發生例外錯誤: " & ex.Message & Environment.NewLine)
'        Finally
'            fs.Dispose()
'        End Try
'    End Sub

'    '(6) 讀取_tempRecordTransFile 20201005 sam
'    Public Sub ReadTemp(type As DataType) '參數為處理的執行緒數量.
'        If File.Exists(IIf(type = DataType.His, _tempRecordTransFile, _tempAlarmTransFile)) Then
'            Dim result As Dictionary(Of String, String) = New Dictionary(Of String, String) '將文件的紀錄, 讀取到該集合裡.
'            SyncLock _SyncLock
'                Using fs As FileStream = New FileStream(IIf(type = DataType.His, _tempRecordTransFile, _tempAlarmTransFile), FileMode.Open, FileAccess.Read)
'                    Dim bytes As Byte() = {} 'fs.Length取得文件的位元組長度
'                    ReDim bytes(fs.Length)
'                    Dim start As Integer = 0
'                    Dim NumberByte As Integer = CType(fs.Length, Integer)
'                    While NumberByte > 0
'                        Dim n As Integer = fs.Read(bytes, start, NumberByte) '將值 讀取位元陣列
'                        If n = 0 Then Exit While
'                        start += n
'                        NumberByte -= n
'                    End While

'                    '已轉換為文字部分.
'                    Dim be As String() = New UTF8Encoding().GetString(bytes).Split(vbCrLf)
'                    If be.Count > 0 Then
'                        For Each i As String In be
'                            If i.Trim() = vbNullChar Then Exit For
'                            Dim keyvalue As String() = i.Split("-")
'                            result.Add(keyvalue(0).Trim(), keyvalue(1).Trim())
'                        Next
'                        If type = DataType.His Then
'                            _tempTransSet = result
'                        Else
'                            _tempAlarmSet = result
'                        End If
'                    End If
'                End Using
'            End SyncLock
'        Else
'            '第一次進行備份處理, 才會進入到這裡.
'            Select Case type
'                Case DataType.His
'                    Dim HisInitDta As Dictionary(Of String, String) = getMacInit(type)
'                    If HisInitDta.Count > 0 Then
'                        For Each i As KeyValuePair(Of String, String) In HisInitDta
'                            RecordHandler(i.Key, Application.StartupPath & "\His\" & i.Key & "\" & i.Value & ".mdb", TransAction.Add, type)
'                        Next
'                    End If
'                Case DataType.Alarm
'                    Dim AlarmInitDta As Dictionary(Of String, String) = getMacInit(type)
'                    If AlarmInitDta.Count > 0 Then
'                        For Each i As KeyValuePair(Of String, String) In AlarmInitDta
'                            'RecordHandler(Application.StartupPath & "\Alr\" & i.Value & ".mdb", TransAction.Add)
'                            RecordHandler(i.Key, Application.StartupPath & "\Alr\" & i.Value & ".mdb", TransAction.Add, type)
'                        Next
'                    End If
'            End Select
'            ReadTemp(type) '寫入完後, 再回讀
'        End If
'    End Sub

'    '第一次初始化時, 建立快取 20201006 sam
'    Public Function getMacInit(type As DataType)
'        Try
'            Dim result As New Dictionary(Of String, String) 'mac, 最早的時間點檔案
'            Dim standard As Integer = 21001231
'            Dim sample As String = ""

'            Select Case type
'                Case DataType.His 'FileName: _tempRecordTransFile.tmp
'                    Dim HisDirect As DirectoryInfo = New DirectoryInfo(Environment.CurrentDirectory & "\His")
'                    Dim files As FileInfo()
'                    Dim target As DirectoryInfo() = HisDirect.GetDirectories()
'                    '在抓取暨寫入的同時再進行判斷.
'                    For Each i As DirectoryInfo In target
'                        files = i.GetFiles()
'                        If files.Length > 1 Then
'                            For Each j As FileInfo In files
'                                sample = j.Name.Split(".")(0)
'                                If j.Extension = ".mdb" And IsNumeric(sample) Then
'                                    standard = IIf(CType(sample, Integer) > standard, standard, CType(sample, Integer))
'                                End If
'                            Next
'                            result.Add(i.Name, standard.ToString())
'                        End If
'                    Next
'                    Return result
'                Case DataType.Alarm 'FileName: _tempAlarmTransFile.tmp
'                    Dim AlarmDirect As DirectoryInfo = New DirectoryInfo(Environment.CurrentDirectory & "\Alr")
'                    If AlarmDirect.GetFiles.Length > 1 Then
'                        For Each j As FileInfo In AlarmDirect.GetFiles()
'                            sample = j.Name.Split(".")(0)
'                            If j.Extension = ".mdb" And IsNumeric(sample) Then
'                                '(1) 僅取其取年月日
'                                'standard = IIf(CType(sample, Integer) > standard, standard, CType(sample, Integer))

'                                '(2) 改依年月, 取其當月最早年月日
'                                'Console.WriteLine("{0}, {1}", standard.ToString().Substring(0, 6), sample.ToString().Substring(0, 6))
'                                If Not result.ContainsKey(sample.ToString().Substring(0, 6)) Then
'                                    result.Add(sample.ToString().Substring(0, 6), sample)
'                                    standard = CType(sample, Integer)
'                                Else
'                                    standard = CType(result(sample.ToString().Substring(0, 6)), Integer)
'                                    result(sample.ToString().Substring(0, 6)) = IIf(CType(sample, Integer) > standard, standard, sample)
'                                End If
'                            End If
'                        Next
'                    End If
'                    Return result
'            End Select
'        Catch ex As Exception
'            Console.WriteLine(ex.ToString())
'        End Try
'    End Function

'    '(7) 刪除紀錄處理中的Access檔案(暫存檔),處理函數如下: 20201005 sam
'    Public Sub remove_tempRecordTransFile(type As DataType)
'        Try
'            Select Case type
'                Case DataType.His
'                    File.Delete(_tempRecordTransFile)
'                Case DataType.Alarm
'                    File.Delete(_tempAlarmTransFile)
'            End Select
'        Catch ex As Exception
'            'DebugList.Add("刪除移植紀錄發生錯誤: " & ex.Message)
'            Form1.LogBox.AppendText("刪除移植紀錄發生錯誤: " & ex.Message & Environment.NewLine)
'        End Try
'    End Sub
'    '(8) 針對紀錄快取的處理動作.
'    Public Enum TransAction
'        Remove = 0
'        Add = 1
'    End Enum

'    '(9) 要移置的資料種類
'    Public Enum DataType
'        His = 0 '設備的歷史即時值
'        Alarm = 1 '警報的歷史資料
'    End Enum
'    '****--------------------------------------------------------------------------------------------****

'    Public Enum MathType
'        quotient = 0 '取其商數
'        remainder = 1 '取其餘數
'    End Enum

'    '刪除執行緒.
'    Public Sub delThread(threadname As String)
'        Dim target As Integer = -1
'        For a = 0 To ThreadWork.Count - 1
'            If ThreadWork(a).Name = threadname Then
'                target = a
'                Exit For
'            End If
'        Next
'        If target <> -1 Then
'            ThreadWork.RemoveAt(target)
'        End If
'    End Sub

'    '構建資料庫檔案及初始資料表sql
'    Public Function GenernalCreateTableSql(tbfile As String, Optional fieldCut As Integer = 120, Optional tbname As String = "AllData") As Boolean
'        Dim QUERY As String = "CREATE TABLE " & tbname & " (ID Integer PRIMARY KEY, Time DateTime, Millisecond VARCHAR(10)"
'        Dim querylist As List(Of String) = New List(Of String)()
'        Try
'            If fieldCut > 0 Then
'                For i As Integer = 0 To fieldCut - 1
'                    QUERY &= ", Label" & i.ToString() & " VARCHAR(10)"
'                Next
'                QUERY &= ");"
'                querylist.Add(QUERY)
'                SqliteHandler.initTable(tbfile, querylist)
'                Return True
'            Else
'                Return False
'            End If
'        Catch ex As Exception
'            Console.WriteLine("Exception_GenernalCreateTableSql: " & ex.ToString())
'            Return False
'        End Try
'    End Function

'    '構建資料庫檔案及初始資料表sql
'    Public Function GenernalCreateTableAlarmSql(tbfile As String, Optional tbname As String = "Acked") As Boolean
'        Dim Acked_QUERY As String = "CREATE TABLE Acked (No Integer PRIMARY KEY, StartTime DateTime, EndTime DateTime, AckTime DateTime, Value VARCHAR(10), Label VARCHAR(30), TagName VARCHAR(24), Message VARCHAR(30), Type VARCHAR(10), Note VARCHAR(24), Personnel VARCHAR(24));"
'        Dim Alarm_QUERY As String = "CREATE TABLE Alarm (No Integer PRIMARY KEY, StartTime DateTime, EndTime DateTime, AckTime DateTime, Value VARCHAR(10), Label VARCHAR(30), TagName VARCHAR(24), Message VARCHAR(30), Type VARCHAR(10), Note VARCHAR(24), Personnel VARCHAR(24));"
'        Dim History_QUERY As String = "CREATE TABLE History (No Integer PRIMARY KEY, StartTime DateTime, EndTime DateTime, AckTime DateTime, Value VARCHAR(10), Label VARCHAR(30), TagName VARCHAR(24), Message VARCHAR(30), Type VARCHAR(10), Note VARCHAR(24), Personnel VARCHAR(24));"
'        Dim querylist As List(Of String) = New List(Of String)()
'        querylist.Add(Acked_QUERY)
'        querylist.Add(Alarm_QUERY)
'        querylist.Add(History_QUERY)
'        Try
'            SqliteHandler.initTable(tbfile, querylist)
'            Return True
'        Catch ex As Exception
'            Console.WriteLine("Exception_GenernalCreateTableSql: " & ex.ToString())
'            Return False
'        End Try
'    End Function

'    '(A) 組合120個自Access撈取出來的值
'    Public Function combineRowValue(target As DataRow)
'        Dim value As String = ""
'        Dim thetime As DateTime
'        For i = 0 To 122 - 1 '包含Time 及 Millisecond
'            thetime = DateTime.Parse(target.Item(0))
'            Dim newtime As String = thetime.ToString("yyyy-MM-dd HH:mm:ss")
'            value &= IIf(value = "", """" & newtime & """", ", """ & target.Item(i) & """")
'        Next
'        Return value
'    End Function

'    '(B) 組合120個頻道的FileName(Label)
'    Public Function getLabelStr(Optional fieldCount As Integer = 120)
'        Dim fields As String = ""
'        For i = 0 To fieldCount - 1
'            fields &= IIf(fields = "", "Label" & i, ", Label" & i)
'        Next
'        Return fields
'    End Function

'    '(1)取得要處理的Access的子目錄.
'    Public Function GetAccessDirectory(path As String) As List(Of DirectoryInfo) '參數為路徑位置
'        '抓取參數路徑下的子目錄列表.
'        Try
'            If Directory.Exists(path) Then
'                Dim result As New List(Of DirectoryInfo)
'                Dim directory As DirectoryInfo = New DirectoryInfo(path)
'                For Each a As DirectoryInfo In directory.GetDirectories()
'                    result.Add(a)
'                Next
'                Return result
'            Else
'                Throw New Exception("Oops!..The directory is not exists!!")
'                Return Nothing
'            End If
'        Catch ex As Exception
'            Console.WriteLine("ex: " & ex.ToString())
'        End Try
'    End Function

'    '(2)取得對應目錄的所有Access檔案.
'    Public Function GetAccessTB(targetDirectory As DirectoryInfo, targetExtension As String) As List(Of FileInfo)
'        Return targetDirectory.GetFiles(targetExtension, SearchOption.TopDirectoryOnly).ToList()
'    End Function

'    '進行移植的必要性[查詢驗證備份的必要性] 回傳百分比
'    Public Function SaveNecessity(tar As List(Of DirectoryInfo), Optional type As String = "bool") '目錄來源
'        Dim denominator As Double = 0.0 '分母,
'        Dim molecular As Double = 0.0 '分子
'        For Each i As DirectoryInfo In tar
'            Dim _context As FileInfo() = i.GetFiles() '取得底下的檔案
'            For j = 0 To _context.Length - 1
'                If _context(j).FullName.IndexOf(".mdb") > 0 Then
'                    '(1) 計算.mdb [Access]的 檔案容量 來計算 (或許較為精確)
'                    Dim nowFilez As Long = FileLen(_context(j).FullName) 'sqln o
'                    denominator += nowFilez
'                    Dim sqlitename As String = i.FullName & "\" & _context(j).Name.Substring(0, 6) & ".db3"
'                    molecular += IIf(File.Exists(sqlitename), nowFilez, 0)

'                    '(2) 計算.mdb [Access]的 檔案數量 來計算 
'                    'denominator += 1
'                    'Dim sqlitename As String = i.FullName & "\" & _context(j).Name.Substring(0, 6) & ".db3"
'                    'molecular += IIf(File.Exists(sqlitename), 1, 0)
'                End If
'            Next
'        Next
'        Dim percent As Double = Math.Round(molecular / denominator, 3) '設定小數點位數.
'        Select Case (type)
'            Case "bool"
'                Return IIf(percent > 0.995, True, False) '若已存在率已達99.5%, 則無須備份.
'            Case "double"
'                Return percent
'        End Select
'    End Function

'    '執行緒 管理
'    Public Class ThreadManager
'        Public Shared LimitCore As Integer = 4 '設定可運行最多的執行緒數量.
'        Public Shared Cores As List(Of MyUnit)

'        '加入執行緒 2020 
'        Public Shared Sub AddThread(thed As MyUnit)

'        End Sub

'        '刪除執行緒 2020
'        Public Shared Sub RemoveThread(name As String)

'        End Sub

'        '查詢現在執行緒的數量
'        Public Shared Function GetCores() As Integer
'            Return Cores.Count()
'        End Function
'    End Class

'    Public Delegate Sub MySubDelegate(ByVal x As Integer) '執行緒的處理Function

'    Public Class MyUnit '重新封裝的執行緒類別.
'        Private th As Thread
'        Private usable As Boolean '是否在運行動作.(用建立事件的作法來處理)
'        Public name As String

'        Public Sub New(ByRef func As MySubDelegate)
'            'th = New ThreadStart(Start)
'        End Sub

'        '僅移植執行緒行
'        Public Sub transplant(mainTh As Thread)
'            th = mainTh
'        End Sub

'        Public Sub Start()
'            th.Start()
'        End Sub

'        Public Function GetUsable() As Boolean

'        End Function
'    End Class

'End Module


'Public Class MyTimer
'    Public Shared WithEvents timer As Windows.Forms.Timer = New Windows.Forms.Timer()
'    Public Shared Sub startcheck(second As Integer)
'        timer.Interval = 3
'        timer.Start()
'    End Sub

'    Private Shared Sub checkworking() Handles timer.Tick
'        If HisTransplant.ThreadWork.Count < 1 Then
'            remove_tempRecordTransFile(DataType.His) '刪除移轉紀錄暫存檔.
'            Form1.LogBox.AppendText("已完成資料庫移轉作業" & Environment.NewLine)
'            System.Threading.Thread.Sleep(10000) '等侯10秒
'            Environment.Exit(Environment.ExitCode) '強制終止程序 
'        Else
'            Form1.LogBox.AppendText("現有 " & ThreadWork.Count & " 支執行緒運行中." & Environment.NewLine)
'            'DebugList.Add("現有 " & ThreadWork.Count & " 支執行緒運行中.")
'        End If
'    End Sub
'End Class



