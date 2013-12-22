Imports System.Threading

Public Class Form1
    Public type As String
    Private ThreadExtraction As Thread

    Private Sub Button1_Click(sender As System.Object, e As System.EventArgs) Handles Button1.Click
        If (Me.Button1.Text.Equals("Lancer l'extraction")) Then
            ThreadExtraction = New Thread(AddressOf Extraction)
            ThreadExtraction.IsBackground = True
            ThreadExtraction.Start()
        Else
            StopThread()
            ThreadExtraction.Abort()
            ChangeLabel2("L'extraction a été annulée.", Color.Red)
        End If
    End Sub

    Private Sub bloquerFormulaire(value As Boolean)
        Me.CheckBox1.Enabled = Not value
        Me.CheckBox2.Enabled = Not value
        Me.CheckBox3.Enabled = Not value
        Me.CheckBox4.Enabled = Not value
        If (value) Then
            Me.Button1.Text = "Stopper l'extraction"
        Else
            Me.Button1.Text = "Lancer l'extraction"
        End If
    End Sub

    Private Sub KillProcess(ByVal value As String)
        Dim worldserver As Process() = Process.GetProcessesByName(value)
        Dim processus As Process
        For Each processus In worldserver
            processus.Kill()
        Next processus
    End Sub

    Private Sub StopThread()
        ChangeTailleFenetre(341, 155)
        bloquerFormulaire(False)
        CleanUp()
    End Sub

    Private Sub Extraction()
        Dim psi As New ProcessStartInfo()
        Dim processus As Process

        Invoke(New Action(Of Boolean)(AddressOf bloquerFormulaire), True)
        Invoke(New Action(Of String, Color)(AddressOf ChangeLabel2), "", Color.Black)

        If Me.CheckBox1.Checked.Equals(True) And (Not CountFiles("dbc", "*").Equals(GetMaxCount("dbc")) Or Not CountFiles("maps", "*").Equals(GetMaxCount("maps"))) Then ' Extraction des Maps & DBC
            Invoke(New Action(Of Integer, Integer)(AddressOf ChangeTailleFenetre), 341, 218)
            Invoke(New Action(Of String, Byte())(AddressOf ExtractFile), "mapextractor.exe", My.Resources.mapextractor)

            psi.FileName = Application.StartupPath & "\mapextractor.exe"
            If CheckBox4.Checked.Equals(True) Then
                psi.WindowStyle = ProcessWindowStyle.Minimized
            Else
                psi.WindowStyle = ProcessWindowStyle.Hidden
            End If

            processus = Process.Start(psi)

            Dim dbcTermine As Boolean
            dbcTermine = False

            If Not CountFiles("dbc", "*").Equals(GetMaxCount("dbc")) Then
                Invoke(New Action(Of String, Color)(AddressOf ChangeLabel2), "Extraction des DBC en cours...", Color.Black)
                While (dbcTermine.Equals(False) And Not UBound(Diagnostics.Process.GetProcessesByName("mapextractor")) < 0)
                    Invoke(New Action(Of String)(AddressOf UpdateProgressBar), "dbc")
                    If CountFiles("dbc", "*").Equals(GetMaxCount("dbc")) Then
                        dbcTermine = True
                    End If
                    Thread.Sleep(50)
                End While
            End If

            If Not CountFiles("dbc", "*").Equals(GetMaxCount("dbc")) Then
                Invoke(New Action(Of String, Color)(AddressOf ChangeLabel2), "L'extraction des DBC a échoué.", Color.Red)
                Invoke(New Action(AddressOf StopThread))
                Exit Sub
            End If

            If Not CountFiles("maps", "*").Equals(GetMaxCount("maps")) Then
                Invoke(New Action(Of String, Color)(AddressOf ChangeLabel2), "Extraction des Maps en cours...", Color.Black)
                While (Not UBound(Diagnostics.Process.GetProcessesByName("mapextractor")) And Not CountFiles("maps", "*").Equals(GetMaxCount("maps")))
                    Invoke(New Action(Of String)(AddressOf UpdateProgressBar), "maps")
                    Thread.Sleep(50)
                End While
            End If

            If Not CountFiles("maps", "*").Equals(GetMaxCount("maps")) Then
                Invoke(New Action(Of String, Color)(AddressOf ChangeLabel2), "L'extraction des Maps a échoué.", Color.Red)
                Invoke(New Action(AddressOf StopThread))
                Exit Sub
            End If

            KillProcess("mapextractor")
            Invoke(New Action(Of String)(AddressOf RemoveFile), "mapextractor.exe")
        End If

            If Me.CheckBox2.Checked.Equals(True) And Not CountFiles("vmaps", "*").Equals(GetMaxCount("vmaps")) Then ' Extraction des VMaps
                Invoke(New Action(Of Integer, Integer)(AddressOf ChangeTailleFenetre), 341, 218)
                Invoke(New Action(Of String, Byte())(AddressOf ExtractFile), "vmap4extractor.exe", My.Resources.vmap4extractor)
                Invoke(New Action(Of String, Byte())(AddressOf ExtractFile), "vmap4assembler.exe", My.Resources.vmap4assembler)
                Invoke(New Action(Of String, Color)(AddressOf ChangeLabel2), "Extraction des VMaps en cours...", Color.Black)

                If Not CountFiles("Buildings", "*").Equals(GetMaxCount("Buildings")) Then
                    RemoveFile("Buildings\*")
                    psi.FileName = Application.StartupPath & "\vmap4extractor.exe"
                If CheckBox4.Checked.Equals(True) Then
                    psi.WindowStyle = ProcessWindowStyle.Minimized
                Else
                    psi.WindowStyle = ProcessWindowStyle.Hidden
                End If
                    processus = Process.Start(psi)

                    While (Not UBound(Diagnostics.Process.GetProcessesByName("vmap4extractor")))
                        Invoke(New Action(Of String)(AddressOf UpdateProgressBar), "Buildings")
                        Thread.Sleep(50)
                    End While
                End If

                If Not CountFiles("Buildings", "*").Equals(GetMaxCount("Buildings")) Then
                    Invoke(New Action(Of String, Color)(AddressOf ChangeLabel2), "L'extraction des VMaps a échoué.", Color.Red)
                    RemoveFolder("Buildings")
                    Invoke(New Action(AddressOf StopThread))
                    Exit Sub
                End If

                KillProcess("vmap4extractor")
                Invoke(New Action(Of String)(AddressOf RemoveFile), "vmap4extractor.exe")

                Invoke(New Action(Of String, Color)(AddressOf ChangeLabel2), "Génération des VMaps en cours...", Color.Black)
                psi.FileName = Application.StartupPath & "\vmap4assembler.exe"
                psi.Arguments = "Buildings vmaps"
            If CheckBox4.Checked.Equals(True) Then
                psi.WindowStyle = ProcessWindowStyle.Minimized
            Else
                psi.WindowStyle = ProcessWindowStyle.Hidden
            End If
                processus = Process.Start(psi)

                While (Not UBound(Diagnostics.Process.GetProcessesByName("vmap4assembler")) And Not CountFiles("vmaps", "*").Equals(GetMaxCount("vmaps")))
                    Invoke(New Action(Of String)(AddressOf UpdateProgressBar), "vmaps")
                    Thread.Sleep(50)
                End While

                If Not CountFiles("vmaps", "*").Equals(GetMaxCount("vmaps")) Then
                    Invoke(New Action(Of String, Color)(AddressOf ChangeLabel2), "La génération des VMaps a échoué.", Color.Red)
                    Invoke(New Action(AddressOf StopThread))
                    Exit Sub
                End If

                RemoveFolder("Buildings")
                KillProcess("vmap4assembler")
                Invoke(New Action(Of String)(AddressOf RemoveFile), "vmap4assembler.exe")
            End If

            If Me.CheckBox3.Checked.Equals(True) And Not CountFiles("mmaps", "*").Equals(GetMaxCount("mmaps")) Then ' Extraction des MMaps
                Invoke(New Action(Of Integer, Integer)(AddressOf ChangeTailleFenetre), 341, 218)
                Invoke(New Action(Of String, Byte())(AddressOf ExtractFile), "ace.dll", My.Resources.ace)
                Invoke(New Action(Of String, Byte())(AddressOf ExtractFile), "mmaps_generator.exe", My.Resources.mmaps_generator)
                Invoke(New Action(Of String, Color)(AddressOf ChangeLabel2), "Génération des MMaps en cours...", Color.Black)

                psi.FileName = Application.StartupPath & "\mmaps_generator.exe"
            If CheckBox4.Checked.Equals(True) Then
                psi.WindowStyle = ProcessWindowStyle.Minimized
            Else
                psi.WindowStyle = ProcessWindowStyle.Hidden
            End If
                processus = Process.Start(psi)

                While (Not UBound(Diagnostics.Process.GetProcessesByName("mmaps_generator")) And Not CountFiles("mmaps", "*").Equals(GetMaxCount("mmaps")))
                    Invoke(New Action(Of String)(AddressOf UpdateProgressBar), "mmaps")
                    Thread.Sleep(50)
                End While

                If Not CountFiles("mmaps", "*").Equals(GetMaxCount("mmaps")) Then
                    Invoke(New Action(Of String, Color)(AddressOf ChangeLabel2), "La génération des MMaps a échoué.", Color.Red)
                    Invoke(New Action(AddressOf StopThread))
                    Exit Sub
                End If

                KillProcess("mmaps_generator")
                Invoke(New Action(Of String)(AddressOf RemoveFile), "ace.dll")
                Invoke(New Action(Of String)(AddressOf RemoveFile), "mmaps_generator.exe")
            End If

            Invoke(New Action(Of String, Color)(AddressOf ChangeLabel2), "Extraction terminée avec succès.", Color.Green)
            Invoke(New Action(AddressOf StopThread))
    End Sub

    Private Function CountFiles(dossier As String, fichier As String) As Integer
        If System.IO.Directory.Exists(Application.StartupPath & "\" & dossier) = False Then
            System.IO.Directory.CreateDirectory(Application.StartupPath & "\" & dossier)
        End If
        Return My.Computer.FileSystem.GetFiles(Application.StartupPath & "\" & dossier, FileIO.SearchOption.SearchTopLevelOnly, fichier).Count
    End Function

    Private Function GetMaxCount(type As String) As Integer
        Select Case (type)
            Case "dbc"
                Return 247
            Case "maps"
                Return 5744
            Case "Buildings"
                Return 7460
            Case "vmaps"
                Return 9876
            Case "mmaps"
                Return 3657
            Case Else
                Return 1
        End Select
    End Function

    Private Sub UpdateProgressBar(type As String)
        Dim maxCount, count As Integer
        If (type.Equals("dbc") Or type.Equals("maps") Or type.Equals("Buildings") Or type.Equals("vmaps") Or type.Equals("mmaps")) Then
            count = CountFiles(type, "*")
            maxCount = GetMaxCount(type)
        Else
            count = 0
            maxCount = 1
        End If

        ProgressBar1.Maximum = maxCount
        ProgressBar1.Value = count
        Label1.Text = count & " / " & maxCount & " - " & Math.Round(count / maxCount * 100, 2, MidpointRounding.AwayFromZero) & "%"
    End Sub

    Private Sub ExtractFile(name As String, ressource As Byte())
        Try
            My.Computer.FileSystem.WriteAllBytes(Application.StartupPath & "\" & name, ressource, False)
        Catch ex As Exception
        End Try
    End Sub

    Private Sub RemoveFile(file As String)
        Try
            Kill(Application.StartupPath & "\" & file)
        Catch ex As Exception
        End Try
    End Sub

    Private Sub RemoveFolder(folder As String)
        Try
            My.Computer.FileSystem.DeleteDirectory(Application.StartupPath & "\" & folder, FileIO.DeleteDirectoryOption.DeleteAllContents)
        Catch ex As Exception
        End Try
    End Sub

    Private Sub ChangeLabel2(text As String, couleur As Color)
        Label2.ForeColor = couleur
        Label2.Text = text
    End Sub

    Private Sub ChangeTailleFenetre(largeur As Integer, hauteur As Integer)
        Me.Size = New System.Drawing.Size(largeur, hauteur)
    End Sub

    Private Sub CleanUp()
        KillProcess("mapextractor")
        KillProcess("vmap4extractor")
        KillProcess("vmap4assembler")
        KillProcess("mmaps_generator")

        RemoveFile("mapextractor.exe")
        RemoveFile("vmap4extractor.exe")
        RemoveFile("vmap4assembler.exe")
        RemoveFile("mmaps_generator.exe")
        RemoveFile("ace.dll")
        If CountFiles("dbc", "*").Equals(0) Then
            RemoveFolder("dbc")
        End If
        If CountFiles("maps", "*").Equals(0) Then
            RemoveFolder("maps")
        End If
        If CountFiles("Buildings", "*").Equals(0) Then
            RemoveFolder("Buildings")
        End If
        If CountFiles("vmaps", "*").Equals(0) Then
            RemoveFolder("vmaps")
        End If
        If CountFiles("mmaps", "*").Equals(0) Then
            RemoveFolder("mmaps")
        End If
    End Sub

    Private Sub Form1_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        CleanUp()
    End Sub

    Private Sub Form1_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        CleanUp()
    End Sub

    Private Sub CheckBox1_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles CheckBox1.CheckedChanged
        If (Me.CheckBox1.Checked.Equals(True)) Then
            Me.Button1.Enabled = True
        Else
            If (Me.CheckBox2.Checked.Equals(True)) Then
                Me.CheckBox2.Checked = False
            End If
            If (Me.CheckBox3.Checked.Equals(True)) Then
                Me.CheckBox3.Checked = False
            End If
            Me.Button1.Enabled = False
        End If
    End Sub

    Private Sub CheckBox2_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles CheckBox2.CheckedChanged
        If CheckBox2.Checked.Equals(True) Then
            If CheckBox1.Checked.Equals(False) Then
                CheckBox1.Checked = True
            End If
            Me.Button1.Enabled = True
        Else
            If CheckBox1.Checked.Equals(False) Then
                Button1.Enabled = False
            End If
            If CheckBox3.Checked.Equals(True) Then
                CheckBox3.Checked = False
            End If
        End If
    End Sub

    Private Sub CheckBox3_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles CheckBox3.CheckedChanged
        If CheckBox3.Checked.Equals(True) Then
            If CheckBox1.Checked.Equals(False) Then
                CheckBox1.Checked = True
            End If
            If CheckBox2.Checked.Equals(False) Then
                CheckBox2.Checked = True
            End If
            Button1.Enabled = True
        Else
            If CheckBox1.Checked.Equals(False) Then
                Button1.Enabled = False
            End If
        End If
    End Sub
End Class
