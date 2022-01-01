Imports System.Data.SQLite
Imports System.IO
Imports System.Threading.Tasks
Public Class SqliteHandler
    Public Shared JppDataBaseFile As String = Application.StartupPath & "\JppDB.db3"
    Public Shared dbfile As String
    Public Shared conn As SQLiteConnection

    Public Shared Function connects(dbf As String)
        Dim dbpath As String = IIf(dbf.IndexOf(Application.StartupPath) >= 0, dbf, Application.StartupPath & "\" & dbf & ".db3")
        Dim mainconn As SQLiteConnection = New SQLiteConnection("Data Source=" & dbpath & ";Pooling=true;FailIfMissing=false")
        If mainconn.State <> ConnectionState.Open Then
            mainconn.Open()
            Console.WriteLine("資料庫連線成功!")
        End If
        Return mainconn
    End Function

    '初始化建立Jpp資料庫(Jpp)
    Public Shared Sub initJppTable(dbf As String)
        'dbfile = dbf & ".db3" '初始化db file名稱.
        Dim dbfile As String = Application.StartupPath & "\" & IIf(dbf.IndexOf(".db3") < 0, dbf & ".db3", dbf) '初始化db file名稱.
        Try
            If Not File.Exists(dbfile) Then
                SQLiteConnection.CreateFile(dbfile)
                createTable(dbf, "CREATE TABLE JppUser (ID Integer PRIMARY KEY, Name VARCHAR(50), Token VARCHAR(40) NOT NULL, Email VARCHAR(100) NOT NULL, Pwd VARCHAR(32), Uuid VARCHAR(50), Authority VARCHAR(15) NOT NULL, Enable VARCHAR(2), Time DateTime, Modify_Time DateTime)")
            Else
                Console.WriteLine("Jpp資料庫已存在.")
            End If
        Catch ex As Exception
            Console.WriteLine("Exception_initJppTable: " & ex.ToString())
        Finally

        End Try
    End Sub

    '初始化建立LineBot資料庫
    Public Shared Sub initTable(dbf As String)
        Dim dbfile As String = Application.StartupPath & "\" & IIf(dbf.IndexOf(".db3") < 0, dbf & ".db3", dbf) '初始化db file名稱.
        Try
            If Not File.Exists(dbfile) Then
                SQLiteConnection.CreateFile(dbfile)
                createTable(dbf, "CREATE TABLE User (ID Integer PRIMARY KEY, Name VARCHAR(50), Lang VARCHAR(16), UserID VARCHAR(50), time DateTime, Token VARCHAR(6), Enable VARCHAR(2), pictureUrl VARCHAR(180), keycode VARCHAR(20))")
            Else
                Console.WriteLine("Agent資料庫已存在.")
            End If
        Catch ex As Exception
            Console.WriteLine("Exception_initTable: " & ex.Message)
            Console.WriteLine("Exception_initTable: " & ex.StackTrace)
        Finally

        End Try
    End Sub

    Public Shared LockJobschedual As Object = {} 'Lock the target to handler it. 20200814 sam 移植
    '初始化建立資料庫 [Access to Sqlite用 for His] 20200814 sam 移植
    Public Shared Sub initTable(dbf As String, createTab As List(Of String))
        Dim dbfile As String = IIf(dbf.IndexOf(".db3") < 0, dbf & ".db3", dbf) '初始化db file名稱.
        Try
            SyncLock LockJobschedual
                If Not File.Exists(dbfile) Then
                    SQLiteConnection.CreateFile(dbfile)
                    For a As Integer = 0 To createTab.Count - 1
                        createTable(dbfile, createTab(a))
                    Next
                Else
                    Console.WriteLine("the database file has be exists..")
                End If
            End SyncLock
        Catch ex As Exception
            Console.WriteLine("Exception_initTable: " & ex.ToString())
        Finally
            Console.WriteLine("完成初始化")
            SqliteHandler.Dispose()
        End Try
    End Sub

    '建立資料庫
    Public Shared Sub createTable(dbpath As String, sqlquery As String)
        Dim mainconn As SQLiteConnection = connects(dbpath)
        Try
            Dim cmd As New SQLiteCommand
            cmd.Connection = mainconn
            '(1)Agent管理員 "CREATE TABLE Administrator (ID Integer PRIMARY KEY,Name VARCHAR(50),password VARCHAR(16),KeyCode VARCHAR(24),Email VARCHAR(32), Time DateTime)"
            '(2)加入LineBot用戶 "CREATE TABLE User (ID Integer PRIMARY KEY,Name VARCHAR(50),Language VARCHAR(16),LineBot_UserID VARCHAR(50),TestTime DateTime,Token VARCHAR(6),KeyCode VARCHAR(24),Enable VARCHAR(1)")"
            '(3)中央集成KeyCode "CREATE TABLE CenterSoft (ID INTEGER PRIMARY KEY, KeyCode VARCHAR(24), IP VARCHAR(32), Time DateTime)"
            '(4)訊息Log "CREATE TABLE Log (ID INTEGER PRIMARY KEY, Time DateTime, UserID VARCHAR(50), Type VARCHAR(16), payload varchar(120))"
            cmd.CommandText = sqlquery
            Dim result As Integer = cmd.ExecuteNonQuery()
            If result = 0 Then
                Console.WriteLine("建立資料表成功")
            Else
                Console.WriteLine("建立資料表失敗")
            End If
        Catch ex As Exception
            Console.WriteLine("Exception_CreateTable: " & ex.Message)
        Finally
            If mainconn.State <> 0 Then
                mainconn.Dispose()
            End If
        End Try
    End Sub

    '寫入/更新資料
    Public Shared Function insertData(dbpath As String, SqlQuery As String) As Integer
        Dim mainconn As SQLiteConnection = connects(dbpath)
        Try
            Dim cmd As New SQLiteCommand
            cmd.Connection = mainconn
            cmd.CommandText = SqlQuery
            Dim result As Integer = cmd.ExecuteNonQuery() '若是寫入或更新的話, 即會回傳寫入更新的筆數.
            If result > 0 Then
                Console.WriteLine("寫入/更新成功")
                Return 1
            Else
                Console.WriteLine("寫入/更新失敗")
                Return 0
            End If
        Catch ex As Exception
            Console.WriteLine("Exception_InsertData: " & ex.Message)
            Return -1
        Finally
            If mainconn.State <> 0 Then
                mainconn.Dispose()
            End If
        End Try
    End Function

    '寫入/更新資料
    Public Shared Function insertData(dbfile As String, SqlQuery As List(Of String)) As Integer
        connect(dbfile)
        Try
            '----------------一次寫入處理寫法---------------
            'startwatch()
            'Console.WriteLine("開始寫入資料(" & SqlQuery.Count & " 筆)中..")
            'Dim cmd As New SQLiteCommand
            'cmd.Connection = conn
            'Dim v As String = ""
            'For a As Integer = 0 To SqlQuery.Count - 1
            '    v &= SqlQuery(a)
            'Next
            'Console.WriteLine("xale-debug: " & v)
            'cmd.CommandText = v 'SqlQuery(a)
            'Dim result As Task(Of Integer) = cmd.ExecuteNonQueryAsync() '若是寫入或更新的話, 即會回傳寫入更新的筆數.

            'Console.WriteLine(IIf(result.Result > 0, "寫入/更新成功", "寫入/更新失敗"))
            'Return IIf(result.Result > 0, 1, 0)


            '----------------分段處理寫法---------------
            Dim cmd As New SQLiteCommand
            cmd.Connection = conn
            Dim v As String = ""
            Dim pointer As Integer = 0
            Dim temp As Integer = 0
            Do
                v = ""
                cmd.CommandText = v
                For a As Integer = pointer To SqlQuery.Count - 1
                    v &= SqlQuery(a)
                    pointer += 1
                    If (pointer - temp) >= 100 OrElse pointer >= SqlQuery.Count Then Exit For
                Next
                cmd.CommandText = v 'SqlQuery(a)
                Dim result As Integer = cmd.ExecuteNonQuery() '若是寫入或更新的話, 即會回傳寫入更新的筆數.
                If result > 0 Then
                    Console.WriteLine("寫入/更新成功")
                    temp = pointer
                    'Return 1
                Else
                    Console.WriteLine("寫入/更新失敗")
                    pointer = temp
                    'Return -1
                End If
            Loop Until pointer >= SqlQuery.Count
            Return 1
            '----------------分段處理寫法---------------------------
        Catch ex As Exception
            Console.WriteLine("Exception_InsertData: " & ex.ToString())
            Return -1
        Finally
            Dispose()
        End Try
    End Function

    '驗證資料是否已存在.
    Public Shared Function ExistsData(dbpath As String, SqlQuery As String) As Integer
        '可用的sqlquery: select count(*) from Administrator where Name = "chenghsun"
        Dim mainconn As SQLiteConnection = connects(dbpath)
        Dim dt As DataTable = Nothing '接收回來的資料表資料.
        Dim ds As New DataSet
        Try
            Dim cmd As New SQLiteCommand
            cmd.Connection = mainconn
            cmd.CommandText = SqlQuery
            '以下將資料嵌入到dt裡.
            Using da As New SQLiteDataAdapter(cmd)
                da.Fill(ds)
                dt = ds.Tables(0)
            End Using
            Console.WriteLine("user數量: " & dt.Rows.Count)
            Return dt.Rows.Count
        Catch ex As Exception
            Console.WriteLine("Exception_ExistsData: " & ex.Message)
            Return -1
        Finally
            If mainconn.State <> 0 Then
                mainconn.Dispose()
            End If
        End Try
    End Function

    '抓取資料
    Public Shared Function selectData(dbpath As String, SqlQuery As String) As DataTable
        Dim mainconn As SQLiteConnection = connects(dbpath)
        Dim cmd As New SQLiteCommand
        Dim dt As DataTable = Nothing '接收回來的資料表資料.
        Dim ds As New DataSet
        Try
            cmd.Connection = mainconn
            cmd.CommandText = SqlQuery
            '以下將資料嵌入到dt裡.
            Using da As New SQLiteDataAdapter(cmd)
                da.Fill(ds)
                dt = ds.Tables(0)
            End Using
            Console.WriteLine("select finished.")
            'Dim result As Integer = cmd.ExecuteNonQuery()
            'Dim sqlResponse As String = cmd.ExecuteScalar()
            'Console.WriteLine("check the data: " & dt.Rows.Count) '抓其數量.
            'Console.WriteLine("check the data: " & dt.Rows(0).Item(0)) '取得內容資料userID.
            'Console.WriteLine("check the data: " & dt.Rows(0).Item(1)) '取得內容資料Name.
            Return dt
        Catch ex As Exception
            Console.WriteLine("Exception_SelectData_sam: " & ex.Message)
            Return dt
        Finally
            cmd.Dispose()
            If mainconn.State <> 0 Then
                mainconn.Dispose()
            End If
        End Try
    End Function

    '一次儲入多筆資料.
    Public Shared Sub InsertDatas(dbpath As String, sqlquerys As List(Of String))
        Dim mainconn As SQLiteConnection = connects(dbpath)
        Try
            For i As Integer = 0 To sqlquerys.Count() - 1
                Dim cmd As New SQLiteCommand
                cmd.Connection = mainconn
                cmd.CommandText = sqlquerys(i)
                Dim result As Integer = cmd.ExecuteNonQuery() '若是寫入或更新的話, 即會回傳寫入更新的筆數.
                If result > 0 Then
                    Console.WriteLine("批次寫入/更新成功")
                Else
                    Console.WriteLine("批次寫入/更新失敗")
                    Exit Sub
                End If
            Next
        Catch ex As Exception
            Console.WriteLine("Exception_InsertDatas: " & ex.Message)
            Console.WriteLine("Exception_InsertDatas: " & ex.StackTrace)
        Finally
            If mainconn.State <> 0 Then
                mainconn.Dispose()
            End If
        End Try
    End Sub

    Public Shared Sub DelData(dbpath As String, sqlquery As String)
        Dim mainconn As SQLiteConnection = connects(dbpath)
        Try
            Dim cmd As New SQLiteCommand
            cmd.Connection = mainconn
            cmd.CommandText = sqlquery
            Dim result As Integer = cmd.ExecuteNonQuery() '若是寫入或更新的話, 即會回傳寫入更新的筆數.
            If result > 0 Then
                Console.WriteLine("刪除成功")
            Else
                Console.WriteLine("刪除失敗")
            End If
        Catch ex As Exception
            Console.WriteLine("Exception_DelData: " & ex.Message)
            Console.WriteLine("Exception_DelData: " & ex.StackTrace)
        Finally
            If mainconn.State <> 0 Then
                mainconn.Dispose()
            End If
        End Try
    End Sub

    '僅關閉連線
    Public Shared Sub Dispose()
        If conn IsNot Nothing Then
            conn.Dispose()
        End If
    End Sub


    '************ 新的處理 20200304 ************
    '執行sqlite query(For Insert/update/delete).
    '連接sqlite資料庫, 自行指定資料庫來源
    Public Shared Sub connect(dbfile As String)
        Dim Originpath As String = Application.StartupPath & "\DB\" & IIf(dbfile.IndexOf(".") < 0, dbfile & ".db3", dbfile)
        If Not File.Exists(Originpath) Then
            Throw New Exception("Error, 找不到" & dbfile & "檔案..")
            Exit Sub
        End If
        If conn IsNot Nothing Then '若連線尚未 建立 實例.
            conn.Dispose()
        End If
        conn = New SQLiteConnection("Data Source=" & Originpath & ";Pooling=true;FailIfMissing=false")

        If conn.State <> ConnectionState.Open Then
            conn.Open()
            Console.WriteLine("資料庫連線成功!")
        End If
    End Sub

    '無須額外處理db3檔案, 用於動態產生His[即時數據], Alr[警報]目錄下的資料庫檔案.
    Public Shared Sub connect2(dbfile As String)
        dbfile = IIf(dbfile.IndexOf(".db3") < 0, dbfile & ".db3", dbfile) '20200814時, 新加入的.

        If Not File.Exists(dbfile) Then
            Throw New Exception("Error, 找不到" & dbfile & "檔案..")
        End If
        If conn IsNot Nothing Then '若連線尚未 建立 實例.
            conn.Dispose()
        End If
        conn = New SQLiteConnection("Data Source=" & dbfile & ";Pooling=true;FailIfMissing=false")

        If conn.State <> ConnectionState.Open Then
            conn.Open()
            Console.WriteLine("資料庫連線成功!")
        End If
    End Sub

    Public Shared Function _ExecSQL(dbfile As String, SqlQuery As String, Optional cFlag As Integer = 0) As Integer
        If cFlag = 0 Then
            connect(dbfile) '資料庫檔案已固定存在執行
        Else
            connect2(dbfile) '資料庫檔案動態存在時執行
        End If
        Try
            Dim cmd As New SQLiteCommand
            cmd.Connection = conn
            cmd.CommandText = SqlQuery
            Dim result As Integer = cmd.ExecuteNonQuery() '若是寫入或更新的話, 即會回傳寫入更新的筆數.
            If result > 0 Then
                Console.WriteLine("執行成功")
                Return 1
            Else
                Console.WriteLine("執行失敗")
                Return 0
            End If
        Catch ex As Exception
            CacheAllData._RecordCache(SqlQuery, dbfile) '進行寫入判斷備份.
            Console.WriteLine("_ExecSQL: " & ex.Message)
            Console.WriteLine("_ExecSQL: " & ex.StackTrace)
            Console.WriteLine(CacheAllData.AllDataTemp.Count)
            Return -1
        Finally
            Dispose()
        End Try
    End Function

    '執行sqlite query(For select).
    Public Shared Function _ExecSQLs(dbfile As String, SqlQuery As String, Optional cFlag As Integer = 0) As DataTable
        If cFlag = 0 Then
            connect(dbfile) '資料庫檔案已固定存在執行
        Else
            connect2(dbfile) '資料庫檔案動態存在時執行
        End If
        Dim cmd As New SQLiteCommand
        Dim dt As DataTable = Nothing
        Dim DS As New DataSet
        Try
            cmd.Connection = conn
            cmd.CommandText = SqlQuery
            Using da As New SQLiteDataAdapter(cmd)
                da.Fill(DS)
                dt = DS.Tables(0)
                Return dt
            End Using
        Catch ex As Exception
            Console.WriteLine("_ExecSQLs: " & ex.Message)
            Console.WriteLine("_ExecSQLs: " & ex.StackTrace)
            Return Nothing
        Finally
            Dispose()
        End Try
    End Function

    '初始化建立資料庫
    Public Shared Sub initTable2(dbf As String)
        '查詢DB目錄是否存在, 不存在即建立.
        If Not Directory.Exists(Application.StartupPath & "\DB") Then
            Directory.CreateDirectory(Application.StartupPath & "\DB")
        End If

        Dim dbfile As String = Application.StartupPath & "\DB\" & IIf(dbf.IndexOf(".") < 0, dbf & ".db3", dbf)
        If Not File.Exists(dbfile) Then
            SQLiteConnection.CreateFile(dbfile) '建立資料庫檔

            Dim allquery As String = ""
            Dim FieldHandler As New Dictionary(Of String, BaseField)

            '(A)新中央集成會員系統
            '(0)_p_idx:父索引, (1)idx:索引, (2)UnitName:企業單位, user:帳號, pass:密碼, OperationPermission:功能權限, ChannelPermission:頻道權限, MapPermission:地圖權限, isStop:啟停用, isAdmin:會長或會員 ,LineBotID:Line用戶ID, WeChatID:微信用戶ID
            FieldHandler.Add("ID", BaseField.GeneralBaseField(DtaType.INT, BaseField.specialtype.PRIMARY))
            '(1)_建立該會員的建立者 token
            FieldHandler.Add("PIdx", BaseField.GeneralBaseField(DtaType.STR, BaseField.specialtype.NORMAL, 10))
            '(3)_類似會員的索引 [重要: 由後端伺服器產生, 亂數產生10位元]
            FieldHandler.Add("Idx", BaseField.GeneralBaseField(DtaType.STR, BaseField.specialtype.NORMAL, 10))
            '(4)_所屬單位名稱
            FieldHandler.Add("UnitName", BaseField.GeneralBaseField(DtaType.STR, BaseField.specialtype.NORMAL, 20))
            '(5)_帳號
            FieldHandler.Add("User", BaseField.GeneralBaseField(DtaType.STR, BaseField.specialtype.NORMAL, 20))
            '(6)_密碼
            FieldHandler.Add("Pass", BaseField.GeneralBaseField(DtaType.STR, BaseField.specialtype.NORMAL, 20))
            '(7)_功能權限
            FieldHandler.Add("OperationPermission", BaseField.GeneralBaseField(DtaType.STR, BaseField.specialtype.NORMAL, 32))
            '(8)_頻道權限:資料DEV001:FFFFFFFF,DEV001:FFFFFFFF(若佔滿約莫1920個字元)
            FieldHandler.Add("ChannelPermission", BaseField.GeneralBaseField(DtaType.STR, BaseField.specialtype.NORMAL, 4096))
            '(9)_地圖權限
            FieldHandler.Add("MapPermission", BaseField.GeneralBaseField(DtaType.STR, BaseField.specialtype.NORMAL, 100))
            '(10)_啟/停用
            FieldHandler.Add("IsStop", BaseField.GeneralBaseField(DtaType.STR, BaseField.specialtype.NORMAL, 1))
            '(11)_為會長/會員
            FieldHandler.Add("IsAdmin", BaseField.GeneralBaseField(DtaType.STR, BaseField.specialtype.NORMAL, 1))
            '(12)_寫入時間點
            FieldHandler.Add("AddTime", BaseField.GeneralBaseField(DtaType.Datetime, BaseField.specialtype.NORMAL))

            '(13)
            FieldHandler.Add("MapShow", BaseField.GeneralBaseField(DtaType.STR, BaseField.specialtype.NORMAL, 10))
            '(14)後來新增
            FieldHandler.Add("MailShow", BaseField.GeneralBaseField(DtaType.STR, BaseField.specialtype.NORMAL, 10))
            '(15)定時上傳啟用
            FieldHandler.Add("Share_Enabled", BaseField.GeneralBaseField(DtaType.STR, BaseField.specialtype.NORMAL, 10))
            '(16)上傳目標網址
            FieldHandler.Add("Share_Url", BaseField.GeneralBaseField(DtaType.STR, BaseField.specialtype.NORMAL, 100))
            '(17)上傳周期
            FieldHandler.Add("Share_UploadCycle", BaseField.GeneralBaseField(DtaType.STR, BaseField.specialtype.NORMAL, 10))
            '(18)上傳間隔
            FieldHandler.Add("Share_UploadDataSpan", BaseField.GeneralBaseField(DtaType.STR, BaseField.specialtype.NORMAL, 10))
            '(19)最後上傳時間
            FieldHandler.Add("Share_LastUpLoadDate", BaseField.GeneralBaseField(DtaType.Datetime, BaseField.specialtype.NORMAL))
            '(20)是否擁有控制權限
            FieldHandler.Add("canControl", BaseField.GeneralBaseField(DtaType.STR, BaseField.specialtype.NORMAL, 10))

            '(21)即時值上傳功能 - 啟停
            FieldHandler.Add("UploadRTValue_Enabled", BaseField.GeneralBaseField(DtaType.STR, BaseField.specialtype.NORMAL, 2))
            '(22)即時值上傳功能 - 上傳間隔(秒)
            FieldHandler.Add("UploadRTValue_Span", BaseField.GeneralBaseField(DtaType.STR, BaseField.specialtype.NORMAL, 10))
            '(23)即時值上傳功能 - 目標網址
            FieldHandler.Add("UploadRTValue_URL", BaseField.GeneralBaseField(DtaType.STR, BaseField.specialtype.NORMAL, 1000))
            '(24)即時值上傳功能 - 使用到的設備及頻道(上傳時，可直接依指定的數據取代上傳內容)
            FieldHandler.Add("UploadRTValue_DeviceChannel", BaseField.GeneralBaseField(DtaType.STR, BaseField.specialtype.NORMAL, 10000))
            '(25)即時值上傳功能 - 上傳內容
            FieldHandler.Add("UploadRTValue_Content", BaseField.GeneralBaseField(DtaType.STR, BaseField.specialtype.NORMAL, 65535))

            allquery &= CreateQuery("Member", FieldHandler)

            FieldHandler.Clear()

            '(B)_LineBot & WeChat UserID
            FieldHandler.Add("ID", BaseField.GeneralBaseField(DtaType.INT, BaseField.specialtype.PRIMARY))
            '(1)_建立該會員的建立者 token
            FieldHandler.Add("Idx", BaseField.GeneralBaseField(DtaType.STR, BaseField.specialtype.NORMAL, 10))
            '(2)_對應LineBot用戶的UserID.
            FieldHandler.Add("UserID", BaseField.GeneralBaseField(DtaType.STR, BaseField.specialtype.NORMAL, 32))
            '(3)_是否接收警報訊息(適用於LineBot, WeChat, Jpp三個平台)
            FieldHandler.Add("AlertNotice", BaseField.GeneralBaseField(DtaType.STR, BaseField.specialtype.NORMAL, 1))
            '(4)_啟停用
            FieldHandler.Add("Enable", BaseField.GeneralBaseField(DtaType.STR, BaseField.specialtype.NORMAL, 1))
            '(5)_Type(L or W)
            FieldHandler.Add("Type", BaseField.GeneralBaseField(DtaType.STR, BaseField.specialtype.NORMAL, 1))
            '(6)_ModifyTime
            FieldHandler.Add("ModifyTime", BaseField.GeneralBaseField(DtaType.Datetime, BaseField.specialtype.NORMAL))
            allquery &= CreateQuery("CommunicationAppUsers", FieldHandler)
            FieldHandler.Clear()

            '(C)_Mail 資料表
            FieldHandler.Add("ID", BaseField.GeneralBaseField(DtaType.INT, BaseField.specialtype.PRIMARY))
            '(1)_會員Idx
            FieldHandler.Add("Idx", BaseField.GeneralBaseField(DtaType.STR, BaseField.specialtype.NORMAL, 10))
            '(2)_對應LineBot用戶的UserID.
            FieldHandler.Add("Mail", BaseField.GeneralBaseField(DtaType.STR, BaseField.specialtype.NORMAL, 32))
            '(3)_加入時間
            FieldHandler.Add("Time", BaseField.GeneralBaseField(DtaType.Datetime, BaseField.specialtype.NORMAL))
            allquery &= CreateQuery("Mail", FieldHandler)
            FieldHandler.Clear()

            '(D)_SMS 資料表
            FieldHandler.Add("ID", BaseField.GeneralBaseField(DtaType.INT, BaseField.specialtype.PRIMARY))
            '(1)_會員Idx
            FieldHandler.Add("Idx", BaseField.GeneralBaseField(DtaType.STR, BaseField.specialtype.NORMAL, 10))
            '(2)_對應LineBot用戶的UserID.
            FieldHandler.Add("SMSnumber", BaseField.GeneralBaseField(DtaType.STR, BaseField.specialtype.NORMAL, 14))
            '(3)_加入時間
            FieldHandler.Add("Time", BaseField.GeneralBaseField(DtaType.Datetime, BaseField.specialtype.NORMAL))
            allquery &= CreateQuery("SMS", FieldHandler)
            FieldHandler.Clear()

            '(E)紀錄Session資料表 (session是由伺服器產生, 發給使用者的)
            FieldHandler.Add("ID", BaseField.GeneralBaseField(DtaType.INT, BaseField.specialtype.PRIMARY))
            FieldHandler.Add("Session", BaseField.GeneralBaseField(DtaType.STR, BaseField.specialtype.NORMAL, 16))
            FieldHandler.Add("Expire", BaseField.GeneralBaseField(DtaType.Datetime, BaseField.specialtype.NORMAL, 20))
            allquery &= CreateQuery("Session", FieldHandler)
            FieldHandler.Clear()
            _ExecSQL(dbf, allquery) '最後全部一次執行.



        Else
            '判斷資料欄位是否正確
            Dim tmpDataTable As DataTable = getTableColumnsName(dbf, "Member")
            Dim tmpRow(tmpDataTable.Rows.Count - 1) As String
            For i = 0 To tmpDataTable.Rows.Count - 1
                tmpRow(i) = tmpDataTable.Rows(i).ItemArray(1)
            Next

            '(21)即時值上傳功能 - 啟停
            Dim RowString(,) As String = {{"UploadRTValue_Enabled", 2},
                                            {"UploadRTValue_Span", 10},
                                            {"UploadRTValue_URL", 1000},
                                            {"UploadRTValue_DeviceChannel", 10000},
                                            {"UploadRTValue_Content", 65535}}
            For i = 0 To RowString.Length / 2 - 1
                If Not tmpRow.Contains(RowString(i, 0)) Then
                    Dim Name As String = RowString(i, 0)
                    Dim size As Integer = RowString(i, 1)
                    AddColumns(dbf, "Member", Name, size)
                End If
            Next
            '判斷資料欄位是否正確
        End If
    End Sub

    '抓取資料表的所有欄位名稱.
    Public Shared Function getTableColumnsName(DBname As String, TableName As String)
        Dim filedta As DataTable = SqliteHandler._ExecSQLs(DBname, "PRAGMA table_info(""" & TableName & """);")
        Return filedta
    End Function

    Public Shared Function AddColumns(DBname As String, TableName As String, ColumnsName As String, ColumnsSize As Integer)
        Return SqliteHandler._ExecSQLs(DBname, "ALTER TABLE " & TableName & " ADD " & ColumnsName & " VARCHAR(" & ColumnsSize & ");")
    End Function

    '檢測現有會員資料表 欄位是否完整, 沒有的話, 即建置[待測試]
    Private Sub checkMemberField(dbfile As String, fields As List(Of String))
        '查詢Member有無sam欄位
        Dim tbfield As DataTable = _ExecSQLs(dbfile, "PRAGMA table_info(""Member"")")
        Dim nowme As New List(Of String)
        If tbfield.Rows.Count > 0 Then
            Dim _contextField As String = ""
            For i = 0 To tbfield.Rows.Count - 1
                _contextField = tbfield.Rows(i).ItemArray(1)
                nowme.Add(_contextField)
            Next
        End If

        Dim thequery As String = ""
        If nowme.Count > 0 Then
            For Each i As String In fields
                If Not nowme.Contains(i) Then
                    thequery &= IIf(thequery <> "", "," & i & " NVARCHAR(10)", i & " NVARCHAR(10)")
                End If
            Next
        End If
        thequery = "ALTER TABLE Member ADD " & thequery & ";"
        Dim result As Integer = _ExecSQL(dbfile, thequery)
        'DebugList.Add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") & IIf(result > 0, "修改Member資料表欄位成功", "修改Member資料表欄位成功"))

    End Sub

    '初始化建立 警報資料庫
    Public Shared Sub initAlrTable(dbf As String) '絶對路徑
        Try
            If Not File.Exists(dbfile) Then
                SQLiteConnection.CreateFile(dbfile)
            End If
            Dim allquery As String = ""
            Dim FieldHandler As New Dictionary(Of String, BaseField)

            '(A)Alert [運行中的警報]
            FieldHandler.Add("ID", BaseField.GeneralBaseField(DtaType.INT, BaseField.specialtype.PRIMARY))
            '(1)_建立該會員的建立者 token
            FieldHandler.Add("StartTime", BaseField.GeneralBaseField(DtaType.Datetime, BaseField.specialtype.INDEX, 20))
            '(3)_類似會員的索引 [重要: 由後端伺服器產生, 亂數產生10位元]
            FieldHandler.Add("EndTime", BaseField.GeneralBaseField(DtaType.Datetime, BaseField.specialtype.NORMAL, 20))
            '(4)_所屬單位名稱
            FieldHandler.Add("AckTime", BaseField.GeneralBaseField(DtaType.Datetime, BaseField.specialtype.NORMAL, 20))
            '(5)_頻道數值
            FieldHandler.Add("Value", BaseField.GeneralBaseField(DtaType.STR, BaseField.specialtype.NORMAL, 8))
            '(6)_標籤
            FieldHandler.Add("Label", BaseField.GeneralBaseField(DtaType.STR, BaseField.specialtype.NORMAL, 20))
            '(7)_標籤名稱
            FieldHandler.Add("TagName", BaseField.GeneralBaseField(DtaType.STR, BaseField.specialtype.NORMAL, 40))
            '(8)_警報訊息
            FieldHandler.Add("Message", BaseField.GeneralBaseField(DtaType.STR, BaseField.specialtype.NORMAL, 40))
            '(9)_警報種類
            FieldHandler.Add("Type", BaseField.GeneralBaseField(DtaType.STR, BaseField.specialtype.NORMAL, 20))
            '(10)_啟/停用
            FieldHandler.Add("Note", BaseField.GeneralBaseField(DtaType.STR, BaseField.specialtype.NORMAL, 20))
            '(11)_為會長/會員
            FieldHandler.Add("Personnel", BaseField.GeneralBaseField(DtaType.STR, BaseField.specialtype.NORMAL, 20))
            allquery &= CreateQuery("Alarm", FieldHandler)
            'allquery &= "CREATE INDEX StartTime ON Alarm;" '建立索引. 
            FieldHandler.Clear()

            '(B)Alert [歷史警報]
            FieldHandler.Add("ID", BaseField.GeneralBaseField(DtaType.INT, BaseField.specialtype.PRIMARY))
            '(1)_建立該會員的建立者 token
            FieldHandler.Add("StartTime", BaseField.GeneralBaseField(DtaType.Datetime, BaseField.specialtype.INDEX, 20))
            '(3)_類似會員的索引 [重要: 由後端伺服器產生, 亂數產生10位元]
            FieldHandler.Add("EndTime", BaseField.GeneralBaseField(DtaType.Datetime, BaseField.specialtype.NORMAL, 20))
            '(4)_所屬單位名稱
            FieldHandler.Add("AckTime", BaseField.GeneralBaseField(DtaType.Datetime, BaseField.specialtype.NORMAL, 20))
            '(5)_頻道數值
            FieldHandler.Add("Value", BaseField.GeneralBaseField(DtaType.STR, BaseField.specialtype.NORMAL, 8))
            '(6)_標籤
            FieldHandler.Add("Label", BaseField.GeneralBaseField(DtaType.STR, BaseField.specialtype.NORMAL, 20))
            '(7)_標籤名稱
            FieldHandler.Add("TagName", BaseField.GeneralBaseField(DtaType.STR, BaseField.specialtype.NORMAL, 40))
            '(8)_警報訊息
            FieldHandler.Add("Message", BaseField.GeneralBaseField(DtaType.STR, BaseField.specialtype.NORMAL, 40))
            '(9)_警報種類?
            FieldHandler.Add("Type", BaseField.GeneralBaseField(DtaType.STR, BaseField.specialtype.NORMAL, 20))
            '(10)_啟/停用
            FieldHandler.Add("Note", BaseField.GeneralBaseField(DtaType.STR, BaseField.specialtype.NORMAL, 20))
            '(11)_為會長/會員
            FieldHandler.Add("Personnel", BaseField.GeneralBaseField(DtaType.STR, BaseField.specialtype.NORMAL, 20))
            allquery &= CreateQuery("History", FieldHandler)
            'allquery &= "CREATE INDEX StartTime ON History;" '建立索引.
            FieldHandler.Clear()

            '(C)Acked [已回報警報]
            FieldHandler.Add("ID", BaseField.GeneralBaseField(DtaType.INT, BaseField.specialtype.PRIMARY))
            '(1)_建立該會員的建立者 token
            FieldHandler.Add("StartTime", BaseField.GeneralBaseField(DtaType.Datetime, BaseField.specialtype.INDEX, 20))
            '(3)_類似會員的索引 [重要: 由後端伺服器產生, 亂數產生10位元]
            FieldHandler.Add("EndTime", BaseField.GeneralBaseField(DtaType.Datetime, BaseField.specialtype.NORMAL, 20))
            '(4)_所屬單位名稱
            FieldHandler.Add("AckTime", BaseField.GeneralBaseField(DtaType.Datetime, BaseField.specialtype.NORMAL, 20))
            '(5)_頻道數值
            FieldHandler.Add("Value", BaseField.GeneralBaseField(DtaType.STR, BaseField.specialtype.NORMAL, 8))
            '(6)_標籤
            FieldHandler.Add("Label", BaseField.GeneralBaseField(DtaType.STR, BaseField.specialtype.NORMAL, 20))
            '(7)_標籤名稱
            FieldHandler.Add("TagName", BaseField.GeneralBaseField(DtaType.STR, BaseField.specialtype.NORMAL, 40))
            '(8)_警報訊息
            FieldHandler.Add("Message", BaseField.GeneralBaseField(DtaType.STR, BaseField.specialtype.NORMAL, 40))
            '(9)_警報種類?
            FieldHandler.Add("Type", BaseField.GeneralBaseField(DtaType.STR, BaseField.specialtype.NORMAL, 20))
            '(10)_啟/停用
            FieldHandler.Add("Note", BaseField.GeneralBaseField(DtaType.STR, BaseField.specialtype.NORMAL, 20))
            '(11)_為會長/會員
            FieldHandler.Add("Personnel", BaseField.GeneralBaseField(DtaType.STR, BaseField.specialtype.NORMAL, 20))
            allquery &= CreateQuery("Acked", FieldHandler)
            'allquery &= "CREATE INDEX StartTime ON Acked;" '建立索引.

            _ExecSQL(dbf, allquery) '最後全部一次執行.
        Catch ex As Exception
            Console.WriteLine("Exception_initAlrTable: " & ex.Message)
            Console.WriteLine("Exception_initAlrTable: " & ex.StackTrace)
        End Try
    End Sub

    '初始化建立 即時訊息資料庫
    Public Shared Sub initHisTable(dbf As String) '絶對路徑
        If Not File.Exists(dbf) Then
            SQLiteConnection.CreateFile(dbf)
        End If

        Dim allquery As String = ""
        Dim FieldHandler As New Dictionary(Of String, BaseField)

        '(A)Alert [運行中的警報]
        FieldHandler.Add("ID", BaseField.GeneralBaseField(DtaType.INT, BaseField.specialtype.PRIMARY))
        '(1)_建立該會員的建立者 token
        FieldHandler.Add("Time", BaseField.GeneralBaseField(DtaType.Datetime, BaseField.specialtype.NORMAL, 20))
        '(3)_類似會員的索引 [重要: 由後端伺服器產生, 亂數產生10位元]
        FieldHandler.Add("Millisecond", BaseField.GeneralBaseField(DtaType.INT, BaseField.specialtype.NORMAL))
        '(4)_Label
        For i As Integer = 0 To 119
            FieldHandler.Add("Label" & i, BaseField.GeneralBaseField(DtaType.STR, BaseField.specialtype.NORMAL, 15))
        Next

        allquery &= CreateQuery("AllData", FieldHandler)
        FieldHandler.Clear()
        _ExecSQL(dbf, allquery, 1) '最後全部一次執行.
    End Sub

    Public createDBcycle As Integer = 1 '代表一個月建立一個資料庫檔.
    'type:R(代表 建立設備即時數值資料庫)/ A(代表 建立警報數值資料庫)
    Public Shared Sub IntervalGeneralTableFile(deviceName As String, type As DBType) '參數為資料庫檔案 名稱
        Try
            Dim NOW As DateTime = DateTime.Now
            Dim timename As String = NOW.Year.ToString() & NOW.Month.ToString().PadLeft(2, "0") '202004
            Dim path As String = Application.StartupPath & IIf(type = DBType.His, "\His\" & deviceName, "\Alr")
            If Not Directory.Exists(path) Then '查詢該目錄是否存在.
                Directory.CreateDirectory(path) '建立該目錄.
                System.Threading.Thread.Sleep(200)
            End If
            '查詢資料庫檔案是否存在.
            Dim dbfile As String = path & "\" & timename & ".db3"
            Console.WriteLine("createtable: " & dbfile)
            If Not File.Exists(dbfile) Then
                SQLiteConnection.CreateFile(dbfile) '建立資料庫檔.
                '建立資料表內容.
                Select Case type
                    Case DBType.His '建立His即時資料庫
                        SqliteHandler.initHisTable(dbfile) '建立His資料表
                    Case DBType.Alr '建立Alr警報資料庫
                        SqliteHandler.initAlrTable(dbfile) '初始建立警報資料庫
                End Select
            End If
        Catch ex As Exception
            Console.WriteLine("Exception_IntervalGeneralTableFile: " & ex.Message)
            Console.WriteLine("Exception_IntervalGeneralTableFile: " & ex.StackTrace)
        End Try
    End Sub

    '建立資料庫檔案(for His)
    Public Shared Sub GeneralTableFile(dbfile As String)
        SQLiteConnection.CreateFile(dbfile) '建立資料庫檔.
        SqliteHandler.initHisTable(dbfile)
    End Sub

    '取得"特定時間範圍"應該處理的資料庫檔名稱列表.
    Public Function getTableName(type As DBType, deviceName As String, date1 As DateTime, Optional date2 As DateTime = Nothing) As List(Of String)
        If type = DBType.His And deviceName = "" Then
            Throw New Exception("ArgumentsError: The DBType.His but deviceName is empty..")
            Exit Function
        End If
        Dim dbfile As New List(Of String)

        Dim starttime As DateTime, endtime As DateTime
        If date2 <> Nothing Then
            Dim result As Integer = DateTime.Compare(date1, date2)
            If (result <= 0) Then 'date1 比較早
                starttime = date1
                endtime = date2
            Else 'date2 比較早
                starttime = date2
                endtime = date1
            End If
            Do
                dbfile.Add(Application.StartupPath & "\DB" & IIf(type = DBType.His, "\His" & deviceName, "\Air") & "\" & starttime.ToString("yyyyMM") & ".db3")
                starttime.AddMonths(1)
            Loop Until (DateTime.Compare(date1, date2)) >= 0
        Else
            dbfile.Add(Application.StartupPath & "\DB" & IIf(type = DBType.His, "\His\" & deviceName, "\Air") & "\" & date1.ToString("yyyyMM") & ".db3")
        End If
        Return dbfile
    End Function

    Public Shared Function TestCreateQueryFunction() 'eg. 建立資料表 範例.
        Dim contain As New Dictionary(Of String, BaseField)
        Dim ID As BaseField = BaseField.GeneralBaseField(DtaType.INT, BaseField.specialtype.PRIMARY)
        Dim Time As BaseField = BaseField.GeneralBaseField(DtaType.Datetime, BaseField.specialtype.NORMAL)
        Dim Cout As BaseField = BaseField.GeneralBaseField(DtaType.INT, BaseField.specialtype.NORMAL)
        Dim Label_1 As BaseField = BaseField.GeneralBaseField(DtaType.STR, BaseField.specialtype.NORMAL, 20)
        Dim Label_2 As BaseField = BaseField.GeneralBaseField(DtaType.STR, BaseField.specialtype.NORMAL, 30)
        Dim Label_3 As BaseField = BaseField.GeneralBaseField(DtaType.STR, BaseField.specialtype.NORMAL, 50)
        Dim Label_4 As BaseField = BaseField.GeneralBaseField(DtaType.STR, BaseField.specialtype.NORMAL, 20)

        contain.Add("ID", ID)
        contain.Add("Time", Time)
        contain.Add("Cout", Cout)
        contain.Add("Label_1", Label_1)
        contain.Add("Label_2", Label_2)
        contain.Add("Label_3", Label_3)
        contain.Add("Label_4", Label_4)
        Dim str As String = CreateQuery("sam", contain)
        Return "建立資料表: " & str
    End Function

    Public Shared Function TestInsertQueryFunction() 'eg. 寫入資料 範例.
        Dim field As New Dictionary(Of String, String)
        field.Add("Time", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))
        field.Add("Cout", "73")
        field.Add("Label_1", "AAAA")
        field.Add("Label_2", "BBBB")
        field.Add("Label_3", "CCCC")
        field.Add("Label_4", "DDDD")
        Dim result As String = InsertQuery("sam", field)
        Return "新建資料: " & result
    End Function

    '用於 寫入即時值 
    Public Shared Function TestInsertValueFunction(DateNTime As DateTime, Value As Single())
        Dim field As New Dictionary(Of String, String)
        field.Add("Time", DateNTime.ToString("yyyy-MM-dd HH:mm:ss"))
        field.Add("Millisecond", DateNTime.Millisecond)
        For i As Integer = 0 To Value.Length - 1
            field.Add("Label" & i, Value(i))
        Next
        Dim result As String = InsertQuery("AllData", field)
        Return result
    End Function

    Public Shared Function TestSelectQueryFunction() 'eg. 抓取資料 範例
        Dim field As New List(Of String)
        field.Add("Time")
        field.Add("Cout")
        field.Add("Label_1")
        field.Add("Label_2")
        field.Add("Label_3")
        field.Add("Label_4")

        '(1)條件式
        Dim conditionobj As New List(Of BaseCondition)
        'conditionobj.Add(New BaseCondition("Label_1", BaseCondition.Judge.EQUAL, "AAAA", DtaType.INT))
        conditionobj.Add(New BaseCondition("Time", BaseCondition.Judge.BETWEEN, New DateTime(2020, 1, 2), New DateTime(2020, 3, 2), DtaType.Datetime))

        '(2)排序
        Dim orderobj As BaseOrderBy = New BaseOrderBy("Time", BaseOrderBy.Arrange.Asc)
        Dim result As String = SelectData("sam", field, conditionobj, orderobj)
        Return "拉取資料: " & result
    End Function

    Public Shared Function TestUpdateQueryFunction() 'eg. 更新資料 範例
        Dim field As New Dictionary(Of String, String)
        field.Add("Time", "2020-03-05")
        field.Add("Cout", "888")
        field.Add("Label_1", "money7")
        field.Add("Label_4", "car1")

        '(1)條件式
        Dim conditionobj As New List(Of BaseCondition)
        conditionobj.Add(New BaseCondition("Label_1", BaseCondition.Judge.EQUAL, 5, Nothing, DtaType.STR))
        conditionobj.Add(New BaseCondition("Label_2", BaseCondition.Judge.EQUALMORE, 8, Nothing, DtaType.INT))
        conditionobj.Add(New BaseCondition("Label_3", BaseCondition.Judge.EQUALLESS, 18, Nothing, DtaType.INT))
        conditionobj.Add(New BaseCondition("Time", BaseCondition.Judge.BETWEEN, New DateTime(2020, 3, 2), New DateTime(2020, 3, 7), DtaType.Datetime))

        Dim result As String = UpdateQuery("sam", field, conditionobj)
        Return "更新資料: " & result
    End Function

    '建立資料表query
    '範例1: "CREATE TABLE Administrator (ID Integer PRIMARY KEY, Name VARCHAR(50),password VARCHAR(16),Email VARCHAR(32), time DateTime)"
    Public Shared Function CreateQuery(tablename As String, Field As Dictionary(Of String, BaseField)) As String
        Try
            Dim sqlquery As String = "CREATE TABLE "
            sqlquery &= tablename
            If Field.Count > 0 Then
                Dim slowfield As String = "("
                For Each i As KeyValuePair(Of String, BaseField) In Field
                    slowfield &= IIf(slowfield = "(", "", ",")
                    slowfield &= i.Key & i.Value.GetStr()
                Next
                slowfield &= ");" '結尾
                sqlquery &= slowfield
                Return sqlquery
            Else
                Return "Field is Empty"
            End If
        Catch ex As Exception
            Console.WriteLine("Exception_CreateQuery: " & ex.Message)
            Console.WriteLine("Exception_CreateQuery: " & ex.StackTrace)
        End Try
    End Function

    '建立資料庫的欄位屬性.
    Public Class BaseField
        Public type As DtaType
        Public stype As specialtype
        Public chars As Integer '字元數
        Public empty As Boolean 'fale: null, true: not null
        Public UNIQUE As Boolean 'false: no, true: yes

        Public Shared Function GeneralBaseField(type As DtaType, Optional stype As specialtype = specialtype.NORMAL, Optional chars As Integer = 10, Optional empty As Boolean = False, Optional UNIQUE As Boolean = False) As BaseField
            If ValicateRationality(type, stype, chars, empty, UNIQUE) Then
                Return New BaseField(type, stype, chars, empty, UNIQUE)
            Else
                Return Nothing '驗證不通過.
            End If
        End Function

        Public Shared Function ValicateRationality(type As DtaType, stype As specialtype, Optional chars As Integer = 0, Optional empty As Boolean = False, Optional UNIQUE As Boolean = False) '驗證各屬性存在的合理性
            Return True
        End Function

        Private Sub New(type As DtaType, stype As specialtype, Optional chars As Integer = 0, Optional empty As Boolean = False, Optional UNIQUE As Boolean = False)
            Me.type = type
            Me.stype = stype
            Me.chars = chars
            Me.empty = empty
            Me.UNIQUE = UNIQUE
        End Sub

        Enum specialtype
            NORMAL = 0
            PRIMARY = 1 'empty不能為空, valuetype必須為int.
            INDEX = 2 'empty不能為空.
        End Enum
        Public Function GetStr() As String
            Dim result As String = ""
            Select Case type
                Case DtaType.NULL '-1
                    result &= " NULL"
                Case DtaType.STR '0
                    result &= " VARCHAR(" & chars.ToString() & ")" & SpecialtypeHandler()
                Case DtaType.TEXT '1
                    result &= " TEXT" & SpecialtypeHandler()
                Case DtaType.INT '2
                    result &= " INTEGER" & SpecialtypeHandler()
                Case DtaType.REAL '3
                    result &= " FLOAT" & SpecialtypeHandler()
                Case DtaType.Datetime '4
                    result &= " DateTime" & SpecialtypeHandler()
            End Select

            'Select Case stype
            '    Case specialtype.NORMAL
            '        result &= ""
            '    Case specialtype.PRIMARY
            '        result &= " PRIMARY NOT NULL"
            '    Case specialtype.INDEX
            '        result &= " INDEX NOT NULL"
            'End Select

            Return result
        End Function

        Public Function SpecialtypeHandler()
            If stype = 0 Then 'Norml
                Return " "
            ElseIf stype = 1 Then 'PRIMARY
                Return " PRIMARY KEY AUTOINCREMENT" '加上Autoincrement關鍵字, SQLITE會自動建立sqlite_seqence資料, reference:https://mrluo.life/article/detail/8/reset-autoid-in-sqlite
            ElseIf stype = 2 Then 'INDEX
                Return " INDEX"
            End If
        End Function
    End Class

    '供外部處理介面 (組織新增資料的動作)
    Public Shared Function InsertQuery(tablename As String, Field As Dictionary(Of String, String))
        Dim sqlquery As String = "INSERT INTO "
        sqlquery &= tablename
        Dim value As New List(Of String)
        If (Field.Count > 0) Then
            Dim keys As String = ""
            Dim values As String = ""
            For Each a As KeyValuePair(Of String, String) In Field
                keys &= IIf(keys = "", "(" & a.Key, "," & a.Key)
                values &= IIf(values = "", "(""" & a.Value & """", ",""" & a.Value & """")
            Next
            sqlquery &= keys & ") VALUES" & values & ");"
            'Dim sqlite_exe As Integer = _ExecSQL(sqlquery) '暫不執行
            Return sqlquery
        Else
            'Return -2 '代表參數有問題, 參數data為空白.
            Return "Field is Empty"
        End If
    End Function

    '供外部處理介面 (組織新增資料的動作)
    Public Shared Function DeleteQuery(tablename As String, Optional ConditionField As Dictionary(Of String, String) = Nothing)
        Dim sqlquery As String = "DELETE FROM "
        sqlquery &= tablename
        Dim value As New List(Of String)
        If ConditionField IsNot Nothing Then
            Dim conditionstr As String = ""
            For Each a As KeyValuePair(Of String, String) In ConditionField
                conditionstr &= IIf(conditionstr = "", a.Key & "=" & a.Value, " AND " & a.Key & "=" & a.Value)
            Next
            sqlquery &= " WHERE " & conditionstr & ";"
            Return sqlquery
        Else
            'Return -2 '代表參數有問題, 參數data為空白.
            Return "Field is Empty"
        End If
    End Function

    '取得設備的Mac, 再針對現在時間去抓
    Public Function getLastTimt(DeviceMac As String, Optional timeStamp As DateTime = Nothing)
        timeStamp = IIf(timeStamp = Nothing, DateTime.Now, timeStamp) '若沒有給時間, 則以當下時間為主
        Dim fileName = DeviceMac & "/" & timeStamp.ToString("yyyyMM")
        Dim SQLquery As String = "SELECT MAX(Time) FROM AllData;"
        Dim result As DataTable = SqliteHandler._ExecSQLs(fileName, SQLquery, 1)
        If result IsNot Nothing And result.Rows.Count > 0 Then
            Return result.Rows(0).ItemArray(0)
        Else
            Return Nothing
        End If
    End Function

    '組update query 字串
    '範例: UPDATE TABLE tbname SET field1 = value1, field2 = value2 WHERE field = value;
    Public Shared Function UpdateQuery(tablename As String, Field As Dictionary(Of String, String), Optional condition As List(Of BaseCondition) = Nothing)
        Try
            Dim sqlquery As String = "UPDATE " & tablename & " SET "
            If (Field.Count > 0) Then
                Dim RowName As String = ""
                For Each a As KeyValuePair(Of String, String) In Field
                    RowName &= IIf(RowName = "", a.Key & " = """ & a.Value & """", "," & a.Key & " = """ & a.Value & """")
                Next
                sqlquery &= RowName '串連更新式

                '若有包含條件式的話, 再組條件式.
                If condition IsNot Nothing Then
                    If condition.Count > 0 Then
                        sqlquery &= " WHERE "
                        Dim conditionname As String = ""
                        For Each i As BaseCondition In condition
                            conditionname &= IIf(conditionname = "", i.GetStr(), " AND " & i.GetStr()) '這裡只要一行.
                        Next
                        sqlquery &= conditionname '串連條件式
                    End If
                End If
                sqlquery &= ";"
                Return sqlquery
            Else
                Return "Field is Empty"
            End If
        Catch ex As Exception
            Console.WriteLine("Excepiton_UpdateQuery: " & ex.Message)
            Console.WriteLine("Excepiton_UpdateQuery: " & ex.StackTrace)
        End Try
    End Function

    '供外部處理介面 (組織新增資料的動作)
    '範例1: select field1, field2 from tablename where condition1 = value1;
    '範例2: select field1, field2 from tablename where condition1 = value1 And condition2 > value2;
    Public Shared Function SelectData(tablename As String, Field As List(Of String), Optional condition As List(Of BaseCondition) = Nothing, Optional orderby As BaseOrderBy = Nothing) '回傳字串
        Try
            Dim sqlquery As String = "SELECT DISTINCT " '預設有加入DINSTINCT, 以確定只取一筆.
            If (Field.Count > 0) Then
                Dim RowName As String = "" '組合要查看的欄位名稱
                For i As Integer = 0 To Field.Count - 1
                    RowName &= IIf(RowName = "", Field(i), "," & Field(i))
                Next
                sqlquery &= RowName & " FROM " & tablename

                '若有包含條件式的話, 再組條件式.
                If condition IsNot Nothing Then
                    If condition.Count > 0 Then
                        sqlquery &= " WHERE "
                        Dim conditionname As String = ""
                        For Each i As BaseCondition In condition
                            conditionname &= IIf(conditionname = "", i.GetStr(), " AND " & i.GetStr())
                        Next
                        sqlquery &= conditionname
                    End If
                End If

                '若有包含排序動作的話, 再組條件式.
                If orderby IsNot Nothing Then
                    sqlquery &= " ORDER BY " & orderby.fieldname & " " & orderby.direction.ToString()
                End If
                sqlquery &= ";"
                Return sqlquery
            Else
                Return "Error" '組織Select string有問題.
            End If
        Catch ex As Exception
            Console.WriteLine("SelectDataException: " & ex.Message)
            Console.WriteLine("SelectDataException: " & ex.StackTrace)
        End Try
    End Function

    'Join的處理 20200417
    '範例1: Select A.field1, B.field1 FROM tbname1 as A JOIN tbname2 AS B ON A.Field = B.Field WHERE A.Field = 1; 
    'JOIN 是作橫向結合 (合併多個資料表的各欄位)；而 UNION 則是作垂直結合 (合併多個資料表中的紀錄)
    Public Function selectjoinQuery(tb As Dictionary(Of MapName, List(Of MapName)), Onobj As List(Of String), Optional Whereobj As Dictionary(Of String, String) = Nothing)
        '須驗證多資料表之間的對應性...
        For Each i As KeyValuePair(Of MapName, List(Of MapName)) In tb
            '
        Next
        '---------------------------------------------------------------------

        Dim result As String = "SELECT "
        Dim nickname As String = "TB" '用於別名

        Dim selectstr As String = "" '組織select部分的字串
        Dim fromstr As String = "" '組織form部分的字串
        Dim onstr As String = "" '組織on部分的字串
        Dim on_index As Integer = 0
        For Each i As KeyValuePair(Of MapName, List(Of MapName)) In tb
            selectstr &= IIf(selectstr = "", i.Key.nickname & "." & """", ", " & i.Key.nickname & "." & """")
            fromstr &= IIf(fromstr = "", i.Key.realname & " AS " & i.Key.nickname, " JOIN " & i.Key.realname & " AS " & i.Key.nickname)
            on_index &= 1
        Next

        If (Whereobj IsNot Nothing) Then

        End If
        selectstr &= " FROM "
    End Function

    '聯集多張資料表 [用組合好的quert字串]
    Public Function UnionQuery_str(querystrs As String()) As String
        Dim query_result As String = ""
        For i As Integer = 0 To querystrs.Length - 1
            query_result &= IIf(query_result = "", "(" & querystrs(i) & ") ", "UNION (" & querystrs(i) & ") ")
        Next
        query_result &= ";"
        Return query_result
    End Function

    '聯集多張資料表 [用集合或物件]:應該不常用
    Public Function UnionQuery_obj(fieldName As List(Of String), tableName As List(Of String), Optional condition As List(Of BaseCondition) = Nothing) As String

    End Function

    Public Structure MapName '處理 實際名稱 別名 對應更換
        Public realname As String '實際欄位名稱
        Public nickname As String '別名
    End Structure

    '進行趨勢圖資料的抓取
    Shared Function RunSQLTrend(FileList As Dictionary(Of String, String), SqlQuery As String, Optional GroupByQuery As String = ";")
        If FileList.Count > 0 Then
            Dim DS As New DataSet
            SqlQuery += GroupByQuery '" GROUP BY substr(strftime('%s',R.Time)*(8640*""" & Percent(i.Key) & """), 1, 8) ORDER BY R.Time ASC;"
            For Each i As KeyValuePair(Of String, String) In FileList
                connect2(i.Value) '資料庫檔案已固定存在執行
                Dim cmd As New SQLiteCommand
                'Dim dt As DataTable = Nothing 'DataTable可以匯整所有的DataTable嗎..
                Try
                    cmd.Connection = conn
                    cmd.CommandText = SqlQuery
                    Using da As New SQLiteDataAdapter(cmd)
                        da.Fill(DS)
                        'dt = DS.Tables(0)
                        'Return dt
                    End Using
                Catch ex As Exception
                    Console.WriteLine("_ExecSQLs: " & ex.ToString)
                    Return Nothing
                Finally
                    Dispose()
                End Try
            Next
            Dim dt As DataTable = DS.Tables(0)
            Return dt
        Else
            Return Nothing
        End If
    End Function

    '基礎的條件處理常式
    Public Class BaseCondition
        Public fieldName As String
        Public Logic As Judge
        Public main_value As Object
        Public secondary_value As Object 'For BETWEEN
        Public type As DtaType

        Public Sub New(fieldName As String, Logic As Judge, main_value As Object, Optional secondary_value As Object = Nothing, Optional type As DtaType = 0)
            Me.fieldName = fieldName
            Me.Logic = Logic
            Me.main_value = main_value
            Me.secondary_value = secondary_value
            Me.type = type
            'If Me.type = DtaType.Datetime Then
            '    Me.main_value = CType(Me.main_value, DateTime)
            '    Me.secondary_value = CType(Me.main_value, DateTime)
            'End If
        End Sub

        Enum Judge
            EQUAL = 0 '等於 =
            MORE = 1 '大於 >
            EQUALMORE = 2 '大於或等於 >=
            LESS = 3 '小於 <
            EQUALLESS = 4 '小於或等於 <=
            BETWEEN = 5 'RANGE(時間)
            IIS = 6 'IS(不確定sqlite是否支援is null之類的寫法)
        End Enum

        '取得其值
        Public Function GetStr()
            Dim result As String = ""
            Select Case Logic
                Case 0
                    result &= fieldName & " = " & GetVal(valueno.any)
                Case 1
                    result &= fieldName & " > " & GetVal(valueno.any)
                Case 2
                    result &= fieldName & " >= " & GetVal(valueno.any)
                Case 3
                    result &= fieldName & " < " & GetVal(valueno.any)
                Case 4
                    result &= fieldName & " <= " & GetVal(valueno.any)
                Case 5
                    result &= fieldName & " BETWEEN " & GetVal(valueno.main) & " AND " & GetVal(valueno.secondary)
                Case 6
                    result &= fieldName & " IS " & GetVal(valueno.any)
            End Select
            Return result
        End Function
        '取得對應的值(取分是string, int, real, datetime)
        Public Function GetVal(NO As valueno) '取分有沒有加分號""而己
            Console.WriteLine(Me)
            If NO = valueno.main Then
                Select Case Me.type
                    Case DtaType.NULL
                        Return ""
                    Case DtaType.STR, DtaType.TEXT
                        Return IIf(main_value IsNot Nothing, """" & main_value & """", "")
                    Case DtaType.INT, DtaType.REAL
                        Return IIf(main_value IsNot Nothing, main_value, "")
                    Case DtaType.Datetime
                        Dim realmain As DateTime = CType(main_value, DateTime)
                        Return IIf(main_value IsNot Nothing, """" & realmain.ToString("yyyy-MM-dd HH:MM:ss") & """", "")
                End Select
            ElseIf NO = valueno.secondary Then
                Select Case Me.type
                    Case DtaType.NULL
                        Return ""
                    Case DtaType.STR, DtaType.TEXT
                        Return IIf(secondary_value IsNot Nothing, """" & secondary_value & """", "")
                    Case DtaType.INT, DtaType.REAL
                        Return IIf(secondary_value IsNot Nothing, secondary_value, "")
                    Case DtaType.Datetime
                        Dim realsecondary As DateTime = CType(secondary_value, DateTime)
                        Return IIf(secondary_value IsNot Nothing, """" & realsecondary.ToString("yyyy-MM-dd HH:mm:ss") & """", "")
                End Select
            ElseIf NO = valueno.any Then
                Select Case Me.type
                    Case DtaType.NULL
                        Return ""
                    Case DtaType.STR, DtaType.TEXT
                        Return IIf(main_value IsNot Nothing, """" & main_value.ToString() & """", IIf(secondary_value IsNot Nothing, """" & secondary_value & """", ""))
                    Case DtaType.INT, DtaType.REAL
                        Return IIf(main_value IsNot Nothing, main_value, IIf(secondary_value IsNot Nothing, secondary_value, ""))
                    Case DtaType.Datetime
                        Return IIf(main_value IsNot Nothing, """" & main_value.ToString("yyyy-MM-dd HH:mm:ss") & """", IIf(secondary_value IsNot Nothing, """" & secondary_value.ToString("yyyy-MM-dd HH:mm:ss") & """", ""))
                End Select
            End If

        End Function

        Enum valueno '決定要取第幾個數值
            main = 0
            secondary = 1
            any = 2
        End Enum
    End Class

    Class BaseOrderBy
        Public fieldname As String
        Public direction As Arrange '排序方向.
        Public Sub New(fieldname As String, direction As Arrange)
            Me.fieldname = fieldname
            Me.direction = direction
        End Sub
        Enum Arrange
            Asc = 0
            Desc = 1
        End Enum
    End Class

    Public Enum DtaType
        NULL = -1
        STR = 0 '字串(預設值)
        TEXT = 1 '長字串
        INT = 2 '數字
        REAL = 3 '浮點數
        Datetime = 4 '時間
    End Enum

    Public Enum DBType
        His = 0
        Alr = 1
    End Enum

    '代換origin文字裡的{0}對應的參數處理.
    Public Sub secureQueryStrHandler(ByRef origin As String, attributes As List(Of String))
        'Dim temp As String = origin
        'Dim left_order As New List(Of Integer) '取{的索引位置
        'Dim right_order As New List(Of Integer) '取}的索引位置
        'Dim left_index As Integer = 0
        'Dim right_index As Integer = 0
        'While (left_index = -1 And right_index = -1) '
        '    right_index = temp.IndexOf("{")
        '    If (right_index > -1) Then
        '        left_order.Add(right_index)
        '    End If
        'End While

        For i As Integer = 0 To attributes.Count - 1
            origin = origin.Replace("{" & i & "}", attributes(i))
        Next
    End Sub

    Public Sub test()
        Dim testline As New List(Of String)
        testline.Add("Sam")
        testline.Add("tina")
        testline.Add("bow")
        testline.Add("mine")
        testline.Add("Fu")
        Dim teststr As String = "this is {0}, {2}, {1}, {3}, {4}"
        'SqliteHandler.secureQueryStrHandler(ByRef ""&"", testline)
        Console.WriteLine("Yoki: " & teststr)
    End Sub

End Class

Public Class GeneralCode '**** 用於產生隨機的亂碼 ****'
    Public Shared BasicChar As Char() = {"0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z"}
    Public Shared Function GeneralBasic(ByVal basic As Integer) As Char
        Static Dim RndNum As New Random()
        Return BasicChar(RndNum.Next(basic))
    End Function

    Public Shared Function GeneralKeyCode(Separate As List(Of Integer), Optional delimiter As Char = "_") As String
        Try
            Dim result As String = ""
            For i As Integer = 0 To Separate.Count - 1
                For j As Integer = 0 To Separate(i) - 1
                    result &= GeneralBasic(BasicChar.Count - 1)
                Next
                If i < Separate.Count - 1 Then result &= delimiter
            Next
            Return result
        Catch ex As Exception
            Console.WriteLine("Exception_GeneralKeyCode: " & ex.Message)
            Console.WriteLine("Exception_GeneralKeyCode: " & ex.StackTrace)
        End Try
    End Function
End Class