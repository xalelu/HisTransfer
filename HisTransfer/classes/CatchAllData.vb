Imports System.Collections.Concurrent
'*** 處理在dbfile be locked的情況, 暫時快取其即時檔
'*** 建立觸發的處理程序.
Module CacheAllData
    Public AllDataTemp As New ConcurrentDictionary(Of String, String)
    Public SyncObj As Object = {}
    Public AllDataTemp_worktime As Integer '處理時間(預設10秒)/每個小時處理1次(一天處理24次).

    '*** (1)紀錄暫無寫入的資料 (20200922) ***
    Public Sub _RecordCache(sqlitequery As String, dbfile As String)
        If sqlitequery.IndexOf("INSERT INTO AllData") >= 0 Then
            If AllDataTemp.ContainsKey(sqlitequery) Then
                AllDataTemp.TryAdd(sqlitequery, dbfile)
            End If
        End If
    End Sub

    '*** (2)回寫未寫入的資料 (20200922) ***
    Public Sub _RewriteCache()
        Try
            If AllDataTemp.Count > 0 Then
                Dim exec_result As Integer = 0
                For Each KV As KeyValuePair(Of String, String) In AllDataTemp
                    If Not _CheckdbLock(KV.Value) Then
                        exec_result = SqliteHandler._ExecSQL(KV.Value, KV.Key)
                    End If
                    If exec_result > 0 Then AllDataTemp.TryRemove(KV.Key, KV.Value)
                    exec_result = 0
                Next
            End If
        Catch ex As Exception
            Console.WriteLine(ex.ToString())
        End Try
    End Sub

    '*** (3)查詢Sqlite資料庫檔案是否Lock, true:鎖住, false:釋放
    Public Function _CheckdbLock(dbfile As String)

    End Function

    '*** (4)刪除Sqlite的所有.db3-Journal檔/中央集成重新啟動時, 執行.
    Public Sub _DeleteJournal()

    End Sub

End Module
